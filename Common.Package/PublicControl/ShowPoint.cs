using Wss.FoundationCore.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Shapes;
using Wss.FoundationCore.Controls;
using System.Windows.Controls;

namespace Common.Package.PublicControl
{
    [DataModel(ModelName = "点位显示")]
    public class ShowPoint : DesignerItem
    {
        #region 点位名称
        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [Designable]
        [DisplayName("点位名称")]
        public string PointName
        {
            get { return (string)GetValue(PointNameProperty); }
            set { SetValue(PointNameProperty, value); }
        }
        public static readonly DependencyProperty PointNameProperty =
            DependencyProperty.Register("PointName", typeof(string), typeof(ShowPoint), new PropertyMetadata("点位名称2", new PropertyChangedCallback(PointNameChanged)));
        private static void PointNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ShowPoint line = sender as ShowPoint;
            (line.Template.FindName("lbPointName", line) as Label).Content = e.NewValue.ToString();
        }
        #endregion

        #region 点位值
        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [Designable]
        [DisplayName("点位值")]
        public string PointValue
        {
            get { return (string)GetValue(PointValueProperty); }
            set { SetValue(PointValueProperty, value); }
        }
        public static readonly DependencyProperty PointValueProperty =
            DependencyProperty.Register("PointValue", typeof(string), typeof(ShowPoint), new PropertyMetadata("", new PropertyChangedCallback(PointValueChanged)));
        private static void PointValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ShowPoint obj = sender as ShowPoint;
            (obj.Template.FindName("txtPointValue", obj) as TextBox).Text = e.NewValue.ToString();
            //obj._DWriteRegister?.Invoke(obj, "PointValue", e.NewValue);
        }
        #endregion

        #region 单位
        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [Designable]
        [DisplayName("单位")]
        public string UnitName
        {
            get { return (string)GetValue(UnitNameProperty); }
            set { SetValue(UnitNameProperty, value); }
        }
        public static readonly DependencyProperty UnitNameProperty =
            DependencyProperty.Register("UnitName", typeof(string), typeof(ShowPoint), new PropertyMetadata("", new PropertyChangedCallback(UnitNameChanged)));
        private static void UnitNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ShowPoint line = sender as ShowPoint;
            (line.Template.FindName("lbUnit", line) as Label).Content = e.NewValue.ToString();
        }
        #endregion

        #region 构造
        private bool _isLeftShowPoint = false;
        static ShowPoint()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ShowPoint), new FrameworkPropertyMetadata(typeof(ShowPoint)));
        }
        public ShowPoint()
        {
            base._groupName = "常用控件";
            base._displayName = "点位显示";
            this.MouseDown += ShowPoint_MouseDown;
        }

        void ShowPoint_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ShowPoint obj = (ShowPoint)sender;
            Path path1 = obj.Template.FindName("StyleShowPoint", obj) as Path;
            if (!_isLeftShowPoint)
            {
                obj.Style = obj.FindResource("StyleShowPoint") as Style;
                _isLeftShowPoint = true;
            }
            else
            {
                obj.Style = obj.FindResource("StyleShowPoint") as Style;
                _isLeftShowPoint = false;
            }
        }
        protected override void OnMergeResource(ref List<Uri> resources)
        {
            base.OnMergeResource(ref resources);
            resources.Add(new Uri(@"/Common.Package;component/PublicControl/Style.xaml", UriKind.Relative));
        }

        protected override Style OnGetToolBoxStyle()
        {
            try
            {
                var style = FindResource("StyleShowPoint") as Style;
                return style;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OnGetToolBoxStyle方法");
                return null;
            }
        }

        #endregion
    }
}
