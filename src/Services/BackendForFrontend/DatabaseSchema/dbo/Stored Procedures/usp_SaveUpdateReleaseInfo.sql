
CREATE PROCEDURE [dbo].[usp_SaveUpdateReleaseInfo]
	-- Add the parameters for the stored procedure here
	@GroupId int,
	@UnitVariables as ReleaseInfoDataPoints READONLY,
	@CreatedBy nvarchar(50),
	@IsEditFlows  nvarchar(30),
	@ActionFlag nvarchar(30),
	@Result int OUTPUT
AS
BEGIN
	BEGIN TRY
		-- Adding unit status flags
		if(@IsEditFlows = 'Valid')
		begin
			set @IsEditFlows ='UNIT_VAL'
			set @Result = 0
		end

		--If the group is ready for release
		declare @ReleaseFlag bit = 0
		If isnull(@ActionFlag,'') like '%release%'
		begin
			set @ReleaseFlag = 1
		end

		If exists (select '*' from groupreleasequeries where GroupId = @GroupId)
		Begin
			Update queries
			set isAcknowledge = t.isAcknowledge
			from groupReleaseQueries queries
			join @UnitVariables t
			on t.unitJsonVariables = queries.queryId
			where queries.GroupId = @GroupId
			and isnull(t.setId,0) = 0

		End
		Else
		Begin
			Insert into groupreleasequeries(GroupId,queryId,queryName,isAcknowledge,createdBy)
			select @GroupId, unitJsonVariables, 
			case when unitJsonVariables = 'FLADCOMP'	 then 'Final Layouts/AS Drawings Complete?' 
				 when unitJsonVariables = 'SUPAPRVD'	 then 'I-Supplier Approved?' 
				 when unitJsonVariables = 'CONFOSD'		 then 'Confirmed OSD (in View)?' 
				 when unitJsonVariables = 'WRNGDIAGCOMP' then 'Wiring Diagram Completed?' 
				 when unitJsonVariables = 'CONTEXEC'	 then 'Contract Executed?' 
				 when unitJsonVariables = 'PPERCVD'	 	 then 'PPE Received?' end,
			isAcknowledge,@CreatedBy
			from @UnitVariables where isnull(setId,0) = 0

		End

		Insert into ReleaseInfoData(SetId, releaseComments, isAcknowledge, ConfigureVariables)
		Select Distinct setId, releaseComments, isAcknowledge, unitJsonVariables
		from @UnitVariables
		where isnull(setId,0) <> 0
		and  unitJsonVariables not in (select distinct ConfigureVariables 
										from ReleaseInfoData 
										where SetId in (select distinct SetId from @UnitVariables where isnull(setId,0) <> 0))

			--update approved values for already saved variables
			update ri
			set ri.isAcknowledge = t.isAcknowledge 
			from @UnitVariables t 
			join ReleaseInfoData ri
			on ri.SetId = t.setId
			and ri.ConfigureVariables = t.unitJsonVariables

			--update approved values for already saved variables
			update ri
			set ri.isAcknowledge = t.isAcknowledge 
			from @UnitVariables t 
			join ReleaseInfoData ri
			on ri.SetId = t.setId
			and ri.ConfigureVariables = t.unitJsonVariables
			where t.isAcknowledge = 1

			--update approved values for the units under selected group
			update ri
			set ri.releaseComments = t.releaseComments
			from @UnitVariables t 
			join Units u
			on t.setId=u.SetId
			join ReleaseInfoData ri
			on ri.SetId = t.setId
			where u.GroupConfigurationId = @GroupId

			update ri
			set ri.releaseComments = t.releaseComments
			from @UnitVariables t 
			join Units u
			on t.setId=u.SetId
			join ReleaseInfoData ri
			on ri.SetId = t.setId
			where u.GroupConfigurationId = @GroupId
			and isnull(t.releaseComments,'') <> ''

			set @Result=@GroupId

		If @IsEditFlows = 'UNIT_VAL'
		Begin
			If  @ReleaseFlag = 1
			Begin
			Update units set WorkflowStatus = 'UNIT_FR' where GroupConfigurationId = @GroupId

			Update groupconfiguration set WorkflowStatus = 'GRP_FR' where GroupId = @GroupId	

			declare @buildingId int
			select @buildingId=BuildingId from GroupConfiguration where GroupId = @GroupId
			exec [dbo].[usp_UpdateWorkflowStatus]@buildingId,'building'
			
			set @Result = -1
			End
		end		

		select @Result

	END TRY
	BEGIN CATCH
		EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';
	END CATCH
END