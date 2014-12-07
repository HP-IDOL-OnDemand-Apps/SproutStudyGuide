using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Collections.ObjectModel;
using HP.PC.Presentation;
using SproutStudyGuide;


namespace WpfCsSample.CodeSampleControls.TrackingHandler
{
    partial class TrackingHandlerMatControl: UserControl//, ICodeSampleControl
    {
        public SproutStudyGuide.MainWindow mainWindow { get; set; }
        public int Order { get { return 13; } }
        public string Title { get { return "Object Tracking"; } }
        public string Description { get { return "This sample demonstrates how to track 2D objects. Place one or more 2D objects such as postcards or photographs on the touch mat, and then tap Capture. Edit the object names, and then tap Start. Move the objects on and above the mat to observe object tracking. Tap Stop to stop object tracking."; } }

        private IPcLink sdk;
        private IPcObjectTrackingHandler trackingHandler;
        IPcSpecification spec;

        private HorizontalWindow horizontalWindow = new HorizontalWindow();
        public HorizontalWindow HorizontalWindow {
            get { return horizontalWindow; }
            set { horizontalWindow = value; }
        }

        private Timer _timer;
        private DateTime _trackingTimeStamp;

        public async void PerformCapture_Click(object sender, RoutedEventArgs e)
        {
            //App.ClearErrorStatus();
            //horizontalWindow = new HorizontalWindow();
            horizontalWindow.GridMat.Children.Clear();
            try
            {
                DataContext.CanCapture = false;
                DataContext.Root = null;
                DataContext.ItemCollection.Clear();

                // The SDK link provides SDK functionality to the application.
                sdk = HPPC.CreateLink();
              
                    // Before registering the window, be sure WPF has a handle to the Window.
                    // See: http://msdn.microsoft.com/en-us/library/system.windows.interop.windowinterophelper.ensurehandle%28v=vs.110%29.aspx
                    var windowInteropHelper = new WindowInteropHelper(horizontalWindow);
                    windowInteropHelper.EnsureHandle();

                    // Register the window for the mat.
                    using(var windowRegistration = await sdk.RegisterWindowAsync(horizontalWindow))
                    {
                        // Turn on the projector.
                        await windowRegistration.DisplayAsync();

                        // Capture a moment. All captured entities (for example, pictures and outlines) will be
                        // extracted from the moment.
                        using (var moment = await sdk.CaptureMomentAsync())
                        {
                            // Not needed any more
                            //horizontalWindow.Hide();

                            // Extract the top-level picture and child images.
                            var picture = await sdk.ExtractPictureAsync(moment);

                            // Extract the top-level outline and child outlines.
                            var outline = await sdk.ExtractOutlineAsync(moment);

                            // From the SDK link, use the AccessSpecificationAsync function to create
                            // an instance of the IPcSpecification interface.
                            spec = await sdk.AccessSpecificationAsync();

                            // Use the IPcScreenSpecification interface through the IPcSpecification interface to
                            // retrieve information about the mat screen. The information is used below when displaying
                            // pictures and outlines.
                            var mat = spec.Screen.Mat;

                            var root = new TrackingHandlerDataObject(picture.Image, outline.Contour,
                                picture.PhysicalBoundaries.ToScreenCoordinates(mat),"");

                            // Extract all child pictures and their outlines, assuming that indexing between
                            // a picture and its outline is consistent (that is, that the picture and the outline
                            // are in the same locations in their respective trees).
                            var combolist =
                                new List<TrackingHandlerDataObject>(
                                    picture.Children.Zip<IPcPicture, IPcOutline, TrackingHandlerDataObject>(outline.Children,
                                        (pic, oln) => new TrackingHandlerDataObject
                                            (
                                            // The image is already device independent.  
                                            image: pic.Image,
                                            // The outline contour is NOT device independent, so its container must be
                                            // stretched to match that of the image (or boundary). In this sample, this
                                            // is done by setting the captureContour control's Stretch property to Fill
                                            // in the XAML.  
                                            outline: oln.Contour,
                                            boundary: pic.PhysicalBoundaries.ToScreenCoordinates(mat),
                                            name: "Object 1")));

                            // Draw extracted pictures and their outlines.
                            DataContext.Root = root;
                            foreach (var item in combolist)
                            {
                                DataContext.ItemCollection.Add(item);

                                Image img = new Image();
                                img.Source = item.Image;
                                img.IsManipulationEnabled = true;
                                //img.RenderTransform = new ScaleTransform(item.OutlineScale.X, item.OutlineScale.Y, item.Boundary.X, item.Boundary.Y);
                                //img.RenderTransform = new MatrixTransform();
                                img.RenderTransform = new MatrixTransform(1, 0, 0, 1,item.Outline.Bounds.Left,item.Outline.Bounds.Top);

                                //Canvas cv = new Canvas();
                                //cv.Height = img.Source.Height;
                                //cv.Width = img.Source.Width;
                                //cv.RenderTransform = new ScaleTransform(item.OutlineScale.X, item.OutlineScale.Y, item.Boundary.X, item.Boundary.Y);
                                //cv.Children.Add(img);

                                //horizontalWindow.GridMat.Children.Add(new Image {Source = picture.Image});
                                horizontalWindow.GridMat.Children.Add(img);
                            }
                            mainWindow.GridRightTop.Children.Add(new Image { Source = picture.Image });
                            this.mainWindow.IsNotBusy = true;

                            this.StartButton.Opacity = 100;
                            this.CaptureButton.Content = "Recapture";
                            this.StartButton.IsEnabled = true;

                            this.CapturePreviewImage.Source = DataContext.Root.Image;

                            this.ItemsControlPanel.ItemsSource = DataContext.ItemCollection;

                        }
                    }
                }
           
            catch (Exception sdkError)
            {
                //App.UpdateErrorStatus("There was a problem running this sample: " + sdkError.Message + ".");
            }
            finally
            {
                //horizontalWindow.Close();
                DataContext.CanCapture = true;
            }
        }

