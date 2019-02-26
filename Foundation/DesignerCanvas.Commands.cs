using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Linq;
using Wss.Foundation.Controls;
using Wss.Foundation.Designer;
using Wss.Foundation.Designer.Command;
using Wss.FoundationCore.Controls;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using CommandManager = Wss.Foundation.Designer.Command.CommandManager;
using DataFormats = System.Windows.DataFormats;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using PrintDialog = System.Windows.Controls.PrintDialog;

namespace Wss.Foundation
{
    public partial class DesignerCanvas
    {
        #region 路由事件集合
        private static int index;
        public static RoutedCommand Lock = new RoutedCommand();
        public static RoutedCommand UnLock = new RoutedCommand();
        public static RoutedCommand DeleteColumn = new RoutedCommand();
        public static RoutedCommand Group = new RoutedCommand();
        public static RoutedCommand Ungroup = new RoutedCommand();
        public static RoutedCommand BringForward = new RoutedCommand();
        public static RoutedCommand BringToFront = new RoutedCommand();
        public static RoutedCommand SendBackward = new RoutedCommand();
        public static RoutedCommand SendToBack = new RoutedCommand();
        public static RoutedCommand AlignTop = new RoutedCommand();
        public static RoutedCommand AlignVerticalCenters = new RoutedCommand();
        public static RoutedCommand AlignBottom = new RoutedCommand();
        public static RoutedCommand AlignLeft = new RoutedCommand();
        public static RoutedCommand AlignHorizontalCenters = new RoutedCommand();
        public static RoutedCommand AlignRight = new RoutedCommand();
        public static RoutedCommand DistributeHorizontal = new RoutedCommand();
        public static RoutedCommand DistributeVertical = new RoutedCommand();
        public static RoutedCommand SelectAll = new RoutedCommand();
        public static RoutedCommand ChangeBg = new RoutedCommand();
        public static RoutedCommand DeleteBg = new RoutedCommand();
        public static RoutedCommand MirrorH = new RoutedCommand();
        public static RoutedCommand MirrorV = new RoutedCommand();
        public static RoutedCommand RotateRight = new RoutedCommand();
        public static RoutedCommand RotateLeft = new RoutedCommand();
        public static RoutedCommand UniformWidth = new RoutedCommand();
        public static RoutedCommand UniformHeight = new RoutedCommand();
        public static RoutedCommand ConnectDevice = new RoutedCommand();
        public static RoutedCommand Undo = new RoutedCommand();
        public static RoutedCommand Redo = new RoutedCommand();
        public static RoutedCommand ShowName = new RoutedCommand();
        public static RoutedCommand HideName = new RoutedCommand();
        public static RoutedCommand SelectSameType = new RoutedCommand();
        private readonly string _path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        #endregion

        public CommandManager CommManager { get; private set; }

