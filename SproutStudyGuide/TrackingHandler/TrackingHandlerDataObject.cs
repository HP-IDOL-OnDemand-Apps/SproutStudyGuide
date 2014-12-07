using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.CompilerServices;

namespace WpfCsSample.CodeSampleControls.TrackingHandler
{
    public class TrackingHandlerDataObject : INotifyPropertyChanged
    {
        private readonly BitmapSource _image;
        private readonly Geometry _outline;
        private readonly Rect _boundary;
        private string _name;
        private bool _isReadOnly;

        public TrackingHandlerDataObject(BitmapSource image, Geometry outline,
            Rect boundary, string name)
        {
            _image = image;
            _outline = outline;
            _boundary = boundary;
            _name = name;
            _isReadOnly = false;
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set 
            { 
                _isReadOnly = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public BitmapSource Image
        {
            get { return _image; }
        }

        public Geometry Outline
        {
            get { return _outline; }
        }

        public Rect Boundary
        {
            get { return _boundary; }
        }
        
        public Vector OutlineScale
        {
            get
            {
                return new Vector(Boundary.Width / Image.PixelWidth, Boundary.Height / Image.PixelHeight);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
