 
Create FUNCTION [dbo].[Fn_GetUnitToLiftDesignerByGroupid]( @groupId int,@GroupConfigurationType nvarchar(500))

RETURNS nvarchar(200)

Begin

    DECLARE @Result nvarchar(200)

    SET @Result = (
           
		   
		    Select top 1  
		        case when GroupConfigurationType like '%ELEVATOR001%'   
			         OR GroupConfigurationType like '%ELEVATOR002%'     
					 OR GroupConfigurationType like '%ELEVATOR003%'     
					 OR GroupConfigurationType like '%ELEVATOR004%'  
					 OR GroupConfigurationType like '%ELEVATOR005%'    
					 OR GroupConfigurationType like '%ELEVATOR006%'       
					 OR GroupConfigurationType like '%ELEVATOR007%'      
					 OR GroupConfigurationType like '%ELEVATOR008%'  then 'Lift_Designer' end   as GroupConfigurationType
			  from GroupConfigurationDetails
		  where GroupId=@groupId   and GroupConfigurationType=@GroupConfigurationType
		  
	)
 
    RETURN @Result

end