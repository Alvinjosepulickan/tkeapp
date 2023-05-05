 
CREATE Procedure [dbo].[usp_GetDrawings] --'US-2021-00000089'--'US-2021-00000001'--'US-2021-000008'  --'ADIA-1N16VG8'
 @OpportunityId nvarchar(25)
,@ConstantMapperList as [dbo].[VariableMapper] READONLY
AS
Begin
  
  declare @projectStatus nvarchar(200)
  declare @quoteStatus nvarchar(200)
  declare @isPrimaryQuote int

	SET @isPrimaryQuote = (case when exists(select * from PrimaryQuotes where primaryQuoteId = @OpportunityId) then 1 else 0 end)

    SET @projectStatus =(Select top 1 case when WorkflowStage is null then '' else WorkflowStage end as statusKey from Projects
    Where OpportunityId in(
		Select distinct top 1 OpportunityId from Quotes
		   Where QuoteId=@OpportunityId
		   ))
	SET @quoteStatus = (select QuoteStatusId from Quotes where QuoteId = @OpportunityId)

     DECLARE @BookCSC INT
	 DECLARE @coordinationStatus INT
     set @BookCSC=1
	 set @coordinationStatus = 1

     DECLARE db_cursor1 CURSOR FOR Select 
	                         gc.GroupId as groupsId
                           ,(Select top 1 Id from [dbo].[FieldDrawingMaster]
								Where GroupId = gc.GroupId
								order by createdOn desc) as id
                           ,(select top 1 IsLocked from FieldDrawingMaster 
						   where GroupId = gc.groupid order by CreatedOn desc) as  lock
						     ,(select top 1 StatusKey from FieldDrawingMaster 
						   where GroupId = gc.groupid order by CreatedOn desc) as statusId

               from building bg
              Left Join GroupConfiguration gc on bg.Id=gc.BuildingId
               Where QuoteId= @OpportunityId and gc.IsDeleted=0 and bg.IsDeleted=0 and id <> ''
     DECLARE @groupsId INT
     DECLARE @id INT
     DECLARE @lock INT
	 DECLARE @statusId NVARCHAR(30)
     OPEN db_cursor1;
     FETCH NEXT FROM db_cursor1 INTO @groupsId, @id, @lock ,@statusId;
     WHILE @@FETCH_STATUS = 0
     BEGIN
         IF @lock = 0 or @lock IS NULL or @statusId = 'DWG_PEN' OR   @statusId = 'DWG_ERR'
         begin
        set @BookCSC = 0
         end
         FETCH NEXT FROM db_cursor1 INTO @groupsId, @id, @lock,@statusId;
     END;
     CLOSE db_cursor1;
     DEALLOCATE db_cursor1;
 
     --setting send to coordination flag to enable or disable button
	 DECLARE db_cursor CURSOR FOR select WorkFlowStatus from groupconfiguration where buildingId in (select id from building where quoteId =@OpportunityId and IsDeleted = 0) and IsDeleted=0
	 DECLARE @workFlowStatus nvarchar(50)
     OPEN db_cursor;
     FETCH NEXT FROM db_cursor INTO  @workFlowStatus;
     WHILE @@FETCH_STATUS = 0
     BEGIN
		 
		 IF @workFlowStatus <> 'GRP_LOC' 
		 Begin
			set @coordinationStatus = 0
		 END
         FETCH NEXT FROM db_cursor INTO  @workFlowStatus;
     END;
     CLOSE db_cursor;
     DEALLOCATE db_cursor;


	   declare @productCategoryId nvarchar(200);
	   SET @productCategoryId = (Select VariableType from @ConstantMapperList where VariableKey ='PRODUCTCATEGORY')

      Select bg.id as buildingId
          ,bg.BldName as buildingName
    ,gc.GroupId as groupId
    ,gc.GroupName as groupName
	, (case when (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType =@productCategoryId and GroupId=gc.GroupId) = 'Elevator' then 'ELEV' 
		   when (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType =@productCategoryId and GroupId=gc.GroupId) = 'Escalator/Moving-Walk'  then 'ESCL'
		    when (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType =@productCategoryId and GroupId=gc.GroupId) = 'TWIN Elevator'  then 'TWIN'
			when (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType =@productCategoryId and GroupId=gc.GroupId) = 'Other'  then 'OTHR' end) as productKey


	 ,(case when (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType =@productCategoryId and GroupId=gc.GroupId) = 'Elevator' AND s.StatusKey = 'DWG_NA'
		       OR (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType =@productCategoryId and GroupId=gc.GroupId) = 'Escalator/Moving-Walk'  AND s.StatusKey = 'DWG_NA'
			   OR (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType =@productCategoryId and GroupId=gc.GroupId) = 'TWIN Elevator' AND s.StatusKey = 'DWG_NA'
			   OR (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType =@productCategoryId and GroupId=gc.GroupId) = 'Other' then 'Due to some assignments/products it will be manual' else '' end) as ManualInfoMessage


     ,us.UnitId as unitId
    ,us.[Name] as unitName
	,
	  case when (Select top 1 s.StatusId  from FieldDrawingMaster fdm
	    Left Join Status s on fdm.StatusKey=s.StatusKey
		  where fdm.IsDeleted=0 and s.IsDeleted=0 and fdm.QuoteId=@OpportunityId and fdm.GroupId=gc.GroupId
		   order by fdm.CreatedOn desc) is null then 0 
		   else (Select top 1 s.StatusId  from FieldDrawingMaster fdm
	    Left Join Status s on fdm.StatusKey=s.StatusKey
		  where fdm.IsDeleted=0 and s.IsDeleted=0 and fdm.QuoteId=@OpportunityId and fdm.GroupId=gc.GroupId
		   order by fdm.CreatedOn desc)  end  as drawingStatusId

		    ,(Select top 1 s.DisplayName  from FieldDrawingMaster fdm
	    Left Join Status s on fdm.StatusKey=s.StatusKey
		  where fdm.IsDeleted=0 and s.IsDeleted=0 and fdm.QuoteId=@OpportunityId and fdm.GroupId=gc.GroupId
		   order by fdm.CreatedOn desc) as drawingDisplayName

		     ,(Select top 1 UPPER(s.StatusKey)  from FieldDrawingMaster fdm
	    Left Join Status s on fdm.StatusKey=s.StatusKey
		  where fdm.IsDeleted=0 and s.IsDeleted=0 and fdm.QuoteId=@OpportunityId and fdm.GroupId=gc.GroupId
		   order by fdm.CreatedOn desc) as drawingStatusKey

		     ,(Select top 1 s.StatusName   from FieldDrawingMaster fdm
	    Left Join Status s on fdm.StatusKey=s.StatusKey
		  where fdm.IsDeleted=0 and s.IsDeleted=0 and fdm.QuoteId=@OpportunityId and fdm.GroupId=gc.GroupId
		   order by fdm.CreatedOn desc) as drawingStatusName

		    ,(Select top 1 s.[Description]  from FieldDrawingMaster fdm
	    Left Join Status s on fdm.StatusKey=s.StatusKey
		  where fdm.IsDeleted=0 and s.IsDeleted=0 and fdm.QuoteId=@OpportunityId and fdm.GroupId=gc.GroupId
		   order by fdm.CreatedOn desc) as drawingDescription


		   ,(case when s.StatusId is null then 0 else s.StatusId end) as grpStatusId
		   ,UPPER(s.StatusKey) as grpStatusKey
		   ,s.StatusName as grpStatusName
		   ,s.DisplayName as grpDisplayName
		   ,s.[Description] as grpDescription

    ,ISNULL((select top 1 IsLocked from FieldDrawingMaster where GroupId = gc.groupid order by CreatedOn desc), 0) as GroupLocked		
	,(case when @coordinationStatus = 1 AND @projectStatus= 'PRJ_BDAWD' then 1      
        else 0 end) as SendToCoordination,
	@isPrimaryQuote isPrimaryQuote
       from building bg
   Left Join GroupConfiguration gc on bg.Id=gc.BuildingId
   Left Join Units us on gc.GroupId = us.GroupConfigurationId
   Left Join Status s on s.StatusKey=gc.WorkflowStatus
      Where QuoteId=@OpportunityId and gc.IsDeleted=0 and us.IsDeleted=0 and bg.IsDeleted=0 
	  order by bg.CreatedOn ASC 


End




