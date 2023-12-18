using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Data;

public class DataContextDapper
{
    private readonly IConfiguration _config;
    public DataContextDapper(IConfiguration config)
    {
        _config = config;
    }

    //Load Data Method - Multiple:
    public IEnumerable<T> LoadData<T>(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        return dbConnection.Query<T>(sql);
    }



    //Load Data Method - Single:
    public T LoadDataSingle<T>(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        return dbConnection.QuerySingle<T>(sql);
    }

    //Execute Add Data record to DB, and Tell Us if it was successful:
    public bool ExecuteSql(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        return dbConnection.Execute(sql) > 0;
    }

    //Execute Add Data record to DB, and Tell Us how many rows were affected:
    public int ExecuteSqlWithRowCount(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        return dbConnection.Execute(sql);
    }


    //Execute Add Data record to DB WITH PARAMETERS, and Tell Us if it was successful:
    // public bool ExecuteSqlWithParameters(string sql, List<SqlParameter> parameters)
    // {
    //     //The Command to be run by this method:
    //     SqlCommand commandWithParams = new SqlCommand(sql);

    //     foreach (SqlParameter parameter in parameters)
    //     {
    //         commandWithParams.Parameters.Add(parameter);
    //     }


    //     //We user SqlConnection here instead of IDbConnection:
    //     SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
    //     dbConnection.Open(); //Open the connection so that we can go inside and execute the Command

    //     commandWithParams.Connection = dbConnection;

    //     //Returning the number of Rows Affected:
    //     int rowsAffected = commandWithParams.ExecuteNonQuery();


    //     return rowsAffected > 0;
    // }


    //Better Alternative for ExecuteWithParameters ===> Using Dynamic Object Parameters:
    public bool ExecuteSqlWithParameters(string sql, DynamicParameters parameters)
    {
        IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        return dbConnection.Execute(sql, parameters) > 0;
    }



    //Working with Parameterized Queries:
    //Load Data Method - Multiple:
    public IEnumerable<T> LoadDataWithParameters<T>(string sql, DynamicParameters parameters)
    {
        IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        return dbConnection.Query<T>(sql, parameters);
    }



    //Load Data Method - Single:
    public T LoadDataSingleWithParameters<T>(string sql, DynamicParameters parameters)
    {
        IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        return dbConnection.QuerySingle<T>(sql, parameters);
    }


    // public T LoadDataSingleWithParameters<T>(string sql, DynamicParameters parameters)
    //     {
    //         using (IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
    //         {
    //             dbConnection.Open();
    //             return dbConnection.QuerySingle<T>(sql, parameters);
    //         }
    //     }


    

}