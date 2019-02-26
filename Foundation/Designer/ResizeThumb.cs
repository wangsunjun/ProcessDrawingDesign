using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer
{
    /// <summary>
    ///     控制图元缩放的控件
    /// </summary>
    public class ResizeThumb : SelectionThumb
    {
        private bool isUniform;
        public List<DesignerContainer> m_des;
        public DragDeltaEventArgs m_drage;

        //拖动左边
        private void DragLeft(double scale, DesignerContainer item, SelectionService selectionService)
        {
            ///改变图元大小
            var oldleft = Canvas.GetLeft(item);
            var oldwidth = item.Width;

            item.Width = (int) (oldwidth*scale); //计算更新后图元宽度
            var newscale = item.Width/oldwidth; //计算更新后图元宽度由于取整数导致变更比例变化
            var delta = (item.Width - oldwidth); //计算宽度差
            Canvas.SetLeft(item, oldleft - delta < 0 ? 0 : (oldleft - delta)); //设置新图元左侧位置

            ///遍历child图元，改变图元相对宽度与左侧位置
            EnumAndChangeGroupChild(item,  oldleft, (parent, child, oldparentleft) =>
            {
                var oldchildleft = Canvas.GetLeft(child);
                child.Width = (child.Width*newscale);
                var newchildleft = Canvas.GetLeft(parent) + (oldchildleft - oldparentleft)*newscale;
                Canvas.SetLeft(child, newchildleft);
                return oldchildleft;
            });
            //else
            //{
            //    double groupItemwidth = Item.Width;
            //    Item.Width = (Item.Width * scale);
            //    double delta = (Item.Width - groupItemwidth);
            //    double groupItemLeft = Canvas.GetLeft(Item);
            //    Canvas.SetLeft(Item, groupItemLeft - delta < 0 ? 0 : (groupItemLeft - delta));
            //}
        }
        //拖动右边
        private void DragRight(double scale, DesignerContainer item, SelectionService selectionService)
        {
            ///改变图元大小
            var oldright = Canvas.GetLeft(item);
            var oldwidth = item.Width;

            item.Width = (int) (oldwidth*scale); //计算更新后图元宽度
            var newscale = item.Width/oldwidth; //计算更新后图元宽度由于取整数导致变更比例变化
            var delta = (item.Width - oldwidth); //计算宽度差
            //Canvas.SetLeft(Item, oldright - delta < 0 ? 0 : (oldright - delta));//设置新图元左侧位置

            ///遍历child图元，改变图元相对宽度与左侧位置
            EnumAndChangeGroupChild(item,  oldright, (parent, child, oldparentleft) =>
            {
                var oldchildright = Canvas.GetLeft(child);
                child.Width = (child.Width*newscale);
                var newchildright = Canvas.GetLeft(parent) + (oldchildright - oldparentleft)*newscale;
                Canvas.SetLeft(child, newchildright);
                return oldchildright;
            });
            //else
            //{
            //    double groupItemwidth = Item.Width;
            //    Item.Width = (Item.Width * scale);
            //    double delta = (Item.Width - groupItemwidth);
            //    double groupItemLeft = Canvas.GetLeft(Item);
            //    Canvas.SetLeft(Item, groupItemLeft - delta < 0 ? 0 : (groupItemLeft - delta));
            //}
        }
        //拖动上面
        private void DragTop(double scale, DesignerContainer item, SelectionService selectionService)
        {
            ///改变图元大小
            var oldtop = Canvas.GetTop(item);
            var oldheight = item.Height;

            item.Height = (int) (oldheight*scale); //计算更新后图元宽度
            var newscale = item.Height/oldheight; //计算更新后图元宽度由于取整数导致变更比例变化
            var delta = (item.Height - oldheight); //计算宽度差
            Canvas.SetTop(item, oldtop - delta < 0 ? 0 : (oldtop - delta)); //设置新图元左侧位置

            ///遍历child图元，改变图元相对宽度与左侧位置
            EnumAndChangeGroupChild(item,  oldtop, (parent, child, oldparenttop) =>
            {
                var oldchildtop = Canvas.GetTop(child);
                child.Height = (child.Height*newscale);
                var newchildtop = Canvas.GetTop(parent) + (oldchildtop - oldparenttop)*newscale;
                Canvas.SetTop(child, newchildtop);
                return oldchildtop;
            });
            //else
            //{
            //    double groupItemwidth = Item.Width;
            //    Item.Width = (Item.Width * scale);
            //    double delta = (Item.Width - groupItemwidth);
            //    double groupItemLeft = Canvas.GetLeft(Item);
            //    Canvas.SetLeft(Item, groupItemLeft - delta < 0 ? 0 : (groupItemLeft - delta));
            //}
        }
        //拖动下面
        private void DragBottom(double scale, DesignerContainer item, SelectionService selectionService)
        {
            
            ///改变图元大小
            var oldbottom = Canvas.GetTop(item);
            var oldheigth = item.Height;

            item.Height = (int) (oldheigth*scale); //计算更新后图元宽度
            var newscale = item.Height/oldheigth; //计算更新后图元宽度由于取整数导致变更比例变化
            var delta = (item.Height - oldheigth); //计算宽度差
            //Canvas.SetTop(Item, oldleft - delta < 0 ? 0 : (oldleft - delta));//设置新图元左侧位置
            Console.WriteLine(scale+"-"+oldheigth+"-"+item.Height+"-"+newscale+"-"+delta);
            ///遍历child图元，改变图元相对宽度与左侧位置
            EnumAndChangeGroupChild(item,  oldbottom, (parent, child, oldparentleft) =>
            {
                var oldchildbottom = Canvas.GetTop(child);
                child.Height = (child.Height*newscale);
                var newchildbottom = Canvas.GetTop(parent) + (oldchildbottom - oldparentleft)*newscale;
                Canvas.SetTop(child, newchildbottom);
                return oldchildbottom;
            });
            //else
            //{
            //    double groupItemwidth = Item.Width;
            //    Item.Width = (Item.Width * scale);
            //    double delta = (Item.Width - groupItemwidth);
            //    double groupItemLeft = Canvas.GetLeft(Item);
            //    Canvas.SetLeft(Item, groupItemLeft - delta < 0 ? 0 : (groupItemLeft - delta));
            //}
        }
        private static void EnumAndChangeGroupChild(DesignerContainer parent, double oldsize, Func<DesignerContainer, DesignerContainer, double, double> changeact)
        {
            if (parent.IsGroup && parent.ParentCanvas.SelectionService != null) //如果是图元组
            {
               
                ///遍历child图元，改变图元相对大小
                var childs = parent.ParentCanvas.SelectionService.GetGroupChilds(parent).Cast<DesignerContainer>();
                foreach (var child in childs)
                {
                    var chlidoldsize = changeact(parent, child, oldsize);
                    EnumAndChangeGroupChild(child, chlidoldsize, changeact);
                }
            }
        }
        //计算缩放时的范围限制
        public void CalculateDragLimits(IEnumerable<DesignerContainer> selectedItems, out double minLeft, out double minTop, out double minDeltaHorizontal, out double minDeltaVertical)
        {
            minLeft = double.MaxValue;
            minTop = double.MaxValue;
            minDeltaHorizontal = double.MaxValue;
            minDeltaVertical = double.MaxValue;

            foreach (var item in selectedItems)
            {
                var left = Canvas.GetLeft(item);
                var top = Canvas.GetTop(item);

                minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
                minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);

                minDeltaVertical = Math.Min(minDeltaVertical, item.ActualHeight - item.Item.MinHeight);
                minDeltaHorizontal = Math.Min(minDeltaHorizontal, item.ActualWidth - item.Item.MinWidth);
            }
        }
        //获取包括选定图元的矩形外框
        public static Rect GetBoundingRectangle(IEnumerable<FrameworkElement> items)
        {
            var elements = items as IList<FrameworkElement> ?? items.ToList();
            if (!elements.Any())
            {
                return new Rect(0, 0, 0, 0);
            }
            var x1 = Double.MaxValue;
            var y1 = Double.MaxValue;
            var x2 = Double.MinValue;
            var y2 = Double.MinValue;

            foreach (var item in elements)
            {
                x1 = Math.Min(Canvas.GetLeft(item), x1);
                y1 = Math.Min(Canvas.GetTop(item), y1);

                x2 = Math.Max(Canvas.GetLeft(item) + item.Width, x2);
                y2 = Math.Max(Canvas.GetTop(item) + item.Height, y2);
            }

            return new Rect(new Point(x1, y1), new Point(x2, y2));
        }
        //等比缩放
        public void UniformScale(DesignerContainer item, double scale, DragDeltaEventArgs e)
        {
            if (VerticalAlignment == VerticalAlignment.Top && HorizontalAlignment == HorizontalAlignment.Left)
            {
                if (Canvas.GetLeft(item) + e.VerticalChange < 0 || Canvas.GetTop(item) + e.VerticalChange < 0)
                {
                    scale = 1;
                }
                DragTop(scale, item, Designer.SelectionService);
                DragLeft(scale, item, Designer.SelectionService);
            }
            else if (VerticalAlignment == VerticalAlignment.Top && HorizontalAlignment == HorizontalAlignment.Right)
            {
                if (Canvas.GetLeft(item) + item.Width - e.VerticalChange > Designer.ActualWidth ||
                    Canvas.GetTop(item) + e.VerticalChange < 0)
                {
                    scale = 1;
                }
                DragTop(scale, item, Designer.SelectionService);
                DragRight(scale, item, Designer.SelectionService);
            }
            else if (VerticalAlignment == VerticalAlignment.Bottom && HorizontalAlignment == HorizontalAlignment.Left)
            {
                if (Canvas.GetLeft(item) - e.VerticalChange < 0 ||
                    Canvas.GetTop(item) + item.Height + e.VerticalChange > Designer.ActualHeight)
                {
                    scale = 1;
                }
                DragBottom(scale, item, Designer.SelectionService);
                DragLeft(scale, item, Designer.SelectionService);
            }
            else if (VerticalAlignment == VerticalAlignment.Bottom && HorizontalAlignment == HorizontalAlignment.Right)
            {
                if (Canvas.GetLeft(item) + item.Width + e.VerticalChange > Designer.ActualWidth ||
                    Canvas.GetTop(item) + item.Height + e.VerticalChange > Designer.ActualHeight)
                {
                    scale = 1;
                }
                DragBottom(scale, item, Designer.SelectionService);
                DragRight(scale, item, Designer.SelectionService);
            }
        }
        protected override void OnDragCompleted(DragCompletedEventArgs e)
        {
            //isUniform = false;
            //if (designerItem.LockDesign)
            //{
            //    return;
            //}
            //Designer = VisualTreeHelper.GetParent(designerItem) as DesignerCanvas;


            //CommandResizeThumb.Run(Designer.CommManager, m_drage, Designer, designerItem, this);
            //// OptStack.SaveOpt(Designer);
        }
        protected override void OnDragDelta(DragDeltaEventArgs e)
        {
            m_drage = e;
            var va = VerticalAlignment;
            var ha = HorizontalAlignment;
            if (Designer != null && Designer.SelectionService.SelectedDesignerContainer.Count > 0)
            {
                var selectContainers = Designer.SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>().ToList();
                if (selectContainers.Any(DesignerContainer.GetDisabledHeight))
                {
                    va = VerticalAlignment.Center;
                }
                if (selectContainers.Any(DesignerContainer.GetDisabledWidth))
                {
                    ha = HorizontalAlignment.Center;
                }
                var rect = GetBoundingRectangle(selectContainers);
                Console.WriteLine(rect.Height + "-" + e.VerticalChange);
                var maxDeltaLeft = double.IsNaN(rect.Left) ? 0 : rect.Left;
                var maxDeltaTop = double.IsNaN(rect.Top) ? 0 : rect.Top;
                var maxDeltaRight = double.IsNaN(rect.Left)
                    ? Designer.Width - rect.Width
                    : Designer.Width - rect.Left - rect.Width;
                var maxDeltaBottom = double.IsNaN(rect.Top)
                    ? Designer.Height - rect.Height
                    : Designer.Height - rect.Top - rect.Height;
                double maxDeltaHorizontal, maxDeltaVertical;

                maxDeltaHorizontal = double.MaxValue;
                maxDeltaVertical = double.MaxValue;

                foreach (var item in selectContainers)
                {
                    if (item.IsGroup)
                    {
                        EnumAndChangeGroupChild(item, 0, (parent, child, oldparenttop) =>
                        {
                            maxDeltaVertical = Math.Min(maxDeltaVertical,
                                rect.Height * (1 - child.Item.MinHeight / child.ActualHeight));
                            maxDeltaHorizontal = Math.Min(maxDeltaHorizontal,
                                rect.Width * (1 - child.Item.MinWidth / child.ActualWidth));
                            return 0;
                        });
                    }
                    else
                    {
                        maxDeltaVertical = Math.Min(maxDeltaVertical,
                      rect.Height * (1 - item.Item.MinHeight / item.ActualHeight));
                        maxDeltaHorizontal = Math.Min(maxDeltaHorizontal,
                            rect.Width * (1 - item.Item.MinWidth / item.ActualWidth));
                    }
                }
                double dragDeltaVertical;
                double scalev;
                switch (va)
                {
                    case VerticalAlignment.Bottom:

                        if (e.VerticalChange > maxDeltaBottom)
                        {
                            break;
                        }
                        dragDeltaVertical = Math.Min(-e.VerticalChange, maxDeltaVertical);
                        scalev = (rect.Height - dragDeltaVertical) / rect.Height;
                        if (isUniform)
                        {
                            foreach (var item in selectContainers)
                            {
                                UniformScale(item, scalev, e);
                            }
                        }
                        else
                        {
                            foreach (var item in selectContainers)
                            {
                                DragBottom(scalev, item, Designer.SelectionService);
                            }

                        }
                        break;
                    case VerticalAlignment.Top:
                        if (e.VerticalChange > maxDeltaBottom)
                        {
                            break;
                        }
                        dragDeltaVertical = Math.Min(Math.Max(-maxDeltaTop, e.VerticalChange), maxDeltaVertical);
                        scalev = (rect.Height - dragDeltaVertical) / rect.Height;
                        if (isUniform)
                        {
                            foreach (var item in selectContainers)
                            {
                                UniformScale(item, scalev, e);
                            }
                        }
                        else
                        {
                            foreach (var item in selectContainers)
                            {
                                DragTop(scalev, item, Designer.SelectionService);
                            }

                        }
                        break;
                }
                double dragDeltaHorizontal;
                double scaleh;
                switch (ha)
                {
                    case HorizontalAlignment.Left:
                        dragDeltaHorizontal = Math.Min(Math.Max(-maxDeltaLeft, e.HorizontalChange),
                            maxDeltaHorizontal);
                        scaleh = (rect.Width - dragDeltaHorizontal) / rect.Width;

                        if (!isUniform)
                        {
                            foreach (var item in selectContainers)
                            {
                                DragLeft(scaleh, item, Designer.SelectionService);
                            }
                        }
                        break;
                    case HorizontalAlignment.Right:

                        if (e.HorizontalChange > maxDeltaRight)
                        {
                            break;
                        }
                        dragDeltaHorizontal = Math.Min(-e.HorizontalChange, maxDeltaHorizontal);
                        scaleh = (rect.Width - dragDeltaHorizontal) / rect.Width;


                        if (!isUniform)
                        {
                            foreach (var item in selectContainers)
                            {
                                DragRight(scaleh, item, Designer.SelectionService);
                            }
                        }
                        break;
                    default:
                        break;
                }


                e.Handled = true;
            }
           
        }
        protected override void OnDragStarted(DragStartedEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None)
            {
                isUniform = true;
            }
        }
    }
}