namespace ResourceInformationV2.Data.Cache {

    public class CacheThinObject(string netid) {
        private const int _minutesValid = 60;

        public string? BaseUrl { get; set; }
        public bool Expired => DateTime.Now > DateInvalid;
        public string? ItemId { get; set; }
        public string NetId { get; set; } = netid;
        public string? Source { get; set; }
        internal DateTime DateInvalid { get; set; } = DateTime.Now.AddMinutes(_minutesValid);

        public void Reset() => DateInvalid = DateTime.Now.AddMinutes(_minutesValid);
    }
}