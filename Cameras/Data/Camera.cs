using System;
using System.IO;
using Cameras.Http;

namespace Cameras.Data
{
	internal class Camera : IDisposable
	{
		#region ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■   Variables                   ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
		internal string Id           { get; private set; } = string.Empty;
		internal string Address      { get; private set; } = string.Empty;
		internal int Index           { get; private set; } = -1;
		internal string Region       { get; private set; } = string.Empty;
		internal Point Location      { get; private set; } = new Point();
		internal DateTime TimeStamp  { get; private set; }
		internal string RoadWay      { get; private set; } = string.Empty;
		internal string Direction    { get; private set; } = string.Empty;
		internal bool DeviceOnline   { get; private set; } = false;
		internal string FormData     { get => $"{{ \"arguments\":\"{Id},{Index}\" }}"; }
		internal string Response     { get; set; }
		internal string FileName     { get; set; }
		internal string SourceString { get; private set; }
		#endregion
		#region ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■   Static Methods              ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
		internal static Camera CreateCamera(string CameraSource, int Index)
		{
			//@"DAL1,33101120,-96677750,US75,North,US75 @ Bethany,US75 @ Bethany__DAL1",
			string[] Tokens = CameraSource.Split(',');
			int LastIndex = Tokens.Length - 1;
			Camera Camera = new Camera
			{
				Region       = Tokens[0],
				Location     = new Point { Longitude  = int.Parse(Tokens[2]), Latitude = int.Parse(Tokens[1]) },
				RoadWay      = Tokens[3],
				Direction    = Tokens[4],
				Id           = Tokens[6],
				Index        = Index,
				Address      = Tokens[5],
				SourceString = CameraSource
			};
			string Temp      = Camera.Address.Replace(" ", "").Replace("@", "At");
			Camera.FileName  = $"{Temp}.JPG";

			return Camera;
		}
		#endregion
		#region ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■   Methods                     ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
		internal void SaveResponsToImageFile( string Folder)
		{
			int StartIndex        = Response.IndexOf(@"data:image\/jpeg;base64,") + @"data:image\/jpeg;base64,".Length;
			int Length            = Response.Length - StartIndex - 1;
			string EncryptedImage = Response.Substring(StartIndex, Length).Replace("\\", "");
			byte[] Bytes          = Convert .FromBase64String(EncryptedImage);

			using (FileStream imageFile = new FileStream(Path.Combine(Folder, FileName), FileMode.Create))
			{
				imageFile.Write(Bytes, 0, Bytes.Length);
				imageFile.Flush();
			}
		}
		public void Dispose() { Id = null; Region = null; Location.Dispose(); RoadWay = null; }

		internal void DownloadAndSave(string Url, string RoadwayFolder)
		{
			bool Fail = false;
			do
			{
				try
				{

					Response = ImageDownloader.DownloadPage(Url, this);
					SaveResponsToImageFile(RoadwayFolder);
					Fail = false;
				}
				catch (Exception) { Fail = true; }
			} while (Fail);
		}
		#endregion
	}
}
