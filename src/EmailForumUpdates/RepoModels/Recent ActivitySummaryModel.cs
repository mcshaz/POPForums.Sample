using System;
using System.Collections.Generic;
using System.Text;

namespace EmailForumUpdates.RepoModels
{
    public class RecentActivitySummaryModel
    {
        public string CatTitle { get; set; }
        public int ForumID { get; set; }
        public string ForumUrlName { get; set; }
        public string ForumTitle { get; set; }
        public string TopicTitle { get; set; }
        public string TopicUrlName { get; set; }
        public int TopicID { get; set; }
        public int ReplyCount { get; set; }
        public int PostId1 { get; set; }
        public string PostName1 { get; set; }
        public DateTime PostTime1 { get; set; }
        public string Text1 { get; set; }
	    public int PostIdL { get; set; }
        public string PostNameL { get; set; }
        public DateTime PostTimeL { get; set; }
        public string TextL { get; set; }
    }
}
