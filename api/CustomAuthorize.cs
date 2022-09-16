using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace api
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomAuthorize : Attribute, IAuthorizationFilter
    {
        public CustomAuthorize()
        {
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context != null)  
            {  
                StringValues authTokens;  
                context.HttpContext.Request.Headers.TryGetValue("authToken", out authTokens);  

                var _token = authTokens.FirstOrDefault(); 
                if (_token != null)  
                {    
                    string authToken = _token;  
                    if (authToken != null)  
                    {  
                        if (IsValidToken(authToken))  
                        {  
                            context.HttpContext.Response.Headers.Add("authToken", authToken);  
                            context.HttpContext.Response.Headers.Add("AuthStatus", "Authorized");  
                            context.HttpContext.Response.Headers.Add("storeAccessiblity", "Authorized");  
    
                            return;  
                        }  
                        else  
                        {  
                            context.HttpContext.Response.Headers.Add("authToken", authToken);  
                            context.HttpContext.Response.Headers.Add("AuthStatus", "NotAuthorized");              

                            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;  
                            //context.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Not Authorized";  
                            context.Result = new JsonResult("NotAuthorized")  
                            {  
                                Value = new  
                                {  
                                    Status = "Error",  
                                    Message = "Invalid Token"  
                                },  
                            };  
                        }  

                    }  

                }  
                else  
                {  
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;  
                    //context.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "Please Provide authToken";  
                    context.Result = new JsonResult("Please Provide authToken")  
                    {  
                        Value = new  
                        {  
                            Status = "Error",  
                            Message = "Please Provide authToken"  
                        },  
                    };  
                }  
            }
        }

        public bool IsValidToken(string authToken)  
        {  
            //validate Token here  
            return true;  
        } 
    }
}