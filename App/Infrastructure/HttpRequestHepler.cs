using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure
{
	public class HttpRequestHepler
	{
		public async Task<string> PostAsyncJson(string url, string json)
		{
			HttpClient httpClient = new HttpClient();
			HttpContent content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			HttpResponseMessage obj = await httpClient.PostAsync(url, content);
			obj.EnsureSuccessStatusCode();
			return await obj.Content.ReadAsStringAsync();
		}

		public string HttpPost(string url, string json)
		{
			Encoding encoding = Encoding.UTF8;
			byte[] responseData = new WebClient
			{
				Headers =
			{
				{
					"Content-Type",
					"application/json"
				}
			}
			}.UploadData(url, "POST", encoding.GetBytes(json));
			return encoding.GetString(responseData).Trim();
		}

		public string HttpPost(string url, string json, string token)
		{
			Encoding encoding = Encoding.UTF8;
			byte[] responseData = new WebClient
			{
				Headers =
			{
				{
					"Content-Type",
					"application/json"
				},
				{
					"Authorization",
					"Bearer " + token
				}
			}
			}.UploadData(url, "POST", encoding.GetBytes(json));
			return encoding.GetString(responseData).Trim();
		}

		public string HttpPost(string url, params KeyValuePair<string, string>[] postData)
		{
			StringBuilder data = new StringBuilder();
			for (int i = 0; i < postData.Length; i++)
			{
				KeyValuePair<string, string> kp = postData[i];
				data.Append(kp.Key + "=" + kp.Value + "&");
			}
			string dataStr = data.ToString();
			if (!string.IsNullOrEmpty(dataStr))
			{
				dataStr = dataStr.TrimEnd('&');
			}
			Encoding encoding = Encoding.UTF8;
			byte[] responseData = new WebClient
			{
				Headers =
			{
				{
					"Content-Type",
					"application/x-www-form-urlencoded"
				}
			}
			}.UploadData(url, "POST", encoding.GetBytes(dataStr));
			return encoding.GetString(responseData).Trim();
		}

		public string HttpGet(string url, params KeyValuePair<string, string>[] postData)
		{
			StringBuilder data = new StringBuilder();
			for (int i = 0; i < postData.Length; i++)
			{
				KeyValuePair<string, string> kp = postData[i];
				data.Append(kp.Key + "=" + kp.Value + "&");
			}
			string dataStr = data.ToString();
			if (!string.IsNullOrEmpty(dataStr))
			{
				dataStr = dataStr.TrimEnd('&');
			}
			Encoding uTF = Encoding.UTF8;
			byte[] responseData = new WebClient
			{
				Headers =
			{
				{
					"Content-Type",
					"application/x-www-form-urlencoded"
				}
			}
			}.DownloadData(url + "?" + dataStr);
			return uTF.GetString(responseData).Trim();
		}
	}
}
