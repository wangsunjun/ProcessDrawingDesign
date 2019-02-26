using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Wss.Foundation.Controls;
using Wss.Foundation.Designer.Command;

namespace Wss.Foundation.Designer
{
    /// <summary>
    ///     拖动控件,用于支持图元的拖动
    /// </summary>
    public class DragThumb :SelectionThumb
    {
        private Dictionary<DesignerContainer, Point> dragdic;
        //private double m_StartLeft, m_StartTop;
        private double maxLeft, maxTop;
        private Dictionary<DesignerContainer, Point> newdragdic;
        //DragDeltaEventArgs m_e;
        public List<double> oldleft;
        public List<double> oldtop;

        /// <summary>
        /// 获取包含指定图元的矩形外框
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private static Rect GetBoundingRectangle(IList<DesignerContainer> items)
        {
            var x1 = Double.MaxValue;
            var y1 = Double.MaxValue;
            var x2 = Double.MinValue;
            var y2 = Double.MinValue;

            foreach (var item in items)
            {
                x1 = Math.Min(Canvas.GetLeft(item), x1);
                y1 = Math.Min(Canvas.GetTop(item), y1);

                x2 = Math.Max(Canvas.GetLeft(item) + item.Width, x2);
                y2 = Math.Max(Canvas.GetTop(item) + item.Height, y2);
            }

            return new Rect(new Point(x1, y1), new Point(x2, y2));
        }

        protected override void OnDragCompleted(DragCompletedEventArgs e)
        {
            var designerItem = DataContext as DesignerContainer;
            if (designerItem == null) //@gw 20121126加入为空判断
                return;
            var designer = VisualTreeHelper.GetParent(designerItem) as DesignerCanvas;
            if (designer == null) //@gw 20121126加入为空判断
                return;
            var designerItems = designer.SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>();
            //循环选中的图元，设置其新坐标；
            foreach (var item in designerItems
                .Where(item => newdragdic != null)
                .Where(
                    item =>
                        !newdragdic.ContainsKey(item) &&
                        !newdragdic.ContainsValue(new Point(Canvas.GetLeft(item), Canvas.GetTop(item)))))
            {
                newdragdic.Add(item, new Point(Canvas.GetLeft(item), Canvas.GetTop(item)));
            }
            if (dragdic != null)
            {
                CommandDragThumb.Run(designer.CommManager, dragdic, newdragdic);
            }

            #region
            //if (designerItem.Item is PolylineCtl)
            //{
            //    //更新折线的节点
            //    UpdatePointDrager(designerItem.Item);
            //}

            //sqh修改
            //CommandDragThumb.Run(designer.CommManager, m_e, designerItem, this);

            //OptStack.SaveOpt(designer);
            #endregion
        }

        protected override void OnDragDelta(DragDeltaEventArgs e)
        {
            if (Designer == null) return;

            #region  //@gw 20121126将锁屏功能代码移入if判断内

            var items = Designer.SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>().ToList();
            foreach (
                 var di in items.Where(di => di.LockDesign))
            {
                Designer.SelectionService.RemoveFromSelection(di);
            }

            #endregion


            var rect = GetBoundingRectangle(items);
            //移动的最大区域
            var minLeft = double.IsNaN(rect.Left) ? 0 : rect.Left;
            var minTop = double.IsNaN(rect.Top) ? 0 : rect.Top;
            maxLeft = double.IsNaN(rect.Left) ? Designer.ActualWidth - rect.Width : Designer.ActualWidth - rect.Left - rect.Width;
            maxTop = double.IsNaN(rect.Top) ? Designer.ActualHeight - rect.Height : Designer.ActualHeight - rect.Top - rect.Height;

            var dragDelta = new Point(e.HorizontalChange, e.VerticalChange);

            //如果图元进行了旋转及翻转则对位移进行相应变换
            var tg = LayoutTransform as TransformGroup;
            if (tg != null)
            {
                foreach (var tf in tg.Children)
                {
                    var rotateTransform = tf as RotateTransform;
                    if (rotateTransform != null)
                    {
                        dragDelta = rotateTransform.Transform(dragDelta);
                    }
                    var ts = tf as ScaleTransform;
                    if (ts != null)
                    {
                        dragDelta = ts.Transform(dragDelta);
                    }
                }
            }

            //使用变换后的位移计算水平及垂直方向上的偏移
            var deltaHorizontal = dragDelta.X < 0 ? Math.Max(-minLeft, dragDelta.X) : Math.Min(maxLeft, dragDelta.X);
            var deltaVertical = dragDelta.Y < 0 ? Math.Max(-minTop, dragDelta.Y) : Math.Min(maxTop, dragDelta.Y);


            //循环选中的图元，设置其新坐标；
            foreach (var item in items)
            {
                var left = double.IsNaN(Canvas.GetLeft(item)) ? 0 : Canvas.GetLeft(item) + deltaHorizontal;
                var top = double.IsNaN(Canvas.GetTop(item)) ? 0 : Canvas.GetTop(item) + deltaVertical;
                if (left < 0)
                {
                    left = 0;
                }
                else if (left + item.Width > Designer.Width)
                {
                    left = Designer.Width - item.Width;
                }
                if (top < 0)
                {
                    top = 0;
                }
                else if (top + item.Height > Designer.Height)
                {
                    top = Designer.Height - item.Height;
                }

                Canvas.SetLeft(item, (int)left);
                Canvas.SetTop(item, (int)top);

                //Canvas.SetLeft(Item, left + deltaHorizontal + Item.Width > designer.Width ? designer.Width - Item.Width : left + deltaHorizontal);
                //Canvas.SetTop(Item, top + deltaVertical + Item.Height > designer.Height ? designer.Height - Item.Height : top + deltaVertical);
            }

            Designer.InvalidateMeasure();


            e.Handled = true;
        }

        protected override void OnDragStarted(DragStartedEventArgs e)
        {
            e.Handled = true;
            #region
            //var designerItem = DataContext as DesignerContainer;
            //var designer = VisualTreeHelper.GetParent(designerItem) as DesignerCanvas;
            //var designerItems = designer.SelectionService.CurrentSelection.OfType<DesignerContainer>();
            //var items = designerItems as IList<DesignerContainer> ?? designerItems.ToList();
            //var rect = GetBoundingRectangle(items);

            ////移动的对大区域
            //m_StartLeft = double.IsNaN(rect.Left) ? 0 : rect.Left;
            //m_StartTop = double.IsNaN(rect.Top) ? 0 : rect.Top;
            //if (!designer.Dragging)
            //{
            //    dragdic = new Dictionary<DesignerContainer, Point>();

            //    newdragdic = new Dictionary<DesignerContainer, Point>();
            //}
            ////循环选中的图元，设置其新坐标；
            //foreach (var item in items.Where(item => !dragdic.ContainsKey(item) &&
            //                                         !dragdic.ContainsValue(new Point(Canvas.GetLeft(item),
            //                                             Canvas.GetTop(item)))))
            //{
            //    dragdic.Add(item, new Point(Canvas.GetLeft(item), Canvas.GetTop(item)));
            //}
            #endregion
        }
    }
}