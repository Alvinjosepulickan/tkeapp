
CREATE FUNCTION [dbo].[Fn_GetEntranceRearOpeningForLD]( @groupId int,@unitId int,@variableId nvarchar(100))

RETURNS nvarchar(200)

Begin

    DECLARE @Result nvarchar(200)

	 declare @tmp varchar(250)
   SET @tmp = ''


 Select  @tmp = @tmp + a.VariableValue  + ', ' from(
 select distinct VariableValue   from EntranceConsole eco
   Left Join EntranceLocations el on el.EntranceConsoleId=eco.EntranceConsoleId
   Left Join EntranceConfiguration ecc on ecc.EntranceConsoleId= eco.EntranceConsoleId 
   Left join units ut on ut.SetId=eco.SetId
    where eco.SetId in (Select distinct SetId from units where GroupConfigurationId=@groupId) 
     and el.Rear=1 and el.IsDeleted=0   and ecc.VariableType like   '%'+@variableId+'%'  and ut.UnitId=@unitId)a

	 
 

    SET @Result = (select SUBSTRING(@tmp, 0, LEN(@tmp)))



    RETURN @Result

end