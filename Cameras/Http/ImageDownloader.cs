using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Cameras.Data;

namespace Cameras.Http
{
	internal static class ImageDownloader
	{
		private static string Response { get; set; } = string.Empty;

		private static CookieContainer CreateCookieContainer(string Url)
		{
			string Domain = new Uri(Url).Host; //"its.txdot.gov";
			CookieContainer CookieContainer = new CookieContainer(20);
			CookieContainer.Add(new Cookie("_ga", "GA1.2.477375114.1549295976") { Domain = Domain });
			CookieContainer.Add(new Cookie("congestionCheckbox", "true") { Domain = Domain });
			CookieContainer.Add(new Cookie("incidentsCheckbox", "true") { Domain = Domain });
			CookieContainer.Add(new Cookie("laneClosureCheckbox", "true") { Domain = Domain });
			CookieContainer.Add(new Cookie("mapZoomKey", "10") { Domain = Domain });
			CookieContainer.Add(new Cookie("mapCenterKey", "32.72276374825203: -97.33662232666016") { Domain = Domain });
			CookieContainer.Add(new Cookie("DAL1dms_roadway", "IH20 East") { Domain = Domain });
			CookieContainer.Add(new Cookie("DAL1cctv_roadway", "US75") { Domain = Domain });
			CookieContainer.Add(new Cookie("_gid", "GA1.2.941361802.1550618209") { Domain = Domain });
			CookieContainer.Add(new Cookie("DAL1cctv_element", "US75 @ Renner__DAL1") { Domain = Domain });

			return CookieContainer;
		}

		private static void FillHttpWebRequestHeaders(HttpWebRequest HttpWebRequest, string Url, string FormData, string ContentType)
		{
			CookieContainer CookieContainer = CreateCookieContainer(Url);

			HttpWebRequest.Accept = "application/json, text/javascript, */*; q=0.01";
			WebHeaderCollection Headers = HttpWebRequest.Headers;
			Headers.Add("Accept-Encoding", "gzip, deflate");
			Headers.Add("Accept-Language", "en-US,en;q=0.9,ar;q=0.8");
			HttpWebRequest.KeepAlive = true;
			HttpWebRequest.ContentLength = FormData.Length;
			HttpWebRequest.ContentType = ContentType;
			HttpWebRequest.CookieContainer = CookieContainer;
			Headers = HttpWebRequest.Headers;
			Headers.Add("DNT", "1");
			Headers.Add("Origin", "http://its.txdot.gov");
			HttpWebRequest.Host = "its.txdot.gov";

			HttpWebRequest.Referer = "http://its.txdot.gov/ITS_WEB/FrontEnd/default.html?r=DAL1&p=Dallas&t=cctv";
			HttpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36";
			Headers = HttpWebRequest.Headers;
			Headers.Add("X-Requested-With", "XMLHttpRequest");
			//Headers.Add("arguments", FormData);
			HttpWebRequest.Expect = null;

		}
		private static byte[] UpdateHttpWebRequestFormPayLoad(HttpWebRequest HttpWebRequest, string FormData)
		{
			byte[] EncodedFromData = Encoding.ASCII.GetBytes(FormData);
			EncodedFromData = Encoding.Convert(Encoding.ASCII, Encoding.UTF8, EncodedFromData);
			Stream RequestStream = HttpWebRequest.GetRequestStream();
			RequestStream.Write(EncodedFromData, 0, EncodedFromData.Length);
			return EncodedFromData;
		}
		private static string FormatText(string InputText, char Separator = ':')
		{
			string[] Lines = InputText.Split('\n');
			string FormattedText = string.Empty;
			foreach (string Line in Lines)
			{
				if (string.IsNullOrEmpty(Line.Trim().Replace("\r", ""))) { continue; }
				string[] Tokens = Line.Replace("\r", "").Split(Separator);
				string Token1 = Tokens[0];
				string RemainderText = string.Empty;
				foreach (string Part in Tokens) { if (Part == Token1) { continue; } RemainderText = $"{RemainderText}:{Part}"; }
				if (RemainderText.Length > 0)
				{
					RemainderText = RemainderText.Substring(1, RemainderText.Length - 1).Trim();
					FormattedText = $"{FormattedText}\"{Token1}\":\"{RemainderText}\",\n";
				}

			}
			return FormattedText;
		}
		private static Stream GetHttpWebRequestResponse(HttpWebRequest HttpWebRequest)
		{
			try
			{
				HttpWebResponse HResponse = (HttpWebResponse)HttpWebRequest.GetResponse();
				return HResponse.GetResponseStream();
			}
			catch (WebException)
			{
				File.WriteAllLines("Errors.json", new string[] { "{\n\"Headers\": {\n\t", FormatText(HttpWebRequest.Headers.ToString()), "\n}\n," });
				throw;
			}
		}


