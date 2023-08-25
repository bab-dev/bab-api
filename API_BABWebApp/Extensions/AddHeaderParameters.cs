﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace API_BABWebApp.Extensions
{
    public class AddHeaderParameters : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                // If [AllowAnonymous] is not applied or [Authorize] or Custom Authorization filter is applied on either the endpoint or the controller
                if (!context.ApiDescription.CustomAttributes().Any((a) => a is AllowAnonymousAttribute)
                    && (context.ApiDescription.CustomAttributes().Any((a) => a is AuthorizeAttribute)
                        || descriptor.ControllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>() != null))
                {
                    if (operation.Security == null)
                        operation.Security = new List<OpenApiSecurityRequirement>();

                    operation.Security.Add(new OpenApiSecurityRequirement{
                        {
                            new OpenApiSecurityScheme
                            {
                                Name = "Authorization",
                                In = ParameterLocation.Header,
                                BearerFormat = "Bearer token",

                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                }
            }
        }
    }
}
