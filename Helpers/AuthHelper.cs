using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers;

public class AuthHelper
{

    private readonly IConfiguration _config;

    private readonly DataContextDapper _dapper;

    public AuthHelper(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _config = config;
    }



    //Abstracting Our PasswordHashing and Salt Generation Functions inside a Helper Method, to avoid repetitions:
    public byte[] GetPasswordHash(string thePassword, byte[] passwordSalt)
    {
        //Getting our PasswordKey String from our Config File to make our PasswordHashing even more stronger:
        string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

        //Creating Our Hash and returning it:
        return KeyDerivation.Pbkdf2(
            password: thePassword,
            salt: Encoding.ASCII.GetBytes(passwordSaltPlusString), //Converting our PasswordSaltPlusString back into Bytes
            prf: KeyDerivationPrf.HMACSHA256, // Our Hashing Schema (prf ==> Pseudo-Random-functionality = How random our hashing is)
            iterationCount: 100000, // The number of iterations of our Hashing operation:
            numBytesRequested: 256 / 8 // The number of bytes requested from
        );
    }



    //Helper Method to return a Token String that we want to Pass back to the User:
    public string CreateToken(int userId)
    {
        //Defining our Claims: Claims are used to Identify the identity of an Authenticated User, it forms an integral part of building the UserToken object
        Claim[] claims = new Claim[]
        {
                new Claim("userId", userId.ToString())
        };

        //Setting Up the Signature: [Key, Signer, TokenBuilder(Key, Signer)]

        //1. SymmetricSecurityKey: Will use our TokenKey from the AppConfig.Json file to create a Real TokenKey that will be used to Sign our token.
        string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;



        SymmetricSecurityKey? tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeyString != null ? tokenKeyString : ""));



        //2. SigningCredentials ==> For Signing our Token using a SigningAlgorithm
        SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);

        //3. Descriptor of How We want our Tokens to behave in our System:
        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials,
            Expires = DateTime.Now.AddDays(1)
        };



        //4. Setting Up Our Token Handler; which will use the Descriptor to Create our Token: This class will Return a Security Token
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        //5. Storing Our Token as A token from the returned SecurityToken:
        SecurityToken token = tokenHandler.CreateToken(descriptor);

        //6. Converting Our Token to A String:
        return tokenHandler.WriteToken(token);

    }

    public bool SetPassword(UserForLoginDto userForSetPassword)
    {
        //Creating Our Password Salt:
        byte[] passwordSalt = new byte[128 / 8]; //Setting the size to 128 bits


        //Creating a RandomNumberGenerator for the purpose of generating our Random Number to be used as Salt :
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())


            //Here we use the RandomNumberGenerator rng to generate a Non-Zero-Byte that will become our Salt, and assign it as our PasswordSalt:
            rng.GetNonZeroBytes(passwordSalt);



        //Creating Our Hash by calling the Helper Method::
        byte[] passwordHash = GetPasswordHash(userForSetPassword.Password, passwordSalt);



        //Sql for adding our User Credentials to the DB:
        string sqlAddAuth = @"EXEC TutorialAppSchema.spRegistration_Upsert 
                        @Email = @EmailParam, 
                        @PasswordHash =@PasswordHashParam, 
                        @PasswordSalt =@PasswordSaltParam";

        //Creating a List to hold our SqlVariable Parameters:
        // List<SqlParameter> sqlParameters = new List<SqlParameter>();


        // //Mapping Values to our SQL-Variables ==> Email:
        // SqlParameter emailParameter = new SqlParameter("@EmailParam", System.Data.SqlDbType.VarChar);
        // emailParameter.Value = userForSetPassword.Email;
        // sqlParameters.Add(emailParameter);   //Adding our SqlParameters to the List above:


        // //Mapping Values to our SQL-Variables ==> PasswordHash:
        // SqlParameter passwordHashParameter = new SqlParameter("@PasswordHashParam", System.Data.SqlDbType.VarBinary);
        // passwordHashParameter.Value = passwordHash;
        // sqlParameters.Add(passwordHashParameter);  //Adding our SqlParameters to the List above:



        // //Mapping Values to our SQL-Variables ==> PasswordSalt:
        // SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSaltParam", System.Data.SqlDbType.VarBinary);
        // passwordSaltParameter.Value = passwordSalt;
        // sqlParameters.Add(passwordSaltParameter);   //Adding our SqlParameters to the List above:

        //Replacing the Good-Old SqlParameters List with our DynamicParameters Object:
        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@EmailParam", userForSetPassword.Email, System.Data.DbType.String);
        sqlParameters.Add("@PasswordHashParam", passwordHash, System.Data.DbType.Binary);
        sqlParameters.Add("@PasswordSaltParam", passwordSalt, System.Data.DbType.Binary);






        //Executing the Sql
        return _dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters);

    }
}