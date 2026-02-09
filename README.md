# IT Partners README.MD file

## Summary: 

This is the new Resource Information for IT Partners, used to organize resources, publications, notes, FAQ items, people, and events.

## Production location: 

This consists of five applications:
* **ResourceInformationV2**: The Blazor server application to handle back-end functions. https://resource.wigg.illinois.edu
* **ResourceInformationV2.Function**: The Function Application for the API. https://resourceapi.wigg.illinois.edu
* **ResourceInformationV2.Data**: The data access for all the processes
* **ResourceInformationV2.Search**: The data access for searching (accessing AWS OpenSearch Service)
* **ResourceInformationV2.Template**: The templating tool for creating high-quality HTML pages for notes

## Development location: 

Currently, none. We do development on local machines, and will host temporary sites when end users need to see development work. 

## How to deploy to production/development: 

CI/CD 

## How to set up locally: 

Download and point to an empty AWS OpenSearch Service. The function code will install the necessary indicies, and the blazor app will install the database tables.  

You can get your IP address by using http://checkip.amazonaws.com/.

### Information about EF Core Tools:

Make sure the ResourceInformationV2 project is set up as the startup project before running the commands below:

``Add-Migration -Name {migration name} -Project ResourceInformationV2.Data``

``Update-Database -Project ResourceInformationV2.Data``

If you run into the issue "The certificate chain was issued by an authority that is not trusted.", then add **TrustServerCertificate=True** to the connection string.

## Code to delete the test items in the OpenSearch Service

``
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
``
