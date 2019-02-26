using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer
{
    /// <summary>
    /// 绘制虚线框的装饰层
    /// </summary>
    public class RubberbandAdorner : Adorner
    {
        private readonly DesignerCanvas designerCanvas;
        private readonly Pen rubberbandPen;
        private Point? endPoint;
        private Point? startPoint;

        public RubberbandAdorner(DesignerCanvas designerCanvas, Point? dragStartPoint) : base(designerCanvas)
        {
            this.designerCanvas = designerCanvas;
            startPoint = dragStartPoint;
            rubberbandPen = new Pen(Brushes.LightSlateGray, 1);
            rubberbandPen.DashStyle = new DashStyle(new double[] {2}, 1);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!IsMouseCaptured) CaptureMouse();
                endPoint = e.GetPosition(this);
                InvalidateVisual();
            }
            else
            {
                if (IsMouseCaptured) ReleaseMouseCapture();
            }

            e.Handled = true;
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            // release mouse capture
            if (IsMouseCaptured) ReleaseMouseCapture();

            // remove this adorner from adorner layer
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(designerCanvas);
            if (adornerLayer != null) adornerLayer.Remove(this);
            UpdateSelection();
            e.Handled = true;
        }
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // without a background the OnMouseMove event would not be fired !
            // Alternative: implement a Canvas as a child of this adorner, like
            // the ConnectionAdorner does.
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

            if (startPoint.HasValue && endPoint.HasValue)
            {
                Point temp = endPoint ?? new Point();
                temp.X = temp.X < 1 ? 1 : temp.X;
                temp.X = temp.X > designerCanvas.Width - 1 ? designerCanvas.Width - 1 : temp.X;
                temp.Y = temp.Y < 1 ? 1 : temp.Y;
                temp.Y = temp.Y > designerCanvas.Height - 1 ? designerCanvas.Height - 1 : temp.Y;
                endPoint = temp;
                dc.DrawRectangle(Brushes.Transparent, rubberbandPen, new Rect(startPoint.Value, endPoint.Value));
            }
        }
        //更新当前选择的图元的集合
        private void UpdateSelection()
        {
            designerCanvas.SelectionService.ClearSelection();

            if (startPoint != null && endPoint != null)
            {
                Rect rubberBand = new Rect(startPoint.Value, endPoint.Value);
                foreach (FrameworkElement item in designerCanvas.Children)
                {
                    DesignerContainer di = item as DesignerContainer;
                    if (di == null || !di.Editable)
                    {
                        continue;
                    }
                    //Rect itemRect = VisualTreeHelper.GetDescendantBounds(Item);
                    //Rect itemBounds = Item.TransformToAncestor(designerCanvas).TransformBounds(itemRect);
                    FrameworkElement content = di.Item;
                    if (content == null)
                    {
                        continue;
                    }
                    Rect itemRect = VisualTreeHelper.GetDescendantBounds(content);
                    Rect itemBounds = content.TransformToAncestor(designerCanvas).TransformBounds(itemRect);

                    if (!rubberBand.Contains(itemBounds)) continue;
                    //if (Item is Connection)
                    //    designerCanvas.SelectionService.AddToSelection(Item as ISelectable);
                    //else
                    //{
                    //DesignerItem di = Item as DesignerItem;
                    int idx = Panel.GetZIndex(di);
                    if (idx == int.MinValue) continue;
                    if (di.ParentID == Guid.Empty) designerCanvas.SelectionService.AddToSelection(di);
                    //}
                }
            }


            if (designerCanvas.SelectionService.SelectedDesignerContainer.Count > 0)
            {
                PropertyView pv = designerCanvas.FindName("pv") as PropertyView;
                if (pv != null) pv.ShowProView(false);
            }
        }
    }
}