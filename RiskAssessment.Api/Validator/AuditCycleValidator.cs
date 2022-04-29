using RiskAssessment.Entity.Enum;
using System;
using RiskAssessment.Entity.DTO;

namespace RiskAssessment.Api.Validator
{
    public static class AuditCycleValidator
    {
        public static bool IsValid(this AuditCycleDto obj)
        {
            obj.Name = obj.Name.Trim();
            if (string.IsNullOrEmpty(obj.Name))
                return false;
            if (obj.RatingPoint < 1)
                return false;

            return true;
        }
    }
}
