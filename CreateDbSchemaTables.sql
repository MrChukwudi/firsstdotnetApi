--Creating Your Project Database, Schema, and Tables:

CREATE DATABASE DotNetCourseDatabase
GO

USE DotNetCourseDatabase
GO

CREATE SCHEMA TutorialAppSchema
GO

CREATE TABLE TutotrialAppSchema.Computer
(
    -- TableId INT IDENTITY(Starting, Increament) => Idnetity is used to tell the DB to automatically increament our Field with every update
    CompouterId INT IDENTITY(1,1) PRIMARY KEY
    , Motherboard VARCHAR(255)
    , CPUCores INT
    , HasWifi BIT
    , HasLTE BIT
    , ReleaseDate DATETIME
    , Price DECIMAL(18, 4)
    , VideoCard NVARCHAR(255)
)
GO

SELECT * FROM TutorialAppSchema.Computer


-- Inserting Data Into Our Table:
INSERT INTO TutorialAppSchema.Computer(
[Motherboard],
[CPUCores],
[HasWifi],
[HasLTE],
[ReleaseDate],
[Price],
[VideoCard]
) VALUES (
    'Sample_Motherboard',
    32,
    1,
    0,
    '2020-11-29',
    3200,
    'Intel-10'

)


-- Deleting a Row of Data
DELETE FROM TutorialAppSchema.Computer WHERE ComputerId = 100

-- Updating Data inside our Table: Without being specific, this will affect the whole Table.
UPDATE TutorialAppSchema.Computer SET CPUCores = 64 


-- Updating Data inside our Table: Being specific, THIS IS THE RECOMMENDED PRACTICE:
-- UPDATE TutorialAppSchema.Computer SET CPUCores = NULL WHERE CPUCores = 0
-- UPDATE TutorialAppSchema.Computer SET CPUCores = 64 WHERE ReleaseDate < '2017-01-01'


--ORDER BY
SELECT * FROM TutorialAppSchema.Computer
    ORDER BY ReleaseDate

-- Working with ISNULL([ColumnName], Value if it's Null) AS ColumnName
    SELECT [ComputerId],
    [Motherboard],
    ISNULL([CPUCores], 0) AS CPUCores,
    [HasWifi],
    [HasLTE],
    [ReleaseDate],
    [Price],
    [VideoCard] FROM TutorialAppSchema.Computer
GO


SELECT * FROM TutorialAppSchema.Users