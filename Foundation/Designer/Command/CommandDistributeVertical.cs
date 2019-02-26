using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     垂直均分  子类
    /// </summary>
    public class CommandDistributeVertical : Command
    {
        private List<double> m_delta;
        private List<DesignerContainer> m_DesignerContainer;
        private double newoffset;
        private List<double> newtop;
        private List<double> oldbottom;
        private List<double> oldtop;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            var selectedItems = param[0] as IEnumerable<DesignerContainer>;
            if (selectedItems.Count() > 1)
            {
                var top = Double.MaxValue;
                var bottom = Double.MinValue;
                double sumHeight = 0;
                foreach (var item in selectedItems)
                {
                    top = Math.Min(top, Canvas.GetTop(item));
                    if (oldtop == null)
                    {
                        oldtop = new List<double>();
                    }
                    oldtop.Add(Canvas.GetTop(item));
                    bottom = Math.Max(bottom, Canvas.GetTop(item) + item.Height);
                    if (oldbottom == null)
                    {
                        oldbottom = new List<double>();
                    }
                    oldbottom.Add(Canvas.GetTop(item) + item.Height);
                    sumHeight += item.Height;
                }

                var distance = Math.Max(0, (bottom - top - sumHeight)/(selectedItems.Count() - 1));
                //double distance = (bottom - top - sumHeight) / (selectedItems.Count() - 1);
                var offset = Canvas.GetTop(selectedItems.First());

                foreach (var item in selectedItems)
                {
                    if (m_DesignerContainer == null)
                    {
                        m_DesignerContainer = new List<DesignerContainer>();
                    }
                    m_DesignerContainer.Add(item);
                    var delta = offset - Canvas.GetTop(item);
                    if (m_delta == null)
                    {
                        m_delta = new List<double>();
                    }
                    m_delta.Add(delta);
                    foreach (DesignerContainer di in mgr.CurrentCanvas.SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                        if (newtop == null)
                        {
                            newtop = new List<double>();
                        }
                        newtop.Add(Convert.ToInt32(Canvas.GetTop(di) + delta));
                    }
                    if (sumHeight > mgr.CurrentCanvas.Height - top)
                    {
                        offset += Math.Max(0, (bottom - top - selectedItems.Last().Height)/(selectedItems.Count() - 1));
                        newoffset = offset;
                    }
                    else
                    {
                        offset = offset + item.Height + distance;
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
                        Canvas.SetTop(di, oldtop[i]);
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
                        Canvas.SetTop(di, newtop[i] - m_delta[i]);
                    }
                }

                return true;
            }


            return false;
        }

        public static void Run(CommandManager cmgr, IEnumerable<DesignerContainer> items)
        {
            cmgr.Run<CommandDistributeVertical>(items);
        }
    }
}