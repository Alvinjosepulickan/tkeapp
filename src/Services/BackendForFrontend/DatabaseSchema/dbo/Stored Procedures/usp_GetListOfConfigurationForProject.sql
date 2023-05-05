CREATE procedure [dbo].[usp_GetListOfConfigurationForProject] --'WE-2021-50000033'
 @QuoteId nvarchar(25)
,@ConstantMapperList as [dbo].[VariableMapper] READONLY
AS
Begin
	BEGIN TRY
	   
	     
	   declare @quotePrice nvarchar(200);
	   declare @unitDispostion nvarchar(200);
	   SET @quotePrice = (Select VariableType from @ConstantMapperList where VariableKey ='QUOTEPRICE')
	   SET @unitDispostion = (Select VariableType from @ConstantMapperList where VariableKey ='UNITDISPOSITION')




	   SELECT DISTINCT bg.projectid,
		bg.QuoteId,
		   bg.CreatedOn as BuildingcreatedDate,
		   bg.id,
			bg.bldname,
		   bg.ConflictStatus as BuildingConflictstatus,
		   isnull(bg.BuildingEquipmentStatus,'BLDGEQP_NA') BuildingEquipmentStatus,
			CASE
			 WHEN gc.groupid IS NULL THEN 0

			 ELSE gc.groupid

		   END AS GroupId,

		   CASE

			 WHEN gc.groupname IS NULL THEN ''

			 ELSE gc.groupname

		   END AS GroupName,

		   (case when (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType ='productCategory' and GroupId=gc.GroupId) = 'Elevator' then 'ELEV' 
		    when (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType ='productCategory' and GroupId=gc.GroupId) = 'Escalator/Moving-Walk'  then 'ESCL'
		    when (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType ='productCategory' and GroupId=gc.GroupId) = 'TWIN Elevator'  then 'TWIN'
		    when (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType ='productCategory' and GroupId=gc.GroupId) = 'Other'  then 'OTHR' end) as productCategory

		   ,CASE

			 WHEN gc.needsvalidation IS NULL THEN 0

			 ELSE gc.needsvalidation

		   END AS NeedsValidation,
		   gc.ConflictStatus as GroupConflictstatus,
		   CASE

			 WHEN ut.unitid IS NULL THEN 0

			 ELSE ut.unitid

		   END AS UnitId,
		   isnull(ut.CreatedOn,GETDATE()) as CreatedOn,
		   ut.ConflictStatus as UnitsConflictstatus,
		   CASE

			 WHEN ut.[designation] IS NULL THEN ''

			 ELSE ut.[designation]

		   END AS UnitName,

		   CASE

			 WHEN ut.[description] IS NULL THEN ''

			 ELSE ut.[description]

		   END AS [Description],

		   CASE

			 WHEN us.productname IS NULL THEN ''

			 ELSE us.productname

		   END AS Product,

		   CASE

			 WHEN us.productname = 'TWIN' THEN (select ConfigureValues from UnitConfiguration UCN where UCN.SetId = us.SetId and ConfigureVariables like '%'+@unitDispostion+'%')

			 ELSE ''

		   END AS UnitPosition,

		   CASE

			 WHEN ut.ueid IS NULL THEN ''

			 ELSE ut.ueid

		   END AS UEID,

			isnull(setID.capacity,'') as capacity,

			isnull(setID.speed,'') as speed ,

		   CASE

			 WHEN ol.nooffloors IS NULL THEN 0

			 ELSE ol.nooffloors

		   END AS Landings,

		   CASE

			 WHEN ol.frontopening IS NULL THEN 0

			 ELSE ol.frontopening

		   END AS FrontOpening,

		   CASE

			 WHEN ol.rearopening IS NULL THEN 0

			 ELSE case when ( exists(select * from doors where UnitId=ut.UnitId and DoorType  like '%rear%' and DoorValue<> '' and DoorValue <> 'NR')) then ol.RearOpening else 0 end --end as RearOpening

		   END AS RearOpening,

		   (case when (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType ='productCategory' and GroupId=gc.GroupId) = 'Elevator'  then 
		   (case when (Select top 1 ProductName from UnitSet where SetId=us.setid) in ('CE_Geared','CE_Gearless','CE_Hydraulic','Synergy') then 
			 (Select ConfigureValues from UnitConfiguration
                 where SetId=us.setid and ConfigureVariables=@quotePrice)
				 else 0 end)
		     when (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType ='productCategory' and GroupId=gc.GroupId) = 'Escalator/Moving-Walk'
			 OR (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType ='productCategory' and GroupId=gc.GroupId) = 'TWIN Elevator'
			 OR (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType ='productCategory' and GroupId=gc.GroupId) = 'Other' then 
			 /* if product category is Escalator/Twin Elevator/Others */
			 (Select ConfigureValues from UnitConfiguration
                 where SetId=us.setid and ConfigureVariables=@quotePrice) end) as Price,

		   /*status section Start*/
		   isnull(unitstatus.StatusKey,'') AS [UnitStatusKey],
		   isnull(unitstatus.StatusName,'') AS [UnitStatusName],
		   isnull(unitstatus.DisplayName,'') AS [UnitStatusDisplayName],
		   isnull(unitstatus.Description,'') AS [UnitStatusDescription],
		   isnull(groupstatus.StatusKey,'') AS [GroupStatusKey],
		   isnull(groupstatus.StatusName,'') AS [GroupStatusName],
		   isnull(groupstatus.DisplayName,'') AS [GroupStatusDisplayName],
		   isnull(groupstatus.Description,'') AS [GroupStatusDescription],
		   isnull(statusbuilding.StatusKey,'') AS [BuildingStatusKey],
		   isnull(statusbuilding.StatusName,'') AS [BuildingStatusName],
		   isnull(statusbuilding.DisplayName,'') AS [BuildingStatusDisplayName],
		   isnull(statusbuilding.Description,'') AS [BuildingStatusDescription],
		   /*status section End*/
		   CASE
			 WHEN us.setid IS NULL THEN 0

			 ELSE us.setid

		   END AS SetId,

		   CASE

			 WHEN us.[name] IS NULL THEN ''

			 ELSE us.[name]

		   END AS SetName,

		   bg.modifiedon

	   FROM building bg

		   LEFT JOIN groupconfiguration gc

			ON bg.id = gc.buildingid

			AND gc.isdeleted = 0

		   LEFT JOIN units ut

			ON ut.groupconfigurationid = gc.groupid

			AND ut.isdeleted = 0

		   LEFT JOIN openinglocation ol

			ON ol.unitid = ut.unitid

			AND ol.isdeleted = 0

		   LEFT JOIN unitset us

			ON us.setid = ut.setid

			AND us.isdeleted = 0

		   --LEFT JOIN @buildingEquipmentStatus bes

			--ON bes.buildingid = bg.id

		   LEFT JOIN (SELECT DISTINCT Isnull(us.setid, 0) AS setId,

				 Isnull([dbo].[Fn_getunitdetailsforunitlistingscreen](us.setid,'CAPACITY'), '') AS Capacity,

				 Isnull([dbo].[Fn_getunitdetailsforunitlistingscreen](us.setid,'SPEED'), '') AS Speed

			FROM building bg

			LEFT JOIN groupconfiguration gc ON bg.id = gc.buildingid AND gc.isdeleted = 0

		   LEFT JOIN units ut ON ut.groupconfigurationid = gc.groupid AND ut.isdeleted = 0

		   LEFT JOIN unitset us ON us.setid = ut.setid AND us.isdeleted = 0

		   WHERE bg.QuoteId =@QuoteId ) setID

			ON setID.setid = us.setid
		   left join Status unitstatus on ut.WorkflowStatus=unitstatus.StatusKey
		   left join Status groupstatus on gc.WorkflowStatus=groupstatus.StatusKey
		   left join Status statusbuilding on bg.workflowstatus=statusbuilding.StatusKey


		WHERE bg.QuoteId = @QuoteId AND bg.isdeleted = 0

		ORDER BY bg.CreatedOn ASC
		select OpportunityId, VersionId from Quotes where QuoteId=@QuoteId
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





