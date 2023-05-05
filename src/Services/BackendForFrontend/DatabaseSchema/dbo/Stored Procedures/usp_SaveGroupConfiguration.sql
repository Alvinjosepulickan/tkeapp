
CREATE PROCEDURE [dbo].[usp_SaveGroupConfiguration]

 @BuildingId int,

 @GroupName nvarchar(250),

 @GroupJson nvarchar(max),

 --@UnitVariables as unitDataTable READONLY,

 @CreatedBy nvarchar(50),

 @Result INT OUTPUT

 AS

BEGIN

  BEGIN TRY

 if(exists(select * from GroupConfiguration where GroupName=@GroupName and BuildingId=@BuildingId and IsDeleted=0))

 BEGIN

  SET @Result=-1

  Return 0

 END

 DECLARE @GroupId [int]

 INSERT INTO [dbo].[GroupConfiguration]

              (

        [BuildingId] ,

        [GroupName] ,

        --[GroupJson],

        [CreatedBy],

		WorkflowStatus

              )

     VALUES

              (

        @BuildingId ,

        @GroupName ,

        --@GroupJson,

        @CreatedBy,

		'Grp_Inc'

              )


 SET @GroupId=(SELECT (GroupId) FROM GroupConfiguration where BuildingId=@BuildingId and GroupName=@GroupName /*and GroupJson=@GroupJson*/ 
 and CreatedBy=@CreatedBy and IsDeleted=0)

 /**Group Config History**/

 insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,createdby,createdon,modifiedby,modifiedon)
 select @GroupId,JSON_VALUE(value,'$.variableId') as VariableId ,JSON_VALUE(value,'$.value') as [Value],'',@CreatedBy,getdate(),@CreatedBy,getdate()
 FROM OPENJSON(@GroupJson)

 /**Group Config History**/
INSERT INTO [dbo].[GroupConfigurationDetails]

(
	GroupId,
	
	BuildingId,

	GroupConfigurationType,

	GroupConfigurationValue,

	CreatedBy
)
Select

	@GroupId,
	
	@BuildingId,

	JSON_VALUE(value,'$.variableId') as VariableId , 
			 
	JSON_VALUE(value,'$.value') as [Value],
	
	@CreatedBy

	FROM OPENJSON(@GroupJson);


 INSERT INTO [dbo].[OpeningLocation]

   (

    GroupConfigurationId,

    CreatedBy

   )

   VALUES

   (

    @GroupId,

    @CreatedBy

   )

	
 SET @Result=@GroupId
 declare @quoteId nvarchar(100)
 select @quoteId=QuoteId From Building where id=@BuildingId
 --Update Workflow status for building when a new group is added to the Building and quote
 EXEC [dbo].[usp_UpdateWorkflowStatus] @BuildingId,'building'
 EXEC [dbo].[usp_UpdateWorkflowStatus] @quoteId,'quote'
  END TRY

  BEGIN CATCH

  SET @Result=0
  EXEC usp_Log_ProcedureCall
		@ObjectID = @@PROCID,
		@AdditionalInfo='Something Went Wrong';

  declare @error nvarchar(max)
			set @error=ERROR_MESSAGE()
			RAISERROR(@error,11,1)
	END CATCH

  END
