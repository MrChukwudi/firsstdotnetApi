--Satrting Projects Querry

USE DotNetCourseDatabase
GO

CREATE TABLE TutorialAppSchema.Users(
    UserId INT IDENTITY(1,1) PRIMARY KEY
    , FirstName NVARCHAR(50)
    , LastName NVARCHAR(50)
    , Eamil NVARCHAR(50)
    , Gender NVARCHAR(50)
    , Active BIT
)
GO

CREATE TABLE TutorialAppSchema.UserSalary(
    UserId INT
    , JobTitle NVARCHAR(50)
    , Salary DECIMAL(18, 4)
)
GO

CREATE TABLE TutorialAppSchema.UserJobInfo(
    UserId INT
    , JobTitle NVARCHAR(50)
    , Department NVARCHAR(50)
)
GO

CREATE TABLE TutorialAppSchema.Auth(
	Email NVARCHAR(50) PRIMARY KEY,
	PasswordHash VARBINARY(MAX),
	PasswordSalt VARBINARY(MAX)
)


SELECT [Users].[UserId],
[Users].[FirstName] + ' ' + [Users].[LastName] AS FullName,
[Users].[Email],
[Users].[Gender],
[Users].[Active],
[UserJobInfo].[JobTitle],
[UserJobInfo].[Department]
FROM TutorialAppSchema.Users AS Users
JOIN TutorialAppSchema.UserJobInfo AS UserJobInfo
ON Users.UserId = UserJobInfo.UserId
WHERE Users.Active = 1
ORDER BY UserJobInfo.UserId DESC
GO



SELECT [Users].[UserId],
[Users].[FirstName] + ' ' + [Users].[LastName] AS FullName,

[Users].[Email],
[Users].[Gender],
[Users].[Active] FROM TutorialAppSchema.Users AS Users
WHERE Users.Active = 0
ORDER BY UserId DESC
GO

SELECT * FROM TutorialAppSchema.UserSalary
GO


SELECT * FROM TutorialAppSchema.Users
GO


SELECT * FROM TutorialAppSchema.UserJobInfo
GO

SELECT * FROM TutorialAppSchema.Auth
GO

SELECT * FROM TutorialAppSchema.UserSalary
    UNION ALL 
    SELECT * FROM TutorialAppSchema.UserSalary
GO

SELECT * FROM TutorialAppSchema.Users AS Users
JOIN TutorialAppSchema.UserSalary AS UserSalary
ON UserSalary.UserId = Users.UserId
LEFT JOIN TutorialAppSchema.UserJobInfo AS UserJobInfo
ON UserJobInfo.UserId = Users.UserId
WHERE Users.Active = 1
ORDER BY Users.UserId DESC


CREATE CLUSTERED INDEX cix_UserSalary_UserId ON TutorialAppSchema.UserSalary(UserId)

CREATE NONCLUSTERED INDEX ix_UserJobInfo_JobTitile ON TutorialAppSchema.UserJobInfo(JobTitle) INCLUDE (Department)



SELECT GETDATE()