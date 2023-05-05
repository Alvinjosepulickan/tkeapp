-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetBuildingIdByGroupId] 
	-- Add the parameters for the stored procedure here
	@groupConfigurationId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select distinct(BuildingId) as BuildingId from GroupConfiguration where GroupId=@groupConfigurationId
END