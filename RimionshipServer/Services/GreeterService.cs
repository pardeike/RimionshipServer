using Api;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace RimionshipServer
{
	//[Authorize]
	public class GreeterService : API.APIBase
	{
		private readonly ILogger<GreeterService> _logger;

		public GreeterService(ILogger<GreeterService> logger)
		{
			_logger = logger;
		}

		public override Task<HelloReply> Hello(HelloRequest request, ServerCallContext context)
		{
			_logger.LogWarning("Hello request");
			return Task.FromResult(new HelloReply { Id = Guid.NewGuid().ToString() });
		}
	}
}
