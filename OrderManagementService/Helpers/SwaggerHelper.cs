﻿using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace OrderManagementService.Helpers
{
    public class SwaggerHelper
    {
        public static void ConfigureSwaggerGen(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Order Managament Service",
                Description = "All reponses are in generic ApiResponse model."
            });

            options.AddSecurityDefinition("Bearer",
                     new OpenApiSecurityScheme
                     {
                         In = ParameterLocation.Header,
                         Description = "Please enter into field the word 'Bearer' following by space and JWT",
                         Name = "Authorization",
                         Type = SecuritySchemeType.ApiKey,
                         Scheme = "Bearer"
                     });

            OpenApiSecurityRequirement openApiSecurityRequirement = new OpenApiSecurityRequirement();

            openApiSecurityRequirement.Add(new OpenApiSecurityScheme()
            {
                Description = "Please enter Bearer Token",
                In = ParameterLocation.Header,
                Name = "Bearer",
                Type = SecuritySchemeType.ApiKey,
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            }, new List<string>());
            options.AddSecurityRequirement(openApiSecurityRequirement);
        }

        public static void ConfigureSwagger(SwaggerOptions swaggerOptions)
        {
            swaggerOptions.RouteTemplate = "swagger/" + "{documentName}/swagger.json";
        }

        public static void ConfigureSwaggerUI(SwaggerUIOptions swaggerUIOptions)
        {
            string swaggerJsonBasePath = string.IsNullOrWhiteSpace(swaggerUIOptions.RoutePrefix) ? "." : "..";
            swaggerUIOptions.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "");
            swaggerUIOptions.RoutePrefix = "swagger";
        }
    }
}
