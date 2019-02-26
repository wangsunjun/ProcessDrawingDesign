using System;
using System.Collections.Generic;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     取消组合命令子类
    /// </summary>
    public class CommandUnGroup : Command
    {
        private DesignerContainer di;
        private List<List<DesignerContainer>> m_de;

        public override Boolean Run(CommandManager mgr, params object[] param)
        {
            var items = param[0] as IEnumerable<DesignerContainer>;

            m_de = CommandGroupOperate.UnGroup(mgr.CurrentCanvas, items);
            if (m_de == null)
            {
                return false;
            }
            return true;
        }

        public override bool UnRun(CommandManager mgr)
        {
            foreach (var de in m_de)
            {
                di = CommandGroupOperate.Group(mgr.CurrentCanvas, de);
                if (di == null)
                {
                    return false;
                }
            }

            return true;
        }

        public override bool ReRun(CommandManager mgr)
        {
            var items = new List<DesignerContainer>();
            items.Add(di);
            var ditems = CommandGroupOperate.UnGroup(mgr.CurrentCanvas, items);
            if (ditems == null)
            {
                //ddd.Add(items);
                return false;
            }

            return true;
        }

        public static void Run(CommandManager cmgr, IEnumerable<DesignerContainer> items)
        {
            cmgr.Run<CommandUnGroup>(items);
        }
    }
}