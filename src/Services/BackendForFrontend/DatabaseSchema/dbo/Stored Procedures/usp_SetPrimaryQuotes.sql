
CREATE procedure [dbo].[usp_SetPrimaryQuotes]
@userName nvarchar(200) = ''
,@QuoteId nvarchar(200) = ''
,@Result int OUTPUT
as
begin
begin try
		

	if(@QuoteId <> '')
		begin
			declare @oppID nvarchar(200)
			set @oppID = (select distinct opportunityid from Quotes where QuoteId = @QuoteId)
			update PrimaryQuotes
			set 
			primaryquoteid = @QuoteId,
			modifiedby = @userName,
			modifiedon = GETDATE()
			where opportunityid = @oppID

			set @Result = 1
			return @Result

		end
	else

		begin
				set @Result = 0
				return @Result
		end

	end try
	begin catch
	EXEC usp_Log_ProcedureCall
			@ObjectID = @@PROCID;
	end catch
end