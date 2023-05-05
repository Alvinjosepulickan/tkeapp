

 

CREATE Procedure [dbo].[usp_GetCarCallCutoutKeyswitchOpenings] 
@setId int


as
Begin
	BEGIN TRY
	   DECLARE @groupConfigurationId int
	   declare @isSaved bit
	   SET @groupConfigurationId = (SELECT DISTINCT GroupConfigurationId FROM Units WHERE SetId = @setId)

	   if(Exists(select * from CarCallCutoutLocations where SetId=@setId and IsDeleted=0))
	   begin
   
		set @isSaved=1
	   select distinct 
	  el.FloorNumber,openings.FloorDesignation,el.Front,el.Rear,
	  openings.OpeningFront,openings.openingRear,FrontOpening,RearOpening
	  from CarCallCutoutLocations el
	  join (select distinct FloorDesignation,FloorNumber,Front as OpeningFront,Rear as openingRear,FrontOpening,RearOpening 
	   from OpeningLocation ol
		   Left Join Units us on ol.UnitId=us.UnitId where ol.GroupConfigurationId=@groupConfigurationId and us.SetId= @setId and ol.IsDeleted = 0)as Openings
	  on el.FloorNumber=Openings.FloorNumber
	  where el.setId=@setId and el.IsDeleted=0
	  order by el.FloorNumber desc
	   end
	   else
	   begin
 
	   set @isSaved=0
	   select distinct  FloorNumber,FloorDesignation,FrontOpening,RearOpening,CASE WHEN Front = 1 THEN 0 ELSE 0 END AS Front, CASE WHEN Rear = 1 THEN 0 ELSE 0 END AS Rear, Front as OpeningFront,Rear as openingRear
	   from OpeningLocation ol
		   Left Join Units us on ol.UnitId=us.UnitId where ol.GroupConfigurationId=@groupConfigurationId and us.SetId= @setId and ol.IsDeleted=0
	   order by FloorNumber desc
	   end


	   select floorNumber from BuildingElevation where BuildingId in (select BuildingId from Groupconfiguration where GroupId = @groupConfigurationId) and MainEgress=1
	   select @isSaved isSaved
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
