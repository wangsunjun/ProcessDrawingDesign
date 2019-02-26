using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     ”“∂‘∆Î ◊”¿‡
    /// </summary>
    public class CommandAlignRight : Command
    {
        private List<DesignerContainer> m_delist;
        private List<double> m_delt;
        private List<double> m_newPlaceList;
        private List<double> m_oldPlaceList;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            var items = param[0] as IEnumerable<DesignerContainer>;
            m_delist = items.ToList();
            if (items.Count() > 1)
            {
                var right = Canvas.GetLeft(items.First()) + items.First().Width;

                foreach (var item in items)
                {
                    var delta = right - (Canvas.GetLeft(item) + item.Width);
                    if (m_delt == null)
                    {
                        m_delt = new List<double>();
                    }
                    m_delt.Add(delta);
                    if (m_oldPlaceList == null)
                    {
                        m_oldPlaceList = new List<double>();
                    }
                    m_oldPlaceList.Add(Canvas.GetLeft(item));
                    foreach (DesignerContainer di in mgr.CurrentCanvas.SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
                        if (m_newPlaceList == null)
                        {
                            m_newPlaceList = new List<double>();
                        }
                        m_newPlaceList.Add(Canvas.GetLeft(di) + delta);
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
                        Canvas.SetLeft(di, m_oldPlaceList[i]);
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
                        Canvas.SetLeft(di, m_newPlaceList[i] - m_delt[i]);
                    }
                }

                return true;
            }
            return false;
        }

        public static void Run(CommandManager cmgr, IEnumerable<DesignerContainer> items)
        {
            cmgr.Run<CommandAlignRight>(items);
        }
    }
}