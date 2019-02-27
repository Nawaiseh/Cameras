using Cameras.Data;
using MahApps.Metro.Controls.Dialogs;

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace Cameras.UI.Controls
{
	public partial class ControlPanel
	{
		private const int TriggerPeriodInMinutes = 1;
		private TimeSpan CountDown { get; set; }
		private readonly TimeSpan ONE_SECOND   = new TimeSpan(0, 0, 1);
		private readonly TimeSpan TOTAL_PERIOD = new TimeSpan(0, TriggerPeriodInMinutes, 0);
		private Timer Timer { get; }           = new Timer { Interval = 1000 };
		public ControlPanel()
		{

			InitializeComponent();
			Timer.Tick += _Timer_Tick;
			ResetTimer();
		}
		internal void SetTime(string Time) => TimeCountDown.Content = Time;
		internal void StartTimer() => Timer.Start();
		internal void DisableTimer()
		{
			Timer.Interval = int.MaxValue;
			StartButton.IsEnabled = false;
			StopButton.IsEnabled = false;
		}
		private void _Timer_Tick(object sender, EventArgs e)
		{
			CountDown -= ONE_SECOND;
			SetTime(CountDown.ToString(@"mm\:ss"));
			if (CountDown.Ticks == 0)
			{
				CCTVCameras.StartDownloading(DateTime.Now);
				Start();
			}
		}


		private void Exit_Click(object sender, RoutedEventArgs e) => MainWindow.Window?.Close();
		public static Process PriorProcess()
		{

			Process[] RunningProcesses = Process.GetProcessesByName("Focus");
			if ((RunningProcesses?.Length ?? 0) == 0) { return null; }
			return RunningProcesses[0];
		}
		private void CheckForPriorInstances()
		{
			try
			{
				Process PreviousProcess = PriorProcess();
				if (PreviousProcess != null)
				{
					MessageDialogResult MessageDialogResult = MessageDialogResult.SecondAuxiliary;
					System.Windows.Application.Current.Dispatcher.Invoke(() =>
					{
						MetroDialogSettings MetroDialogSettings = new MetroDialogSettings
						{
							AffirmativeButtonText = "Close Focus Instance",
							NegativeButtonText = "Abort",
							AnimateShow = true,
							AnimateHide = true,
							DialogMessageFontSize = 14,
							ColorScheme = MetroDialogColorScheme.Accented,
							DefaultButtonFocus = MessageDialogResult.FirstAuxiliary
						};
						string Title = "A Previous Instance of Focus Is Running!";
						string Body = "A Previous Instance of Focus Is Running! Close Instance and Continue or Abort Execution?";
						MessageDialogStyle MessageDialogStyle = MessageDialogStyle.AffirmativeAndNegative;
						MessageDialogResult = MainWindow.Window.ShowModalMessageExternal(Title, Body, MessageDialogStyle, MetroDialogSettings);

						if (MessageDialogResult == MessageDialogResult.Affirmative)
						{
							PreviousProcess.Kill();
							System.Threading.Thread.Sleep(50);
						}
						else
						{
							MainWindow.Window.Close();
						}
					});

				}

			}
			catch (Exception) { }
		}

		private void Start()
		{
			CountDown             = new TimeSpan(TOTAL_PERIOD.Hours, TOTAL_PERIOD.Minutes, TOTAL_PERIOD.Seconds);
			StartButton.IsEnabled = false;
			StartTimer();
		}
		private void Start_Click(object sender, RoutedEventArgs e) => Start();
		internal void ResetTimer()
		{
			
		}

		private void Stop_Click(object sender, RoutedEventArgs e)
		{
			Timer.Stop();
			StartButton.IsEnabled = true;
		}
	}
}
