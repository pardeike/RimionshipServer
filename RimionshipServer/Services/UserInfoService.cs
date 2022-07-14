using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RimionshipServer.Auth;
using System.Threading.Tasks;

namespace RimionshipServer.Services
{
	public class UserInfoService
	{
		private readonly IConfiguration _configuration;
		private readonly TokenProvider _tokenProvider;
		private readonly ILogger<UserInfoService> _logger;

		public UserInfoService(IConfiguration configuration, TokenProvider tokenProvider, ILogger<UserInfoService> logger)
		{
			_configuration = configuration;
			_tokenProvider = tokenProvider;
			_logger = logger;
		}

		public async Task<UserInfo> RunAsync(string userName) // change to use channel_id
		{
			var clientid = _configuration["Twitch:ClientId"];
			var token = _tokenProvider.AccessToken;

			_logger.LogWarning("clientid = {clientid}", clientid);
			_logger.LogWarning("token = {token}", token);

			var result = await "https://api.twitch.tv/helix/users/follows" // change path to something else than .../follows
				 .SetQueryParam("to_id", userName)
				 .SetQueryParam("first", 100)
				 .WithHeader("Authorization", $"Bearer {token}")
				 .WithHeader("Client-ID", clientid)
				 .GetJsonAsync<UserInfoWrapper>();

			_logger.LogWarning("result = {userinfo}", result.Info);
			return result.Info;
		}
	}
}
