using AiTools.DAL.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTools.DAL.Managers
{
    public class SignInManager : SignInManager<User>
    {
        public SignInManager(UserManager<User> userManager, IHttpContextAccessor contextAccessor, 
            IUserClaimsPrincipalFactory<User> claimsFactory, IOptions<IdentityOptions> optionsAccessor, 
            ILogger<SignInManager<User>> logger, IAuthenticationSchemeProvider schemes) 
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
        }
    }
}
