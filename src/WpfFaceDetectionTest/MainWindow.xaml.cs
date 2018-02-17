using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Runtime.InteropServices;

namespace WpfFaceDetectionTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VideoCapture _capture;
        private CascadeClassifier _haarCascade;
        DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _capture = new VideoCapture();
            _haarCascade = new CascadeClassifier(@"haarcascade_frontalface_alt_tree.xml");

            _timer = new DispatcherTimer();
            _timer.Tick += (_, __) => 
            {
                Image<Bgr, Byte> currentFrame = _capture.QueryFrame().ToImage<Bgr, Byte>();

                if (currentFrame != null)
                {
                    Image<Gray, Byte> grayFrame = currentFrame.Convert<Gray, Byte>();

                    var detectedFaces = _haarCascade.DetectMultiScale(grayFrame);

                    foreach (var face in detectedFaces)
                        currentFrame.Draw(face, new Bgr(0, double.MaxValue, 0), 3);

                    image1.Source = ToBitmapSource(currentFrame);
                }
            };
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            _timer.Start();
            
        }
        
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }
    }
}
