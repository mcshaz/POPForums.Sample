using System;
using System.Collections.Generic;
using System.Text;

namespace RazorToEmail.Views.Emails.ActivityUpdate
{
    public class ActivityUpdateEmailVM
    {
        // public int UserId { get; set; }
        public IEnumerable<ActivityUpdateEmailTopicVM> Topics { get; set; }
        public string BaseUrl { get; set; }
    }
    // PagedTopicContainer
    public class ActivityUpdateEmailTopicVM
    {
        public string CatTitle { get; set; }
        public string ForumTitle { get; set; }
        public string ForumUrlName { get; set; }
        public string TopicTitle { get; set; }
        public string TopicUrlName { get; set; }
        public bool IsTopicUserSubscribed { get; set; }
        public string StartedByName { get; set; }
        public string StartedTimeAgo { get; set; }
        public int ReplyCount { get; set; } 
        public string StartingText { get; set; }
        public string LastText { get; set; }
        public string LastPostName { get; set; }
        public string LastPostTimeAgo { get; set; }
        public DateTime LastPostUtc { get; set; }
    }
}
