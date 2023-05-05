
CREATE procedure [dbo].[usp_CreateUpdateProject]
@ProjectDetailList MiniProjectsDetailsListValuesData READONLY
,@quoteCreationFlag bit
,@ProjectsInfoTemp ProjectsAccountDetails READONLY
,@Result int --OUTPUT
as
begin
begin try
		--select @Result 
		declare @IsNewQuote bit = 0
		declare @BranchNumber nvarchar(20)
		DECLARE @QuoteId nvarchar(50)
		declare @opportunityId nvarchar(50)
		declare @previousStatus nvarchar(50)
		set @previousStatus=(select WorkflowStage  from projects where OpportunityId in  (select distinct(OpportunityId) from @ProjectDetailList))
		set @BranchNumber = (select  min(BranchNumber) from Branch where Branch like '%' +(Select distinct(BranchValue) from @ProjectDetailList)+ '%') 
		declare @isPrimaryFlag bit
		set @isPrimaryFlag = (select distinct isPrimaryQuote from @ProjectDetailList)
		if(@BranchNumber is null)
		begin
			select  'Invalid BranchID' as InvalidBranchID
			RAISERROR('Invalid BranchID.',16,1)
			EXEC usp_Log_ProcedureCall
			@ObjectID = @@PROCID;
			return 0
		end 
		declare @workflowStageValue nvarchar(20)
		set @workFlowStageValue = (select StatusKey from status where StatusId = (select min(StatusId) from Status where StatusName = (Select distinct(SalesId) from @ProjectDetailList)))
		--(select statusKey from status where StatusId = (select  min(Id) from Sales where Code = (select distinct(SalesId) from @ProjectDetailList)))
		declare @BusinessLineIdValue nvarchar(20)
		set @BusinessLineIdValue = (select  min(Id) from BusinessLine where Code = (select distinct(BusinessLine) from @ProjectDetailList))
		declare @MeasuringIdValue nvarchar(20)
		set @MeasuringIdValue = (select min(Id) from MeasuringUnits where Code = (Select distinct(MeasuringUnit) from @ProjectDetailList))
		declare @QuoteStatus nvarchar(20)
		--set @QuoteStatus = (select StatusKey from status where StatusId = cas(select min(StatusId) from Status where StatusName = (Select distinct(QuoteStatus) from @ProjectDetailList)))
		declare @country nvarchar(10)
		 
		 --	if(exists (select * from Quotes where OpportunityId =(select distinct(OpportunityId) from @ProjectDetailList) and VersionId=((select distinct(VersionId) from @ProjectDetailList))))
			--Begin
			--   SET @quoteCreationFlag = 0;
			--End
			--Else
			--Begin
			-- SET @quoteCreationFlag = 1;
			--End
		if(@quoteCreationFlag=0)-- update scenario
		begin
			set @country=(select distinct(country) from @ProjectDetailList)
			update projectsValues 
					set projectsValues.[Name] = ProjectDtl.[Name],projectsValues.BranchNumber = @BranchNumber,projectsValues.workFlowStage = @workFlowStageValue,
					projectsValues.BusinessLineId = @BusinessLineIdValue,projectsValues.MeasuringUnitId = @MeasuringIdValue
					,projectsValues.ModifiedBy = ProjectDtl.CreatedBy,
					projectsValues.ModifiedOn = getdate(), projectsValues.ProjectJson = ProjectDtl.ProjectJson,
					projectsValues.Salesman = ProjectDtl.Salesman, ProjectsValues.SalesmanActiveDirectoryID = ProjectDtl.SalesmanActiveDirectoryID
					from Projects projectsValues
					join @ProjectDetailList ProjectDtl
					on ProjectDtl.OpportunityId = projectsValues.OpportunityId
					where projectsValues.OpportunityId = (select distinct(OpportunityId) from @ProjectDetailList)
					-- address details check
					if exists(select isnull(AddressLine1,'') from @ProjectsInfoTemp)
				Begin
				update AccDtl
				set AccDtl.OpportunityId = ProTmp.opportunityId,
					AccDtl.AccountName   = ProTmp.AccountName,
					AccDtl.Type			 = ProTmp.Type,
					AccDtl.AddressLine1  = ProTmp.AddressLine1,
					AccDtl.City		     = ProTmp.City,
					AccDtl.State		 = ProTmp.State,
					AccDtl.County		 = ProTmp.Country,
					AccDtl.ZipCode		 = ProTmp.ZipCode,
					AccDtl.CustomerNumber= ProTmp.CustomerNumber,
					AccDtl.AddressLine2  = ProTmp.AddressLine2,
					AccDtl.AwardCloseDate=ProTmp.AwardCloseDate
				from AccountDetails AccDtl
				join @ProjectsInfoTemp ProTmp
				on AccDtl.opportunityId = ProTmp.opportunityId
				and AccDtl.Type = ProTmp.Type
				where AccDtl.opportunityId =  (select distinct(OpportunityId) from @ProjectDetailList)
				End

			update QuoteValues 
					set --QuoteStatusId = @QuoteStatus,
					QuoteValues.[Description] = ProjectDtl.[description],
					QuoteValues.ModifiedBy = ProjectDtl.CreatedBy,
					QuoteValues.ModifiedOn = getdate()
					from Quotes QuoteValues
					join @ProjectDetailList ProjectDtl
					on ProjectDtl.OpportunityId = QuoteValues.OpportunityId
				where QuoteValues.OpportunityId  = (select distinct(OpportunityId) from @ProjectDetailList)
				and QuoteValues.VersionId=((select distinct(VersionId) from @ProjectDetailList))
			select OpportunityId,0 as VersionId,'' as QuoteId,'' as QuoteStatus , @IsNewQuote as IsNewQuote from Projects where OpportunityId  = (select distinct(OpportunityId) from @ProjectDetailList)
					--and VersionId=((select distinct(VersionId) from @ProjectDetailList))
			--set @Result=(select QuoteId from Quotes where OpportunityId  = (select distinct(OpportunityId) from @ProjectDetailList)
			--	and VersionId=((select distinct(VersionId) from @ProjectDetailList)))
			return 0
		end
		else if(@Result <>0)-- from list of configuration sp
		begin
			set @country=(select distinct(country) from @ProjectDetailList)
			if(exists(select * from Projects where OpportunityId in (select distinct(OpportunityId) from @ProjectDetailList)))
			begin
				update projectsValues 
					set projectsValues.[Name] = ProjectDtl.[Name],projectsValues.BranchNumber = @BranchNumber,projectsValues.workFlowStage = @workFlowStageValue,
					projectsValues.BusinessLineId = @BusinessLineIdValue,projectsValues.MeasuringUnitId = @MeasuringIdValue
					,projectsValues.ModifiedBy = ProjectDtl.CreatedBy,
					projectsValues.ModifiedOn = getdate(), projectsValues.ProjectJson = ProjectDtl.ProjectJson,
					projectsValues.Salesman = ProjectDtl.Salesman, ProjectsValues.SalesmanActiveDirectoryID = ProjectDtl.SalesmanActiveDirectoryID
					from Projects projectsValues
					join @ProjectDetailList ProjectDtl
					on ProjectDtl.OpportunityId = projectsValues.OpportunityId
					where projectsValues.OpportunityId = (select distinct(OpportunityId) from @ProjectDetailList)
					
				if exists (select isnull(AddressLine1,'') from @ProjectsInfoTemp)
				Begin
				update AccDtl
				set AccDtl.OpportunityId = ProTmp.opportunityId,
					AccDtl.AccountName   = ProTmp.AccountName,
					AccDtl.Type			 = ProTmp.Type,
					AccDtl.AddressLine1  = ProTmp.AddressLine1,
					AccDtl.City		     = ProTmp.City,
					AccDtl.State		 = ProTmp.State,
					AccDtl.County		 = ProTmp.Country,
					AccDtl.ZipCode		 = ProTmp.ZipCode,
					AccDtl.CustomerNumber= ProTmp.CustomerNumber,
					AccDtl.AddressLine2  = ProTmp.AddressLine2,
					AccDtl.AwardCloseDate=ProTmp.AwardCloseDate
				from AccountDetails AccDtl
				join @ProjectsInfoTemp ProTmp
				on AccDtl.opportunityId = ProTmp.opportunityId
				and AccDtl.Type = ProTmp.Type
				where AccDtl.opportunityId =  (select distinct(OpportunityId) from @ProjectDetailList)
				End
				set @opportunityId= (select distinct(OpportunityId) from @ProjectDetailList)
				if(@workflowStageValue<>'PRJ_OH'and @workflowStageValue<>'PRJ_CANC' and @workflowStageValue<>'PRJ_CLSD' and @previousStatus='PRJ_OH')
				begin
						UPDATE Building set workflowstatus='BLDG_INC' WHERE quoteid in (SELECT distinct QuoteId FROM Quotes WHERE opportunityid=@opportunityId)
						UPDATE GroupConfiguration set workflowstatus='GRP_INC' WHERE BuildingId in(SELECT Id FROM Building WHERE quoteid in (SELECT distinct QuoteId FROM Quotes WHERE opportunityid=@opportunityId))
						UPDATE Units set workflowstatus='UNIT_INC' WHERE GroupConfigurationId in (SELECT GroupId FROM GroupConfiguration g inner join Building b on
										b.id=g.buildingid WHERE quoteid in (SELECT distinct QuoteId FROM Quotes WHERE opportunityid=@opportunityId))
				end
				exec [dbo].[usp_UpdateWorkflowStatus]@opportunityId,'project'
			end
			else
			begin
				insert into Projects (Name,BranchId,BranchNumber,workFlowStage,Salesman,SalesmanActiveDirectoryID,BusinessLineId,MeasuringUnitId,CreatedBy,CreatedOn, ModifiedBy, ModifiedOn,ProjectJson,ProjectSource)
						select [Name],0,@BranchNumber,@workFlowStageValue,Salesman,SalesmanActiveDirectoryID,@BusinessLineIdValue,@MeasuringIdValue,CreatedBy,GETDATE(),CreatedBy,GETDATE(),ProjectJson,2
						from @ProjectDetailList
				
				-- Address details Added
				if exists (select isnull(AddressLine1,'') from @ProjectsInfoTemp)
			Begin
			Insert into AccountDetails(OpportunityId,Type,AddressLine1,City,State,County,ZipCode,CustomerNumber,AddressLine2, AccountName)
			select OpportunityId,Type,AddressLine1,City,State,Country,ZipCode,CustomerNumber,AddressLine2, AccountName from @ProjectsInfoTemp
			End
			UPDATE PROJECTS SET OpportunityId =(SELECT DISTINCT(OpportunityId) FROM @ProjectDetailList) WHERE ID=SCOPE_IDENTITY()
			if exists (select isnull(AddressLine1,'') from @ProjectsInfoTemp)
			Begin
			UPDATE ACCOUNTDETAILS set opportunityid = (SELECT DISTINCT(OpportunityId) FROM @ProjectDetailList) WHERE ID=SCOPE_IDENTITY()
		end
			end
			if(exists (select * from Quotes where OpportunityId =(select distinct(OpportunityId) from @ProjectDetailList) and VersionId=((select distinct(VersionId) from @ProjectDetailList))))
			BEGIN
				update QuoteValues 
					set --QuoteStatusId = @QuoteStatus,
					QuoteValues.[Description] = ProjectDtl.[description],
					QuoteValues.ModifiedBy = ProjectDtl.CreatedBy,
					QuoteValues.ModifiedOn = getdate()
					from Quotes QuoteValues
					join @ProjectDetailList ProjectDtl
					on ProjectDtl.OpportunityId = QuoteValues.OpportunityId
				where QuoteValues.OpportunityId  = (select distinct(OpportunityId) from @ProjectDetailList)
				and QuoteValues.VersionId=((select distinct(VersionId) from @ProjectDetailList))
			select OpportunityId,VersionId,QuoteId , s.DisplayName QuoteStatus,@IsNewQuote as IsNewQuote 
			from Quotes q join status s on q.QuoteStatusId=s.StatusKey 
			where OpportunityId  = (select distinct(OpportunityId) from @ProjectDetailList)
				and VersionId=((select distinct(VersionId) from @ProjectDetailList))
			--set @Result=(select QuoteId from Quotes where OpportunityId  = (select distinct(OpportunityId) from @ProjectDetailList)
			--	and VersionId=((select distinct(VersionId) from @ProjectDetailList)))
			if(@isPrimaryFlag = 1)
			begin
			if exists(select * from PrimaryQuotes where OpportunityId = (select distinct OpportunityId from @ProjectDetailList))
			begin
				update PrimaryQuotes
				set PrimaryQuoteId = (select QuoteId from Quotes where OpportunityId = (select distinct OpportunityId from @ProjectDetailList)
				and VersionId = (select distinct VersionId from @ProjectDetailList))
				where OpportunityId = (select distinct OpportunityId from @ProjectDetailList)
			end
			else
			begin

				Insert into PrimaryQuotes(OpportunityId, PrimaryQuoteId, CreatedBy, ModifiedBy, ModifiedOn)
				select OpportunityId, QuoteId, CreatedBy, CreatedBy, GETDATE() 
				from quotes where QuoteId = (select QuoteId from Quotes where OpportunityId = (select distinct OpportunityId from @ProjectDetailList) 
				and VersionId = (select distinct VersionId from @ProjectDetailList))

			End
			end

			return 0
			END
			ELSE
			BEGIN
				insert into Quotes(OpportunityId,VersionId,QuoteStatusId,[Description],CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
				select OpportunityId,VersionId,'QT_INC',[Description],CreatedBy,GETDATE(),CreatedBy,GETDATE() from @ProjectDetailList
				--update Quotes set QuoteId=@country+'-'+cast(year(GETDATE()) as nvarchar(4))+'-'
				--+(
				--case when LEN(SCOPE_IDENTITY()) = 1 then '0000000'+CAST(SCOPE_IDENTITY() as varchar(6))
				--when LEN(SCOPE_IDENTITY()) = 2 then '000000'+CAST(SCOPE_IDENTITY() as varchar(6))
				--when LEN(SCOPE_IDENTITY()) = 3 then '00000'+CAST(SCOPE_IDENTITY() as varchar(6))
				--when LEN(SCOPE_IDENTITY()) = 4 then '0000'+CAST(SCOPE_IDENTITY() as varchar(6))
				--when LEN(SCOPE_IDENTITY()) = 5 then '000'+CAST(SCOPE_IDENTITY() as varchar(6))
				--when LEN(SCOPE_IDENTITY()) = 6 then '00'+CAST(SCOPE_IDENTITY() as varchar(6))
				--when LEN(SCOPE_IDENTITY()) = 7 then '0'+CAST(SCOPE_IDENTITY() as varchar(6))
				--when LEN(SCOPE_IDENTITY()) = 8 then +CAST(SCOPE_IDENTITY() as varchar(6))
				--end) where id=SCOPE_IDENTITY()
				
				EXEC usp_GenerateQuoteId @country, @QuoteId = @QuoteId OUTPUT
				update Quotes set QuoteId=@QuoteId where id=SCOPE_IDENTITY()
				set @IsNewQuote =1
				select OpportunityId,VersionId,QuoteId,s.DisplayName QuoteStatus, @IsNewQuote as IsNewQuote 
				from Quotes q join status s on q.QuoteStatusId=s.StatusKey
				where id=SCOPE_IDENTITY()

			if(@isPrimaryFlag = 1)
			begin
			if exists(select * from PrimaryQuotes where OpportunityId = (select distinct OpportunityId from @ProjectDetailList))
			begin
				update PrimaryQuotes
				set PrimaryQuoteId = (select QuoteId from Quotes where OpportunityId = (select distinct OpportunityId from @ProjectDetailList) and VersionId = (select distinct VersionId from @ProjectDetailList))
				where OpportunityId = (select distinct OpportunityId from @ProjectDetailList)
			end
			else
			begin

				Insert into PrimaryQuotes(OpportunityId, PrimaryQuoteId, CreatedBy, ModifiedBy, ModifiedOn)
				select OpportunityId, QuoteId, CreatedBy, CreatedBy, GETDATE() 
				from quotes where QuoteId = (select QuoteId from Quotes where id=SCOPE_IDENTITY())

			End
			end
				--set @Result=SCOPE_IDENTITY()
				return 0
			END
		end
		else--save scenario
		begin
			set @country=(select distinct(country) from @ProjectDetailList)
			declare @oppId nvarchar(60)
			set @oppId=(select distinct(OpportunityId) from @ProjectDetailList)
			if(@oppId='')
			begin
				insert into Projects (Name,BranchId,BranchNumber,workFlowStage,Salesman,SalesmanActiveDirectoryID,BusinessLineId,MeasuringUnitId,CreatedBy,CreatedOn, ModifiedBy, ModifiedOn,ProjectJson,ProjectSource)
						select [Name],0,@BranchNumber,@workFlowStageValue,Salesman,SalesmanActiveDirectoryID,@BusinessLineIdValue,@MeasuringIdValue,CreatedBy,GETDATE(),CreatedBy,GETDATE(),ProjectJson,1
						from @ProjectDetailList
				update projects set OpportunityId='SC-'+cast(SCOPE_IDENTITY() as nvarchar(10)),@oppId='SC-'+cast(SCOPE_IDENTITY() as nvarchar(10)) where id=SCOPE_IDENTITY()
				set @oppId=(select OpportunityId from Projects where Id=SCOPE_IDENTITY())

				-- Address Details check 
					if exists (select isnull(AddressLine1,'') from @ProjectsInfoTemp)
				Begin
				Insert into AccountDetails(OpportunityId,Type,AddressLine1,City,State,County,ZipCode,CustomerNumber,AddressLine2, AccountName)
				select @oppId,Type,AddressLine1,City,State,Country,ZipCode,CustomerNumber,AddressLine2, AccountName from @ProjectsInfoTemp
				end
			end
			else
			begin
				update projectsValues 
					set projectsValues.[Name] = ProjectDtl.[Name],projectsValues.BranchNumber = @BranchNumber,projectsValues.workFlowStage = @workFlowStageValue,
					projectsValues.BusinessLineId = @BusinessLineIdValue,projectsValues.MeasuringUnitId = @MeasuringIdValue
					,projectsValues.ModifiedBy = ProjectDtl.CreatedBy,
					projectsValues.ModifiedOn = getdate(), projectsValues.ProjectJson = ProjectDtl.ProjectJson,
					projectsValues.Salesman = ProjectDtl.Salesman, ProjectsValues.SalesmanActiveDirectoryID = ProjectDtl.SalesmanActiveDirectoryID
					from Projects projectsValues
					join @ProjectDetailList ProjectDtl
					on ProjectDtl.OpportunityId = projectsValues.OpportunityId
					where projectsValues.OpportunityId = (select distinct(OpportunityId) from @ProjectDetailList)
					if exists (select isnull(AddressLine1,'') from @ProjectsInfoTemp)
				Begin
				update AccDtl
				set AccDtl.OpportunityId = ProTmp.opportunityId,
					AccDtl.AccountName   = ProTmp.AccountName,
					AccDtl.Type			 = ProTmp.Type,
					AccDtl.AddressLine1  = ProTmp.AddressLine1,
					AccDtl.City		     = ProTmp.City,
					AccDtl.State		 = ProTmp.State,
					AccDtl.County		 = ProTmp.Country,
					AccDtl.ZipCode		 = ProTmp.ZipCode,
					AccDtl.CustomerNumber= ProTmp.CustomerNumber,
					AccDtl.AddressLine2  = ProTmp.AddressLine2,
					AccDtl.AwardCloseDate=ProTmp.AwardCloseDate
				from AccountDetails AccDtl
				join @ProjectsInfoTemp ProTmp
				on AccDtl.opportunityId = ProTmp.opportunityId
				and AccDtl.Type = ProTmp.Type
				where AccDtl.opportunityId =  (select distinct(OpportunityId) from @ProjectDetailList)
				End
			end
			declare @opId nvarchar(20)
			set @opId=@oppId
			insert into Quotes(OpportunityId,VersionId ,QuoteStatusId,Description,CreatedBy,CreatedOn,modifiedBy,ModifiedOn)
				select @oppId,VersionId,'QT_INC',[description],CreatedBy,GETDATE(),CreatedBy,getdate() from @ProjectDetailList
		
			set @oppId= SCOPE_IDENTITY()
			declare @versionid int
			set @versionid=(select distinct(VersionId) from Quotes where id=SCOPE_IDENTITY())
			if(@versionid is null or @versionid=0)
			begin
				--update Quotes set VersionId=(select count(*) from Quotes where OpportunityId=@opId),QuoteId=@country+'-'+cast(year(GETDATE()) as nvarchar(4))+'-'
				--	+(
				--	case when LEN(SCOPE_IDENTITY()) = 1 then '0000000'+CAST(@oppId as varchar(6))
				--	when LEN(SCOPE_IDENTITY()) = 2 then '000000'+CAST(@oppId as varchar(6))
				--	when LEN(SCOPE_IDENTITY()) = 3 then '00000'+CAST(@oppId as varchar(6))
				--	when LEN(SCOPE_IDENTITY()) = 4 then '0000'+CAST(@oppId as varchar(6))
				--	when LEN(SCOPE_IDENTITY()) = 5 then '000'+CAST(@oppId as varchar(6))
				--	when LEN(SCOPE_IDENTITY()) = 6 then '00'+CAST(@oppId as varchar(6))
				--	when LEN(SCOPE_IDENTITY()) = 7 then '0'+CAST(@oppId as varchar(6))
				--	when LEN(SCOPE_IDENTITY()) = 8 then +CAST(@oppId as varchar(6))
				--	end) where id=SCOPE_IDENTITY()

					EXEC usp_GenerateQuoteId @country, @QuoteId = @QuoteId OUTPUT
					update Quotes set VersionId=(select count(*) from Quotes where OpportunityId=@opId),QuoteId=@QuoteId where id=SCOPE_IDENTITY()
			end
			else
			begin
				--update Quotes set QuoteId=@country+'-'+cast(year(GETDATE()) as nvarchar(4))+'-'
				--	+(
				--	case when LEN(SCOPE_IDENTITY()) = 1 then '0000000'+CAST(@oppId as varchar(6))
				--	when LEN(SCOPE_IDENTITY()) = 2 then '000000'+CAST(@oppId as varchar(6))
				--	when LEN(SCOPE_IDENTITY()) = 3 then '00000'+CAST(@oppId as varchar(6))
				--	when LEN(SCOPE_IDENTITY()) = 4 then '0000'+CAST(@oppId as varchar(6))
				--	when LEN(SCOPE_IDENTITY()) = 5 then '000'+CAST(@oppId as varchar(6))
				--	when LEN(SCOPE_IDENTITY()) = 6 then '00'+CAST(@oppId as varchar(6))
				--	when LEN(SCOPE_IDENTITY()) = 7 then '0'+CAST(@oppId as varchar(6))
				--	when LEN(SCOPE_IDENTITY()) = 8 then +CAST(@oppId as varchar(6))
				--	end) where id=SCOPE_IDENTITY()

					EXEC usp_GenerateQuoteId @country, @QuoteId = @QuoteId OUTPUT
					update Quotes set QuoteId=@QuoteId where id=SCOPE_IDENTITY()
			end
			set @IsNewQuote = 1
			select OpportunityId,VersionId,QuoteId ,s.DisplayName QuoteStatus,@IsNewQuote as IsNewQuote 
			from Quotes q join status s on q.QuoteStatusId=s.StatusKey--where OpportunityId = 'SC-70'
			where id=SCOPE_IDENTITY()
			--select SCOPE_IDENTITY() as QuoteId 
			--set @Result=SCOPE_IDENTITY()

			If ((select count(*) from quotes where OpportunityId = (select OpportunityId from Quotes where id=SCOPE_IDENTITY())) = 1)
			Begin
			Insert into PrimaryQuotes(OpportunityId, PrimaryQuoteId, CreatedBy, ModifiedBy, ModifiedOn)
			select OpportunityId, QuoteId, CreatedBy, CreatedBy, GETDATE() 
			from quotes where OpportunityId = (select OpportunityId from Quotes where id=SCOPE_IDENTITY())
			End

			return 0

		end
	end try
	begin catch
	EXEC usp_Log_ProcedureCall
			@ObjectID = @@PROCID;
	end catch
end

