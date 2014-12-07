using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using HP.PC.Presentation;

namespace WpfCsSample.CodeSampleControls.OCR
{
    // Interaction logic for OCRSample.xaml
    // This sample code demonstrates how to capture a moment and how to extract text
    // from the moment.
    partial class OCRMatControl: UserControl//, ICodeSampleControl
    {
        public TextBox DebugTextBox { get; set; }
        public int Order { get { return 11; } }
        public string Title { get { return "Text Extraction"; } }
        public string Description { get { return "This sample demonstrates how to extract text from a moment that captured a document. Place a document on the touch mat, and then click Capture to capture a moment and to extract text from the moment."; } }


        public async void PerformCapture_Click(object sender, RoutedEventArgs e)
        {
            //App.ClearErrorStatus();
            var horizontalWindow = new WpfCsSample.CodeSampleControls.TrackingHandler.HorizontalWindow();
            try
            {
                DataContext.Root = null;

                // The SDK link provides SDK functionality to the application. 
                using (var link = HPPC.CreateLink())
                {

                    // Before registering the window, be sure WPF has a handle to the Window.
                    // See: http://msdn.microsoft.com/en-us/library/system.windows.interop.windowinterophelper.ensurehandle%28v=vs.110%29.aspx
                    var windowInteropHelper = new WindowInteropHelper(horizontalWindow);
                    windowInteropHelper.EnsureHandle();

                    // Register the window for the mat.
                    using (var windowRegistration = await link.RegisterWindowAsync(horizontalWindow))
                    {
                        // Turning on the projector
                        await windowRegistration.DisplayAsync();

                        // Clear captured images/text from previous capture
                        DataContext.ItemCollection.Clear();

                         // Capture a moment. 
                        using (var moment = await link.CaptureMomentAsync())
                        {
                            // Not needed any more
                            horizontalWindow.Hide();

                            // Extract the top-level picture and child images.
                            var picture = await link.ExtractPictureAsync(moment);

                            // From the SDK link, use the AccessSpecificationAsync function to create
                            // an instance of the IPcSpecification interface.
                            var spec = await link.AccessSpecificationAsync();

                            // Use the IPcScreenSpecification interface through the IPcSpecification interface to
                            // retrieve information about the mat screen. The information is used below when displaying
                            // pictures and outlines.
                            var mat = spec.Screen.Mat;

                            var root = new CapturedDataImage(picture.Image, "",
                                picture.PhysicalBoundaries.ToScreenCoordinates(mat));

                            // Draw extracted picture
                            DataContext.Root = root;

                            // Extract the text from the moment.
                            var text = await link.ExtractTextAsync(moment);

                            // Navigate through the IPcText tree structure to get the text for each box that we want.
                            // The text should be on the third level, where there are no children left. To get all
                            // of the text for the root, or for a specific page, you should get the text for all of its children.
                            // Children are not ordered!
                            var rootText = new CapturedDataText(text.Text,
                                new Rect(text.PhysicalBoundaries.Location.X, text.PhysicalBoundaries.Location.Y,
                                    text.PhysicalBoundaries.Size.Width, text.PhysicalBoundaries.Size.Height));

                            List<CapturedDataText> secondLevel = new List<CapturedDataText>();
                            List<CapturedDataText> thirdLevel = new List<CapturedDataText>();
                            foreach (IPcText child in text.Children)
                            {
                                secondLevel.Add(new CapturedDataText(child.Text,
                                    child.PhysicalBoundaries.ToScreenCoordinates(mat)));

                                // Check each captured item for text.
                                foreach (IPcText gchild in child.Children)
                                {
                                    string leafText = gchild.Text;
                                    if (String.IsNullOrEmpty(leafText))
                                    {
                                        leafText = "No Text found in this box";
                                    }
                                    else if (String.IsNullOrWhiteSpace(leafText))
                                    {
                                        leafText = "Only white spaces were found in this box";
                                    }

                                    thirdLevel.Add(new CapturedDataText(leafText, gchild.PhysicalBoundaries.ToScreenCoordinates(mat)));
                                }
                            }

                            foreach (CapturedDataText results in thirdLevel)
                            {
                                CroppedBitmap croppedImage = new CroppedBitmap(picture.Image,
                                    new Int32Rect(
                                        (int) (results.Box.TopLeft.X/mat.PixelDensity.X*picture.PixelDensity.X),
                                        (int) (results.Box.TopLeft.Y/mat.PixelDensity.Y*picture.PixelDensity.Y),
                                        (int) (results.Box.Width/mat.PixelDensity.X*picture.PixelDensity.X),
                                        (int) (results.Box.Height/mat.PixelDensity.Y*picture.PixelDensity.Y)));

                                CapturedDataImage capText = new CapturedDataImage(croppedImage, results.Text,
                                    results.Box);

                                DataContext.ItemCollection.Add(capText);
                            }
                        }
                    }
                }

                DebugTextBox.Text = "";
                foreach (var item in DataContext.ItemCollection)
                {
                    DebugTextBox.Text += item.Text;
                }
            }
            catch (Exception sdkError)
            {
                //App.UpdateErrorStatus("There was a problem running this sample: " + sdkError.Message + ".");
            }
            finally
            {
                horizontalWindow.Close();
            }
        }
    }
}