		// Main Method
		public static string XMLHttpRequest(string Url, string FormData, string ContentType)
		{
			HttpWebRequest HttpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
			HttpWebRequest.Method = "POST";
			byte[] EncodedFromData = null;
			try
			{
				FillHttpWebRequestHeaders(HttpWebRequest, Url, FormData, ContentType);

				EncodedFromData = UpdateHttpWebRequestFormPayLoad(HttpWebRequest, FormData);

				using (Stream ResponseStream = GetHttpWebRequestResponse(HttpWebRequest))

				using (StreamReader StreamReader = new StreamReader(ResponseStream, Encoding.UTF8)) { return StreamReader.ReadToEnd(); }

			}
			catch (WebException WebException)
			{
				string FormPayLoadDecrypted = Encoding.ASCII.GetString(EncodedFromData).Trim();
				FormPayLoadDecrypted = FormPayLoadDecrypted.Substring(1, FormPayLoadDecrypted.Length - 1).Trim();
				FormPayLoadDecrypted = FormPayLoadDecrypted.Substring(0, FormPayLoadDecrypted.Length - 1).Trim();
				File.AppendAllLines("Errors.json", new string[] { "\"FormData\": {\n\t", $"{FormPayLoadDecrypted.Replace("\",\"", "\": \"")}}},\n" });

				using (WebResponse WebResponse = WebException.Response)
				{
					using (Stream respStream = WebResponse.GetResponseStream())
					{
						StreamReader StreamReader = new StreamReader(respStream);
						string ErrorResponse = StreamReader.ReadToEnd();
						string Msg = ErrorResponse.Substring(1, ErrorResponse.Length - 1);
						Msg = Msg.Substring(0, Msg.Length - 1);
						File.AppendAllLines("Errors.json", new string[] { Msg, "\n}" });

						return ErrorResponse;
					}
				}
			}
		}



		private static void ToImage(string EncryptedImage, string FileName)
		{
			byte[] Bytes = Convert.FromBase64String(EncryptedImage);
			using (FileStream imageFile = new FileStream(FileName, FileMode.Create))
			{
				imageFile.Write(Bytes, 0, Bytes.Length);
				imageFile.Flush();
			}
		}

		internal static void DownloadPage(string Url, string SavePath, List<KeyValuePair<string, string>> Queries)
		{
			Url = "http://its.txdot.gov/ITS_WEB/FrontEnd/svc/DataRequestWebService.svc/GetCctvContent";
			string FormData = "{ \"arguments\":\"US75 @ SMU Blvd.__DAL1,61\" }";

			FileInfo FileInfo = new FileInfo(SavePath);
			if (!Directory.Exists(FileInfo.DirectoryName)) { Directory.CreateDirectory(FileInfo.DirectoryName); }
			Response = XMLHttpRequest(Url, FormData, "application/json; charset=UTF-8");
			int StartIndex = Response.IndexOf(@"data:image\/jpeg;base64,") + @"data:image\/jpeg;base64,".Length;

			int Length = Response.Length - StartIndex - 1;
			string EncryptedImage = Response.Substring(StartIndex, Length).Replace("\\", "");

			SavePath = "Page";
			File.WriteAllText(SavePath, EncryptedImage);
			ToImage(EncryptedImage, "US75 @ SMU Blvd.jpg");
		}

		internal static string DownloadPage(string Url, Camera Camera) => XMLHttpRequest(Url, Camera.FormData, "application/json; charset=UTF-8");

	}
}
