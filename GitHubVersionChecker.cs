using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IPAM_NOTE
{
	public class GitHubVersionChecker
	{
		private readonly string _repositoryOwner;
		private readonly string _repositoryName;

		public GitHubVersionChecker(string repositoryOwner, string repositoryName)
		{
			_repositoryOwner = repositoryOwner;
			_repositoryName = repositoryName;
		}

		public async Task<string> GetLatestVersionAsync()
		{
			try
			{
				string apiUrl = $"https://api.github.com/repos/{_repositoryOwner}/{_repositoryName}/releases/latest";

				using (HttpClient client = new HttpClient())
				{
					client.DefaultRequestHeaders.Add("User-Agent", "YourAppName");
					HttpResponseMessage response = await client.GetAsync(apiUrl);
					response.EnsureSuccessStatusCode();

					string json = await response.Content.ReadAsStringAsync();
					var release = JsonConvert.DeserializeObject<GitHubRelease>(json);

					return release.TagName;
				}
			}
			catch (Exception ex)
			{
				// 处理异常
				Console.WriteLine($"An error occurred: {ex.Message}");
				return null;
			}
		}

		public async Task<string> GetDownloadUrlAsync()
		{
			try
			{
				string apiUrl = $"https://api.github.com/repos/{_repositoryOwner}/{_repositoryName}/releases/latest";

				using (HttpClient client = new HttpClient())
				{
					client.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36 Edg/95.0.1020.53");
					HttpResponseMessage response = await client.GetAsync(apiUrl);
					response.EnsureSuccessStatusCode();

					string json = await response.Content.ReadAsStringAsync();
					var release = JsonConvert.DeserializeObject<GitHubRelease>(json);

					// 遍历附件列表，找到下载链接
					foreach (var asset in release.Assets)
					{
						if (!string.IsNullOrEmpty(asset.DownloadUrl))
						{
							return asset.DownloadUrl;
						}
					}

					// 如果找不到下载链接，返回 null
					return null;
				}
			}
			catch (Exception ex)
			{
				// 处理异常
				Console.WriteLine($"An error occurred: {ex.Message}");
				return null;
			}
		}

		private class GitHubRelease
		{
			[JsonProperty("tag_name")]
			public string TagName { get; set; }

			[JsonProperty("assets")]
			public List<GitHubAsset> Assets { get; set; }
		}

		private class GitHubAsset
		{
			[JsonProperty("browser_download_url")]
			public string DownloadUrl { get; set; }
		}

	}
}
