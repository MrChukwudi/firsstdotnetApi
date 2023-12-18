
using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.DTOs;

using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Text.Json.Serialization;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    DataContextEF _entityFramework;
    IUserRepository _userRepository;
    IMapper _mapper;


    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {
        _entityFramework = new DataContextEF(config);


        _userRepository = userRepository;


        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserToAddDto, User>();
            cfg.CreateMap<UserSalary, UserSalary>();
            cfg.CreateMap<UserJobInfo, UserJobInfo>();
        }));
    }





    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _userRepository.GetUsers();
        return users;


    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {

        return _userRepository.GetSingleUser(userId);

    }

    //To Edit a User in Our Database:
    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userFromDB = _userRepository.GetSingleUser(user.UserId);

        if (userFromDB != null)
        {
            userFromDB.Active = user.Active;
            userFromDB.FirstName = user.FirstName;
            userFromDB.LastName = user.LastName;
            userFromDB.Email = user.Email;
            userFromDB.Gender = user.Gender;

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to Update User");

        }
        throw new Exception("Failed to Get User");
    }


    //To Add a new User to our Database:
    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        /*
        User userToDB = new User();

        userToDB.Active = user.Active;
        userToDB.FirstName = user.FirstName;
        userToDB.LastName = user.LastName;
        userToDB.Email = user.Email;
        userToDB.Gender = user.Gender;
        */

        //Using AutoMapper: => The (UserToAddDto user) that is passed in, compare its fields and map their values to corresponding fields in our <User> Model:
        User userToDB = _mapper.Map<User>(user);

        _userRepository.AddEntity<User>(userToDB);

        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        throw new Exception("Failed to add User");
    }


    //To Delete a User from Our Database:
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userFromDB = _userRepository.GetSingleUser(userId);

        if (userFromDB != null)
        {
            _userRepository.RemoveEntity<User>(userFromDB);


            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to Delete User");

        }
        throw new Exception("Failed to Get User");


    }

    //To Get All UserSalary:
    [HttpGet("GetAllUserSalary")]
    public IActionResult GetAllSalary()
    {
        try
        {

            IEnumerable<UserSalary> usersSalary = _userRepository.GetUsersSalary();
            return Ok(usersSalary);

        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }


    //To Get Single UserSalary Record:
    [HttpGet("GetSingleUserSalary/{userId}")]
    public IActionResult GetSingleUserSalary(int userId)
    {
        try
        {
            UserSalary? userSalary = _userRepository.GetSingleUserSalary(userId);

            return Ok(userSalary);

        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }



    //Create New UserSalary Record:
    [HttpPost("CreateNewUserSalary")]
    public IActionResult CreateNewUserSalary(UserSalary userSalary)
    {
        try
        {
            _userRepository.AddEntity<UserSalary>(userSalary);
            if (_userRepository.SaveChanges())
            {
                return Ok("UserSalary created successfully.");
            }
            throw new Exception("Adding UserSalary Failed on Save");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    //To Edit a UserSalary Record:
    [HttpPut("EditUserSalary/{userId}")]
    public IActionResult EditUserSalary(UserSalary userSalaryForUpdate)
    {
        UserSalary? userSalaryToUpdate = _userRepository.GetSingleUserSalary(userSalaryForUpdate.UserId);
        if (userSalaryToUpdate != null)
        {
            _mapper.Map(userSalaryForUpdate, userSalaryToUpdate);
            if (_userRepository.SaveChanges())
            {
                return Ok("Salary updated successfully");
            }
            throw new Exception("Updating UserSalary Failed on Save");
        }
        throw new Exception("Could not find the userSalary for update record");
    }


    //Delete a UserSalaryRecord from the database
    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        UserSalary? userSalaryToDelete = _userRepository.GetSingleUserSalary(userId);
        if (userSalaryToDelete != null)
        {
            _userRepository.RemoveEntity<UserSalary>(userSalaryToDelete);
            if (_userRepository.SaveChanges())
            {
                return Ok("Salary deleted successfully");
            }
            throw new Exception("UserSalary Failed on Delete On Save");
        }
        throw new Exception("UserSalary to be Deleted not Found");
    }


    [HttpGet("UserJobInfo/{userId}")]
    public UserJobInfo GetUserJobInfoEF(int userId)
    {
        return _userRepository.GetSingleUserJobInfo(userId);
    }




    [HttpGet("UserJobInfo")]
    public IEnumerable<UserJobInfo> GetUsersJobInfo()
    {
        return _userRepository.GetUsersJobInfo();
    }




    [HttpPost("UserJobInfo")]
    public IActionResult PostUserJobInfoEf(UserJobInfo userForInsert)
    {
        _userRepository.AddEntity<UserJobInfo>(userForInsert);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        throw new Exception("Adding UserJobInfo failed on save");
    }


    [HttpPut("UserJobInfo")]
    public IActionResult PutUserJobInfoEf(UserJobInfo userForUpdate)
    {
        UserJobInfo? userToUpdate = _userRepository.GetSingleUserJobInfo(userForUpdate.UserId);

        if (userToUpdate != null)
        {
            _mapper.Map(userForUpdate, userToUpdate);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Updating UserJobInfo failed on save");
        }
        throw new Exception("Failed to find UserJobInfo to Update");
    }


    [HttpDelete("UserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfoEf(int userId)
    {
        UserJobInfo? userToDelete = _userRepository.GetSingleUserJobInfo(userId);

        if (userToDelete != null)
        {
            _userRepository.RemoveEntity<UserJobInfo>(userToDelete);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Deleting UserJobInfo failed on save");
        }
        throw new Exception("Failed to find UserJobInfo to delete");
    }


    //  public void ImplementationWithoutRepositories()
    //{
    // //[Just remove these implementation from the enclosing method to use:]


    // //Our Controller Logics Without Repository Pattern:


    // [HttpGet("GetUsers")]
    // public IEnumerable<User> GetUsers()
    // {
    //     IEnumerable<User> users = _entityFramework.Users.ToList<User>();
    //     return users;

    // }

    // [HttpGet("GetSingleUser/{userId}")]
    // public User GetSingleUser(int userId)
    // {

    //     User? singleUser = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();
    //     if (singleUser != null)
    //     {
    //         return singleUser;
    //     }
    //     throw new Exception("Failed to Get User");

    // }

    // //To Edit a User in Our Database:
    // [HttpPut("EditUser")]
    // public IActionResult EditUser(User user)
    // {
    //     User? userFromDB = _entityFramework.Users.Where(u => u.UserId == user.UserId).FirstOrDefault<User>();

    //     if (userFromDB != null)
    //     {
    //         userFromDB.Active = user.Active;
    //         userFromDB.FirstName = user.FirstName;
    //         userFromDB.LastName = user.LastName;
    //         userFromDB.Email = user.Email;
    //         userFromDB.Gender = user.Gender;

    //         if (_entityFramework.SaveChanges() > 0)
    //         {
    //             return Ok();
    //         }
    //         throw new Exception("Failed to Update User");

    //     }
    //     throw new Exception("Failed to Get User");
    // }


    // //To Add a new User to our Database:
    // [HttpPost("AddUser")]
    // public IActionResult AddUser(UserToAddDto user)
    // {
    //     /*
    //     User userToDB = new User();

    //     userToDB.Active = user.Active;
    //     userToDB.FirstName = user.FirstName;
    //     userToDB.LastName = user.LastName;
    //     userToDB.Email = user.Email;
    //     userToDB.Gender = user.Gender;
    //     */

    //     //Using AutoMapper: => The (UserToAddDto user) that is passed in, compare its fields and map their values to corresponding fields in our <User> Model:
    //     User userToDB = _mapper.Map<User>(user);

    //     _entityFramework.Add(userToDB);

    //     if (_entityFramework.SaveChanges() > 0)
    //     {
    //         return Ok();
    //     }
    //     throw new Exception("Failed to add User");
    // }


    // //To Delete a User from Our Database:
    // [HttpDelete("DeleteUser/{userId}")]
    // public IActionResult DeleteUser(int userId)
    // {
    //     User? userFromDB = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();

    //     if (userFromDB != null)
    //     {
    //         _entityFramework.Users.Remove(userFromDB);

    //         if (_entityFramework.SaveChanges() > 0)
    //         {
    //             return Ok();
    //         }
    //         throw new Exception("Failed to Delete User");

    //     }
    //     throw new Exception("Failed to Get User");


    // }

    // //To Get All UserSalary:
    // [HttpGet("GetAllUserSalary")]
    // public IActionResult GetAllSalary()
    // {
    //     try
    //     {
    //         IEnumerable<UserSalary> usersSalary = _entityFramework.UserSalary.ToList();
    //         return Ok(usersSalary);

    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest($"Error: {ex.Message}");
    //     }
    // }


    // //To Get Single UserSalary Record:
    // [HttpGet("GetSingleUserSalary/{userId}")]
    // public IActionResult GetSingleUserSalary(int userId)
    // {
    //     try
    //     {
    //         IEnumerable<UserSalary> userSalary = _entityFramework.UserSalary.Where(u => u.UserId == userId).ToList();

    //         return Ok(userSalary);

    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest($"Error: {ex.Message}");
    //     }
    // }



    // //Create New UserSalary Record:
    // [HttpPost("CreateNewUserSalary")]
    // public IActionResult CreateNewUserSalary(UserSalary userSalary)
    // {
    //     try
    //     {
    //         _entityFramework.UserSalary.Add(userSalary);
    //         if (_entityFramework.SaveChanges() > 0)
    //         {
    //             return Ok("UserSalary created successfully.");
    //         }
    //         throw new Exception("Adding UserSalary Failed on Save");
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest($"Error: {ex.Message}");
    //     }
    // }

    // //To Edit a UserSalary Record:
    // [HttpPut("EditUserSalary/{userId}")]
    // public IActionResult EditUserSalary(UserSalary userSalaryForUpdate)
    // {
    //     UserSalary? userSalaryToUpdate = _entityFramework.UserSalary.Where(u => u.UserId == userSalaryForUpdate.UserId).FirstOrDefault();
    //     if (userSalaryToUpdate != null)
    //     {
    //         _mapper.Map(userSalaryForUpdate, userSalaryToUpdate);
    //         if (_entityFramework.SaveChanges() > 0)
    //         {
    //             return Ok("Salary updated successfully");
    //         }
    //         throw new Exception("Updating UserSalary Failed on Save");
    //     }
    //     throw new Exception("Could not find the userSalary for update record");
    // }


    // //Delete a UserSalaryRecord from the database
    // [HttpDelete("DeleteUserSalary/{userId}")]
    // public IActionResult DeleteUserSalary(int userId)
    // {
    //     UserSalary? userSalaryToDelete = _entityFramework.UserSalary.Where(u => u.UserId == userId).FirstOrDefault();
    //     if (userSalaryToDelete != null)
    //     {
    //         _entityFramework.UserSalary.Remove(userSalaryToDelete);
    //         if (_entityFramework.SaveChanges() > 0)
    //         {
    //             return Ok("Salary deleted successfully");
    //         }
    //         throw new Exception("UserSalary Failed on Delete On Save");
    //     }
    //     throw new Exception("UserSalary to be Deleted not Found");
    // }


    // [HttpGet("UserJobInfo/{userId}")]
    // public IEnumerable<UserJobInfo> GetUserJobInfoEF(int userId)
    // {
    //     return _entityFramework.UserJobInfo
    //         .Where(u => u.UserId == userId)
    //         .ToList();
    // }

    // [HttpPost("UserJobInfo")]
    // public IActionResult PostUserJobInfoEf(UserJobInfo userForInsert)
    // {
    //     _entityFramework.UserJobInfo.Add(userForInsert);
    //     if (_entityFramework.SaveChanges() > 0)
    //     {
    //         return Ok();
    //     }
    //     throw new Exception("Adding UserJobInfo failed on save");
    // }


    // [HttpPut("UserJobInfo")]
    // public IActionResult PutUserJobInfoEf(UserJobInfo userForUpdate)
    // {
    //     UserJobInfo? userToUpdate = _entityFramework.UserJobInfo
    //         .Where(u => u.UserId == userForUpdate.UserId)
    //         .FirstOrDefault();

    //     if (userToUpdate != null)
    //     {
    //         _mapper.Map(userForUpdate, userToUpdate);
    //         if (_entityFramework.SaveChanges() > 0)
    //         {
    //             return Ok();
    //         }
    //         throw new Exception("Updating UserJobInfo failed on save");
    //     }
    //     throw new Exception("Failed to find UserJobInfo to Update");
    // }


    // [HttpDelete("UserJobInfo/{userId}")]
    // public IActionResult DeleteUserJobInfoEf(int userId)
    // {
    //     UserJobInfo? userToDelete = _entityFramework.UserJobInfo
    //         .Where(u => u.UserId == userId)
    //         .FirstOrDefault();

    //     if (userToDelete != null)
    //     {
    //         _entityFramework.UserJobInfo.Remove(userToDelete);
    //         if (_entityFramework.SaveChanges() > 0)
    //         {
    //             return Ok();
    //         }
    //         throw new Exception("Deleting UserJobInfo failed on save");
    //     }
    //     throw new Exception("Failed to find UserJobInfo to delete");
    // }




    //}



}