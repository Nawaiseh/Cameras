using Cameras.Data;


namespace Cameras.UI
{
	public partial class MainWindow
	{
		internal static MainWindow Window { get; private set; }
		public MainWindow()
		{
			InitializeComponent();
			Window = this;
			CCTVCameras.CreateCameras();
			//CCTVCameras.DownloadCamerasImagery(FilePath.Combine(Environment.CurrentDirectory, "Output"), TimeStamp);
		}

		private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{

		}
	}
}
