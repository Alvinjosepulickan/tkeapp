
CREATE FUNCTION [dbo].[Fn_IsAllCoordinationGroupsSaved]( @QuoteId nvarchar(25))

RETURNS int
AS
BEGIN
 
	    DECLARE @Result int

		DECLARE @Saved INT
		set @Saved=1

		DECLARE db_cursor1 CURSOR FOR    Select  gc.GroupId as groupId
												 from building bg
												  Left Join GroupConfiguration gc on bg.Id=gc.BuildingId
												   Where QuoteId=  @QuoteId and gc.IsDeleted=0 and bg.IsDeleted=0 

					DECLARE @groupId INT

					OPEN db_cursor1;
					FETCH NEXT FROM db_cursor1 INTO @groupId;
					WHILE @@FETCH_STATUS = 0  
					BEGIN  

						   IF NOT EXISTS(select * from CoordinationQuestions where GroupId = @groupId)
						   begin
								set @Saved = 0
						   end

						   FETCH NEXT FROM db_cursor1 INTO @groupId;
					END;
					CLOSE db_cursor1;
					DEALLOCATE db_cursor1;


    SET @Result = @Saved
    RETURN @Result

END
