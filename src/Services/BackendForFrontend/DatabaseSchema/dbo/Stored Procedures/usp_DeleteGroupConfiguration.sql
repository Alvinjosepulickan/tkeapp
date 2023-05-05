
CREATE PROCEDURE [dbo].[usp_DeleteGroupConfiguration]



 @GroupConfigId int



,@Result int OUTPUT



AS



BEGIN


	BEGIN TRY
		set nocount on



		IF EXISTS (SELECT GroupId from GroupConfiguration where GroupId = @GroupConfigId)



		 BEGIN TRY



		  update GroupConfiguration set IsDeleted=1 where GroupId = @GroupConfigId;



		  IF EXISTS (SELECT GroupId from GroupConfigurationDetails where GroupId = @GroupConfigId)



		 update GroupConfigurationDetails set IsDeleted=1 where GroupId=@GroupConfigId;


		  IF EXISTS (SELECT GroupConfigurationId from OpeningLocation where GroupConfigurationId = @GroupConfigId)



		 update OpeningLocation set IsDeleted=1 where GroupConfigurationId=@GroupConfigId;



		  IF EXISTS (SELECT GroupConfigurationId from Units where GroupConfigurationId = @GroupConfigId)

		 update Units set IsDeleted=1 where GroupConfigurationId=@GroupConfigId;



		  IF EXISTS (SELECT GroupConfigurationId from Doors where GroupConfigurationId = @GroupConfigId)

		 update Doors set IsDeleted=1 where GroupConfigurationId=@GroupConfigId;


		 IF EXISTS (SELECT GroupId from BldgEquipmentGroupMapping where GroupId = @GroupConfigId)

		 delete from BldgEquipmentGroupMapping where GroupId = @GroupConfigId;


		  IF EXISTS (SELECT GroupConfigurationId from HallRiser where GroupConfigurationId = @GroupConfigId)

		 update HallRiser set IsDeleted=1 where GroupConfigurationId=@GroupConfigId;

		 --update workflow status
				declare @buildingid int,@quoteId nvarchar(100)
				select @buildingid= BuildingId from GroupConfiguration where GroupId=@GroupConfigId
				select @quoteId=QuoteId from Building where id=@buildingid
				exec [dbo].[usp_UpdateWorkflowStatus]@buildingid,'building'
				EXEC [dbo].[usp_UpdateWorkflowStatus] @quoteId,'quote'
		

		 SET @Result=1



		  END TRY



		  BEGIN CATCH



		 SET @Result=0



		  END CATCH
	END TRY
	BEGIN CATCH
	EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo=@GroupConfigId;
	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH


  END

