 
CREATE FUNCTION [dbo].[Fn_GetGroupsCount]( @buildingId int, @consoleid int,@panelname nvarchar(50))

RETURNS INT

Begin

    DECLARE @Result nvarchar(200)

    SET @Result = (
	--SELECT  * 
	--		FROM   GroupConfiguration  gc
	--			   Where  gc.BuildingId =8   
	
	Select(Select count(*)  from [dbo].[BldgEquipmentConsole] bec
	  Left Join [dbo].[BldgEquipmentGroupMapping] bem on bec.ConsoleId =  bem.ConsoleId
	    Where bec.BuildingId=@buildingId and  bec.Name like '%'+@panelname+'%' and 
		     bem.GroupName <> '' and bem.is_Checked =1 
		  and bec.ConsoleId=@consoleid)  
		   
		   +

		  (case when (Select Count(*) from [dbo].[BldgEquipmentCategoryCnfgn]
                   Where ConsoleId=@consoleid) is null then 0 else (Select Count(*) from [dbo].[BldgEquipmentCategoryCnfgn]
                   Where ConsoleId=@consoleid) end) 
	)
 
    RETURN @Result

end  
