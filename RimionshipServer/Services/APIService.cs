using Grpc.Core;
using Microsoft.Extensions.Logging;
using RimionshipAPI;
using System.Threading.Tasks;

namespace RimionshipServer
{
	public class APIService : API.APIBase
	{
		private readonly ILogger<APIService> _logger;

		public APIService(ILogger<APIService> logger)
		{
			_logger = logger;
		}

		public override Task<ProgressReply> Send(ProgressRequest request, ServerCallContext context)
		{
			_logger.LogWarning("Progress request from {Name}", request.Name);
			return Task.FromResult(new ProgressReply
			{
				Message = "Hello " + request.Name
			});
		}
	}
}
