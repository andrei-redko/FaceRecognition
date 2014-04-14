using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
using Label = System.Windows.Controls.Label;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Orientation = System.Windows.Controls.Orientation;
using FaceRecognition.FaceDetection;

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

        private Capture capture;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private bool cameraIsActivated = false;

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Image<Bgr, Byte> kadr = capture.QueryFrame();
            cameraImage.Source = ToBitmapSource(kadr);
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

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (cameraIsActivated)
            {
                dispatcherTimer.Stop();
                if (capture != null)
                {
                    capture.Dispose();
                }
                buttonStart.Content = "Старт";
                cameraIsActivated = false;
            }
            else
            {
                try
                {
                    capture = new Capture();
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                buttonStart.Content = "Стоп";
                cameraIsActivated = true;
                dispatcherTimer.Start();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000/60);
        }

        private void buttonSnapshot_Click(object sender, RoutedEventArgs e)
        {
            if (cameraImage.Source != null)
            {
                AddImage(cameraImage.Source);
            }
        }

        private void uploadFromFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "(*.bmp, *.jpg, *.jpeg)|*.bmp; *.jpg; *.jpeg";
            ofd.ShowDialog();

            BitmapImage imageSource = new BitmapImage();
            if (ofd.FileName != String.Empty)
            {
                imageSource.BeginInit();
                imageSource.UriSource = new Uri(ofd.FileName, UriKind.Relative);
                imageSource.CacheOption = BitmapCacheOption.OnLoad;
                imageSource.EndInit();

                AddImage(imageSource);
            }
        }

        private void AddImage(ImageSource bitmapSource)
        {
            StackPanel imagePanel = new StackPanel();
            imagePanel.Orientation = Orientation.Vertical;

            Image image = new Image();
            image.Source = bitmapSource;
            image.Height = 300;

            Label label = new Label();
            label.Content = "Удалить";
            label.MouseLeftButtonDown += RemoveImage;

            imagePanel.Children.Add(label);
            imagePanel.Children.Add(image);
            imagePanel.Margin = new Thickness(10, 0, 10, 0);
            listImage.Children.Add(imagePanel);
        }

        private void RemoveImage(object sender, RoutedEventArgs e)
        {
            Label label = (Label) sender;
            listImage.Children.Remove((StackPanel)label.Parent);
        }

        private void faceDetection_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in listImage.Children)
            {
                ImageSource imagrSource = ((Image)((StackPanel) child).Children[1]).Source;
                FaceDetectionBase fd = new FaceDetectionHaarCascade(imagrSource);
                fd.DetectionFace();
            }
        }
    }
}
