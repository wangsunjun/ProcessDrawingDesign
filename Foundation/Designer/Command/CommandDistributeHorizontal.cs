using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     水平均分  子类
    /// </summary>
    public class CommandDistributeHorizontal : Command
    {
        private List<double> m_delta;
        private List<DesignerContainer> m_DesignerContainer;
        private List<double> newleft;
        private double newoffset;
        private double olddistance;
        private List<double> oldleft;
        private double oldoffset;
        private List<double> oldright;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            var selectedItems = param[0] as IEnumerable<DesignerContainer>;
            if (selectedItems.Count() > 1)
            {
                //获得double最大值
                var left = Double.MaxValue;
                //获得double最小值
                var right = Double.MinValue;
                double sumWidth = 0;
                foreach (var item in selectedItems)
                {
                    //返回离左边较小的那一个
                    left = Math.Min(left, Canvas.GetLeft(item));
                    if (oldleft == null)
                    {
                        oldleft = new List<double>();
                    }
                    oldleft.Add(Canvas.GetLeft(item));
                    //返回离右边较小的那一个
                    right = Math.Max(right, Canvas.GetLeft(item) + item.Width);
                    if (oldright == null)
                    {
                        oldright = new List<double>();
                    }
                    oldright.Add(Canvas.GetLeft(item) + item.Width);
                    //图元的总宽度
                    sumWidth += item.Width;
                }

                var distance = Math.Max(0, (right - left - sumWidth)/(selectedItems.Count() - 1));
                olddistance = distance;
                //double distance = (right - left - sumWidth) / (selectedItems.Count() - 1);
                var offset = Canvas.GetLeft(selectedItems.First());
                oldoffset = offset;
                foreach (var item in selectedItems)
                {
                    if (m_DesignerContainer == null)
                    {
                        m_DesignerContainer = new List<DesignerContainer>();
                    }
                    m_DesignerContainer.Add(item);
                    var delta = offset - Canvas.GetLeft(item);
                    if (m_delta == null)
                    {
                        m_delta = new List<double>();
                    }
                    m_delta.Add(delta);
                    foreach (DesignerContainer di in mgr.CurrentCanvas.SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Convert.ToInt32(Canvas.GetLeft(di) + delta));
                        if (newleft == null)
                        {
                            newleft = new List<double>();
                        }
                        newleft.Add(Convert.ToInt32(Canvas.GetLeft(di) + delta));
                    }
                    if (sumWidth > mgr.CurrentCanvas.Width - left)
                    {
                        offset += Math.Max(0, (right - left - selectedItems.Last().Width)/(selectedItems.Count() - 1));
                        newoffset = offset;
                    }
                    else
                    {
                        offset = offset + item.Width + distance;
                        newoffset = offset;
                    }
                }
                return true;
            }
            return false;
        }

        public override bool UnRun(CommandManager mgr)
        {
            if (m_DesignerContainer.Count() > 1)
            {
                DesignerContainer di = null;
                for (var i = 0; i <= m_DesignerContainer.Count - 1; i++)
                {
                    di = m_DesignerContainer[i];

                    if (di != null)
                    {
                        Canvas.SetLeft(di, oldleft[i]);
                    }
                }

                return true;
            }


            return false;
        }

        public override bool ReRun(CommandManager mgr)
        {
            if (m_DesignerContainer.Count() > 1)
            {
                DesignerContainer di = null;
                for (var i = m_DesignerContainer.Count - 1; i > -1; i--)
                {
                    di = m_DesignerContainer[i];
                    if (di != null)
                    {
                        Canvas.SetLeft(di, newleft[i] - m_delta[i]);
                    }
                }

                return true;
            }


            return false;
        }

        public static void Run(CommandManager cmgr, IEnumerable<DesignerContainer> items)
        {
            cmgr.Run<CommandDistributeHorizontal>(items);
        }
    }
}