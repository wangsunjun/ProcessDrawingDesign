using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     ÏÂÒÆÒ»²ã
    /// </summary>
    public class CommandBringBack : Command
    {
        private List<int> m_currentnewint;
        private List<UIElement> m_ordered;
        private List<UIElement> ordered;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            ordered = param[0] as List<UIElement>;
            if (ordered != null)
            {
                m_ordered = ordered;
                var count = mgr.CurrentCanvas.Children.Count;

                for (var i = 0; i < ordered.Count; i++)
                {
                    var currentIndex = Panel.GetZIndex(ordered[i]);
                    if (m_currentnewint == null)
                    {
                        m_currentnewint = new List<int>();
                    }
                    m_currentnewint.Add(currentIndex);
                    var newIndex = Math.Max(i - count, currentIndex - 1);
                    if (currentIndex != newIndex)
                    {
                        Panel.SetZIndex(ordered[i], newIndex);
                        var it =
                            mgr.CurrentCanvas.Children.OfType<UIElement>()
                                .Where(item => Panel.GetZIndex(item) == newIndex);

                        foreach (var elm in it)
                        {
                            if (elm != ordered[i])
                            {
                                Panel.SetZIndex(elm, currentIndex);
                                break;
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public override bool UnRun(CommandManager mgr)
        {
            if (m_ordered != null)
            {
                for (var i = 0; i < m_ordered.Count; i++)
                {
                    Panel.SetZIndex(m_ordered[i], m_currentnewint[i]);
                }
                return true;
            }
            return false;
        }

        public override bool ReRun(CommandManager mgr)
        {
            var count = mgr.CurrentCanvas.Children.Count;
            if (ordered != null)
            {
                for (var i = 0; i < ordered.Count; i++)
                {
                    var currentIndex = Panel.GetZIndex(ordered[i]);
                    var newIndex = Math.Max(i - count, currentIndex - 1);
                    if (currentIndex != newIndex)
                    {
                        Panel.SetZIndex(ordered[i], newIndex);
                        var it =
                            mgr.CurrentCanvas.Children.OfType<UIElement>()
                                .Where(item => Panel.GetZIndex(item) == newIndex);

                        foreach (var elm in it)
                        {
                            if (elm != ordered[i])
                            {
                                Panel.SetZIndex(elm, currentIndex);
                                break;
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public static void Run(CommandManager cmgr, List<UIElement> items)
        {
            cmgr.Run<CommandBringBack>(items);
        }
    }
}