-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SavePriceDetails]
	-- Add the parameters for the stored procedure here
	@setId as int,
	@UnitVariables as unitConfigurationDataTable READONLY,
	@LeadTimeVariables as unitConfigurationDataTable READONLY,
	@CreatedBy nvarchar(50),
	@Result int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	if(exists (select * from PriceDetails where UnitId in (select UnitId from Units where SetId=@setId)))
	begin

			--updating already saved variables--

			update PriceDetails set VariableValue=t.value,modifiedby=@CreatedBy,ModifiedOn=getdate()
			from @UnitVariables t join PriceDetails g on t.UnitJson=g.VariableId
			where g.UnitId in (select UnitId from Units where SetId=@setId)
			and VariableId<>'ManufacturingComments'
			--insert new variable assignments--

			insert into PriceDetails(UnitId,VariableId,VariableValue, CreatedBy,CreatedOn)
			select u.UnitId,uv.UnitJson,uv.value,@CreatedBy,getdate()
			from @UnitVariables uv cross join Units u where uv.unitjson not in
			(select VariableId from PriceDetails where UnitId in (select UnitId from Units where SetId=@setId)) and u.SetId=@setId
			and uv.unitjson<>'ManufacturingComments'

			insert into PriceDetails(UnitId,VariableId,VariableValue, CreatedBy,CreatedOn)
			select u.UnitId,uv.UnitJson,uv.value,@CreatedBy,getdate()
			from @UnitVariables uv cross join Units u where  u.SetId=@setId
			and uv.unitjson ='ManufacturingComments'

			set @Result=@SetId
	end

	else
	begin

			insert into PriceDetails(UnitId,VariableId,VariableValue, CreatedBy,CreatedOn)
			select u.UnitId,uv.UnitJson,uv.value,@CreatedBy,getdate() from @UnitVariables uv cross join Units u
			where u.SetId=@setId
			set @Result=@SetId
	end

	if(exists(select * from LeadTimeDetails where SetId=@setId))
	begin
		delete from LeadTimeDetails where SetId=@setId
		insert into LeadTimeDetails(SetId,VariableId,VariableValue, CreatedBy,CreatedOn)
		select @setId, uv.UnitJson, uv.value, @CreatedBy, getdate()
		from @LeadTimeVariables uv
	end
	else
	begin
		insert into LeadTimeDetails(SetId,VariableId,VariableValue, CreatedBy,CreatedOn)
		select @setId, uv.UnitJson, uv.value, @CreatedBy, getdate()
		from @LeadTimeVariables uv
	end
END