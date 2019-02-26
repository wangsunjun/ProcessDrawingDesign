using System.Collections.Generic;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     显示名称  子类
    /// </summary>
    public class CommandShowName : Command
    {
        private List<DesignerContainer> m_di;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            var items = param[0] as IEnumerable<DesignerContainer>;
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (m_di == null)
                    {
                        m_di = new List<DesignerContainer>();
                    }
                    m_di.Add(item);
                    item.NameVisible = true;
                }
                return true;
            }
            return false;
        }

        public override bool UnRun(CommandManager mgr)
        {
            DesignerContainer di;
            if (m_di != null)
            {
                for (var i = 0; i < m_di.Count; i++)
                {
                    di = m_di[i];
                    di.NameVisible = false;
                }
                return true;
            }
            return false;
        }

        public override bool ReRun(CommandManager mgr)
        {
            foreach (var item in m_di)
            {
                item.NameVisible = true;
            }
            return true;
        }

        public static void Run(CommandManager cmgr, IEnumerable<DesignerContainer> items)
        {
            cmgr.Run<CommandShowName>(items);
        }
    }
}