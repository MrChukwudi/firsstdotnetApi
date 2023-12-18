

using DotnetAPI.Models;

namespace DotnetAPI.Data;
public class UserRepository : IUserRepository
{
     DataContextEF _entityFramework;


    //Using the Constructor to Create a Connection to EF in our Repository:
    public UserRepository(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);
        
    }

    //Creating our Method for Checking if _entityFramework.SaveChanges() is true(affected more than 0 line)
    public bool SaveChanges()
    {
        return _entityFramework.SaveChanges() > 0;
    }


    //Creating our Method for Adding any Record to our [_entityFramework.Add()]
    public void AddEntity<T>(T entityToAdd)
    {
        //To Handle the Nullable warning:
         if(entityToAdd != null)
         {
            _entityFramework.Add(entityToAdd);
         }
    }


    //Alternatively: Creating our Method for Adding any Record to our [_entityFramework.Add()]
    public bool AddEntityBool<T>(T entityToAdd)
    {
        //To Handle the Nullable warning:
         if(entityToAdd != null)
         {
            _entityFramework.Add(entityToAdd);
            return true;
         }
         return false;
    }

    //Creating our Method for Removing any Record to our [_entityFramework.Add()]
    public void RemoveEntity<T>(T entityToRemove)
    {
        //To Handle the Nullable warning:
         if(entityToRemove != null)
         {
            _entityFramework.Remove(entityToRemove);
         }
    }

     public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _entityFramework.Users.ToList<User>();
        return users;

    }

    public IEnumerable<UserSalary> GetUsersSalary()
    {
        IEnumerable<UserSalary> usersSalary = _entityFramework.UserSalary.ToList<UserSalary>();
        return usersSalary;

    }

    public IEnumerable<UserJobInfo> GetUsersJobInfo()
    {
        IEnumerable<UserJobInfo> userJobInfos = _entityFramework.UserJobInfo.ToList<UserJobInfo>();
       
        return userJobInfos;

    }


    public User GetSingleUser(int userId)
    {

        User? singleUser = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();
        if (singleUser != null)
        {
            return singleUser;
        }
        throw new Exception("Failed to Get User");

    }


     public UserSalary GetSingleUserSalary(int userId)
    {

        UserSalary? singleUserSalary = _entityFramework.UserSalary.Where(u => u.UserId == userId).FirstOrDefault<UserSalary>();
        if (singleUserSalary != null)
        {
            return singleUserSalary;
        }
        throw new Exception("Failed to Get User");

    }


     public UserJobInfo GetSingleUserJobInfo(int userId)
    {

        UserJobInfo? singleUserJobInfo = _entityFramework.UserJobInfo.Where(u => u.UserId == userId).FirstOrDefault<UserJobInfo>();
        if (singleUserJobInfo != null)
        {
            return singleUserJobInfo;
        }
        throw new Exception("Failed to Get User");

    }
}

