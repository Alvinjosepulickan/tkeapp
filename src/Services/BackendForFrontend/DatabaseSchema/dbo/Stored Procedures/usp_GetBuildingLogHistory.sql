CREATE PROCEDURE [dbo].[usp_GetBuildingLogHistory] --24,null
@buildingId int,
@date date
as
begin
	BEGIN TRY
		declare @minDate datetime = isnull((select min(CreatedOn) from BuildingConfigHistory where BuildingId = @buildingId),getdate())
		declare @maxDate datetime = isnull((select max(CreatedOn) from BuildingConfigHistory where BuildingId = @buildingId),getdate())
		declare @lastDate datetime
	
	
		if(@date is null)
		begin
			set @lastDate = dateadd(day, 10, @minDate)
		end
		else
		begin
			select @lastDate=max(CreatedOn) from BuildingConfigHistory 
			where BuildingId = @buildingId and CreatedOn<dateadd(day, 1, @date)
			set @minDate=@lastDate
			set @lastDate = dateadd(day, 10, @minDate)
		end
		if(@lastDate<@maxDate)
		begin
			select @lastDate= [dbo].[Fn_GetLastDateForHistoryBuilding](@buildingId,@minDate,@lastDate)
		end
	

		select BuildingId,VariableId,CurrentValue,PreviousValue,cast(CreatedOn as datetime) CreatedOn,CreatedBy
		from BuildingConfigHistory 
		where CreatedOn<= @lastDate and BuildingId=@buildingId
		order by CreatedOn
		DECLARE @buildingtDesignation nvarchar(100)

		SELECT @buildingtDesignation = BldName
		FROM Building where Id = @buildingId

		select @buildingtDesignation Designation  
		if(exists(select * from BuildingConfigHistory where BuildingId=@buildingId))
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