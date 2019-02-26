using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    public class CommandGroupOperate
    {
        private static double m_oldLeft;
        private static double m_oldTop;

        public static DesignerContainer Group(DesignerCanvas dc, IEnumerable<DesignerContainer> items)
        {
            if (items != null && dc != null)
            {
                var rect = GetBoundingRectangle(items);
                var groupItem = new DesignerContainer();
                groupItem.IsGroup = true;
                groupItem.Width = rect.Width;
                groupItem.Height = rect.Height;
                m_oldLeft = rect.Left;
                m_oldTop = rect.Top;
                Canvas.SetLeft(groupItem, rect.Left);
                Canvas.SetTop(groupItem, rect.Top);
                var groupCanvas = new Canvas();

                groupItem.Content = groupCanvas;
                Panel.SetZIndex(groupItem, dc.Children.Count);
                dc.Add(groupItem);

                foreach (var item in items)
                {
                    item.ParentID = groupItem.ID;
                }
                dc.SelectionService.SelectItem(groupItem);
                groupItem.NameVisible = false;
                ///SaveOpt(mgr.CurrentCanvas, mgr);
                return groupItem;
            }
            return null;
        }

        /// <param name="dc">画布</param>
        /// <param name="items">组合后的图元</param>
        /// <returns></returns>
        public static List<List<DesignerContainer>> UnGroup(DesignerCanvas dc, IEnumerable<DesignerContainer> items)
        {
            var m_de = new List<List<DesignerContainer>>();

            if (dc != null && items != null)
            {
                foreach (var groupRoot in items)
                {
                    var children = from child in dc.SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                        where child.ParentID == groupRoot.ID
                        select child;

                    var ListGroup = new List<DesignerContainer>();

                    foreach (var child in children)
                    {
                        child.ParentID = Guid.Empty;
                        ListGroup.Add(child);
                    }
                    m_de.Add(ListGroup);

                    dc.SelectionService.RemoveFromSelection(groupRoot);
                    dc.Remove(groupRoot);
                    UpdateZIndex(dc);
                }

                return m_de;
            }
            return null;
        }

        public static void UpdateZIndex(DesignerCanvas mgr)
        {
            var ordered = (from UIElement item in mgr.Children
                orderby Panel.GetZIndex(item)
                select item).ToList();

            for (var i = 0; i < ordered.Count; i++)
            {
                if (Panel.GetZIndex(ordered[i]) == int.MinValue)
                {
                    continue;
                }
                Panel.SetZIndex(ordered[i], i);
            }
        }

        private static Rect GetBoundingRectangle(IEnumerable<DesignerContainer> items)
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
    }
}