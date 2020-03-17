using Dapper;
using EmailForumUpdates.RepoModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace EmailForumUpdates
{
    public class SummaryInfoDAL
    {
        private readonly IDbConnection _conn;
        public SummaryInfoDAL(IDbConnection conn)
        {
            _conn = conn;
        }
        public async Task<IEnumerable<RecentActivitySummaryModel>> GetTopicSummaries(DateTime sinceUtc, int truncLen)
        {
            return (await _conn.QueryAsync<RecentActivitySummaryModel>(@"SELECT F.ForumID, F.UrlName as ForumUrlName, C.Title AS CatTitle, F.Title AS ForumTitle, T.Title as TopicTitle, T.UrlName as TopicUrlName, S.ReplyCount,
        P1.PostID as PostId1, P1.Name as PostName1, P1.PostTime as PostTime1, LEFT(P1.FullText, @truncLen) as Text1,
	    PL.PostID as PostIdL, PL.Name as PostNameL, PL.PostTime as PostTimeL, LEFT(PL.FullText, @truncLen) as TextL
FROM (SELECT MIN(PostID) as FirstPostId, MAX(PostID) as LastPostId, Count(PostId) - 1 as ReplyCount, TopicID
	    FROM pf_Post
	    GROUP BY TopicID
        HAVING MAX(PostTime) > @sinceUtc) as S
	INNER JOIN pf_Topic as T on S.TopicID = T.TopicID
	INNER JOIN pf_Forum as F on T.ForumID = F.ForumID
	INNER JOIN pf_Category as C on F.CategoryID = C.CategoryID
	INNER JOIN pf_Post as P1 on S.FirstPostId = P1.PostID
	INNER JOIN pf_Post as PL on s.LastPostId = PL.PostID", new { sinceUtc, truncLen })
                ).AsList();
        } 
        public async Task<IEnumerable<ForumViewPermission>> GetForumViewPermissions()
        {
            return (await _conn.QueryAsync<ForumViewPermission>("SELECT [ForumID],[Role] FROM [pf_ForumViewRestrictions]")
                ).AsList();
        }
        public async Task<IEnumerable<UserMinimum>> GetSubscribedApprovedUsers()
        {
            return (await _conn.QueryAsync<UserMinimum>(@"SELECT u.[UserID],[Name],[Email] FROM [pf_PopForumsUser] u
                                        INNER JOIN pf_Profile p ON u.UserID = p.UserID
                                        WHERE IsApproved = 1 AND p.IsSubscribed = 1")
                ).AsList();
        }
        public async Task<IEnumerable<UserRole>> GetUserRoles()
        {
            return (await _conn.QueryAsync<UserRole>("SELECT [UserID],[Role] FROM [pf_PopForumsUserRole]")
                ).AsList();
        }
        public async Task<IEnumerable<SubscribeTopic>> GetSubscriptions()
        {
            return (await _conn.QueryAsync<SubscribeTopic>("SELECT [UserID], [TopicID] FROM[pf_SubscribeTopic]")
                ).AsList();
        }
        public async Task<SmtpSettings> GetSmtpSettings() {
            return await _conn.QuerySingleAsync<SmtpSettings>(@"SELECT ForumTitle, SmtpPassword, CAST(SmtpPort AS int) as SmtpPort, SmtpServer, SmtpUser, CASE IsMailerEnabled WHEN 'True' THEN 1 ELSE 0 END as IsMailerEnabled, CASE UseSslSmtp WHEN 'True' THEN 1 ELSE 0 END as UseSslSmtp, CASE UseEsmtp WHEN 'True' THEN 1 ELSE 0 END as UseEsmtp,MailerAddress, CAST(MailerQuantity as Int) as MailerQuantity, Cast(MailSendingInverval  as Int) as MailerQuantity, MailSignature
FROM (SELECT [Setting], [Value] FROM [pf_Setting]) AS SourceTable
PIVOT (
    MIN([Value])--Must be an aggregate function
    FOR [Setting] IN
        (ForumTitle, SmtpPassword, SmtpPort, SmtpServer, SmtpUser, IsMailerEnabled, UseSslSmtp, UseEsmtp, MailerAddress, MailerQuantity, MailSendingInverval, MailSignature)
    ) As PivotOutput");
        }
    }
}
