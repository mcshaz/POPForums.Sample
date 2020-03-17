
using EmailForumUpdates.RepoModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailForumUpdates.RepoModels
{
    public class ActivitySummaryForUser
    {
        public UserMinimum User { get; set; }
        public IEnumerable<RecentActivitySummaryModel> Summaries { get; set; }
        public HashSet<int> SubscribedTopics { get;set; }
    }
}
