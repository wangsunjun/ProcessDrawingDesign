using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Wss.Foundation.Controls;
using Wss.FoundationCore.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     逆时针旋转90度命令  子类
    /// </summary>
    public class CommandRotateLeft : Command
    {
        private List<double> m_angle;
        private List<double> m_angleRedo;
        private List<ISelectable> M_list;
        private List<double> m_newHeight;
        private List<double> m_newLeft;
        private List<double> m_newTop;
        private List<double> m_newWidth;
        private List<double> m_oldHeight;
        private List<double> m_oldLeft;
        private List<double> m_oldTop;
        private List<double> m_oldWidth;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            M_list = mgr.CurrentCanvas.SelectionService.SelectedDesignerContainer;
            var d = Convert.ToDouble(param[0]);
            if (Rotate(d, mgr))
            {
                return true;
            }
            return false;
        }

        public override bool UnRun(CommandManager mgr)
        {
            DesignerContainer item = null;

            for (var i = 0; i < M_list.Count; i++)
            {
                item = M_list[i] as DesignerContainer;
                Canvas.SetLeft(item, m_oldLeft[i]);
                Canvas.SetTop(item, m_oldTop[i]);
                item.Width = m_oldWidth[i];
                item.Height = m_oldHeight[i];
                for (var j = 0; j <= m_angle.Count - 1; j++)
                {
                    //Item.Angle = (m_angle[i] ) % 360;
                    item.Angle = (m_angle[j] + 90)%360;
                    if (m_angleRedo == null)
                    {
                        m_angleRedo = new List<double>();
                    }
                    m_angleRedo.Add(item.Angle);
                }
            }
            return true;
        }

        public override bool ReRun(CommandManager mgr)
        {
            DesignerContainer item = null;
            if (M_list.Count > 0)
            {
                for (var i = 0; i < M_list.Count; i++)
                {
                    item = M_list[i] as DesignerContainer;
                    Canvas.SetTop(item, m_newTop[i]);
                    Canvas.SetLeft(item, m_newLeft[i]);
                    item.Height = m_newWidth[i];
                    item.Width = m_newHeight[i];
                    for (var j = 0; j <= m_angle.Count - 1; j++)
                    {
                        item.Angle = (m_angle[j])%360;
                    }
                }


                return true;
            }
            return false;
        }

        private Boolean Rotate(double angle, CommandManager mgr)
        {
            if (mgr.CurrentCanvas.SelectionService.SelectedDesignerContainer.Count > 0)
            {
                foreach (DesignerContainer item in mgr.CurrentCanvas.SelectionService.SelectedDesignerContainer)
                {
                    if (item != null && item.ParentID == Guid.Empty)
                    {
                        item.Angle = (item.Angle + angle)%360;
                        if (m_angle == null)
                        {
                            m_angle = new List<double>();
                        }
                        m_angle.Add(item.Angle);
                        var left = Convert.ToInt32(Canvas.GetLeft(item) - (item.Height - item.Width)/2);
                        if (m_oldLeft == null)
                        {
                            m_oldLeft = new List<double>();
                        }
                        m_oldLeft.Add(Canvas.GetLeft(item));
                        var top = Convert.ToInt32(Canvas.GetTop(item) - (item.Width - item.Height)/2);
                        if (m_oldTop == null)
                        {
                            m_oldTop = new List<double>();
                        }
                        m_oldTop.Add(Canvas.GetTop(item));
                        left = left + item.Height > mgr.CurrentCanvas.ActualWidth
                            ? (int) (mgr.CurrentCanvas.ActualWidth - item.Height)
                            : left < 0 ? 0 : left;
                        top = top + item.Width > mgr.CurrentCanvas.ActualHeight
                            ? (int) (mgr.CurrentCanvas.ActualHeight - item.Width)
                            : top < 0 ? 0 : top;
                        if (m_newLeft == null)
                        {
                            m_newLeft = new List<double>();
                        }
                        m_newLeft.Add(left);
                        if (m_newTop == null)
                        {
                            m_newTop = new List<double>();
                        }
                        m_newTop.Add(top);
                        Canvas.SetLeft(item, left);
                        Canvas.SetTop(item, top);
                        if (m_oldWidth == null)
                        {
                            m_oldWidth = new List<double>();
                        }
                        m_oldWidth.Add(item.Width);
                        if (m_oldHeight == null)
                        {
                            m_oldHeight = new List<double>();
                        }
                        m_oldHeight.Add(item.Height);
                        var width = item.Width;
                        item.Width = item.Height;
                        item.Height = width;
                        if (m_newWidth == null)
                        {
                            m_newWidth = new List<double>();
                        }
                        m_newWidth.Add(item.Width);
                        if (m_newHeight == null)
                        {
                            m_newHeight = new List<double>();
                        }
                        m_newHeight.Add(item.Height);
                        var dt = item.Template.FindName("PART_DragThumb", item) as DragThumb;
                        if (dt != null && dt.Template.HasContent && item.Content is FrameworkElement)
                        {
                            dt.LayoutTransform = (item.Content as FrameworkElement).LayoutTransform;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public static void Run(CommandManager cmgr, double items)
        {
            cmgr.Run<CommandRotateLeft>(items);
        }
    }
}