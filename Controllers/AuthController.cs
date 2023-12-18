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
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;


        private readonly AuthHelper _authHelper;

        public AuthController(IConfiguration config)
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

                    //Creating Our Password Salt:
                    byte[] passwordSalt = new byte[128 / 8]; //Setting the size to 128 bits


                    //Creating a RandomNumberGenerator for the purpose of generating our Random Number to be used as Salt :
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())


                        //Here we use the RandomNumberGenerator rng to generate a Non-Zero-Byte that will become our Salt, and assign it as our PasswordSalt:
                        rng.GetNonZeroBytes(passwordSalt);



                    //Creating Our Hash by calling the Helper Method::
                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);



                    //Sql for adding our User Credentials to the DB:
                    string sqlAddAuth = @"
                                INSERT INTO TutorialAppSchema.Auth([Email], [PasswordHash], [PasswordSalt]) VALUES ('" + userForRegistration.Email + "', @PasswordHash, @PasswordSalt)";

                    //Creating a List to hold our SqlVariable Parameters:
                    // List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    // //Mapping Values to our SQL-Variables ==> PasswordSalt:
                    // SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", System.Data.SqlDbType.VarBinary);
                    // passwordSaltParameter.Value = passwordSalt;

                    // //Mapping Values to our SQL-Variables ==> PasswordHash:
                    // SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", System.Data.SqlDbType.VarBinary);
                    // passwordHashParameter.Value = passwordHash;

                    // //Adding our SqlParameters to the List above:
                    // sqlParameters.Add(passwordSaltParameter);
                    // sqlParameters.Add(passwordHashParameter);



                    //Replacing the Good-Old SqlParameters List with our DynamicParameters Object:
                    DynamicParameters sqlParameters = new DynamicParameters();

                    sqlParameters.Add("@PasswordHash", passwordHash, System.Data.DbType.Binary);
                    sqlParameters.Add("@PasswordSalt", passwordSalt, System.Data.DbType.Binary);


                    //Executing the Sql
                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                    {
                        //Create Our User's Record:
                        string sqlAddUser = @"
                                    INSERT INTO TutorialAppSchema.Users(
                                        [FirstName],
                                        [LastName],
                                        [Email],
                                        [Gender],
                                        [Active]
                                    ) VALUES(
                                        '" + userForRegistration.FirstName +
                                        "', '" + userForRegistration.LastName +
                                        "', '" + userForRegistration.Email +
                                        "', '" + userForRegistration.Gender +
                                        "', 1)";



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



        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            //String for fetching the PasswordHash and PasswordSalt from the DB:
            string sqlForHashAndSalt = @"SELECT [PasswordHash], [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = '" + userForLogin.Email + "' ";

            UserForLoginConfirmationDto userForConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);


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