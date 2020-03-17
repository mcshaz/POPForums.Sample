using EmailForumUpdates.RepoModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailForumUpdates.SummaryEngine
{
    public static class SummaryInfoBLL
    {
        public static async Task<SmtpSettings> GetSmtpSettings(string connectionString)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var repo = new SummaryInfoDAL(conn);
                await conn.OpenAsync();
                return await repo.GetSmtpSettings();
            }
        }
        public static async Task<IEnumerable<ActivitySummaryForUser>> GetSummariesForUsers(string connectionString, DateTime sinceUtc, int truncateAtChar = 128)
        {
            IEnumerable<RecentActivitySummaryModel> topicSummaries;
            IEnumerable<ForumViewPermission> forumViewPermissions;
            IEnumerable<UserMinimum> users;
            IEnumerable<UserRole> userRoles;
            IEnumerable<SubscribeTopic> subscriptions;
            using (var conn = new SqlConnection(connectionString))
            {
                var repo = new SummaryInfoDAL(conn);
                await conn.OpenAsync();
                
                topicSummaries = await repo.GetTopicSummaries(sinceUtc, truncateAtChar);
                userRoles = await repo.GetUserRoles();
                users = await repo.GetSubscribedApprovedUsers();
                subscriptions = await repo.GetSubscriptions();
                forumViewPermissions = await repo.GetForumViewPermissions();
            } // note close & dispose are functionally equivalent
            var rolesForUsr = userRoles.ToLookup(k => k.UserID, v => v.Role);
            var allowedForumIdsForRole = forumViewPermissions.ToLookup(k => k.Role, v => v.ForumID);
            var allowedAll = topicSummaries.Select(s => s.ForumID).Except(forumViewPermissions.Select(fvp => fvp.ForumID)).ToList();
            var subscriptionTopicsForUser = subscriptions.ToLookup(k => k.UserID, v => v.TopicID);
            var returnVar = new List<ActivitySummaryForUser>();
            foreach (var u in users)
            {
                var forumIdsForUsr = new HashSet<int>(rolesForUsr[u.UserID]
                    .SelectMany(ur => allowedForumIdsForRole[ur])
                    .Concat(allowedAll));
                /*
                foreach (var s in subscriptionTopicsForUser[u.UserID]) {
                    forumIdsForUsr.Remove(s);
                }
                */
                var summaries = topicSummaries.Where(s => forumIdsForUsr.Contains(s.ForumID)).ToList();
                if (summaries.Any())
                {
                    returnVar.Add(new ActivitySummaryForUser
                    {
                        User = u,
                        Summaries = summaries,
                        SubscribedTopics = new HashSet<int>(subscriptionTopicsForUser[u.UserID])
                    });
                }
            }
            return returnVar;
        }
    }
}
