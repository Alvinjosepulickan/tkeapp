 
CREATE FUNCTION [dbo].[Fn_GetFDAUnitNameByGroupid]( @groupId int,@GroupConfigurationType nvarchar(500))

RETURNS nvarchar(200)

Begin

    DECLARE @Result nvarchar(200)

    SET @Result = (
           
		   
		    Select top 1  
		        case when GroupConfigurationType like '%ELEVATOR001%'  then 'U1'   
			         when GroupConfigurationType like '%ELEVATOR002%'  then 'U2'    
					 when GroupConfigurationType like '%ELEVATOR003%'  then 'U3'    
					 when GroupConfigurationType like '%ELEVATOR004%'  then 'U4'    
					 when GroupConfigurationType like '%ELEVATOR005%'  then 'U5'    
					 when GroupConfigurationType like '%ELEVATOR006%'  then 'U6'    
					 when GroupConfigurationType like '%ELEVATOR007%'  then 'U7'    
					 when GroupConfigurationType like '%ELEVATOR008%'  then 'U8' end   as GroupConfigurationType
			  from GroupConfigurationDetails
		  where GroupId=@groupId   and GroupConfigurationType=@GroupConfigurationType
		  
	)
 
    RETURN @Result

end  
