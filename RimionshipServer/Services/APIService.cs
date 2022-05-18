using Api;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RimionshipServer.Models;
using System;
using System.Linq;
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
			using var dataContext = new DataContext();
			var oldId = request.Id ?? "";
			var existingParticipant = await dataContext.Participants.FirstOrDefaultAsync(p => p.Mod == oldId);
			if (existingParticipant != null)
				return new HelloReply { Id = oldId };

			var randBytes = new byte[80];
			new Random().NextBytes(randBytes);
			var newId = BitConverter.ToString(randBytes).Replace("-", string.Empty);
			_ = await dataContext.Participants.AddAsync(new Participant() { Mod = newId });
			_ = await dataContext.SaveChangesAsync();
			return new HelloReply { Id = newId };
		}
	}
}
