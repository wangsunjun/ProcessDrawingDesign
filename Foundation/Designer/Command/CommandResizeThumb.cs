using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     缩放  子类
    /// </summary>
    public class CommandResizeThumb : Command
    {
        private readonly bool flag = false;
        private DesignerCanvas m_dc;
        private List<DesignerContainer> m_des;
        private DesignerContainer m_di;
        private DragDeltaEventArgs m_drage;
        private List<double> m_height;
        private List<double> m_left;
        private List<double> m_pointy;
        private ResizeThumb m_rt;
        private List<double> m_width;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            if (param.Length > 3)
            {
                m_drage = param[0] as DragDeltaEventArgs;
                m_dc = param[1] as DesignerCanvas;
                m_di = param[2] as DesignerContainer;
                m_rt = param[3] as ResizeThumb;
                // m_drage = param[3] as DragDeltaEventArgs;
                if (m_di != null && m_dc != null && m_di.IsSelected)
                {
                    double scale = 1;

                    var selectedDesignerContainers = m_dc.SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>();

                    var designerContainers = selectedDesignerContainers as IList<DesignerContainer> ??
                                             selectedDesignerContainers.ToList();
                    if (m_rt != null)
                    {
                        double minDeltaHorizontal;
                        double minDeltaVertical;
                        double minLeft;
                        double minTop;
                        m_rt.CalculateDragLimits(designerContainers, out minLeft, out minTop,
                            out minDeltaHorizontal, out minDeltaVertical);

                        var rect = GetBoundingRectangle(designerContainers);


                        var bd = m_dc.Parent as Viewbox;
                        var maxLeft = double.IsNaN(rect.Left)
                            ? m_dc.Width - rect.Width
                            : m_dc.Width - rect.Left - rect.Width;
                        var maxTop = double.IsNaN(rect.Top)
                            ? m_dc.Height - rect.Height
                            : m_dc.Height - rect.Top - rect.Height;

                        foreach (var item in designerContainers)
                        {
                            if (m_width == null)
                            {
                                m_width = new List<double>();
                            }
                            m_width.Add(item.Width);
                            if (m_height == null)
                            {
                                m_height = new List<double>();
                            }
                            m_height.Add(item.Height);
                            if (m_des == null)
                            {
                                m_des = new List<DesignerContainer>();
                            }
                            m_des.Add(item);

                            var disabledH = DesignerContainer.GetDisabledHeight(m_di);
                            var disabledW = DesignerContainer.GetDisabledWidth(m_di);
                            if (item != null && item.ParentID == Guid.Empty)
                            {
                                //if (isUniform)
                                //{
                                //    UniformScale(Item, scale);
                                //    break;
                                //}

                                double dragDeltaVertical;
                                switch (m_rt.VerticalAlignment)
                                {
                                    case VerticalAlignment.Bottom:
                                        if (disabledH)
                                        {
                                            break;
                                        }
                                        var bottom = Canvas.GetTop(item);
                                        if (m_pointy == null)
                                        {
                                            m_pointy = new List<double>();
                                        }
                                        m_pointy.Add(bottom);
                                        if (m_drage != null)
                                        {
                                            if (m_drage.VerticalChange > maxTop)
                                            {
                                                dragDeltaVertical = 0;
                                                scale = 1;
                                            }
                                        }
                                        else
                                        {
                                            dragDeltaVertical = Math.Min(m_drage != null ? -m_drage.VerticalChange : 0,
                                                minDeltaVertical);
                                            scale = (item.ActualHeight - dragDeltaVertical)/item.ActualHeight;
                                        }
                                        if (flag)
                                        {
                                            m_rt.UniformScale(item, scale, m_drage);
                                        }
                                        else
                                        {
                                            DragBottom(scale, item, m_dc.SelectionService);
                                        }
                                        break;
                                    case VerticalAlignment.Top:
                                        if (disabledH)
                                        {
                                            break;
                                        }
                                        var top = Canvas.GetTop(item);
                                        if (m_pointy == null)
                                        {
                                            m_pointy = new List<double>();
                                        }
                                        m_pointy.Add(top);

                                        dragDeltaVertical =
                                            Math.Min(Math.Max(-minTop, m_drage != null ? m_drage.VerticalChange : 0),
                                                minDeltaVertical);

                                        scale = (item.ActualHeight - dragDeltaVertical)/item.ActualHeight;

                                        if (flag)
                                        {
                                            m_rt.UniformScale(item, scale, m_drage);
                                        }
                                        else
                                        {
                                            DragTop(scale, item, m_dc.SelectionService);
                                        }
                                        break;
                                    default:
                                        break;
                                }

                                double dragDeltaHorizontal;
                                switch (m_rt.HorizontalAlignment)
                                {
                                    case HorizontalAlignment.Left:
                                        if (disabledW)
                                        {
                                            break;
                                        }
                                        var left = Canvas.GetLeft(item);
                                        if (m_left == null)
                                        {
                                            m_left = new List<double>();
                                        }
                                        m_left.Add(left);
                                        dragDeltaHorizontal =
                                            Math.Min(
                                                Math.Max(-minLeft, m_drage != null ? m_drage.HorizontalChange : 0),
                                                minDeltaHorizontal);
                                        scale = (item.ActualWidth - dragDeltaHorizontal)/item.ActualWidth;
                                        if (!flag)
                                        {
                                            DragLeft(scale, item, m_dc.SelectionService);
                                        }
                                        break;
                                    case HorizontalAlignment.Right:
                                        if (disabledW)
                                        {
                                            break;
                                        }
                                        var left1 = Canvas.GetLeft(item);
                                        if (m_left == null)
                                        {
                                            m_left = new List<double>();
                                        }
                                        m_left.Add(left1);
                                        if (m_drage != null)
                                        {
                                            if (m_drage.HorizontalChange > maxLeft)
                                            {
                                                dragDeltaHorizontal = 0;
                                                scale = 1;
                                            }
                                            else
                                            {
                                                dragDeltaHorizontal =
                                                    Math.Min(m_drage != null ? -m_drage.HorizontalChange : 0,
                                                        minDeltaHorizontal);
                                                scale = (item.ActualWidth - dragDeltaHorizontal)/item.ActualWidth;
                                            }
                                        }
                                        if (!flag)
                                        {
                                            DragRight(scale, item, m_dc.SelectionService);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    if (m_drage != null)
                    {
                        m_drage.Handled = true;
                    }
                }
                return true;
            }
            return false;
        }

        public Rect GetBoundingRectangle(IEnumerable<DesignerContainer> items)
        {
            var x1 = Double.MaxValue;
            var y1 = Double.MaxValue;
            var x2 = Double.MinValue;
            var y2 = Double.MinValue;

            foreach (var item in items)
            {
                x1 = Math.Min(Canvas.GetLeft(item), x1);
                y1 = Math.Min(Canvas.GetTop(item), y1);


                x2 = Math.Max(Canvas.GetLeft(item) + item.ActualWidth, x2);
                y2 = Math.Max(Canvas.GetTop(item) + item.ActualHeight, y2);
            }

            return new Rect(new Point(x1, y1), new Point(x2, y2));
        }

        public override bool UnRun(CommandManager mgr)
        {
            //DesignerContainer di;
            //if (m_rt.m_des != null)
            //{
            //    for (var i = 0; i < m_rt.m_des.Count; i++)
            //    {
            //        di = m_rt.m_des[i];
            //        if (di != null)
            //        {
            //            di.Height = m_rt.m_height[i];
            //            di.Width = m_rt.m_width[i];
            //            if (m_pointy != null)
            //            {
            //                Canvas.SetTop(di, m_rt.m_pointy[i]);
            //            }
            //            if (m_left != null)
            //            {
            //                Canvas.SetLeft(di, m_rt.m_left[i]);
            //            }
            //        }
            //    }
            //    return true;
            //}
            return false;
        }

        public override bool ReRun(CommandManager mgr)
        {
            DesignerContainer di;
            if (m_rt.m_des != null)
            {
                for (var i = 0; i < m_des.Count; i++)
                {
                    di = m_des[i];
                    if (di != null)
                    {
                        di.Height = m_height[i];
                        di.Width = m_width[i];
                        if (m_pointy != null)
                        {
                            Canvas.SetTop(di, m_pointy[i]);
                        }
                        if (m_left != null)
                        {
                            Canvas.SetLeft(di, m_left[i]);
                        }
                    }
                }
                return true;
            }
            return false;
            // m_drage = param[3] as DragDeltaEventArgs;
            //    if (m_di != null && m_dc != null && m_di.IsSelected)
            //    {
            //        double minLeft, minTop, minDeltaHorizontal, minDeltaVertical;
            //        double dragDeltaVertical, dragDeltaHorizontal, scale = 1;

            //        IEnumerable<DesignerContainer> selectedDesignerContainers = m_dc.SelectionService.CurrentSelection.OfType<DesignerContainer>();

            //        m_rt.CalculateDragLimits(selectedDesignerContainers, out minLeft, out minTop,
            //                            out minDeltaHorizontal, out minDeltaVertical);

            //        Rect rect = GetBoundingRectangle(selectedDesignerContainers);
            //        //if (m_width == null)
            //        //{
            //        //    m_width = new List<double>();
            //        //}
            //        //m_width.Add(rect.Width);
            //        //if (m_height == null)
            //        //{
            //        //    m_height = new List<double>();
            //        //}
            //        //m_height.Add(rect.Height);

            //        Viewbox bd = m_dc.Parent as Viewbox;
            //        double maxLeft = double.IsNaN(rect.Left) ? m_dc.Width - rect.Width : m_dc.Width - rect.Left - rect.Width;
            //        double maxTop = double.IsNaN(rect.Top) ? m_dc.Height - rect.Height : m_dc.Height - rect.Top - rect.Height;

            //        foreach (DesignerContainer Item in selectedDesignerContainers)
            //        {

            //            if (m_des == null)
            //            {
            //                m_des = new List<DesignerContainer>();
            //            }
            //            m_des.Add(Item);
            //            bool disabledH = (!double.IsNaN(m_di.Item.MinHeight) &&
            //                            m_di.Item.MinHeight != 0 &&
            //                            m_di.Item.MaxHeight == m_di.Item.MinHeight &&
            //                            m_di.Item.GetType() != typeof(DesignerContainer)) ||
            //                             (m_di.Content is ContentControl &&
            //                             (m_di.Content as ContentControl).MaxHeight != 0 &&
            //                             !double.IsInfinity((m_di.Content as ContentControl).MaxHeight) &&
            //                             (m_di.Content as ContentControl).MaxHeight == (m_di.Content as ContentControl).MinHeight);
            //            bool disabledW = (!double.IsNaN(m_di.Item.MinWidth) &&
            //                            m_di.Item.MinWidth != 0 &&
            //                            m_di.Item.MinWidth == m_di.Item.MaxWidth &&
            //                            m_di.Item.GetType() != typeof(DesignerContainer)) ||
            //                            (m_di.Content is ContentControl &&
            //                            (m_di.Content as ContentControl).MaxWidth != 0 &&
            //                            !double.IsInfinity((m_di.Content as ContentControl).MaxWidth) &&
            //                            (m_di.Content as ContentControl).MaxWidth == (m_di.Content as ContentControl).MinWidth);
            //            if (Item != null && Item.ParentID == Guid.Empty)
            //            {
            //                //if (isUniform)
            //                //{
            //                //    UniformScale(Item, scale);
            //                //    break;
            //                //}

            //                switch (this.m_rt.VerticalAlignment)
            //                {
            //                    case VerticalAlignment.Bottom:
            //                        if (disabledH)
            //                        {
            //                            break;
            //                        }
            //                        double bottom = Canvas.GetBottom(Item);

            //                        if (m_drage.VerticalChange > maxTop)
            //                        {
            //                            dragDeltaVertical = 0;
            //                            scale = 1;
            //                        }
            //                        else
            //                        {
            //                            dragDeltaVertical = Math.Min(-m_drage.VerticalChange, minDeltaVertical);
            //                            scale = (Item.ActualHeight - dragDeltaVertical) / Item.ActualHeight;
            //                        }
            //                        if (flag)
            //                        {
            //                            m_rt.UniformScale(Item, scale, m_drage);
            //                        }
            //                        else
            //                        {
            //                            DragBottom(scale, Item, m_dc.SelectionService);
            //                        }
            //                        break;
            //                    case VerticalAlignment.Top:
            //                        if (disabledH)
            //                        {
            //                            break;
            //                        }
            //                        double top = Canvas.GetTop(Item);
            //                        if (m_top == null)
            //                        {
            //                            m_top = new List<double>();
            //                        }
            //                        m_top.Add(top);
            //                        dragDeltaVertical = Math.Min(Math.Max(-minTop, m_drage.VerticalChange), minDeltaVertical);
            //                        scale = (Item.ActualHeight - dragDeltaVertical) / Item.ActualHeight;

            //                        if (flag)
            //                        {
            //                            m_rt.UniformScale(Item, scale, m_drage);
            //                        }
            //                        else
            //                        {
            //                            DragTop(scale, Item, m_dc.SelectionService);
            //                        }
            //                        break;
            //                    default:
            //                        break;
            //                }

            //                switch (this.m_rt.HorizontalAlignment)
            //                {
            //                    case HorizontalAlignment.Left:
            //                        if (disabledW)
            //                        {
            //                            break;
            //                        }
            //                        double left = Canvas.GetLeft(Item);
            //                        if (m_left == null)
            //                        {
            //                            m_left = new List<double>();

            //                        }
            //                        m_left.Add(left);
            //                        dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, m_drage.HorizontalChange), minDeltaHorizontal);
            //                        scale = (Item.ActualWidth - dragDeltaHorizontal) / Item.ActualWidth;
            //                        if (!flag)
            //                        {
            //                            DragLeft(scale, Item, m_dc.SelectionService);
            //                        }
            //                        break;
            //                    case HorizontalAlignment.Right:
            //                        if (disabledW)
            //                        {
            //                            break;
            //                        }
            //                        if (m_drage != null)
            //                        {
            //                            if (m_drage.HorizontalChange > maxLeft)
            //                            {
            //                                dragDeltaHorizontal = 0;
            //                                scale = 1;
            //                            }
            //                            else
            //                            {
            //                                dragDeltaHorizontal = Math.Min(-m_drage.HorizontalChange, minDeltaHorizontal);
            //                                scale = (Item.ActualWidth - dragDeltaHorizontal) / Item.ActualWidth;
            //                            }
            //                        }
            //                        if (!flag)
            //                        {
            //                            DragRight(scale, item, m_dc.SelectionService);
            //                        }
            //                        break;
            //                    default:
            //                        break;
            //                }
            //            }

            //        }
            //        m_drage.Handled = true;
            //        return true;
            //    }


            //return false;
        }

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
            EnumAndChangeGroupChild(item, selectionService, oldleft, (parent, child, oldparentleft) =>
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

        /// <summary>
        ///     拖动右边
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="item"></param>
        /// <param name="selectionService"></param>
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
            EnumAndChangeGroupChild(item, selectionService, oldright, (parent, child, oldparentleft) =>
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

        /// <summary>
        ///     拖动上面
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="item"></param>
        /// <param name="selectionService"></param>
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
            EnumAndChangeGroupChild(item, selectionService, oldtop, (parent, child, oldparenttop) =>
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

        private void DragTopUnRun(double scale, DesignerContainer item, SelectionService selectionService)
        {
            ///改变图元大小
            var oldtop = Canvas.GetTop(item);
            var oldheight = item.Height;

            item.Height = (int) (oldheight*scale); //计算更新后图元宽度
            var newscale = item.Height/oldheight; //计算更新后图元宽度由于取整数导致变更比例变化
            var delta = (item.Height - oldheight); //计算宽度差
            Canvas.SetTop(item, oldtop - delta < 0 ? 0 : (oldtop - delta)); //设置新图元左侧位置

            ///遍历child图元，改变图元相对宽度与左侧位置
            EnumAndChangeGroupChild(item, selectionService, oldtop, (parent, child, oldparenttop) =>
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

        /// <summary>
        ///     拖动下面
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="item"></param>
        /// <param name="selectionService"></param>
        private void DragBottom(double scale, DesignerContainer item, SelectionService selectionService)
        {
            ///改变图元大小
            var oldbottom = Canvas.GetTop(item);
            var oldheigth = item.Height;

            item.Height = (int) (oldheigth*scale); //计算更新后图元宽度
            var newscale = item.Height/oldheigth; //计算更新后图元宽度由于取整数导致变更比例变化
            var delta = (item.Height - oldheigth); //计算宽度差
            //Canvas.SetTop(Item, oldleft - delta < 0 ? 0 : (oldleft - delta));//设置新图元左侧位置

            ///遍历child图元，改变图元相对宽度与左侧位置
            EnumAndChangeGroupChild(item, selectionService, oldbottom, (parent, child, oldparentleft) =>
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

        private static void EnumAndChangeGroupChild(DesignerContainer parent, SelectionService selectionService,
            double oldsize, Func<DesignerContainer, DesignerContainer, double, double> changeact)
        {
            if (parent.IsGroup) //如果是图元组
            {
                ///遍历child图元，改变图元相对大小
                var childs = selectionService.GetGroupChilds(parent).Cast<DesignerContainer>();
                foreach (var child in childs)
                {
                    var chlidoldsize = changeact(parent, child, oldsize);
                    EnumAndChangeGroupChild(child, selectionService, chlidoldsize, changeact);
                }
            }
        }

        public static void Run(CommandManager cmgr, DragDeltaEventArgs m_drage, DesignerCanvas dc,
            DesignerContainer items, ResizeThumb rt)
        {
            cmgr.Run<CommandResizeThumb>(m_drage, dc, items, rt);
        }
    }
}