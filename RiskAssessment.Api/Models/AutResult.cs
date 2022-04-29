using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskAssessment.Api.Models
{
    public class AutResult
    {
        public string Token { get; set; }
        public bool Result { get; set; }
        public List<string> Errors { get; set; }
    }
}
