using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
//using// Wss.Common;
using Wss.Foundation.Controls;
using Wss.Foundation.Designer;
using Wss.FoundationCore.Controls;
using System.Windows.Input;
using Wss.FoundationCore.Attributes;
using OmniLib.Threading;
//using// Wss.Objects;
//using// Wss.Objects.Linkages;
//using// Wss.Pattern;
//using Wss.Threading;
//using Ecms.Communication.ModBus;
//using Ecms.Communication;

namespace Wss.Foundation
{
    /// <summary>
    /// 绘图区
    /// </summary>
    public class DesignArea : Control
    {
        #region 全局属性
        private readonly RwLockSlim _lock = new RwLockSlim();
        public Dictionary<string, CanvasInfo> DesCanvasDic = new Dictionary<string, CanvasInfo>();
        public double CanvasWidth { get; set; }
        public double CanvasHeight { get; set; }
        
        #region 工具栏 操作栏 属性栏 背景
        private StackPanel _toolBox; //图元工具栏
        private ContentControl ToolbarContent
        {
            get { return Template != null ? Template.FindName("toolBar", this) as ContentControl : null; }
        }
        private ToolBar Toolbar//操作工具栏
        {
            get
            {
                if (Template == null) return null;
                var barContent = Template.FindName("toolBar", this) as ContentControl;
                return barContent != null ? barContent.Content as ToolBar : null;
            }
        }
        private PropertyView PropertyViewWindow
        {
            get { return Template != null ? Template.FindName("pv", this) as PropertyView : null; }
        }
        public DesignerCanvas CurrentCanvas { get; private set; }
        private Viewbox ViewBox
        {
            get
            {
                if (Template != null)
                {
                    return Template.FindName("vb", this) as Viewbox;
                }
                return null;
            }
        }
        private Border Bd
        {
            get { return Template != null ? Template.FindName("bd", this) as Border : null; }
        }
        private MODE _designMode = MODE.EDIT;
        public MODE DesignMode
        {
            get { return _designMode; }
            set
            {
                if (_designMode != value) { _designMode = value; LoadToolBar(); }
            }
        }
        #endregion

