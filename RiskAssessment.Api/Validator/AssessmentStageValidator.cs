using RiskAssessment.Entity.DTO;

namespace RiskAssessment.Api.Validator
{
    public static class AssessmentStageValidator
    {
        public static bool IsValid(this AssessmentStageDto obj)
        {
            if (obj.Year == 0 || obj.Year < System.DateTime.Now.Year - 1 || obj.Stage <= 0)
                return false;
            if (obj.Stage == 3 && (!obj.StageValue.HasValue || obj.StageValue <= 0 || obj.StageValue > 4))
                return false;
                return true;
        }
    }
}
