using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     ¼ôÇÐÍ¼Ôª
    /// </summary>
    public class CommandCut : Command
    {
        private List<DesignerContainer> DesignerContainers;
        private List<DesignerContainer> m_DesignerContainerRedo;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            var di = param[0] as IEnumerable<DesignerContainer>;
            if (di != null)
            {
                foreach (var item in di)
                {
                    if (DesignerContainers == null)
                    {
                        DesignerContainers = new List<DesignerContainer>();
                    }
                    DesignerContainers.Add(item);
                    mgr.CurrentCanvas.Remove(item);
                }

                mgr.CurrentCanvas.SelectionService.ClearSelection();

                var pv = mgr.CurrentCanvas.FindName("pv") as PropertyView;
                if (pv != null) pv.InitProView();
                return true;
            }
            return false;
        }

        public override bool UnRun(CommandManager mgr)
        {
            if (DesignerContainers != null)
            {
                foreach (var item1 in DesignerContainers)
                {
                    if (m_DesignerContainerRedo == null)
                    {
                        m_DesignerContainerRedo = new List<DesignerContainer>();
                    }
                    m_DesignerContainerRedo.Add(item1);
                    mgr.CurrentCanvas.Add(item1);
                }


                UpdateZIndex(mgr.CurrentCanvas);
                return true;
            }
            return false;
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
            cmgr.Run<CommandCut>(items);
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