
CREATE FUNCTION [dbo].[Fn_GetTotalUnitsCount](@groupid int)

RETURNS INT

Begin
 
	DECLARE @Result nvarchar(200)

    SET @Result = (
	Select count(*) from Units ut
				    Left Join GroupConfiguration gc on ut.GroupConfigurationId=gc.GroupId
					  Where ut.GroupConfigurationId=@groupid 
	)

    RETURN @Result

end  
