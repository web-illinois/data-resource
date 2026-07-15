# IT Partners Resource Information System

## Summary

This is the Resource Information system for IT Partners, used to organize resources, publications, notes, FAQ items, people, and events. Built with Blazor Server on .NET 10, it provides a comprehensive content management and search solution leveraging AWS OpenSearch Service.

## Schema update

In the rare case that the AWS OpenSearch Schema needs to be updated, go through the following process, change the `forceIndexCreation` value to true in the `OpenSearchFactory\MapIndex` class in the `ProgramInformationV2.Search` project for the schemas you want to rebuild. 

This will rebuild the index and reindex all data. After the schema update is complete, change the `forceIndexCreation` value back to false to prevent unnecessary reindexing in the future.


## Architecture Overview

This solution consists of five interconnected applications:

* **ResourceInformationV2**: Blazor Server application providing the main UI and back-end functions
  - Production URL: https://resource.wigg.illinois.edu
  - Framework: .NET 10 Blazor Server
  - Handles user interfaces, authentication, and business logic

* **ResourceInformationV2.Function**: Azure Function Application exposing the REST API
  - Production URL: https://resourceapi.wigg.illinois.edu
  - Provides API endpoints for external integrations
  
* **ResourceInformationV2.Data**: Data access layer
  - Entity Framework Core for SQL Server database operations
  - Contains models, migrations, and repository patterns
  
* **ResourceInformationV2.Search**: Search infrastructure layer
  - AWS OpenSearch Service integration
  - Handles indexing and search queries across all content types
  
* **ResourceInformationV2.Template**: Template engine
  - Generates high-quality HTML output for notes and content
  - Provides consistent formatting and styling

## Technical Stack

- **Framework**: .NET 10
- **Web Framework**: Blazor Server
- **Database**: SQL Server with Entity Framework Core
- **Search Engine**: AWS OpenSearch Service
- **Cloud Platform**: Azure (Functions, App Service)
- **CI/CD**: Automated deployment pipeline

## Local Development Setup

### Prerequisites

1. .NET 10 SDK installed
2. SQL Server (LocalDB, Express, or full instance)
3. AWS account with OpenSearch Service access
4. Visual Studio 2022 or later (or VS Code with C# extension)

### Initial Setup Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/web-illinois/data-resource
   cd data-resource
   ```

2. **Configure AWS OpenSearch Service**
   - Create an empty OpenSearch Service instance
   - Get your public IP: http://checkip.amazonaws.com/
   - Add your IP to the OpenSearch access policy
   - Update connection settings in `appsettings.Development.json`

3. **Configure Database Connection**
   - Update connection string in `appsettings.Development.json` in ResourceInformationV2 project
   - If encountering SSL certificate errors, add `TrustServerCertificate=True` to connection string

4. **Run Database Migrations**
   ```powershell
   # Set ResourceInformationV2 as startup project
   Add-Migration -Name InitialCreate -Project ResourceInformationV2.Data
   Update-Database -Project ResourceInformationV2.Data
   ```

5. **Start the Application**
   - Set ResourceInformationV2 as the startup project
   - Press F5 or run `dotnet run` in the ResourceInformationV2 directory
   - On first run, the app will automatically create OpenSearch indices

### Configuration Files

Key configuration files for local development:
- `appsettings.Development.json` - Local development settings
- `local.settings.json` - Azure Functions local configuration (ResourceInformationV2.Function)

## Debugging Guide

### Common Issues and Solutions

#### 1. Database Connection Issues

**Problem**: "The certificate chain was issued by an authority that is not trusted"
```
Solution: Add TrustServerCertificate=True to your connection string
Example: "Server=localhost;Database=ResourceInfo;Trusted_Connection=True;TrustServerCertificate=True"
```

**Problem**: Migration errors or database not found
```
Solution:
1. Ensure SQL Server is running
2. Verify connection string in appsettings.Development.json
3. Set ResourceInformationV2 as startup project
4. Run: Update-Database -Project ResourceInformationV2.Data
```

#### 2. OpenSearch Connection Issues

**Problem**: Cannot connect to OpenSearch Service
```
Debugging Steps:
1. Verify your IP is whitelisted: http://checkip.amazonaws.com/
2. Check OpenSearch access policies in AWS console
3. Verify OpenSearch endpoint URL in configuration
4. Test connectivity: curl -X GET https://your-opensearch-endpoint/_cluster/health
5. Check CloudWatch logs in AWS for OpenSearch errors
```

**Problem**: Index not found errors
```
Solution: Set forceIndexCreation = true in OpenSearchFactory\MapIndex
This will rebuild all indices on next application start
Remember to set back to false after indices are created
```

#### 3. Blazor Server Debugging

**Problem**: SignalR connection failures
```
Debugging Steps:
1. Check browser console for WebSocket errors
2. Verify firewall/proxy allows WebSocket connections
3. Check Application Insights or logs for SignalR errors
4. Increase SignalR timeout in Startup/Program.cs if needed
```

**Problem**: Component not updating/refreshing
```
Solutions:
1. Ensure StateHasChanged() is called after async operations
2. Check for proper use of @key directives on lists
3. Verify component parameters are properly decorated with [Parameter]
4. Use InvokeAsync() when updating UI from background threads
```

#### 4. Azure Functions Local Debugging

**Problem**: Functions not starting locally
```
Solutions:
1. Verify Azure Functions Core Tools are installed
2. Check local.settings.json exists and is configured
3. Ensure correct startup project (ResourceInformationV2.Function)
4. Check port conflicts (default: 7071)
```

### Debug Logging

Enable detailed logging by adding to `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.EntityFrameworkCore": "Information",
      "ResourceInformationV2": "Debug"
    }
  }
}
```

### Breakpoint Tips

- **Blazor Component Lifecycle**: Set breakpoints in `OnInitializedAsync()`, `OnParametersSet()`, `OnAfterRender()`
- **EF Core Queries**: Enable query logging to see generated SQL
- **Search Operations**: Debug OpenSearchFactory methods to inspect search queries
- **API Endpoints**: Debug Function triggers to inspect HTTP requests/responses

## Entity Framework Core Operations

### Creating Migrations

```powershell
# Ensure ResourceInformationV2 is set as startup project
Add-Migration -Name DescriptiveMigrationName -Project ResourceInformationV2.Data
```

### Applying Migrations

```powershell
# Apply to local database
Update-Database -Project ResourceInformationV2.Data

