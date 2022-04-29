using RiskAssessment.Entity.Enum;
using System;
using RiskAssessment.Entity.DTO;

namespace RiskAssessment.Api.Validator
{
    public static class RatingScaleValidator
    {
        public static bool IsValid(this RatingScaleDto obj)
        {
            if (obj.RiskLevel < 0)
                return false;

            if (string.IsNullOrEmpty(obj.MinFunction) || (obj.MinFunction != "gte" && obj.MinFunction != "gt"))
                return false;

            if (!obj.Min.HasValue || obj.Min < 0)
                return false;

            if (obj.Max.HasValue && obj.Max < 0)
                return false;

            return true;
        }
    }
}
