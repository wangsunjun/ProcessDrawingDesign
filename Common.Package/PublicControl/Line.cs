using System;
using System.Collections.Generic;
using System.Windows;
using Wss.FoundationCore.Attributes;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using Wss.FoundationCore.Attributes;
using Wss.FoundationCore.Controls;
using SerializableAttribute = Wss.FoundationCore.Attributes.SerializableAttribute;

namespace Common.Package.PublicControl
{
    [DataModel(ModelName = "线条")]
    public class Line : DesignerItem
    {
        [Designable]
        [Serializable]
        [DisplayName("线条颜色2")]
        public Brush LineColor
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("LineColor", typeof(Brush), typeof(Line), new PropertyMetadata(new SolidColorBrush(Colors.Blue), new PropertyChangedCallback(StrokeChanged)));

        private static void StrokeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Line line = sender as Line;
            //DesignerCanvas dc = line.ParentCanvas;
            (line.Template.FindName("linePath",line) as Path).Fill=(Brush)new BrushConverter().ConvertFromString(e.NewValue.ToString());
        }

        //public static DependencyProperty RunningStateProperty =
        //    DependencyProperty.Register("RunningState", typeof(int), typeof(Line),
        //        new PropertyMetadata(
        //            (sender, e) =>
        //            {
        //                Line obj = (Line)sender;
        //                Path path1 = obj.Template.FindName("linePath", obj) as Path;

        //                if ((int)e.NewValue == 1)//运行
        //                {
        //                    //obj.Style = obj.FindResource("StyleKeyGreenLineTag") as Style;
        //                    //obj.ApplyTemplate();//重新应用一下模板,不然下面的控件找不到
        //                    path1.Fill = new SolidColorBrush(Color.FromRgb( 255, 1, 1));
        //                }
        //                else if ((int)e.NewValue == 2) path1.Fill = new SolidColorBrush(Color.FromRgb(33, 255, 33));
        //                else path1.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 255));
        //            }));


        ///// <summary>
        /////     真：运行，假：停止
        ///// </summary>
        //[DataModelField]
        //[DisplayName("颜色（红1绿2蓝3）")]
        //public int RunningState
        //{
        //    get { return (int)GetValue(RunningStateProperty); }
        //    set { SetValue(RunningStateProperty, value); }
        //}

        #region 构造

        static Line()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Line),  new FrameworkPropertyMetadata(typeof(Line)));
        }
        public Line()
        {
            MinHeight = 4;
            MinWidth = 4;
            base._groupName = "常用控件";
            base._displayName = "线条";
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
                var style = FindResource("StyleKeyLineTag") as Style;
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