using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ProjectToDo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ProjectToDo.Services.Dto;
using System.Globalization;

namespace ProjectToDo.Services
{
    public interface IAuthenticationService
    {
        Task<LoginDto> Login(string username, string password);
    }
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ToDoDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthenticationService(ToDoDbContext context, IConfiguration configuration) 
        { 
            _context = context;
            _configuration = configuration;
        }
        public async Task<LoginDto> Login(string username, string password)
        {
            var obj = new LoginDto();
            if (string.IsNullOrEmpty(username))
                throw new Exception("Username wajib diisi");

            var passwordHasher = new PasswordHasher<User>();
            var getData = (from a in _context.Users
                           where a.Username == username 
                           select a);

            if (!getData.Any()) throw new Exception("User tidak ditemukan");
            var dataUser = await getData.FirstOrDefaultAsync();
            if (passwordHasher.VerifyHashedPassword(dataUser, dataUser.Password, password) == PasswordVerificationResult.Failed) throw new Exception("Password Salah");

            var token = SetSessionToken(dataUser);

            obj.Token = token;
            obj.UserId = dataUser.Id;
            obj.Username = dataUser.Username;

            return obj;
        }

        private string SetSessionToken(User user)
        {
            DateTime expiredDate = DateTime.UtcNow.AddYears(1);
            //create claims details based on the user information
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, "InventoryServiceAccessToken"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
                new Claim("Id", user.Id.ToString()),
                new Claim("Username", user.Username),
                new Claim("RoleId", user.RoleId.ToString()),
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("5dacc24ac51bf183b7f1d1c6664ee3faaa960889"));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken("InventoryAuthenticationServer",
                "InventoryServicePostmanClient",
                claims,
                expires: expiredDate,
                signingCredentials: signIn
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
