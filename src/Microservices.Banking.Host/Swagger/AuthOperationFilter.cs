using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microservices.Banking.Host.Swagger;

public sealed class AuthOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var allowAnonymousAttributes = context!.MethodInfo!.DeclaringType!.GetCustomAttributes(true)
            .Union(context.MethodInfo.GetCustomAttributes(true))
            .OfType<AllowAnonymousAttribute>();

        if (allowAnonymousAttributes.Any())
        {
            return;
        }

        var securityRequirement = new OpenApiSecurityRequirement();
        securityRequirement.Add(new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "ApiKey"
            },
            Scheme = "basic",
            Name = HeaderNames.Authorization,
            In = ParameterLocation.Header,
        }, Array.Empty<string>());

        operation.Security = new[] { securityRequirement };

        operation.Responses.Add(
            ((int)HttpStatusCode.Unauthorized).ToString(),
            GetEmptyJsonResponse(nameof(HttpStatusCode.Unauthorized))
        );

        operation.Responses.Add(
            ((int)HttpStatusCode.Forbidden).ToString(),
            GetEmptyJsonResponse(nameof(HttpStatusCode.Forbidden))
        );
    }

    private static OpenApiResponse GetEmptyJsonResponse(string description)
    {
        return new OpenApiResponse
        {
            Content = new Dictionary<string, OpenApiMediaType>()
            {
                {
                    "application/json", new OpenApiMediaType()
                    {
                        Schema = new OpenApiSchema()
                        {
                            Default = new OpenApiString("{}")
                        }
                    }
                }
            },
            Description = description
        };
    }
}