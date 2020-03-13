using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Collections.Generic;
using MimeKit;
using System;

namespace PopForums.Sample.Controllers
{
    public class HomeController : Controller
    {
        /*
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        */
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult TestSmtp()
        {
            // there were problems with the security certificate - ran this to figure the problems
            // in cmd prompt ran tracert example.com
            var returnVar = new List<string>();
            var i = 0;
            foreach (var ssl in new[] { true, false })
            {
                foreach (var port in new[] { 25, 465, 587 })
                {
                    foreach (var host in new[] { "xxx", "xxx" })
                    {
                        var describe = $"ssl:{ssl}|port:{port}|host:{host}";
                        var msg = new MimeMessage();
                        msg.From.Add(new MailboxAddress("xxx"));
                        msg.To.Add(new MailboxAddress("xxx"));
                        msg.Subject = $"Test {++i}";
                        msg.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = describe };
                        using (var client = new SmtpClient())
                        {
                            client.Timeout = 2000;
                            try
                            {
                                client.Connect(host, port, ssl);
                                describe += "|connect OK";
                                client.Authenticate("xxx@xxx", "xxx");
                                describe += "|authenticate OK";
                                client.Send(msg);
                                describe += "|Sent!";
                            }
                            catch(Exception e)
                            {
                                describe += "|-Fail-:\r\n\t" + e.ToString().Replace("\r\n", "\r\n\t");
                            }
                            finally
                            {
                                client.Disconnect(true);
                            }
                            returnVar.Add(describe);
                        }
                    }
                }
            };
            ViewBag.result = returnVar;
            return View();
        }
    }
}