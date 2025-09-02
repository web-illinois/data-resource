using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ResourceInformationV2.Data.DataContext;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.Email;
using ResourceInformationV2.Search;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Helpers;
using ResourceInformationV2.Search.Setters;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureOpenApi()
    .ConfigureAppConfiguration((hostContext, config) => {
        if (hostContext.HostingEnvironment.IsDevelopment()) {
            _ = config.AddUserSecrets<Program>();
        }
    })
    .ConfigureServices((hostContext, services) => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        _ = services.AddApplicationInsightsTelemetryWorkerService();
        _ = services.Configure<JsonSerializerOptions>(options => {
            options.PropertyNamingPolicy = new JsonNamingPolicyLowerCase();
        });
        _ = services.ConfigureFunctionsApplicationInsights();
        _ = services.AddDbContextFactory<ResourceContext>(options => options.UseSqlServer(hostContext.Configuration["AppConnection"]).EnableSensitiveDataLogging(true));
        _ = services.AddScoped<ResourceRepository>();
        _ = services.AddScoped<BulkEditor>();
        _ = services.AddScoped<SourceHelper>();
        _ = services.AddScoped<FilterHelper>();
        _ = services.AddScoped<FilterTranslator>();
        _ = services.AddScoped<ApiHelper>();
        _ = services.AddScoped(b => new EmailClient(hostContext.Configuration["EmailApiKey"] ?? "", hostContext.Configuration["EmailFromEmail"] ?? "", hostContext.Configuration["EmailServerId"] ?? "", hostContext.Configuration["EmailUrl"] ?? ""));
        _ = services.AddScoped<SourceEmailHelper>();
        _ = services.AddScoped<LogHelper>();
        _ = services.AddSingleton(c => OpenSearchFactory.CreateLowLevelClient(hostContext.Configuration["SearchUrl"], hostContext.Configuration["AccessKey"], hostContext.Configuration["SecretKey"], hostContext.Configuration["Debug"] == "true"));
        _ = services.AddSingleton(c => OpenSearchFactory.CreateClient(hostContext.Configuration["SearchUrl"], hostContext.Configuration["AccessKey"], hostContext.Configuration["SecretKey"], true));
        _ = services.AddScoped<ResourceGetter>();
        _ = services.AddScoped<PublicationGetter>();
        _ = services.AddScoped<NoteGetter>();
        _ = services.AddScoped<FaqGetter>();
        _ = services.AddScoped<PersonGetter>();
        _ = services.AddScoped<PersonSetter>();
        _ = services.AddScoped<EventGetter>();
    })
    .Build();

host.Run();