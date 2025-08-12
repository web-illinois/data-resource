using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using OpenSearch.Client;
using ResourceInformationV2.Components;
using ResourceInformationV2.Data.Cache;
using ResourceInformationV2.Data.DataContext;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.Uploads;
using ResourceInformationV2.Search;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Helpers;
using ResourceInformationV2.Search.Setters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options => {
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddWebOptimizer(pipeline => {
    pipeline.AddJavaScriptBundle("/js/site.js", "/wwwroot/js/*.js").UseContentRoot();
    pipeline.AddCssBundle("/css/site.css", "/wwwroot/css/*.css").UseContentRoot();
});

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped(b => new UploadStorage(builder.Configuration["AzureStorage"], builder.Configuration["AzureAccountName"], builder.Configuration["AzureAccountKey"], builder.Configuration["AzureImageContainerName"]));

builder.Services.AddDbContextFactory<ResourceContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AppConnection")).EnableSensitiveDataLogging(true));
builder.Services.AddScoped<ResourceRepository>();
builder.Services.AddSingleton(b => OpenSearchFactory.CreateClient(builder.Configuration["SearchUrl"], builder.Configuration["SearchAccessKey"], builder.Configuration["SearchSecretAccessKey"], bool.Parse(builder.Configuration["SearchDebug"] ?? "false")));
builder.Services.AddSingleton(b => OpenSearchFactory.CreateLowLevelClient(builder.Configuration["SearchUrl"], builder.Configuration["SearchAccessKey"], builder.Configuration["SearchSecretAccessKey"], bool.Parse(builder.Configuration["SearchDebug"] ?? "false")));
builder.Services.AddScoped<BulkEditor>();
builder.Services.AddScoped<JsonHelper>();
builder.Services.AddSingleton<CacheHolder>();
builder.Services.AddScoped<SourceHelper>();
builder.Services.AddScoped<InstructionHelper>();
builder.Services.AddScoped<FilterHelper>();
builder.Services.AddScoped<FilterTranslator>();
builder.Services.AddScoped<SecurityHelper>();
builder.Services.AddScoped<LogHelper>();

builder.Services.AddScoped<ResourceGetter>();
builder.Services.AddScoped<ResourceSetter>();
builder.Services.AddScoped<PublicationGetter>();
builder.Services.AddScoped<PublicationSetter>();
builder.Services.AddScoped<FaqGetter>();
builder.Services.AddScoped<FaqSetter>();
builder.Services.AddScoped<NoteGetter>();
builder.Services.AddScoped<NoteSetter>();
builder.Services.AddScoped<PersonGetter>();
builder.Services.AddScoped<PersonSetter>();
builder.Services.AddScoped<EventGetter>();
builder.Services.AddScoped<EventSetter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
} else {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseWebOptimizer();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Lifetime.ApplicationStarted.Register(() => {
    var factory = app.Services.GetService<IServiceScopeFactory>() ?? throw new NullReferenceException("service scope factory is null");
    using var serviceScope = factory.CreateScope();
    // Ensure the database is created
    var context = serviceScope.ServiceProvider.GetRequiredService<ResourceContext>();
    _ = context.Database.EnsureCreated();
    // Ensure the search index is created
    var openSearchClient = serviceScope.ServiceProvider.GetRequiredService<OpenSearchClient>();
    Console.WriteLine(OpenSearchFactory.MapIndex(openSearchClient));
});

app.Run();