        //Point? _dragStartPoint;
        private List<Type> _designerItemTypes;
        #endregion
        public StackPanel Toolbox   //工具栏
        {
            get
            {
                try
                {
                    //this.Template
                    if (Template == null) return null;
                    if (_toolBox != null) return _toolBox;
                    _toolBox = Template.FindName("toolBox", this) as StackPanel;
                    if (_toolBox == null) return null;

                    _designerItemTypes = new List<Type>();
                    var strPath =  AppDomain.CurrentDomain.BaseDirectory + "Package";
                    if (!Directory.Exists(strPath)) Directory.CreateDirectory(strPath);
                    var dinfo = new DirectoryInfo(strPath);
                    var fileInfos = dinfo.GetFiles();
                    if (fileInfos.Length <= 0) throw new Exception("package文件中没有资源文件或dll文件！");

                    var dicGroup = new Dictionary<string, Expander>(); //记录已有的分组信息
                    var dicToolBox = new Dictionary<string, Toolbox>();
                    foreach (var file in fileInfos) //遍历所有dll
                    {
                        if (!file.Name.EndsWith(".dll")) continue;
                        try
                        {
                            Assembly ably = Assembly.LoadFrom(file.FullName);
                            Type[] types = ably.GetTypes();

                            //Type[] types = Common.Package.WaterTank.GetClasses("Common.Package");

                            foreach (var type in types)
                            {
                                if (type.BaseType != typeof(DesignerItem)) continue;
                                DesignerItem item = TypeDescriptor.CreateInstance(null, type, null, null) as DesignerItem;//先创建控件
                                if (item == null) continue;
                                _designerItemTypes.Add(type);
                                item.ToolTip = item._displayName;//控件显示名称
                                //item.MouseDown += Item_MouseDown;
                                string groupName = string.IsNullOrEmpty(item._groupName) ? "杂项" : item._groupName;//控件分组信息

                                #region 依次创建分组控件，工具箱toolbox控件
                                Expander expTemp;
                                Toolbox toolBoxTemp;
                                if (dicGroup.ContainsKey(groupName))
                                {
                                    expTemp = dicGroup[groupName];
                                    toolBoxTemp = dicToolBox[groupName];
                                }
                                else
                                {
                                    Expander exp1 = new Expander();
                                    exp1.Header = groupName;
                                    _toolBox.Children.Add(exp1);
                                    expTemp = exp1;
                                    Toolbox tboxTemp = new Toolbox();
                                    tboxTemp.Name = groupName;
                                    expTemp.Content = tboxTemp;
                                    expTemp.IsExpanded = true;
                                    toolBoxTemp = tboxTemp;

                                    dicGroup.Add(groupName, exp1);
                                    dicToolBox.Add(groupName, tboxTemp);
                                }
                                item.ShowToolBoxStyle();
                                toolBoxTemp.Items.Add(item); //将控件添加到toolBox
                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }

                    return _toolBox;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "获得Toolbox时");
                    return _toolBox;
                }
            }
        }

        public DesignArea()
        {
            try
            {
                CurrentCanvas = null;
                Name = "绘图区";
                //OnMergeResource();
                Style = FindResource("DesignArea") as Style;//找不到DesignArea的Style，Template为空
            }
            catch(Exception ex)
            {

            }

            Loaded += DesignArea_Loaded;
        }
        protected  void OnMergeResource()//ref List<Uri> resources
        {
            ResourceDictionary newRD = new ResourceDictionary();
            newRD.Source = new Uri("Resources/DesignArea.xaml", UriKind.Relative);
            this.Resources.MergedDictionaries.Add(newRD);

            //this.Resources.Add(new Uri(@"Wss.Foundation;component/Resources/DesignArea.xaml", UriKind.Relative));
        }
        private void DesignArea_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadArea();
        }

        #region WPF控件的获取
        public T GetParentObject<T>(DependencyObject obj) where T : FrameworkElement //获得指定元素的父元素
        {
            var parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                if (parent is T)
                {
                    return (T) parent;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
        public List<T> GetChildObjects<T>(DependencyObject obj) where T : FrameworkElement//获得指定元素的所有子元素
        {
            DependencyObject child = null;
            var childList = new List<T>();

            for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T)
                {
                    childList.Add((T) child);
                }
                childList.AddRange(GetChildObjects<T>(child));
            }
            return childList;
        }
        //查找子元素
        public T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;


            for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);


