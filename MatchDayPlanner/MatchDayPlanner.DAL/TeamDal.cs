using MatchDayPlanner.Core;
using MatchDayPlanner.Core.Interfaces;

namespace MatchDayPlanner.DAL
{
    public class TeamDal : ITeamDal
    {
        private Database database = new Database();
        public Team GetTeamById(Guid TeamID)
        {
            return database.Query<Team>(string.Format("SELECT * FROM Teams WHERE teamID = '{0}'", TeamID)).FirstOrDefault();
        }
    }
}
