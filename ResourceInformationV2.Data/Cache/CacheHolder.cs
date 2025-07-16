namespace ResourceInformationV2.Data.Cache {

    public class CacheHolder {
        private readonly Dictionary<string, CacheThinObject> _dictionary = [];

        public bool ClearCache(string netid) => _dictionary.Remove(netid);

        public string? GetCacheSource(string netid) => _dictionary.ContainsKey(netid) ? _dictionary[netid].Source : null;

        public CacheThinObject? GetItem(string netid) => _dictionary.ContainsKey(netid) ? _dictionary[netid] : null;

        public bool HasCachedItem(string netid) => _dictionary.ContainsKey(netid);

        public void SetCacheItem(string netid, string id) {
            if (_dictionary.ContainsKey(netid)) {
                _dictionary[netid].ItemId = id;
                _dictionary[netid].Reset();
            }
        }

        public void SetCacheSource(string netid, string source, string baseUrl) {
            if (_dictionary.ContainsKey(netid)) {
                _dictionary[netid].Source = source;
                _dictionary[netid].ItemId = "";
                _dictionary[netid].BaseUrl = baseUrl;
                _dictionary[netid].Reset();
            } else {
                _dictionary.Add(netid, new CacheThinObject(netid) { Source = source, BaseUrl = baseUrl });
            }
            ClearExpired();
        }

        private void ClearExpired() {
            foreach (var items in _dictionary.Where(i => i.Value != null && i.Value.Expired)) {
                _dictionary.Remove(items.Key);
            }
        }
    }
}