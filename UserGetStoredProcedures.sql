-- STORED PROCEDURES BASICS => Building Stored Procedures

USE DotNetCourseDatabase
GO


--Creating Stored Procedures:
-- ALTER PROCEDURE TutorialAppSchema.spUsers_Get
-- /*EXEC TutorialAppSchema.spUsers_Get*/
-- AS
-- BEGIN
--     SELECT [Users].[UserId],
--         [Users].[FirstName],
--         [Users].[LastName],
--         [Users].[Email],
--         [Users].[Gender],
--         [Users].[Active] 
--     FROM TutorialAppSchema.Users AS Users
-- END
--GO



--Altering/Editing Storefd Procedures:

-- ALTER PROCEDURE TutorialAppSchema.spUsers_Get
-- /*EXEC TutorialAppSchema.spUsers_Get*/
-- AS
-- BEGIN
--     SELECT [Users].[UserId],
--         [Users].[FirstName],
--         [Users].[LastName],
--         [Users].[Email],
--         [Users].[Gender],
--         [Users].[Active] 
--     FROM TutorialAppSchema.Users AS Users
-- END
-- GO



--Adding Parameters --> Filtering my Query with ultiple Parameters:
-- ALTER PROCEDURE TutorialAppSchema.spUsers_Get
-- /*EXEC TutorialAppSchema.spUsers_Get @SecondParameter=2, @UserId=3*/
--     @SecondParamerter BIT, @UserId INT
-- AS
-- BEGIN
--     SELECT [Users].[UserId],
--         [Users].[FirstName],
--         [Users].[LastName],
--         [Users].[Email],
--         [Users].[Gender],
--         [Users].[Active] 
--     FROM TutorialAppSchema.Users AS Users
--         WHERE Users.UserId = @UserId
-- END







--Adding Parameters --> Filtering my Query to return only the User with UserId = 3:
-- ALTER PROCEDURE TutorialAppSchema.spUsers_Get
-- /*EXEC TutorialAppSchema.spUsers_Get @UserId=3*/
--     @UserId INT
-- AS
-- BEGIN
--     SELECT [Users].[UserId],
--         [Users].[FirstName],
--         [Users].[LastName],
--         [Users].[Email],
--         [Users].[Gender],
--         [Users].[Active] 
--     FROM TutorialAppSchema.Users AS Users
--         WHERE Users.UserId = @UserId
-- END



--Adding Parameters --> Filtering my Query With parameters that can be set to Defaults, and we can set this to NULLABLE so that we can Pass Values if Null:
-- ALTER PROCEDURE TutorialAppSchema.spUsers_Get
-- /*EXEC TutorialAppSchema.spUsers_Get @SecondParameter=2, @UserId=3*/
--     @UserId INT = Null --Setting it to a Nullable:
-- AS
-- BEGIN
--     SELECT [Users].[UserId],
--         [Users].[FirstName],
--         [Users].[LastName],
--         [Users].[Email],
--         [Users].[Gender],
--         [Users].[Active] 
--     FROM TutorialAppSchema.Users AS Users
--         WHERE Users.UserId = ISNULL(@UserId, Users.UserId) --Check if the Filter Parameter is Null, if yess, Retturn all Records that has a UserId, I.E No filter.
-- END


--Adding Parameters --> Filtering my Query With parameters that can be set to Defaults, and we can set this to NULLABLE so that we can Pass Values if Null:
-- ALTER PROCEDURE TutorialAppSchema.spUsers_Get
-- /*EXEC TutorialAppSchema.spUsers_Get @UserId=3*/
--     @UserId INT = Null --Setting it to a Nullable:
-- AS
-- BEGIN
--     SELECT [Users].[UserId],
--         [Users].[FirstName],
--         [Users].[LastName],
--         [Users].[Email],
--         [Users].[Gender],
--         [Users].[Active] 
--     FROM TutorialAppSchema.Users AS Users
--         WHERE Users.UserId = ISNULL(@UserId, 0) --Check if the Filter Parameter is Null, if yess, Retturn only record(s) with UserId as 0, I.E Return No record:.
-- END


--Adding Parameters And Working with MULTIPLE TABLES 
CREATE OR ALTER PROCEDURE TutorialAppSchema.spUsers_Get
/*EXEC TutorialAppSchema.spUsers_Get @UserId=3, @Active=0*/
    @UserId INT = Null --Setting it to a Nullable:
    , @Active BIT = Null
AS
BEGIN

    --Check if Table Exists First, and Drop it if it does:
    -- DROP TABLE IF EXISTS #AverageDeptSalary --Only works on most recent MsSQL servers

        --The Older MsSQL Servers will only run with this.
        IF OBJECT_ID('tempdb..#AverageDeptSalary', 'U') IS NOT NULL
            BEGIN 
                DROP TABLE #AverageDeptSalary
            END


    --Building out the Average Salary By Department:
    SELECT UserJobInfo.Department
        , AVG(UserSalary.Salary) AvgSalary
        INTO #AverageDeptSalary --Using # to indicate a temporary Table
    FROM TutorialAppSchema.Users AS Users
        LEFT JOIN TutorialAppSchema.UserSalary AS UserSalary
            ON UserSalary.UserId = Users.UserId
        LEFT JOIN TutorialAppSchema.UserJobInfo AS UserJobInfo
            ON UserJobInfo.UserId = Users.UserId
        GROUP BY UserJobInfo.Department
    

--Creating a Clustered Index for our #AverageDeptSalary Table:

    CREATE CLUSTERED INDEX cix_AverageDeptSalary_Deparment ON #AverageDeptSalary(Department)


    SELECT [Users].[UserId],
        [Users].[FirstName],
        [Users].[LastName],
        [Users].[Email],
        [Users].[Gender],
        [Users].[Active],
        UserSalary.Salary,
        UserJobInfo.Department,
        UserJobInfo.JobTitle
    FROM TutorialAppSchema.Users AS Users
        LEFT JOIN TutorialAppSchema.UserSalary AS UserSalary
            ON UserSalary.UserId = Users.UserId
        LEFT JOIN TutorialAppSchema.UserJobInfo AS UserJobInfo
            ON UserJobInfo.UserId = Users.UserId
        LEFT JOIN #AverageDeptSalary AS AvgSalary
            ON AvgSalary.Department = UserJobInfo.Department
        WHERE Users.UserId = ISNULL(@UserId, Users.UserId) --Check if the Filter Parameter is Null, if yess, Retturn only record(s) with UserId as 0, I.E Return No record:.
        AND Users.Active = ISNULL(@Active, Users.Active)
        --TO ADD Null Values that might be in our DB:
        AND ISNULL(@Active, 0) = COALESCE(@Active, Users.Active, 0) -- If 
END
GO