        private async void StartCapture_Click(object sender, RoutedEventArgs e)
        {
            // Create Mat Window
            horizontalWindow = new HorizontalWindow();
            var windowInteropHelper = new WindowInteropHelper(horizontalWindow);
            windowInteropHelper.EnsureHandle();

            IPcWindowRegistration windowRegistration =await sdk.RegisterWindowAsync(horizontalWindow);

            // Turning on the projector
            await windowRegistration.DisplayAsync();
            
            List<PcTrainingImage> images = new List<PcTrainingImage>();

            // Add training images
            foreach (var item in DataContext.ItemCollection)
            {
                PcTrainingImage trainingImage = new PcTrainingImage(item.Name, item.Image);
                images.Add(trainingImage);
                item.IsReadOnly = true;
            }

            // Create the object-tracking handler.
            trackingHandler = sdk.CreateObjectTrackingHandler(images);

            // Subscribe to object-tracking events.
            trackingHandler.TrackUpdated += trackUpdated;

            // Start object tracking.
            trackingHandler.StartAsync().Wait();
            
            CaptureButton.Opacity = 0;
            CaptureButton.IsEnabled = false;

            this.CapturePreviewImage.Source = null;
            this.StartButton.IsEnabled = false;
            this.StartButton.Opacity = 0;

            this.StopButton.Opacity = 100;
            this.StopButton.IsEnabled = true;
            
            _trackingTimeStamp = DateTime.Now;

            // Start timer to clean the mat every 300 milliseconds
            _timer = new Timer(OnTimer, null, 300, 300);
        }

        // Clean the objects from the Mat window
        // Timer is set to every 300 milliseconds. If no object is detected after 300 milliseconds, the screen is cleaned 
        private void OnTimer(object state)
        {
            DateTime updateScreen = DateTime.Now;

            TimeSpan difference = updateScreen - _trackingTimeStamp;

            if (difference.TotalMilliseconds > 300)
            {
                Dispatcher.BeginInvoke((Action)delegate()
                {
                    horizontalWindow.GridMat.Children.Clear();
                });

            }
        }

