using System.Windows;
using Wss.Foundation;

namespace TestDesigner
{
    /// <summary>
    ///     ModeChangeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ModeChangeWindow : Window
    {
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof (MODE), typeof (ModeChangeWindow));

        public ModeChangeWindow()
        {
            InitializeComponent();

            a1.Checked += a1_Checked;
            a2.Checked += a2_Checked;
            Mode = MODE.EDIT;
        }

        public MODE Mode
        {
            get { return (MODE) GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        private void a2_Checked(object sender, RoutedEventArgs e)
        {
            Mode = MODE.VIEW;
        }

        private void a1_Checked(object sender, RoutedEventArgs e)
        {
            Mode = MODE.EDIT;
        }
    }
}