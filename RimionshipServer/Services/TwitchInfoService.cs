using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using RimionshipServer.Auth;
using RimionshipServer.Models;
using System.Threading.Tasks;

namespace RimionshipServer.Services
{
	public class TwitchInfoService
	{
		private readonly IConfiguration _configuration;
		private readonly TokenProvider _tokenProvider;

		public TwitchInfoService(IConfiguration configuration, TokenProvider tokenProvider)
		{
			_configuration = configuration;
			_tokenProvider = tokenProvider;
		}

		public async Task<UserInformation> GetUserInformation(string twitchId)
		{
			var result = await $"https://api.twitch.tv/helix/users?id={twitchId}"
					.WithHeader("Authorization", $"Bearer {_tokenProvider.AccessToken}")
					.WithHeader("Client-ID", _configuration["Twitch:ClientId"])
					.GetAsync();
			var userInfo = await result.GetJsonAsync<UserInformationHolder>();
			if (userInfo.Data.Length == 0) return null;
			return userInfo.Data[0];
		}

		public async Task<StreamInformation> GetStreamInformation(string twitchId)
		{
			var result = await $"https://api.twitch.tv/helix/streams?user_id={twitchId}"
					.WithHeader("Authorization", $"Bearer {_tokenProvider.AccessToken}")
					.WithHeader("Client-ID", _configuration["Twitch:ClientId"])
					.GetAsync();
			var streamInfo = await result.GetJsonAsync<StreamInformationHolder>();
			if (streamInfo.Data.Length == 0) return null;
			return streamInfo.Data[0];
		}
	}
}
