
CREATE PROCEDURE [dbo].[usp_UpdateOpeningLocation] 
	@GroupConfigurationId int,
	@OpeningLocationDataTable  AS OpeningLocationDataTableType READONLY ,
	@historyTable as HistoryTable readonly,
	@Result INT OUTPUT
	AS 
BEGIN 
  
  BEGIN TRY
	DECLARE @CreatedBy [nvarchar](50)
	DECLARE @date [datetime]
	SET @CreatedBy=(SELECT DISTINCT(CreatedBy) FROM OpeningLocation where GroupConfigurationId=@GroupConfigurationId )
	SET @date=(SELECT max(CreatedOn) FROM OpeningLocation where GroupConfigurationId=@GroupConfigurationId )
	DELETE FROM OpeningLocation where GroupConfigurationId=@GroupConfigurationId
	insert into OpeningLocation ([GroupConfigurationId],[UnitId],[Travelfeet],[NoOfFloors],[TravelInch],[OcuppiedSpaceBelow],[FrontOpening],[RearOpening],[SideOpening],[FloorDesignation],[FloorNumber],[Front],[Side],[Rear],[CreatedBy]) SELECT [GroupConfigurationId],[UnitId],[Travelfeet],[NoOfFloors],[TravelInch],[OcuppiedSpaceBelow],[FrontOpening],[RearOpening],[SideOpening],[FloorDesignation],[FloorNumber],[Front],[Side],[Rear],[UserName] FROM @OpeningLocationDataTable
	DELETE FROM OpeningLocation where GroupConfigurationId=@GroupConfigurationId
	insert into OpeningLocation ([GroupConfigurationId],[UnitId],[Travelfeet],[NoOfFloors],[TravelInch],[OcuppiedSpaceBelow],[FrontOpening],[RearOpening],[SideOpening],[FloorDesignation],[FloorNumber],[Front],[Side],[Rear],[CreatedBy]) SELECT [GroupConfigurationId],[UnitId],[Travelfeet],[NoOfFloors],[TravelInch],[OcuppiedSpaceBelow],[FrontOpening],[RearOpening],[SideOpening],[FloorDesignation],[FloorNumber],[Front],[Side],[Rear],[UserName] FROM @OpeningLocationDataTable
	UPDATE OpeningLocation SET GroupConfigurationId=@GroupConfigurationId,CreatedBy=@CreatedBy,CreatedOn=@date,ModifiedBy=@CreatedBy,ModifiedOn=GETDATE() WHERE unitId IN (SELECT [unitId] FROM @OpeningLocationDataTable)

	--Updating workflow status
	UPDATE GroupConfiguration set WorkflowStatus='GRP_COM' where GroupId=@GroupConfigurationId and WorkflowStatus<>'GRP_CINV'

	/**HistoryTable**/

		insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn)
		select @GroupConfigurationId,variableId,Updatedvalue,PreviousValue,@CreatedBy,getdate(),@CreatedBy,getdate()
		from @historyTable

	/**HistoryTable**/
	
	SET @Result=1
  END TRY
  BEGIN CATCH
	SET  @Result=0
	EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';
  declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
  END
