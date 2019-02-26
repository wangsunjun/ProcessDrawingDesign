using System;
using System.Collections.Generic;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     组合命令子类
    /// </summary>
    public class CommandGroup : Command
    {
        private List<List<DesignerContainer>> de;

        /// <summary>
        /// </summary>
        private List<DesignerContainer> di;

        public override Boolean Run(CommandManager mgr, params object[] param)
        {
            var items = param[0] as IEnumerable<DesignerContainer>;
            var DesignerContainer = CommandGroupOperate.Group(mgr.CurrentCanvas, items);

            if (DesignerContainer != null)
            {
                di = new List<DesignerContainer>();
                di.Add(DesignerContainer);
                return true;
            }

            return false;
            //return RunCmd(mgr, param[0] as IEnumerable<DesignerContainer>);
        }

        public override bool UnRun(CommandManager mgr)
        {
            ///CommandUndo.Run(mgr);\

            de = CommandGroupOperate.UnGroup(mgr.CurrentCanvas, di);
            if (de == null)
            {
                return false;
            }
            return true;
        }

        public override bool ReRun(CommandManager mgr)
        {
            di.Clear();
            foreach (var dei in de)
            {
                var re = CommandGroupOperate.Group(mgr.CurrentCanvas, dei);
                if (re == null)
                {
                    return false;
                }

                di.Add(re);
            }


            return true;
        }

        //private Boolean RunCmd(CommandManager mgr, IEnumerable<DesignerContainer> items)
        //{
        //   di= CommandGroupOperate.Group(mgr.CurrentCanvas, items);
        //   if (di != null)
        //   {
        //       return true;
        //   }
        //   return false;
        //}

        public static void Run(CommandManager cmgr, IEnumerable<DesignerContainer> items)
        {
            cmgr.Run<CommandGroup>(items);
        }
    }
}