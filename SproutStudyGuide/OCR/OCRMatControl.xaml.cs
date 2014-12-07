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

namespace WpfCsSample.CodeSampleControls.OCR
{
    /// <summary>
    /// Interaction logic for OCRMatControl.xaml
    /// </summary>
    public partial class OCRMatControl : UserControl
    {
        public OCRMatControl()
        {
            DataContext = new OCRViewModel();
            InitializeComponent();
        }

        public new OCRViewModel DataContext
        {
            get { return base.DataContext as OCRViewModel; }
            set { base.DataContext = value; }
        }
    }
}
