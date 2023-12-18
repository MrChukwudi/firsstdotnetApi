using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace DotnetAPI.Data
{
    public class DataContextEF : DbContext
    {
        //To have access to our Connection String property:
        private readonly IConfiguration _config;

        public DataContextEF(IConfiguration config)
        {
            _config = config;
        }




        //Setting Up Our Connection to the DB Using the OptionsBuilder:
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"), optionsBuilder => optionsBuilder.EnableRetryOnFailure());
            }
        }





        //To Create our DBSets (The Set of Tables in our DB) property: So that Our EF can have mapped access to Tables inside our DB: => public virtual DbSet<ModelName> DBTableName {get; set;}
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserJobInfo> UserJobInfo { get; set; }
        public virtual DbSet<UserSalary> UserSalary { get; set; }



        /*
       By Default, EF will be looking for our Tables inside the DB-O schema directory which is the Default Schema Directory, but because our Tables are actually created inside the TutorialsAppSchema Directory, we need to point EF to where our Tables are, hence the following:
       */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //Telling EF what our Project Schema Directory is: Access to the Rigth Schema => modelBuilder.HasDefaultSchema("ProjectSchemaName");
            modelBuilder.HasDefaultSchema("TutorialAppSchema");


            //Telling EF What our Project Table is: Access to the Rigth Table:
            //Telling the ModelBuilder that Our Entity<For User-Model>.IsGoingToHaveTheTableName("TableName", "ProjectSchemaName").HasAPrimaryKey(user => user.PrimaryKey)
            modelBuilder.Entity<User>().ToTable("Users", "TutorialAppSchema").HasKey(u => u.UserId);

            //Because these other Tables has the same Model Name as their Table Names, we don't need to specify the ToTable()Property:
            modelBuilder.Entity<UserJobInfo>().HasKey(u => u.UserId);
            modelBuilder.Entity<UserSalary>().HasKey(u => u.UserId);


        }
    }




}