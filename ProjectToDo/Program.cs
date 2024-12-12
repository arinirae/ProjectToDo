using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using ProjectToDo.Helpers;
using ProjectToDo.Models;
using ProjectToDo.Services;
using System;
using System.Text;
using Task = System.Threading.Tasks.Task;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql("Server=localhost;Database=ToDo;User=root;Password=;",
        new MySqlServerVersion(new Version(8, 0, 29))));

builder.Services.AddControllers();

builder.Services.AddScoped<UserValidation>();
builder.Services.AddScoped<IUserService, UserService>(); 
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>(); 
builder.Services.AddScoped<IProjectService, ProjectService>(); 
builder.Services.AddScoped<IRoleService, RoleService>(); 
builder.Services.AddScoped<IStatusService, StatusService>(); 
builder.Services.AddScoped<ITaskService, TaskService>(); 
builder.Services.AddMemoryCache();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        RequireExpirationTime = false,
        ClockSkew = TimeSpan.FromMinutes(5),
        ValidAudience = "InventoryServicePostmanClient",
        ValidIssuer = "InventoryAuthenticationServer",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("5dacc24ac51bf183b7f1d1c6664ee3faaa960889"))
    };
    options.EventsType = typeof(UserValidation);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
