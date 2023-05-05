
 
CREATE PROCEDURE [dbo].[usp_GetUserDetails]   --'nagaraj.manjappa@tke.dev' --'aravind.chakragiri@tke.dev'
	-- Add the parameters for the stored procedure here
	@guid ListOfGuids READONLY

AS   
BEGIN
	BEGIN TRY
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;		

		-- Insert statements for procedure here
		SELECT b.Branch as locationName, b.BranchID as locationId,TRIM(b.City) as City,TRIM(b.State) as [State]  ,TRIM(b.Region) as country 
		from Branch b where b.BranchNumber = (select top 1 BranchNumber from AzureGuidBranchMapping where AzureGuid in (select * from @guid))

		SELECT r.Rolekey as roleKey,r.RoleName as roleName 
		from [UserRoleMaster] r where r.RoleKey = (select top 1 RoleKey from AzureGuidRoleMapping where AzureGuid in (select * from @guid)) 

	END TRY
	BEGIN CATCH
	 EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
END