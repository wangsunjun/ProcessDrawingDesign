using System.Windows.Documents;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     ’€œﬂ   ◊”¿‡
    /// </summary>
    public class CommandPolyline : Command
    {
        private DesignerCanvas dc;
        private DesignerContainer m_di;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            dc = param[0] as DesignerCanvas;
            var di = param[1] as DesignerContainer;
            if (dc != null)
            {
                m_di = di;
                dc.Add(di);
                dc.SelectionService.SelectItem(di);
            }
            var adornerLayer = AdornerLayer.GetAdornerLayer(dc);
            //  PolylineAdorner adorner = dc.m_pAdorner;
            //if (adornerLayer != null && adorner != null)
            //{
            //    if (adorner.IsMouseCaptured)
            //    {
            //        adorner.ReleaseMouseCapture();
            //    }
            //    adornerLayer.Remove(adorner);
            //}
            return true;
        }

        public override bool UnRun(CommandManager mgr)
        {
            if (m_di != null)
            {
                dc.Remove(m_di);
                return true;
            }
            return false;
        }

        public override bool ReRun(CommandManager mgr)
        {
            if (dc != null)
            {
                dc.Add(m_di);
                dc.SelectionService.SelectItem(m_di);
            }
            var adornerLayer = AdornerLayer.GetAdornerLayer(dc);
            //PolylineAdorner adorner = dc.m_pAdorner;
            //if (adornerLayer != null && adorner != null)
            //{
            //    if (adorner.IsMouseCaptured)
            //    {
            //        adorner.ReleaseMouseCapture();
            //    }
            //    adornerLayer.Remove(adorner);
            //}
            return true;
        }

        public static void Run(CommandManager cmgr, DesignerCanvas items, DesignerContainer di)
        {
            cmgr.Run<CommandPolyline>(items, di);
        }
    }
}