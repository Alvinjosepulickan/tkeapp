
CREATE PROC [dbo].[usp_CheckGroupExist]
@buildingConfigurationId INT,
@Result int output
as
begin
	--if(exists(select * from GroupConfiguration  where BuildingId=@buildingConfigurationId and isDeleted=0))
	--begin
	--	set @Result=1;
	--	return 1
	--end
	--else
	--begin
		set @Result=0;
		return 0
	--end
end
