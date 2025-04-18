using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AuthenticationApi.Configurations.Swagger
{
    public class SwaggerAuthorizationHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var auth = operation.Security.FirstOrDefault();
            if (auth != null)
            {
                var param = operation.Parameters?.FirstOrDefault(p => p.Name == "Authorization");
                if (param != null)
                {
                    param.Description = "Sadece JWT token'ı girin. `Bearer` yazmanıza gerek yoktur.";
                    param.Example = new Microsoft.OpenApi.Any.OpenApiString("eyJhbGciOiJIUzI1NiIsInR5cCI6...");
                }
            }
        }
    }
}