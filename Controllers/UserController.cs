
using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;


namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly DataContextDapper _dapper;



    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);

    }


    // [HttpGet("TestConnection")]
    // public DateTime TestConnection()
    // {
    //     return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    // }


    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        string sql = @"
            SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
            [Active] FROM TutorialAppSchema.Users";

        IEnumerable<User> users = _dapper.LoadData<User>(sql);
        return users;


    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        string sql = @"
                    SELECT  [UserId],
                            [FirstName],
                            [LastName],
                            [Email],
                            [Gender],
                            [Active] 
                    FROM TutorialAppSchema.Users
                    WHERE UserId = " + userId.ToString();

        User singleUser = _dapper.LoadDataSingle<User>(sql);
        return singleUser;

    }

    //To Edit a User in Our Database:
    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
        UPDATE TutorialAppSchema.Users
        SET [FirstName] = '" + user.FirstName +
        "', [LastName] = '" + user.LastName +
        "', [Email] = '" + user.Email +
        "', [Gender] = '" + user.Gender +
        "', [Active] = '" + user.Active +
        "' WHERE UserId = " + user.UserId.ToString();
        Console.WriteLine(sql);


        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Update User");
    }


    //To Add a new User to our Database:
    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        string sql = @"
                    INSERT INTO TutorialAppSchema.Users(
                        [FirstName],
                        [LastName],
                        [Email],
                        [Gender],
                        [Active]
                    ) VALUES(
                        '" + user.FirstName +
                        "', '" + user.LastName +
                        "', '" + user.Email +
                        "', '" + user.Gender +
                        "', '" + user.Active +
                        "')";

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to add User");
    }


    //To Delete a User from Our Database:
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"
        DELETE FROM TutorialAppSchema.Users 
        WHERE UserId = " + userId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete User");
    }

    //To Get Single User Salary:
    [HttpGet("GetUserSingleUserSalary/{userId}")]
    public IEnumerable<UserSalary> GetUserSingleUserSalary(int userId)
    {
        string sql = @"SELECT [UserId],
                              [Salary] 
                       FROM TutorialAppSchema.UserSalary
                       WHERE [UserId] =" + userId;
        return _dapper.LoadData<UserSalary>(sql);

    }


    //To Get All Users Salary:
    [HttpGet("GetUserAllUserSalary")]
    public IEnumerable<UserSalary> GetUserAllUserSalary()
    {
        string sql = @"SELECT [UserId],
                              [Salary] 
                       FROM TutorialAppSchema.UserSalary";
        return _dapper.LoadData<UserSalary>(sql);

    }


    //To Create a new UserSalary record:
    [HttpPost("AddUserSalary")]
    public IActionResult AddUserSalary(UserSalary userSalary)
    {
        string sql = @"
        INSERT INTO TutorialAppSchema.UserSalary(
            UserID,
            Salary
        ) VALUES ('" + userSalary.UserId
            + "', '" + userSalary.Salary
            + "')";



        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Unable to Add UserSalary");
    }


    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary userSalary)
    {
        string sql = @" UPDATE TutorialAppSchema.UserSalary
                        SET Salary = '" + userSalary.Salary +
                        "' WHERE UserId = " + userSalary.UserId;

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Could not update userSalary");

    }

    //Delete UserSalary
    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        string sql = @"
                        DELETE FROM TutorialAppSchema.UserSalary 
                        WHERE UserId = " + userId.ToString();



        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Could not delete user");
    }


    //Get All the UserJobInfo Records:
    [HttpGet("GetAllUserJobInfo")]
    public IEnumerable<UserJobInfo> GetUserJobInfo()
    {
        string sql = @"
                        SELECT UserId
                            , JobTitle
                            , Department
                        FROM TutorialAppSchema.UserJobInfo";

        return _dapper.LoadData<UserJobInfo>(sql);
    }



    //Get Single UserJobInfo Record:
    [HttpGet("GetSingleUserJobInfo/{userId}")]
    public IEnumerable<UserJobInfo> GetSingleUserJobInfo(int userId)
    {
        string sql = @"
                        SELECT UserId
                            , JobTitle
                            , Department
                        FROM TutorialAppSchema.UserJobInfo
                        WHERE UserId = " + userId.ToString();

        return _dapper.LoadData<UserJobInfo>(sql);
    }


    //Add Single UserJobInfo Record:
    [HttpPost("AddUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
    {
        string sql = @"
                        INSERT INTO TutorialAppSchema.UserJobInfo (
                            Department
                            , JobTitle
                        )
                        VALUES(
                            '" + userJobInfo.Department
                            + "', '" + userJobInfo.JobTitle + "')";

        if (_dapper.ExecuteSqlWithRowCount(sql) > 0)
        {
            return Ok();
        }
        throw new Exception("Could not create the user job info");
    }




    //Edit Single UserJobInfo Record:
    [HttpPut("EditSingleUserJobInfo/{userId}")]
    public IActionResult EditSingleUserJobInfo(UserJobInfo userJobInfo)
    {
        string sql = @"
                    UPDATE TutorialAppSchema.UserJobInfo SET 
                    Department = '" + userJobInfo.Department
                    + "', JobTitle = '" + userJobInfo.JobTitle
                    + "' WHERE userId = " + userJobInfo.UserId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Could not Update the user job info");
    }


    //Delete UserJobInfo
    [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        string sql = @"
                        DELETE FROM TutorialAppSchema.UserJobInfo
                        WHERE UserId = " + userId.ToString();



        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Could not delete user");
    }


}

