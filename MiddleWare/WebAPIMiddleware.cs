
using WebAPITemplate.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace WebAPITemplate.MiddleWare
{
    public class WebAPIMiddleware : IMiddleware
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<WebAPIMiddleware> _logger;
        public WebAPIMiddleware(ILogger<WebAPIMiddleware> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var controllerName = context.GetRouteData().Values["controller"];
            var actionName = context.GetRouteData().Values["action"];
            try
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (token != null)
                {
                    attachUserToContext(context, token);
                    //await _next(context);
                }
                await next.Invoke(context);
            }
            catch (Exception ex)
            {
                string ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message.ToString() : ex.Message.ToString();
                _logger.LogWarning("ExceptionMiddleware ||" + controllerName + "||" + actionName);
                _logger.LogError(ex, "ExceptionMiddleware ||" + controllerName + "||" + actionName + ", Exception : " + ExceptionMessage);
                await context.Response.WriteAsync("Error: " + ExceptionMessage);
            }
        }

        private void attachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                _ = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // attach user to context on successful jwt validation
                context.Items["User"] = userId;// userService.GetUserByUserId(userId, false);
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}
