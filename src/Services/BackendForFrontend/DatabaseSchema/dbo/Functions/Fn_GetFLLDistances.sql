Create FUNCTION [dbo].[Fn_GetFLLDistances]( @buildingId int)

RETURNS Nvarchar(200)

Begin

    DECLARE @Result nvarchar(200)

	  declare @tmp varchar(max)
	SET @tmp = ''

      Select @tmp = @tmp +  cast( cast ((select (FloorToFloorHeightFeet*12) + FloorToFloorHeightInch ) as int) as nvarchar(50)) + ','   from BuildingElevation
   where BuildingId=@buildingId

   SET @Result = (Select (SUBSTRING(@tmp, 0, LEN(@tmp))))
  


    RETURN @Result

end