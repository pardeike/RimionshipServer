using Api;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using RimionshipServer.Models;
using System;
using System.Threading.Tasks;

namespace RimionshipServer
{
	//[Authorize]
	public class APIService : API.APIBase
	{
		private readonly ILogger<APIService> _logger;

		public APIService(ILogger<APIService> logger)
		{
			_logger = logger;
		}

		public override async Task<HelloReply> Hello(HelloRequest request, ServerCallContext context)
		{
			var randBytes = new byte[80];
			new Random().NextBytes(randBytes);
			var modId = BitConverter.ToString(randBytes).Replace("-", string.Empty);
			_logger.LogWarning("Hello request -> {}", modId);

			using var dataContext = new DataContext();
			_ = await dataContext.Participants.AddAsync(new Participant() { Mod = modId });
			_ = await dataContext.SaveChangesAsync();

			return new HelloReply { Id = modId };
		}
	}
}
