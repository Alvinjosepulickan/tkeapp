
CREATE FUNCTION [dbo].[Fn_GetSeriesStartValueForUUID] ()
RETURNS BIGINT
AS
BEGIN
    DECLARE @seriesStartValue BIGINT
       
       --DEV = 000001 - 400000
       --UAT = 400001 - 600000
       --PRF = 000001 - 999999
       --PRD = 000001 - 999999
       SET @seriesStartValue = 700001

    --return first number of the series to start for the environment
    RETURN @seriesStartValue
END