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
public class PostController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    public PostController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }


    //Get All the Posts 
    [HttpGet("GetAllPosts")]
    public IEnumerable<Post> GetAllPosts()
    {
        string sql = @"SELECT [PostId],
                [UserId], 
                [PostTitle], 
                [PostContent], 
                [PostCreated], 
                [PostUpdated] 
            FROM TutorialAppSchema.Posts";



        IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);

        return posts;
    }

    //Get A Single Post By it's Id
    [HttpGet("GetSinglePosts/{postId}")]
    public Post GetASinglePosts(int postId)
    {
        string sql = @"SELECT [PostId],
                [UserId], 
                [PostTitle], 
                [PostContent], 
                [PostCreated], 
                [PostUpdated] 
            FROM TutorialAppSchema.Posts WHERE PostId = " + postId.ToString();



        return _dapper.LoadDataSingle<Post>(sql);


    }


    //Get All Post for a Single User:
    [HttpGet("AllPostByUser/{userId}")]
    public IEnumerable<Post> GetAllPostByUser(int userId)
    {
        string sql = @"SELECT [PostId],
                [UserId], 
                [PostTitle], 
                [PostContent], 
                [PostCreated], 
                [PostUpdated] 
            FROM TutorialAppSchema.Posts WHERE UserId = " + userId.ToString();

        return _dapper.LoadData<Post>(sql);

    }


    //Get All of My Post for a LoggedIn User:
    [HttpGet("AllPostByLoggedInUser")]
    public IEnumerable<Post> GetAllPostByLoggedInUser()
    {
        string sql = @"SELECT [PostId],
                [UserId], 
                [PostTitle], 
                [PostContent], 
                [PostCreated], 
                [PostUpdated] 
            FROM TutorialAppSchema.Posts WHERE UserId = " + this.User.FindFirst("userId")?.Value; //Retrieving the UserId from the Logged-in token (User here is from ControllerBase) That is this.User.FindFirst AND NOT DotnetAPI.Models.User

        return _dapper.LoadData<Post>(sql);

    }



    //Get a Post by it's (Content Search Parameter) or (Title Search Parameter):
    [HttpGet("PostsBySearch/{searchParam}")]
        public IEnumerable<Post> PostsBySearch(string searchParam)
        {
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                    WHERE PostTitle LIKE '%" + searchParam + "%'" +
                        " OR PostContent LIKE '%" + searchParam + "%'";
                
            return _dapper.LoadData<Post>(sql);
        }





    //Enabling a Logged-In User to Create a Post:
    [HttpPost("AddPost")]
    public IActionResult AddPost(PostToAddDto postToAdd)
    {
        string sql = @"INSERT INTO TutorialAppSchema.Posts ( 
            [UserId], 
            [PostTitle], 
            [PostContent], 
            [PostCreated], 
            [PostUpdated]
        ) VALUES(" + this.User.FindFirst("userId")?.Value
        + ", '" + postToAdd.PostTitle
        + "', '" + postToAdd.PostContent
        + "', GETDATE(), GETDATE() )"; //Here, we are telling the DataService to GetTheDate For us.


        if (_dapper.ExecuteSql(sql))
        {
            return Ok("Successfully Added Post!");
        }
        throw new Exception("Failed to Add Post!");
    }


    //Enabling a Logged-In User to Edit a Post:
    [HttpPut("EditPost")]
    public IActionResult EditPost(PostToEditDto postToEdit)
    {
        string sql = @"
        UPDATE TutorialAppSchema.Posts SET 
            PostContent = '" + postToEdit.PostContent
            + "', PostTitle = '" + postToEdit.PostTitle
            + "', PostUpdated =  GETDATE() WHERE PostId = " + postToEdit.PostId.ToString() +
            " AND UserId = " + this.User.FindFirst("userId")?.Value;


        if (_dapper.ExecuteSql(sql))
        {
            return Ok("Successfully Edited Post!");
        }
        throw new Exception("Failed to Edit Post!");
    }



    //Deleting a Post for a Logged In User:
    [HttpDelete("DeletePost/{postId}")]
    public IActionResult DeletePost(int postId)
    {
        // string sql = @"DELETE FROM TutorialAppSchema.Posts WHERE PostId = " + postId.ToString();

        string sql = @"DELETE FROM TutorialAppSchema.Posts 
                WHERE PostId = " + postId.ToString() +
                    " AND UserId = " + this.User.FindFirst("userId")?.Value;

        if (_dapper.ExecuteSql(sql))
        {
            return Ok("Successfully Deleted Post!");
        }
        throw new Exception("Could not Delete Post!");

    }






}



