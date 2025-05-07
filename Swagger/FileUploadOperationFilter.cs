using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using LibraryManagementSystem.Dto;

namespace LibraryManagementSystem.Swagger
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.MethodInfo.GetParameters().Any(p => p.ParameterType == typeof(BookUpdateDto)))
            {
                var schema = new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["Id"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                        ["BookName"] = new OpenApiSchema { Type = "string" },
                        ["Writer"] = new OpenApiSchema { Type = "string" },
                        ["Genre"] = new OpenApiSchema { Type = "string" },
                        ["ReleaseDate"] = new OpenApiSchema { Type = "string", Format = "date-time" },
                        ["BookImage"] = new OpenApiSchema { Type = "string", Format = "binary" },
                        ["Price"] = new OpenApiSchema { Type = "number", Format = "decimal" },
                        ["Language"] = new OpenApiSchema { Type = "string" },
                        ["Format"] = new OpenApiSchema { Type = "string" },
                        ["PublisherId"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                        ["Stock"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                        ["Description"] = new OpenApiSchema { Type = "string" },
                        ["IsOnSale"] = new OpenApiSchema { Type = "boolean" },
                        ["DiscountPercentage"] = new OpenApiSchema { Type = "number", Format = "decimal", Nullable = true },
                        ["DiscountStartDate"] = new OpenApiSchema { Type = "string", Format = "date-time", Nullable = true },
                        ["DiscountEndDate"] = new OpenApiSchema { Type = "string", Format = "date-time", Nullable = true },
                    }
                };

                operation.RequestBody = new OpenApiRequestBody
                {
                    Content =
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = schema
                        }
                    }
                };
            }
        }
    }
}
