using Wss.FoundationCore.Attributes;
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
using System.Windows.Shapes;

namespace Common.Package
{
    /// <summary>
    /// frmBackwaterValve.xaml 的交互逻辑
    /// </summary>
    public partial class frmPump : Window
    {
        private Guid _controlID;


        public frmPump(string str)
        {
            InitializeComponent();
            lbHeader.Content = str;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void textBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        public void ShowPointValue(List<ISerializeProperty> list)
        {

        }
    }
}
