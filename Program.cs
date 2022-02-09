using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using log4net;
using log4net.Config;
using System.Reflection;
 
namespace Brandix.DCAP.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());           
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            var logger = LogManager.GetLogger(typeof(Program));

            CreateWebHostBuilder(args).Build().Run();
            
           
            logger.Info("API Started");
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
