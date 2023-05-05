
 
CREATE procedure [dbo].[usp_GetVariableValuesForOZ]--'US-2021-00000102'
(
	@quoteId nvarchar(20)
	
)
as
begin

	BEGIN TRY





	select distinct GroupId,BuildingId,us.UnitId,isnull(UEID,'') as UEID,isnull(us.SetId,'')as Setid,isnull(ust.ProductName,'')as ProductName,isnull(us.designation,'') as designation,
		ISNULL((Select QuestionnaireJson from CoordinationQuestions where GroupId = gc.GroupId), '') as questions
		
		--,(Select ConfigureValues from UnitConfiguration
	 -- where ConfigureVariables = 'Parameters.Factory' and SetId=us.SetId) as factory
		
		
		from  Units us 
		Left Join UnitSet ust on us.SetId= ust.SetId
		Left Join GroupConfiguration gc on us.GroupConfigurationId=gc.GroupId
		where  
		 us.GroupConfigurationId in (select GroupId from GroupConfiguration 
		where BuildingId in (select Id from Building where QuoteId=@quoteId and IsDeleted=0)and IsDeleted=0)


		declare @temp table
		(
		id int,
		groupID int,
		quoteId nvarchar(20)
		)
		Insert into @temp
		select id,groupId,quoteId from (select ROW_NUMBER() over(partition by groupId order by createdOn desc) as seq,* from FieldDrawingMaster where QuoteId = @quoteId)x
		where  seq = 1

		--select * from @temp



		select distinct(FDAType) 
		from FieldDrawingAutomation
		join FieldDrawingMaster
		on FieldDrawingMaster.id = FieldDrawingAutomation.FieldDrawingId
		where FieldDrawingAutomation.FieldDrawingId in (select id from @temp) and FDAValue=1

		select SetId as Setid,SystemVariableKeys,SystemVariableValues from SystemsVariables where setid in (select SetId from units us 
			left join GroupConfiguration gc on us.GroupConfigurationId=gc.GroupId 
			left join Building b on gc.BuildingId=b.Id 
		where BuildingId in (select id from Building where QuoteId=@quoteId and b.IsDeleted=0)and gc.IsDeleted=0) 
		select SetId as Setid,isnull(ConfigureVariables,'') as SystemVariableKeys,isnull(ConfigureValues,'') as SystemVariableValues from UnitConfiguration where setid in (select SetId from units us 
			left join GroupConfiguration gc on us.GroupConfigurationId=gc.GroupId 
			left join Building b on gc.BuildingId=b.Id 
		where BuildingId in (select id from Building where QuoteId=@quoteId and b.IsDeleted=0)and gc.IsDeleted=0)

	END TRY
	BEGIN CATCH
	 EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
end