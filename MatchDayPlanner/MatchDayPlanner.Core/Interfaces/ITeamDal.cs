using MatchDayPlanner.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.Core.Interfaces
{
    public interface ITeamDal
    {
        public Team GetTeamById(Guid TeamID);
    }
}
