using RiskAssessment.Entity.Enum;
using System;
using RiskAssessment.Entity.DTO;

namespace RiskAssessment.Api.Validator
{
    public static class RiskAssessmentScaleValidator
    {
        public static bool IsValid(this RiskAssessmentScaleDto obj)
        {
            if (obj.Point < 0)
                return false;

            if (obj.MinValue > 0 && (obj.MinCondition == null || obj.MinCondition.ID == 0))
                return false;
            if (obj.MaxValue > 0 && (obj.MaxCondition == null || obj.MaxCondition.ID == 0))
                return false;

            return true;
        }
    }
}
