using RiskAssessment.Entity.Enum;
using System;
using RiskAssessment.Entity.DTO;

namespace RiskAssessment.Api.Validator
{
    public static class RiskIssueValidator
    {
        public static bool IsValid(this RiskIssueDto obj)
        {
            obj.Code = obj.Code.Trim();
            if (string.IsNullOrEmpty(obj.Code))
                return false;
            obj.Name = obj.Name.Trim();
            if (string.IsNullOrEmpty(obj.Name))
                return false;

            if (obj.Code.Length > 15)
                return false;

            return true;
        }
    }
}
