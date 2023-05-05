


CREATE FUNCTION [dbo].[Fn_GetUnitDetailsForUnitListingScreen](@SetId int,@ConfigVariable nvarchar(250))







RETURNS nvarchar(500)







Begin







    DECLARE @Result nvarchar(500)







    SET @Result = (







  select ConfigureValues from UnitConfiguration where UnitConfigurationId=(select max(UnitConfigurationId) from UnitConfiguration where SetId=@SetId and ConfigureVariables like '%'+@ConfigVariable+'%')







 )







    RETURN @Result







end
