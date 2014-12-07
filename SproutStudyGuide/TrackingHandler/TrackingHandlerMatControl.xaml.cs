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

namespace WpfCsSample.CodeSampleControls.TrackingHandler
{
    /// <summary>
    /// Interaction logic for TrackingHandlerMatControl.xaml
    /// </summary>
    public partial class TrackingHandlerMatControl : UserControl
    {
        public TrackingHandlerMatControl()
        {
            DataContext = new TrackingHandlerViewModel();
            InitializeComponent();
        }

        public new TrackingHandlerViewModel DataContext
        {
            get { return base.DataContext as TrackingHandlerViewModel; }
            set { base.DataContext = value; }
        }       
        
    }
}
