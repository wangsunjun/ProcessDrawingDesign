using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     Ëõ·Å  ×ÓÀà
    /// </summary>
    public class CommandDragThumb : Command
    {
        private Dictionary<DesignerContainer, Point> m_dic;
        private Dictionary<DesignerContainer, Point> m_newdic;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            m_dic = param[0] as Dictionary<DesignerContainer, Point>;
            m_newdic = param[1] as Dictionary<DesignerContainer, Point>;
            return true;
        }

        public override bool UnRun(CommandManager mgr)
        {
            if (m_dic != null)
            {
                foreach (var item in m_dic)
                {
                    Canvas.SetLeft(item.Key, item.Value.X);
                    Canvas.SetTop(item.Key, item.Value.Y);
                }
                return true;
            }
            return false;
        }

        public override bool ReRun(CommandManager mgr)
        {
            if (m_newdic != null)
            {
                foreach (var item in m_newdic)
                {
                    Canvas.SetLeft(item.Key, item.Value.X);
                    Canvas.SetTop(item.Key, item.Value.Y);
                }
                return true;
            }
            return false;
        }

        public static void Run(CommandManager cmgr, Dictionary<DesignerContainer, Point> m_dic,
            Dictionary<DesignerContainer, Point> m_newdic)
        {
            cmgr.Run<CommandDragThumb>(m_dic, m_newdic);
        }
    }
}