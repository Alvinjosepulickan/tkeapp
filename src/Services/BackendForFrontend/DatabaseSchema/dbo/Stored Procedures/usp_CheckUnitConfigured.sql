CREATE PROC [dbo].[usp_CheckUnitConfigured]
@groupConfigurationId INT,
@Result int output
as
begin
	--if(exists(select * from Units  where GroupConfigurationId=@groupConfigurationId and setId>0 and isDeleted=0))
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
