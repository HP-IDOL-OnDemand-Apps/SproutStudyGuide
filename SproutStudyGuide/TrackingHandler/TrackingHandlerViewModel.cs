using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfCsSample.CodeSampleControls.TrackingHandler
{
    public class TrackingHandlerViewModel : INotifyPropertyChanged
    {
        private TrackingHandlerDataObject _root;
        private readonly ObservableCollection<TrackingHandlerDataObject> _itemCollection = new ObservableCollection<TrackingHandlerDataObject>();
        private double _scaleFactor = 0.5;
        private bool _canCapture = true;
        

        public bool CanCapture
        {
            get { return _canCapture; }
            set
            {
                if (_canCapture != value)
                {
                    _canCapture = value;
                    OnPropertyChanged();
                }
            }
        }

        public TrackingHandlerDataObject Root
        {
            get { return _root; }
            set
            {
                if (Equals(value, _root)) return;
                _root = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TrackingHandlerDataObject> ItemCollection
        {
            get { return _itemCollection; }
        }

        
        public double ScaleFactor
        {
            get { return _scaleFactor; }
            set
            {
                if (value.Equals(_scaleFactor)) return;
                _scaleFactor = value;
                OnPropertyChanged();
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
