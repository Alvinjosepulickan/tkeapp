 
CREATE FUNCTION [dbo].[Fn_GetFLRMRKFrontForLD]( @groupId int,@unitId int)

RETURNS nvarchar(200)

Begin

    DECLARE @Result nvarchar(200)

	 
 declare @tmp varchar(250)
   SET @tmp = ''


 
	Select @tmp = @tmp +  be.FloorDesignation  + ', ' 
		from units u
		 Left join GroupConfiguration gc on u.GroupConfigurationId =gc.GroupId
		 Left Join BuildingElevation be on be.BuildingId = gc.BuildingId
	Where u.GroupConfigurationId=@groupId and u.IsDeleted=0 and be.IsDeleted=0 and gc.IsDeleted=0 and u.UnitId=@unitId



    SET @Result = (select SUBSTRING(@tmp, 0, LEN(@tmp)))

    RETURN @Result

end