
-- =============================================

-- Author: Harshada

-- Create date: <Create Date,,>

-- Description: <Description,,>

-- =============================================

CREATE PROCEDURE [dbo].[usp_GetListOfProjectsForUser]

 -- Add the parameters for the stored procedure here

 @id int

AS

BEGIN
	BEGIN TRY
	 -- SET NOCOUNT ON added to prevent extra result sets from

	 -- interfering with SELECT statements.

	 SET NOCOUNT ON;



		-- Insert statements for procedure here

	 SELECT p.[id],p.[name]

		  ,p.[locationId], l.city, l.[state],l.country

		  ,[clientId], c.[name] as clientName

		  ,[contactPersonId], u.firstName, u.lastName

		  ,[currency], [price], p.[completedDate],p.[bidDate],p.[bookDate]

	   ,p.[createdBy],uf.firstName as createdByFN,uf.lastName as createdByLN,p.[createdOn]

	   ,p.[modifiedBy] ,uf1.firstName as modifiedByFN,uf1.lastName as modifiedByLN,p.[modifiedOn]

	  FROM [dbo].[ProjectsData] p

	  inner join [Location] l on l.id = p.locationId

	  inner join [Client] c on c.id = p.clientId

	  inner join [UserInfo] u on u.id = p.contactPersonId

	  inner join [UserInfo] uf on uf.id = p.createdBy

	  inner join [UserInfo] uf1 on uf1.id = p.modifiedBy

	  where p.createdBy = @id and p.[isDeleted] = 0

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
