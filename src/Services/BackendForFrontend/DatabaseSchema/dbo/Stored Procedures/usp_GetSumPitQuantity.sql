CREATE Procedure usp_GetSumPitQuantity
 @groupId int
,@ConstantMapperList as [dbo].[VariableMapper] READONLY
as
Begin

       declare @elevator nvarchar(200);
	   declare @sumpQty nvarchar(200);
	   SET @elevator = (Select VariableType from @ConstantMapperList where VariableKey ='ELEVATOR')
	   SET @sumpQty = (Select VariableType from @ConstantMapperList where VariableKey ='SUMPQTY')

     
	declare @totalCount int;
	SET @totalCount = (select count(*)
			from  GroupConfiguration gc  
			 Left Join Units us on us.GroupConfigurationId = gc.GroupId
					Where us.GroupConfigurationId= @groupId and gc.IsDeleted=0 and us.IsDeleted=0)

		Declare @variableId nvarchar(200)
		Declare @variables as [dbo].[VariableMapper]  
		DECLARE @Counter INT 
		SET @Counter=1
		WHILE ( @Counter <= @totalCount)
		BEGIN
			 SET @variableId = concat(@elevator , @Counter ,@sumpQty)
    

			IF Not Exists(Select * from GroupConfigurationDetails where GroupId=@groupId and GroupConfigurationType = @variableId )
			Begin
				insert into @variables
				Select @variableId as VariableId , 0 as [Value] 
			End


			 SET @Counter  = @Counter  + 1
		END

		Select * from @variables

End