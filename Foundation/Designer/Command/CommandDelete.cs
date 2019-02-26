using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     删除命令子类
    /// </summary>
    public class CommandDelete : Command
    {
        private List<DesignerContainer> DesignerContainer;
        private List<DesignerContainer> m_DesignerContainerRedo;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            var di = param[0] as IEnumerable<DesignerContainer>;
            //foreach (DesignerContainer dd in di)
            //{
            //    DesignerContainer = new List<DesignerContainer>();
            //    DesignerContainer.Add(dd);
            //}
            //if (DeleteCurrentSelection(mgr, di))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            if (di != null)
            {
                foreach (var item1 in di)
                {
                    if (DesignerContainer == null)
                    {
                        DesignerContainer = new List<DesignerContainer>();
                    }
                    DesignerContainer.Add(item1);
                    mgr.CurrentCanvas.Remove(item1);
                }

                mgr.CurrentCanvas.SelectionService.ClearSelection();
                UpdateZIndex(mgr.CurrentCanvas);
                var pv = mgr.CurrentCanvas.FindName("pv") as PropertyView;
                if (pv != null) pv.InitProView();
                mgr.CurrentCanvas.Focus(); //当前画布获取焦点
                return true;
            }
            return false;
        }

        public override bool UnRun(CommandManager mgr)
        {
            if (DesignerContainer != null)
            {
                foreach (var item1 in DesignerContainer)
                {
                    if (m_DesignerContainerRedo == null)
                    {
                        m_DesignerContainerRedo = new List<DesignerContainer>();
                    }
                    m_DesignerContainerRedo.Add(item1);
                    if (!mgr.CurrentCanvas.Children.Contains(item1))
                    {
                        mgr.CurrentCanvas.Add(item1);
                    }
                }


                UpdateZIndex(mgr.CurrentCanvas);
                return true;
            }
            return false;
            //if (RecoverDeleted(mgr, DesignerContainer))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        public override bool ReRun(CommandManager mgr)
        {
            foreach (var dd in m_DesignerContainerRedo)
            {
                mgr.CurrentCanvas.Remove(dd);
            }

            mgr.CurrentCanvas.SelectionService.ClearSelection();
            UpdateZIndex(mgr.CurrentCanvas);

            return true;
        }

        public static void Run(CommandManager cmgr, IEnumerable<DesignerContainer> items)
        {
            cmgr.Run<CommandDelete>(items);
        }

        public void UpdateZIndex(DesignerCanvas mgr)
        {
            var ordered = (from UIElement item in mgr.Children
                orderby Panel.GetZIndex(item)
                select item).ToList();

            for (var i = 0; i < ordered.Count; i++)
            {
                if (Panel.GetZIndex(ordered[i]) == int.MinValue)
                {
                    continue;
                }
                Panel.SetZIndex(ordered[i], i);
            }
        }
    }
}