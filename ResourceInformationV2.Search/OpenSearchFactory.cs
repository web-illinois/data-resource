using Amazon;
using Amazon.Runtime;
using OpenSearch.Client;
using OpenSearch.Net;
using OpenSearch.Net.Auth.AwsSigV4;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search {

    public static class OpenSearchFactory {
        private const string TempIndex = "pcr2_tempindex";

        public static OpenSearchClient CreateClient(string? baseUrl, string? accessKey, string? secretKey, bool debug) {
            var client = new OpenSearchClient(GenerateConnection(baseUrl, accessKey, secretKey, debug));
            _ = client.ConnectionSettings.DefaultIndices.Add(typeof(FaqItem), UrlTypes.Faqs.ConvertToUrlString());
            _ = client.ConnectionSettings.DefaultIndices.Add(typeof(NoteItem), UrlTypes.Notes.ConvertToUrlString());
            _ = client.ConnectionSettings.DefaultIndices.Add(typeof(Person), UrlTypes.People.ConvertToUrlString());
            _ = client.ConnectionSettings.DefaultIndices.Add(typeof(Publication), UrlTypes.Publications.ConvertToUrlString());
            _ = client.ConnectionSettings.DefaultIndices.Add(typeof(Resource), UrlTypes.Resources.ConvertToUrlString());
            _ = client.ConnectionSettings.DefaultIndices.Add(typeof(Event), UrlTypes.Events.ConvertToUrlString());
            return client;
        }

        public static OpenSearchLowLevelClient CreateLowLevelClient(string? baseUrl, string? accessKey, string? secretKey, bool debug) => new(GenerateConnection(baseUrl, accessKey, secretKey, debug));

        public static string MapIndex(OpenSearchClient openSearchClient) {
            var returnValue = "Mapping: ";
            // NOTE: change the 'forceIndexCreation' to true if you are changing the index -- this will greatly increase the load time.
            returnValue += ReloadIndex(openSearchClient, UrlTypes.Events, false);
            returnValue += ReloadIndex(openSearchClient, UrlTypes.Faqs, false);
            returnValue += ReloadIndex(openSearchClient, UrlTypes.Notes, false);
            returnValue += ReloadIndex(openSearchClient, UrlTypes.People, false);
            returnValue += ReloadIndex(openSearchClient, UrlTypes.Publications, false);
            returnValue += ReloadIndex(openSearchClient, UrlTypes.Resources, false);
            return returnValue;
        }

        private static ConnectionSettings GenerateConnection(string? baseUrl, string? accessKey, string? secretKey, bool debug) {
            var nodeAddress = new Uri(baseUrl ?? "");
            var connection = string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrWhiteSpace(secretKey) ? null : new AwsSigV4HttpConnection(new BasicAWSCredentials(accessKey, secretKey), RegionEndpoint.USEast2);
            var config = new ConnectionSettings(nodeAddress, connection);
            if (debug) {
                _ = config.DisableDirectStreaming(true);
            }
            return config;
        }

        private static string ReloadIndex(OpenSearchClient openSearchClient, UrlTypes url, bool forceIndexCreation) {
            if (!forceIndexCreation) {
                return CreateIndex(openSearchClient, url, false);
            }
            var indexName = url.ConvertToUrlString();
            var returnValue = $"Reloading {indexName}: ";
            returnValue += CreateIndex(openSearchClient, url, true);
            var reindexResponse = openSearchClient.ReindexOnServer(r => r
                .Source(s => s.Index(indexName))
                .Destination(d => d.Index(TempIndex))
                .WaitForCompletion(true)
            );
            if (!reindexResponse.IsValid) {
                throw new Exception(reindexResponse.DebugInformation);
            }
            var deleteResponse = openSearchClient.Indices.Delete(indexName);
            returnValue += $"Delete {(deleteResponse.IsValid ? "succeeded" : "failed")} - {deleteResponse.DebugInformation}; ";
            returnValue += CreateIndex(openSearchClient, url, false);
            var movebackResponse = openSearchClient.ReindexOnServer(r => r
                .Source(s => s.Index(TempIndex))
                .Destination(d => d.Index(indexName))
                .WaitForCompletion(true)
            );
            if (!movebackResponse.IsValid) {
                throw new Exception(movebackResponse.DebugInformation);
            }
            var deleteTempResponse = openSearchClient.Indices.Delete(TempIndex);
            returnValue += $"Delete Temp {(deleteTempResponse.IsValid ? "succeeded" : "failed")} - {deleteTempResponse.DebugInformation}; ";
            return returnValue;
        }

        private static string CreateIndex(OpenSearchClient openSearchClient, UrlTypes url, bool temp) {
            var indexName = temp ? TempIndex : UrlTypes.Events.ConvertToUrlString();
            return url switch {
                UrlTypes.Events => openSearchClient.Indices.Create(indexName, c => c.Map(m => m.AutoMap<Event>())).ConvertResponse(url),
                UrlTypes.Faqs => openSearchClient.Indices.Create(indexName, c => c.Map(m => m.AutoMap<FaqItem>())).ConvertResponse(url),
                UrlTypes.Notes => openSearchClient.Indices.Create(indexName, c => c.Map(m => m.AutoMap<NoteItem>())).ConvertResponse(url),
                UrlTypes.People => openSearchClient.Indices.Create(indexName, c => c.Map(m => m.AutoMap<Person>())).ConvertResponse(url),
                UrlTypes.Publications => openSearchClient.Indices.Create(indexName, c => c.Map(m => m.AutoMap<Publication>())).ConvertResponse(url),
                UrlTypes.Resources => openSearchClient.Indices.Create(indexName, c => c.Map(m => m.AutoMap<Resource>())).ConvertResponse(url),
                _ => string.Empty,
            };
        }
        private static string ConvertResponse(this CreateIndexResponse response, UrlTypes url) => $"Create {url} {(response.IsValid ? "succeeded" : "failed")} - {response.DebugInformation}; ";
    }
}