

CREATE PROCEDURE [dbo].[usp_SaveUnitHallFixtureConfiguration(backup)]
	-- Add the parameters for the stored procedure here

	@SetId INT,
	@ConsoleNumber INT,
	@UnitHallFixtureConsoleVariables AS UnitHallFixtureConsoleInfoDataTable READONLY,
	@UnitHallFixtureConfigurationVariables AS EntranceConfigurationDataTable READONLY,
	@UnitHallFixtureLocationVariables AS EntranceLocationDataTable READONLY,
	@CreatedBy NVARCHAR(50),
	@Result INT OUTPUT
AS
BEGIN
declare @FixtureType nvarchar(100);
set @FixtureType= (select FixtureType from @UnitHallFixtureConsoleVariables)
    IF(EXISTS (SELECT * FROM UnitHallFixtureConsole ec WHERE ec.ConsoleNumber=@ConsoleNumber AND ec.SetId=@SetId AND ec.FixtureType = 
	@FixtureType))
	BEGIN
		 
		/****** Hall Lantern Console******/

		UPDATE UnitHallFixtureConsole 
		  SET Name = ecv.ConsoleName
		     ,IsController= ecv.IsController
			 ,ModifiedBy =@CreatedBy 
			 ,ModifiedOn = GETDATE()
			 
		    FROM @UnitHallFixtureConsoleVariables ecv  
			    WHERE ConsoleNumber = @ConsoleNumber AND SetId = @SetId AND UnitHallFixtureConsole.FixtureType = @FixtureType


		/****** Hall Lantern Configuration******/

        DELETE FROM UnitHallFixtureConfiguration 
		   WHERE UnitHallFixtureConsoleId in
		   (
		      SELECT DISTINCT UnitHallFixtureConsoleId 
			      FROM UnitHallFixtureConsole 
				    WHERE ConsoleNumber =@ConsoleNumber AND SetId=@SetId AND FixtureType = @FixtureType
			)


		INSERT INTO UnitHallFixtureConfiguration
		   (
		     UnitHallFixtureConsoleId
			,VariableType
			,VariableValue
			,CreatedBy
			,CreatedOn
			,ModifiedBy
			,ModifiedOn
			)

		 SELECT ec.UnitHallFixtureConsoleId
				,VariableType
				,VariableValue
				,ec.CreatedBy
				,ec.CreatedOn
				,@CreatedBy
				,GETDATE()
			FROM @UnitHallFixtureConfigurationVariables ecv 
				LEFT JOIN UnitHallFixtureConsole ec on ecv.EntranceConsoleId = ec.ConsoleNumber
					WHERE ecv.EntranceConsoleId = @ConsoleNumber AND ec.SetId = @SetId AND FixtureType =@FixtureType


		/****** Entrance Location******/

        DELETE FROM UnitHallFixtureLocations 
		   WHERE UnitHallFixtureConsoleId in
		   (
		      SELECT DISTINCT UnitHallFixtureConsoleId 
			      FROM UnitHallFixtureConsole 
				    WHERE ConsoleNumber =@ConsoleNumber AND SetId=@SetId AND FixtureType=@FixtureType
			)


		INSERT INTO UnitHallFixtureLocations
		  (
		    UnitHallFixtureConsoleId
		   ,FloorNumber
		   ,Front
		   ,Rear
		   ,CreatedBy
		   ,CreatedOn
		   ,ModifiedBy
		   ,ModifiedOn
		   )

         SELECT ec.UnitHallFixtureConsoleId
		       ,elv.FloorNumber
			   ,elv.Front
			   ,elv.Rear
			   ,ec.CreatedBy
			   ,ec.CreatedOn
			   ,@CreatedBy
			   ,GETDATE()
		    FROM @UnitHallFixtureLocationVariables elv 
			  LEFT JOIN UnitHallFixtureConsole ec ON elv.EntranceConsoleId = ec.ConsoleNumber
			     WHERE elv.EntranceConsoleId = @ConsoleNumber AND ec.SetId = @SetId AND FixtureType = @FixtureType
				  
		SET @Result=@SetId
	END
	ELSE
	BEGIN
	   
	    /****** Hall Lantern Console******/
		INSERT INTO UnitHallFixtureConsole
		 (
		  ConsoleNumber
		 ,Name
		 ,FixtureType
		 ,IsController
		 ,SetId
		 ,CreatedBy
		 ,CreatedOn
		 )
        SELECT ConsoleId
		      ,ConsoleName
			  ,FixtureType
			  ,IsController
			  ,@SetId
			  ,@CreatedBy
			  ,getdate() 
			FROM @UnitHallFixtureConsoleVariables  

		
		 /****** Hall Lantern Configuration******/
		INSERT INTO UnitHallFixtureConfiguration
		 (
		   UnitHallFixtureConsoleId
		  ,VariableType
		  ,VariableValue
		  ,CreatedBy
		  ,CreatedOn
		  )
         SELECT ec.UnitHallFixtureConsoleId
		       ,ecv.VariableType
			   ,ecv.VariableValue
			   ,@CreatedBy
			   ,GETDATE()  
		    FROM @UnitHallFixtureConfigurationVariables ecv 
			  LEFT JOIN UnitHallFixtureConsole ec ON ecv.EntranceConsoleId = ec.ConsoleNumber
			  WHERE ecv.EntranceConsoleId = @ConsoleNumber AND ec.SetId = @SetId AND FixtureType = @FixtureType


        /****** Hall Lantern Location******/
	    	INSERT INTO UnitHallFixtureLocations
			 (
			   UnitHallFixtureConsoleId
			  ,FloorNumber
			  ,Front
			  ,Rear
			  ,CreatedBy
			  ,CreatedOn
			 )
         SELECT ec.UnitHallFixtureConsoleId
		       ,elv.FloorNumber
			   ,elv.Front
			   ,elv.Rear
			   ,@CreatedBy
			   ,GETDATE()  
		    FROM @UnitHallFixtureLocationVariables elv 
			  LEFT JOIN UnitHallFixtureConsole ec ON elv.EntranceConsoleId = ec.ConsoleNumber
			     WHERE elv.EntranceConsoleId = @ConsoleNumber AND ec.SetId = @SetId AND FixtureType = @FixtureType
		   
		SET @Result=@SetId
	END
End
