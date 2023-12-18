using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[Controller]")]
public class PostCompleteController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    public PostCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }


    //Get All the Posts 
    [HttpGet("GetAllPosts/{postId}/{userId}/{searchParameters}")]
    public IEnumerable<Post> GetAllPosts(int postId = 0, int userId = 0, string searchParameters = "None")
    {
        string sql = @"EXEC TutorialAppSchema.spPosts_Get
            ";

        string parameters = "";

        if (postId != 0)
        {
            parameters += ", @PostId=" + postId.ToString();
        }
        if (userId != 0)
        {
            parameters += ", @UserId=" + userId.ToString();
        }
        if (searchParameters != "None")
        {
            parameters += ", @SearchValue='" + searchParameters + "'";
        }

        if (parameters.Length > 0)
        {
            sql += parameters.Substring(1);
        }


        IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);

        return posts;
    }




    //Get All of My Post for a LoggedIn User:
    [HttpGet("MyPost")]
    public IEnumerable<Post> GetAllPostByLoggedInUser()
    {
        string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId = " + this.User.FindFirst("userId")?.Value; //Retrieving the UserId from the Logged-in token (User here is from ControllerBase) That is this.User.FindFirst AND NOT DotnetAPI.Models.User

        return _dapper.LoadData<Post>(sql);

    }









    //Enabling a Logged-In User to Create a Post:
    [HttpPut("UpsertPost")]
    public IActionResult UpsertPost(Post postToUpsert)
    {
        string sql = @"EXEC TutorialAppSchema.spPosts_Upsert 
            @UserId = " + this.User.FindFirst("userId")?.Value +
            ", @PostTitle = '" + postToUpsert.PostTitle +
            "', @PostContent = '" + postToUpsert.PostContent + "'";

        if (postToUpsert.PostId > 0)
        {
            sql += ", @PostId = " + postToUpsert.PostId; //Because we have already added a Value for @UserId Parameter, every other following parameter should have a Comma 
        }

        if (_dapper.ExecuteSql(sql))
        {
            return Ok("Successfully Upsert-ed Post!");
        }
        throw new Exception("Failed to Upsert Post!");
    }




    //Deleting a Post for a Logged In User:
    [HttpDelete("DeletePost/{postId}")]
    public IActionResult DeletePost(int postId)
    {
        // string sql = @"DELETE FROM TutorialAppSchema.Posts WHERE PostId = " + postId.ToString();

        string sql = @"EXEC TutorialAppSchema.spPost_Delete @PostId = " + postId.ToString() +
                    ", @UserId = " + this.User.FindFirst("userId")?.Value;

        if (_dapper.ExecuteSql(sql))
        {
            return Ok("Successfully Deleted Post!");
        }
        throw new Exception("Could not Delete Post!");

    }






}



