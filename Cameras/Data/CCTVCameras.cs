using Cameras.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
namespace Cameras.Data
{
    internal class CCTVCameras
    {
		#region ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■   Static Variables            ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
		private static string Url                       { get; } = "http://its.txdot.gov/ITS_WEB/FrontEnd/svc/DataRequestWebService.svc/GetCctvContent";
		private static int StartIndex                   { get; } = 25;
		private static List<string> CamerasListAsString { get; } = new List<string>
		{
            // Replace "DateTime Here" with Current Date And Time
            // Sample: 02\/04\/2019 at 10:06 AM
            @"DAL1,33101120,-96677750,US75,North,US75 @ Bethany,US75 @ Bethany__DAL1",
			@"DAL1,33091139,-96680964,US75,North,US75 @ Ridgemont,US75 @ Ridgemont__DAL1",
			@"DAL1,33079716,-96684443,US75,North,US75 @ Legacy,US75 @ Legacy__DAL1",
			@"DAL1,33066685,-96689663,US75,North,US75 @ Spring Creek Pkwy,US75 @ Spring Creek Pkwy__DAL1",
			@"DAL1,33058644,-96692967,US75,North,US75 @ Parker North,US75 @ Parker North__DAL1",
			@"DAL1,33047800,-96699640,US75,North,US75 @ Parker South,US75 @ Parker South__DAL1",
			@"DAL1,33036360,-96705560,US75,North,US75 @ Park Blvd,US75 @ Park Blvd__DAL1",
			@"DAL1,33025550,-96709390,US75,North,US75 @ PGBT N.E. 1,US75 @ PGBT N.E. 1__DAL1",
			@"DAL1,33007578,-96707446,US75,North,US75 @ PGBT S.W. 1,US75 @ PGBT S.W. 1__DAL1",
			@"DAL1,33003523,-96712635,US75,North,US75 @ PGBT N.E. 2,US75 @ PGBT N.E. 2__DAL1",
			@"DAL1,33004489,-96704176,US75,North,US75 @ PGBT S.W. 2,US75 @ PGBT S.W. 2__DAL1",
			@"DAL1,33000612,-96709205,US75,North,US75 @ SH190 (PGBT),US75 @ SH190 (PGBT)__DAL1",
			@"DAL1,33002599,-96708814,US75,North,US75 @ Renner,US75 @ Renner__DAL1",
			@"DAL1,32995308,-96709811,US75,North,US75 @ Galatyn North,US75 @ Galatyn North__DAL1",
			@"DAL1,32987090,-96710500,US75,North,US75 @ Galatyn South,US75 @ Galatyn South__DAL1",
			@"DAL1,32980768,-96713357,US75,North,US75 @ Campbell,US75 @ Campbell__DAL1",
			@"DAL1,32974500,-96716010,US75,North,US75 @ Collins,US75 @ Collins__DAL1",
			@"DAL1,32968424,-96720862,US75,North,US75 @ Arapaho,US75 @ Arapaho__DAL1",
			@"DAL1,32963990,-96723670,US75,North,US75 @ Beltline,US75 @ Beltline__DAL1",
			@"DAL1,32955040,-96732178,US75,North,US75 @ Spring Valley North,US75 @ Spring Valley North__DAL1",
			@"DAL1,32945603,-96739115,US75,North,US75 @ Spring Valley,US75 @ Spring Valley__DAL1",
			@"DAL1,32941656,-96744109,US75,North,US75 @ Midpark,US75 @ Midpark__DAL1",
			@"DAL1,32937440,-96749430,US75,North,US75 @ IH635 North,US75 @ IH635 North__DAL1",
			@"DAL1,32931430,-96756920,US75,North,High Five S.E. 1,High Five S.E. 1__DAL1",
			@"DAL1,32922852,-96763736,US75,North,High Five S.E. 2,High Five S.E. 2__DAL1",
			@"DAL1,32920326,-96765005,US75,North,US75 @ Coit,US75 @ Coit__DAL1",
			@"DAL1,32918000,-96768220,US75,North,US75 @ Forest,US75 @ Forest__DAL1",
			@"DAL1,32910059,-96769262,US75,North,US75 @ Royal North,US75 @ Royal North__DAL1",
			@"DAL1,32901890,-96769120,US75,North,US75 @ Royal South,US75 @ Royal South__DAL1",
			@"DAL1,32897420,-96768990,US75,North,US75 @ Meadow North,US75 @ Meadow North__DAL1",
			@"DAL1,32889797,-96769890,US75,North,US75 @ Meadow South,US75 @ Meadow South__DAL1",
			@"DAL1,32887006,-96769729,US75,North,US75 @ Walnut Hill,US75 @ Walnut Hill__DAL1",
			@"DAL1,32879500,-96769860,US75,North,US75 @ Park Lane,US75 @ Park Lane__DAL1",
			@"DAL1,32873515,-96769986,US75,North,US75 @ NorthPark,US75 @ NorthPark__DAL1",
			@"DAL1,32868710,-96769940,US75,North,US75 @ Caruth Haven,US75 @ Caruth Haven__DAL1",
			@"DAL1,32861150,-96770490,US75,North,US75 @ Lovers Lane,US75 @ Lovers Lane__DAL1",
			@"DAL1,32851480,-96771480,US75,North,US75 @ University,US75 @ University__DAL1",
			@"DAL1,32845630,-96773140,US75,North,US75 @ SMU Blvd.,US75 @ SMU Blvd.__DAL1",
			@"DAL1,32842216,-96774989,US75,North,US75 @ Mockingbird,US75 @ Mockingbird__DAL1",
			@"DAL1,32837550,-96778240,US75,North,US75 @ McCommas,US75 @ McCommas__DAL1",
			@"DAL1,32831930,-96780810,US75,North,US75 @ Monticello,US75 @ Monticello__DAL1",
			@"DAL1,32828460,-96783000,US75,North,US75 @ Knox North,US75 @ Knox North__DAL1",
			@"DAL1,32824010,-96785090,US75,North,US75 @ Knox South,US75 @ Knox South__DAL1",
			@"DAL1,32818880,-96787810,US75,North,US75 @ Fitzhugh,US75 @ Fitzhugh__DAL1",
			@"DAL1,32814370,-96790270,US75,North,US75 @ Haskell,US75 @ Haskell__DAL1",
			@"DAL1,32809170,-96792980,US75,North,US75 @ Lemmon,US75 @ Lemmon__DAL1",
			@"DAL1,32804980,-96793540,US75,North,US75 @ Hall,US75 @ Hall__DAL1",
			@"DAL1,32801180,-96792780,US75,North,US80 @ Big Town (DalTrans),US80 @ Big Town__DAL1",
		};


