using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Extensions
{
    //we are going to add a static method for host extensions.
    public static class HostExtensions
    {
        /*
         the return type is IHost as we are trying to extend the IHost and migrate the db
        using the IHost Object from program class.

        TContext : means our method is generic method

        Params : host-> explains that our method is of type IHost
                 retry -> nullable int, to retry the MigrateDatabase if fails.
         */
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            // read the value from nullable parameter.
            int retryForAvailability = retry.Value;

            //CreateScope() is from Microsoft Dependency injection.
            // we wanted to use the dependency injection concept of logging and configuration here.
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating Postgresql Database.");

                    // to migrate the database we should have the connection string
                    using var connection = new NpgsqlConnection
                        (configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

                    connection.Open();

                    using var command = new NpgsqlCommand
                    {
                        Connection = connection
                    };

                    //Migration scripts
                    #region Migration scripts
                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();

                    command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone X Discount', 450);";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung 10 Discount', 300);";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Nokia 6.1+', 'Nokia 6.1+ Discount', 120);";
                    command.ExecuteNonQuery();
                    #endregion
                    logger.LogInformation("Migrated postresql database.");

                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occured while migrating the postgre sql dataabase");

                    //while application start, docker may not contain  the image for db due to latency.
                    //So we invoke the migrate database if fails during startup.

                    /* to check retry functionality : check the docker container is running 
                     or not for the discount db. if its is starting condition, stop the container
                    and run the discount.api by setting up as startup project.
                    
                     Note: while starting this project concern (discount db) docker container should 
                    be up and running, for migrating the database*/

                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        //invoke the method again.
                        MigrateDatabase<TContext>(host, retryForAvailability);
                    }

                }
            }
            //return the host of type IHost.
            return host;
        }
    }
}
