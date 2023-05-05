CREATE PROCEDURE [dbo].[usp_GetUnitLogHistory]--23,'2021-02-17'
@setId int,
@date date,
@unitId int
as
begin
	BEGIN TRY
		declare @minDate datetime = (select min(CreatedOn) from UnitConfigHistory where SetId = @setId)
		declare @maxDate datetime = (select max(CreatedOn) from UnitConfigHistory where SetId = @setId)
		declare @lastDate datetime
	
	
		if(@date is null)
		begin
			set @lastDate = dateadd(day, 10, @minDate)
		end
		else
		begin
			select @lastDate=max(CreatedOn) from UnitConfigHistory 
			where SetId = @setId and CreatedOn<dateadd(day, 1, @date)
			set @minDate=@lastDate
			set @lastDate = dateadd(day, 10, @minDate)
		end
		if(@lastDate<@maxDate)
		begin
			select @lastDate= [dbo].[Fn_GetLastDateForHistory](@setId,@minDate,@lastDate)
		end
	
		--select @lastDate
		select SetId,VariableId,CurrentValue,PreviousValue,cast(CreatedOn as datetime) CreatedOn,CreatedBy
		from UnitConfigHistory 
		where CreatedOn<= @lastDate and SetId=@setId 
		order by CreatedOn

		DECLARE @unitDesignation nvarchar(100)
		--SELECT @unitDesignation = COALESCE(@unitDesignation + ', ', '') + Designation
		--FROM Units where SetId=@setId

		SELECT @unitDesignation = Designation
		FROM Units where UnitId = @unitId

		select @unitDesignation Designation  
		if(exists(select * from UnitConfigHistory where setId=@setId))
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