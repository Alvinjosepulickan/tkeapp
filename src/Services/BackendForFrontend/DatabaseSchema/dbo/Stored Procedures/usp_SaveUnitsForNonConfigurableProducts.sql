CREATE Procedure [dbo].[usp_SaveUnitsForNonConfigurableProducts]  --2,2,'c2duser'
 @groupId int
,@numberOfUnits int
,@createdBy nvarchar(50)
as
Begin
				   declare @noOfUnits int
				   declare @unitName nvarchar(10)
				   declare @existingunit int

					SET @noOfUnits=(Select count(*) from units
						where GroupConfigurationId in (Select groupid from GroupConfiguration where BuildingId =(Select top 1 BuildingId from GroupConfigurationDetails
					where GroupId=@groupid) ))


					if not exists (Select * from units
						where Designation=@unitName and GroupConfigurationId in (Select groupid from GroupConfiguration where BuildingId =(Select top 1 BuildingId from GroupConfigurationDetails
					where GroupId=@groupid)))
					Begin
							Set @unitName = (Select 'U' + cast (@noOfUnits as nvarchar(10)))
					End
					Else
					Begin
						set @existingunit =  (Select top 1 CONVERT(int,replace(Designation,'U','')) from (
						Select top 3 *  from units
							where GroupConfigurationId in (Select groupid from GroupConfiguration where BuildingId =(Select top 1 BuildingId from GroupConfigurationDetails
							where GroupId=@groupId))  order by CONVERT(int,replace(Designation,'U','')) desc
							) a)

							Set @unitName = (Select 'U' + cast (@existingunit + 1 as nvarchar(10)))

					End


					declare @totalCount int;
					SET @totalCount = (Select CONVERT(int,replace(@unitName,'U','')))


					DECLARE @unitTable UnitIDList;  
					DECLARE @unitId int
 					Declare @unitDesignationName nvarchar(10) 
					DECLARE @Counter INT 
					SET @Counter=1
					WHILE ( @Counter <= @numberOfUnits)
					BEGIN
						SET @unitDesignationName = (Select 'U' + cast ( @Counter + @totalCount as nvarchar(10)))
						 Insert into Units
								(
								   [Name]
								  ,GroupConfigurationId
								  ,[Location]
								  ,CreatedBy
								  ,Designation
								  ,[Description]
								  ,IsFutureElevator
								  ,ConflictStatus
								  ,WorkflowStatus
								)
								Values
								(
								   @unitDesignationName
								  ,@groupid
								  ,''
								  ,@createdBy
								  ,@unitDesignationName
								  ,@unitDesignationName
								  ,0
								  ,'UNIT_VAL'
								  ,'UNIT_VAL'
								)

								SET @unitId = (SELECT SCOPE_IDENTITY())  


								 INSERT INTO @unitTable(UnitId)  
								 VALUES (@unitId)  
								

						SET @Counter  = @Counter  + 1
					END
                  


				--  exec [dbo].[usp_SaveUpdateProductSelection] @createdBy,@unitTable,'','NI','CN','BR',0,0

			   ----Supplying factory – 2 digits – Possible values: SJ, ZS, KR, IN, DE, ES, BR, US

			   --China - CN
			   --Germany- GR
			   --Spain - SP

		    declare @opportunityId nvarchar(20)
			declare @country nvarchar(5)
			SET @opportunityId = (Select top 1 OpportunityId from Quotes where QuoteId = (Select distinct OpportunityId from Building where Id in (Select distinct BuildingId from GroupConfiguration
			where GroupId =@groupId)))

			IF(@opportunityId like 'SC-%')
			Begin
				Set @country ='CA'
			End
			Else
			Begin
				Set @country ='US'
			End

				

       exec [dbo].[usp_SaveUpdateProductSelection] @createdBy,@unitTable,'','NI',@country ,@country,0,0


	  Select top 1 SetId as setId from Units 
	        where UnitId in (Select UnitId from @unitTable)    
End