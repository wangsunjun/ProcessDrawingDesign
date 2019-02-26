using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using Wss.Foundation.Controls;
using Wss.Foundation.Designer;
using Wss.Foundation.Designer.Command;
using Wss.FoundationCore.Models;
using Wss.FoundationCore.Controls;
using System.Windows.Markup;
using System.Xml;
using Wss.FoundationCore.Attributes;
using System.Reflection;
using System.ComponentModel;
//using Wss.Objects;
//using Wss.Objects.Linkages;

namespace Wss.Foundation
{
    public delegate void DClearPopup();

    /// <summary>
    ///     画布类,用来绘制图元
    /// </summary>
    public partial class DesignerCanvas : Canvas
    {
        internal SelectionService SelectionService
        {
            get
            {
                if (_SelectionService == null) _SelectionService = new SelectionService(this);
                return _SelectionService;
            }
            set { _SelectionService = value; }
        }
        public MODE DesignMode { get; set; }
        public static DesignerCanvas _designerCanvas { get; set; }
        
        public DClearPopup _DClearPopup;
        private static MD5 _canvasMd5;
        private readonly Dictionary<string, TimerDelegateInfo> _timerDelegateList = new Dictionary<string, TimerDelegateInfo>();//委托缓存
        public double _CreateWidth;                     //表示画布创建时，使用的分辨率宽度,当与显示时的宽度不一至时，则需要重新加载
        public double _CretaeHeight;                    //表示画布创建时，使用的分辨率高度
        private Point? _rubberbandSelectionStartPoint;  //start point of the rubberband drag operation
        private SelectionService _SelectionService;
        public const string _DefaultCanvasName="画布";

        static DesignerCanvas()
        {
            _canvasMd5 = MD5.Create();
        }
        public DesignerCanvas()
        {
            IntlizeCommands();
            this.Name = "asdf";
            this.Loaded += DesignerCanvas_Loaded;
            //this.BaissObject.Clear();
            CompositionTarget.Rendering += CompositionTarget_Rendering;//利用界面定时器定时刷新界面动画
        }
        private void DesignerCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            _designerCanvas = this;
        }
        private void CompositionTarget_Rendering(object sender, EventArgs e)//利用界面定时器定时刷新界面动画
        {
            try
            {
                var tlist = new List<TimerDelegateInfo>();
                lock (_timerDelegateList)
                {
                    tlist.AddRange(_timerDelegateList.Values);
                    _timerDelegateList.Clear();
                }
                tlist.ForEach(d => { d.act.DynamicInvoke(d.param); });
                tlist.Clear();
            }
            catch// (Exception ex)
            {
                //LogManager.Instance.AddLog(LogEventType.Error,"界面刷新", ex);//@gw 2013.09.24 add
            }
        }
       
        public void Remove(DesignerContainer item)
        {
            //OmniObjects.Remove(item.OmniObjects);
            Children.Remove(item);
        }
        public void Add(DesignerContainer item)//向画布上添加图元
        {
            try
            {
                DesignerItem ctrlDI = item.Content as DesignerItem;//content有时会空
                //item.MouseDoubleClick += Item_MouseDoubleClick;
                //string strrr = dittt._displayName;
                //dittt.PreviewMouseDoubleClick += Dittt_PreviewMouseDoubleClick;  8aaba731-f66a-4b5b-8a16-0190969879b4
                //if (item.Item == null) throw new Exception();
                if (String.IsNullOrEmpty(item.Name)) item.Name = ctrlDI._displayName;
                
                this.Children.Add(item);
                #region

                //检查保存的tag内容
                //注意：需要考虑对象改名的问题
                //此处用于处理MODEL与VIEW的直接绑定
                //不处理单独Omni对象的绑定
                string s = item.Item.Tag as string;

                if (s != null)
                {
                    #region
                    var xe = XElement.Load(new StringReader(s));

                    if (xe.Name == "Model")
                    {
                        var xe2 = new XElement("Linkage");
                        #region
                        //从系统中查找绑定对象
                        //var obj = Pattern.Omniframe.Instance.GetOmniObjectByFullNameFromFrame(xe.Value);
                        //if (obj == null) return;
                        //根据元数据进行绑定字段查找
                        //foreach (var o in item.OmniObjects.Where(o => o.Metadatas.ContainsKey(MetadataKey.Field)))
                        //{
                        //    var bo = obj;
                        //    var fm = (o.Metadatas[MetadataKey.Field] as FieldMetadata);
                        //    if (fm != null && fm.FieldName != obj.Name)
                        //    {
                        //        var manager = obj as IOmniObjectManager;
                        //        if (manager != null)
                        //        {
                        //            bo = manager.GetOmniObjectApproach(fm.FieldName);
                        //        }
                        //    }
                        //    if (bo == null) continue;
                        //    //进行双向绑定
                        //    LinkageManager.CreateLinkage(bo.FullName, o.FullName, LinkModeFlag.TwoWay);
                        //    var node = new XElement("Node");
                        //    node.Add(new XElement("Source", bo.FullName),
                        //        new XElement("Target", o.FullName));
                        //    xe2.Add(node);
                        //}
                        #endregion
                        item.Item.Tag = xe2.ToString();
                    }
                    else if (xe.Name == "Linkage")
                    {
                        foreach (var node in xe.Elements("Node"))
                        {
                            var source = node.Element("Source");
                            var target = node.Element("Target");
                            if (source != null && target != null && !string.IsNullOrWhiteSpace(source.Value) && !string.IsNullOrWhiteSpace(target.Value))
                            {
                                try
                                {
                                    //LinkageManager.CreateLinkage(source.Value, target.Value);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("联动配置文件载入失败！");
                                    Console.WriteLine(e.Message);
                                    return;
                                }
                            }
                        }
                    }
                    #endregion
                }
                #endregion
            }
            catch(Exception ex)
            {
                MessageBox.Show("Add---"+ex.Message);
            }
        }
        public void Clear()//清除画布上的图元
        {
            Children.Clear();
        }

