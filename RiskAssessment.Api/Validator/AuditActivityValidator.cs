using RiskAssessment.Entity.Enum;
using System;
using RiskAssessment.Entity.DTO;

namespace RiskAssessment.Api.Validator
{
    public static class AuditActivityValidator
    {
        public static bool IsValid(this AuditActivityDto obj)
        {
            obj.Name = obj.Name.Trim();
            if (string.IsNullOrEmpty(obj.Name))
                return false;

            return true;
        }
    }
}
