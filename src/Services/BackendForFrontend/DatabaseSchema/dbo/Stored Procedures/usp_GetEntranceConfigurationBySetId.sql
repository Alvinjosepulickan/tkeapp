
 
 --usp_GetEntranceConfigurationBySetId 5
CREATE Procedure [dbo].[usp_GetEntranceConfigurationBySetId] --1158,72
@groupConfigurationId int,
@setId int,
@isJambMounted bit =0

as
Begin

	BEGIN TRY
	   declare @chkJambMounted as bit=0;
	   declare @productName as Nvarchar(50);
	   if(Exists(select * from ControlLocation where GroupConfigurationId=@groupConfigurationId 
	   and ControlLocationType='Parameters_SP.controllerLocation_SP' and ControlLocationValue='Jamb-Mounted' and IsDeleted=0))
	   begin
			set @chkJambMounted=1;
	   end
	 else
	   begin
			if(@isJambMounted = 1)
				begin
					set @chkJambMounted =1
				end
			else
				begin				
					set @chkJambMounted=0;
				end
	   end
	   if(Exists(select * from EntranceConsole where SetId=@setId and IsDeleted=0))
	   begin
	   select @productName=productname from UnitSet where SetId=@setId
	   select isnull(Max(FloorNumber),0) nooffloors,@productName productName from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0

	   select distinct ec.ConsoleNumber EntranceConsoleId,ec.Name EntranceConsoleName,ec.IsController,
	  ecn.VariableType,ecn.VariableValue,
	  el.FloorNumber,openings.FloorDesignation,el.Front,el.Rear,
	  openings.OpeningFront,openings.openingRear,FrontOpening,RearOpening
	  from EntranceConsole ec left join EntranceConfiguration ecn on ec.EntranceConsoleId=ecn.EntranceConsoleId
	  left join EntranceLocations el on ec.EntranceConsoleId=el.EntranceConsoleId
	  join (select distinct FloorDesignation,FloorNumber,Front as OpeningFront,Rear as openingRear,FrontOpening,RearOpening 
	   from OpeningLocation ol
		   Left Join Units us on ol.UnitId=us.UnitId where ol.GroupConfigurationId=@groupConfigurationId and us.SetId= @setId and ol.IsDeleted = 0)as Openings
	  on el.FloorNumber=Openings.FloorNumber
	  where ec.setId=@setId and ec.IsDeleted=0
	  and ec.IsController=(case when @chkJambMounted=0 then 0 else ec.IsController end)

	  order by ec.IsController desc,ec.ConsoleNumber asc,el.FloorNumber desc

	   end
	   else
	   begin
	   declare @nooffloor int
	   declare @consoleId int
	   select @productName= ProductName from UnitSet where setid=@setId
	   select @nooffloor=isnull(Max(FloorNumber),0) from OpeningLocation where GroupConfigurationId=@groupConfigurationId and IsDeleted=0
   
	   select distinct @nooffloor nooffloors, FloorNumber,FloorDesignation,FrontOpening,RearOpening,Front,Rear,cast(0 as bit) IsController,@productName productName 
	   from OpeningLocation ol
		   Left Join Units us on ol.UnitId=us.UnitId where ol.GroupConfigurationId=@groupConfigurationId and us.SetId= @setId and ol.IsDeleted=0
	   order by FloorNumber desc
	   end
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
