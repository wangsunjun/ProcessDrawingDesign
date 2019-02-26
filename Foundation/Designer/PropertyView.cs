using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Wss.FoundationCore.Attributes;
using Wss.Foundation.Controls;
using Binding = System.Windows.Data.Binding;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using Control = System.Windows.Controls.Control;
using Label = System.Windows.Controls.Label;
using ListView = System.Windows.Controls.ListView;
using ListViewItem = System.Windows.Controls.ListViewItem;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using TextBox = System.Windows.Controls.TextBox;
using System.Collections.Generic;

namespace Wss.Foundation.Designer
{
    /// <summary>
    /// gw_展示图元属性窗体类
    /// </summary>
    public class PropertyView : Control
    {
        private static bool _isLoaded;
        private Button _btnLink;
        private DesignerCanvas _designerCanvas;
        //private DesignerContainer _mDesignerItem;
        private Point _position;
        private Thickness _margin;
        private double _mousePointX;
        private double _mousePointY;

        public PropertyView()
        {
            this.Loaded += PropertyView_Loaded;
        }
        private void PropertyView_Loaded(object sender, RoutedEventArgs e)
        {
            InitProView();
            ApplyTemplate();
            if (!_isLoaded)
            {
                _isLoaded = true;
                AddBtnLinkEvent();
            }
        }

        //显示属性窗口
        public void ShowProView(bool isShowContainerPro)
        {
            if (_designerCanvas == null || _designerCanvas.SelectionService == null) return;
            var title = Template.FindName("title", this) as Label;
            if (title != null)
            {
                title.MouseLeftButtonDown += title_MouseLeftButtonDown;
                title.MouseLeftButtonUp += title_MouseLeftButtonUp;
                title.MouseRightButtonDown += title_MouseRightButtonDown;
            }
            if (_designerCanvas.SelectionService.SelectedDesignerContainer == null || _designerCanvas.SelectionService.SelectedDesignerContainer.Count <= 0) return;
            //画布上有选择的控件
            List<DesignerContainer> listCtn = _designerCanvas.SelectionService.SelectedDesignerContainer.OfType<DesignerContainer>().Where(x => x.ParentID == Guid.Empty).ToList();
            if (listCtn.Count != 1) { this.Visibility = Visibility.Collapsed; return; }                

            ListView lvName = Template.FindName("lvName", this) as ListView;
            ListView lvValue = Template.FindName("lvValue", this) as ListView;//找到属性窗口的listView
            lvName.Items.Clear(); lvValue.Items.Clear();

            DependencyObject dobj = listCtn.First();//显示第一个被选择的控件的属性
            var pros = DesignAttributeService.GetDesignPropertys(dobj).OrderBy(o => o.DisplayName);//获取属性列表
            foreach (IDesignProperty p in pros)
            {
                if(isShowContainerPro)
                    if(p.Owner.ToString().Contains("DesignerContainer")) continue;
                
                FrameworkElement ctr;
                DependencyProperty ctrpp = TextBox.TextProperty;
                if (p.Editer != null)
                {
                    PropertyEditerButton bt = new PropertyEditerButton(p.Editer);
                    ctr = bt;
                    bt.Content = "编 辑";
                    ctrpp = PropertyEditerButton._property;
                }
                else
                {
                    #region 根据属性的类型，设定属性窗口该给什么控件
                    if (p.Descriptor != null)
                    {
                        if (p.Descriptor.PropertyType == typeof(bool))
                        {
                            ctr = new CheckBox();
                            ctrpp = ToggleButton.IsCheckedProperty;
                        }
                        else if (p.Descriptor.PropertyType == typeof(Brush))
                        {
                            TextBox tb = new TextBox();
                            tb.MouseDoubleClick += tb_MouseDoubleClick;//显示颜色面板
                            ctr = tb;
                            ctrpp = TextBox.TextProperty;
                        }
                        else if (p.Descriptor.PropertyType == typeof(Point))//坐标
                        {
                            TextBox tb = new TextBox();
                            tb.PreviewMouseRightButtonDown += tb_PreviewMouseRightButtonDown;
                            tb.MouseMove += tb_MouseMove1;
                            tb.MouseRightButtonUp += tb_MouseRightButtonUp;
                            ctr = tb;
                        }
                        else
                        {
                            ctr = new TextBox();
                            if (p.DisplayName.Equals("名称"))
                            {
                                p.Binding.ValidationRules.Add(new NameRule());
                            }
                        }
                    }
                    else
                    {
                        ctr = new TextBox();
                        if (p.DisplayName.Equals("名称"))
                        {
                            p.Binding.ValidationRules.Add(new NameRule());
                        }
                    }
                    #endregion
                }
                ctr.IsEnabled = !p.ReadOnly;
                ctr.SetBinding(ctrpp, p.Binding);

                AddProItem(p.DisplayName, ctr);
            }
            Visibility = Visibility.Visible;
        }
        //属性窗口标题栏右键单击收缩展开
        private void title_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var t = sender as Label;
            if (Height == t.ActualHeight)
            {
                var lc = new LengthConverter();
                Height = (double) lc.ConvertFromString("auto");
            }
            else
            {
                Height = t.ActualHeight;
            }
            e.Handled = true;
        }
        //属性列表初始化
        public void InitProView()
        {
            var da = TemplatedParent as DesignArea;
            if (da == null)
            {
                return;
            }
            _designerCanvas = da.CurrentCanvas;
            if (_designerCanvas == null || double.IsNaN(_designerCanvas.Width)) return;

            _margin.Top = 5;
            _margin.Right = 0;
            Visibility = Visibility.Collapsed;
            Margin = _margin;
        }

