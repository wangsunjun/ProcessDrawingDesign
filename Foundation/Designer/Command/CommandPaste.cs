using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Wss.Foundation.Controls;
using Wss.FoundationCore.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     粘贴图元  gw
    /// </summary>
    public class CommandPaste : Command
    {
        private static int index; //记录新粘贴出图元名称后缀
        private Dictionary<Guid, DesignerContainer> NewIDsDItem; //记录

        private XElement LoadSerializedDataFromClipBoard()
        {
            if (Clipboard.ContainsData(DataFormats.Xaml))
            {
                var clipboardData = Clipboard.GetData(DataFormats.Xaml) as String;

                if (String.IsNullOrEmpty(clipboardData))
                    return null;
                try
                {
                    return XElement.Load(new StringReader(clipboardData));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                    //LogManager.Instance.AddLog(LogEventType.Warning, e, "异常日志");
                }
            }

            return null;
        }

        /// <summary>
        ///     反序列化DesignerContainer
        /// </summary>
        /// <returns></returns>
        public static DesignerContainer DeserializeDesignerContainer(XElement itemXml, Guid id)
        {
            return itemXml.ToDesignerContainer();
        }

        public string CreateNewName(CommandManager mgr, DesignerContainer di)
        {
            var name = "新建图元";
            //name = name.Length > 4 ? name.Substring(0, 4) : name;
            //if (string.IsNullOrEmpty(name))
            //{
            //    name = "新建图元";
            //}
            index = 0;
            var nameList = mgr.CurrentCanvas.Children.OfType<DesignerContainer>()
                .ToDictionary(item => item.Name, item => item.ID);
            int i = nameList.Count-1;
            while (nameList.ContainsKey(name))
            {
                i++;
                name ="新建图元"+ i;
            }

            return name;
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

        public override bool Run(CommandManager mgr, params object[] param)
        {
            //IEnumerable<DesignerContainer> di = param[0] as IEnumerable<DesignerContainer>;
            var root = LoadSerializedDataFromClipBoard();

            if (root == null)
                return false;

            // create DesignerContainers
            DoRun(mgr, root);
            return true;
        }

        private void DoRun(CommandManager mgr, XElement root)
        {
            NewIDsDItem = new Dictionary<Guid, DesignerContainer>(); //create dic to add new paste DesignerContainer
            var mappingOldToNewIDs = new Dictionary<Guid, Guid>();

            var newItems = new List<ISelectable>();
            var itemsXML = root.Elements("DesignerContainers").Elements();

            var offsetX = Double.Parse(root.Attribute("OffsetX").Value, CultureInfo.InvariantCulture);
            var offsetY = Double.Parse(root.Attribute("OffsetY").Value, CultureInfo.InvariantCulture);
            double speedX = 10;
            double speedY = 10;

            foreach (var itemXML in itemsXML)
            {
                var oldID = new Guid(itemXML.Element("ID").Value);//--
                var newID = Guid.NewGuid();
                mappingOldToNewIDs.Add(oldID, newID);
                //DesignerContainer Item = DeserializeDesignerContainer(itemXML, newID, offsetX, offsetY);

                try
                {
                    var item = DeserializeDesignerContainer(itemXML, newID);
                    item.Name = CreateNewName(mgr, item);

                    mgr.CurrentCanvas.Add(item);
                    //Add new paste DesignerContainer
                    NewIDsDItem.Add(newID, item);
                    //SetConnectorDecoratorTemplate(Item);
                    newItems.Add(item);
                    var newLeft = Canvas.GetLeft(item) + offsetX;
                    var newTop = Canvas.GetTop(item) + offsetY;
                    if (newLeft + item.Width + 10 > mgr.CurrentCanvas.Width)
                    {
                        speedX = -10;
                        //newLeft = Canvas.GetLeft(Item) - offsetX;
                    }
                    if (newTop + item.Height + 10 > mgr.CurrentCanvas.Height)
                    {
                        speedY = -10;
                        //newTop = Canvas.GetTop(Item) - offsetY;
                    }
                    Canvas.SetLeft(item, newLeft);
                    Canvas.SetTop(item, newTop);
                }
                catch
                {
                }
            }

            // update group hierarchy
            mgr.CurrentCanvas.SelectionService.ClearSelection();
            foreach (DesignerContainer el in newItems)
            {
                if (el.ParentID != Guid.Empty)
                    el.ParentID = mappingOldToNewIDs[el.ParentID];
            }


            foreach (DesignerContainer item in newItems)
            {
                if (item.ParentID == Guid.Empty)
                {
                    mgr.CurrentCanvas.SelectionService.AddToSelection(item);
                }
            }

            DesignerCanvas.BringToFront.Execute(null, mgr.CurrentCanvas);
            //PropertyView pv = mgr.CurrentCanvas.FindName("pv") as PropertyView;
            //if (pv != null) pv.ShowProView();
            //OptStack.SaveOpt(mgr.CurrentCanvas);

            // update paste offset
            root.Attribute("OffsetX").Value = (offsetX + speedX).ToString();
            root.Attribute("OffsetY").Value = (offsetY + speedY).ToString();
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
        }

        public override bool UnRun(CommandManager mgr)
        {
            foreach (var item1 in NewIDsDItem)
            {
                mgr.CurrentCanvas.Remove(item1.Value);
                mgr.CurrentCanvas.SelectionService.ClearSelection();
                UpdateZIndex(mgr.CurrentCanvas);
                var pv = mgr.CurrentCanvas.FindName("pv") as PropertyView;
                if (pv != null) pv.InitProView();
                mgr.CurrentCanvas.Focus(); //当前画布获取焦点
                return true;
            }
            return false;
        }

        public override bool ReRun(CommandManager mgr)
        {
            var root = LoadSerializedDataFromClipBoard();

            if (root == null)
                return false;

            // create DesignerContainers
            DoRun(mgr, root);
            //throw new NotImplementedException();
            return true;
        }

        public XElement SerializeDesignerContainers(IEnumerable<DesignerContainer> DesignerContainers)
        {
            return DesignerContainers.ToXElement();
        }

        public static void Run(CommandManager cmgr, IEnumerable<DesignerContainer> items)
        {
            cmgr.Run<CommandPaste>(items);
        }
    }
}