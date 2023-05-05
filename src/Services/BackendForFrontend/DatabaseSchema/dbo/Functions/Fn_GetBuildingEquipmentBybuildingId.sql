
CREATE FUNCTION [dbo].[Fn_GetBuildingEquipmentBybuildingId]( @buildingId int)

RETURNS INT

Begin

    DECLARE @Result nvarchar(200)

    SET @Result = (
        
                 
    case when (select distinct
                              
	case
        when
            isnull(g.GroupId, 0) > 0 
        then
            cast(1 as bit) 
        else
            cast(0 as bit) 
        end
        isBuildingEquipment 
                             
    from
        GroupConfiguration g  
        inner join
            Units u 
            on g.GroupId = u.GroupConfigurationId 
        inner join
        --    doors d 
        --    on g.GroupId = d.GroupConfigurationId 
        --inner join
        --    HallRiser h 
        --    on g.GroupId = h.GroupConfigurationId 
        --inner join
        --    ControlLocation cl 
        --    on g.GroupId = cl.GroupConfigurationId 
        --inner join
            OpeningLocation o 
            on g.GroupId = o.GroupConfigurationId 
        inner join
            GroupHallFixtureConsole gh 
            on g.GroupId = gh.groupid 
			Where g.BuildingId = @buildingId and g.IsDeleted=0 --and   o.UnitId <> 0  
    group by
        g.GroupId ,g.GroupName
    having
        count(o.openinglocationid) > 0 ) is null then 0 else 
		 
		 (select distinct
                              
	case
        when
            isnull(g.GroupId, 0) > 0 
        then
            cast(1 as bit) 
        else
            cast(0 as bit) 
        end
        isBuildingEquipment 
                             
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
        count(o.openinglocationid) > 0 ) end
		)

    RETURN @Result

end   