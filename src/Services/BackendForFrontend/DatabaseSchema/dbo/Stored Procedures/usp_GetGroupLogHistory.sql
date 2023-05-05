CREATE PROCEDURE [dbo].[usp_GetGroupLogHistory]--87,
@GroupId int,
@date date
as
begin
	BEGIN TRY
		declare @minDate datetime = (select min(CreatedOn) from GroupConfigHistory where GroupId = @groupId)
		declare @maxDate datetime = (select max(CreatedOn) from GroupConfigHistory where GroupId = @groupId)
		declare @lastDate datetime
	
	
		if(@date is null)
		begin
			set @lastDate = dateadd(day, 10, @minDate)
		end
		else
		begin
			select @lastDate=max(CreatedOn) from GroupConfigHistory 
			where GroupId = @groupId and CreatedOn<dateadd(day, 1, @date)
			set @minDate=@lastDate
			set @lastDate = dateadd(day, 10, @minDate)
		end
		if(@lastDate<@maxDate)
		begin
			select @lastDate= [dbo].[Fn_GetLastDateForHistoryGroup](@groupId,@minDate,@lastDate)
		end
	
		select GroupId,VariableId,CurrentValue,PreviousValue,cast(CreatedOn as datetime) CreatedOn,CreatedBy
		from GroupConfigHistory 
		where CreatedOn<= @lastDate and GroupId=@groupId 
		order by CreatedOn

		DECLARE @GroupDesignation nvarchar(100)
		SELECT @GroupDesignation = GroupName
		FROM GroupConfiguration where GroupId = @groupId

		select @GroupDesignation Designation  
		if(exists(select * from GroupConfigHistory where GroupId=@groupId))
		begin
			if(@lastDate >= @maxDate)
			begin
				select cast(0 as bit) ShowLoadMore
			end
			else
			begin
				select cast(1 as bit) ShowLoadMore
			end
		end
    END TRY
	BEGIN CATCH
	 EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
end