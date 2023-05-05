 
CREATE FUNCTION [dbo].[Fn_GetENTFForLD]( @groupId int,@unitId int)

RETURNS nvarchar(200)

Begin

    DECLARE @Result nvarchar(200)

	 
 declare @tmp varchar(250)
   SET @tmp = ''


	Select  @tmp = @tmp +  (case when Front = 1 then 'TRUE' else'FALSE' end)  + ', '  from OpeningLocation
	   where GroupConfigurationId =@groupId and IsDeleted=0 and UnitId=@unitId



    SET @Result = (select SUBSTRING(@tmp, 0, LEN(@tmp)))

    RETURN @Result

end