using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Wss.FoundationCore.Attributes;
using Wss.FoundationCore.Models;
//using// Wss.Objects;
using System.Globalization;
using System.Collections;
using Wss.Foundation.Designer;
using Wss.FoundationCore.Controls;

namespace Wss.Foundation.Controls
{
    #region 类属性
    [DesignProperty("旋转角度", "Angle")]
    [DesignProperty("X缩放", "ScaleX")]
    [DesignProperty("Y缩放", "ScaleY")]
    [SerializeProperty("ID")]
    [SerializeProperty("ParentID")]
    [SerializeProperty("IsGroup")]
    [SerializeProperty("Angle")]
    [SerializeProperty("ScaleX")]
    [SerializeProperty("ScaleY")]
    [TemplatePart(Name = "PART_DragThumb", Type = typeof(DragThumb))]
    [TemplatePart(Name = "PART_ResizeDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ConnectorDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_PopupClose", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_TopLeftArea", Type = typeof(Border))]
    [TemplatePart(Name = "PART_TopRightArea", Type = typeof(Border))]
    [TemplatePart(Name = "lb_ConfigViewPropertys", Type = typeof(ListBox))]
    #endregion
    public class DesignerContainer : BaseContainer
    {
        public bool _hasLoadCompleted = false;
        private Popup _popup;
        private bool _isLoadedConfigData = false;
        private ListBox lb_lb_ConfigViewPropertys;
        #region 依赖属性
        #region 可编辑

        /// <summary>
        ///     可编辑
        /// </summary>
        public bool Editable
        {
            get { return (bool)GetValue(EditableProperty); }
            set
            {
                if (value)
                {
                    if (!Lock) SetValue(EditableProperty, true);
                }
                else
                    SetValue(EditableProperty, false);
            }
        }

        public static readonly DependencyProperty EditableProperty =
            DependencyProperty.Register("Editable", typeof(bool), typeof(DesignerContainer),
                new UIPropertyMetadata(false));

        #endregion

        #region 不透明度

        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [DisplayName("透明度")]
        public double ItemOpacity
        {
            get { return (double)GetValue(ItemOpacityProperty); }
            set { SetValue(ItemOpacityProperty, value); }
        }

        public static readonly DependencyProperty ItemOpacityProperty =
            DependencyProperty.Register("ItemOpacity", typeof(double), typeof(DesignerContainer),
                new UIPropertyMetadata(1.0, ItemOpacityChanged));

        public static void ItemOpacityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var di = sender as DesignerContainer;
            if (di == null) return;
            di.Item.Opacity = (double)e.NewValue;
        }

        #endregion

        #region 锁定图元
        [Designable]
        [DisplayName("锁定元素")]
        [Wss.FoundationCore.Attributes.Serializable]
        public bool LockDesign { get; set; }
        #endregion

        [Wss.FoundationCore.Attributes.Serializable]
        [DisplayName("名称提示")]
        public bool WindowTip
        {
            get { return (bool)GetValue(WindowTipProperty); }
            set { SetValue(WindowTipProperty, value); }
        }

        public static readonly DependencyProperty WindowTipProperty = DependencyProperty.Register("WindowTip", typeof(bool), typeof(DesignerContainer), new UIPropertyMetadata(false));

        #region 标签可视状态

        /// <summary>
        ///     标签可视状态
        /// </summary>
        [Designable]
        [DisplayName("名称可见")]
        [Wss.FoundationCore.Attributes.Serializable]
        public bool NameVisible
        {
            get { return (bool)GetValue(NameVisibleProperty); }
            set { SetValue(NameVisibleProperty, value); }
        }

        public static readonly DependencyProperty NameVisibleProperty =
            DependencyProperty.Register("NameVisible", typeof(bool), typeof(DesignerContainer),
                new UIPropertyMetadata(true, NameVisibleChange));

        public static void NameVisibleChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = sender as DesignerContainer;
            if (item == null) return;

            try
            {
                var tb = item.Template.FindName("tbTag", item) as TextBlock;
                if (tb == null || item.ParentID != new Guid()) return;
                tb.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Hidden;
            }
            catch
            {
                // ignored
            }
        }

        #endregion

        #region 名称显示位置

        [Designable]
        [DisplayName("名称位置")]
        [Wss.FoundationCore.Attributes.Serializable]
        public string NamePosition
        {
            get { return (string)GetValue(NamePositionProperty); }
            set { SetValue(NamePositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NamePosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NamePositionProperty =
            DependencyProperty.Register("NamePosition", typeof(string), typeof(DesignerContainer),
                new UIPropertyMetadata("", NamePositionChanged));

        public static void NamePositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = sender as DesignerContainer;
            ;
            if (item == null) return;

            var nameInfo = e.NewValue.ToString().Split('|');
            var tb = item.Template.FindName("tbTag", item) as TextBlock;
            try
            {
                if (tb != null)
                {
                    tb.HorizontalAlignment = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), nameInfo[0]);
                    tb.VerticalAlignment = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), nameInfo[1]);
                    var tc = new ThicknessConverter();
                    var convertFromString = tc.ConvertFromString(nameInfo[2]);
                    if (convertFromString != null)
                    {
                        var t = (Thickness)convertFromString;
                        tb.Margin = t;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        #endregion

        #region DragThumbTemplate Property

        // can be used to replace the default template for the DragThumb
        public static readonly DependencyProperty DragThumbTemplateProperty =
            DependencyProperty.RegisterAttached("DragThumbTemplate", typeof(ControlTemplate),
                typeof(DesignerContainer));

        public static ControlTemplate GetDragThumbTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(DragThumbTemplateProperty);
        }

        public static void SetDragThumbTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(DragThumbTemplateProperty, value);
        }
        #endregion

        /// <summary>
        /// 取得图元所在画布
        /// </summary>
        public DesignerCanvas ParentCanvas
        {
            get
            {
                var obj = VisualTreeHelper.GetParent(this);
                while ((obj != null) && !(obj is DesignerCanvas))
                {
                    obj = VisualTreeHelper.GetParent(obj);
                }
                return obj as DesignerCanvas;
            }
        }
        public string StyleName { get; set; }

        /// <summary>
        ///     tooltip框内的信息源
        /// </summary>
        public ObservableCollection<UnitOmniProperty> InfoViewPropertys { get; set; }//杨洋写的
        /// <summary>
        ///     点击框内的信息源
        /// </summary>
        public ObservableCollection<UnitOmniProperty> ConfigViewPropertys { get; set; }
        /// <summary>
        ///     左上角内的信息源
        /// </summary>
        public ObservableCollection<UnitOmniProperty> ModeViewPropertys { get; set; }
        /// <summary>
        ///     右上角内的信息源
        /// </summary>
        public ObservableCollection<UnitOmniProperty> DataViewPropertys { get; set; }
        #endregion

        #region 构造
        static DesignerContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata( typeof(DesignerContainer), new FrameworkPropertyMetadata(typeof(DesignerContainer)));
        }
        public DesignerContainer(Guid id, string name): this(id)
        {
            Name = name;
        }
        public DesignerContainer() : this(Guid.NewGuid())
        {
        }
        public DesignerContainer(Guid id)
        {
            Loaded += DesignerContainer_Loaded;
            //AddedToCanvas += DesignerContainer_AddedToCanvas;

            ID = id;
            InfoViewPropertys = new ObservableCollection<UnitOmniProperty>();
            ConfigViewPropertys = new ObservableCollection<UnitOmniProperty>();
            DataViewPropertys = new ObservableCollection<UnitOmniProperty>();
            ModeViewPropertys = new ObservableCollection<UnitOmniProperty>();
        }
        #endregion

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            if (ParentCanvas == null) return;
            Editable = ParentCanvas.DesignMode.Equals(MODE.EDIT);
        }
        private void DesignerContainer_AddedToCanvas(object sender, RoutedEventArgs e)
        {
            if (ParentCanvas == null) return;

            Editable = ParentCanvas.DesignMode.Equals(MODE.EDIT);
            SetContent();
        }
        //图元加载完成
        protected virtual void DesignerContainer_Loaded(object sender, RoutedEventArgs e)
        {
            if (ParentCanvas != null)
            {
                SetContent();
            }
        }
        public T FindFirstVisualChild2<T>(DependencyObject obj, string childName)where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T && child.GetValue(NameProperty).ToString() == childName)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindFirstVisualChild2<T>(child, childName);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }
        public override void OnApplyTemplate()
        {
            _popup = Template.FindName("PART_Popup", this) as Popup;
            var closeButton = Template.FindName("PART_PopupClose", this) as Button;

            if (_popup != null)
            {
                _popup.Opened += Popup_Opened;
                closeButton.Click += closeButton_Click;
                lb_lb_ConfigViewPropertys = Template.FindName("lb_ConfigViewPropertys", this) as ListBox;

            }
            base.OnApplyTemplate();
        }
        public T GetChild<T>(DependencyObject obj) where T : DependencyObject
        {
            DependencyObject child = null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child.GetType() == typeof(T))
                    break;
                else if (child != null)
                {
                    child = GetChild<T>(child);
                    if (child != null && child.GetType() == typeof(T))

                        break;
                }
            }
            return child as T;
        }
        private void Popup_Opened(object sender, EventArgs e)
        {
            if (lb_lb_ConfigViewPropertys != null)
            {
                for (int i = 0; i < lb_lb_ConfigViewPropertys.Items.Count; i++)
                {
                    ListBoxItem listitem = (ListBoxItem)(lb_lb_ConfigViewPropertys.ItemContainerGenerator.ContainerFromItem(lb_lb_ConfigViewPropertys.Items[i]));
                    if (listitem != null)
                    {
                        var txt = GetChild<TextBox>(listitem);
                        if (txt != null)
                        {
                            BindingExpression bind = txt.GetBindingExpression(TextBox.TextProperty);
                            bind.UpdateTarget();
                            continue;
                        }

                        var rdo = GetChild<AyRadioList>(listitem);
                        if (rdo != null)
                        {
                            //var a = rdo.DataContext as IGeneralValueable;
                            //if (a != null)
                            //{
                            //    rdo.CheckedRadioValue=a.Value;
                            //}
                            continue;
                        }
                    }
                }
            }
        }
        protected override void OnClick()//控件单击事件
        {
            base.OnClick();
            if (!Editable && _popup != null)//杨洋写的
            {
                //_popup.IsOpen = true;
                //if (!_isLoadedConfigData)
                //{
                //    var lbCurrent = Template.FindName("lb_ConfigViewPropertys", this) as ListBox;

                //    if (lbCurrent != null)
                //    {
                //        foreach (object o in lbCurrent.Items)
                //        {
                //            ListBoxItem lbi = lbCurrent.ItemContainerGenerator.ContainerFromItem(o) as ListBoxItem;
                //            if (lbi != null)
                //            {
                //                Button btnSendValue = FindFirstVisualChild2<Button>(lbi, "btnSendValue");
                //                if (btnSendValue != null)
                //                {
                //                    btnSendValue.Click += btnSendValue_Click;
                //                }

                //            }
                //        }
                //    }
                //    _isLoadedConfigData = true;
                //}
            }
        }
        private void btnSendValue_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                var rdo = btn.Tag as AyRadioList;

                if (rdo != null)
                {
                    //var a = rdo.DataContext as IOmniValueWriteable;
                    //if (a != null)
                    //{
                    //    a.SetValue(rdo.CheckedRadioValue);
                    //}
                    return;
                }
                else
                {
                    var txt = btn.Tag as TextBox;
                    if (txt != null)
                    {
                        if (string.IsNullOrEmpty(txt.Text))
                        {
                            txt.Focus();
                            MessageBox.Show("值不能为空");
                            return;
                        }
                        #region
                        //var itemvalue = txt.DataContext as IGeneralValueable;
                        //if (itemvalue != null)
                        //{
                        //    if (itemvalue.Value.GetTypeCode() == TypeCode.Double ||
                        //        itemvalue.Value.GetTypeCode() == TypeCode.Int32 ||
                        //        itemvalue.Value.GetTypeCode() == TypeCode.Int16)
                        //    {
                        //        double itemValue_out;
                        //        if (double.TryParse(txt.Text, out itemValue_out))
                        //        {
                        //            itemvalue.SetValue(itemValue_out);
                        //        }
                        //        else
                        //        {
                        //            txt.SelectAll();
                        //            txt.Focus();
                        //            MessageBox.Show("值必须是数字");
                        //            return;
                        //        }
                        //    }
                        //    else if (itemvalue.Value.GetTypeCode() == TypeCode.String)
                        //    {
                        //        if (txt.Text.Length > 100)
                        //        {
                        //            txt.SelectAll();
                        //            txt.Focus();
                        //            MessageBox.Show("值的字符长度不能超过100");
                        //        }
                        //        else
                        //        {
                        //            itemvalue.SetValue(txt.Text);
                        //        }
                        //    }
                        //}
                        #endregion
                    }
                }
            }
        }
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_popup != null)
            {
                _popup.IsOpen = false;
                e.Handled = true;
            }
        }

        #region 2015-7-3 16:21:40 ay增加
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private object toolTipContent;
        public object ToolTipContent
        {
            get { return toolTipContent; }
            set
            {
                toolTipContent = value;
                OnPropertyChanged("ToolTipContent");
            }
        }
        private object popupContent;
        public object PopupContent
        {
            get { return popupContent; }
            set
            {
                popupContent = value;
                OnPropertyChanged("PopupContent");
            }
        }
        #endregion

        #region 锁定图元
        public bool Lock
        {
            get { return (bool)GetValue(LockProperty); }
            set { SetValue(LockProperty, value); }
        }

        public static readonly DependencyProperty LockProperty = DependencyProperty.Register("Lock", typeof(bool), typeof(DesignerContainer),new UIPropertyMetadata(false, OnLockPropertyChanged));
        private static void OnLockPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DesignerContainer s = sender as DesignerContainer;
            if (s == null) return;
            s.Editable = !(bool)e.NewValue;
            if (!s.Editable)
            {
                s.IsSelected = true;
            }
        }
        #endregion

        /// <summary>
        ///     设置内容控件
        /// </summary>
        private void SetContent()
        {
            if (Template == null) return;
            var contentPresenter = Template.FindName("PART_ContentPresenter", this) as ContentPresenter;
            if (contentPresenter != null)
            {
                UIElement contentVisual;
                if (VisualTreeHelper.GetChildrenCount(contentPresenter) > 0)
                {
                    contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;
                }
                else
                {
                    contentVisual = Item;
                }
                if (contentVisual != null)
                {
                    var thumb = Template.FindName("PART_DragThumb", this) as DragThumb;
                    if (thumb != null)
                    {
                        var template = GetDragThumbTemplate(contentVisual);
                        if (template != null)
                            thumb.Template = template;
                    }
                    var cc = contentVisual as ContentControl;
                    if (cc != null)
                    {
                        if (!Double.IsNaN(cc.MinHeight) && cc.MinHeight != 0)
                        {
                            if (String.IsNullOrEmpty(StyleName))
                            {
                                Height = cc.MinHeight * 2;
                            }
                            MinHeight = cc.MinHeight;
                        }
                        if (!Double.IsNaN(cc.MinWidth) && cc.MinWidth != 0)
                        {
                            if (String.IsNullOrEmpty(StyleName))
                            {
                                Width = cc.MinWidth * 2;
                            }
                            MinWidth = cc.MinWidth;
                        }
                    }
                }
            }
            InitTag();
        }
        //初始化逻辑设备，（1）在从文件中加载时需要调用，（2）更改名称时调用。
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            var g = Template.FindName("grd", this) as Canvas;
            if (g != null)
            {
                g.ContextMenu = FindResource("DesignerContainerContextMenu") as ContextMenu;
            }
            if (!Editable && g != null)
            {
                if (g.ContextMenu != null)
                {
                    g.ContextMenu.Visibility = Visibility.Collapsed;
                }
                return;
            }
            if (g != null)
            {
                if (g.ContextMenu != null)
                {
                    g.ContextMenu.Visibility = Visibility.Visible;
                    g.ContextMenu.Focus();
                    g.Focus();
                }
            }
            //e.Handled = true;
        }
        /// <summary>
        ///     初始化名称标签
        /// </summary>
        private void InitTag()
        {
            var nameInfo = NamePosition.Split('|');

            var tb = Template.FindName("tbTag", this) as TextBlock;
            if (tb == null)
            {
                return;
            }

            tb.Visibility = NameVisible ? Visibility.Visible : Visibility.Hidden;
            if (nameInfo.Length < 3)
            {
                return;
            }
            try
            {
                tb.HorizontalAlignment = String.IsNullOrWhiteSpace(nameInfo[0])
                    ? tb.HorizontalAlignment
                    : (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), nameInfo[0]);
                tb.VerticalAlignment = String.IsNullOrWhiteSpace(nameInfo[1])
                    ? tb.VerticalAlignment
                    : (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), nameInfo[1]);
                var tc = new ThicknessConverter();
                var convertFromString = tc.ConvertFromString(nameInfo[2]);
                if (convertFromString != null)
                    tb.Margin = String.IsNullOrWhiteSpace(nameInfo[2]) ? tb.Margin : (Thickness)convertFromString;
            }
            catch (Exception)
            {
                // LogManager.Instance.AddLog(LogEventType.Warning, e, "异常日志");
            }
        }
        public static bool GetDisabledWidth(DesignerContainer m_DesignerItem)
        {
            var disabledW = (m_DesignerItem.Item != null && (!Double.IsNaN(m_DesignerItem.Item.MinWidth) &&
                                                             m_DesignerItem.Item.MinWidth != 0 &&
                                                             m_DesignerItem.Item.MinWidth ==
                                                             m_DesignerItem.Item.MaxWidth &&
                                                             m_DesignerItem.Item.GetType() != typeof(DesignerContainer))) ||
                            (m_DesignerItem.Content is ContentControl &&
                             (m_DesignerItem.Content as ContentControl).MaxWidth != 0 &&
                             !Double.IsInfinity((m_DesignerItem.Content as ContentControl).MaxWidth) &&
                             (m_DesignerItem.Content as ContentControl).MaxWidth ==
                             (m_DesignerItem.Content as ContentControl).MinWidth);
            return disabledW;
        }
        public static bool GetDisabledHeight(DesignerContainer m_DesignerItem)
        {
            var disabledH =
                (m_DesignerItem.Item != null && (!Double.IsNaN(m_DesignerItem.Item.MinHeight) &&
                                                 m_DesignerItem.Item.MinHeight != 0 &&
                                                 m_DesignerItem.Item.MaxHeight == m_DesignerItem.Item.MinHeight)) ||
                (m_DesignerItem.Content is ContentControl &&
                 (m_DesignerItem.Content as ContentControl).MaxHeight != 0 &&
                 !Double.IsInfinity((m_DesignerItem.Content as ContentControl).MaxHeight) &&
                 (m_DesignerItem.Content as ContentControl).MaxHeight ==
                 (m_DesignerItem.Content as ContentControl).MinHeight);
            return disabledH;
        }

        #region 不用
        ///// <summary>
        ///// 名称位置属性
        ///// </summary>
        ///// <param name="pv"></param>
        //private void AddNamePosition(PropertyView pv)
        //{
        //    TextBlock tbTag = this.Template.FindName("tbTag", this) as TextBlock;
        //    string[] nameInfo = new string[3];

        //    ComboBox cbHA = new ComboBox();
        //    cbHA.MaxWidth = 80;
        //    cbHA.Items.Add(new ComboBoxItem() { Content = "Left", Tag = "Left" });
        //    cbHA.Items.Add(new ComboBoxItem() { Content = "Center", Tag = "Center" });
        //    cbHA.Items.Add(new ComboBoxItem() { Content = "Right", Tag = "Right" });
        //    Binding bdHA = new Binding("HorizontalAlignment") { Source = tbTag };
        //    bdHA.Mode = BindingMode.TwoWay;
        //    bdHA.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        //    cbHA.SetBinding(ComboBox.TextProperty, bdHA);
        //    nameInfo[0] = (cbHA.SelectedItem as ComboBoxItem).Content.ToString();
        //    cbHA.SelectionChanged += new SelectionChangedEventHandler((sender, e) => { nameInfo[0] = (cbHA.SelectedItem as ComboBoxItem).Content.ToString(); NamePosition = ArrToStr(nameInfo); });
        //    if (NamePosition.Split('|').Length > 2)
        //    {
        //        foreach (ComboBoxItem Item in cbHA.Items)
        //        {
        //            if (Item.Content.ToString().Equals(NamePosition.Split('|')[0]))
        //            {
        //                Item.IsSelected = true;
        //                break;
        //            }
        //        }
        //    }
        //    pv.AddProItem("名称水平", cbHA);

        //    ComboBox cbVA = new ComboBox();
        //    cbVA.MaxWidth = 80;
        //    cbVA.Items.Add(new ComboBoxItem() { Content = "Top", Tag = "Top" });
        //    cbVA.Items.Add(new ComboBoxItem() { Content = "Center", Tag = "Center" });
        //    cbVA.Items.Add(new ComboBoxItem() { Content = "Bottom", Tag = "Bottom" });
        //    Binding bdVA = new Binding("VerticalAlignment") { Source = tbTag };
        //    bdVA.Mode = BindingMode.TwoWay;
        //    bdVA.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        //    cbVA.SetBinding(ComboBox.TextProperty, bdVA);
        //    nameInfo[1] = (cbVA.SelectedItem as ComboBoxItem).Content.ToString();
        //    cbVA.SelectionChanged += new SelectionChangedEventHandler((sender, e) => { nameInfo[1] = (cbVA.SelectedItem as ComboBoxItem).Content.ToString(); NamePosition = ArrToStr(nameInfo); });
        //    if (NamePosition.Split('|').Length > 2)
        //    {
        //        foreach (ComboBoxItem Item in cbVA.Items)
        //        {
        //            if (Item.Content.ToString().Equals(NamePosition.Split('|')[1]))
        //            {
        //                Item.IsSelected = true;
        //                break;
        //            }
        //        }
        //    }
        //    pv.AddProItem("名称垂直", cbVA);

        //    TextBox tbOffset = new TextBox();
        //    Binding bdOffset = new Binding("Margin") { Source = tbTag };
        //    bdOffset.Mode = BindingMode.TwoWay;
        //    bdOffset.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        //    tbOffset.SetBinding(TextBox.TextProperty, bdOffset);
        //    nameInfo[2] = tbOffset.Text;
        //    tbOffset.TextChanged += new TextChangedEventHandler((sender, e) => { nameInfo[2] = tbOffset.Text; NamePosition = ArrToStr(nameInfo); });
        //    pv.AddProItem("名称偏移", tbOffset);
        //}
        ///// <summary>
        ///// 数值转换为字符串
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //private string ArrToStr(string[] input)
        //{
        //    string result = "";
        //    foreach (string Item in input)
        //    {
        //        result += Item + "|";
        //    }
        //    return result.Substring(0, result.Length - 1);
        //}
        #endregion

        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [DisplayName("旋转角度")]
        public double Angle //旋转角度
        {
            get { return (double)GetValue(BaseContainer.AngleProperty); }
            set { SetValue(BaseContainer.AngleProperty, value); }
        }
    }

    #region
    public class NumberConverter : IValueConverter
    {
        public double PlusValue { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double a = (double)value;
            return a * PlusValue;

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double a = (double)value;
            return a / PlusValue; ;
        }
    }
    public class InfoViewPropertysVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var a = value as ICollection;
            if (a == null || a.Count == 0)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
    #endregion
}