using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfCsSample.CodeSampleControls.OCR
{
    public class OCRViewModel: INotifyPropertyChanged
    {
        private CapturedDataImage _root;
        private readonly ObservableCollection<CapturedDataImage> _itemCollection = new ObservableCollection<CapturedDataImage>();
        public CapturedDataImage Root
        {
            get { return _root; }
            set
            {
                if (Equals(value, _root)) return;
                _root = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CapturedDataImage> ItemCollection
        {
            get { return _itemCollection; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

       

    }
}
