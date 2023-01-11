using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace JSQuery;

public class WebParser {
	HttpClient client;
	HttpClientHandler handler;
	CookieContainer session;



	public WebParser() {
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

		session = new CookieContainer();

		handler = new HttpClientHandler() {
			CookieContainer = session,
			ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => {
				return true;
			}
		};
		client = new HttpClient(handler);

		SetUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.61 Safari/537.36");
	}





	public string Get(string url) {
        Task<string> task = Task.Run(async () => {
			var response = await client.GetAsync(url);
			return await ReadAsStringAsync(response.Content);
		});
        return task.Result;

	}



	public string Post<T>(string url, T value) {
		Task<string> task = Task.Run(async () => {
			var response = await client.PostAsJsonAsync(url, value);
			return await ReadAsStringAsync(response.Content);
		});
		return task.Result;
	}



	public string Put<T>(string url, T value) {
		Task<string> task = Task.Run(async () => {
			var response = await client.PutAsJsonAsync(url, value);
			return await ReadAsStringAsync(response.Content);
		});
		return task.Result;
	}



	public string Delete(string url) {
		Task<string> task = Task.Run(async () => {
			var response = await client.DeleteAsync(url);
			return await ReadAsStringAsync(response.Content);
		});
		return task.Result;
	}





	public void SetUserAgent(string header) {
		client.DefaultRequestHeaders.UserAgent.Clear();
		client.DefaultRequestHeaders.UserAgent.TryParseAdd(header);
	}




	private Task<string> ReadAsStringAsync(HttpContent content) {
		return Task.Run(async () => {
			string s;
			using (var sr = new StreamReader(await content.ReadAsStreamAsync(), Encoding.GetEncoding("iso-8859-1"))) {
				s = sr.ReadToEnd();
			}
			return s;
		});
	}




}
