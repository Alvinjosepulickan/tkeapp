CREATE PROCEDURE [dbo].[usp_GetBuildingEquipmentAllGroups]  
@buildingId INT
AS
BEGIN
	BEGIN TRY
	declare @GroupMapping table(groupId INT, groupName NVARCHAR(500))
      
		  Insert into @GroupMapping 
		  (
			 GroupId
			,GroupName
		  )
		select distinct
                              
			g.GroupId
			,g.GroupName
                             
		from
			GroupConfiguration g  
			inner join
				Units u 
				on g.GroupId = u.GroupConfigurationId 
			--inner join
			--    doors d 
			--    on g.GroupId = d.GroupConfigurationId 
			--inner join
			--    HallRiser h 
			--    on g.GroupId = h.GroupConfigurationId 
			--inner join
			--    ControlLocation cl 
			--    on g.GroupId = cl.GroupConfigurationId 
			inner join
				OpeningLocation o 
				on g.GroupId = o.GroupConfigurationId 
			inner join
				GroupHallFixtureConsole gh 
				on g.GroupId = gh.groupid 
				Where g.BuildingId = @buildingId and g.IsDeleted=0 and   o.UnitId <> 0  
		group by
			g.GroupId ,g.GroupName
		having
			count(o.openinglocationid) > 0 

				Select * from @GroupMapping
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
