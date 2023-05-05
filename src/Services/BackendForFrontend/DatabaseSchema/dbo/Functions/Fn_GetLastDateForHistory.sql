CREATE FUNCTION [dbo].[Fn_GetLastDateForHistory]
(	@setId int,
	@minDate datetime,
	@lastDate datetime
)

RETURNS datetime

Begin
	--declare @SQLQuery nvarchar(max), @rowCount int
	--SET @SQLQuery = 'SELECT * FROM ' + @tableName + ' WHERE SetId = @setId and (CreatedOn between @minDate and @lastDate)' 
	--EXECUTE sp_executesql @SQLQuery, @setId,@minDate,@lastDate
	--set @rowCount=@@ROWCOUNT
	declare @maxDate datetime = (select max(CreatedOn) from UnitConfigHistory where SetId = @setId)
	if(not exists(SELECT * FROM unitconfighistory WHERE SetId = @setId and 
	(CreatedOn>@minDate and CreatedOn<=@lastDate)))
	begin
		set @minDate=@lastDate
		set @lastDate = dateadd(day, 10, @minDate)
		if(@lastDate<@maxDate)
		begin
			select @lastDate= [dbo].[Fn_GetLastDateForHistory](@setId,@minDate,@lastDate)
		end
	end
    RETURN @lastDate

end   
