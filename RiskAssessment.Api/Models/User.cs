using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Text.Json;

namespace RiskAssessment.Api.Models
{
    public class User
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        [JsonPropertyName("user_name")]
        public string UserName { get; set; }

        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }
        [JsonPropertyName("avartar")]
        public string Avartar { get; set; }

        [JsonPropertyName("last_online_at")]
        public DateTime? LastOnline { get; set; }

        [JsonPropertyName("roleId")]
        public int? RoleId { get; set; }

        [JsonPropertyName("domain_id")]
        public int DomainId { get; set; }

    }
}
