--Registeration/Login Procedure

USE DotNetCourseDatabase
GO


--Registration UPSERT
CREATE OR ALTER PROCEDURE TutorialAppSchema.spRegistration_Upsert
/*EXEC TutorialAppSchema.spPost_Upsert */
    @Email NVARCHAR(50),
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX)


AS 
BEGIN 

    IF NOT EXISTS (SELECT * FROM TutorialAppSchema.Auth WHERE Email = @Email)
        BEGIN
            INSERT INTO TutorialAppSchema.Auth (
                [Email],
                [PasswordHash],
                [PasswordSalt]
             ) VALUES (
                 @Email ,
                 @PasswordHash ,
                 @PasswordSalt
             )
        END

    ELSE
        BEGIN
            UPDATE TutorialAppSchema.Auth 
            SET PasswordHash = @PasswordHash,
                PasswordSalt = @PasswordSalt
            WHERE Email = @Email
        END
END 
GO



--Login Confirmation 
CREATE OR ALTER PROCEDURE TutorialAppSchema.spLoginConfirmation_Get
/*TutorialAppSchema.spLoginConfirmation_Get*/
@Email NVARCHAR(50)

AS
BEGIN 
        SELECT [Auth].[PasswordHash], 
                [Auth].[PasswordSalt]

            FROM TutorialAppSchema.Auth AS Auth 
            WHERE Auth.Email = @Email
END 
GO 