                if (child is T && (((T) child).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T) child;
                }
                grandChild = GetChildObject<T>(child, name);
                if (grandChild != null)
                    return grandChild;
            }
            return null;
        }
        #endregion

        #region 界面加载
        public void LoadToolBar()
        {
            AdornerLayer al = null;
            if (CurrentCanvas != null) al = AdornerLayer.GetAdornerLayer(CurrentCanvas);
            SetCanvasSize();

            //判断当前页面编辑状态  如果是 则显示工具栏 反之。。。
            //DesignMode = MODE.EDIT;
            if (!(Toolbox != null && PropertyViewWindow != null && ToolbarContent != null && Bd != null)) return;
            if (DesignMode == MODE.EDIT)
            {
                Grid grid = this.Template.FindName("gridToolbox", this) as Grid;
                grid.Visibility = System.Windows.Visibility.Visible;
                PropertyViewWindow.Visibility = Visibility.Collapsed;
                ToolbarContent.Visibility = Visibility.Visible;
                Toolbar.Visibility = Visibility.Visible;
                Toolbar.IsEnabled = true;
                if (CurrentCanvas != null)
                {
                    Toolbox.Visibility = Visibility.Visible;
                    CurrentCanvas.AllowDrop = true;
                    CurrentCanvas.IsEnabled = true;
                    CurrentCanvas.Focusable = true;
                    if (CurrentCanvas.ContextMenu != null)
                    {
                        CurrentCanvas.ContextMenu.Visibility = Visibility.Visible;
                        CurrentCanvas.ContextMenu.Focus();
                    }

                    CurrentCanvas.Focus();
                }
                if (al != null)
                {
                    al.IsEnabled = true;
                }
                Bd.BorderThickness = new Thickness(1);
            }
            else
            {
                if (CurrentCanvas != null)
                {
                    CurrentCanvas.SelectionService.SelectedDesignerContainer.Clear();
                    CurrentCanvas.AllowDrop = false;
                    CurrentCanvas.ContextMenu.Visibility = Visibility.Collapsed;
                }
                Toolbox.Visibility = Visibility.Collapsed;
                Grid grid = this.Template.FindName("gridToolbox", this) as Grid;
                grid.Visibility = System.Windows.Visibility.Collapsed;
                PropertyViewWindow.Visibility = Visibility.Collapsed;
                ToolbarContent.Visibility = Visibility.Collapsed;
                Bd.BorderThickness = new Thickness(0);
                if (CurrentCanvas != null)
                {
                    CurrentCanvas.Visibility = System.Windows.Visibility.Visible;
                    foreach (var item in CurrentCanvas.Children.OfType<DesignerContainer>())
                    {
                        item.IsSelected = false;
                    }
                }

                if (al != null)
                {
                    al.IsEnabled = false;
                }
            }

            if (CurrentCanvas == null) return;
            CurrentCanvas.DesignMode = DesignMode;
            foreach (var item in CurrentCanvas.Children.OfType<DesignerContainer>())
            {
                //em.Editable = editable;
                //string str = ((DesignerItem)item.DesignerChild)._displayName;
                //item.IsHitTestVisible = editable;
                item.IsHitTestVisible = true;
                if (item.Item == null) { continue; }
                item.Item.ToolTip = null;
            }
        }
        private DesignerCanvas LoadCanvas(string fileName, Style dcstyle = null)
        {
            SetCanvasSize();
            var dc = new DesignerCanvas
            {
                Width = CanvasWidth,
                Height = CanvasHeight,
                _CreateWidth = CanvasWidth,
                _CretaeHeight = CanvasHeight,
                DesignMode = DesignMode
            };
            if (dcstyle != null) dc.Style = dcstyle;
            else dc.Style = FindResource("StyleDesignerCanvas") as Style;

            if (File.Exists(fileName))  dc.OpenFile(fileName);
            else dc.SaveFile(false);
            return dc;
        }
        public bool LoadFileToCanvas(string canvasName, string filePath)//通过XML序列化成XAML的控件对象
        {
            if (!File.Exists(filePath)) return false;
            
            var fileTime = File.GetLastWriteTimeUtc(filePath);
            canvasName = string.IsNullOrWhiteSpace(canvasName) ? DesignerCanvas._DefaultCanvasName : canvasName.ToLower().Trim();
            return _lock.UpgradeableReadLock(() =>
            {
                try
                {
                    CanvasInfo ci;
                    if (DesCanvasDic.TryGetValue(canvasName, out ci) && ci.FileTime == fileTime) return true;

                    DesignerCanvas dc = LoadCanvas(filePath);//加载
                    dc.DesignMode = this.DesignMode;
                    if (dc == null) return false;

                    //dc.ChangeName(canvasName);
                    if (ci == null)
                    {
                        ci = new CanvasInfo
                        {
                            Name = dc.Name,
                            LoadedFile = filePath,
                        };
                    }
                    else if (ci.Content != null)
                    {
                        var vparent = ci.Content.Parent as Viewbox;
                        if (vparent != null && vparent.Child != null) vparent.Child = null;
                    }
                    ci.Content = dc;
                    ci.FileTime = fileTime;
                    DesCanvasDic[ci.Name] = ci;
                    CurrentCanvas = dc;
                    return true;
                }
                catch(Exception ex)
                {
                    MessageBox.Show("UpgradeableReadLock" + ex.Message);
                    return false;
                }
                
            });
            //return false;
        }
        public void ShowCanvas(string canvasName)
        {
            canvasName = string.IsNullOrWhiteSpace(canvasName)
                ? DesignerCanvas._DefaultCanvasName
                : canvasName.ToLower().Trim();

            var canvas = CheckAndGetCanvas(canvasName);
            if (canvas == null)
            {
                return;
            }
            CurrentCanvas = canvas;
            var vparent = CurrentCanvas.Parent as Viewbox;
            if (vparent != null && vparent.Child != null)
            {
                vparent.Child = null;
            }
            //if (OmniObjects.Count > 0)
            //{
            //    ReloadArea();
            //}
        }
        private DesignerCanvas CheckAndGetCanvas(string canvasName)
        {
            var canvas = _lock.ReadLock(() =>
            {
                if (!DesCanvasDic.ContainsKey(canvasName))
                {
                    return null;
                }
                var ci = DesCanvasDic[canvasName];
                if (string.IsNullOrWhiteSpace(ci.LoadedFile) || !File.Exists(ci.LoadedFile))
                {
                    return ci.Content;
                }
                var fileTime = File.GetLastWriteTimeUtc(ci.LoadedFile);
                if (ci.FileTime != fileTime && !LoadFileToCanvas(ci.Name, ci.LoadedFile))
                {
                    return null;
                }
                return ci.Content;
            });
            return canvas;
        }
        
        public void ReloadArea()//重新加载绘图区
        {
            if (ViewBox == null)
            {
                return;
            }
           
            ApplyTemplate();
            
            //初始化画布区域
            //获取画面文件所在路径
            if (CurrentCanvas == null) CurrentCanvas = ViewBox.Child as DesignerCanvas;
            else CurrentCanvas = CheckAndGetCanvas(CurrentCanvas.Name);

            if (Toolbox == null || ViewBox == null || ToolbarContent == null)
            {
                MessageBox.Show("工作区加载失败,请重新打开!");
                IsEnabled = false;
                if (PropertyViewWindow != null) PropertyViewWindow.ShowProView(false);
                return;
            }
            ViewBox.Child = CurrentCanvas;

            CurrentCanvas.DesignMode = DesignMode;
            CurrentCanvas.Width = CanvasWidth;
            CurrentCanvas.Height = CanvasHeight;
            CurrentCanvas._CreateWidth = CanvasWidth;
            CurrentCanvas._CretaeHeight = CanvasHeight;
            LoadToolBar();
        }
        //设置绘图区的大小
        private void SetCanvasSize()
        {
            //DockPanel dp = null;
            //if (this.Template != null)

            //    dp = this.Template.FindName("dp", this) as DockPanel;

            double width = 0;
            if (!double.IsNaN(ActualWidth) && ActualWidth > 0)
            {
                width = ActualWidth;
            }
            double height = 0;
            if (!double.IsNaN(ActualHeight) && ActualHeight > 0)
            {
                height = ActualHeight;
            }
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Math.Abs(width) < 0.0001 &&
                Math.Abs(height) < 0.0001)
            {
                //不能得到画布大小,画布采用默认大小
                return;
            }

            if (DesignMode == MODE.EDIT)
            {
                //编辑模式
                if (ViewBox == null || Toolbox == null || Toolbar == null) return;
                ViewBox.Width = width - Toolbox.Width;
                ViewBox.Height = height - Toolbar.Height;
            }
            else
            {
                //运行模式
                if (ViewBox == null) return;
                ViewBox.Width = width;
                ViewBox.Height = height;
            }
        }
        #endregion
    }
    public enum MODE//运行状态
    {
        /// <summary>
        ///     运行
        /// </summary>
        RUN = 0,
        /// <summary>
        ///     查看
        /// </summary>
        VIEW = 1,
        /// <summary>
        ///     编辑
        /// </summary>
        EDIT = 2
    }
    public class CanvasInfo
    {
        public string Name { get; set; }
        public DesignerCanvas Content { get; set; }
        public string LoadedFile { get; set; }
        public DateTime FileTime { get; set; }
    }
}