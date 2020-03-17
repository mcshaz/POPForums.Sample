using EmailForumUpdates.Extensions;
using EmailForumUpdates.RepoModels;
using EmailForumUpdates.SummaryEngine;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MimeKit;
using PopForums.Configuration;
using RazorEmails.Services;
using RazorToEmail.Views.Emails.ActivityUpdate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PopForumTasks
{
    public class SendUpdateEmailCron : CronJobService
    {
        private readonly ILogger<SendUpdateEmailCron> _logger;
        private readonly string _dbConnectionString;
        public IServiceProvider Services { get; }

        public SendUpdateEmailCron(IScheduleConfig<SendUpdateEmailCron> cronConfig, IServiceProvider services, ILogger<SendUpdateEmailCron> logger, IConfig configStrings)
            : base(cronConfig.CronExpression, cronConfig.TimeZoneInfo)
        {
            _logger = logger;
            _dbConnectionString = configStrings.DatabaseConnectionString;
            Services = services;
        }

        public override async Task DoWork(DateTimeOffset scheduled, CancellationToken cancellationToken)
        {
            // a real hack - GetPriorOccurence method is what is really needed
            var priorDue = _expression.GetOccurrences(scheduled.AddDays(-60), scheduled, _timeZoneInfo, false, false)
                    .Last();
            var smtpSettings = await SummaryInfoBLL.GetSmtpSettings(_dbConnectionString);
            if (!smtpSettings.IsMailerEnabled)
            {
                return;
            }
            var userSumaries = await SummaryInfoBLL.GetSummariesForUsers(_dbConnectionString, priorDue.UtcDateTime, 128);
            using (var scope = Services.CreateScope())
            {
                var razorViewToStringRenderer = scope.ServiceProvider.GetRequiredService<IRazorViewToStringRenderer>();
                var mailHelper = new MailHelper(smtpSettings, razorViewToStringRenderer);
                foreach (var s in userSumaries)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogWarning("Cancellation requested during email sending loop");
                        break;
                    }
                    try
                    {
                        await mailHelper.SendMail(s, cancellationToken);
                        _logger.LogInformation("Email sent to " + s.User.Name);
                    }
                    catch(Exception e)
                    {
                        _logger.LogError("Unable to send email to " + s.User.Email, e);
                    }
                    
                }
            }
        }


    }
}