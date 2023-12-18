using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthCompleteController : ControllerBase
    {
        private readonly DataContextDapper _dapper;


        private readonly AuthHelper _authHelper;

        public AuthCompleteController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);

            _authHelper = new AuthHelper(config);
        }


        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            //Check if the Password and ConfirmPassword are a match before continuing further:
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                //Check if the User with the same email already exists on the DB:
                string sqlCheckUserExists = "SELECT * FROM TutorialAppSchema.Auth WHERE Email = '" +
                 userForRegistration.Email + "'";

                //Because we are interested in pulling form only one field (Email) for this check, we will pull it as a <String>
                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);

                //If the IEnumerable has any string value returned, then we know that the user already exists:
                if (existingUsers.Count() == 0)
                {
                    UserForLoginDto userForSetPassword = new UserForLoginDto()
                    {
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password
                    };



                    if (_authHelper.SetPassword(userForSetPassword))
                    {
                        //Create Our User's Record:
                        string sqlAddUser = @"EXEC TutorialAppSchema.spUser_Upsert
                            @FirstName = '" + userForRegistration.FirstName +
                            "', @LastName = '" + userForRegistration.LastName +
                            "', @Email = '" + userForRegistration.Email +
                            "', @Gender = '" + userForRegistration.Gender +
                            "', @Active = 1" +
                            ", @JobTitle = '" + userForRegistration.JobTitle +
                            "', @Department = '" + userForRegistration.Department +
                            "', @Salary = '" + userForRegistration.Salary + "'";



                        //Adding the New User to the DataBase:
                        if (_dapper.ExecuteSql(sqlAddUser))
                        {

                            return Ok();
                        }
                        throw new Exception("Failed to Add User");
                    }
                    throw new Exception("Failed to register User");

                }
                throw new Exception("User already exists with the email");
            }
            throw new Exception("Password Does not Match!");
        }


        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
        {
            if (_authHelper.SetPassword(userForSetPassword))
            {
                return Ok();
            }
            throw new Exception("Failed to Update Password!");
        }


        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            //String for fetching the PasswordHash and PasswordSalt from the DB:
            string sqlForHashAndSalt = @"EXEC TutorialAppSchema.spLoginConfirmation_Get @Email = @EmailParam";

            //Creating a DynamicParameter object for our SqlVariable Parameters:
            DynamicParameters sqlParameters = new DynamicParameters();

            //Naming and Adding our Parameters on the Fly:
            sqlParameters.Add("@EmailParam", userForLogin.Email);


            //Calling Our DB with the Parameter Object:
            UserForLoginConfirmationDto userForConfirmation = _dapper.LoadDataSingleWithParameters<UserForLoginConfirmationDto>(sqlForHashAndSalt, sqlParameters);



            //Creating Our Hash by calling the Helper Method::
            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);



            //Now, we need to compare the two HashedPasswords: ==> But, since the Hashes are Arrays/List of bytes:
            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForConfirmation.PasswordHash[index])
                {
                    //throw new Exception("Incorrect Password");
                    return StatusCode(401, "Incorrect Password");
                }
            }


            //Sql for the UserId:
            string userIdSql = @"SELECT userId FROM TutorialAppSchema.Users WHERE Email = '" + userForLogin.Email + "'";

            //Defining out userId for Creation of Token:
            int userId = _dapper.LoadDataSingle<int>(userIdSql);


            return Ok(new Dictionary<string, string>{
                {"token", _authHelper.CreateToken(userId)}
            });
        }



        //Endpoint for refreshing our Token when it expires:
        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            //User.FindFirst("userId")?.Value looks at the Token of the Logged-in user and compares the UserId used for it's claim
            string userIdSql = @"
                SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '" +
                this.User.FindFirst("userId")?.Value + "'";


            //Using our Confirmed UserId returned from the DB check to create a new token:
            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return _authHelper.CreateToken(userId);
        }

    }
}