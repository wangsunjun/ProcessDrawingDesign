using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wss.Foundation.Designer.Resources
{
    /// <summary>
    /// DragAdornerThumb.xaml 的交互逻辑
    /// </summary>
    public partial class DragAdornerThumb : UserControl
    {
        public DragAdornerThumb()
        {
            InitializeComponent();
        }

        protected void OnThumbUpdated(object sender, EventArgs e)
        {
            var handle = ThumbUpdated;
            if (handle != null)
            {
                handle(this, new EventArgs());
            }
        }

        public event EventHandler ThumbUpdated;

        public bool IsDragging
        {
            get { return Drag.IsDragging; }
        }
    }
}
