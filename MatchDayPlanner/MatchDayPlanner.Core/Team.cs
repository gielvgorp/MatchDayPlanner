using MatchDayPlanner.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchDayPlanner.DAL
{
    public class Team
    {
        public ITeamDal TeamDal { get; }
        public Guid teamID { get; set; }
        public string teamName { get; set; }
        public Guid captainID { get; set; }

        public Team()
        {
            
        }

        public Team(ITeamDal c_dal, Guid c_teamID, string c_teamName, Guid c_captainID)
        {
            this.TeamDal = c_dal;
            this.teamID = c_teamID;
            this.teamName = c_teamName;
            this.captainID = c_captainID;
        }

        public Team GetTeam()
        {
            return TeamDal.GetTeamById(teamID);
        }
    }
}
