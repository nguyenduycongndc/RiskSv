using RiskAssessment.Entity.DTO;

namespace RiskAssessment.Api.Validator
{
    public static class BussinessActivityValidator
    {
        public static bool IsValid(this BussinessActivityDto obj)
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
