using Discount.API.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /*Database Migration:
                even in code first approach, most of the time we will create the db tables manually
                and then try to create the classes based on them manually. But to minimise the manaul 
                efforts for creating tables, we will use migrations. This step will automatically
                create the db tables based on the entity classes and seed the data. */

            /*in order to do the migrations for the postgredb we will create the extension 
              here. */

            //CreateHostBuilder(args).Build(); line will return the IHostBuilder
            var host = CreateHostBuilder(args).Build();
            host.MigrateDatabase<Program>();
            host.Run();

           /*below is the original line, divided this line using IHostBuilder object
           and inserted method "MigrateDatabase" between the Build() and Run()*/

           //CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
