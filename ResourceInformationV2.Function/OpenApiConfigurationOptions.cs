using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace ResourceInformationV2.Function {
    internal class OpenApiConfigurationOptions : IOpenApiConfigurationOptions {
        public List<IDocumentFilter> DocumentFilters { get; set; } = new List<IDocumentFilter>();

        public bool ForceHttp { get; set; } = false;

        public bool ForceHttps { get; set; } = false;

        public bool IncludeRequestingHostName { get; set; } = true;

        public OpenApiInfo Info { get; set; } = new OpenApiInfo() {
            Version = "1.0.0",
            Title = "WIGG Resource Application API"
            pull information from the WIGG Resource Application. Use this with the WIGG Resource Application at
            Contact = new OpenApiContact() {
                Name = "Bryan Jonker",
                Email = "jonker@illinios.edu",
                Url = new Uri("https://github.com/web-illinois/data-resource"),
            }
        };

        public OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;
        public List<OpenApiServer> Servers { get; set; } = new List<OpenApiServer>();
    }
}
