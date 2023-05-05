
CREATE FUNCTION [dbo].[Fn_GetTotalGroupsCount]( @groupid int)

RETURNS INT

Begin

    DECLARE @Result nvarchar(200)

    SET @Result = (
	SELECT count(*)
			FROM   GroupConfiguration  gc
				   Where  gc.GroupId =@groupid
	)

    RETURN @Result

end
