using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Wss.FoundationCore.Attributes;
using Wss.Foundation.Controls;
using Wss.FoundationCore.Controls;

namespace Wss.Foundation.Designer
{
    public class ToolboxViewItem : ContentControl
    {
        private Point? _dragStartPoint;
        static ToolboxViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata( typeof (ToolboxViewItem), new FrameworkPropertyMetadata(typeof (ToolboxViewItem)));
        }
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            _dragStartPoint = e.GetPosition(this);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed) _dragStartPoint = null;

            if (!_dragStartPoint.HasValue) return;
            var di = Content as DesignerItem;
            if (di != null)
            {
                var dcontainer = new DesignerContainer();
                dcontainer.Content = di;
                var panel = VisualTreeHelper.GetParent(this) as WrapPanel;

                var dataObject = new DragObject();
                dataObject.Xaml = DesignAttributeService.SerializToXElement(dcontainer).ToString();

                if (panel != null)
                {
                    // desired size for DesignerCanvas is the stretched Toolbox Item size
                    var scale = 1.0;
                    dataObject.DesiredSize = new Size(panel.ItemWidth * scale, panel.ItemHeight * scale);
                }

                DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
                #region 不用
                //var dcontainer = new DesignerContainer();
                //dcontainer.Content = item;

                //WrapPanel panel = VisualTreeHelper.GetParent(this) as WrapPanel;
                //DragObject dataObject = new DragObject();
                //dataObject.Xaml= System.Windows.Markup.XamlWriter.Save(item);//dataObject.Xaml = DesignAttributeService.SerializToXElement(di).ToString();
                //if (panel != null)
                //{
                //    var scale = 1.0;
                //    dataObject.DesiredSize = new Size(panel.ItemWidth*scale, panel.ItemHeight*scale);
                //}

                ////DragDrop.DoDragDrop(this, di, DragDropEffects.Copy);
                //DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
                #endregion
            }
            e.Handled = true;
        }
    }

    public class DragObject
    {
        public String Xaml { get; set; }
        public Size? DesiredSize { get; set; }
    }
}