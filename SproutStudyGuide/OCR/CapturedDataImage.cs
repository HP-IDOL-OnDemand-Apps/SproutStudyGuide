using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfCsSample.CodeSampleControls.OCR
{
    public class CapturedDataImage
    {
        private readonly BitmapSource _image;
        private string _text;
        private Rect _boundary;

        public CapturedDataImage(BitmapSource image, string text, Rect boundary)
        {
            _image = image;
            _text = text;
            _boundary = boundary;
        }

        public BitmapSource Image
        {
            get { return _image; }
        }

        public string Text
        {
            get { return _text; }
        }

        public Rect Boundary
        {
            get { return _boundary; }
        }
    }
}
