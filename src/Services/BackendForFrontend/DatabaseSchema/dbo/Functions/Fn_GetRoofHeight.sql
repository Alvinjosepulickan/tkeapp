Create FUNCTION [dbo].[Fn_GetRoofHeight]( @buildingId int)

RETURNS Nvarchar(200)

Begin

    DECLARE @Result nvarchar(200)

	declare @tmp varchar(max)
	SET @tmp = ''

  select top 1 @tmp = @tmp +  cast( cast ((select (FloorToFloorHeightFeet*12) + FloorToFloorHeightInch ) as int) as nvarchar(50)) + ','  from BuildingElevation
    where BuildingId=@buildingId order by cast(FloorDesignation as int) desc


   SET @Result = (Select (SUBSTRING(@tmp, 0, LEN(@tmp))))
  

    RETURN @Result

end