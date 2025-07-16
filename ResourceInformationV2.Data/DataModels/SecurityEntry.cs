using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceInformationV2.Data.DataModels {

    public class SecurityEntry : BaseDataItem {

        public SecurityEntry() {
        }

        public SecurityEntry(string netId, int sourceId, bool requestedOnly = false) {
            Email = TransformName(netId);
            IsActive = !requestedOnly;
            IsOwner = false;
            IsPublic = false;
            IsRequested = requestedOnly;
            SourceId = sourceId;
        }

        public string DepartmentTag { get; set; } = "";
        public string Email { get; set; } = "";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public bool IsFullAdmin { get; set; }
        public bool IsOwner { get; set; }
        public bool IsPublic { get; set; }
        public bool IsRequested { get; set; }
        public virtual Source? Source { get; set; }
        public int? SourceId { get; set; }

        public static string TransformName(string netid) => (netid.EndsWith("@illinois.edu") ? netid : netid + "@illinois.edu").ToLowerInvariant();
    }
}