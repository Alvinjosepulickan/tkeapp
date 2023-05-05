-- =============================================
-- Author:        Raja Veeramalla
-- Create date: 7/11/2021
-- Description:    Function to generate the next UEID
-- =============================================
CREATE PROCEDURE [dbo].[usp_GenerateQuoteId]
(
    @country NVARCHAR(2) NULL,
    @QuoteId NVARCHAR(25) OUTPUT
)
AS
BEGIN
    DECLARE @nextSequence BIGINT 
    DECLARE @currentYearStartSequence BIGINT
    DECLARE @queryForNewSequence NVARCHAR(100)
       DECLARE @seriesStartValue BIGINT
    EXEC @seriesStartValue = Fn_GetSeriesStartValueForQuoteId

       --getting the start value sequence for current year
    SET @currentYearStartSequence = (CAST(YEAR(SYSDATETIME()) AS BIGINT) * 10000000) + @seriesStartValue
    SET @nextSequence = NEXT VALUE FOR QuoteSequence
       --select @currentYearStartSequence, @nextSequence

    --checking if the start value sequence for current year is less than the generated next sequence
    --if so reset the sequence
    IF @currentYearStartSequence > @nextSequence
       BEGIN
              -- Resetting the sequence based on the new year
              SET @queryForNewSequence = N'ALTER SEQUENCE QuoteSequence RESTART WITH ' + CAST(@currentYearStartSequence AS NVARCHAR(25))
              EXEC (@queryForNewSequence)         
              
              --Get the next sequence
              SET @nextSequence = NEXT VALUE FOR QuoteSequence
       END
    
       --generating the UEID in the specified format and Return the result in OUTPUT parameter
    SELECT @QuoteId = CONCAT(@country, '-', LEFT(@nextSequence, 4), '-', RIGHT(@nextSequence, LEN(@nextSequence) - 4))
END