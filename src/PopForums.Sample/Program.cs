using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PopForums.Sample
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{
			// var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
			var builder = WebHost.CreateDefaultBuilder(args);

			builder.UseIIS();

			builder.UseStartup<Startup>();
			return builder;
		}
	}
}
