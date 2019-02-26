using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     ¸´ÖÆÍ¼Ôª  gw
    /// </summary>
    public class CommandCopy : Command
    {
        private IEnumerable<DesignerContainer> di;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            di = param[0] as IEnumerable<DesignerContainer>;
            DoRun(di);
            return true;
        }

        private void DoRun(IEnumerable<DesignerContainer> di)
        {
            var DesignerContainersXML = SerializeDesignerContainers(di);
            //XElement connectionsXML = SerializeConnections(selectedConnections);

            var root = new XElement("Root");
            root.Add(DesignerContainersXML);
            //root.Add(connectionsXML);

            root.Add(new XAttribute("OffsetX", 10));
            root.Add(new XAttribute("OffsetY", 10));

            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
        }

        public override bool UnRun(CommandManager mgr)
        {
            //throw new NotImplementedException();
            Clipboard.Clear();
            return true;
        }

        public override bool ReRun(CommandManager mgr)
        {
            DoRun(di);
            //throw new NotImplementedException();
            return true;
        }

        public XElement SerializeDesignerContainers(IEnumerable<DesignerContainer> DesignerContainers)
        {
            return DesignerContainers.ToXElement();
        }

        public static void Run(CommandManager cmgr, IEnumerable<DesignerContainer> items)
        {
            cmgr.Run<CommandCopy>(items);
        }
    }
}