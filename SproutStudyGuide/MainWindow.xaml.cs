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

namespace SproutStudyGuide
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WpfCsSample.CodeSampleControls.TrackingHandler.HorizontalWindow horizontalWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //horizontalWindow = new WpfCsSample.CodeSampleControls.TrackingHandler.HorizontalWindow();
            //horizontalWindow.Show();
        }

        void Window_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = this;
            e.Handled = true;
        }

        void Window_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {

            // Get the Rectangle and its RenderTransform matrix.
            Image rectToMove = e.OriginalSource as Image;
            Matrix rectsMatrix = ((MatrixTransform)rectToMove.RenderTransform).Matrix;

            // Rotate the Rectangle.
            rectsMatrix.RotateAt(e.DeltaManipulation.Rotation,
                                 e.ManipulationOrigin.X,
                                 e.ManipulationOrigin.Y);

            // Resize the Rectangle.  Keep it square 
            // so use only the X value of Scale.
            rectsMatrix.ScaleAt(e.DeltaManipulation.Scale.X,
                                e.DeltaManipulation.Scale.X,
                                e.ManipulationOrigin.X,
                                e.ManipulationOrigin.Y);

            // Move the Rectangle.
            rectsMatrix.Translate(e.DeltaManipulation.Translation.X,
                                  e.DeltaManipulation.Translation.Y);

            // Apply the changes to the Rectangle.
            rectToMove.RenderTransform = new MatrixTransform(rectsMatrix);

            Rect containingRect =
                new Rect(((FrameworkElement)e.ManipulationContainer).RenderSize);

            Rect shapeBounds =
                rectToMove.RenderTransform.TransformBounds(
                    new Rect(rectToMove.RenderSize));

            // Check if the rectangle is completely in the window.
            // If it is not and intertia is occuring, stop the manipulation.
            if (e.IsInertial && !containingRect.Contains(shapeBounds))
            {
                e.Complete();
            }


            e.Handled = true;
        }

        void Window_InertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {

            // Decrease the velocity of the Rectangle's movement by 
            // 10 inches per second every second.
            // (10 inches * 96 pixels per inch / 1000ms^2)
            e.TranslationBehavior.DesiredDeceleration = 10.0 * 96.0 / (1000.0 * 1000.0);

            // Decrease the velocity of the Rectangle's resizing by 
            // 0.1 inches per second every second.
            // (0.1 inches * 96 pixels per inch / (1000ms^2)
            e.ExpansionBehavior.DesiredDeceleration = 0.1 * 96 / (1000.0 * 1000.0);

            // Decrease the velocity of the Rectangle's rotation rate by 
            // 2 rotations per second every second.
            // (2 * 360 degrees / (1000ms^2)
            e.RotationBehavior.DesiredDeceleration = 720 / (1000.0 * 1000.0);

            e.Handled = true;
        }

        private void PerformTrackingCapture_Click(object sender, RoutedEventArgs e)
        {
            WpfCsSample.CodeSampleControls.TrackingHandler.TrackingHandlerMatControl thmControl = new WpfCsSample.CodeSampleControls.TrackingHandler.TrackingHandlerMatControl();
            thmControl.PerformCapture_Click(sender, e);
        }

        private void PerformOCRCapture_Click(object sender, RoutedEventArgs e)
        {
            WpfCsSample.CodeSampleControls.OCR.OCRMatControl ocrControl = new WpfCsSample.CodeSampleControls.OCR.OCRMatControl();
            ocrControl.DebugTextBox = this.DebugTextBox;
            ocrControl.PerformCapture_Click(sender, e);
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
