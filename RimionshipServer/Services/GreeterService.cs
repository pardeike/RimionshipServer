using Grpc.Core;
using Helloworld;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace RimionshipServer
{
	//[Authorize]
	public class GreeterService : Greeter.GreeterBase
	{
		private readonly ILogger<GreeterService> _logger;

		public GreeterService(ILogger<GreeterService> logger)
		{
			_logger = logger;
		}

		public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
		{
			_logger.LogWarning("Progress request from {Name} with age {Age}", request.Name, request.Age);
			return Task.FromResult(new HelloReply
			{
				Message = $"Hello {request.Name}"
			});
		}
	}
}
