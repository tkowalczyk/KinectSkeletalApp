using Microsoft.Kinect;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KinectSkeletalApp
{
    public partial class MainWindow : Window
    {
        KinectSensor _sensor;

        public MainWindow()
        {
            InitializeComponent();
        }

        bool closing = false;
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                _sensor = KinectSensor.KinectSensors[0];
                if (_sensor.Status == KinectStatus.Connected)
                {
                    _sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    _sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    _sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(_sensor_AllFramesReady);

                    var parameters = new TransformSmoothParameters
                    {
                        Smoothing = 0.3f,
                        Correction = 0.0f,
                        Prediction = 0.0f,
                        JitterRadius = 1.0f,
                        MaxDeviationRadius = 0.5f
                    };
                    _sensor.SkeletonStream.Enable(parameters);

                    _sensor.Start();

                    _sensor.ElevationAngle = 0;
                }
            }
        }

        void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (closing)
            {
                return;
            }

            Skeleton first = GetFirstSkeleton(e);

            if (first == null)
            {
                return;
            }

            GetCameraPoint(first, e);
        }

        void GetCameraPoint(Skeleton first, AllFramesReadyEventArgs e)
        {
            using (DepthImageFrame depth = e.OpenDepthImageFrame())
            {
                if (depth == null ||
                    _sensor == null)
                {
                    return;
                }

                DepthImagePoint headDepthPoint = this._sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(
                    first.Joints[JointType.Head].Position, DepthImageFormat.Resolution640x480Fps30);

                DepthImagePoint leftDepthPoint = this._sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(
                    first.Joints[JointType.HandLeft].Position, DepthImageFormat.Resolution640x480Fps30);

                DepthImagePoint rightDepthPoint = this._sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(
                    first.Joints[JointType.HandRight].Position, DepthImageFormat.Resolution640x480Fps30);

                ColorImagePoint headColorPoint = this._sensor.CoordinateMapper.MapDepthPointToColorPoint(
                    DepthImageFormat.Resolution640x480Fps30, headDepthPoint, ColorImageFormat.RgbResolution640x480Fps30);

                ColorImagePoint leftColorPoint = this._sensor.CoordinateMapper.MapDepthPointToColorPoint(
                    DepthImageFormat.Resolution640x480Fps30, leftDepthPoint, ColorImageFormat.RgbResolution640x480Fps30);

                ColorImagePoint rightColorPoint = this._sensor.CoordinateMapper.MapDepthPointToColorPoint(
                    DepthImageFormat.Resolution640x480Fps30, rightDepthPoint, ColorImageFormat.RgbResolution640x480Fps30);

                CameraPosition(ellipseHead, headColorPoint);
                CameraPosition(ellipseLeft, leftColorPoint);
                CameraPosition(ellipseRight, rightColorPoint);
            }
        }

        Skeleton GetFirstSkeleton(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }

                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                Skeleton first = (from s in allSkeletons
                                  where s.TrackingState == SkeletonTrackingState.Tracked
                                  select s).FirstOrDefault();

                return first;
            }
        }

        private void CameraPosition(FrameworkElement element, ColorImagePoint point)
        {
            Canvas.SetLeft(element, point.X - element.Width / 2);
            Canvas.SetTop(element, point.Y - element.Height / 2);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_sensor != null)
            {
                _sensor.Stop();
            }
        }
    }
}