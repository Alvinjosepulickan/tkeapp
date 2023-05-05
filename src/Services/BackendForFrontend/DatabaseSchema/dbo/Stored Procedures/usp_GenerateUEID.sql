
-- =============================================
-- Author:        Raja Veeramalla
-- Create date:   7/11/2021
-- Description:   Function to generate the next UEID
-- =============================================
CREATE PROCEDURE [dbo].[usp_GenerateUEID]
(
    @businessLine NVARCHAR(2) NULL,
    @country NVARCHAR(2) NULL,
    @supplyingFactory NVARCHAR(2) NULL,
    @UEID NVARCHAR(25) OUTPUT
)
AS
BEGIN
    DECLARE @nextSequence BIGINT 
    DECLARE @currentYearStartSequence BIGINT
    DECLARE @queryForNewSequence NVARCHAR(100)
    DECLARE @seriesStartValue BIGINT
    EXEC @seriesStartValue = Fn_GetSeriesStartValueForUUID

       --getting the start value sequence for current year
    SET @currentYearStartSequence = (CAST(YEAR(SYSDATETIME()) AS BIGINT) * 100000) + @seriesStartValue
    SET @nextSequence = NEXT VALUE FOR UEIDSequence
       --select @currentYearStartSequence, @nextSequence

    --checking if the start value sequence for current year is less than the generated next sequence
    --if so reset the sequence
    IF @currentYearStartSequence > @nextSequence
       BEGIN
              -- Resetting the sequence based on the new year
              SET @queryForNewSequence = N'ALTER SEQUENCE UEIDSequence RESTART WITH ' + CAST(@currentYearStartSequence AS NVARCHAR(25))
              EXEC (@queryForNewSequence)         
              
              --Get the next sequence
              SET @nextSequence = NEXT VALUE FOR UEIDSequence
       END
    
       --generating the UEID in the specified format and Return the result in OUTPUT parameter
    SET @UEID = CONCAT(@BusinessLine, @country, @nextSequence, @supplyingFactory)
END
/****** Object:  StoredProcedure [dbo].[usp_GenerateQuoteId]    Script Date: 7/21/2021 12:32:21 PM ******/
SET ANSI_NULLS ON