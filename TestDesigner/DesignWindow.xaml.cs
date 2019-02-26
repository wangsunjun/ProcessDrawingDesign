using Wss.Foundation.Designer;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace TestDesigner
{
    /// <summary>
    /// DesignWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DesignWindow : Window
    {
        private ModeChangeWindow modechange;

        public DesignWindow()
        {
            InitializeComponent();
            this.Loaded += DesignWindow_Loaded;
        }

        private void DesignWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //string path = Directory.GetCurrentDirectory() + "/CustomDiagram/test.xml";
            //if (File.Exists(path))
            //{
            //    designerArea.LoadFileToCanvas("实时工况", path);
            //}

            modechange = new ModeChangeWindow();
            modechange.Topmost = true;
            modechange.ShowInTaskbar = false;
            modechange.ResizeMode = ResizeMode.NoResize;
            modechange.MouseDown += modechange_MouseDown;
            modechange.Owner = this;
            modechange.Left = Left + Width - 100;
            modechange.Top = Top + 80;
            modechange.Show();
            modechange.SetBinding(ModeChangeWindow.ModeProperty, new Binding("DesignMode")
            {
                Source = designerArea,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            var exp1 = new Expander { Header = "实体" };
            designerArea.Toolbox.Children.Add(exp1);
            var tboxTemp = new Toolbox { Name = "实体" };
            exp1.Content = tboxTemp;
            exp1.IsExpanded = true;
        }

        private void modechange_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((ModeChangeWindow)sender).DragMove();
        }


    }
}
