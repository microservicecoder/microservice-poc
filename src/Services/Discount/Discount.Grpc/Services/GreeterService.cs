using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc
{
    /*
    This class contains the services implementations. if we observe this service class 
    inherited from the "Greeter.GreeterBase", which is the C# generated class, that we
    discussed in greet.proto file.

    which means, greet.proto file generat the C# classes for Grpc service methods &
    GreeterService.cs implement the methods by overriding the C# generated class methods. 
     */
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
