using EmailForumUpdates.Extensions;
using EmailForumUpdates.RepoModels;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using RazorEmails.Services;
using RazorToEmail.Views.Emails.ActivityUpdate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmailForumUpdates.SummaryEngine
{
    public class MailHelper
    {
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;
        private readonly ILogger _logger;
        private readonly SmtpSettings _smtpSettings;
        private readonly int _forumPostTruncLen;
        private readonly string _baseUrl;
        public MailHelper(SmtpSettings smtpSettings, IRazorViewToStringRenderer razorViewToStringRenderer, int forumPostTruncLen = 64)
        {
            _smtpSettings = smtpSettings;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _forumPostTruncLen = forumPostTruncLen;
            _baseUrl = (smtpSettings.IsHttpSsl ? "https" : "http") + $":${smtpSettings.DomainName}/Forums";
        }

        public async Task SendMail(ActivitySummaryForUser uas, CancellationToken cancellationToken)
        {
            // note not heeding the PopForums Settings:
            //    MailerQuantity, MailSendingInverval
            // you will have to institute these yourself if important to your project
            //
            var msg = await CreateMail(uas);
            using (var smtpClient = await OpenSmtpConn(cancellationToken))
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    await smtpClient.SendAsync(msg);
                }
            }
        }

        public async Task<MimeMessage> CreateMail(ActivitySummaryForUser uas)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_smtpSettings.MailerAddress));
            msg.To.Add(new MailboxAddress(uas.User.Email));
            msg.Subject = $"Update on the latest {_smtpSettings.ForumTitle} activity";
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = $"This is an update on the goings on in the {_smtpSettings.ForumTitle} community. It is optimised for display with HTML. If you cannot read the details of recent forum posts, please go to {_baseUrl}/Recent for an update";
            var sumVM = MapToSummaryVM(uas);
            bodyBuilder.HtmlBody = await _razorViewToStringRenderer.RenderViewToStringAsync("/Views/Emails/ActivityUpdate/ActivityUpdateEmail.cshtml", sumVM);
            msg.Body = bodyBuilder.ToMessageBody();
            return msg;
        }

        private async Task<SmtpClient> OpenSmtpConn(CancellationToken cancellationToken)
        {
            var client = new SmtpClient();
            client.Timeout = 2000;
            await client.ConnectAsync(_smtpSettings.SmtpServer, _smtpSettings.SmtpPort, _smtpSettings.UseSslSmtp, cancellationToken);
            if (_smtpSettings.UseEsmtp & !cancellationToken.IsCancellationRequested)
            {
                client.Authenticate(_smtpSettings.SmtpUser, _smtpSettings.SmtpPassword, cancellationToken);
            }
            return client;
        }

        private ActivityUpdateEmailVM MapToSummaryVM(ActivitySummaryForUser userSum)
        {
            var utcNow = DateTime.UtcNow;
            var returnVar = new ActivityUpdateEmailVM
            {
                BaseUrl = _baseUrl,
                Topics = userSum.Summaries.Select(us =>
                {
                    var uasVM = MapActivitySummaryToVM(us, utcNow);
                    uasVM.IsTopicUserSubscribed = userSum.SubscribedTopics.Contains(us.TopicID);
                    return uasVM;
                })
            };
            return returnVar;
        }
        private ActivityUpdateEmailTopicVM MapActivitySummaryToVM(RecentActivitySummaryModel dbModel, DateTime utcNow)
        {
            string ago = " before this was sent";
            return new ActivityUpdateEmailTopicVM
            {
                CatTitle = dbModel.CatTitle,
                ForumTitle = dbModel.ForumTitle,
                ForumUrlName = dbModel.ForumUrlName,
                TopicTitle = dbModel.TopicTitle,
                TopicUrlName = dbModel.TopicUrlName,
                ReplyCount = dbModel.ReplyCount,
                StartedByName = dbModel.PostName1,
                StartedTimeAgo = dbModel.PostTime1.ToLongAgoString(utcNow, ago),
                StartingText = dbModel.Text1.StripTruncateHtml(_forumPostTruncLen),
                LastPostName = dbModel.PostNameL,
                LastPostUtc = dbModel.PostTimeL,
                LastPostTimeAgo = dbModel.PostTimeL.ToLongAgoString(utcNow, ago),
                LastText = dbModel.TextL.StripTruncateHtml(_forumPostTruncLen),
            };
        }
    }
}
