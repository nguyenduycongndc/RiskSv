using RiskAssessment.Entity.Enum;
using System;
using RiskAssessment.Entity.DTO;

namespace RiskAssessment.Api.Validator
{
    public static class SystemCategoryValidator
    {
        public static bool IsValid(this SystemCategoryDto obj)
        {
            obj.ParentGroup = obj.ParentGroup.Trim();
            if (string.IsNullOrEmpty(obj.ParentGroup))
                return false;
            obj.Code = obj.Code.Trim();
            if (string.IsNullOrEmpty(obj.Code))
                return false;
            obj.Name = obj.Name.Trim();
            if (string.IsNullOrEmpty(obj.Name))
                return false;

            return true;
        }
    }
}