# Apply specific migration
Update-Database -Migration MigrationName -Project ResourceInformationV2.Data

# Rollback to previous migration
Update-Database -Migration PreviousMigrationName -Project ResourceInformationV2.Data
```

### Useful EF Core Commands

```powershell
# List all migrations
Get-Migration -Project ResourceInformationV2.Data

# Generate SQL script for migration
Script-Migration -Project ResourceInformationV2.Data

# Remove last migration (if not applied)
Remove-Migration -Project ResourceInformationV2.Data
```

## OpenSearch Service Management

### Schema Updates

In rare cases when OpenSearch schema needs updating:

1. Navigate to `ResourceInformationV2.Search` project
2. Open `OpenSearchFactory\MapIndex` class
3. Set `forceIndexCreation = true` for schemas to rebuild
4. Run the application - indices will be recreated and reindexed
5. **Important**: Set `forceIndexCreation = false` after completion

### Index Names

- `rr2_resources` - Resources index
- `rr2_publications` - Publications index
- `rr2_events` - Events index
- `rr2_faqs` - FAQ items index
- `rr2_notes` - Notes index
- `rr2_people` - People index

### Deleting Test Data

Use these OpenSearch queries to remove test data:

```json
POST /rr2_resources/_delete_by_query
{ "query": { "match": { "source": "test" } } }

POST /rr2_publications/_delete_by_query
{ "query": { "match": { "source": "test" } } }

POST /rr2_events/_delete_by_query
{ "query": { "match": { "source": "test" } } }

POST /rr2_faqs/_delete_by_query
{ "query": { "match": { "source": "test" } } }

POST /rr2_notes/_delete_by_query
{ "query": { "match": { "source": "test" } } }

POST /rr2_people/_delete_by_query
{ "query": { "match": { "source": "test" } } }
```

### Monitoring OpenSearch

```bash
# Check cluster health
GET /_cluster/health

# View all indices
GET /_cat/indices?v

# Check index stats
GET /rr2_resources/_stats

# View index mapping
GET /rr2_resources/_mapping

