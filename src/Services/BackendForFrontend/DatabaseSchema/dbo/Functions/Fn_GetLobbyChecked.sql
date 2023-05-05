
CREATE FUNCTION [dbo].[Fn_GetLobbyChecked]( @buildingId int)

RETURNS INT

Begin

    DECLARE @Result nvarchar(200)

	SET @Result = (            CASE
								WHEN
									(
										SELECT
											TOP 1 (
											CASE
											WHEN
												Travelfeet > 75 
												or Travelfeet = 75 
											THEN
												1 
											ELSE
												0 
											END
	                                     ) 
										FROM
											(
											SELECT
												Travelfeet 
											FROM
												OpeningLocation ol 
												LEFT JOIN
													GroupConfiguration gc 
													ON ol.GroupConfigurationId = gc.GroupId 
											WHERE
												gc.BuildingId = @buildingId
											)
											a
									)
									IS NULL 
								THEN
									0 
								ELSE
									( 
									SELECT
										TOP 1 (
										CASE
											WHEN
											Travelfeet > 75 
											or Travelfeet = 75 
											THEN
											1 
											ELSE
											0 
										END
									) 
									FROM
										(
											SELECT
											Travelfeet 
											FROM
											OpeningLocation ol 
											LEFT JOIN
												GroupConfiguration gc 
												ON ol.GroupConfigurationId = gc.GroupId 
											WHERE
											gc.BuildingId = @buildingId
										)
										a ) 
								END )
    RETURN @Result

end