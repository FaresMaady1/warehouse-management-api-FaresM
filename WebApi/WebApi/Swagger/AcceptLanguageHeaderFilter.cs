namespace WebApi.Swagger;

using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

public class AcceptLanguageHeaderFilter : IOperationFilter
{
    private static readonly string[] SupportedCultures = { "en", "fr", "ar" };

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<IOpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Accept-Language",
            In = ParameterLocation.Header,
            Required = false,
            Description = "Culture for localized responses.",
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Default = JsonValue.Create("en"),
                Enum = SupportedCultures.Select(c => (JsonNode)JsonValue.Create(c)).ToList()
            }
        });
    }
}