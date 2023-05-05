 
CREATE Procedure [dbo].[usp_GetCarPositionFromJson] --632
 @groupId int
AS
Begin
 declare @carPosition nvarchar(max)
 declare @tmp varchar(max)
	SET @tmp = ''
	
Select @tmp = @tmp +  UnitJson + ', '  from Units
   where GroupConfigurationId=@groupId


  SET @carPosition = (Select '[' + (SUBSTRING(@tmp, 0, LEN(@tmp))) + ']')
 
SELECT * FROM  
 OPENJSON ( @carPosition )  
WITH (   
              FDAType   varchar(200) '$.variableId' ,  
             FDAValue     varchar(200)     '$.value'
 ) 
 End