		private static SortedList<string, SortedList<string, SortedList<string, Camera>>> Cameras { get; } = new SortedList<string, SortedList<string, SortedList<string, Camera>>>();
		#endregion
		#region ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■   Static Methods              ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
		internal static void CreateCameras()
		{
			int EndIndex = StartIndex + CamerasListAsString.Count;
			Cameras.Clear();
			for (int CameraIndex = StartIndex; CameraIndex < EndIndex; CameraIndex++)
			{
				Camera Camera = Camera.CreateCamera(CamerasListAsString[CameraIndex - StartIndex], CameraIndex);
				if (!Cameras.ContainsKey(Camera.Region)) { Cameras.Add(Camera.Region, new SortedList<string, SortedList<string, Camera>>()); }
				if (!Cameras[Camera.Region].ContainsKey(Camera.RoadWay)) { Cameras[Camera.Region].Add(Camera.RoadWay, new SortedList<string, Camera>()); }
				Cameras[Camera.Region][Camera.RoadWay].Add(Camera.Id, Camera);
			}
		}
		internal static void DownloadCamerasImagery(string Folder, string Date, string Time)
		{
			Folder = Path.Combine(Folder, $"{Date}__{Time}");
			foreach (string Region in Cameras.Keys)
			{
				string RegionFolder = Path.Combine(Folder, Region);
				SortedList<string, SortedList<string, Camera>> RegionCameras = Cameras[Region];
				foreach (string Roadway in RegionCameras.Keys)
				{
					string RoadwayFolder = Path.Combine(RegionFolder, Roadway);
					SortedList<string, Camera> RoadwayCameras = RegionCameras[Roadway];
					if (!Directory.Exists(RoadwayFolder)) { Directory.CreateDirectory(RoadwayFolder); }
					foreach (Camera Camera in RoadwayCameras.Values)
					{
						Camera.Response = ImageDownloader.DownloadPage(Url, Camera);
						Camera.SaveResponsToImageFile(RoadwayFolder);
					}
				}
			}
		}

		internal static void StartDownloading(DateTime TimeStamp)
		{
			string Folder = Path.Combine(Environment.CurrentDirectory,"Output");
			string Date   = $"TimeStamp___{TimeStamp.ToString(@"yyyy_MM_dd")}";
			string Time   = TimeStamp.ToString(@"hh_mm_ss_tt");
			Thread Thread = new Thread(() => DownloadCamerasImagery(Folder, Date, Time));
			Thread.Start();
		}

		#endregion
	}
}
