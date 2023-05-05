CREATE FUNCTION [dbo].[Fn_GetSystemValidationForQuickSummaryScreen](@SetId int,@SystemVariableKeys nvarchar(250))
RETURNS nvarchar(500)
Begin
    DECLARE @Result nvarchar(500)
    SET @Result =
	(
  select distinct SystemVariableValues from [SystemsVariables] 
				where SetId=@SetId and SystemVariableKeys like '%'+@SystemVariableKeys+'%'
	)
    RETURN @Result
end