using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using Wss.Foundation.Controls;

namespace Wss.Foundation.Designer.Command
{
    public class CommandOpen : Command
    {
        //IEnumerable<XElement> items;

        private List<DesignerContainer> diitems;
        private byte[] md5;
        private XElement root;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            root = param[0] as XElement;
            if (root != null)
            {
                var canvasMd5 = MD5.Create();
                md5 = canvasMd5.ComputeHash(Encoding.Default.GetBytes(root.ToString()));
                Application.Current.Properties["FileMD5"] = md5;

                mgr.CurrentCanvas.Children.Clear();
                mgr.CurrentCanvas.SelectionService.ClearSelection();
                diitems = mgr.CurrentCanvas.DeserializeDesignerItems(root);

                return true;
            }
            return false;
        }

        public override bool UnRun(CommandManager mgr)
        {
            if (diitems != null)
            {
                foreach (var item in diitems)
                {
                    mgr.CurrentCanvas.Remove(item);
                }
                return true;
            }
            return false;
        }

        public override bool ReRun(CommandManager mgr)
        {
            var canvasMd5 = MD5.Create();
            md5 = canvasMd5.ComputeHash(Encoding.Default.GetBytes(root.ToString()));
            Application.Current.Properties["FileMD5"] = md5;

            mgr.CurrentCanvas.Children.Clear();
            mgr.CurrentCanvas.SelectionService.ClearSelection();
            // mgr.CurrentCanvas.BaissObject.Clear();
            diitems = mgr.CurrentCanvas.DeserializeDesignerItems(root);

            return true;
        }

        public static void Run(CommandManager cmgr, XElement root)
        {
            cmgr.Run<CommandOpen>(root);
        }
    }
}