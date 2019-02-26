using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     ÖÃÓÚµ×²ã  ×ÓÀà
    /// </summary>
    public class CommandToBack : Command
    {
        private List<UIElement> childrenSorted;
        private List<int> m_int;
        private List<UIElement> m_listdi;
        private List<UIElement> selectionSorted;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            if (param.Length > 1)
            {
                selectionSorted = param[0] as List<UIElement>;
                childrenSorted = param[1] as List<UIElement>;
                var i = 0;
                var j = 0;
                foreach (var item in childrenSorted)
                {
                    if (m_listdi == null)
                    {
                        m_listdi = new List<UIElement>();
                    }
                    m_listdi.Add(item);
                    var idx = Panel.GetZIndex(item);
                    if (m_int == null)
                    {
                        m_int = new List<int>();
                    }
                    m_int.Add(idx);
                    if (idx == int.MinValue)
                    {
                        continue;
                    }
                    if (selectionSorted.Contains(item))
                    {
                        Panel.SetZIndex(item, j++);
                    }
                    else
                    {
                        Panel.SetZIndex(item, selectionSorted.Count + i++);
                    }
                }
                return true;
            }
            return false;
        }

        public override bool UnRun(CommandManager mgr)
        {
            UIElement item;
            if (m_listdi != null)
            {
                for (var i = 0; i < m_listdi.Count; i++)
                {
                    item = m_listdi[i];
                    if (item != null)
                    {
                        Panel.SetZIndex(item, m_int[i]);
                    }
                }
                return true;
            }
            return false;
        }

        public override bool ReRun(CommandManager mgr)
        {
            var i = 0;
            var j = 0;
            if (childrenSorted != null)
            {
                foreach (var item in childrenSorted)
                {
                    if (m_listdi == null)
                    {
                        m_listdi = new List<UIElement>();
                    }
                    m_listdi.Add(item);
                    var idx = Panel.GetZIndex(item);
                    if (m_int == null)
                    {
                        m_int = new List<int>();
                    }
                    m_int.Add(idx);
                    if (idx == int.MinValue)
                    {
                        continue;
                    }
                    if (selectionSorted.Contains(item))
                    {
                        Panel.SetZIndex(item, j++);
                    }
                    else
                    {
                        Panel.SetZIndex(item, selectionSorted.Count + i++);
                    }
                }
                return true;
            }
            return false;
        }

        public static void Run(CommandManager cmgr, List<UIElement> items, List<UIElement> di)
        {
            cmgr.Run<CommandToBack>(items, di);
        }
    }
}