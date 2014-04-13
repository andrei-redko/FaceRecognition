using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace FaceRecognition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Capture capture = new Capture();
        private DispatcherTimer timer = new DispatcherTimer();

        private void GetVideo(object sender, EventArgs e)
        {
            Image<Bgr, Byte> Kadr = capture.QueryFrame();
            CameraImage.Source = ToBitmapSource(Kadr);
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap();

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr);
                return bs;
            }
        }

        private void ButtonStart_OnClickuttonStart_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Interval = new TimeSpan(0, 0, 0, 1000 / 60);
            timer.Tick += GetVideo;
            timer.Start();
        }
    }
}