# Sample search query for debugging
GET /rr2_resources/_search
{
  "query": {
    "match_all": {}
  }
}
```

## Deployment

### CI/CD Pipeline

Automated deployment is configured via CI/CD. Pushes to specific branches trigger automated builds and deployments:

- **Main branch**: Deploys to production
- **Develop branch**: Can deploy to staging/development environments

### Manual Deployment Checklist

If manual deployment is required:

1. **Build Solution**
   ```bash
   dotnet build --configuration Release
   ```

2. **Run Tests** (if available)
   ```bash
   dotnet test
   ```

3. **Publish Blazor App**
   ```bash
   dotnet publish ResourceInformationV2/ResourceInformationV2.csproj -c Release -o ./publish/blazor
   ```

4. **Publish Function App**
   ```bash
   dotnet publish ResourceInformationV2.Function/ResourceInformationV2.Function.csproj -c Release -o ./publish/functions
   ```

5. **Deploy to Azure**
   - Use Azure Portal, Azure CLI, or Visual Studio Publish
   - Ensure connection strings and app settings are configured
   - Run database migrations if schema changed

## Performance Considerations

### Blazor Server Performance

- **Connection Limits**: Blazor Server maintains a SignalR connection per user
- **Memory Usage**: Monitor memory with many concurrent users
- **Circuit Timeout**: Default is 3 minutes; adjust in configuration if needed
- **Prerendering**: Used for faster initial load; ensure state is handled properly

### Database Performance

- **Connection Pooling**: Enabled by default in EF Core
- **Query Optimization**: Use `.AsNoTracking()` for read-only queries
- **Indexing**: Ensure proper indices on frequently queried columns
- **Bulk Operations**: Use `AddRange`/`UpdateRange` for batch operations

### OpenSearch Performance

- **Batch Indexing**: Index documents in batches rather than individually
- **Refresh Interval**: Adjust for balance between freshness and performance
- **Replica Shards**: Configure for high availability and read performance
- **Query Caching**: OpenSearch caches frequent queries automatically

## Troubleshooting Tools

### Visual Studio Debugging

- **Hot Reload**: Modify Razor/C# code without restarting (Ctrl+Alt+F5)
- **Edit and Continue**: Modify code during debugging session
- **Diagnostic Tools**: Monitor CPU, Memory, and Events during debugging
- **IntelliTrace**: Record and replay debugging sessions (Enterprise edition)

### Logging and Monitoring

- **Application Insights**: Monitor production application performance
- **Azure App Service Logs**: View application logs in Azure Portal
- **OpenSearch CloudWatch**: Monitor search performance in AWS
- **SQL Server Profiler**: Trace database queries and performance

### Useful Links

- Get your IP for OpenSearch whitelist: http://checkip.amazonaws.com/
- Blazor documentation: https://learn.microsoft.com/aspnet/core/blazor
- EF Core documentation: https://learn.microsoft.com/ef/core
- OpenSearch documentation: https://opensearch.org/docs/latest/

## Contributing

When contributing to this project:

1. Create a feature branch from `develop`
2. Follow existing code style and patterns
3. Update migrations if database schema changes
4. Test locally before pushing
5. Create pull request with clear description
6. Ensure CI/CD pipeline passes

## Technical Notes

### Project Dependencies

- Projects are organized in a layered architecture
- **ResourceInformationV2** references Data, Search, and Template projects
- **ResourceInformationV2.Function** references Data and Search projects
- Avoid circular dependencies between layers

### Authentication & Authorization

- Authentication handled by Blazor Server middleware
- Authorization policies defined in Startup/Program configuration
- User context available throughout application via AuthenticationStateProvider

### Blazor Component Best Practices

- Use `@code` blocks for component logic
- Implement `IDisposable` for cleanup (event handlers, timers)
- Use `EventCallback<T>` for component events
- Leverage cascading parameters for shared state
- Consider component lifecycle for data loading

### Search Implementation Notes

- Full-text search powered by OpenSearch
- Search queries support fuzzy matching, filters, and faceting
- Results are paginated and cached for performance
- Indexing happens asynchronously after data changes

### Known Limitations

- Blazor Server requires constant connection; disconnections require page refresh
- Large file uploads may timeout; consider implementing chunked uploads
- OpenSearch indices require manual rebuild if schema changes significantly
- High concurrent user load may require scaling Azure App Service plan
