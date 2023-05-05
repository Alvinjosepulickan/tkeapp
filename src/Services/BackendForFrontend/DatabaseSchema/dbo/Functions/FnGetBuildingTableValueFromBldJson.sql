CREATE FUNCTION [dbo].[FnGetBuildingTableValueFromBldJson]  
(
 @id int
,@variableId nvarchar(500)
)  
RETURNS varchar(15)  
BEGIN  


DECLARE @TempTable table (id int, VariableId nvarchar(255),[Value] nvarchar(255))
declare  @json nvarchar(max)

	Select  @json= '['+ SUBSTRING( 
			( 
		     SELECT ',{"VariableId":"'+k.BuindingType+'","Value":"'+k.BuindingValue+'"}'
			from building b
			Left Join BuildingConfiguration k
			on b.Id = k.BuildingId
			where b.id = @id
			and b.isDeleted=0
					 FOR XML PATH('') 
		), 2 , 9999) + ']' --As BldJson

			--Print @json
			--Print @json2
 
 insert into @TempTable
	SELECT row_number() OVER (ORDER BY JSON_VALUE(value,'$.VariableId')
		,JSON_VALUE(value,'$.Value')) id ,  JSON_VALUE(value,'$.VariableId') as VariableId 
		,JSON_VALUE(value,'$.Value') as [Value]   
	FROM OPENJSON(@json);
 
    RETURN (Select Value from @TempTable where VariableId = @variableId)  
END


