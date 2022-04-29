using System;
using System.Text.Json.Serialization;
using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.JsonOption;

namespace RiskAssessment.Entity.DTO
{
    public class BaseDto<T> where T : BaseEntity, new()
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int ID { get; set; }
        [JsonPropertyName("code")]
        [JsonConverter(typeof(FormatString))]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        [JsonConverter(typeof(FormatString))]
        public string Name { get; set; }
        [JsonPropertyName("status")]
        [JsonConverter(typeof(StatusImport))]
        public bool Status { get; set; }
        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }
        [JsonPropertyName("description")]
        [JsonConverter(typeof(FormatString))]
        public string Description { get; set; }
        [JsonPropertyName("create_date")]
        public DateTime? CreateDate { get; set; }
        [JsonPropertyName("create_date_str")]
        public string CreateDateStr { get; set; }
        [JsonPropertyName("user_create")]
        public int? UserCreate { get; set; }
        [JsonPropertyName("name_create")]
        [JsonConverter(typeof(FormatString))]
        public string NameCreate { get; set; }
        [JsonPropertyName("last_modified")]
        public DateTime? LastModified { get; set; }
        [JsonPropertyName("last_modified_str")]
        public string LastModifiedStr { get; set; }
        [JsonPropertyName("modified_by")]
        public int? ModifiedBy { get; set; }
        [JsonPropertyName("name_modified")]
        [JsonConverter(typeof(FormatString))]
        public string NameModified { get; set; }
        [JsonPropertyName("domain_id")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int DomainId { get; set; }


        [JsonPropertyName("note")]
        [JsonConverter(typeof(FormatString))]
        public string Note { get; set; }
        [JsonPropertyName("valid")]
        public bool Valid { get; set; }
        public BaseDto()
        {

        }
        public BaseDto(T t)
        {
            ID = t.ID;
            Code = t.Code;
            Name = t.Code;
            Status = t.Status;
            Description = t.Description;
            UserCreate = t.UserCreate;
            CreateDate = t.CreateDate;
            ModifiedBy = t.ModifiedBy;
            LastModified = t.LastModified;
            DomainId = t.DomainId;
        }
        public T GetT()
        {
            var t = new T()
            {
                ID = this.ID,
                Code = this.Code,
                Name = this.Code,
                Status = this.Status,
                Description = this.Description,
                UserCreate = this.UserCreate,
                CreateDate = this.CreateDate ?? DateTime.Now,
                ModifiedBy = this.ModifiedBy,
                LastModified = this.LastModified,
                DomainId = this.DomainId
            };

            return t;
        }
        public T Map(T t)
        {
            t.Name = Name;
            t.Code = Code;
            t.Status = Status;
            t.Description = Description;
            t.DomainId = DomainId;
            return t;
        }
        public void UpdateStatus(int modifiedBy, bool status)
        {
            Status = status;
            LastModified = DateTime.Now;
            ModifiedBy = modifiedBy;
        }
    }
}
