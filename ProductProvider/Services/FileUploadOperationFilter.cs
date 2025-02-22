//using Swashbuckle.AspNetCore.SwaggerGen;
//using Microsoft.OpenApi.Models;
//using Microsoft.AspNetCore.Http;
//using System.Linq;
//using System.Collections.Generic;

//namespace ProductProvider.Services
//{
//    public class FileUploadOperationFilter : IOperationFilter
//    {
//        public void Apply(OpenApiOperation operation, OperationFilterContext context)
//        {
//            var fileParam = context.ApiDescription.ActionDescriptor.Parameters
//                .FirstOrDefault(p => p.ParameterType == typeof(IFormFile));

//            if (fileParam != null)
//            {
//                // Ensure the file is part of the request body with multipart/form-data
//                operation.RequestBody = new OpenApiRequestBody
//                {
//                    Content = new Dictionary<string, OpenApiMediaType>
//                    {
//                        {
//                            "multipart/form-data", new OpenApiMediaType
//                            {
//                                Schema = new OpenApiSchema
//                                {
//                                    Type = "object",
//                                    Properties = new Dictionary<string, OpenApiSchema>
//                                    {
//                                        {
//                                            fileParam.Name, new OpenApiSchema
//                                            {
//                                                Type = "string",
//                                                Format = "binary"
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                };
//            }
//        }
//    }
//}
