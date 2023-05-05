CREATE FUNCTION [dbo].[Fn_GetDoorVariableFromDoors](@UnitId int,@DoorType nvarchar(250))

RETURNS nvarchar(500)
Begin
 DECLARE @Result nvarchar(500)
 SET @Result = (
 select distinct(DoorValue) from Doors 
	where UnitId = @UnitId and  
		 DoorType like ('%'+@DoorType+'%'))
RETURN @Result

end
