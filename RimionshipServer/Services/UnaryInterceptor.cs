using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace RimionshipServer.Services
{
	public class UnaryInterceptor : Interceptor
	{
		private readonly ILogger<APIService> _logger;

		public UnaryInterceptor(ILogger<APIService> logger)
		{
			_logger = logger;
		}

		public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
			 TRequest request,
			 ServerCallContext context,
			 UnaryServerMethod<TRequest, TResponse> continuation)
		{
			_logger.LogInformation("Client call {Method}", context.Method);
			try
			{
				return await continuation(request, context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception in {Method}: {Ex}", context.Method, ex);
				throw;
			}
		}
	}
}
