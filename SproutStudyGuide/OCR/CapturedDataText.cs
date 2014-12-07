using System.Windows;

namespace WpfCsSample.CodeSampleControls.OCR
{
    public class CapturedDataText
    {
        private readonly string _text;
        private readonly Rect _boundingBox;

        public CapturedDataText(string text, Rect box)
        {
            _text = text;
            _boundingBox = box;
        }

        public string Text
        {
            get { return _text; }
        }

        public Rect Box
        {
            get { return _boundingBox; }
        }
    }
}
