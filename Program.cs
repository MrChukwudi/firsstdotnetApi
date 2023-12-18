using System.Text;
using DotnetAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


//Setting up Our API Application Builder

var builder = WebApplication.CreateBuilder(args);

// Add services for the controllers: Adds Our Endpoints to our Controller reference and make them discoverable by MapControllers()
builder.Services.AddControllers();




//Enabling Our App to discover Endpoint routes and effectively route incoming requests
builder.Services.AddEndpointsApiExplorer();



//Enabling Our App to Add the Swagger Service so that we can test Our API Service during development
builder.Services.AddSwaggerGen();

//Adding our CORS middleware Service:
builder.Services.AddCors((options) =>
{
    options.AddPolicy("DevCors", (corsBuilder) =>
        {
            corsBuilder.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:8000").AllowAnyMethod().AllowAnyHeader().AllowCredentials();

        });
    options.AddPolicy("ProdCors", (corsBuilder) =>
    {
        corsBuilder.WithOrigins("https://myProductionSite.com").AllowAnyMethod().AllowAnyHeader().AllowCredentials();

    });

});

/*
connecting our Repository : IRepository for the Entity Framework DataContext
//It's very Important that we do it before Builder.build() is called Using ScopedConnection
*/
builder.Services.AddScoped<IUserRepository, UserRepository>();


//Authentication Builder Service: => Tell it that you will be using JWT, and make sure to install your Nuget Package for JWT

//retrieving your TokenKey from Config File:
string? tokenKeyString = builder.Configuration.GetSection("AppSettings:TokenKey").Value;


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeyString != null ? tokenKeyString : "")),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


//Building Our Application with all the above Configurations
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors"); //Using our DevCors setUp for Development
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    //Using our ProdCors setUp for Production
    app.UseCors("ProdCors");

    //To Use only HTTPS when we are in Production Environment
    app.UseHttpsRedirection();
}

//NB: UseAuthentication MUST ALWAYS ALWAYS Come before UseAuthorization

//Will be responsible for Managing Authentication of Users: and will use the Token Refresh Logic:
app.UseAuthentication();



app.UseAuthorization();





//Accesses our Controller Map, and sets up our routes: Takes incoming requests and discovers the right controller with the correct endpoints and routes the request to it.
app.MapControllers();

app.Run();