        //绑定属性列表 item 选定的图元
        private void CtlInfoBind(DesignerContainer item)
        {
            //PropertyList pl = Item.Item.ctlInfo;
            // ShowPropertyList(Item);//lxd 修改 王运钢修改-2011-05-16
            AddNamePosition(item);
        }
        
        #region 文本框右键改变数值

        private void tb_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && tb.IsMouseCaptured)
            {
                tb.ReleaseMouseCapture();
            }
            e.Handled = true;
        }
        private void tb_MouseMove(object sender, MouseEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && e.RightButton == MouseButtonState.Pressed)
            {
                if (!tb.IsMouseCaptured)
                {
                    tb.CaptureMouse();
                }
                var value = double.Parse(tb.Text);
                value += e.GetPosition(DesignerCanvas._designerCanvas).Y - _mousePointY;
                _mousePointY = e.GetPosition(DesignerCanvas._designerCanvas).Y;
                tb.Text = value.ToString();
            }
        }
        private void tb_MouseMove1(object sender, MouseEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && e.RightButton == MouseButtonState.Pressed)
            {
                if (!tb.IsMouseCaptured)
                {
                    tb.CaptureMouse();
                }
                var value = (Point) new PointConverter().ConvertFromString(tb.Text);
                value.X += e.GetPosition(DesignerCanvas._designerCanvas).X - _mousePointX;
                value.Y += e.GetPosition(DesignerCanvas._designerCanvas).Y - _mousePointY;
                _mousePointX = e.GetPosition(DesignerCanvas._designerCanvas).X;
                _mousePointY = e.GetPosition(DesignerCanvas._designerCanvas).Y;
                tb.Text = value.ToString();
            }
        }

        private void tb_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var tb = sender as TextBox;
            _mousePointX = e.GetPosition(DesignerCanvas._designerCanvas).X;
            _mousePointY = e.GetPosition(DesignerCanvas._designerCanvas).Y;
            if (!tb.IsMouseCaptured)
            {
                tb.CaptureMouse();
            }
            e.Handled = true;
        }
        #endregion
        #region 属性窗口拖动
        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var t = sender as Label;
            if (!t.IsMouseCaptured) t.CaptureMouse();
            _position = e.GetPosition(this);
            e.Handled = true;
        }

        private void title_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MovePropertyWindow(sender, e);
        }

        /// <summary>
        ///     移动属性窗口@gw 20120831修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MovePropertyWindow(object sender, MouseButtonEventArgs e)
        {
            var da = TemplatedParent as DesignArea;

            var t = sender as Label;
            t.ReleaseMouseCapture();
            if (_margin.Left + e.GetPosition(this).X - _position.X < 0) //zhang 12.17 限制属性窗口的移动范围
            {
                _margin.Left = 0;
            }
            else if (_margin.Left + e.GetPosition(this).X - _position.X > da.ActualWidth - Width * 1.5)
            {
                _margin.Left = da.ActualWidth - Width * 1.5;
            }
            else
            {
                _margin.Left += (e.GetPosition(this).X - _position.X);
            }
            if (_margin.Top + e.GetPosition(this).Y - _position.Y < 0)
            {
                _margin.Top = 0;
            }
            else if (_margin.Top + e.GetPosition(this).Y - _position.Y > da.ActualHeight)
            {
                _margin.Top = da.ActualHeight;
            }
            else
            {
                _margin.Top += (e.GetPosition(this).Y - _position.Y);
            }
            Margin = _margin;
            //PX = margin.Left;
            //PY = margin.Top;
            _position = new Point();
            e.Handled = true;
        }

        #endregion

        //颜色文本框双击打开颜色对话框
        private void tb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var tb = sender as TextBox;
            var cd = new ColorDialog();
            if (DialogResult.OK == cd.ShowDialog())
            {
                var bc = new BrushConverter();
                tb.Text =
                    bc.ConvertFromString("#" + cd.Color.R.ToString("X2") + cd.Color.G.ToString("X2") +
                                         cd.Color.B.ToString("X2")).ToString();
            }
        }
        //给关联按钮添加单击事件
        public void AddBtnLinkEvent()
        {
            if (_btnLink != null)
            {
                _btnLink.Click += BtnLink_Click;
            }
            else
            {
                MessageBox.Show("没有找到关联按钮");
            }
        }
        //关联事件 
        private void BtnLink_Click(object sender, RoutedEventArgs e)
        {
            // DesignerCanvas dc = this.FindName("dc") as DesignerCanvas;
            
            //if (bo.DeviceModel!="管道");//@2013-10-18 多选必然是管道 如发现显示的是上次选中的图元 更改成管道图元
            //{
            //    if (m_ListDesignerItem != null && m_ListDesignerItem.Count > 1)
            //    {
            //        bo = m_ListDesignerItem[0].Item.BasisObject;
            //    }
            //}
            //m_listBasisDevice = new List<BasisDevice>();
            //if (m_ListDesignerItem!=null&&m_ListDesignerItem.Count>1)
            //{

            //    for (int i = 0; i < m_ListDesignerItem.Count; i++)
            //    {

            //        m_listBasisDevice.Add(m_ListDesignerItem[i].Item.BasisObject);
            //    }
            //}

            //if (bo == null)
            //{
            //    MessageBox.Show("没有可关联的对象");
            //    return;
            //}
            //if (!String.IsNullOrEmpty(bo.DeviceType) || String.IsNullOrEmpty(bo.DeviceModel))
            //{
            //    //@sqh 2013-7-30 此处针对复选框图元特殊判断 可能还有其他类似图元需要判断 可加以核实判断
            //    if (bo.DeviceModel != "复选框")
            //    {
            //        string sTip = "该图元需关联设备使用，无需在此窗口进行一一关联事件或者动作。";
            //        sTip += "\n如果需要特殊配置操作，请点击确定。";
            //        if (MessageBoxResult.Cancel == MessageBox.Show(sTip, "温馨提示", MessageBoxButton.OKCancel, MessageBoxImage.Information))
            //        {
            //            return;
            //        }
            //    }

            //}
            //if (string.IsNullOrEmpty(bo._name))
            //{
            //    MessageBox.Show("图元名称为空");
            //    return;
            //}
            //ConfigWindowManager configWidow = Omniframe.Instance.GetInstance<ConfigWindowManager>(SystemManagerName.c_ConfigWindowManagerName);
            //if (configWidow != null)
            //{
            //    configWidow.OpenMetafileForm(bo,m_listBasisDevice);
            //    //configWidow.OpenCreateDeviceForm(m_DesignerItem);
            //}
            //else
            MessageBox.Show("没有得到配置窗体的实例，未能打开!");
        }
        //添加一个属性项
        public void AddProItem(string proName, FrameworkElement obj, int index = -1)
        {
            var lvName = Template.FindName("lvName", this) as ListView;
            var lvValue = Template.FindName("lvValue", this) as ListView;
            var lviName = new ListViewItem();
            lviName.Content = new TextBlock {Text = proName};
            if (index == -1) lvName.Items.Add(lviName);
            else
            {
                lvName.Items.Insert(index, lviName);
            }
            var lviValue = new ListViewItem();
            lviValue.Content = obj;
            if (index == -1) lvValue.Items.Add(lviValue);
            else lvValue.Items.Insert(index, lviValue);
        }
        //获取指定名称的属性索引位置
        public int GetPropertyItemIndex(string proName)
        {
            var lvName = Template.FindName("lvName", this) as ListView;
            foreach (var itm in lvName.Items)
            {
                if (itm is ListViewItem)
                {
                    if (((ListViewItem) itm).Content is TextBlock)
                    {
                        if (((TextBlock) ((ListViewItem) itm).Content).Text == proName)
                        {
                            return lvName.Items.IndexOf(itm);
                        }
                    }
                }
            }
            return -1;
        }
        //设置图元名称显示位置
        private void AddNamePosition(DesignerContainer di)
        {
            var tbTag = di.Template.FindName("tbTag", di) as TextBlock;
            var nameInfo = new string[3];
            var cbHA = new ComboBox();
            cbHA.MaxWidth = 80;
            cbHA.Items.Add(new ComboBoxItem {Content = "Left", Tag = "Left"});
            cbHA.Items.Add(new ComboBoxItem {Content = "Center", Tag = "Center"});
            cbHA.Items.Add(new ComboBoxItem {Content = "Right", Tag = "Right"});
            var bdHA = new Binding("HorizontalAlignment") {Source = tbTag};
            bdHA.Mode = BindingMode.TwoWay;
            bdHA.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            cbHA.SetBinding(ComboBox.TextProperty, bdHA);
            if (cbHA.SelectedItem == null)
                cbHA.SelectedIndex = 1;
            nameInfo[0] = (cbHA.SelectedItem as ComboBoxItem).Content.ToString();
            cbHA.SelectionChanged += (sender, e) =>
            {
                nameInfo[0] = (cbHA.SelectedItem as ComboBoxItem).Content.ToString();
                di.NamePosition = ArrToStr(nameInfo);
            };
            if (di.NamePosition.Split('|').Length > 2)
            {
                foreach (ComboBoxItem item in cbHA.Items)
                {
                    if (item.Content.ToString().Equals(di.NamePosition.Split('|')[0]))
                    {
                        item.IsSelected = true;
                        break;
                    }
                }
            }
            AddProItem("名称水平", cbHA);

            var cbVA = new ComboBox();
            cbVA.MaxWidth = 80;
            cbVA.Items.Add(new ComboBoxItem {Content = "Top", Tag = "Top"});
            cbVA.Items.Add(new ComboBoxItem {Content = "Center", Tag = "Center"});
            cbVA.Items.Add(new ComboBoxItem {Content = "Bottom", Tag = "Bottom"});
            var bdVA = new Binding("VerticalAlignment") {Source = tbTag};
            bdVA.Mode = BindingMode.TwoWay;
            bdVA.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            cbVA.SetBinding(ComboBox.TextProperty, bdVA);
            if (cbVA.SelectedItem == null)
                cbVA.SelectedIndex = 2;
            nameInfo[1] = (cbVA.SelectedItem as ComboBoxItem).Content.ToString();
            cbVA.SelectionChanged += (sender, e) =>
            {
                nameInfo[1] = (cbVA.SelectedItem as ComboBoxItem).Content.ToString();
                di.NamePosition = ArrToStr(nameInfo);
            };
            if (di.NamePosition.Split('|').Length > 2)
            {
                foreach (ComboBoxItem item in cbVA.Items)
                {
                    if (item.Content.ToString().Equals(di.NamePosition.Split('|')[1]))
                    {
                        item.IsSelected = true;
                        break;
                    }
                }
            }
            AddProItem("名称垂直", cbVA);

            var tbOffset = new TextBox();
            var bdOffset = new Binding("Margin") {Source = tbTag};
            bdOffset.Mode = BindingMode.TwoWay;
            bdOffset.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            tbOffset.SetBinding(TextBox.TextProperty, bdOffset);
            if (string.IsNullOrEmpty(tbOffset.Text))
                tbOffset.Text = "-100,-20,-100,-20";
            if (tbOffset.Text == "-20,-20,-20,-20")
                tbOffset.Text = "-100,-20,-100,-20";
            nameInfo[2] = tbOffset.Text;

            tbOffset.TextChanged += (sender, e) =>
            {
                nameInfo[2] = tbOffset.Text;
                di.NamePosition = ArrToStr(nameInfo);
            };
            AddProItem("名称偏移", tbOffset);
        }
        private string ArrToStr(string[] input)
        {
            var result = "";
            foreach (var item in input)
            {
                result += item + "|";
            }
            return result.Substring(0, result.Length - 1);
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _btnLink = Template.FindName("btnlink", this) as Button;
        }
    }
}