        #region 拖动，选择，点击，方向键事件
        public void SelectContainer(DesignerContainer item)
        {
            //if (item == null || !item.Editable) return;
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
            {
                if (item.IsSelected)
                {
                    SelectionService.RemoveFromSelection(item);
                }
                else
                {
                    SelectionService.AddToSelection(item);
                }
            }
            else if (!item.IsSelected)
            {
                SelectionService.SelectItem(item);
            }
            item.Focus();
            var pv = FindName("pv") as PropertyView;
            if (pv != null)
            {
                pv.InitProView();
                if (this.DesignMode == MODE.RUN) pv.ShowProView(true);
                else pv.ShowProView(false);
            }
        }
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)//显示图元属性窗口
        {
            base.OnPreviewMouseDown(e);
            if (_DClearPopup != null) _DClearPopup();
            var item = e.Source as DesignerContainer;
            if (item != null)
            {
                SelectContainer(item);//选择了控件并且显示属性窗口
                //e.Handled = true;
            }
            else if (Equals(e.Source, this))
            {
                _rubberbandSelectionStartPoint = e.GetPosition(this);
                SelectionService.ClearSelection();
                var pv = FindName("pv") as PropertyView;

                if (pv != null)pv.InitProView();
                Focus();
                e.Handled = true;
            }
        }
        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)//右击显示图元类型
        {
            base.OnPreviewMouseRightButtonDown(e);
            DesignerContainer item = e.Source as DesignerContainer;
            if (item != null)
            {
                DesignerItem o = item.Content as DesignerItem;
                o._controlID = item.ID;
                o.ShowWindow();
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)//鼠标移动事件,绘制矩形虚线选择框
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed) _rubberbandSelectionStartPoint = null;
            if (_rubberbandSelectionStartPoint.HasValue)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(this);// create rubberband adorner
                if (adornerLayer != null && DesignMode == MODE.EDIT)
                {
                    var adorner = new RubberbandAdorner(this, _rubberbandSelectionStartPoint);
                    adornerLayer.Add(adorner);
                }
            }
            e.Handled = true;
        }
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            if (e.Data.GetDataPresent(typeof(DesignerItem)))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
        protected override void OnDrop(DragEventArgs e)//画布上放置图元事件
        {
            CommandDrop.Run(CommManager, e);
        }
        protected override Size MeasureOverride(Size constraint)//测量画布上图元的尺寸
        {
            //"参数值必须介于“0”或“35791.3940666667”之间              
            //@gw 20121121 判断size大小不满足要求时返回一个固定大小
            if (constraint.Width > 2000 || constraint.Height > 2000)
                return new Size(500, 500);

            var size = base.MeasureOverride(constraint);

            //foreach (UIElement element in base.Children)
            //{
            //    //double left = Canvas.GetLeft(element);
            //    //double top = Canvas.GetTop(element);
            //    //left = double.IsNaN(left) ? 0 : left;
            //    //top = double.IsNaN(top) ? 0 : top;

            //    //measure desired size for each child
            //    element.Measure(constraint);

            //    //Size desiredSize = element.DesiredSize;
            //    //if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
            //    //{
            //    //    size.Width = Math.Max(size.Width, left + desiredSize.Width);
            //    //    size.Height = Math.Max(size.Height, top + desiredSize.Height);
            //    //}
            //}
            //this.Measure(constraint);
            // add margin 
            //size.Width += 10;
            //size.Height += 10;
            return size;
        }
        protected override void OnKeyDown(KeyEventArgs e)//键盘按下事件
        {
            base.OnKeyDown(e);
            var speed = 1;
            foreach (DesignerContainer item in SelectionService.SelectedDesignerContainer)
            {
                var left = GetLeft(item);
                var top = GetTop(item);

                if (double.IsNaN(left)) left = 0;
                if (double.IsNaN(top)) top = 0;

                if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None)
                {
                    speed = 10;
                }

                switch (e.Key)
                {
                    case Key.Down:
                        SetTop(item, GetTop(item) + item.Height + speed > Height ? Height - item.Height : GetTop(item) + speed);
                        e.Handled = true;
                        break;
                    case Key.Left:
                        SetLeft(item, GetLeft(item) < +speed ? 0 : GetLeft(item) - speed);
                        e.Handled = true;
                        break;
                    case Key.Right:
                        SetLeft(item,
                            GetLeft(item) + item.Width + speed > Width ? Width - item.Width : GetLeft(item) + speed);
                        e.Handled = true;
                        break;
                    case Key.Up:
                        SetTop(item, GetTop(item) < +speed ? 0 : GetTop(item) - speed);
                        e.Handled = true;
                        break;
                    default:
                        e.Handled = false;
                        break;
                }
            }
        }
        #endregion

        public List<DesignerContainer> DeserializeDesignerItems(XElement root)// 从xml文件解析图元
        {
            //CommandOpen.Run(CommManager, root);
            Dictionary<Guid, DesignerContainer> items = new Dictionary<Guid, DesignerContainer>();

            IEnumerable<XElement> itemsXml = root.Elements("DesignerContainers").Elements();
            foreach (XElement itemXml in itemsXml)
            {
                #region xml
                /*                
                <DesignerElement Version="1.0" Type="Wss.Foundation.Controls.DesignerContainer, Wss.Foundation">
                  <Name>换热器</Name>
                  <Canvas.Left>522</Canvas.Left>
                  <Canvas.Top>257</Canvas.Top>
                  <Width>142</Width>
                  <Height>200</Height>
                  <Panel.ZIndex>20</Panel.ZIndex>
                  <ItemOpacity>1</ItemOpacity>
                  <LockDesign>False</LockDesign>
                  <WindowTip>False</WindowTip>
                  <NameVisible>True</NameVisible>
                  <NamePosition></NamePosition>
                  <Angle>0</Angle>
                  <BaseContainer.ScaleX>1</BaseContainer.ScaleX>
                  <BaseContainer.ScaleY>1</BaseContainer.ScaleY>
                  <BaseContainer.ID>ba33c2e8-f1d9-4d37-9d54-4d7a4800d720</BaseContainer.ID>
                  <BaseContainer.ParentID>00000000-0000-0000-0000-000000000000</BaseContainer.ParentID>
                  <BaseContainer.IsGroup>False</BaseContainer.IsGroup>
                  <DesignerElement Version="1.0" Type="Common.Package.HeatExchanger, Common.Package" />
                </DesignerElement>
                */
                #endregion
                DesignerContainer item = DesignAttributeService.DeSerializFromXElement(itemXml) as DesignerContainer;// itemXml.ToDesignerContainer(); //从xml序列化成控件
                Add(item);//item.Content有值的
                if (!items.ContainsKey(item.ID))
                {
                    items.Add(item.ID, item);
                    PropertyInfo[] ps = item.Content.GetType().GetProperties();

                    item.IsHitTestVisible = !(GetZIndex(item).Equals(int.MinValue));
                    item.Focusable = item.IsHitTestVisible;
                    item.IsEnabled = item.IsHitTestVisible;
                    item.IsSelected = false;
                    item.PreviewMouseMove += Item_PreviewMouseMove;
                }
            }
            //InvalidateVisual();
            return items.Values.ToList();
        }

        private void Item_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            DesignerContainer dci = sender as DesignerContainer ;
            DesignerItem di = dci.Content as DesignerItem;
            if(di._DShowWindow!=null) di._DShowWindow(di);
        }

        public bool OpenFile(string fullName)//打开文件
        {
            try
            {
                var root = XElement.Load(fullName);
                if (root == null) return false;
                DeserializeDesignerItems(root);
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        #region
        public void InvokTimerDelegate(string itemName, Delegate d, object param)//传入需掉要界面的委托，待界面线程统一调用
        {
            var name = itemName + d.Method.Name;
            var tdi = new TimerDelegateInfo
            {
                act = d,
                param = param
            };
            lock (_timerDelegateList)
            {
                _timerDelegateList[name] = tdi;
            }
        }
        
        private struct TimerDelegateInfo
        {
            public Delegate act;
            public object param;
        }
        #endregion
    }
}
 