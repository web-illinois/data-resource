using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json.Linq;

namespace ResourceInformationV2.Function.Helper {

    public class RequestHelper {
        private readonly List<string> _badValues = [];
        private Dictionary<string, string>? _formProperties;

        public string ErrorMessage => _badValues.Count != 0 ? $"Failed to get variables: '{string.Join(", ", _badValues)}'." : string.Empty;
        public RequestItem RequestItem { get; private set; } = new RequestItem();

        public IEnumerable<string> GetArray(HttpRequestData req, string name) {
            var s = GetRequest(req, name, false);
            return string.IsNullOrWhiteSpace(s) ? new List<string>() : s.Split("[-]").Select(s => s.Trim());
        }

        public bool GetBoolean(HttpRequestData req, string name, bool defaultValue = false) {
            var s = GetRequest(req, name, false);
            return bool.TryParse(s, out var i) ? i : defaultValue;
        }

        public string GetCodeFromHeader(HttpRequestData req) => req.Headers.FirstOrDefault(h => h.Key == "ilw-key").Value.FirstOrDefault() ?? string.Empty;

        public int GetInteger(HttpRequestData req, string name, int defaultValue = 0) {
            var s = GetRequest(req, name, false);
            return int.TryParse(s, out var i) ? i : defaultValue;
        }

        public string GetRequest(HttpRequestData req, string name, bool isRequired = true) {
            if (req == null) {
                return string.Empty;
            }
            var returnValue = System.Web.HttpUtility.ParseQueryString(req.Url.Query)[name]?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(returnValue)) {
                if (_formProperties == null)
                    try {
                        var bodyText = new StreamReader(req.Body).ReadToEnd();
                        RequestItem.Body = bodyText;
                        if (string.IsNullOrWhiteSpace(bodyText))
                            _formProperties = [];
                        else {
                            var jo = JObject.Parse(bodyText);
                            _formProperties = jo.Properties().ToDictionary(n => n.Name, v => v.Value.ToString());
                        }
                    } catch {
                        _formProperties = [];
                    }
                returnValue = _formProperties.TryGetValue(name, out var value) ? value : string.Empty;
            }
            if (string.IsNullOrWhiteSpace(returnValue) && isRequired) {
                _badValues.Add(name);
            }
            RequestItem.Parameters.Add(new Tuple<string, string>(name, returnValue));
            return returnValue.Trim();
        }

        public void Initialize(HttpRequestData req) => RequestItem ??= new RequestItem {
            Parameters = [],
            Path = req.Url.LocalPath ?? string.Empty,
            Host = req.Headers.FirstOrDefault(h => h.Key == "Host").Value.FirstOrDefault() ?? string.Empty,
            Referrer = req.Headers.FirstOrDefault(h => h.Key == "Referer").Value.FirstOrDefault() ?? string.Empty,
            StartDate = DateTime.Now
        };

        public void Validate() {
            if (!string.IsNullOrEmpty(ErrorMessage)) {
                throw new Exception(ErrorMessage);
            }
        }
    }
}