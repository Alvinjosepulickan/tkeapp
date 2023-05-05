 

CREATE Procedure [dbo].[usp_GetFieldDrawingAutomationByGroupId] --118, 'FieldDrawingAutomation'
 @GroupId int
,@OpportunityId nvarchar(25)
,@Action nvarchar(200)
,@ConstantMapperList as [dbo].[VariableMapper] READONLY
AS
Begin
	BEGIN TRY
	   IF(@Action = 'FieldDrawingAutomation')
	   Begin

	                  DECLARE @carPositionTable TABLE
						  (
						  FDAType NVARCHAR(50),
						  FDAValue NVARCHAR(30)
						  )

					   INSERT @carPositionTable
					   EXEC [dbo].[usp_GetCarPositionFromJson] @GroupId
					 

	   /* Get latest Layout GenerationSettings */
	   declare @fielddrawingId int
	   Select @fielddrawingId = Id from (
					 Select * from (Select top 1 fdm.Id, fda.CreatedOn as [date] from FieldDrawingAutomation fda
						  right Join FieldDrawingMaster fdm on fda.FieldDrawingId = fdm.Id where FDAValue=1
									   and fdm.GroupId= @GroupId
											and fdm.QuoteId= @OpportunityId 
										  order by fda.CreatedOn desc)a
							   union all

					 Select * from (Select top 1 fdm.Id, fda.ModifiedOn as [date] from FieldDrawingAutomation fda
							  right Join FieldDrawingMaster fdm on fda.FieldDrawingId = fdm.Id where fda.ModifiedOn <> ''
										and fda.FDAValue=1  and fdm.GroupId= @GroupId
											 and fdm.QuoteId= @OpportunityId 
											order by fda.ModifiedOn desc ) b
				) c order by c.date asc

		 Select FDAType,FDAValue as FDAValue
		   from FieldDrawingAutomation
		   Where FieldDrawingId = @fielddrawingId

			  union all

					Select GroupConfigurationType as FDAType
						  ,GroupConfigurationValue as FDAValue from GroupConfigurationDetails Where GroupId = @GroupId and IsDeleted=0

					  union all

                   Select FDAType,FDAValue from @carPositionTable

		End
		ELSE  IF(@Action = 'FieldDrawingLayoutDetails')
		Begin

		  
	   declare @frontDoorSP nvarchar(200);
	   declare @RearDoorSP nvarchar(200);
	   SET @frontDoorSP = (Select VariableType from @ConstantMapperList where VariableKey ='FRONTDOOR_SP')
	   SET @RearDoorSP = (Select VariableType from @ConstantMapperList where VariableKey ='REARDOOR_SP')

 
			select us.UnitId
				  ,us.Designation as [UnitDesignation]
				  ,us.MappedLocation as [DisplayCarPosition]
				  ,us.Designation as [Name]
				,(case when [dbo].[Fn_GetDoorVariableFromDoors](us.UnitId,@frontDoorSP) is null then 'Center' else [dbo].[Fn_GetDoorVariableFromDoors](us.UnitId,@frontDoorSP)  end) AS Front 
				,Isnull( [dbo].[Fn_GetDoorVariableFromDoors](us.UnitId,@RearDoorSP), '') AS Rear

			from  GroupConfiguration gc  
			 Left Join Units us on us.GroupConfigurationId = gc.GroupId
					Where us.GroupConfigurationId= @GroupId and gc.IsDeleted=0 and us.IsDeleted=0
		End
	END TRY
	BEGIN CATCH
	 EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
End
