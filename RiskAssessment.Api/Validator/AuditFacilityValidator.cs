using RiskAssessment.Entity.DTO;

namespace RiskAssessment.Api.Validator
{
    public static class AuditFacilityValidate
    {
        public static bool IsValid(this AuditFacilityDto obj)
        {
            obj.Name = obj.Name.Trim();
            if (string.IsNullOrEmpty(obj.Name))
                return false;

            obj.Code = obj.Code.Trim();
            if (string.IsNullOrEmpty(obj.Code))
                return false;

            return true;
        }
    }
}
