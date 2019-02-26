using System.Windows.Controls;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     É¾³ý±³¾°  ×ÓÀà
    /// </summary>
    public class CommandDeleteBg : Command
    {
        private DesignerContainer m_di;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            var di = param[0] as DesignerContainer;
            m_di = di;
            if (di != null)
            {
                var idx = Panel.GetZIndex(di);
                if (idx == int.MinValue)
                {
                    mgr.CurrentCanvas.Remove(di);
                }
                return true;
            }
            return false;
        }

        public override bool UnRun(CommandManager mgr)
        {
            if (m_di != null)
            {
                if (!mgr.CurrentCanvas.Children.Contains(m_di))
                {
                    mgr.CurrentCanvas.Add(m_di);
                }
                return true;
            }
            return false;
        }

        public override bool ReRun(CommandManager mgr)
        {
            if (m_di != null)
            {
                var idx = Panel.GetZIndex(m_di);
                if (idx == int.MinValue)
                {
                    mgr.CurrentCanvas.Remove(m_di);
                }
                return true;
            }
            return false;
        }

        public static void Run(CommandManager cmgr, DesignerContainer item)
        {
            cmgr.Run<CommandDeleteBg>(item);
        }
    }
}