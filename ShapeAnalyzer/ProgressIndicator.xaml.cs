using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace ShapeAnalyzer
{
    /// <summary>
    /// Interaction logic for ProgressIndicator.xaml
    /// </summary>
    public partial class ProgressIndicator : UserControl
    {
        public ProgressIndicator()
        {
            InitializeComponent();
            Visibility = Visibility.Hidden;
        }

        public bool Show
        {
            get => (bool)GetValue(ShowProperty);
            set => SetValue(ShowProperty, value);
        }

        public static readonly DependencyProperty ShowProperty =
            DependencyProperty.Register("Show", typeof(bool), typeof(ProgressIndicator), new PropertyMetadata(false, ShowChanged));

        private static void ShowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ProgressIndicator;
            var show = (bool)e.NewValue;

            control.Visibility = show ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
