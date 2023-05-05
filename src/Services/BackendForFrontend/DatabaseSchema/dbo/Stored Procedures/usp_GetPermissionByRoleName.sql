CREATE procedure [dbo].[usp_GetPermissionByRoleName] --'62','CSC - Power User','releaseinfo'--null,'Canada - Sales Rep','Unit'
@id nvarchar(50),
@roleName nvarchar(100),
@entity nvarchar(50)
as
begin
	BEGIN TRY
		declare @projectid nvarchar(10),@buildingid int,@groupid int,@setid int
		declare @projectstatus nvarchar(10), @buildingstatus nvarchar(10), @groupstatus nvarchar(10), @unitstatus nvarchar(10)
		
		--added for initial testing
		declare @roleKey nvarchar(20)
		select @roleKey = RoleKey from UserRoleMaster where RoleName = @roleName

		--declare @editPermission nvarchar(10), @addPermission bit
		if(@entity='Building')
		begin

		
			set @buildingid=cast(@id as int)
			if(@buildingid=-1)
			begin
				select distinct  PermissionKey from PermissionMapping
			where entityid in(select entityid from Entity where EntityName in('Building')) 
			and Projectstage in(select distinct projectstage from PermissionMapping 
								where Permissionkey in(select permissionkey from Permissions 
														where EntityId in(select entityid from Entity where EntityName in('Building') and PermissionName='ADD'))) 
			and Buildingstatus='BLDG_INC' 
			and RoleKey = @roleKey
			union
			select distinct PermissionKey from PermissionMapping
			where entityid in(select entityid from Entity where EntityName in('Group')) 
			and Projectstage in(select distinct projectstage from PermissionMapping 
								where Permissionkey in(select permissionkey from Permissions 
														where EntityId in(select entityid from Entity where EntityName in('Building') and PermissionName='ADD'))) 
			and Buildingstatus='BLDG_INC'  
			and RoleKey = @roleKey
			and PermissionKey in(select Permissionkey from Permissions where EntityId in (select entityid from Entity where EntityName in('Group')) and PermissionName='ADD')
		

			end
			else
			begin
				select @projectid= q.opportunityid from Quotes q inner join Building b on q.Quoteid=b.Quoteid where b.id=@buildingid
				select @projectstatus=workflowstage from projects where opportunityid=@projectid
				select @buildingstatus=workflowstatus from Building where id=@buildingid
				select distinct  PermissionKey from PermissionMapping
				where entityid in(select entityid from Entity where EntityName in('Building')) 
				and Projectstage =@projectstatus and Buildingstatus=@buildingstatus 
				and RoleKey = @roleKey
				union
				select distinct PermissionKey from PermissionMapping
				where entityid in(select entityid from Entity where EntityName in('Group')) 
				and Projectstage =@projectstatus and Buildingstatus=@buildingstatus 
				and RoleKey = @roleKey
				and PermissionKey in(select Permissionkey from Permissions where EntityId in (select entityid from Entity where EntityName in('Group')) and PermissionName='ADD')
		
			end
		
			--and PermissionName in('EDIT','READONLY')
			--SET @addPermission= case when exists(select PermissionName from PermissionMapping where entity='Group' and Projectstagekey =@projectstatus and Buildingstatuskey=@buildingstatus
			--and RoleKey=@roleKey and PermissionName in('ADD')) then 1 else 0 end

		end
		else if(@entity='Project')
		begin
			set @projectId=@id
			if(@projectId=null or @projectId='')
			begin
				select distinct PermissionKey from PermissionMapping
				where entityid in(select entityid from Entity where EntityName in('Project')) 
				--and Projectstage =@projectstatus-- and Buildingstatus=@buildingstatus and GroupStatus=null
				and RoleKey = @roleKey
				and PermissionKey in('PRJ_UPDATE','PRJ_DELETE','PRJ_ADD')
			end
			else
			begin
				select @projectstatus=WorkflowStage from Projects where OpportunityId=@projectid
				select distinct PermissionKey from PermissionMapping
				where entityid in(select entityid from Entity where EntityName in('Project')) 
				and Projectstage =@projectstatus-- and Buildingstatus=@buildingstatus and GroupStatus=null
				and RoleKey = @roleKey
			end
		 

		end
		else if(@entity='Group')
		begin
			set @groupid=cast(@id as int)
			set @groupStatus = (select distinct(WorkflowStatus) from GroupConfiguration where GroupId=@groupid);
			set @buildingId  = (select distinct(BuildingId) from GroupConfiguration where GroupId=@groupid);
			set @buildingStatus  = (select distinct WorkflowStatus from Building where Id=@buildingId);
			select @projectid= q.opportunityid from Quotes q inner join Building b on q.Quoteid=b.Quoteid where b.id=@buildingid
			select @projectstatus=workflowstage from projects where opportunityid=@projectid

			select PermissionKey from PermissionMapping
			where entityid in(select entityid from Entity where EntityName in('Group')) 
			and Projectstage =@projectstatus and Buildingstatus=@buildingstatus and GroupStatus=@groupStatus 
			and RoleKey = @roleKey
			union
			select PermissionKey from PermissionMapping
			where entityid in(select entityid from Entity where EntityName in('Unit')) 
			and Projectstage =@projectstatus and Buildingstatus=@buildingstatus and GroupStatus=@groupStatus 
			and RoleKey = @roleKey
			and PermissionKey in(select Permissionkey from Permissions where EntityId in (select entityid from Entity where EntityName in('Unit')) and PermissionName='ADD')
		
		end
		else if(@entity='Unit')
		begin
			set @setId=cast(@id as int)
			--select
			set @groupid=(select distinct(GroupConfigurationId) from Units where SetId=@setId);
			set @unitstatus  = (select distinct WorkflowStatus from Units where SetId=@setId);
			set @groupStatus = (select distinct(WorkflowStatus) from GroupConfiguration where GroupId=@groupid);
			set @buildingId  = (select distinct(BuildingId) from GroupConfiguration where GroupId=@groupid);
			set @buildingStatus  = (select distinct WorkflowStatus from Building where Id=@buildingId);
			select @projectid= q.opportunityid from Quotes q inner join Building b on q.Quoteid=b.Quoteid where b.id=@buildingid
			select @projectstatus=workflowstage from projects where opportunityid=@projectid

			select PermissionKey from PermissionMapping
			where entityid in(select entityid from Entity where EntityName in('Unit')) 
			and Projectstage =@projectstatus and Buildingstatus=@buildingstatus and GroupStatus=@groupStatus 
			and UnitStatus=@unitstatus 
			and RoleKey = @roleKey
			and PermissionKey in(select Permissionkey from Permissions where EntityId in (select entityid from Entity where EntityName in('Unit')))
		
		end
		else if(@entity='ListofConfiguration')
		begin
			
			select @projectstatus =  WorkflowStage from Projects p inner join Quotes q on p.OpportunityId=q.OpportunityId 
			where q.QuoteId=@id
			--select @projectstage
			select distinct e.EntityName,
					pm.PermissionKey,
					isnull(bldgSt.StatusName,'') BuildingStatus,
					isnull(grpSt.StatusName,'') GroupStatus,
					isnull(unitSt.StatusName,'') UnitStatus
			from PermissionMapping pm inner join Entity e on pm.EntityId=e.EntityId
			left join Status bldgSt on pm.BuildingStatus=bldgSt.StatusKey
			left join Status grpSt on pm.GroupStatus=grpSt.StatusKey
			left join Status unitSt on pm.UnitStatus=unitSt.StatusKey
			inner join UserRoleMaster ur on pm.RoleKey=ur.RoleKey
			where ur.RoleKey=@roleKey and pm.ProjectStage=@projectstatus
		end
		else if(@entity='ListofProjects')
		begin
			select distinct e.EntityName,
					pm.PermissionKey,
					st.StatusName ProjectStage
			from PermissionMapping pm inner join Entity e on pm.EntityId=e.EntityId
			left join Status st on pm.ProjectStage=st.StatusKey
			inner join UserRoleMaster ur on pm.RoleKey=ur.RoleKey
			where ur.RoleKey=@roleKey  and e.EntityName in('Project','Quote')


		end
		else if(@entity='FDAList')
		begin
			
			select @projectstatus =  WorkflowStage from Projects p inner join Quotes q on p.OpportunityId=q.OpportunityId 
			where q.QuoteId=@id
			--select @projectstage
			select distinct e.EntityName,
					pm.PermissionKey,
					isnull(grpSt.StatusName,'') GroupStatus
			from PermissionMapping pm inner join Entity e on pm.EntityId=e.EntityId
			left join Status grpSt on pm.GroupStatus=grpSt.StatusKey
			inner join UserRoleMaster ur on pm.RoleKey=ur.RoleKey
			where ur.RoleKey=@roleKey and pm.ProjectStage=@projectstatus and pm.EntityId in(5,3)
		end
		else if(@entity='FDA')
		begin
			
			set @groupid=cast(@id as int)
			set @groupStatus = (select distinct(WorkflowStatus) from GroupConfiguration where GroupId=@groupid);
			set @buildingId  = (select distinct(BuildingId) from GroupConfiguration where GroupId=@groupid);
			select @projectid= q.opportunityid from Quotes q inner join Building b on q.Quoteid=b.Quoteid where b.id=@buildingid
			select @projectstatus=workflowstage from projects where opportunityid=@projectid
			select distinct e.EntityName,
					pm.PermissionKey,
					isnull(grpSt.StatusName,'') GroupStatus
			from PermissionMapping pm inner join Entity e on pm.EntityId=e.EntityId
			left join Status grpSt on pm.GroupStatus=grpSt.StatusKey
			inner join UserRoleMaster ur on pm.RoleKey=ur.RoleKey
			where ur.RoleKey=@roleKey and pm.ProjectStage=@projectstatus and pm.EntityId=5 and pm.GroupStatus=@groupstatus
		end
		else if(@entity='ReleaseInfoList')
		begin
			
			select @projectstatus =  WorkflowStage from Projects p inner join Quotes q on p.OpportunityId=q.OpportunityId 
			where q.QuoteId=@id
			--select @projectstage
			select distinct e.EntityName,
					pm.PermissionKey,
					isnull(grpSt.StatusName,'') GroupStatus
			from PermissionMapping pm inner join Entity e on pm.EntityId=e.EntityId
			left join Status grpSt on pm.GroupStatus=grpSt.StatusKey
			inner join UserRoleMaster ur on pm.RoleKey=ur.RoleKey
			where ur.RoleKey=@roleKey and pm.ProjectStage=@projectstatus and pm.EntityId=8
		end
		else if(@entity='ReleaseInfo')
		BEGIN
			
			set @groupid=cast(@id as int)
			set @groupStatus = (select distinct(WorkflowStatus) from GroupConfiguration where GroupId=@groupid);
			set @buildingId  = (select distinct(BuildingId) from GroupConfiguration where GroupId=@groupid);
			set @buildingStatus  = (select distinct WorkflowStatus from Building where Id=@buildingId);
			select @projectid= q.opportunityid from Quotes q inner join Building b on q.Quoteid=b.Quoteid where b.id=@buildingid
			select @projectstatus=workflowstage from projects where opportunityid=@projectid
			If Exists(select '*' from PermissionMapping pm inner join Entity e on pm.EntityId=e.EntityId
			left join Status grpSt on pm.GroupStatus=grpSt.StatusKey
			inner join UserRoleMaster ur on pm.RoleKey=ur.RoleKey
			where ur.RoleKey=@roleKey and pm.ProjectStage=@projectstatus and pm.EntityId=8 
			and pm.BuildingStatus=@buildingstatus and pm.GroupStatus=@groupstatus)
			Begin
			select distinct e.EntityName,
					pm.PermissionKey,
					isnull(grpSt.StatusName,'') GroupStatus
			from PermissionMapping pm inner join Entity e on pm.EntityId=e.EntityId
			left join Status grpSt on pm.GroupStatus=grpSt.StatusKey
			inner join UserRoleMaster ur on pm.RoleKey=ur.RoleKey
			where ur.RoleKey=@roleKey and pm.ProjectStage=@projectstatus and pm.EntityId=8 
			and pm.BuildingStatus=@buildingstatus and pm.GroupStatus=@groupstatus
			End
			Else
			Begin
			select 'Release Info' as EntityName, 'RI_READONLY' as PermissionKey, 'Factory Released' as GroupStatus
			End

		END
		--select isnull(@editPermission,'') EditPermission,isnull(@addPermission,0) AddPermission
		--select * from Building
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