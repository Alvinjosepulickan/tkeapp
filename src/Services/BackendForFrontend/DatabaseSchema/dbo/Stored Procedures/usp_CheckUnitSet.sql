
CREATE Procedure [dbo].[usp_CheckUnitSet]
@unitList as UnitIDList READONLY,
@Result int OUTPUT
as
begin
	BEGIN TRY
		DECLARE @SetConfigrationId int
		Declare @GroupConfigurationId int
		DECLARE @NumberOfUnits int
		set @Result=1
		set @NumberOfUnits=(select count(*) from @unitList)
		if(@NumberOfUnits=1)
		begin
			if((select count(*) from OpeningLocation where UnitId in (select UnitId from @unitList))=0)
			begin
				set @Result=2
				return 0
			end

			set @Result=1
			return 1
		end
		begin try
			set @GroupConfigurationId=(SELECT DISTINCT GroupConfigurationId FROM units WHERE unitId IN (select UnitID from @unitList))
		end try
		begin catch
			set @Result=3
			return 0
		end catch 
		begin try
			SET @SetConfigrationId=(SELECT DISTINCT SetId FROM units WHERE unitId IN (select UnitID from @unitList))
		end try
		begin catch
			set @Result=4
			return 0
		end catch
		SET @Result=1
		if(@NumberOfUnits>1)
		BEGIN
			DECLARE @MinUnitId int
			set @MinUnitId= (select min(UnitID) from @unitList)
			declare @numberOfRows int
			declare @doortype nvarchar(250)
			declare @center nvarchar(250)
			set @center='''Center'''
			set @doortype='''B1P1.Parameters_SP.frontDoorTypeAndHand_SP'''
			set @numberOfRows=(select count(*) from doors where UnitId=@MinUnitId)
			if((select  count(*) from Doors where UnitId=@MinUnitId and  DoorType like '%frontDoorType%' )=0)
			begin
				set @numberOfRows=@numberOfRows+1
			end
			Declare @MaxUnitId int
			set @MaxUnitId= (select max(UnitID) from @unitList)
			declare @sqlQuery nvarchar(max)
			begin try
				set @sqlQuery='create table ##'+cast(@GroupConfigurationId as nvarchar(10))+'Door(
								DoorType nvarchar(max),
								Doorvalue nvarchar(max)
								)'
				exec sp_executesql @sqlquery
				set @sqlQuery='create table ##'+cast(@GroupConfigurationId as nvarchar(10))+'Doorduplicate(
								DoorType nvarchar(max),
								Doorvalue nvarchar(max)
								)'
				exec sp_executesql @sqlquery
			end try
			begin catch
				set @sqlQuery='delete from ##'+cast (@GroupConfigurationId as nvarchar(10))+'Door'
				exec sp_executesql @sqlquery
			end catch
			set @sqlQuery='insert into ##'+cast(@GroupConfigurationId as nvarchar(10))+'Door(DoorType,Doorvalue) '
			while (@MinUnitId<=@MaxUnitId)
			begin
				if (exists (select * from @unitList where UnitID=@MinUnitId))
				begin
					begin 
					if((select  count(*) from Doors where UnitId=@MinUnitId and  DoorType like '%frontDoorType%' )=0)
						set @sqlQuery =@sqlQuery+'select SUBSTRING('+@doortype+',20,len('+@doortype+')-1) as DoorType,'+@center+' as DoorValue union '
					end 
					set @sqlQuery =@sqlQuery+'select SUBSTRING(DoorType,20,len(DoorType)-1) as DoorType,DoorValue from Doors where UnitId='+cast(@MinUnitId as nvarchar(100))+' union '
				end
				set @MinUnitId+=1
			end
			set @sqlQuery=SUBSTRING(@sqlQuery,0,len(@sqlQuery)-4)
			declare @numberOfCoulmnsAfterJoin int
			exec sp_executesql @sqlQuery
			declare @right nvarchar(250)
			set @right='''Right'''
			declare @left nvarchar(250)
			set @left='''Left'''
			set @sqlQuery= N'update  ##'+cast(@GroupConfigurationId as nvarchar(10))+'Door set DoorValue='+@right+' where DoorValue='+@left
			EXEC sp_executesql @sqlQuery
			declare @ParmDefinition nvarchar(50)
			declare @retval int
			set @retval=0
			set @sqlQuery =N'insert into ##'+cast(@GroupConfigurationId as nvarchar(10))+'Doorduplicate select distinct DoorType,DoorValue from ##'+cast(@GroupConfigurationId as nvarchar(10))+'Door'
			EXEC sp_executesql @sqlQuery
			--set @sqlQuery= N'SELECT * FROM ##'+cast(@GroupConfigurationId as nvarchar(10))+'Door'
			--EXEC sp_executesql @sqlQuery
			--set @sqlQuery= N'SELECT * FROM ##'+cast(@GroupConfigurationId as nvarchar(10))+'Doorduplicate'
			--EXEC sp_executesql @sqlQuery
			set @sqlQuery= N'SELECT @retvalOUT =  count(*) FROM ##'+cast(@GroupConfigurationId as nvarchar(10))+'Doorduplicate'
			SET @ParmDefinition = N'@retvalOUT int OUTPUT';
			EXEC sp_executesql @sqlQuery, @ParmDefinition, @retvalOUT=@retval OUTPUT;
			if(@retval!=@numberOfRows)
			begin
				SET @Result=5
				return 0
			end
			else
			begin
				begin try
					set @sqlQuery='create table ##'+cast(@GroupConfigurationId as nvarchar(10))+'OpeningLocation(
									FloorDesignation nvarchar(3),
									Front bit,
									Rear bit,
									Side bit
									)'
					exec sp_executesql @sqlquery
				end try
				begin catch
					set @sqlQuery='delete from ##'+cast (@GroupConfigurationId as nvarchar(10))+'OpeningLocation'
					exec sp_executesql @sqlquery
				end catch
				set @sqlQuery='insert into ##'+cast(@GroupConfigurationId as nvarchar(10))+'OpeningLocation(FloorDesignation,Front,Rear,Side) '
				set @MinUnitId= (select min(UnitID) from @unitList)
				set @MaxUnitId= (select max(UnitID) from @unitList)
				SET @numberOfRows=(SELECT COUNT(*) FROM OpeningLocation WHERE UnitId=@MinUnitId )
				while (@MinUnitId<=@MaxUnitId)
				begin
					if (exists (select * from @unitList where UnitID=@MinUnitId))
					begin
						if((select count(*) from OpeningLocation where UnitId =@MinUnitId)=0)
						begin
							set @Result=6
							return 0
						end
						set @sqlQuery =@sqlQuery+'select FloorDesignation,Front,Rear,Side from OpeningLocation where UnitId='+cast(@MinUnitId as nvarchar(100))+' union '
					end
					set @MinUnitId+=1
				end
				set @sqlQuery=SUBSTRING(@sqlQuery,0,len(@sqlQuery)-5)
				exec sp_executesql @sqlQuery
				set @retval=0
				set @sqlQuery= N'SELECT @retvalOUT = count(*) FROM ##'+cast(@GroupConfigurationId as nvarchar(10))+'OpeningLocation'
				SET @ParmDefinition = N'@retvalOUT int OUTPUT';
				EXEC sp_executesql @sqlQuery, @ParmDefinition, @retvalOUT=@retval OUTPUT;
				if(@retval!=@numberOfRows)
				begin
					SET @Result=7
					return 0
				end
			end
			SET @Result=1
			
			RETURN 1
		END
	END TRY
	BEGIN CATCH
		SET @Result=8
		EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID;

	declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH
end