        public void IntlizeCommands()
        {
            CommManager = new CommandManager(this);
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, New_Executed, New_Enabled));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Open_Executed, Open_Enabled));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save_Executed, Save_Enabled));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, Print_Executed));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, Cut_Executed, Cut_Enabled));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, Copy_Executed, Copy_Enabled));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Paste_Executed, Paste_Enabled));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, Delete_Executed, Delete_Enabled));
            CommandBindings.Add(new CommandBinding(Lock, Lock_Executed, Lock_Enabled));
            CommandBindings.Add(new CommandBinding(DeleteColumn, DeleteColumn_Executed, DeleteColumn_Enabled));
            CommandBindings.Add(new CommandBinding(UnLock, UnLock_Executed, UnLock_Enabled));
            CommandBindings.Add(new CommandBinding(Group, Group_Executed, Group_Enabled)); //组合命令   LXD
            CommandBindings.Add(new CommandBinding(Ungroup, Ungroup_Executed, Ungroup_Enabled)); //取消组合 
            CommandBindings.Add(new CommandBinding(BringForward, BringForward_Executed, Order_Enabled));
            CommandBindings.Add(new CommandBinding(BringToFront, BringToFront_Executed, Order_Enabled));
            CommandBindings.Add(new CommandBinding(SendBackward, SendBackward_Executed, Order_Enabled));
            CommandBindings.Add(new CommandBinding(SendToBack, SendToBack_Executed, Order_Enabled));
            CommandBindings.Add(new CommandBinding(AlignTop, AlignTop_Executed, Align_Enabled));
            CommandBindings.Add(new CommandBinding(AlignVerticalCenters, AlignVerticalCenters_Executed, Align_Enabled));
            CommandBindings.Add(new CommandBinding(AlignBottom, AlignBottom_Executed, Align_Enabled));
            CommandBindings.Add(new CommandBinding(AlignLeft, AlignLeft_Executed, Align_Enabled));
            CommandBindings.Add(new CommandBinding(AlignHorizontalCenters, AlignHorizontalCenters_Executed,  Align_Enabled));
            CommandBindings.Add(new CommandBinding(AlignRight, AlignRight_Executed, Align_Enabled));
            CommandBindings.Add(new CommandBinding(DistributeHorizontal, DistributeHorizontal_Executed, Distribute_Enabled));
            CommandBindings.Add(new CommandBinding(DistributeVertical, DistributeVertical_Executed, Distribute_Enabled));
            CommandBindings.Add(new CommandBinding(SelectAll, SelectAll_Executed, SelectAll_Enabled));
            SelectAll.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(ChangeBg, ChangeBg_Executed));
            CommandBindings.Add(new CommandBinding(DeleteBg, DeleteBg_Executed, DeleteBg_Enabled));
            CommandBindings.Add(new CommandBinding(MirrorH, MirrorH_Executed, MirrorH_Enabled));
            CommandBindings.Add(new CommandBinding(MirrorV, MirrorV_Executed, MirrorV_Enabled));
            CommandBindings.Add(new CommandBinding(RotateRight, RotateRight_Executed, RotateRight_Enabled));
            CommandBindings.Add(new CommandBinding(RotateLeft, RotateLeft_Executed, RotateLeft_Enabled));
            CommandBindings.Add(new CommandBinding(UniformWidth, UniformWidth_Executed, UniformWidth_Enabled));
            CommandBindings.Add(new CommandBinding(UniformHeight, UniformHeight_Executed, UniformHeight_Enabled));
            CommandBindings.Add(new CommandBinding(ConnectDevice, ConnectDevice_Executed, ConnectDevice_Enabled));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, Undo_Executed, Undo_Enabled));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, Redo_Executed, Redo_Enabled));
            CommandBindings.Add(new CommandBinding(ShowName, ShowName_Executed, ShowName_Enabled));
            CommandBindings.Add(new CommandBinding(HideName, HideName_Executed, HideName_Enabled));
            CommandBindings.Add(new CommandBinding(SelectSameType, SelectSameType_Executed, SelectSameType_Enabled));

            AllowDrop = true;    
        }

        //Print Command
        private void Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectionService.ClearSelection();

            var printDialog = new PrintDialog();

            if (true == printDialog.ShowDialog())
            {
                printDialog.PrintVisual(this, "WPF Diagram");
            }
        }

        /// <summary>
        ///     置于顶层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BringToFront_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectionSorted = (from item in SelectionService.SelectedDesignerContainer
                orderby GetZIndex(item as UIElement) ascending
                select item as UIElement).ToList();

            var childrenSorted = (from UIElement item in Children
                orderby GetZIndex(item) ascending
                select item).ToList();
            //sqh修改
            CommandToFront.Run(CommManager, selectionSorted, childrenSorted);

            //int i = 0;
            //int j = 0;
            //foreach (UIElement Item in childrenSorted)
            //{
            //    int idx = Canvas.GetZIndex(Item);
            //    if (idx == int.MinValue) { continue; }
            //    if (selectionSorted.Contains(Item))
            //    {
            //        Canvas.SetZIndex(Item, childrenSorted.Count - selectionSorted.Count + j++);
            //    }
            //    else
            //    {
            //        Canvas.SetZIndex(Item, i++);
            //    }
            //}
            // OptStack.SaveOpt(this);
        }

        /// <summary>
        ///     下移一层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendBackward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var ordered = (from item in SelectionService.SelectedDesignerContainer
                orderby GetZIndex(item as UIElement) ascending
                select item as UIElement).ToList();
            //sqh修改
            CommandBringBack.Run(CommManager, ordered);

            //int count = this.Children.Count;

            //for (int i = 0; i < ordered.Count; i++)
            //{
            //    int currentIndex = Canvas.GetZIndex(ordered[i]);
            //    int newIndex = Math.Max(i, currentIndex - 1);
            //    if (currentIndex != newIndex)
            //    {
            //        Canvas.SetZIndex(ordered[i], newIndex);
            //        IEnumerable<UIElement> it = this.Children.OfType<UIElement>().Where(Item => Canvas.GetZIndex(Item) == newIndex);

            //        foreach (UIElement elm in it)
            //        {
            //            if (elm != ordered[i])
            //            {
            //                Canvas.SetZIndex(elm, currentIndex);
            //                break;
            //            }
            //        }
            //    }
            //}
            //  OptStack.SaveOpt(this);
        }

        /// <summary>
        ///     置于底层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendToBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectionSorted = (from item in SelectionService.SelectedDesignerContainer
                orderby GetZIndex(item as UIElement) ascending
                select item as UIElement).ToList();

            var childrenSorted = (from UIElement item in Children
                orderby GetZIndex(item) ascending
                select item).ToList();
            //sqh修改
            CommandToBack.Run(CommManager, selectionSorted, childrenSorted);
            //int i = 0;
            //int j = 0;
            //foreach (UIElement Item in childrenSorted)
            //{
            //    int idx = Canvas.GetZIndex(Item);
            //    if (idx == int.MinValue) { continue; }
            //    if (selectionSorted.Contains(Item))
            //    {
            //        Canvas.SetZIndex(Item, j++);
            //    }
            //    else
            //    {
            //        Canvas.SetZIndex(Item, selectionSorted.Count + i++);
            //    }
            //}
            //OptStack.SaveOpt(this);
        }

        //AlignVerticalCenters Command
        private void AlignVerticalCenters_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                where item.ParentID == Guid.Empty
                select item;
            //sqh修改
            //CommandAlignVerticalCenter.Run(m_CommManager, selectedItems);
            CommandAlignHorizontalCenter.Run(CommManager, selectedItems);
            //if (selectedItems.Count() > 1)
            //{
            //    double bottom = Canvas.GetTop(selectedItems.First()) + selectedItems.First().Height / 2;

            //    foreach (DesignerContainer Item in selectedItems)
            //    {
            //        double delta = bottom - (Canvas.GetTop(Item) + Item.Height / 2);
            //        foreach (DesignerContainer di in SelectionService.GetGroupMembers(Item))
            //        {
            //            Canvas.SetTop(di, Canvas.GetTop(di) + delta);
            //        }
            //    }
            //}
            //OptStack.SaveOpt(this);
        }
        //AlignBottom Command
        private void AlignBottom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                where item.ParentID == Guid.Empty
                select item;
            CommandAlignBottom.Run(CommManager, selectedItems);
            //    if (selectedItems.Count() > 1)
            //    {
            //        double bottom = Canvas.GetTop(selectedItems.First()) + selectedItems.First().Height;

            //        foreach (DesignerContainer Item in selectedItems)
            //        {
            //            double delta = bottom - (Canvas.GetTop(Item) + Item.Height);
            //            foreach (DesignerContainer di in SelectionService.GetGroupMembers(Item))
            //            {
            //                Canvas.SetTop(di, Canvas.GetTop(di) + delta);
            //            }
            //        }
            //    }
            //    OptStack.SaveOpt(this);
        }

        //AlignLeft Command
        private void AlignLeft_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                where item.ParentID == Guid.Empty
                select item;
            CommandAlignLeft.Run(CommManager, selectedItems);
            //if (selectedItems.Count() > 1)
            //{
            //    double left = Canvas.GetLeft(selectedItems.First());

            //    foreach (DesignerContainer Item in selectedItems)
            //    {
            //        double delta = left - Canvas.GetLeft(Item);
            //        foreach (DesignerContainer di in SelectionService.GetGroupMembers(Item))
            //        {
            //            Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
            //        }
            //    }
            //}
            //OptStack.SaveOpt(this);
        }
        //AlignHorizontalCenters Command
        private void AlignHorizontalCenters_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                where item.ParentID == Guid.Empty
                select item;
            //sqh修改

            CommandAlignVerticalCenter.Run(CommManager, selectedItems);
            //if (selectedItems.Count() > 1)
            //{
            //    double center = Canvas.GetLeft(selectedItems.First()) + selectedItems.First().Width / 2;

            //    foreach (DesignerContainer Item in selectedItems)
            //    {
            //        double delta = center - (Canvas.GetLeft(Item) + Item.Width / 2);
            //        foreach (DesignerContainer di in SelectionService.GetGroupMembers(Item))
            //        {
            //            Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
            //        }
            //    }
            //}
            // OptStack.SaveOpt(this);
        }
        //AlignRight Command
        private void AlignRight_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                where item.ParentID == Guid.Empty
                select item;
            CommandAlignRight.Run(CommManager, selectedItems);
            //if (selectedItems.Count() > 1)
            //{
            //    double right = Canvas.GetLeft(selectedItems.First()) + selectedItems.First().Width;

            //    foreach (DesignerContainer Item in selectedItems)
            //    {
            //        double delta = right - (Canvas.GetLeft(Item) + Item.Width);
            //        foreach (DesignerContainer di in SelectionService.GetGroupMembers(Item))
            //        {
            //            Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
            //        }
            //    }
            //}
            //OptStack.SaveOpt(this);
        }
        //DistributeVertical Command
        private void DistributeVertical_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                where item.ParentID == Guid.Empty
                let itemTop = GetTop(item)
                orderby itemTop
                select item;
            //sqh修改
            CommandDistributeVertical.Run(CommManager, selectedItems);
            //if (selectedItems.Count() > 1)
            //{
            //    double top = Double.MaxValue;
            //    double bottom = Double.MinValue;
            //    double sumHeight = 0;
            //    foreach (DesignerContainer Item in selectedItems)
            //    {
            //        top = Math.Min(top, Canvas.GetTop(Item));
            //        bottom = Math.Max(bottom, Canvas.GetTop(Item) + Item.Height);
            //        sumHeight += Item.Height;
            //    }

            //    double distance = Math.Max(0, (bottom - top - sumHeight) / (selectedItems.Count() - 1));
            //    //double distance = (bottom - top - sumHeight) / (selectedItems.Count() - 1);
            //    double offset = Canvas.GetTop(selectedItems.First());

            //    foreach (DesignerContainer Item in selectedItems)
            //    {
            //        double delta = offset - Canvas.GetTop(Item);
            //        foreach (DesignerContainer di in SelectionService.GetGroupMembers(Item))
            //        {
            //            Canvas.SetTop(di, Canvas.GetTop(di) + delta);
            //        }
            //        if (sumHeight > this.Height - top)
            //        {
            //            offset += Math.Max(0, (bottom - top - selectedItems.Last().Height) / (selectedItems.Count() - 1));
            //        }
            //        else
            //        {
            //            offset = offset + Item.Height + distance;
            //        }
            //    }
            //}
            //OptStack.SaveOpt(this);
        }

        //更新背景
        private void ChangeBg_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var fd = new System.Windows.Forms.OpenFileDialog();
            fd.Filter = "png(.png)|*.png|jpg(.jpg)|*.jpg|bmp(.bmp)|*.bmp|all|*.*";
            fd.FilterIndex = 4;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                var filename = fd.FileName;
                var shortFileName = filename.Substring(filename.LastIndexOf("\\") + 1,
                    filename.Length - filename.LastIndexOf("\\") - 1);
                var newFileName = _path + "\\CustomImg\\";
                newFileName += shortFileName;
                try
                {
                    if (!Directory.Exists(_path + "\\CustomImg"))
                    {
                        Directory.CreateDirectory(_path + "\\CustomImg");
                    }
                    if (!File.Exists(newFileName))
                    {
                        File.Copy(filename, newFileName);
                    }
                    else
                    {
                        if (!filename.Replace(shortFileName, "").Equals(_path + "\\CustomImg\\"))
                        {
                            #region old

                            //if (MessageBoxResult.OK == MessageBox.Show("同名文件已存在,是否覆盖?", "警告", MessageBoxButton.OKCancel, MessageBoxImage.Warning))
                            //{
                            //    try
                            //    {
                            //        File.Delete(newFileName);
                            //        File.Copy(filename, newFileName);
                            //    }
                            //    catch(Exception ex)
                            //    {
                            //    }
                            //}
                            //else
                            //{
                            //    return;
                            //}

                            #endregion

                            if (MessageBoxResult.OK !=
                                MessageBox.Show("同名文件已存在,使用已有同名文件？", "警告", MessageBoxButton.OKCancel,
                                    MessageBoxImage.Warning))
                            {
                                MessageBox.Show("可更改图片名称后再选择!", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("设置图片失败!具体原因如下：\n" + ex.Message);
                    return;
                }

                CommandChangeBg.Run(CommManager, filename, newFileName);
                //foreach (var Item in this.Children.OfType<ISelectable>())
                //{
                //    DesignerContainer di = Item as DesignerContainer;
                //    int idx = Canvas.GetZIndex(di);
                //    if (idx == int.MinValue)
                //    {
                //        this.Children.Remove(di);
                //        this.BaissObject.Remove(di.BasisObject);
                //        break;
                //    }
                //}
                ////Image img = new Image();
                ////img.Source = new BitmapImage(new Uri(newFileName, UriKind.Relative));
                ////img.Stretch = Stretch.Fill;
                //CustomImage ci = new CustomImage();
                //ci.FileName = filename.Substring(filename.LastIndexOf("\\") + 1, filename.Length - filename.LastIndexOf("\\") - 1);
                //ci.Style = FindResource("ciStyle") as Style;
                //ci.NameVisible = false;
                ////ci.Width = (this.Parent as Viewbox).ActualWidth;
                ////ci.Height = (this.Parent as Viewbox).ActualHeight;
                //DesignerContainer diBg = new DesignerContainer();
                //diBg.Width = AppParams.CanvasWidth;// (this.Parent as Viewbox).ActualWidth;
                //diBg.Height = AppParams.CanvasHeight; //(this.Parent as Viewbox).ActualHeight;

                //diBg.Content = ci;
                //diBg.StyleName = "ciStyle";
                //diBg.Focusable = false;
                //diBg.IsEnabled = false;
                //diBg.IsHitTestVisible = false;
                //diBg.NameVisible = false;
                ////diBg.Width = img.Source.Width;
                ////diBg.Height = img.Source.Height;

                //Canvas.SetLeft(diBg, 0);
                //Canvas.SetTop(diBg, 0);
                //Canvas.SetZIndex(diBg, int.MinValue);
                //this.Children.Add(diBg);
            }
            // OptStack.SaveOpt(this);
        }
        //New Command
        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string fileName = _path + "\\CustomDiagram\\" + Application.Current.Properties["CurrentFile"] + ".xml";
            bool hasChange = false;
            if (string.IsNullOrEmpty(Application.Current.Properties["CurrentFile"].ToString()))
            {
                if (Children.OfType<DesignerContainer>().Count() > 0) hasChange = true;
            }
            else
            {
                XElement xe = XElement.Load(fileName);
                if (xe.Elements().First().Elements().Count() != Children.Count)
                {
                    hasChange = true;
                }
                else
                {
                    var canvasMd5 = MD5.Create();
                    var fileMd5 = canvasMd5.ComputeHash(Encoding.Default.GetBytes(xe.ToString()));
                    var DesignerContainers = Children.OfType<DesignerContainer>();
                    var DesignerContainersXML = DesignerContainers.ToXElement();
                    XElement root = new XElement("Root");
                    root.Add(DesignerContainersXML);
                    var notE = false;
                    var md5 = canvasMd5.ComputeHash(Encoding.Default.GetBytes(root.ToString()));
                    for (var i = 0; i < md5.Length; i++)
                    {
                        if (md5[i] != fileMd5[i])
                        {
                            notE = true;
                            break;
                        }
                    }
                    if (notE)
                    {
                        hasChange = true;
                    }
                    //Application.Current.Properties["FileMD5"] = md5;
                }
            }
            if (hasChange)
            {
                if (MessageBox.Show("文件已改变,是否保存?", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SaveFile();
                }
            }
            Application.Current.Properties.Remove("Polyline");
            Application.Current.Properties.Remove("DrawMode");

            Children.Clear();
            SelectionService.ClearSelection();
            // this.BaissObject.Clear();
            // OptStack.SaveOpt(this);
        }
        private void New_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            // e.CanExecute = Application.Current.Properties["Mode"].Equals(CommonParam.MODE.EDIT);
        }
        
        //Open Command
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            XElement root = LoadSerializedDataFromFile();
            //if (root == null) return;
            CommandOpen.Run(CommManager, root);
        }
        private void Open_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.DesignMode == MODE.EDIT;  
        }

        //Save Command
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFile();
        }
        private void Save_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DesignMode.Equals(MODE.EDIT);
        }

        //Copy Command
        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedDesignerContainers =
                SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>();
            CommandCopy.Run(CommManager, selectedDesignerContainers);
            //CopyCurrentSelection();
        }
        private void Copy_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.SelectedDesignerContainer.Count() > 0;
        }

        //Paste Command
        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //IEnumerable<DesignerContainer> selectedDesignerContainers =
            //  this.SelectionService.CurrentSelection.OfType<DesignerContainer>();
            CommandPaste.Run(CommManager, null);

            #region old paste command

            //XElement root = LoadSerializedDataFromClipBoard();

            //if (root == null)
            //    return;

            //// create DesignerContainers
            //Dictionary<Guid, Guid> mappingOldToNewIDs = new Dictionary<Guid, Guid>();
            //List<ISelectable> newItems = new List<ISelectable>();
            //IEnumerable<XElement> itemsXML = root.Elements("DesignerContainers").Elements("DesignerContainer");

            //double offsetX = Double.Parse(root.Attribute("OffsetX").Value, CultureInfo.InvariantCulture);
            //double offsetY = Double.Parse(root.Attribute("OffsetY").Value, CultureInfo.InvariantCulture);
            //double speedX = 10;
            //double speedY = 10;

            //foreach (XElement itemXML in itemsXML)
            //{
            //    Guid oldID = new Guid(itemXML.Element("ID").Value);
            //    Guid newID = Guid.NewGuid();
            //    mappingOldToNewIDs.Add(oldID, newID);
            //    //DesignerContainer Item = DeserializeDesignerContainer(itemXML, newID, offsetX, offsetY);

            //    try
            //    {
            //        DesignerContainer Item = DeserializeDesignerContainer(itemXML, newID);
            //        Item._name = CreateNewName(Item);

            //        this.Children.Add(Item);
            //        //SetConnectorDecoratorTemplate(Item);
            //        newItems.Add(Item);
            //        double newLeft = Canvas.GetLeft(Item) + offsetX;
            //        double newTop = Canvas.GetTop(Item) + offsetY;
            //        if (newLeft + Item.Width + 10 > this.Width)
            //        {
            //            speedX = -10;
            //            //newLeft = Canvas.GetLeft(Item) - offsetX;
            //        }
            //        if (newTop + Item.Height + 10 > this.Height)
            //        {
            //            speedY = -10;
            //            //newTop = Canvas.GetTop(Item) - offsetY;
            //        }
            //        Canvas.SetLeft(Item, newLeft);
            //        Canvas.SetTop(Item, newTop);
            //    }
            //    catch
            //    {
            //        continue;
            //    }
            //}

            //// update group hierarchy
            //SelectionService.ClearSelection();
            //foreach (DesignerContainer el in newItems)
            //{
            //    if (el.ParentID != Guid.Empty)
            //        el.ParentID = mappingOldToNewIDs[el.ParentID];
            //}


            //foreach (DesignerContainer Item in newItems)
            //{
            //    if (Item.ParentID == Guid.Empty)
            //    {
            //        SelectionService.AddToSelection(Item);
            //    }
            //}

            //DesignerCanvas.BringToFront.Execute(null, this);
            //PropertyView pv = this.FindName("pv") as PropertyView;
            //if (pv != null) pv.ShowProView();
            //OptStack.SaveOpt(this);

            //// update paste offset
            //root.Attribute("OffsetX").Value = (offsetX + speedX).ToString();
            //root.Attribute("OffsetY").Value = (offsetY + speedY).ToString();
            //Clipboard.Clear();
            //Clipboard.SetData(DataFormats.Xaml, root);

            #endregion
        }
        public string CreateNewName(DesignerContainer di)
        {
            var name = "";
            name = name.Length > 4 ? name.Substring(0, 4) : name;
            if (string.IsNullOrEmpty(name))
            {
                name = "新建图元";
            }
            index = 0;
            var nameList = new Dictionary<string, Guid>();
            foreach (DesignerContainer item in Children)
            {
                if (item != null)
                {
                    nameList.Add(item.Name, item.ID);
                }
            }
            while (nameList.ContainsKey(name + index))
            {
                index++;
            }
            name += index.ToString();
            return name;
        }
        private void Paste_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsData(DataFormats.Xaml);
        }

        //Delete Command
        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //sqh修改
            //DeleteCurrentSelection();
            var list = SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>();
            CommandDelete.Run(CommManager, list);
        }
        private void Delete_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.SelectedDesignerContainer.Count() > 0;
        }

        //Cut Command
        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //sqh修改  


            CopyCurrentSelection();
            DeleteCurrentSelection();
        }
        private void Cut_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.SelectedDesignerContainer.Count() > 0;
        }

        //组合命令
        private void Group_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var items = from item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                where item.ParentID == Guid.Empty
                select item;
            //sqh修改
            CommandGroup.Run(CommManager, items);
            //m_CommManager.Run<CommandGroup>(items);
            //m_CommManager.m_UndoList.Push(m_CommManager);
            //Rect rect = GetBoundingRectangle(items);

            //DesignerContainer groupItem = new DesignerContainer();
            //groupItem.IsGroup = true;
            //groupItem.Width = rect.Width;
            //groupItem.Height = rect.Height;
            //Canvas.SetLeft(groupItem, rect.Left);
            //Canvas.SetTop(groupItem, rect.Top);
            //Canvas groupCanvas = new Canvas();

            //groupItem.Content = groupCanvas;
            //Canvas.SetZIndex(groupItem, this.Children.Count);
            //this.Children.Add(groupItem);

            //foreach (DesignerContainer Item in items)
            //Item.ParentID = groupItem.ID;

            //this.SelectionService.SelectItem(groupItem);
            //groupItem.NameVisible = false;
            //PropertyView pv = this.FindName("pv") as PropertyView;
            //if (pv != null) pv.ShowProView();
            //OptStack.SaveOpt(this);
        }
        private void Group_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var count = (from item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                where item.ParentID == Guid.Empty
                select item).Count();

            e.CanExecute = count > 1;
        }

        //锁定命令
        private void Lock_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedDesignerContainers =
                SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>();
            //var items = from Item in this.SelectionService.CurrentSelection.OfType<DesignerContainer>()
            //where Item.ParentID == Guid.Empty
            //select Item;
            //sqh修改
            foreach (var item in selectedDesignerContainers)
            {
                item.Lock = true;
            }
        }

        public delegate void InformCreate(bool flag);

        public static event InformCreate GetInformCreteInfo;

        public void GetCreate(bool f)
        {
            if (GetInformCreteInfo != null)
                GetInformCreteInfo(f);
        }
        private void Lock_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var count = (from item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                where !item.LockDesign
                select item).Count();

            e.CanExecute = count >= 1;
        }
        private void DeleteColumn_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedDesignerContainers =
                SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>();
            //sqh修改
            foreach (var item in selectedDesignerContainers)
            {
                if (item.StyleName == "StyPicFire")
                {
                    // ((PicFire)Item.Item).VisibleColumnCreate = true;
                    GetCreate(true);
                }
                //Item.LockDesign = false;
            }
        }
        private void DeleteColumn_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var count = (from item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                select item).Count();

            e.CanExecute = count >= 1;
        }

        //解锁命令
        private void UnLock_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedDesignerContainers =
                SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>();
            //sqh修改
            foreach (var item in selectedDesignerContainers)
            {
                item.LockDesign = false;
            }
        }
        private void UnLock_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var count = (from item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                where item.LockDesign
                select item).Count();

            e.CanExecute = count >= 1;
        }

        #region Ungroup Command

        private void Ungroup_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var groups = (from item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                where item.IsGroup && item.ParentID == Guid.Empty
                select item).ToArray();
            //sqh修改
            CommandUnGroup.Run(CommManager, groups);
        }

        private void Ungroup_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var groupedItem = from item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                where item.ParentID != Guid.Empty
                select item;


            e.CanExecute = groupedItem.Count() > 0;
        }

        #endregion

        #region BringForward Command

        /// <summary>
        ///     上移一层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BringForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var ordered = (from item in SelectionService.SelectedDesignerContainer
                orderby GetZIndex(item as UIElement) descending
                select item as UIElement).ToList();
            //sqh修改
            CommandBringForward.Run(CommManager, ordered);
            //int count = this.Children.Count;

            //for (int i = 0; i < ordered.Count; i++)
            //{
            //    int currentIndex = Canvas.GetZIndex(ordered[i]);
            //    int newIndex = Math.Min(count - 1 - i, currentIndex + 1);
            //    if (currentIndex != newIndex)
            //    {
            //        Canvas.SetZIndex(ordered[i], newIndex);
            //        IEnumerable<UIElement> it = this.Children.OfType<UIElement>().Where(Item => Canvas.GetZIndex(Item) == newIndex);

            //        foreach (UIElement elm in it)
            //        {
            //            if (elm != ordered[i])
            //            {
            //                Canvas.SetZIndex(elm, currentIndex);
            //                break;
            //            }
            //        }
            //    }
            //}
            //OptStack.SaveOpt(this);
        }

        private void Order_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = SelectionService.CurrentSelection.Count() > 0;
            e.CanExecute = true;
        }

        #endregion

        #region AlignTop Command

        private void AlignTop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                where item.ParentID == Guid.Empty
                select item;
            //sqh修改
            CommandAlignTop.Run(CommManager, selectedItems);
            //if (selectedItems.Count() > 1)
            //{
            //    double top = Canvas.GetTop(selectedItems.First());

            //    foreach (DesignerContainer Item in selectedItems)
            //    {
            //        double delta = top - Canvas.GetTop(Item);
            //        foreach (DesignerContainer di in SelectionService.GetGroupMembers(Item))
            //        {
            //            Canvas.SetTop(di, Canvas.GetTop(di) + delta);
            //        }
            //    }
            //}
            //OptStack.SaveOpt(this);
        }

        private void Align_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //var groupedItem = from Item in SelectionService.CurrentSelection.OfType<DesignerContainer>()
            //                  where Item.ParentID == Guid.Empty
            //                  select Item;


            //e.CanExecute = groupedItem.Count() > 1;
            e.CanExecute = true;
        }

        #endregion

        #region DistributeHorizontal Command

        private void DistributeHorizontal_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                where item.ParentID == Guid.Empty
                let itemLeft = GetLeft(item)
                orderby itemLeft
                select item;
            //sqh修改
            CommandDistributeHorizontal.Run(CommManager, selectedItems);
            //if (selectedItems.Count() > 1)
            //{
            //    double left = Double.MaxValue;
            //    double right = Double.MinValue;
            //    double sumWidth = 0;
            //    foreach (DesignerContainer Item in selectedItems)
            //    {
            //        left = Math.Min(left, Canvas.GetLeft(Item));
            //        right = Math.Max(right, Canvas.GetLeft(Item) + Item.Width);
            //        sumWidth += Item.Width;
            //    }

            //    double distance = Math.Max(0, (right - left - sumWidth) / (selectedItems.Count() - 1));
            //    //double distance = (right - left - sumWidth) / (selectedItems.Count() - 1);
            //    double offset = Canvas.GetLeft(selectedItems.First());

            //    foreach (DesignerContainer Item in selectedItems)
            //    {
            //        double delta = offset - Canvas.GetLeft(Item);
            //        foreach (DesignerContainer di in SelectionService.GetGroupMembers(Item))
            //        {
            //            Canvas.SetLeft(di, Convert.ToInt32(Canvas.GetLeft(di) + delta));
            //        }
            //        if (sumWidth > this.Width - left)
            //        {
            //            offset += Math.Max(0, (right - left - selectedItems.Last().Width) / (selectedItems.Count() - 1));
            //        }
            //        else
            //        {
            //            offset = offset + Item.Width + distance;
            //        }
            //    }
            //}
            //OptStack.SaveOpt(this);
        }

        private void Distribute_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //var groupedItem = from Item in SelectionService.CurrentSelection.OfType<DesignerContainer>()
            //                  where Item.ParentID == Guid.Empty
            //                  select Item;


            //e.CanExecute = groupedItem.Count() > 1;
            e.CanExecute = true;
        }

        #endregion

        #region SelectAll Command

        private void SelectAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectionService.SelectAll();

            var pv = FindName("pv") as PropertyView;
            if (pv != null) pv.ShowProView(false);
        }

        private void SelectAll_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            // e.CanExecute = Application.Current.Properties["Mode"].Equals(CommonParam.MODE.EDIT);
        }

        #endregion

        #region Helper Methods

        #region 注释导入导出方法

        /// <summary>
        ///     导出
        /// </summary>
        //public void Export(string name,Boolean blen)
        //{
        //    try
        //    {
        //        //System.Windows.Forms.FolderBrowserDialog fold = new System.Windows.Forms.FolderBrowserDialog();

        //        //if (System.Windows.Forms.DialogResult.Cancel == fold.ShowDialog())
        //        //{
        //        //    return;
        //        //}
        //        int m = 0;
        //        string m_path = "";
        //        string N_name = "";
        //        if (blen)
        //        {
        //            m_path = path;
        //            N_name = "\\Backup" + name;
        //            while (Directory.Exists(m_path + N_name))
        //            {

        //                Directory.Delete(m_path + N_name);

        //            }
        //        }
        //        else
        //        {
        //            m_path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        //            N_name = "\\Export" + DateTime.Now.Millisecond.ToString();
        //            while (Directory.Exists(m_path + N_name))
        //            {
        //                N_name = "\\Export" + DateTime.Now.Millisecond.ToString();
        //                continue;
        //            }
        //        }
        //        m_path += N_name;
        //        Directory.CreateDirectory(m_path);
        //        string expName = path + "List.zd";
        //        string[] subDir = Directory.GetDirectories(path);
        //        List<string> allFile = new List<string>();
        //        List<string> m_list = new List<string>();
        //        List<string> n_list = new List<string>();
        //        n_list.Add("BackupStart");
        //        n_list.Add("BackupRun");
        //        n_list.Add("BackupEnd");
        //        m_list.Add("CustomDiagram");
        //        m_list.Add("CustomImg");
        //        m_list.Add("Images");
        //        m_list.Add("DeviceConfigFileName.xml");
        //        m_list.Add("DeviceLinkageFileName.xml");
        //        m_list.Add("DeviceMateName.xml");
        //        m_list.Add("DictionaryMenuf.xml");
        //        m_list.Add("DictionaryMenus.xml");
        //        m_list.Add("DictionaryMenut.xml");
        //        m_list.Add("LinkageRuleh.xml");
        //        m_list.Add("Pageconfig.xml");
        //        m_list.Add("MateLinkageConfig.xml");
        //        if (File.Exists(expName))
        //        {
        //            File.Delete(expName);
        //        }
        //        foreach (var dir in subDir)
        //        {
        //            string l_name = dir.Substring(dir.LastIndexOf("\\"), dir.Length - dir.LastIndexOf("\\")).Replace("\\","");
        //            if (!n_list.Contains(l_name))
        //            {
        //                if (Directory.Exists(dir))
        //                {
        //                    if (Directory.GetDirectories(dir).Count() == 0)
        //                    {
        //                        allFile.AddRange(Directory.GetFiles(dir));
        //                    }
        //                    else
        //                    {
        //                        foreach (var Item in Directory.GetDirectories(dir))
        //                        {
        //                            allFile.AddRange(Directory.GetFiles(Item));
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        allFile.AddRange(Directory.GetFiles(path));
        //        FileInfo fi = new FileInfo(expName);

        //        using (StreamWriter sw = fi.AppendText())
        //        {
        //            m_list.ForEach((x) =>
        //            {
        //                List<string> file = allFile.FindAll((y) =>
        //                {
        //                    return y.Contains(x);
        //                });
        //                if (file != null)
        //                {
        //                    for (int i = 0; i < file.Count; i++)
        //                    {
        //                        sw.WriteLine(file[i].Replace(path, ""));
        //                    }
        //                }
        //            });
        //            sw.Flush();
        //        }
        //        string OldZDname = System.Windows.Application.Current.Properties["CurrentZDName"].ToString();
        //        string[] filename = File.ReadAllLines(expName);
        //        string[] st_Delete = Directory.GetFiles(m_path);
        //        foreach (var Item in st_Delete)
        //        {
        //            System.IO.File.Delete(Item);
        //        }
        //        foreach (var Item in filename)
        //        {
        //            string[] temp = Item.Split('\\');
        //            if (temp.Length > 2)
        //            {
        //                if (!Directory.Exists(m_path + "\\" + temp[0]))
        //                {
        //                    Directory.CreateDirectory(m_path + "\\" + temp[0]);
        //                }
        //                if (!Directory.Exists(m_path + "\\" + temp[1]))
        //                {
        //                    Directory.CreateDirectory(m_path + "\\" + temp[0]+ "\\"+temp[1]);
        //                }
        //                File.Copy(expName.Substring(0, expName.LastIndexOf("\\") + 1) + Item, m_path + "\\" + Item, true);
        //            }
        //            else if (temp.Length > 1)
        //            {
        //                if (!Directory.Exists(m_path + "\\" + temp[0]))
        //                {
        //                    Directory.CreateDirectory(m_path + "\\" + temp[0]);
        //                }

        //            }
        //            File.Copy(expName.Substring(0, expName.LastIndexOf("\\") + 1) + Item, m_path + "\\" + Item, true);

        //        }
        //        if (!blen)
        //        {
        //            allFile = new List<string>();
        //            string m_expName = m_path + "\\List.zd";
        //            string[] m_subDir = Directory.GetDirectories(m_path);
        //            List<string> m_allFile = new List<string>();
        //            if (File.Exists(m_expName))
        //            {
        //                File.Delete(m_expName);
        //            }
        //            foreach (var m_dir in m_subDir)
        //            {
        //                if (Directory.Exists(m_dir))
        //                {
        //                    if (Directory.GetDirectories(m_dir)!=null&&Directory.GetDirectories(m_dir).Count() == 0)
        //                    {
        //                        allFile.AddRange(Directory.GetFiles(m_dir));
        //                    }
        //                    else
        //                    {
        //                        foreach (var items in Directory.GetDirectories(m_dir))
        //                        {
        //                            allFile.AddRange(Directory.GetFiles(items));
        //                        }
        //                    }
        //                }
        //            }
        //            allFile.AddRange(Directory.GetFiles(m_path));
        //            FileInfo m_fi = new FileInfo(m_expName);
        //            using (StreamWriter sw = m_fi.AppendText())
        //            {
        //                sw.WriteLine(OldZDname);
        //                foreach (string file in allFile)
        //                {
        //                    sw.WriteLine(file.Replace(m_path + "\\", ""));
        //                    sw.Flush();
        //                }

        //            }

        //                MessageBox.Show("导出成功,已导入桌面" + N_name + "文件夹");

        //        }

        //    }
        //    catch
        //    {

        //    }
        //}
        ////public void Import()
        ////{
        ////    string expName = "";
        ////    OpenFileDialog impFile = new OpenFileDialog();
        ////    impFile.Filter = "zd Files (*.zd)|*.zd|All Files (*.*)|*.*";
        ////    if (impFile.ShowDialog() == true)
        ////    {
        ////        expName = impFile.FileName;
        ////    }
        ////    Import(expName,AppParams.CurrentZDName);
        ////}
        ///// <summary>
        ///// 导入
        ///// </summary>
        //public void Import(string expName, string ZDname, string pageName, string GoldPage)
        //{
        //    if (string.IsNullOrEmpty(expName))
        //    {
        //        MessageBox.Show("导入路径不能为空！");
        //        return;
        //    }
        //    FileInfo fi = new FileInfo(expName);
        //    if (!fi.Exists)
        //    {
        //        MessageBox.Show("文件不存在！");
        //        return;
        //    }
        //    string[] filename = File.ReadAllLines(expName);
        //    #region 非页面导入
        //    if (string.IsNullOrEmpty(pageName))
        //    {
        //        try
        //        {

        //            if (path == expName.Substring(0, expName.LastIndexOf("\\")))
        //            {
        //                MessageBox.Show("同个ZD转换，请查看");

        //            }
        //            else
        //            {
        //                string st = "";
        //                if (ZDname != null)
        //                {
        //                    st = ZDname;
        //                }
        //                else
        //                {
        //                    st = "";
        //                }
        //                //导入存在ZD的工程
        //                #region 导入存在ZD的工程
        //                if (st != "")
        //                {
        //                    string[] st_Delete = Directory.GetFiles(path + ZDname);
        //                    foreach (var Item in st_Delete)
        //                    {
        //                        if (!Item.Contains("DictionaryMenuf.xml") && !Item.Contains("DictionaryMenus.xml") && !Item.Contains("DictionaryMenut.xml"))
        //                        {
        //                            System.IO.File.Delete(Item);
        //                        }

        //                    }
        //                    foreach (var Item in Directory.GetDirectories(path + ZDname))
        //                    {
        //                        Directory.Delete(Item, true);

        //                    }

        //                    for (int i = 0; i < filename.Length; i++)
        //                    {
        //                        //不导入菜单
        //                        if (filename[i].Contains("DictionaryMenuf.xml") || filename[i].Contains("DictionaryMenus.xml") || filename[i].Contains("DictionaryMenut.xml"))
        //                        {
        //                            continue;
        //                        }

        //                        string OldZDname = filename[0].ToString();
        //                        if (!string.IsNullOrEmpty(filename[0]))
        //                        {
        //                            if (i != 0)
        //                            {
        //                                try
        //                                {

        //                                    string[] temp = filename[i].Split('\\');

        //                                    if (temp.Length > 2)
        //                                    {
        //                                        if (!Directory.Exists(path + ZDname + "\\" + temp[0]))
        //                                        {
        //                                            Directory.CreateDirectory(path + ZDname + "\\" + temp[0]);
        //                                        }
        //                                        if (!Directory.Exists(path + ZDname + "\\" + temp[1]))
        //                                        {
        //                                            Directory.CreateDirectory(path + ZDname + "\\" + temp[0] + "\\" + temp[1]);
        //                                        }
        //                                        //File.Copy(expName.Substring(0, expName.LastIndexOf("\\") + 1) + filename[i], path + "\\" + filename[i], true);
        //                                    }
        //                                    else if (temp.Length > 1)
        //                                    {
        //                                        if (!Directory.Exists(path + ZDname + "\\" + temp[0]))
        //                                        {
        //                                            Directory.CreateDirectory(path + ZDname + "\\" + temp[0]);
        //                                        }

        //                                    }
        //                                    File.Copy(expName.Substring(0, expName.LastIndexOf("\\") + 1) + filename[i], path + ZDname + "\\" + filename[i], true);
        //                                    XmlDocument myDoc = new XmlDocument();
        //                                    myDoc.Load(path + ZDname + "\\" + filename[i]);
        //                                    myDoc.InnerXml = myDoc.InnerXml.Replace(OldZDname, st);
        //                                    // myDoc.InnerXml=myDoc.InnerXml.Replace("变量.", "变量." + System.Windows.Application.Current.Properties["CurrentZDName"].ToString() + ".");
        //                                    myDoc.Save(path + ZDname + "\\" + filename[i]);
        //                                }
        //                                catch
        //                                {
        //                                    continue;
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (i != 0)
        //                            {
        //                                string[] temp = filename[i].Split('\\');

        //                                if (temp.Length > 2)
        //                                {
        //                                    if (!Directory.Exists(path + ZDname + "\\" + temp[0]))
        //                                    {
        //                                        Directory.CreateDirectory(path + ZDname + "\\" + temp[0]);
        //                                    }
        //                                    if (!Directory.Exists(path + ZDname + "\\" + temp[1]))
        //                                    {
        //                                        Directory.CreateDirectory(path + ZDname + "\\" + temp[0] + "\\" + temp[1]);
        //                                    }
        //                                    //File.Copy(expName.Substring(0, expName.LastIndexOf("\\") + 1) + filename[i], path + "\\" + filename[i], true);
        //                                }
        //                                else if (temp.Length > 1)
        //                                {
        //                                    if (!Directory.Exists(path + ZDname + "\\" + temp[0]))
        //                                    {
        //                                        Directory.CreateDirectory(path + ZDname + "\\" + temp[0]);
        //                                    }

        //                                }
        //                                File.Copy(expName.Substring(0, expName.LastIndexOf("\\") + 1) + filename[i], path + ZDname + "\\" + filename[i], true);
        //                                if (filename[i].Equals("DeviceConfigFileName.xml"))
        //                                {
        //                                    XElement myDoc = XElement.Load((path + ZDname + "\\" + filename[i]));

        //                                    XElement zdgc = new XElement("Zbgc", from ele in myDoc.Elements() select ele);
        //                                    zdgc.SetAttributeValue("_name", st);
        //                                    zdgc.SetAttributeValue("Dh", "");
        //                                    zdgc.SetAttributeValue("Nm", "");
        //                                    myDoc.RemoveAll();
        //                                    myDoc.Add(zdgc);
        //                                    myDoc.SetAttributeValue("version", "1.1.0");
        //                                    myDoc.Save(path + ZDname + "\\" + filename[i]);
        //                                }
        //                                else if (filename[i].Contains(".xml"))
        //                                {
        //                                    XmlDocument myDoc = new XmlDocument();

        //                                    myDoc.Load(path + ZDname + "\\" + filename[i]);
        //                                    myDoc.InnerXml = myDoc.InnerXml.Replace("设备.", "设备." + st + ".");
        //                                    myDoc.InnerXml = myDoc.InnerXml.Replace("变量.", "变量." + st + ".");
        //                                    myDoc.InnerXml = myDoc.InnerXml.Replace("绘图区", "绘图区" + st);
        //                                    myDoc.Save(path + ZDname + "\\" + filename[i]);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                #endregion
        //                #region 不存在ZD
        //                else
        //                {
        //                    Boolean boolen = false ;
        //                    foreach (var itemss in filename)
        //                    {
        //                        if (itemss.ToString().Contains("Images"))
        //                        {
        //                            foreach (var Item in Directory.GetDirectories(path))
        //                            {
        //                                if (Item.Contains("Images"))
        //                                {

        //                                    try
        //                                    {
        //                                        Directory.Delete(Item, true);
        //                                        boolen = true;
        //                                    }
        //                                    catch
        //                                    {
        //                                        continue;
        //                                    }
        //                                }

        //                            }
        //                        }
        //                    }
        //                    foreach (var Item in Directory.GetDirectories(path))
        //                    {
        //                        if (Item.Contains("CustomDiagram") || Item.Contains("CustomImg"))
        //                        {

        //                            try
        //                            {
        //                                Directory.Delete(Item, true);
        //                            }
        //                            catch
        //                            {
        //                                continue;
        //                            }
        //                        }

        //                    }
        //                    string[] subDir = Directory.GetDirectories(path);
        //                    List<string> allFile = new List<string>();

        //                    foreach (var dir in subDir)
        //                    {
        //                        if (Directory.Exists(dir))
        //                        {
        //                            if (Directory.GetDirectories(dir) != null && Directory.GetDirectories(dir).Count() == 0)
        //                            {
        //                                allFile.AddRange(Directory.GetFiles(dir));
        //                            }
        //                            else
        //                            {
        //                                foreach (var items in Directory.GetDirectories(dir))
        //                                {
        //                                    allFile.AddRange(Directory.GetFiles(items));
        //                                }
        //                            }
        //                        }
        //                    }
        //                    allFile.AddRange(Directory.GetFiles(path));
        //                    List<string> m_list = new List<string>();
        //                    m_list.Add("CustomDiagram");
        //                    m_list.Add("CustomImg");
        //                    m_list.Add("Images");
        //                    m_list.Add("DeviceConfigFileName.xml");
        //                    m_list.Add("DeviceLinkageFileName.xml");
        //                    m_list.Add("DeviceMateName.xml");
        //                    m_list.Add("DictionaryMenuf.xml");
        //                    m_list.Add("DictionaryMenus.xml");
        //                    m_list.Add("DictionaryMenut.xml");
        //                    m_list.Add("LinkageRuleh.xml");
        //                    m_list.Add("MateLinkageConfig.xml");
        //                    m_list.ForEach((x) =>
        //                    {
        //                        List<string> file = allFile.FindAll((y) =>
        //                        {
        //                            return y.Contains(x);
        //                        });
        //                        if (file != null)
        //                        {
        //                            foreach (var Item in file)
        //                            {
        //                                if (Item.ToString().Contains("Images") && boolen)
        //                                {
        //                                    if (!Item.Contains("DictionaryMenuf.xml") && !Item.Contains("DictionaryMenus.xml") && !Item.Contains("DictionaryMenut.xml"))
        //                                    {
        //                                        System.IO.File.Delete(Item);
        //                                    }
        //                                }

        //                            }
        //                        }
        //                    });

        //                    for (int i = 0; i < filename.Length; i++)
        //                    {
        //                        //不导入菜单
        //                        if (filename[i].Contains("DictionaryMenuf.xml") || filename[i].Contains("DictionaryMenus.xml") || filename[i].Contains("DictionaryMenut.xml"))
        //                        {
        //                            continue;
        //                        }

        //                        string OldZDname = filename[0].ToString();
        //                        if (!string.IsNullOrEmpty(filename[0]))
        //                        {
        //                            if (i != 0)
        //                            {
        //                                try
        //                                {

        //                                    string[] temp = filename[i].Split('\\');
        //                                    if (temp.Length > 2)
        //                                    {
        //                                        if (!Directory.Exists(path + "\\" + temp[0]))
        //                                        {
        //                                            Directory.CreateDirectory(path + "\\" + temp[0]);
        //                                        }
        //                                        if (!Directory.Exists(path + "\\" + temp[1]))
        //                                        {
        //                                            Directory.CreateDirectory(path + "\\" + temp[0] + "\\" + temp[1]);
        //                                        }
        //                                        //File.Copy(expName.Substring(0, expName.LastIndexOf("\\") + 1) + filename[i], path + "\\" + filename[i], true);
        //                                    }
        //                                    else if (temp.Length > 1)
        //                                    {
        //                                        if (!Directory.Exists(path + "\\" + temp[0]))
        //                                        {
        //                                            Directory.CreateDirectory(path + "\\" + temp[0]);
        //                                        }

        //                                    }

        //                                    File.Copy(expName.Substring(0, expName.LastIndexOf("\\") + 1) + filename[i], path + "\\" + filename[i], true);
        //                                    XmlDocument myDoc = new XmlDocument();
        //                                    myDoc.Load(path + "\\" + filename[i]);
        //                                    myDoc.InnerXml = myDoc.InnerXml.Replace("." + OldZDname, "");
        //                                    myDoc.InnerXml = myDoc.InnerXml.Replace(OldZDname, "");

        //                                    myDoc.Save(path + "\\" + filename[i]);
        //                                }
        //                                catch
        //                                {
        //                                    continue;
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (i != 0)
        //                            {
        //                                string[] temp = filename[i].Split('\\');
        //                                if (temp.Length > 2)
        //                                {
        //                                    if (!Directory.Exists(path + "\\" + temp[0]))
        //                                    {
        //                                        Directory.CreateDirectory(path + "\\" + temp[0]);
        //                                    }
        //                                    if (!Directory.Exists(path + "\\" + temp[1]))
        //                                    {
        //                                        Directory.CreateDirectory(path + "\\" + temp[0] + "\\" + temp[1]);
        //                                    }
        //                                    //File.Copy(expName.Substring(0, expName.LastIndexOf("\\") + 1) + filename[i], path + "\\" + filename[i], true);
        //                                }
        //                                else if (temp.Length > 1)
        //                                {
        //                                    if (!Directory.Exists(path + "\\" + temp[0]))
        //                                    {
        //                                        Directory.CreateDirectory(path + "\\" + temp[0]);
        //                                    }

        //                                } 
        //                                File.Copy(expName.Substring(0, expName.LastIndexOf("\\") + 1) + filename[i], path + "\\" + filename[i], true);
        //                            }
        //                        }
        //                    }
        //                }
        //                #endregion
        //                MessageBox.Show("导入成功");
        //                //Application.Current.Shutdown();
        //            }
        //        }
        //        catch { MessageBox.Show("导入失败或导入被中断"); }
        //    }
        //    #endregion
        //    #region 页面导入
        //    else
        //    {
        //        try
        //        {
        //            string st = "";
        //            if (ZDname != null)
        //            {
        //                st = ZDname;
        //            }
        //            else
        //            {
        //                st = "";
        //            }
        //            string m_ZDname = object.Equals(ZDname, null) ? "" : (ZDname + "\\");
        //            string[] subDir = Directory.GetDirectories(path);
        //            List<string> allFile = new List<string>();

        //            foreach (var dir in subDir)
        //            {
        //                if (Directory.Exists(dir))
        //                {
        //                    allFile.AddRange(Directory.GetFiles(dir));
        //                }
        //            }
        //            allFile.AddRange(Directory.GetFiles(path));
        //            List<string> m_list = new List<string>();
        //            string[] st_Delete = Directory.GetFiles(path);
        //            foreach (var Item in allFile)
        //            {
        //                if (Item.Contains(GoldPage))
        //                {
        //                    System.IO.File.Delete(Item);
        //                }

        //            }
        //            string path_dxk = expName.Substring(0, expName.LastIndexOf("\\") + 1);
        //            Dictionary<string, string> MateDevname = new Dictionary<string, string>();
        //            List<string> GoldMateDevname = new List<string>();
        //            Dictionary<string, string> oldGoldMateDevname = new Dictionary<string, string>();
        //            //Dictionary<string, string> MateLinkageDevname = new Dictionary<string, string>();
        //            List<string> m_SourceName = new List<string>();
        //            List<string> m_Targetname = new List<string>();
        //            List<string> m_SourceName1 = new List<string>();
        //            List<string> m_Targetname1 = new List<string>();
        //            // Dictionary<string, string> MateLinkageDevname1 = new Dictionary<string, string>();
        //            XmlManager node_XmlManager = new XmlManager(path_dxk + "DeviceMateName.xml");
        //            string st_path = pageName.Substring(0, pageName.LastIndexOf("."));
        //            string GoldName = GoldPage.Substring(0, GoldPage.LastIndexOf("."));
        //            foreach (XElement node_item in node_XmlManager.Selects("DeviceConfig"))
        //            {
        //                string Source = node_XmlManager.Attribute(node_item, "Device").Value;
        //                string Meta = node_XmlManager.Attribute(node_item, "Meta").Value;
        //                if (Meta.Contains(st_path))
        //                {
        //                    MateDevname.Add(Meta, Source);
        //                }
        //            }

        //            MessageBoxResult Result = MessageBox.Show("是否覆盖已存在配置", "警告", MessageBoxButton.YesNoCancel);
        //            if (Result == MessageBoxResult.No)
        //            {
        //                if (MateDevname.Count == 0)
        //                {
        //                    File.Copy(expName.Substring(0, expName.LastIndexOf("\\") + 1) + "CustomDiagram\\" + pageName, path + ZDname + "CustomDiagram\\" + GoldPage, true);
        //                    MessageBox.Show("导入成功");
        //                    //Application.Current.Shutdown();
        //                    return;
        //                }
        //            }
        //            else if (Result == MessageBoxResult.Cancel)
        //            {
        //                return;
        //            }
        //            #region DeviceMateName

        //            for (int i = 0; i < filename.Length; i++)
        //            {
        //                if (filename[i].Contains("DeviceMateName.xml"))
        //                {

        //                    XmlManager m_XmlManager = new XmlManager(path + m_ZDname + filename[i]);
        //                    XmlManager m_XmlManagers = new XmlManager(expName.Substring(0, expName.LastIndexOf("\\") + 1) + filename[i]);
        //                    string OldZDname = filename[0].ToString();
        //                    if (m_XmlManager.Selects("DeviceConfig") != null)
        //                    {
        //                        //删掉和目标页面相同页面的DeviceMateName 关联并记录
        //                        foreach (XElement d_item in m_XmlManager.Selects("DeviceConfig"))
        //                        {
        //                            //d_item.Attribute("Device").Value = d_item.Attribute("Device").Value.Replace(st_path, GoldName);
        //                            //d_item.Attribute("Meta").Value = d_item.Attribute("Meta").Value.Replace(st_path, GoldName);
        //                            string Source = m_XmlManager.Attribute(d_item, "Device").Value;
        //                            string Target = m_XmlManager.Attribute(d_item, "Meta").Value;
        //                            string m_Source = m_XmlManager.Attribute(d_item, "Device").Value;
        //                            string m_Target = m_XmlManager.Attribute(d_item, "Meta").Value;
        //                            List<string> st_namelist = new List<string>();
        //                            while (Target.Contains("."))
        //                            {
        //                                string list_name = Target.Substring(Target.LastIndexOf("."), Target.Length - Target.LastIndexOf(".")).Replace(".", "");
        //                                Target = Target.Remove(Target.LastIndexOf("."));
        //                                st_namelist.Add(list_name);
        //                            }
        //                            st_namelist.Add(Target);
        //                            if (st_namelist.Contains(GoldName))
        //                            {
        //                                m_XmlManager.Delete(d_item);
        //                                //oldGoldMateDevname.Add(m_Target, m_Source);
        //                            }
        //                        }
        //                    }
        //                    if (m_XmlManagers.Selects("DeviceConfig") != null)
        //                    {
        //                        foreach (XElement d_item in m_XmlManagers.Selects("DeviceConfig"))
        //                        {
        //                            string Source = m_XmlManagers.Attribute(d_item, "Device").Value;
        //                            string Target = m_XmlManagers.Attribute(d_item, "Meta").Value;
        //                            string m_Source = m_XmlManager.Attribute(d_item, "Device").Value;
        //                            string m_Target = m_XmlManager.Attribute(d_item, "Meta").Value;
        //                            List<string> st_namelist = new List<string>();
        //                            while (Target.Contains("."))
        //                            {
        //                                string list_name = Target.Substring(Target.LastIndexOf("."), Target.Length - Target.LastIndexOf(".")).Replace(".", "");
        //                                Target = Target.Remove(Target.LastIndexOf("."));
        //                                st_namelist.Add(list_name);
        //                            }
        //                            st_namelist.Add(Target);
        //                            if (st_namelist.Contains(st_path))
        //                            {
        //                                // d_item.Document.ToString().Replace(st_path, GoldName);
        //                                d_item.Attribute("Device").Value = d_item.Attribute("Device").Value.Replace(st_path, GoldName);
        //                                d_item.Attribute("Meta").Value = d_item.Attribute("Meta").Value.Replace(st_path, GoldName);

        //                                if (filename[0] == "")
        //                                {
        //                                    if (ZDname == null)
        //                                    {
        //                                        m_XmlManager.Add(d_item);
        //                                        if (!GoldMateDevname.Contains(m_Source))
        //                                        {
        //                                            GoldMateDevname.Add(m_Source);
        //                                        }

        //                                    }
        //                                    else
        //                                    {

        //                                        d_item.Attribute("Device").Value = d_item.Attribute("Device").Value.Replace("设备.", "设备." + ZDname + ".");
        //                                        d_item.Attribute("Device").Value = d_item.Attribute("Device").Value.Replace("变量.", "变量." + ZDname + ".");
        //                                        d_item.Attribute("Device").Value = d_item.Attribute("Device").Value.Replace("绘图区", "绘图区" + ZDname);
        //                                        d_item.Attribute("Meta").Value = d_item.Attribute("Meta").Value.Replace("设备.", "设备." + ZDname + ".");
        //                                        d_item.Attribute("Meta").Value = d_item.Attribute("Meta").Value.Replace("变量.", "变量." + ZDname + ".");
        //                                        d_item.Attribute("Meta").Value = d_item.Attribute("Meta").Value.Replace("绘图区", "绘图区" + ZDname);
        //                                        m_XmlManager.Add(d_item);
        //                                        if (!GoldMateDevname.Contains(m_Source))
        //                                        {
        //                                            GoldMateDevname.Add(m_Source);
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (ZDname == null)
        //                                    {
        //                                        d_item.Attribute("Device").Value = d_item.Attribute("Device").Value.Replace(filename[0], ZDname);
        //                                        d_item.Attribute("Device").Value = d_item.Attribute("Device").Value.Replace("." + filename[0], ZDname);

        //                                        d_item.Attribute("Meta").Value = d_item.Attribute("Meta").Value.Replace(filename[0], ZDname);
        //                                        d_item.Attribute("Meta").Value = d_item.Attribute("Meta").Value.Replace("." + filename[0], ZDname);

        //                                        m_XmlManager.Add(d_item);
        //                                        if (!GoldMateDevname.Contains(m_Source))
        //                                        {
        //                                            GoldMateDevname.Add(m_Source);
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        d_item.Attribute("Device").Value = d_item.Attribute("Device").Value.Replace(filename[0], ZDname);
        //                                        d_item.Attribute("Meta").Value = d_item.Attribute("Meta").Value.Replace(filename[0], ZDname);
        //                                        if (!GoldMateDevname.Contains(m_Source))
        //                                        {
        //                                            GoldMateDevname.Add(m_Source);
        //                                        }
        //                                        m_XmlManager.Add(d_item);
        //                                    }
        //                                }
        //                            }
        //                        }

        //                        m_XmlManager.Save();
        //                        break;
        //                    }
        //                }
        //            }
        //            #endregion
        //            #region DeviceLinkageFileName
        //            for (int i = 0; i < filename.Length; i++)
        //            {
        //                if (filename[i].Contains("DeviceLinkageFileName.xml"))
        //                {
        //                    XmlManager m_XmlManager = new XmlManager(path + m_ZDname + filename[i]);
        //                    XmlManager m_XmlManagers = new XmlManager(expName.Substring(0, expName.LastIndexOf("\\") + 1) + filename[i]);
        //                    string OldZDname = filename[0].ToString();
        //                    if (m_XmlManager.Selects("LinkageConfig") != null)
        //                    {
        //                        foreach (XElement d_item in m_XmlManager.Selects("LinkageConfig"))
        //                        {
        //                            string Source = m_XmlManager.Attribute(d_item, "Source").Value;
        //                            string Target = m_XmlManager.Attribute(d_item, "Target").Value;
        //                            string m_Source = m_XmlManager.Attribute(d_item, "Source").Value;
        //                            string m_Target = m_XmlManager.Attribute(d_item, "Target").Value;
        //                            List<string> st_namelist = new List<string>();
        //                            while (Source.Contains("."))
        //                            {
        //                                string list_name = Source.Substring(Source.LastIndexOf("."), Source.Length - Source.LastIndexOf(".")).Replace(".", "");
        //                                Source = Source.Remove(Source.LastIndexOf("."));
        //                                st_namelist.Add(list_name);
        //                            }
        //                            while (Target.Contains("."))
        //                            {
        //                                string list_name = Target.Substring(Target.LastIndexOf("."), Target.Length - Target.LastIndexOf(".")).Replace(".", "");
        //                                Target = Target.Remove(Target.LastIndexOf("."));
        //                                st_namelist.Add(list_name);
        //                            }
        //                            st_namelist.Add(Target);
        //                            st_namelist.Add(Source);
        //                            if (st_namelist.Contains(GoldName))
        //                            {

        //                                m_XmlManager.Delete(d_item);
        //                                if (!m_SourceName1.Contains(m_Source))
        //                                {
        //                                    m_SourceName1.Add(m_Source);
        //                                }
        //                                if (!m_Targetname1.Contains(m_Target))
        //                                {
        //                                    m_Targetname1.Add(m_Source);
        //                                }
        //                            }
        //                            foreach (var Item in GoldMateDevname)
        //                            {
        //                                if (m_Source.Contains(Item) || m_Target.Contains(Item))
        //                                {
        //                                    m_XmlManager.Delete(d_item);
        //                                    if (!m_SourceName1.Contains(m_Source))
        //                                    {
        //                                        m_SourceName1.Add(m_Source);
        //                                    }
        //                                    if (!m_Targetname1.Contains(m_Target))
        //                                    {
        //                                        m_Targetname1.Add(m_Source);
        //                                    }
        //                                    break;
        //                                }

        //                            }

        //                        }
        //                    }
        //                    if (m_XmlManagers.Selects("LinkageConfig") != null)
        //                    {
        //                        foreach (XElement d_item in m_XmlManagers.Selects("LinkageConfig"))
        //                        {
        //                            string Source = m_XmlManagers.Attribute(d_item, "Source").Value;
        //                            string Target = m_XmlManagers.Attribute(d_item, "Target").Value;
        //                            string m_Source = m_XmlManagers.Attribute(d_item, "Source").Value;
        //                            string m_Target = m_XmlManagers.Attribute(d_item, "Target").Value;
        //                            List<string> st_namelist = new List<string>();
        //                            List<string> Dst_namelist = new List<string>();
        //                            while (Source.Contains("."))
        //                            {
        //                                string list_name = Source.Substring(Source.LastIndexOf("."), Source.Length - Source.LastIndexOf(".")).Replace(".", "");
        //                                Source = Source.Remove(Source.LastIndexOf("."));
        //                                st_namelist.Add(list_name);

        //                            }
        //                            while (Target.Contains("."))
        //                            {
        //                                string list_name = Target.Substring(Target.LastIndexOf("."), Target.Length - Target.LastIndexOf(".")).Replace(".", "");
        //                                Target = Target.Remove(Target.LastIndexOf("."));
        //                                st_namelist.Add(list_name);
        //                            }
        //                            st_namelist.Add(Target);
        //                            st_namelist.Add(Source);
        //                            if (st_namelist.Contains(st_path))
        //                            {
        //                                d_item.Attribute("Source").Value = d_item.Attribute("Source").Value.Replace(st_path, GoldName);
        //                                d_item.Attribute("Target").Value = d_item.Attribute("Target").Value.Replace(st_path, GoldName);
        //                                if (filename[0] == "")
        //                                {
        //                                    if (ZDname == null)
        //                                    {
        //                                        m_XmlManager.Add(d_item);
        //                                        if (!m_SourceName.Contains(m_Source))
        //                                        {
        //                                            m_SourceName.Add(m_Source);
        //                                        }
        //                                        if (!m_Targetname.Contains(m_Target))
        //                                        {
        //                                            m_Targetname.Add(m_Target);
        //                                        }
        //                                        //MateLinkageDevname.Add(m_Target, m_Source);
        //                                    }
        //                                    else
        //                                    {
        //                                        d_item.Attribute("Source").Value = d_item.Attribute("Source").Value.Replace("设备.", "设备." + ZDname + ".");
        //                                        d_item.Attribute("Source").Value = d_item.Attribute("Source").Value.Replace("变量.", "变量." + ZDname + ".");
        //                                        d_item.Attribute("Source").Value = d_item.Attribute("Source").Value.Replace("绘图区", "绘图区" + ZDname);
        //                                        d_item.Attribute("Target").Value = d_item.Attribute("Target").Value.Replace("设备.", "设备." + ZDname + ".");
        //                                        d_item.Attribute("Target").Value = d_item.Attribute("Target").Value.Replace("变量.", "变量." + ZDname + ".");
        //                                        d_item.Attribute("Target").Value = d_item.Attribute("Target").Value.Replace("绘图区", "绘图区" + ZDname);
        //                                        if (!m_SourceName.Contains(m_Source))
        //                                        {
        //                                            m_SourceName.Add(m_Source);
        //                                        }
        //                                        if (!m_Targetname.Contains(m_Target))
        //                                        {
        //                                            m_Targetname.Add(m_Target);
        //                                        }
        //                                        m_XmlManager.Add(d_item);
        //                                        // MateLinkageDevname.Add(m_Target, m_Source);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (ZDname == null)
        //                                    {
        //                                        d_item.Attribute("Source").Value = d_item.Attribute("Source").Value.Replace(filename[0], ZDname);
        //                                        d_item.Attribute("Source").Value = d_item.Attribute("Source").Value.Replace("." + filename[0], ZDname);

        //                                        d_item.Attribute("Target").Value = d_item.Attribute("Target").Value.Replace(filename[0], ZDname);
        //                                        d_item.Attribute("Target").Value = d_item.Attribute("Target").Value.Replace("." + filename[0], ZDname);

        //                                        m_XmlManager.Add(d_item);
        //                                        if (!m_SourceName.Contains(m_Source))
        //                                        {
        //                                            m_SourceName.Add(m_Source);
        //                                        }
        //                                        if (!m_Targetname.Contains(m_Target))
        //                                        {
        //                                            m_Targetname.Add(m_Target);
        //                                        }
        //                                        // MateLinkageDevname.Add(m_Target, m_Source);
        //                                    }
        //                                    else
        //                                    {
        //                                        d_item.Attribute("Source").Value = d_item.Attribute("Source").Value.Replace(filename[0], ZDname);
        //                                        d_item.Attribute("Target").Value = d_item.Attribute("Target").Value.Replace(filename[0], ZDname);

        //                                        m_XmlManager.Add(d_item);
        //                                        if (!m_SourceName.Contains(m_Source))
        //                                        {
        //                                            m_SourceName.Add(m_Source);
        //                                        }
        //                                        if (!m_Targetname.Contains(m_Target))
        //                                        {
        //                                            m_Targetname.Add(m_Target);
        //                                        }
        //                                        // MateLinkageDevname.Add(m_Target, m_Source);
        //                                    }
        //                                }

        //                            }
        //                            foreach (var Item in GoldMateDevname)
        //                            {
        //                                if (st_namelist.Contains(st_path) || m_Source.Contains(Item + ".") || m_Target.Contains(Item + "."))
        //                                {
        //                                    d_item.Attribute("Source").Value = d_item.Attribute("Source").Value.Replace(st_path, GoldName);
        //                                    d_item.Attribute("Target").Value = d_item.Attribute("Target").Value.Replace(st_path, GoldName);
        //                                    if (filename[0] == "")
        //                                    {
        //                                        if (ZDname == null)
        //                                        {
        //                                            m_XmlManager.Add(d_item);
        //                                            if (!m_SourceName.Contains(m_Source))
        //                                            {
        //                                                m_SourceName.Add(m_Source);
        //                                            }
        //                                            if (!m_Targetname.Contains(m_Target))
        //                                            {
        //                                                m_Targetname.Add(m_Target);
        //                                            }
        //                                            //MateLinkageDevname.Add(m_Target, m_Source);
        //                                        }
        //                                        else
        //                                        {
        //                                            d_item.Attribute("Source").Value = d_item.Attribute("Source").Value.Replace("设备.", "设备." + ZDname + ".");
        //                                            d_item.Attribute("Source").Value = d_item.Attribute("Source").Value.Replace("变量.", "变量." + ZDname + ".");
        //                                            d_item.Attribute("Source").Value = d_item.Attribute("Source").Value.Replace("绘图区", "绘图区" + ZDname);
        //                                            d_item.Attribute("Target").Value = d_item.Attribute("Target").Value.Replace("设备.", "设备." + ZDname + ".");
        //                                            d_item.Attribute("Target").Value = d_item.Attribute("Target").Value.Replace("变量.", "变量." + ZDname + ".");
        //                                            d_item.Attribute("Target").Value = d_item.Attribute("Target").Value.Replace("绘图区", "绘图区" + ZDname);
        //                                            if (!m_SourceName.Contains(m_Source))
        //                                            {
        //                                                m_SourceName.Add(m_Source);
        //                                            }
        //                                            if (!m_Targetname.Contains(m_Target))
        //                                            {
        //                                                m_Targetname.Add(m_Target);
        //                                            }
        //                                            m_XmlManager.Add(d_item);
        //                                            // MateLinkageDevname.Add(m_Target, m_Source);
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        if (ZDname == null)
        //                                        {
        //                                            d_item.Attribute("Source").Value = d_item.Attribute("Source").Value.Replace(filename[0], ZDname);
        //                                            d_item.Attribute("Source").Value = d_item.Attribute("Source").Value.Replace("." + filename[0], ZDname);

        //                                            d_item.Attribute("Target").Value = d_item.Attribute("Target").Value.Replace(filename[0], ZDname);
        //                                            d_item.Attribute("Target").Value = d_item.Attribute("Target").Value.Replace("." + filename[0], ZDname);

        //                                            m_XmlManager.Add(d_item);
        //                                            if (!m_SourceName.Contains(m_Source))
        //                                            {
        //                                                m_SourceName.Add(m_Source);
        //                                            }
        //                                            if (!m_Targetname.Contains(m_Target))
        //                                            {
        //                                                m_Targetname.Add(m_Target);
        //                                            }
        //                                            // MateLinkageDevname.Add(m_Target, m_Source);
        //                                        }
        //                                        else
        //                                        {
        //                                            d_item.Attribute("Source").Value = d_item.Attribute("Source").Value.Replace(filename[0], ZDname);
        //                                            d_item.Attribute("Target").Value = d_item.Attribute("Target").Value.Replace(filename[0], ZDname);

        //                                            m_XmlManager.Add(d_item);
        //                                            if (!m_SourceName.Contains(m_Source))
        //                                            {
        //                                                m_SourceName.Add(m_Source);
        //                                            }
        //                                            if (!m_Targetname.Contains(m_Target))
        //                                            {
        //                                                m_Targetname.Add(m_Target);
        //                                            }
        //                                            // MateLinkageDevname.Add(m_Target, m_Source);
        //                                        }
        //                                    }
        //                                    break;
        //                                }

        //                            }
        //                        }
        //                    }

        //                    m_XmlManager.Save();
        //                    break;
        //                }
        //            }
        //            #endregion
        //            #region DeviceConfigFileName
        //            for (int i = 0; i < filename.Length; i++)
        //            {

        //                if (filename[i].Contains("DeviceConfigFileName.xml"))
        //                {

        //                    XmlManager m_XmlManager = new XmlManager(path + m_ZDname + filename[i]);
        //                    XmlManager m_XmlManagers = new XmlManager(expName.Substring(0, expName.LastIndexOf("\\") + 1) + filename[i]);
        //                    string OldZDname = filename[0].ToString();
        //                    #region 双方都不存在多工程
        //                    if (OldZDname == "" && m_ZDname == "")
        //                    {
        //                        List<XElement> xe = m_XmlManager.Selects("DeviceConfig");
        //                        List<string> T_name = new List<string>();
        //                        string t_name = null;
        //                        if (m_XmlManager.Selects("DeviceConfig") != null)
        //                        {
        //                            foreach (XElement dd_item in m_XmlManager.Selects("DeviceConfig"))
        //                            {
        //                                string name = "设备." + dd_item.Attribute("_name").Value;
        //                                string type = dd_item.Attribute("Type").Value;
        //                                if (type == "摄像头")
        //                                {
        //                                    foreach (XElement t in dd_item.Selects("Property"))
        //                                    {
        //                                        if (t.Attribute("_name").Value == "父设备")
        //                                        {
        //                                            t_name = t.Attribute("IniValue").Value;
        //                                        }
        //                                    }
        //                                }
        //                                if (GoldMateDevname.Contains(name))
        //                                {
        //                                    m_XmlManager.Delete(dd_item);
        //                                    if (!T_name.Contains(t_name))
        //                                    {
        //                                        T_name.Add(t_name);
        //                                    }
        //                                }
        //                                else if (m_SourceName1.Contains(name) || m_Targetname1.Contains(name))
        //                                {
        //                                    m_XmlManager.Delete(dd_item);
        //                                    if (!T_name.Contains(t_name))
        //                                    {
        //                                        T_name.Add(t_name);
        //                                    }
        //                                }
        //                            }
        //                            //删除视频服务
        //                            foreach (XElement dd_item in m_XmlManager.Selects("DeviceConfig"))
        //                            {
        //                                string name = dd_item.Attribute("_name").Value;
        //                                if (T_name.Contains(name))
        //                                {
        //                                    m_XmlManager.Delete(dd_item);
        //                                }
        //                            }
        //                        }
        //                        if (m_XmlManagers.Selects("DeviceConfig") != null)
        //                        {
        //                            T_name = new List<string>();
        //                            t_name = null;
        //                            foreach (XElement d_item in m_XmlManagers.Selects("DeviceConfig"))
        //                            {
        //                                string name = "设备." + d_item.Attribute("_name").Value;
        //                                string type = d_item.Attribute("Type").Value;
        //                                if (type == "摄像头")
        //                                {
        //                                    foreach (XElement t in d_item.Selects("Property"))
        //                                    {
        //                                        if (t.Attribute("_name").Value == "父设备")
        //                                        {
        //                                            t_name = t.Attribute("IniValue").Value;
        //                                        }
        //                                    }

        //                                }
        //                                //string name = "设备." + d_item.Attribute("_name").Value;
        //                                if (GoldMateDevname.Contains(name))
        //                                {
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace(st_path, GoldName);
        //                                    m_XmlManager.Add(d_item);
        //                                    if (!T_name.Contains(t_name))
        //                                    {
        //                                        T_name.Add(t_name);
        //                                    }
        //                                }
        //                                else if (m_SourceName.Contains(name) || m_Targetname.Contains(name))
        //                                {
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace(st_path, GoldName);
        //                                    m_XmlManager.Add(d_item);
        //                                    if (!T_name.Contains(t_name))
        //                                    {
        //                                        T_name.Add(t_name);
        //                                    }
        //                                }
        //                            }
        //                            foreach (XElement d_item in m_XmlManagers.Selects("DeviceConfig"))
        //                            {
        //                                string name = d_item.Attribute("_name").Value;
        //                                if (T_name.Contains(name))
        //                                {
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace(st_path, GoldName);
        //                                    m_XmlManager.Add(d_item);
        //                                }
        //                            }
        //                        }
        //                        m_XmlManager.Save();
        //                        break;
        //                    }
        //                    #endregion
        //                    #region 双方都为多工程
        //                    else if (OldZDname != "" && m_ZDname != "")
        //                    {
        //                        List<XElement> xe = m_XmlManager.Selects("Zbgc");
        //                        List<string> T_name = new List<string>();
        //                        string t_name = null;
        //                        if (m_XmlManager.Selects("Zbgc") != null)
        //                        {
        //                            foreach (XElement dd_item in m_XmlManager.Selects("Zbgc"))
        //                            {
        //                                foreach (XElement ddd_item in dd_item.Selects("DeviceConfig"))
        //                                {
        //                                    string name = "设备." + m_ZDname.Replace("\\", null) + "." + ddd_item.Attribute("_name").Value;
        //                                    string type = ddd_item.Attribute("Type").Value;
        //                                    if (type == "摄像头")
        //                                    {
        //                                        foreach (XElement t in ddd_item.Selects("Property"))
        //                                        {
        //                                            if (t.Attribute("_name").Value == "父设备")
        //                                            {
        //                                                t_name = t.Attribute("IniValue").Value;
        //                                            }
        //                                        }

        //                                    }
        //                                    if (GoldMateDevname.Contains(name))
        //                                    {
        //                                        m_XmlManager.Delete(ddd_item);
        //                                        if (!T_name.Contains(t_name))
        //                                        {
        //                                            T_name.Add(t_name);
        //                                        }
        //                                    }
        //                                    else if (m_SourceName1.Contains(name) || m_Targetname1.Contains(name))
        //                                    {
        //                                        m_XmlManager.Delete(ddd_item);
        //                                        if (!T_name.Contains(t_name))
        //                                        {
        //                                            T_name.Add(t_name);
        //                                        }
        //                                    }
        //                                }
        //                                foreach (XElement ddd_item in dd_item.Selects("DeviceConfig"))
        //                                {
        //                                    string name = ddd_item.Attribute("_name").Value;
        //                                    if (T_name.Contains(name))
        //                                    {
        //                                        m_XmlManager.Delete(dd_item);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        if (m_XmlManagers.Selects("Zbgc") != null)
        //                        {
        //                            T_name = new List<string>();
        //                            t_name = null;
        //                            if (m_XmlManager.Select("Zbgc") == null)
        //                            {
        //                                m_XmlManager.Attribute("version", "1.1.0");
        //                                XElement root = new XElement("Zbgc");
        //                                root.SetAttributeValue("_name", ZDname);
        //                                root.SetAttributeValue("Dh", "");
        //                                root.SetAttributeValue("Nm", "");
        //                                m_XmlManager.Add(root);
        //                            }
        //                            foreach (XElement dd_item in m_XmlManagers.Select("Zbgc").Selects("DeviceConfig"))
        //                            {
        //                                string name = "设备." + m_XmlManagers.Select("Zbgc").FirstAttribute.Value + "." + dd_item.Attribute("_name").Value;

        //                                string type = dd_item.Attribute("Type").Value;
        //                                if (type == "摄像头")
        //                                {
        //                                    foreach (XElement t in dd_item.Selects("Property"))
        //                                    {
        //                                        if (t.Attribute("_name").Value == "父设备")
        //                                        {
        //                                            t_name = t.Attribute("IniValue").Value;
        //                                        }
        //                                    }

        //                                }
        //                                if (GoldMateDevname.Contains(name))
        //                                {

        //                                    dd_item.Attribute("_name").Value = dd_item.Attribute("_name").Value.Replace(st_path, GoldName);

        //                                    m_XmlManager.Select("Zbgc").Add(dd_item);
        //                                    if (!T_name.Contains(t_name))
        //                                    {
        //                                        T_name.Add(t_name);
        //                                    }
        //                                }
        //                                else if (m_SourceName.Contains(name) || m_Targetname.Contains(name))
        //                                {

        //                                    dd_item.Attribute("_name").Value = dd_item.Attribute("_name").Value.Replace(st_path, GoldName);

        //                                    m_XmlManager.Select("Zbgc").Add(dd_item);
        //                                    if (!T_name.Contains(t_name))
        //                                    {
        //                                        T_name.Add(t_name);
        //                                    }
        //                                }

        //                            }
        //                            foreach (XElement dd_item in m_XmlManagers.Select("Zbgc").Selects("DeviceConfig"))
        //                            {
        //                                string name = dd_item.Attribute("_name").Value;
        //                                if (T_name.Contains(name))
        //                                {

        //                                    dd_item.Attribute("_name").Value = dd_item.Attribute("_name").Value.Replace(st_path, GoldName);
        //                                    m_XmlManager.Select("Zbgc").Add(dd_item);
        //                                }
        //                            }
        //                            //}
        //                        }
        //                        m_XmlManager.Save();
        //                        break;
        //                    }
        //                    #endregion
        //                    #region 单工程向多工程导入
        //                    else if (OldZDname == "" && m_ZDname != "")
        //                    {
        //                        if (m_XmlManager.Select("Zbgc") == null)
        //                        {
        //                            m_XmlManager.Attribute("version", "1.1.0");
        //                            XElement root = new XElement("Zbgc");
        //                            root.SetAttributeValue("_name", ZDname);
        //                            root.SetAttributeValue("Dh", "");
        //                            root.SetAttributeValue("Nm", "");
        //                            m_XmlManager.Add(root);
        //                        }
        //                        List<string> T_name = new List<string>();
        //                        string t_name = null;
        //                        if (m_XmlManager.Selects("Zbgc") != null)
        //                        {

        //                            foreach (XElement dd_item in m_XmlManager.Selects("Zbgc"))
        //                            {

        //                                foreach (XElement ddd_item in dd_item.Selects("DeviceConfig"))
        //                                {
        //                                    string name = "设备." + ddd_item.Attribute("_name").Value;
        //                                    string type = ddd_item.Attribute("Type").Value;
        //                                    if (type == "摄像头")
        //                                    {
        //                                        foreach (XElement t in ddd_item.Selects("Property"))
        //                                        {
        //                                            if (t.Attribute("_name").Value == "父设备")
        //                                            {
        //                                                t_name = t.Attribute("IniValue").Value;
        //                                            }
        //                                        }

        //                                    }
        //                                    if (GoldMateDevname.Contains(name))
        //                                    {
        //                                        m_XmlManager.Delete(dd_item);
        //                                        if (!T_name.Contains(t_name))
        //                                        {
        //                                            T_name.Add(t_name);
        //                                        }
        //                                    }
        //                                    else if (m_SourceName1.Contains(name) || m_Targetname1.Contains(name))
        //                                    {
        //                                        m_XmlManager.Delete(dd_item);
        //                                        if (!T_name.Contains(t_name))
        //                                        {
        //                                            T_name.Add(t_name);
        //                                        }
        //                                    }
        //                                }
        //                                foreach (XElement ddd_item in dd_item.Selects("DeviceConfig"))
        //                                {
        //                                    string name = ddd_item.Attribute("_name").Value;
        //                                    if (T_name.Contains(name))
        //                                    {
        //                                        m_XmlManager.Delete(dd_item);
        //                                    }
        //                                }

        //                            }
        //                        }
        //                        if (m_XmlManagers.Selects("DeviceConfig") != null)
        //                        {
        //                            T_name = new List<string>();
        //                            t_name = null;
        //                            foreach (XElement d_item in m_XmlManagers.Selects("DeviceConfig"))
        //                            {

        //                                string name = "设备." + d_item.Attribute("_name").Value;
        //                                string type = d_item.Attribute("Type").Value;
        //                                if (type == "摄像头")
        //                                {
        //                                    foreach (XElement t in d_item.Selects("Property"))
        //                                    {
        //                                        if (t.Attribute("_name").Value == "父设备")
        //                                        {
        //                                            t_name = t.Attribute("IniValue").Value;
        //                                        }
        //                                    }

        //                                }
        //                                if (GoldMateDevname.Contains(name))
        //                                {

        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace(st_path, GoldName);

        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("设备.", "设备." + ZDname + ".");
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("变量.", "变量." + ZDname + ".");
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("绘图区", "绘图区" + ZDname);
        //                                    m_XmlManager.Select("Zbgc").Add(d_item);
        //                                    if (!T_name.Contains(t_name))
        //                                    {
        //                                        T_name.Add(t_name);
        //                                    }
        //                                }
        //                                else if (m_SourceName.Contains(name) || m_Targetname.Contains(name))
        //                                {
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace(st_path, GoldName);

        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("设备.", "设备." + ZDname + ".");
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("变量.", "变量." + ZDname + ".");
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("绘图区", "绘图区" + ZDname);
        //                                    m_XmlManager.Select("Zbgc").Add(d_item);
        //                                    if (!T_name.Contains(t_name))
        //                                    {
        //                                        T_name.Add(t_name);
        //                                    }
        //                                }

        //                            }
        //                            foreach (XElement d_item in m_XmlManagers.Selects("DeviceConfig"))
        //                            {

        //                                string name = d_item.Attribute("_name").Value;
        //                                if (T_name.Contains(name))
        //                                {
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace(st_path, GoldName);
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("设备.", "设备." + ZDname + ".");
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("变量.", "变量." + ZDname + ".");
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("绘图区", "绘图区" + ZDname);
        //                                    m_XmlManager.Select("Zbgc").Add(d_item);
        //                                }
        //                            }

        //                        }
        //                        m_XmlManager.Save();
        //                        break;
        //                    }
        //                    #endregion
        //                    #region 多工程向单工程导入
        //                    // List<XElement> xe = m_XmlManager.Selects("DeviceConfig");
        //                    else
        //                    {
        //                        List<string> T_name = new List<string>();
        //                        string t_name = null;
        //                        if (m_XmlManager.Selects("Zbgc") != null)
        //                        {
        //                            foreach (XElement dd_item in m_XmlManager.Select("Zbgc").Selects("DeviceConfig"))
        //                            {
        //                                string name = "设备." + dd_item.Attribute("_name").Value;
        //                                string type = dd_item.Attribute("Type").Value;
        //                                if (type == "摄像头")
        //                                {
        //                                    foreach (XElement t in dd_item.Selects("Property"))
        //                                    {
        //                                        if (t.Attribute("_name").Value == "父设备")
        //                                        {
        //                                            t_name = t.Attribute("IniValue").Value;
        //                                        }
        //                                    }

        //                                }
        //                                if (GoldMateDevname.Contains(name))
        //                                {
        //                                    //m_XmlManager.Select("Zbgc").re.(dd_item);
        //                                    dd_item.Remove();
        //                                    if (!T_name.Contains(t_name))
        //                                    {
        //                                        T_name.Add(t_name);
        //                                    }
        //                                }
        //                                else if (m_SourceName1.Contains(name) || m_Targetname1.Contains(name))
        //                                {
        //                                    dd_item.Remove();
        //                                    if (!T_name.Contains(t_name))
        //                                    {
        //                                        T_name.Add(t_name);
        //                                    }
        //                                }
        //                            }
        //                            foreach (XElement dd_item in m_XmlManager.Select("Zbgc").Selects("DeviceConfig"))
        //                            {
        //                                string name = dd_item.Attribute("_name").Value;
        //                                if (T_name.Contains(name))
        //                                {
        //                                    dd_item.Remove();
        //                                }
        //                            }

        //                        }
        //                        if (m_XmlManagers.Selects("DeviceConfig") != null)
        //                        {
        //                            T_name = new List<string>();
        //                            t_name = null;
        //                            foreach (XElement d_item in m_XmlManagers.Selects("DeviceConfig"))
        //                            {
        //                                string name = d_item.Attribute("_name").Value;
        //                                string type = d_item.Attribute("Type").Value;
        //                                if (type == "摄像头")
        //                                {
        //                                    foreach (XElement t in d_item.Selects("Property"))
        //                                    {
        //                                        if (t.Attribute("_name").Value == "父设备")
        //                                        {
        //                                            t_name = t.Attribute("IniValue").Value;
        //                                        }
        //                                    }
        //                                }
        //                                if (GoldMateDevname.Contains(name))
        //                                {
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace(st_path, GoldName);
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("设备.", "设备." + ZDname + ".");
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("变量.", "变量." + ZDname + ".");
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("绘图区", "绘图区" + ZDname);
        //                                    m_XmlManager.Select("Zbgc").Add(d_item);
        //                                    if (!T_name.Contains(t_name))
        //                                    {
        //                                        T_name.Add(t_name);
        //                                    }
        //                                }
        //                                else if (m_SourceName.Contains(name) || m_Targetname.Contains(name))
        //                                {

        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace(st_path, GoldName);
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("设备.", "设备." + ZDname + ".");
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("变量.", "变量." + ZDname + ".");
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("绘图区", "绘图区" + ZDname);
        //                                    m_XmlManager.Select("Zbgc").Add(d_item);
        //                                    if (!T_name.Contains(t_name))
        //                                    {
        //                                        T_name.Add(t_name);
        //                                    }
        //                                }

        //                            }
        //                            foreach (XElement d_item in m_XmlManagers.Selects("DeviceConfig"))
        //                            {
        //                                string name = d_item.Attribute("_name").Value;
        //                                if (T_name.Contains(name))
        //                                {
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace(st_path, GoldName);
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("设备.", "设备." + ZDname + ".");
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("变量.", "变量." + ZDname + ".");
        //                                    d_item.Attribute("_name").Value = d_item.Attribute("_name").Value.Replace("绘图区", "绘图区" + ZDname);
        //                                    m_XmlManager.Select("Zbgc").Add(d_item);
        //                                }
        //                            }
        //                            m_XmlManager.Save();
        //                            break;
        //                        }
        //                    }
        //                }
        //                    #endregion
        //            }
        //            #endregion
        //            #region MateLinkageConfig
        //            for (int i = 0; i < filename.Length; i++)
        //            {

        //                if (filename[i].Contains("MateLinkageConfig.xml"))
        //                {
        //                    XmlManager m_XmlManager = new XmlManager(path + m_ZDname + filename[i]);
        //                    XmlManager m_XmlManagers = new XmlManager(expName.Substring(0, expName.LastIndexOf("\\") + 1) + filename[i]);
        //                    string OldZDname = filename[0].ToString();
        //                    if (m_XmlManager.Select("UserEventStates") != null && m_XmlManager.Select("UserEventStates").Selects("EventState") != null)
        //                    {
        //                        foreach (XElement d_item in m_XmlManager.Select("UserEventStates").Selects("EventState"))
        //                        {
        //                            string Source = "用户事件管理." + m_XmlManager.Attribute(d_item, "name").Value;
        //                            if (m_SourceName1.Contains(Source) || m_Targetname1.Contains(Source))
        //                            {
        //                                m_XmlManager.Delete(d_item);
        //                            }
        //                        }
        //                    }
        //                    if (m_XmlManager.Select("UserActions") != null && m_XmlManager.Select("UserActions").Selects("Action") != null)
        //                    {
        //                        foreach (XElement d_item in m_XmlManager.Select("UserActions").Selects("Action"))
        //                        {
        //                            string Source = "用户动作管理." + m_XmlManager.Attribute(d_item, "name").Value;
        //                            if (m_SourceName1.Contains(Source) || m_Targetname1.Contains(Source))
        //                            {
        //                                m_XmlManager.Delete(d_item);
        //                            }
        //                        }
        //                    }
        //                    if (m_XmlManagers.Select("UserEventStates") != null && m_XmlManagers.Select("UserEventStates").Selects("EventState") != null)
        //                    {
        //                        foreach (XElement d_item in m_XmlManagers.Select("UserEventStates").Selects("EventState"))
        //                        {
        //                            string Source = "用户事件管理." + m_XmlManagers.Attribute(d_item, "name").Value;
        //                            if (m_SourceName.Contains(Source) || m_Targetname.Contains(Source))
        //                            {
        //                                d_item.Attribute("name").Value = d_item.Attribute("name").Value.Replace(st_path, GoldName);
        //                                if (filename[0] == "")
        //                                {
        //                                    if (ZDname == null)
        //                                    {
        //                                        m_XmlManager.Add(d_item);
        //                                    }
        //                                    else
        //                                    {
        //                                        d_item.Attribute("name").Value = d_item.Attribute("name").Value.Replace("设备.", "设备." + ZDname + ".");
        //                                        d_item.Attribute("name").Value = d_item.Attribute("name").Value.Replace("变量.", "变量." + ZDname + ".");
        //                                        d_item.Attribute("name").Value = d_item.Attribute("name").Value.Replace("绘图区", "绘图区" + ZDname);

        //                                        m_XmlManager.Add(d_item);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (ZDname != null)
        //                                    {
        //                                        d_item.Attribute("name").Value = d_item.Attribute("name").Value.Replace(filename[0], ZDname);
        //                                        d_item.Attribute("name").Value = d_item.Attribute("name").Value.Replace("." + filename[0], ZDname);

        //                                        m_XmlManager.Add(d_item);
        //                                    }
        //                                    else
        //                                    {
        //                                        d_item.Attribute("name").Value = d_item.Attribute("name").Value.Replace(filename[0], ZDname);

        //                                        m_XmlManager.Add(d_item);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                    if (m_XmlManagers.Select("UserActions") != null && m_XmlManagers.Select("UserActions").Selects("Action") != null)
        //                    {
        //                        foreach (XElement d_item in m_XmlManagers.Select("UserActions").Selects("Action"))
        //                        {
        //                            string Source = "用户动作管理." + m_XmlManagers.Attribute(d_item, "name").Value;
        //                            if (m_SourceName.Contains(Source) || m_Targetname.Contains(Source))
        //                            {
        //                                d_item.Attribute("name").Value = d_item.Attribute("name").Value.Replace(st_path, GoldName);
        //                                if (filename[0] == "")
        //                                {
        //                                    if (ZDname == null)
        //                                    {
        //                                        m_XmlManager.Add(d_item);
        //                                    }
        //                                    else
        //                                    {
        //                                        d_item.Attribute("name").Value = d_item.Attribute("name").Value.Replace("设备.", "设备." + ZDname + ".");
        //                                        d_item.Attribute("name").Value = d_item.Attribute("name").Value.Replace("变量.", "变量." + ZDname + ".");
        //                                        d_item.Attribute("name").Value = d_item.Attribute("name").Value.Replace("绘图区", "绘图区" + ZDname);

        //                                        m_XmlManager.Add(d_item);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (ZDname != null)
        //                                    {
        //                                        d_item.Attribute("name").Value = d_item.Attribute("name").Value.Replace(filename[0], ZDname);
        //                                        d_item.Attribute("name").Value = d_item.Attribute("name").Value.Replace("." + filename[0], ZDname);

        //                                        m_XmlManager.Add(d_item);
        //                                    }
        //                                    else
        //                                    {
        //                                        d_item.Attribute("name").Value = d_item.Attribute("name").Value.Replace(filename[0], ZDname);

        //                                        m_XmlManager.Add(d_item);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                    m_XmlManager.Save();
        //                    break;
        //                }

        //            #endregion
        //            }
        //            if (!File.Exists(path + m_ZDname + "CustomDiagram\\" + GoldPage))
        //            {
        //                Directory.CreateDirectory(path + m_ZDname + "CustomDiagram");
        //            }
        //            File.Copy(expName.Substring(0, expName.LastIndexOf("\\") + 1) + "CustomDiagram\\" + pageName, path + m_ZDname + "CustomDiagram\\" + GoldPage, true);
        //            MessageBox.Show("导入成功");
        //            // Application.Current.Shutdown();

        //        }
        //        catch { MessageBox.Show("导入失败或导入被中断"); }

        //    }
        //}

        #endregion
        private XElement LoadSerializedDataFromFile()
        {
            var openFile = new OpenFileDialog();
            openFile.Filter = "Designer Files (*.xml)|*.xml";
            openFile.InitialDirectory = _path + "\\CustomDiagram";
            //openFile.Multiselect = true;

           
            if (openFile.ShowDialog() == true)
            {
                try
                {
                    return XElement.Load(openFile.FileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show("文件内容格式错误!", "文件打开失败"+e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                    //LogManager.Instance.AddLog(LogEventType.Warning, e, "异常日志");
                }
            }

            return null;
        }

        public string FullFileName { get; internal set; }

        public void SaveFile(bool deleteDeviceLinkage = true)
        {
            IEnumerable<DesignerContainer> DesignerContainers = Children.OfType<DesignerContainer>();//找到画布里面的所有DesignerContainer

            XElement xContainersXML = DesignerContainers.ToXElement();//PropertyExtension去序列化XML
            XElement root = new XElement("Root");
            root.Add(xContainersXML);

            #region 保存文件细节处理
            string diaPath = _path + "\\CustomDiagram";
            if (!Directory.Exists(diaPath)) Directory.CreateDirectory(diaPath);
            string fileName = "test";
            if (string.IsNullOrEmpty(fileName))
            {
                if (string.IsNullOrEmpty(Application.Current.Properties["CurrentFile"].ToString())) return;
                fileName = Application.Current.Properties["CurrentFile"].ToString();
            }
            string fullName = diaPath + "\\" + fileName + ".xml";
            FullFileName = fullName;

            try
            {
                if (File.Exists(fullName))
                {
                    if (MessageBox.Show("文件已存在,是否覆盖?", "警告", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    {
                        fullName = diaPath + "\\" + fileName + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xml";
                    }
                }
                //保存文件
                root.Save(fullName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            #endregion
        }

        //反序列化DesignerContainer 王运钢修改-2011-05-16
        private void CopyCurrentSelection()
        {
            var selectedDesignerContainers = SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>();
            var DesignerContainersXML = selectedDesignerContainers.ToXElement();
            var root = new XElement("Root");
            root.Add(DesignerContainersXML);

            root.Add(new XAttribute("OffsetX", 10));
            root.Add(new XAttribute("OffsetY", 10));

            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
        }

        private void DeleteCurrentSelection()
        {
            var selectedDesignerContainers =
                SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>();
            //foreach (DesignerContainer Item in selectedDesignerContainers)
            //{

            //    this.BaissObject.Remove(Item.BasisObject);
            //    Item.IsNewItem = false;
            //    this.Children.Remove(Item);

            //    //m_LinkageManager.DeleteLinkageByPreName(Item.BasisObject._name);
            //}

            //SelectionService.ClearSelection();
            //UpdateZIndex();
            //PropertyView pv = this.FindName("pv") as PropertyView;
            //if (pv != null) pv.InitProView();

            CommandCut.Run(CommManager, selectedDesignerContainers);
            //OptStack.SaveOpt(this);
            Focus();
        }

        private void UpdateZIndex()
        {
            var ordered = (from UIElement item in Children
                orderby GetZIndex(item)
                select item).ToList();

            for (var i = 0; i < ordered.Count; i++)
            {
                if (GetZIndex(ordered[i]) == int.MinValue)
                {
                    continue;
                }
                SetZIndex(ordered[i], i);
            }
        }

        private static Rect GetBoundingRectangle(IEnumerable<DesignerContainer> items)
        {
            var x1 = Double.MaxValue;
            var y1 = Double.MaxValue;
            var x2 = Double.MinValue;
            var y2 = Double.MinValue;

            foreach (var item in items)
            {
                x1 = Math.Min(GetLeft(item), x1);
                y1 = Math.Min(GetTop(item), y1);

                x2 = Math.Max(GetLeft(item) + item.Width, x2);
                y2 = Math.Max(GetTop(item) + item.Height, y2);
            }

            return new Rect(new Point(x1, y1), new Point(x2, y2));
        }

        private bool BelongToSameGroup(IGroupable item1, IGroupable item2)
        {
            var root1 = SelectionService.GetGroupRoot(item1);
            var root2 = SelectionService.GetGroupRoot(item2);

            return (root1.ID == root2.ID);
        }

        /// <summary>
        ///     旋转
        /// </summary>
        /// <param name="angle"></param>
        private void Rotate(double angle)
        {
            foreach (DesignerContainer item in SelectionService.SelectedDesignerContainer)
            {
                if (item != null && item.ParentID == Guid.Empty)
                {
                    item.Angle = (item.Angle + angle)%360;
                    var left = Convert.ToInt32(GetLeft(item) - (item.Height - item.Width)/2);
                    var top = Convert.ToInt32(GetTop(item) - (item.Width - item.Height)/2);
                    left = left + item.Height > ActualWidth ? (int) (ActualWidth - item.Height) : left < 0 ? 0 : left;
                    top = top + item.Width > ActualHeight ? (int) (ActualHeight - item.Width) : top < 0 ? 0 : top;
                    SetLeft(item, left);
                    SetTop(item, top);
                    var width = item.Width;
                    item.Width = item.Height;
                    item.Height = width;
                    var dt = item.Template.FindName("PART_DragThumb", item) as DragThumb;
                    if (dt != null && dt.Template.HasContent && item.Content is FrameworkElement)
                    {
                        dt.LayoutTransform = (item.Content as FrameworkElement).LayoutTransform;
                    }
                }
            }
        }

        /// <summary>
        ///     镜像
        /// </summary>
        /// <param name="isHor"></param>
        private void Mirror(bool isHor)
        {
            foreach (DesignerContainer item in SelectionService.SelectedDesignerContainer)
            {
                if (item != null && item.ParentID == Guid.Empty)
                {
                    if (isHor)
                    {
                        //st.ScaleX = (st.ScaleX < 0 & isHor) ? 1 : -1;
                        item.ScaleX = (item.ScaleX < 0 & isHor) ? 1 : -1;
                    }
                    else
                    {
                        //st.ScaleY = (st.ScaleY < 0 & !isHor) ? 1 : -1;
                        item.ScaleY = (item.ScaleY < 0 & !isHor) ? 1 : -1;
                    }
                    //if (element is System.Windows.Shapes.bindPath)
                    //{
                    //    dt.LayoutTransform = tg;
                    //}
                    //}
                    var dt = item.Template.FindName("PART_DragThumb", item) as DragThumb;
                    if (dt != null && item.Content is FrameworkElement)
                    {
                        dt.LayoutTransform = (item.Content as FrameworkElement).LayoutTransform;
                    }
                }
            }
        }
        #endregion

        #region DeleteBg Command

        private void DeleteBg_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DesignerContainer di;
            var des = Children.OfType<ISelectable>().ToList();
            for (var i = 0; i < des.Count; i++)
            {
                di = des[i] as DesignerContainer;
                CommandDeleteBg.Run(CommManager, di);
            }
            //foreach (var Item in this.Children.OfType<ISelectable>())
            //{
            //    di = Item as DesignerContainer;
            //    CommandDeleteBg.Run(m_CommManager, di);
            //    //if (di != null)
            //    //{
            //    //    int idx = Canvas.GetZIndex(di);
            //    //    if (idx == int.MinValue)
            //    //    {
            //    //        this.Children.Remove(di);
            //    //        this.BaissObject.Remove(di.BasisObject);
            //    //        break;
            //    //    }
            //    //}
            //}
            //OptStack.SaveOpt(this);
        }

        private void DeleteBg_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var canMirror = false;

            //int k=  designerCanvas.Children.Contains(
            foreach (var item in _designerCanvas.Children.OfType<DesignerContainer>())
            {
                if (item.StyleName == "ciStyle")
                {
                    canMirror = true;
                }
                //if (this.DeleteCurrentSelection())
                //{
                //    canMirror = false;
                //    break;
                //}
            }
            e.CanExecute = canMirror;
        }

        #endregion

        #region MirrorH Command

        private void MirrorH_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //sqh修改

            CommandMirrorH.Run(CommManager, true);

            //Mirror(true);

            //OptStack.SaveOpt(this);
        }

        private void MirrorH_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var canMirror = true;

            e.CanExecute = SelectionService.SelectedDesignerContainer.Count() > 0 && canMirror;
        }

        #endregion
         
        #region MirrorV Command

        private void MirrorV_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //sqh修改
            CommandMirrorV.Run(CommManager, false);
            //Mirror(false);
            //OptStack.SaveOpt(this);
        }

        private void MirrorV_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var canMirror = true;
            foreach (var item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>())
            {
                if (item.IsGroup)
                {
                    canMirror = false;
                    break;
                }
            }
            e.CanExecute = SelectionService.SelectedDesignerContainer.Count() > 0 && canMirror;
        }

        #endregion

        #region RotateRight Command

        private void RotateRight_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //sqh修改

            CommandRotateRight.Run(CommManager, 90);
            //Rotate(90);
            //OptStack.SaveOpt(this);
        }

        private void RotateRight_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var canRotate = true;

            foreach (var item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>())
            {
                if (item.IsGroup)
                {
                    canRotate = false;
                    break;
                }
                if (item.Width > Height || item.Height > Width)
                {
                    canRotate = false;
                    break;
                }
            }
            e.CanExecute = SelectionService.SelectedDesignerContainer.Count() > 0 && canRotate;
        }

        #endregion

        #region RotateLeft Command

        private void RotateLeft_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //sqh修改
            CommandRotateLeft.Run(CommManager, -90);
            //Rotate(-90);
            //OptStack.SaveOpt(this);
        }


        private void RotateLeft_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var canRotate = true;
            foreach (var item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>())
            {
                if (item.IsGroup)
                {
                    canRotate = false;
                    break;
                }
                if (item.Width > Height || item.Height > Width)
                {
                    canRotate = false;
                    break;
                }
            }
            e.CanExecute = SelectionService.SelectedDesignerContainer.Count() > 0 && canRotate;
        }

        #endregion

        #region UniformWidth Command

        private void UniformWidth_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                where item.ParentID == Guid.Empty && !item.IsGroup
                select item;
            //sqh修改
            CommandUniformWidth.Run(CommManager, selectedItems);
            //if (selectedItems.Count() > 1)
            //{
            //    double width = selectedItems.First().Width;

            //    foreach (DesignerContainer Item in selectedItems)
            //    {
            //        Item.Width = width;
            //    }
            //}
            //OptStack.SaveOpt(this);
        }

        private void UniformWidth_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var hasGroup = false;

            foreach (var item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>())
            {
                if (item.IsGroup)
                {
                    hasGroup = true;
                    break;
                }
            }
            e.CanExecute = SelectionService.SelectedDesignerContainer.Count() > 0 && !hasGroup;
        }

        #endregion

        #region UniformHeight Command

        private void UniformHeight_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>()
                where item.ParentID == Guid.Empty && !item.IsGroup
                select item;
            //sqh修改
            CommandUniformHeight.Run(CommManager, selectedItems);
            //if (selectedItems.Count() > 1)
            //{
            //    double height = selectedItems.First().Height;

            //    foreach (DesignerContainer Item in selectedItems)
            //    {
            //        Item.Height = height;
            //    }
            //}
            //OptStack.SaveOpt(this);
        }

        private void UniformHeight_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var hasGroup = false;
            foreach (var item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>())
            {
                if (item.IsGroup)
                {
                    hasGroup = true;
                    break;
                }
            }
            e.CanExecute = SelectionService.SelectedDesignerContainer.Count() > 0 && !hasGroup;
        }

        #endregion

        #region Undo Command

        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // OptStack.Undo(this);

            CommManager.Undo();
        }

        private void Undo_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = OptStack.stUndo(this).Count > 0;
            e.CanExecute = CommManager.CanUndo;
        }

        #endregion

        #region Redo Command

        private void Redo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //OptStack.Redo(this);
            CommManager.ReDo();
        }

        private void Redo_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = OptStack.stRedo(this).Count > 0;
            e.CanExecute = CommManager.CanRedo;
        }

        #endregion

        #region ShowName Command

        private void ShowName_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //sqh修改
            var items = SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>();
            CommandShowName.Run(CommManager, items);
            //foreach (var Item in SelectionService.CurrentSelection.OfType<DesignerContainer>())
            //{
            //    Item.NameVisible = true;
            //}
            //OptStack.SaveOpt(this);
        }

        private void ShowName_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.SelectedDesignerContainer.Count > 0;
        }

        #endregion

        #region HideName Command

        private void HideName_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //sqh修改
            var items = SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>();
            CommandHidName.Run(CommManager, items);
            //foreach (var Item in SelectionService.CurrentSelection.OfType<DesignerContainer>())
            //{
            //    Item.NameVisible = false;
            //}
            //OptStack.SaveOpt(this);
        }

        private void HideName_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.SelectedDesignerContainer.Count > 0;
        }

        #endregion

        #region SelectSameType Command

        private void SelectSameType_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var di = SelectionService.SelectedDesignerContainer.First() as DesignerContainer;
            foreach (var item in Children)
            {
                var dItem = item as DesignerContainer;
                if (di != null && dItem != null && dItem.Item.GetType() == di.Item.GetType() && !dItem.IsSelected)
                {
                    dItem.IsSelected = true;
                    SelectionService.SelectedDesignerContainer.Add(dItem);
                }
            }
            var pv = FindName("pv") as PropertyView;
            if (pv != null) pv.ShowProView(false);
        }

        private void SelectSameType_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var isEnabled = true;
            if (SelectionService.SelectedDesignerContainer.Count <= 0)
            {
                e.CanExecute = false;
                return;
            }
            var di = SelectionService.SelectedDesignerContainer.First() as DesignerContainer;
            foreach (var item in SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>())
            {
                if (di != null && item.Item.GetType() != di.Item.GetType())
                {
                    isEnabled = false;
                    break;
                }
            }
            e.CanExecute = isEnabled;
        }

        #endregion

        #region 关联设备，暂未实现
        private void ConnectDevice_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void ConnectDevice_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Children.Count > 0;
        }
        #endregion
    }
}