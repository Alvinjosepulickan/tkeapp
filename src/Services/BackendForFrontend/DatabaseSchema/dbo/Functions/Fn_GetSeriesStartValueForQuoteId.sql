
CREATE FUNCTION [dbo].[Fn_GetSeriesStartValueForQuoteId] ()
RETURNS BIGINT
AS
BEGIN
    DECLARE @seriesStartValue BIGINT

       --DEV = 00000001 - 40000000
       --UAT = 40000001 - 60000000
       --PRF = 00000001 - 99999999
       --PRD = 00000001 - 99999999
       SET @seriesStartValue = 70000001

    --return first number of the series to start for the environment
    RETURN @seriesStartValue
END