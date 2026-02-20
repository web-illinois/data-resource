using Amazon;
using Amazon.Runtime;
using OpenSearch.Client;
using OpenSearch.Net;
using OpenSearch.Net.Auth.AwsSigV4;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search {

    public static class OpenSearchFactory {

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
            var indexEvents = openSearchClient.Indices.Create(UrlTypes.Events.ConvertToUrlString(), c => c.Map(m => m.AutoMap<Event>()));
            returnValue += $"Events {(indexEvents.IsValid ? "created" : "failed")} - {indexEvents.DebugInformation}; ";
            var indexFaqs = openSearchClient.Indices.Create(UrlTypes.Faqs.ConvertToUrlString(), c => c.Map(m => m.AutoMap<FaqItem>()));
            returnValue += $"FAQs {(indexFaqs.IsValid ? "created" : "failed")} - {indexFaqs.DebugInformation}; ";
            var indexNotes = openSearchClient.Indices.Create(UrlTypes.Notes.ConvertToUrlString(), c => c.Map(m => m.AutoMap<NoteItem>()));
            returnValue += $"Notes {(indexNotes.IsValid ? "created" : "failed")} - {indexNotes.DebugInformation}; ";
            var indexPeople = openSearchClient.Indices.Create(UrlTypes.People.ConvertToUrlString(), c => c.Map(m => m.AutoMap<Person>()));
            returnValue += $"FAQs {(indexPeople.IsValid ? "created" : "failed")} - {indexPeople.DebugInformation}; ";

            // Need to rebuild publication index. DELETE AFTER DEPLOY
            openSearchClient.Indices.Delete(UrlTypes.Publications.ConvertToUrlString());

            var indexPublications = openSearchClient.Indices.Create(UrlTypes.Publications.ConvertToUrlString(), c => c.Map(m => m.AutoMap<Publication>()));
            returnValue += $"Publications {(indexPublications.IsValid ? "created" : "failed")} - {indexPublications.DebugInformation}; ";
            var indexResources = openSearchClient.Indices.Create(UrlTypes.Resources.ConvertToUrlString(), c => c.Map(m => m.AutoMap<Resource>()));
            returnValue += $"Resources {(indexResources.IsValid ? "created" : "failed")} - {indexResources.DebugInformation}; ";
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
    }
}