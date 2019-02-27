using System;

namespace Cameras.Data
{

	internal class Point : IDisposable
	{
		#region ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■   Variables                   ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
		internal int Latitude  { get; set; } = 0;
		internal int Longitude { get; set; } = 0;
		internal int Altitude  { get; set; } = 0;
		internal int X         { get { return Longitude; } set { Longitude = value; } }
		internal int Y         { get { return Latitude;  } set { Latitude  = value; } }
		internal int Z         { get { return Altitude;  } set { Altitude  = value; } }

		#endregion
		#region ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■   Methods                     ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
		public void Dispose() => Latitude = Longitude = Altitude = 0;
		#endregion
	}
}
