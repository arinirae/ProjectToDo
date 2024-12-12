using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using ProjectToDo.Models;
using System.Security.Claims;
using System.Text.Json;

namespace ProjectToDo.Helpers
{
    public class UserValidation : JwtBearerEvents
    {
        private string UserId { get; set; }
        private string Username { get; set; }

        public override System.Threading.Tasks.Task Challenge(JwtBearerChallengeContext c)
        {
            try
            {
                c.Response.StatusCode = 401;
                c.Response.ContentType = "application/problem+json";

                var title = "Unauthorized";
                var detail = "You are not authorized";

                if (c.Error != null)
                {
                    if (c.Error == "invalid_token")
                    {
                        title = "Token Expired";
                    }
                    else
                    {
                        title = c.Error;
                    }
                }
                if (c.ErrorDescription != null)
                {
                    detail = c.ErrorDescription;
                }

                var problem = new ProblemDetails
                {
                    Status = 401,
                    Title = title,
                    Detail = detail,
                };

                var stream = c.Response.Body;
                return JsonSerializer.SerializeAsync(stream, problem);

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public override async System.Threading.Tasks.Task TokenValidated(TokenValidatedContext context)
        {
            try
            {
                ToDoDbContext dbcontext = context.HttpContext.RequestServices.GetRequiredService<ToDoDbContext>();

                ClaimsPrincipal userPrincipal = context.Principal;

                this.UserId = userPrincipal.Claims.First(c => c.Type == "Id").Value;
                this.Username = userPrincipal.Claims.First(c => c.Type == "Username").Value;

                var checkUser = dbcontext.Users.Where(x => x.Id == Convert.ToInt32(this.UserId) && x.Username == this.Username).FirstOrDefault();

                if (checkUser == null)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/problem+json";

                    var title = "Token Expired";
                    var detail = "Your token is expired";

                    var problem = new ProblemDetails
                    {
                        Status = 401,
                        Title = title,
                        Detail = detail,
                    };

                    var stream = context.Response.Body;

                    throw new Exception(title);
                    //return await JsonSerializer.SerializeAsync(stream, problem);
                }
                return;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
