
CREATE PROCEDURE [dbo].[usp_UpdateGroupConfiguration]

 @BuildingId [int],

 @GroupName [nvarchar](250),

 @GroupJson [nvarchar](max),

 --@UnitVariables as unitDataTable READONLY,

 @GroupConfigurationId [int],

 @Result [INT] OUTPUT

 AS

BEGIN



  BEGIN TRY

 IF EXISTS(SELECT * FROM GroupConfiguration WHERE GroupName=@GroupName AND BuildingId=@BuildingId AND GroupId!=@GroupConfigurationId)

 BEGIn

  SET @Result=-1

  RETURN 0

 END

 IF (EXISTS (SELECT DISTINCT(GroupId) FROM GroupConfiguration WHERE GroupId=@GroupConfigurationId ))

 BEGIN

 DECLARE @CreatedBy [nvarchar](25)

 DECLARE @Date [DateTime]

 SET @CreatedBy=(SELECT DISTINCT(CreatedBy) FROM GroupConfiguration WHERE GroupId=@GroupConfigurationId and BuildingId=@BuildingId)

 SET @Date=(SELECT DISTINCT(CreatedOn) FROM GroupConfiguration WHERE GroupId=@GroupConfigurationId and BuildingId=@BuildingId)
 DECLARE @NewGroupNameJson NVARCHAR(MAX)

 set @NewGroupNameJson='[{"variableId":"Group_Validation.Parameters.Basic_Info.GRPDESG","value":"'+(@GroupName)+'"}]'
 declare @OldGroupName nvarchar(max)=(select GroupName from GroupConfiguration where GroupId=@GroupConfigurationId)
 IF(@GroupJson = @NewGroupNameJson)

 BEGIN

  DECLARE @GroupJsonString NVARCHAR(MAX)

  DECLARE @OldGroupNameJson NVARCHAR(MAX)


  --set @GroupJsonString = (Select '[' +  
		--SUBSTRING( 
		--( 
		--     SELECT ',{"VariableId":"'+k.GroupConfigurationType+'","Value":"'+k.GroupConfigurationValue+'"}'
		-- from [GroupConfiguration] g
		-- Left join [GroupConfigurationDetails] k
		-- on g.GroupId = k.GroupId
		-- and g.BuildingId = k.BuildingId
		-- where  g.GroupId = @id		 
		-- and g.isDeleted  = 0
		-- FOR XML PATH('') 
		--), 2 , 9999) + ']')


 



  --set @OldGroupNameJson='{"VariableId":"Group_Validation.Parameters.Basic_Info.GRPDESG","Value":"'+@OldGroupName+'"},'



  --set @NewGroupNameJson='{"VariableId":"Group_Validation.Parameters.Basic_Info.GRPDESG","Value":"'+(@GroupName)+'"},'



  --set @GroupJsonString= replace(@GroupJsonString,@OldGroupNameJson,@NewGroupNameJson)
  /**Group Config History**/
	insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,createdby,createdon,modifiedby,modifiedon)
	select @GroupConfigurationId,'Group Name',@GroupName,@OldGroupName,@CreatedBy,getdate(),@CreatedBy,getdate()
  /**Group Config History**/

  update GroupConfiguration set GroupName=@GroupName,/*GroupJson=@GroupJsonString,*/ModifedBy=@CreatedBy,ModifiedOn=GETDATE() where GroupId=@GroupConfigurationId

  update GroupConfigurationDetails set GroupConfigurationValue = @GroupName, ModifedBy=@CreatedBy,ModifiedOn=GETDATE() where GroupId = @GroupConfigurationId and GroupConfigurationType = 'Group_Validation.Parameters.Basic_Info.GRPDESG'

  SET @Result=1

 END
 ELSE
 BEGIN

  /**Group Config History**/
	insert into GroupConfigHistory(GroupId,VariableId,CurrentValue,PreviousValue,createdby,createdon,modifiedby,modifiedon)
	select @GroupConfigurationId,'Group Name',@GroupName,@OldGroupName,@CreatedBy,getdate(),@CreatedBy,getdate()
  /**Group Config History**/

 UPDATE [dbo].[GroupConfiguration] SET [GroupName]=@GroupName,/*[GroupJson]=@GroupJson,*/ModifedBy=@CreatedBy,ModifiedOn=GETDATE() WHERE GroupId=@GroupConfigurationId AND BuildingId=@BuildingId

 DECLARE @TempTable table (id int, VariableId nvarchar(255),[Value] nvarchar(255))

 INSERT INTO @TempTable
 SELECT @GroupConfigurationId id, 
 JSON_VALUE(value,'$.variableId') as VariableId , 
 JSON_VALUE(value,'$.value') as [Value]
 FROM OPENJSON(@GroupJson);
			 
 update grp
 set GroupConfigurationValue = [Value], 
 IsDeleted = 0,
 ModifedBy=@CreatedBy,
 ModifiedOn=GETDATE() 
 from GroupConfigurationDetails grp
 join @TempTable tmp
 on tmp.id = grp.GroupId
 and tmp.VariableId = grp.GroupConfigurationType
 where grp.GroupId = @GroupConfigurationId 


 ----deleting the parameters removed in the json----
			update grp
			set IsDeleted = 1,
			ModifedBy=@CreatedBy,
			ModifiedOn=GETDATE() 
			From GroupConfigurationDetails grp
			where not exists (select '*' from @TempTable tmp
								where	tmp.id = grp.GroupId
								and	tmp.VariableId = grp.GroupConfigurationType
								)
				and grp.GroupId = @GroupConfigurationId

----
		If exists (Select '*' from @TempTable 
					where VariableId not in(select GroupConfigurationType 
											from GroupConfigurationDetails 
											where GroupId = @GroupConfigurationId
											)
					)
		Begin
			INSERT INTO [dbo].[GroupConfigurationDetails](GroupId,BuildingId,GroupConfigurationType,GroupConfigurationValue,CreatedBy)
			select  @GroupConfigurationId,@BuildingId,VariableId,[Value],@CreatedBy
			from @TempTable 
			where VariableId not in(select GroupConfigurationType 
									from GroupConfigurationDetails 
									where GroupId = @GroupConfigurationId
									)
		End
 
 set @Result=1
 END

 SET @Result=1

 END

 ELSE

 BEGIN

  SET @result=0

 END

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



