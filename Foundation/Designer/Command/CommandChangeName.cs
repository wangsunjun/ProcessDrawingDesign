using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     改变图元名称子类   暂未实现
    /// </summary>
    public class CommandChangeName : Command
    {
        private DesignerContainer m_de;
        private string m_NewName = string.Empty;
        private string m_OldName = string.Empty;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            if (param != null)
            {
                if (param.Length == 3)
                {
                    m_de = param[0] as DesignerContainer;
                    m_NewName = param[1].ToString();
                    m_OldName = param[2].ToString();

                    return true;
                }
                return false;
            }

            return false;
        }

        public override bool UnRun(CommandManager mgr)
        {
            //throw new NotImplementedException();
            if (m_de != null)
            {
                m_de.Name = m_OldName;
                return true;
            }
            return false;
        }

        public override bool ReRun(CommandManager mgr)
        {
            if (m_de != null)
            {
                m_de.Name = m_NewName;
                return true;
            }
            return false;
        }

        public static void Run(CommandManager cmgr, DesignerContainer di, string NewName, string OldName)
        {
            var Pa = new object[3];
            Pa[0] = di;
            Pa[1] = NewName;
            Pa[2] = OldName;

            cmgr.Run<CommandChangeName>(Pa);
        }
    }
}