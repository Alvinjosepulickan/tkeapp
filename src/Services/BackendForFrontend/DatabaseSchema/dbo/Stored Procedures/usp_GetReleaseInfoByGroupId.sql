--select * from Units where GroupConfigurationId = 196

CREATE Procedure [dbo].[usp_GetReleaseInfoByGroupId] --62--275--9--,195
-- Add the parameters for the stored procedure here
 @groupConfigurationId int 
--,@setId nvarchar(500)
as
Begin
	BEGIN TRY
		Declare @count int
		Declare @fixtureStrategy nvarchar(50)
    
		SET @fixtureStrategy = (SELECT distinct(ControlLocationValue) FROM ControlLocation WHERE GroupConfigurationId = @groupConfigurationId 
		and ControlLocationType = 'Parameters_SP.fixtureStrategy_SP')

		select Distinct GroupId, GroupName from GroupConfiguration where groupId = @groupConfigurationId

		if not exists (select * from ReleaseInfoData where SetId in (select SetId from Units where GroupConfigurationId = @groupConfigurationId))
		Begin
			 Select Distinct uc.ConfigureVariables,uc.ConfigureValues, isnull(ri.isAcknowledge,0) as isAcknowledge ,us.setid   ---taking datapoints from unitConfiguration
			 from Units us 
			 Left Join UnitConfiguration uc on us.SetId = uc.SetId
			 --Left Join UnitSet ust on us.SetId = ust.SetId
			 left join ReleaseInfoData ri on ri.SetId = us.SetId and uc.ConfigureVariables = ri.ConfigureVariables
			 where --uc.IsDeleted=0 and 
			 us.IsDeleted=0 --and ust.IsDeleted=0 
			 and us.GroupConfigurationId = @groupConfigurationId --and us.SetId=@setId
			 order by us.setId
		End
		Else 
		Begin
			 Select Distinct ri.ConfigureVariables,uc.ConfigureValues, isnull(ri.isAcknowledge,0) as isAcknowledge ,us.setid  ------taking datapoints from ReleaseInfoData
			 from Units us 
			 Left Join UnitConfiguration uc on us.SetId = uc.SetId
			 --Left Join UnitSet ust on us.SetId = ust.SetId
			 left join ReleaseInfoData ri on ri.SetId = us.SetId and uc.ConfigureVariables = ri.ConfigureVariables
			 where --uc.IsDeleted=0 and 
			 us.IsDeleted=0 --and ust.IsDeleted=0 
			 and us.GroupConfigurationId = @groupConfigurationId --and us.SetId=@setId
			 order by us.setId
		End
			-- right join
			--(
			--Select Distinct uc.ConfigureVariables as distinctDataPoints, null as ConfigureValues, null as isAcknowledge, null as setid
			--from UnitConfiguration uc
			-- Left Join Units us on us.SetId = uc.SetId
			-- --Left Join UnitSet ust on us.SetId = ust.SetId
			-- where uc.IsDeleted=0 and us.IsDeleted=0 --and ust.IsDeleted=0 
			-- and us.GroupConfigurationId = @groupConfigurationId
			--)y
			--on x.distinctDataPoints = y.distinctDataPoints

       
	   		Select Distinct uc.ConfigureVariables distinctDataPoints
			from UnitConfiguration uc
			 Left Join Units us on us.SetId = uc.SetId
			 --Left Join UnitSet ust on us.SetId = ust.SetId
			 where uc.IsDeleted=0 and us.IsDeleted=0 --and ust.IsDeleted=0 
			 and us.GroupConfigurationId = @groupConfigurationId

			Select Distinct UnitId as unitid ,[Designation] as unitname,UEID, u.SetId ,ri.releaseComments
			 from Units u
			 left join UnitConfiguration uc
			 on u.SetId = uc.SetId
			 Left join ReleaseInfoData ri
			 on u.SetId = ri.SetId
			 Where u.IsDeleted=0 /*and u.SetId=@setId*/ and u.GroupConfigurationId=@groupConfigurationId

			-- Select Distinct 1 as unitid ,unitNameConcatination.unitName as unitname, u.SetId ,ri.releaseComments
			-- from Units u
			-- --left join UnitConfiguration uc
			-- --on u.SetId = uc.SetId
			-- Left join ReleaseInfoData ri
			-- on u.SetId = ri.SetId
			-- Left join (
			-- SELECT DISTINCT y.setid, 
			--SUBSTRING(
			--(
   --         SELECT ','+x.Desi AS [text()]
   --         from (select distinct designation as desi, SetId 
			--from Units u
			-- Where u.IsDeleted=0 
			-- and u.GroupConfigurationId=@groupConfigurationId
			-- )x
			-- where x.SetId = y.SetId
   --         FOR XML PATH ('')
			--), 2, 1000) [unitName]
			--FROM (select setid from units 
			--where GroupConfigurationId=@groupConfigurationId)y
			--) unitNameConcatination
			--on unitNameConcatination.SetId = u.SetId
			-- Where u.IsDeleted=0 /*and u.SetId=@setId*/ and u.GroupConfigurationId=@groupConfigurationId

			 
		--select FrontOpening as FrontOpening, RearOpening as RearOpening from OpeningLocation where GroupConfigurationId=@groupConfigurationId

		if exists (select statuskey from systems where setid in (select SetId from Units where GroupConfigurationId = @groupConfigurationId))
	  begin

		select (case when StatusKey = 'UNIT_VAL' then 1 else 0 end) as relaseToManufacture
		from Status where StatusKey in (select Distinct StatusKey from Systems 
										where SetId in (select SetId from Units where GroupConfigurationId = @groupConfigurationId))

	  end
	  Else
	  Begin
	  select 0 as relaseToManufacture
	  End


	  Declare @TempQueries table
	  (
	  queryId [NVARCHAR](50),
	  queryName [NVARCHAR](50),
	  isAcknowledge bit
	  )
	  Insert into @TempQueries
	  Values ('FLADCOMP'	,'Final Layouts/AS Drawings Complete?' ,0),
			 ('SUPAPRVD'	,'I-Supplier Approved?' 			   ,0),
			 ('CONFOSD'		,'Confirmed OSD (in View)?' 		   ,0),
			 ('WRNGDIAGCOMP','Wiring Diagram Completed?' 		   ,0),
			 ('CONTEXEC'	,'Contract Executed?'				   ,0),
			 ('PPERCVD'		,'PPE Received?'					   ,0)
			 
	  if exists (Select '*' from groupreleasequeries where GroupId = @groupConfigurationId)
	  Begin
		select queryId, queryName, isAcknowledge 
		from groupreleasequeries where GroupId = @groupConfigurationId
		union 
		select queryId, queryName, isAcknowledge from @TempQueries
		where queryId not in (select queryId from groupreleasequeries where GroupId = @groupConfigurationId)
	  End
	  Else 
	  Begin
		Select queryId, queryName, isAcknowledge  from @TempQueries
	  End

	  declare @AllSelected bit = 0

	  If not exists(select top 1 '*' from  groupreleasequeries 
					where GroupId = @groupConfigurationId and isAcknowledge = 0)
	  Begin
		If not exists(select top 1 '*' from Units u Left join ReleaseInfoData ri on u.SetId = ri.SetId 
						where u.GroupConfigurationId = @groupConfigurationId and isnull(ri.releaseComments,'') = '')
		Begin
			if not exists(select top 1 '*' from unitconfiguration 
							where SetId in (select SetId from units where GroupConfigurationId = @groupConfigurationId) and Isnull(isAcknowledge,0) = 0)
			Begin
				set @AllSelected = 1
			End
		End
	  End

	  select @AllSelected as AllSelected

	END TRY
	BEGIN CATCH
	 EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH


END