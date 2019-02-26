using System.Collections.Generic;
using System.Linq;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     µ»øÌ√¸¡Ó ◊”¿‡
    /// </summary>
    public class CommandUniformWidth : Command
    {
        private List<DesignerContainer> m_di;
        private double m_newwidth;
        private List<double> m_oldwidth;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            var items = param[0] as IEnumerable<DesignerContainer>;
            //if (items.Count() > 1)
            //{
            //    double width = items.First().Width;

            //    foreach (DesignerContainer Item in items)
            //    {
            //        Item.Width = width;
            //    }
            //    return true;
            //}
            if (items.Count() > 1)
            {
                var width = items.First().Width;
                m_newwidth = width;
                foreach (var item in items)
                {
                    if (m_oldwidth == null)
                    {
                        m_oldwidth = new List<double>();
                    }
                    m_oldwidth.Add(item.Width);
                    item.Width = width;


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
                    di.Width = m_oldwidth[i];
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
                    item.Width = m_newwidth;
                }
                return true;
            }
            return false;
        }

        public static void Run(CommandManager cmgr, IEnumerable<DesignerContainer> items)
        {
            cmgr.Run<CommandUniformWidth>(items);
        }
    }
}