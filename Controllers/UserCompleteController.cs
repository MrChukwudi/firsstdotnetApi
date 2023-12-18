
using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;


namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    private readonly DataContextDapper _dapper;



    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);

    }


    // [HttpGet("TestConnection")]
    // public DateTime TestConnection()
    // {
    //     return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    // }


    [HttpGet("GetUsers")]
    public IEnumerable<UserComplete> GetUsers()
    {
        string sql = @"Exec TutorialAppSchema.spUsers_Get"; //We just Execute Our sTored Procedure here

        //To Pass Parameters to the Stored Procedure:



        IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
        return users;


    }

    [HttpGet("GetSingleUser/{userId}")]
    public UserComplete GetSingleUser(int userId) //Changing User to UserComplete Model Here:
    {
        //Unless you return this as an IEnumerable, you'll be running into errors when you tey to filter by a parameter that does not exist.. Because, the Procedure will try to return all the records:
        //Simple Filter Scenario:
        //string sql = @"Exec TutorialAppSchema.spUsers_Get @UserId=" + userId.ToString(); //We just Execute Our sTored Procedure here

        //Complex Filter Scenario: I.E When working with more than one Filter:
        string sql = @"Exec TutorialAppSchema.spUsers_Get";
        //Another way to Work with Multiple Filters are:
        if (userId != 0) //If we set the userId  = 0 then we wil be returning all the records/instances in the DB.
        {
            sql += " @UserId=" + userId.ToString();
        }

        UserComplete singleUser = _dapper.LoadDataSingle<UserComplete>(sql);
        return singleUser;

    }



    /*
    The Method has an Issue in that it Is returning a sSIngle record instead of an IEnumerable: Though we are interested in a single record, it should be an IEnumerable of a Single record:
    */
    // [HttpGet("GetSingleUserIsActive/{userId}/{active}")]
    // public UserComplete GetSingleUserIsActive(int userId, bool active) //Changing User to UserComplete Model Here:
    // {
    //     //Simple Filter Scenario:
    //     //string sql = @"Exec TutorialAppSchema.spUsers_Get @UserId=" + userId.ToString(); //We just Execute Our sTored Procedure here

    //     //Complex Filter Scenario: I.E When working with more than one Filter:
    //     string sql = @"Exec TutorialAppSchema.spUsers_Get";

    //     //To hold our Parameters:
    //     string parameters = "";


    //     //Another way to Work with Multiple Filters are: Firstly make sure that the filter is not Null.
    //     if(userId != 0) //If we set the @userId  = 0 then we wil be returning all the records/instances in the DB.
    //     {
    //         parameters += ", @UserId=" + userId.ToString();
    //     }
    //     if(active) //If we set the @active  = 0 then we wil be returning the record/instance from the DB iff it is active.
    //     {
    //         parameters += ", @Active=" + active.ToString();
    //     }
    //     /*
    //     Because we need a way to add comma separator between the two filters, we will need to add a leading coma to the Filters and 
    //     then add all the filters to a String, 
    //     Then return the SubString of the string without the Leading comma separator.

    //     Because, when a filter is not supplied, we don't want errors:
    //     string parameters = "";
    //     parameters += filter;
    //     sql += parameters.Substring(1, filter.Length); starting from Index (1) till the length of the string:
    //     */


    //     sql += parameters.Substring(1); //, filter.Length);



    //     //Just to investigate our SQL:
    //     Console.WriteLine(sql);

    //     UserComplete singleUser = _dapper.LoadDataSingle<UserComplete>(sql);
    //     return singleUser;

    // }




    [HttpGet("GetSingleUserActive/{userId}/{active}")]
    public IEnumerable<UserComplete> GetSingleUserActive(int userId, bool active) //Changing User to UserComplete Model Here:
    {
        //Simple Filter Scenario:
        //string sql = @"Exec TutorialAppSchema.spUsers_Get @UserId=" + userId.ToString(); //We just Execute Our sTored Procedure here

        //Complex Filter Scenario: I.E When working with more than one Filter:
        string sql = @"Exec TutorialAppSchema.spUsers_Get";

        //To hold our Parameters:
        string parameters = "";


        //Another way to Work with Multiple Filters are: Firstly make sure that the filter is not Null.
        if (userId != 0) //If we set the @userId  = 0 then we wil be returning all the records/instances in the DB.
        {
            parameters += ", @UserId=" + userId.ToString();
        }
        if (active) //If we set the @active  = 0 then we wil be returning the record/instance from the DB iff it is active.
        {
            parameters += ", @Active=" + active.ToString();
        }
        /*
        Because we need a way to add comma separator between the two filters, we will need to add a leading coma to the Filters and 
        then add all the filters to a String, 
        Then return the SubString of the string without the Leading comma separator.

        Because, when a filter is not supplied, we don't want errors:
        string parameters = "";
        parameters += filter;
        sql += parameters.Substring(1, filter.Length); starting from Index (1) till the length of the string:
        */


        sql += parameters.Substring(1); //, filter.Length);


        IEnumerable<UserComplete> singleUser = _dapper.LoadData<UserComplete>(sql);
        return singleUser;

    }

    //To Edit a User in Our Database:
    [HttpPut("UpsertUser")]
    public IActionResult EditUser(UserComplete user)
    {
        string sql = @"EXEC TutorialAppSchema.spUser_Upsert
            @FirstName = '" + user.FirstName +
            "', @LastName = '" + user.LastName +
            "', @Email = '" + user.Email +
            "', @Gender = '" + user.Gender +
            "', @Active = '" + user.Active +
            "', @JobTitle= '" + user.JobTitle +
            "', @Department = '" + user.Department +
            "', @Salary = '" + user.Salary +
            "', @UserId = " + user.UserId.ToString();
        Console.WriteLine(sql);


        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Update User");
    }





    //To Delete a User from Our Database:
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"EXEC TutorialAppSchema.spUser_Delete
            @UserId = " + userId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete User");
    }


}

