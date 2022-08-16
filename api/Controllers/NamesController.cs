using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Web.Resource;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NamesController : Controller
    {
        [Route("GetNames")]
        public async Task<IActionResult> GetNames()
        {
            var foo = await ValidateJwtToken(GetAuthToken());
            return Ok(Data.NamesList);
        }

        [RequiredScope("Api.ReadWrite")]
        [Route("PostName")]
        public async Task<IActionResult> PostName([FromBody]NameModel nameModel)
        {
            Data.NamesList.Add(nameModel);
            return Ok(Data.NamesList);
        }

        public string GetAuthToken()
        {
            StringValues token;
            if (Request.Headers.TryGetValue("Authorization", out token))
            {
                return token.ToString().Split(" ").Last();
            }

            return string.Empty;
        }

        public async Task<string> ValidateJwtToken(string token)
        {
            string myTenant = "db03158f-1008-4215-bb30-5a1ea0e114b9";  
            var myAudience = "b27db65a-237d-4a59-ba27-9b8078b371c1";  
            var myIssuer = String.Format(CultureInfo.InvariantCulture, "https://login.microsoftonline.com/{0}/v2.0", myTenant);  
            var mySecret = "PKF8Q~17nyQKtG..aNg0DeaAqzFSYDB~tLfkDc9c";  
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));              
            var stsDiscoveryEndpoint = String.Format(CultureInfo.InvariantCulture, "https://login.microsoftonline.com/{0}/.well-known/openid-configuration", myTenant);  
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());  
            var config = await configManager.GetConfigurationAsync();  
  
            var tokenHandler = new JwtSecurityTokenHandler();  
  
            var validationParameters = new TokenValidationParameters  
            {  
                ValidAudience = myAudience,  
                ValidIssuer = myIssuer,  
                //ValidateIssuer = false,  
                IssuerSigningKeys = config.SigningKeys,  
                ValidateLifetime = false,  
                IssuerSigningKey = mySecurityKey  
            };  
  
            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var audience = jwtToken.Claims.First(x => x.Type == "aud").Value;

                // return audience from JWT token if validation successful
                return audience;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }
    }
}