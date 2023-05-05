
CREATE Procedure [dbo].[usp_GetReleaseInfo] --'rf-2021-000005'--'rf-2021-000005'--'Te-2021-000001'  --'ADIA-1N16VG8'
@OpportunityId nvarchar(25)

AS
Begin
    
     Select bg.id as buildingId
          ,bg.BldName as buildingName
    ,gc.GroupId as groupId
    ,gc.GroupName as groupName
	,(case when s.StatusId is null then 0 else s.StatusId end) as grpStatusId
	,s.StatusKey as grpStatusKey
	,s.StatusName as grpStatusName
	,s.[Description] as grpDescription
	,s.DisplayName as grpDisplayName
	,count(us.UnitId) as unitCount
	,(case when gc.WorkflowStatus in ('GRP_CRD','GRP_FR') then 1 else 0 end) as ReleaseToManufacturing
	, (case when (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType ='Group_Validation.Parameters.Basic_Info.PRODUCTCATEGORY' and GroupId=gc.GroupId) = 'Elevator' then 'ELEV' 
		   when (Select top 1 GroupConfigurationValue from GroupConfigurationDetails
             where GroupConfigurationType ='Group_Validation.Parameters.Basic_Info.PRODUCTCATEGORY' and GroupId=gc.GroupId) = 'Escalator/Moving-Walk'  then 'ESCL' end) as productCategory	
   from building bg
   Left Join GroupConfiguration gc on bg.Id=gc.BuildingId
   Left Join Units us on gc.GroupId = us.GroupConfigurationId
   Left Join Status s on s.StatusKey=gc.WorkflowStatus
   Where QuoteId=@OpportunityId and gc.IsDeleted=0 and us.IsDeleted=0 and bg.IsDeleted=0
   Group by bg.id, bg.BldName, bg.workflowstatus, gc.GroupId, gc.GroupName, gc.WorkflowStatus, us.WorkflowStatus, s.StatusName, s.StatusId, s.StatusKey, s.DisplayName,s.[Description]
   Order by bg.id

End


--update building set workflowstatus = 'BLDG_COM' where bui
--update GroupConfiguration set WorkflowStatus = 'GRP_COM' where GroupId = 9
--update Units set WorkflowStatus = 'UNIT_COM' where SetId in (217,218)


--select * from Building where id = 4
--select * from GroupConfiguration where GroupId = 9
--select * from units where GroupConfigurationId = 16
--select * from UnitConfiguration where SetId = 222