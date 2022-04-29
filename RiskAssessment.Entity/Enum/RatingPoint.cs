using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace RiskAssessment.Entity.Enum
{
    public enum RatingPoint
    {
        [EnumMember(Value = "0")]
        Low,
        [EnumMember(Value = "1")]
        Medium,
        [EnumMember(Value = "2")]
        High
    }
}
