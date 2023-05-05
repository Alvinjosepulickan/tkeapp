-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[usp_GetLDResponseJson]
	-- Add the parameters for the stored procedure here
	@groupId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select 
		FDAProcessJson as response
	from FieldDrawingAutomationProcessDetails 
	Where IntegratedProcessId in(
		Select 
			Id 
		From FieldDrawingIntegrationMaster 
		Where FieldDrawingIntegrationId In(
			Select
				Top(1) Id  
			From FieldDrawingMaster 
			Where GroupId = @groupId 
			Order By CreatedOn Desc))
END