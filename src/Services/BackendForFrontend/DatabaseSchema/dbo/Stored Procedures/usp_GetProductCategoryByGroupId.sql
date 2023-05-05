CREATE Procedure [dbo].[usp_GetProductCategoryByGroupId]
 @id int
,@type nvarchar(50)
,@ConstantMapperList as [dbo].[VariableMapper] READONLY
As
Begin
	declare @productCategory nvarchar(200);
	SET @productCategory = (Select VariableType from @ConstantMapperList where VariableKey ='PRODUCTCATEGORY')
  
 IF (@type = 'group')
 Begin
   Select GroupConfigurationValue from GroupConfigurationDetails
    Where GroupId=@id and GroupConfigurationType=@productCategory
 End
 Else IF (@type = 'set')
 Begin
  Select GroupConfigurationValue from GroupConfigurationDetails
    Where GroupId=(Select top 1 GroupConfigurationId from Units
	 Where SetId = @id) and GroupConfigurationType=@productCategory
 End
End