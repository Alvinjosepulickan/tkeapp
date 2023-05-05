
CREATE FUNCTION [dbo].[Fn_GetUnitsCount](@buildingId int, @consoleid int,@panelname nvarchar(50))

RETURNS INT

Begin
 
	DECLARE @Result nvarchar(200)

    SET @Result = (
	--Select count(*) from Units ut
	--			    Left Join GroupConfiguration gc on ut.GroupConfigurationId=gc.GroupId
	--				  Where gc.BuildingId=@buildingId 
	
	Select (Select count(*) from Units 
		  Where GroupConfigurationId  in (Select GroupId  from [dbo].[BldgEquipmentConsole] bec
	  Left Join [dbo].[BldgEquipmentGroupMapping] bem on bec.ConsoleId =  bem.ConsoleId
	    Where bec.BuildingId=@buildingId and  bec.Name like '%'+@panelname+'%' and  bem.GroupName <> '' and bem.is_Checked =1
		    and bec.ConsoleId=@consoleid)) +


			(case when (Select sum(NoOfUnits) from [dbo].[BldgEquipmentCategoryCnfgn]
                    Where ConsoleId=@consoleid) is null then 0 else (Select sum(NoOfUnits) from [dbo].[BldgEquipmentCategoryCnfgn]
                    Where ConsoleId=@consoleid) end)
	)

    RETURN @Result

end   
