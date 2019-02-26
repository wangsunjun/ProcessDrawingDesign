using System.Collections.Generic;
using System.Linq;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     等高命令 子类
    /// </summary>
    public class CommandUniformHeight : Command
    {
        private List<DesignerContainer> m_di;
        private double m_newheight;
        private List<double> m_oldheight;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            var items = param[0] as IEnumerable<DesignerContainer>;

            if (items.Count() > 1)
            {
                var height = items.First().Height;
                m_newheight = height;
                foreach (var item in items)
                {
                    if (m_oldheight == null)
                    {
                        m_oldheight = new List<double>();
                    }
                    m_oldheight.Add(item.Height);
                    item.Height = height;


                    if (m_di == null)
                    {
                        m_di = new List<DesignerContainer>();
                    }
                    m_di.Add(item);
                }
                return true;
            }
            return false;
        }

        public override bool UnRun(CommandManager mgr)
        {
            if (m_di.Count > 0)
            {
                DesignerContainer di = null;
                for (var i = 0; i <= m_di.Count - 1; i++)
                {
                    di = m_di[i];
                    di.Height = m_oldheight[i];
                }
                return true;
            }
            return false;
        }

        public override bool ReRun(CommandManager mgr)
        {
            if (m_di != null)
            {
                foreach (var item in m_di)
                {
                    item.Height = m_newheight;
                }
                return true;
            }
            return false;
        }

        public static void Run(CommandManager cmgr, IEnumerable<DesignerContainer> items)
        {
            cmgr.Run<CommandUniformHeight>(items);
        }
    }
}