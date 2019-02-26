using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     µ×¶ÔÆë ×ÓÀà
    /// </summary>
    public class CommandAlignBottom : Command
    {
        private List<DesignerContainer> m_delist;
        private List<double> m_delt;
        private List<double> m_height;
        private double m_newplace;
        private List<double> m_newPlaceList;
        private double m_oldplace;
        private List<double> m_oldPlaceList;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            var items = param[0] as IEnumerable<DesignerContainer>;
            m_delist = items.ToList();
            if (items.Count() > 1)
            {
                var bottom = Canvas.GetTop(items.First()) + items.First().Height;

                foreach (var item in items)
                {
                    var delta = bottom - (Canvas.GetTop(item) + item.Height);
                    if (m_delt == null)
                    {
                        m_delt = new List<double>();
                    }
                    m_delt.Add(delta);
                    if (m_height == null)
                    {
                        m_height = new List<double>();
                    }
                    m_height.Add(item.Height);
                    m_oldplace = Canvas.GetTop(item) + item.Height;
                    if (m_oldPlaceList == null)
                    {
                        m_oldPlaceList = new List<double>();
                    }
                    m_oldPlaceList.Add(m_oldplace);
                    foreach (DesignerContainer di in mgr.CurrentCanvas.SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                        m_newplace = Canvas.GetTop(di);
                        if (m_newPlaceList == null)
                        {
                            m_newPlaceList = new List<double>();
                        }
                        m_newPlaceList.Add(m_newplace);
                    }
                }
                return true;
            }
            return false;
        }

        public override bool UnRun(CommandManager mgr)
        {
            if (m_delist.Count > 0)
            {
                DesignerContainer di = null;
                for (var i = 0; i <= m_delist.Count - 1; i++)
                {
                    di = m_delist[i];
                    if (di != null)
                    {
                        Canvas.SetTop(di, m_oldPlaceList[i] - m_height[i]);
                    }
                }

                return true;
            }
            return false;
        }

        public override bool ReRun(CommandManager mgr)
        {
            if (m_delist.Count > 0)
            {
                DesignerContainer di = null;
                for (var i = m_delist.Count - 1; i > -1; i--)
                {
                    di = m_delist[i];
                    if (di != null)
                    {
                        Canvas.SetTop(di, m_newPlaceList[i]);
                    }
                }

                return true;
            }
            return false;
        }

        public static void Run(CommandManager cmgr, IEnumerable<DesignerContainer> items)
        {
            cmgr.Run<CommandAlignBottom>(items);
        }
    }
}