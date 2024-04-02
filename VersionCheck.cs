using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IPAM_NOTE
{
	internal class VersionCheck
	{

		public class GitHubRelease
		{
			[JsonProperty("tag_name")]
			public string TagName { get; set; }
		}

		public class VersionChecker
		{
			private const string GitHubRepoUrl = "https://api.github.com/repos/yaobus/IPAM-NOTE/releases/latest";
			private readonly HttpClient _httpClient;

			public VersionChecker()
			{
				_httpClient = new HttpClient();
				_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("IPAM-NOTE");
			}

			public async Task<string> GetLatestVersionAsync()
			{
				try
				{
					HttpResponseMessage response = await _httpClient.GetAsync(GitHubRepoUrl);
					response.EnsureSuccessStatusCode();

					string json = await response.Content.ReadAsStringAsync();
					var release = JsonConvert.DeserializeObject<GitHubRelease>(json);

					return release.TagName;
				}
				catch (Exception ex)
				{
					// 处理异常
					Console.WriteLine($"An error occurred: {ex.Message}");
					return null;
				}
			}
		}


	}

}