        // Create the objects to show on the Mat window, representing the tracking results
        void trackUpdated(object sender, PcTrackEventArgs e)
        {
            // Remove previous objects
            horizontalWindow.GridMat.Children.Clear();

            foreach (PcTrackedObject child in e.TrackedObjects)
            {
                // Tracked object boundaries
                PointCollection boundPointCollection = new PointCollection();
                boundPointCollection.Add(ConversionUtilities.ToScreenCoordinates(child.PhysicalBoundaries.Location,
                    spec.Screen.Mat));
                boundPointCollection.Add(ConversionUtilities.ToScreenCoordinates(new PcPhysicalPoint(child.PhysicalBoundaries.Location.X+child.PhysicalBoundaries.Size.Width,
                    child.PhysicalBoundaries.Location.Y), 
                    spec.Screen.Mat));
                boundPointCollection.Add(ConversionUtilities.ToScreenCoordinates(new PcPhysicalPoint(child.PhysicalBoundaries.Location.X + child.PhysicalBoundaries.Size.Width, 
                    child.PhysicalBoundaries.Location.Y-child.PhysicalBoundaries.Size.Height), 
                    spec.Screen.Mat));
                boundPointCollection.Add(ConversionUtilities.ToScreenCoordinates(new PcPhysicalPoint(child.PhysicalBoundaries.Location.X, 
                    child.PhysicalBoundaries.Location.Y - child.PhysicalBoundaries.Size.Height), 
                    spec.Screen.Mat));

                Polygon myRect = new Polygon();
                myRect.Points = boundPointCollection;
                myRect.Fill = Brushes.Transparent;
                myRect.Stroke = Brushes.Red;
                myRect.StrokeThickness = 2;
                horizontalWindow.GridMat.Children.Add(myRect);

                // Tracked object quadrilateral
                PointCollection quadPointCollection = new PointCollection();
                quadPointCollection.Add(ConversionUtilities.ToScreenCoordinates(child.PhysicalQuadrilateral.A,
                    spec.Screen.Mat));
                quadPointCollection.Add(ConversionUtilities.ToScreenCoordinates(child.PhysicalQuadrilateral.B,
                    spec.Screen.Mat));
                quadPointCollection.Add(ConversionUtilities.ToScreenCoordinates(child.PhysicalQuadrilateral.C, 
                    spec.Screen.Mat));
                quadPointCollection.Add(ConversionUtilities.ToScreenCoordinates(child.PhysicalQuadrilateral.D,
                    spec.Screen.Mat));
                
                Polygon myQuad = new Polygon();
                myQuad.Points = quadPointCollection;
                myQuad.Fill = Brushes.Transparent;
                myQuad.Stroke = Brushes.Black;
                myQuad.StrokeThickness = 2;
                horizontalWindow.GridMat.Children.Add(myQuad);

                //Tracked object name
                TextBox textBlock1 = new TextBox();
                textBlock1.TextWrapping = TextWrapping.Wrap;
                textBlock1.Text = child.Name;
                textBlock1.Margin = new Thickness(boundPointCollection[0].X, boundPointCollection[0].Y,
                    boundPointCollection[2].X, 0);
                textBlock1.BorderBrush = Brushes.Transparent;
                textBlock1.FontSize = 16;
                horizontalWindow.GridMat.Children.Add(textBlock1);

            }
                      
            // Update time stamp used on timer
            _trackingTimeStamp = DateTime.Now;
        }

        public void KillTimer()
        {
            _timer.Change(0, 0);
            _timer.Dispose();
        }

        private void StopTracking_Click(object sender, RoutedEventArgs e)
        {
            // Stop object tracking.
            trackingHandler.StopAsync();
                       
            KillTimer();

            // Remove objects from both windows
            horizontalWindow.GridMat.Children.Clear();
            DataContext.ItemCollection.Clear();

            // Turn off projector
            horizontalWindow.Close();
                               
            this.CaptureButton.Content = "Capture";
            CaptureButton.Opacity = 100;
            CaptureButton.IsEnabled = true;

            this.StopButton.Opacity = 0;
            this.StopButton.IsEnabled = false;

            DataContext.Root = null;
        }
    }
}

