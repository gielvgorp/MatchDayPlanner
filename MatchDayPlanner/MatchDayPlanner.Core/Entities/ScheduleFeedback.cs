using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.Core.Entities
{
    public class ScheduleFeedback
    {
        public Guid feedbackID { get; set; }
        public Guid schemeID { get; set; }
        public Guid memberID { get; set; }
        public TeamMember member { get; set; }
        public string feedbackMessage { get; set; }
        public DateTime feedbackDateTime { get; set; }
    }
}
