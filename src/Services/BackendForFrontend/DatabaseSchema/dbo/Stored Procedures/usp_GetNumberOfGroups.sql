CREATE PROCEDURE [dbo].[usp_GetNumberOfGroups] 

       @buildingid int

AS

BEGIN
	BEGIN TRY
		declare @GroupNameCheck int
		declare @GroupCount int
		declare @GroupName nvarchar(10)
		set @GroupCount=(SELECT count(*)  from [dbo].[GroupConfiguration]  where BuildingId=@buildingid)
		set @GroupNameCheck=1
		while(@GroupNameCheck=1)
		begin
			set @GroupNameCheck =0
			set @GroupCount+=1
			set @GroupName='B'+CAST(@GroupCount as nvarchar(3))
					if(exists (select * from GroupConfiguration where GroupName=@GroupName and BuildingId=@buildingid))
					begin
						set @GroupNameCheck=1
					end
					if(@GroupNameCheck=0)
					begin
						select @GroupCount-1
						return 0
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
