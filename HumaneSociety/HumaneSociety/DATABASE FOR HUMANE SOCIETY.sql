/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [AnimalId]
      ,[Name]
      ,[Weight]
      ,[Age]
      ,[Demeanor]
      ,[KidFriendly]
      ,[PetFriendly]
      ,[Gender]
      ,[AdoptionStatus]
      ,[CategoryId]
      ,[DietPlanId]
      ,[EmployeeId]
  FROM [HumaneSociety].[dbo].[Animals]