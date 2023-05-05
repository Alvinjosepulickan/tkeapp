
CREATE procedure [dbo].[usp_UpdateWorkflowStatus]--'95','project'
@id nvarchar(30),
@section nvarchar(50)
as
BEGIN
	BEGIN TRY
		DECLARE @setid int,@groupid int,@buildingid int
		DECLARE @projectid nvarchar(10),@quouteid nvarchar(20)
		DECLARE @conflictstatus nvarchar(10),@previousstatus nvarchar(100),@status nvarchar(100),@projectstatus nvarchar(50)
		IF(@section='group')
		BEGIN
			set @groupid=cast(@id as int)
			SELECT @buildingid=BuildingId FROM GroupConfiguration WHERE GroupId=@id
			DECLARE @unitcompleted int,@noofunits int
			SELECT @unitcompleted=count(*) FROM Units WHERE GroupConfigurationId=@groupid and WorkflowStatus='UNIT_VAL' and IsDeleted=0
			SELECT @noofunits=count(*) FROM Units WHERE GroupConfigurationId=@groupid and IsDeleted=0
			SELECT @previousstatus= workflowstatus FROM GroupConfiguration WHERE GroupId=@groupid
			SET @status=@previousstatus
			IF(@previousstatus<>'GRP_CINV')
			begin
				IF(@noofunits>0)
				BEGIN
					IF(@unitcompleted=@noofunits)
					BEGIN
						UPDATE GroupConfiguration set WorkflowStatus='GRP_VAL' WHERE GroupId=@groupid
						set @status='GRP_VAL'
					END
					else
					BEGIN
						IF(@previousstatus='GRP_VAL')
						BEGIN
							UPDATE GroupConfiguration set WorkflowStatus='GRP_COM' WHERE GroupId=@groupid
							set @status='GRP_COM'
						END
					END
				END
				ELSE
				BEGIN
					UPDATE GroupConfiguration set WorkflowStatus='GRP_INC' WHERE GroupId=@groupid
					set @status='GRP_INC'
				END
				
				IF(@previousstatus<>@status)
				BEGIN	
					exec [dbo].[usp_UpdateWorkflowStatus]@buildingid,'building'
				END
			END
			
			
		END
		else IF(@section='building')
		BEGIN
			set @buildingid=cast(@id as int)
			DECLARE @groupcomplted int,@noofgroup int,@projectsource int,@grpReleased int
			SELECT @quouteid = QuoteId FROM Building WHERE id=@buildingid
			SELECT @groupcomplted=count(*) FROM GroupConfiguration WHERE BuildingId=@buildingid and WorkflowStatus='GRP_VAL' and IsDeleted=0
			SELECT @noofgroup=count(*) FROM GroupConfiguration WHERE BuildingId=@buildingid and IsDeleted=0
			SELECT @grpReleased=count(*) FROM GroupConfiguration WHERE BuildingId=@buildingid and WorkflowStatus='GRP_FR' and IsDeleted=0
			SELECT @previousstatus= workflowstatus FROM Building WHERE id=@buildingid
			SET @status=@previousstatus
			
			IF(@previousstatus<>'BLDG_CINV')
			BEGIN
				IF(@noofgroup>0)
				BEGIN
					--All groups are released check
					IF(@grpReleased=@noofgroup)
					BEGIN
						update Building set WorkflowStatus='BLDG_FR' WHERE id=@buildingid
						set @status='BLDG_FR'
					END
					ELSE
					BEGIN
						--all groups are valid check
						IF(@groupcomplted=@noofgroup)
						BEGIN
							update Building set WorkflowStatus='BLDG_VAL' WHERE id=@buildingid
							set @status='BLDG_VAL'
						END
						else
						BEGIN
							IF(@previousstatus='BLDG_VAL' or @previousstatus='BLDG_LOC')
							BEGIN
								UPDATE Building set WorkflowStatus='BLDG_COM' WHERE id=@buildingid
								set @status='BLDG_COM'
							END
						END
					END
				END
				ELSE
				BEGIN
					IF(EXISTS(SELECT * FROM BuildingElevation where buildingid=@buildingid))
					BEGIN
						UPDATE Building set WorkflowStatus='BLDG_COM' WHERE id=@buildingid
						set @status='BLDG_COM'
					END
					ELSE
					BEGIN
						UPDATE Building set WorkflowStatus='BLDG_INC' WHERE id=@buildingid
						set @status='BLDG_INC'
					END
				END
				--SELECT @projectsource=ProjectSource FROM Projects 
				--WHERE OpportunityId in (SELECT q.OpportunityId FROM Quotes q inner join Building b on q.quoteid=b.Quoteid
				--						WHERE b.Id=@buildingid)
				IF(@previousstatus<>@status)
				BEGIN	
					exec [dbo].[usp_UpdateWorkflowStatus]@quouteid,'quote'
				END
			END
			
			
		END
		else IF(@section='project')
		BEGIN
			set @projectid=@id
			DECLARE @source int
			SELECT @source=projectsource FROM Projects WHERE OpportunityId=@projectid
			SELECT @projectstatus=WorkflowStage from Projects where OpportunityId=@projectid
			IF(@source=1)
			BEGIN
				
				DECLARE @buidingcount int,@unitcount int,@step int
				SELECT @buidingcount=count(*) FROM Building WHERE QuoteId in (SELECT QuoteId FROM Quotes WHERE OpportunityId=@projectid)
				SELECT @unitcount=count(*) FROM units u inner join
				GroupConfiguration g on u.groupconfigurationid=g.GroupId
				inner join Building b on g.buildingid=b.id
				WHERE QuoteId in(SELECT distinct QuoteId FROM Quotes WHERE opportunityid=@projectid)
				IF(@buidingcount>0)
				BEGIN
					IF(@unitcount>0)
					BEGIN 
						set @step=(SELECT step FROM Status WHERE StatusKey='PRJ_BDSUB')
						UPDATE Projects set workFlowStage='PRJ_BDSUB' 
						FROM Projects p inner join Status s on p.WorkflowStage=s.StatusKey
						WHERE opportunityid=@projectid and s.Step<@step
					END
					else
					BEGIN
						set @step=(SELECT step FROM Status WHERE StatusKey='PRJ_OPP')
						UPDATE Projects set workFlowStage='PRJ_OPP' 
						FROM Projects p inner join Status s on p.WorkflowStage=s.StatusKey
						WHERE opportunityid=@projectid and s.Step<@step
					END
				END
				else
				BEGIN
					set @step=(SELECT step FROM Status WHERE StatusKey='PRJ_LEAD')
					UPDATE Projects set workFlowStage='PRJ_LEAD' 
					FROM Projects p inner join Status s on p.WorkflowStage=s.StatusKey
					WHERE opportunityid=@projectid and s.Step<@step
				END
			END
			else 
			begin
				IF(@projectstatus='PRJ_OH')
				begin
					UPDATE Building set workflowstatus='BLDG_OH' WHERE quoteid in (SELECT distinct QuoteId FROM Quotes WHERE opportunityid=@projectid)
					UPDATE GroupConfiguration set workflowstatus='GRP_OH' WHERE BuildingId in(SELECT Id FROM Building WHERE quoteid in (SELECT distinct QuoteId FROM Quotes WHERE opportunityid=@projectid))
					UPDATE Units set workflowstatus='UNIT_OH' WHERE GroupConfigurationId in (SELECT GroupId FROM GroupConfiguration g inner join Building b on
																					  b.id=g.buildingid WHERE quoteid in (SELECT distinct QuoteId FROM Quotes WHERE opportunityid=@projectid)	)
				end
				else if(@projectstatus='PRJ_CANC')
				begin
					UPDATE Building set workflowstatus='BLDG_CANC' WHERE quoteid in (SELECT distinct QuoteId FROM Quotes WHERE opportunityid=@projectid)
					UPDATE GroupConfiguration set workflowstatus='GRP_CANC' WHERE BuildingId in(SELECT Id FROM Building WHERE quoteid in (SELECT distinct QuoteId FROM Quotes WHERE opportunityid=@projectid))
					UPDATE Units set workflowstatus='UNIT_CANC' WHERE GroupConfigurationId in (SELECT GroupId FROM GroupConfiguration g inner join Building b on
																					  b.id=g.buildingid WHERE quoteid in (SELECT distinct QuoteId FROM Quotes WHERE opportunityid=@projectid)	)
				end
				else if(@projectstatus='PRJ_CLSD')
				begin
					UPDATE Building set workflowstatus='BLDG_CLSD' WHERE quoteid  in (SELECT distinct QuoteId FROM Quotes WHERE opportunityid=@projectid)
					UPDATE GroupConfiguration set workflowstatus='GRP_CLSD' WHERE BuildingId in(SELECT Id FROM Building WHERE quoteid in (SELECT distinct QuoteId FROM Quotes WHERE opportunityid=@projectid))
					UPDATE Units set workflowstatus='UNIT_CLSD' WHERE GroupConfigurationId in (SELECT GroupId FROM GroupConfiguration g inner join Building b on
																					  b.id=g.buildingid WHERE quoteid in (SELECT distinct QuoteId FROM Quotes WHERE opportunityid=@projectid)	)
				end
			end
				
		END
		else IF(@section='quote')
		BEGIN
			set @quouteid=@id
			SELECT @projectid= OpportunityId FROM Quotes WHERE QuoteId=@quouteid 
			DECLARE @noofbuilding int,@buildingvalid int,@buildinginvalid int,@groupReleased int,@grpLocked int
			SELECT @noofbuilding=count(*) FROM Building WHERE QuoteId=@quouteid and IsDeleted=0
			SELECT @buildingvalid=count(*) FROM Building WHERE QuoteId=@quouteid and WorkflowStatus='BLDG_VAL' and IsDeleted=0
			
			SELECT @noofGroup=count(*) FROM GroupConfiguration g
			join Building b on g.BuildingId=b.Id
			WHERE b.QuoteId=@quouteid and g.IsDeleted=0 and b.IsDeleted=0
			
			Select @groupReleased=count(*) FROM GroupConfiguration g
			join Building b on g.BuildingId=b.Id
			WHERE b.QuoteId=@quouteid and g.WorkflowStatus='GRP_FR' and g.IsDeleted=0 and b.IsDeleted=0

			Select @grpLocked=count(*) FROM GroupConfiguration g
			join Building b on g.BuildingId=b.Id
			WHERE b.QuoteId=@quouteid and g.WorkflowStatus='GRP_LOC' and g.IsDeleted=0 and b.IsDeleted=0

			--IF(@noofbuilding=@buildingvalid)
			--BEGIN
			--	UPDATE Projects set workFlowStage='PRJ_BDAWD' WHERE OpportunityId in(SELECT OpportunityId FROM Quotes WHERE QuoteId=@quouteid) and ProjectSource=1
			--END
			SELECT @buildinginvalid=count(*) FROM Building WHERE QuoteId=@quouteid and WorkflowStatus in('BLDG_CINV') and IsDeleted=0
			IF(@noofbuilding=0)
			BEGIN
				UPDATE Quotes SET QuoteStatusId='QT_INC' where QuoteId=@quouteid
			END
			ELSE
			BEGIN
				IF(@buildinginvalid>0)
				BEGIN
					UPDATE Quotes SET QuoteStatusId='QT_INV' where QuoteId=@quouteid
				END
				ELSE
				BEGIN
					IF(@groupReleased=@noofGroup)
					BEGIN
						UPDATE Quotes SET QuoteStatusId='QT_FR' where QuoteId=@quouteid
					END
					ELSE
					BEGIN
						IF(@noofbuilding=@buildingvalid)
						BEGIN
							UPDATE Quotes SET QuoteStatusId='QT_COM' where QuoteId=@quouteid
							UPDATE Projects set workFlowStage='PRJ_BDAWD' 
							WHERE OpportunityId in(SELECT OpportunityId FROM Quotes WHERE QuoteId=@quouteid) and ProjectSource=1
						END
						ELSE IF(@noofGroup=@grpLocked)
						BEGIN
							UPDATE Quotes SET QuoteStatusId='QT_LOC' where QuoteId=@quouteid
						END
						ELSE
						BEGIN
							UPDATE Quotes SET QuoteStatusId='QT_INC' where QuoteId=@quouteid and QuoteStatusId<>'QT_CRD'
						END
					END
				
				END
			END
			
		END
		else IF(@section='senttocordination')
		BEGIN
			set @quouteid=@id
			UPDATE Quotes SET QuoteStatusId='QT_CRD' where QuoteId=@quouteid
			UPDATE Building set workflowstatus='BLDG_CRD' WHERE quoteid=@quouteid
			UPDATE GroupConfiguration set workflowstatus='GRP_CRD' WHERE BuildingId in(SELECT Id FROM Building WHERE quoteid=@quouteid)
			UPDATE Units set workflowstatus='UNIT_CRD' WHERE GroupConfigurationId in (SELECT GroupId FROM GroupConfiguration g inner join Building b on
																					  b.id=g.buildingid WHERE quoteid=@quouteid	)
		END
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

