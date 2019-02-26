using System;
using System.Collections.Generic;
using System.Windows;
using Wss.FoundationCore.Attributes;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using Wss.FoundationCore.Controls;

namespace Common.Package.PublicControl
{
    /// <summary>
    /// 静态文字
    /// </summary>
    [DataModel(ModelName = "label")]
    public class RunaLabel:DesignerItem
    {
        static string d_DigitText = "0.00";

        #region 显示位数
        [Designable]
        [DisplayName("显示位数")]
        public string DigitText
        {
            get { return (string)GetValue(DigitTextProperty); }
            set { SetValue(DigitTextProperty, value); }

        }
        public static readonly DependencyProperty DigitTextProperty =
           DependencyProperty.Register("DigitText", typeof(string), typeof(RunaLabel), new UIPropertyMetadata("0.00", new PropertyChangedCallback(DigitTextChanged)));

        public static void DigitTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            string m_decimaldigits = "F";
            d_DigitText = e.NewValue.ToString();

            RunaLabel txt = sender as RunaLabel;
            if (txt!=null && txt.Template != null)
            {
                TextBlock tb = txt.Template.FindName("RunaLabel", txt) as TextBlock;

                double ftnamber;
                if (d_DigitText.Contains("."))
                {

                    m_decimaldigits += d_DigitText.Substring(d_DigitText.IndexOf('.')).Length - 1;
                }
                else
                {
                    m_decimaldigits = "";
                }
                if (tb != null)
                {
                    Regex textNumberReg = new Regex(@"^[0-9]+[.]?[0-9]+$");
                    Match m = textNumberReg.Match(tb.Text);
                    if (m.Success)
                    {
                        ftnamber =Double.Parse( tb.Text);
                        tb.Text = ftnamber.ToString(m_decimaldigits);
                        txt.TextValue = tb.Text;
                    }
                }
            }
        }
        #endregion
        #region 显示文本

        [Wss.FoundationCore.Attributes.Serializable]
        [DataModelField]
        [Designable]
        [DisplayName("显示文本")]
        public string TextValue
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(RunaLabel), new UIPropertyMetadata("静态文字", new PropertyChangedCallback(TextChanged)));
        public static void TextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RunaLabel RunaLb = sender as RunaLabel;
            TextBlock lbl = RunaLb.Template.FindName("lbRuna", RunaLb) as TextBlock;
            if (lbl != null) lbl.Text = e.NewValue.ToString();        
        }

        #endregion
        #region 背景色
        [Wss.FoundationCore.Attributes.Serializable]
        [DataModelField]
        [Designable]
        [DisplayName("背景色")]
        public new Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public new static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(RunaLabel), new FrameworkPropertyMetadata(Brushes.Transparent, new PropertyChangedCallback(BackgroundChanged)));

        public static void BackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RunaLabel RunaLb = sender as RunaLabel;
            (RunaLb.Template.FindName("lbRuna", RunaLb) as TextBlock).Background = (Brush)new BrushConverter().ConvertFromString(e.NewValue.ToString());
        }
        #endregion
        #region 前景色
        [Wss.FoundationCore.Attributes.Serializable]
        [DataModelField]
        [Designable]
        [DisplayName("前景色")]
        public new Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }
        public new static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(RunaLabel), new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(ForegroundChanged)));
        public static void ForegroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RunaLabel RunaLb = sender as RunaLabel;
            //TextBlock lbl = RunaLb.Template.FindName("lbRuna", RunaLb) as TextBlock;
            //if (lbl != null) lbl.FontSize = (int)e.NewValue;
            (RunaLb.Template.FindName("lbRuna", RunaLb) as TextBlock).Foreground = (Brush)new BrushConverter().ConvertFromString(e.NewValue.ToString());
        }
        #endregion
        #region 字号
        [Designable]
        [DisplayName("字号")]
        public int tagFontSize
        {
            get { return (int)GetValue(tagFontSizeProperty); }
            set { SetValue(tagFontSizeProperty, value); }
        }
        // Using a DependencyProperty as the backing store for FontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty tagFontSizeProperty =
            DependencyProperty.Register("tagFontSize", typeof(int), typeof(RunaLabel), new UIPropertyMetadata(12, tagFontSizeChanged));
        public static void tagFontSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RunaLabel RunaLb = sender as RunaLabel;
            TextBlock lbl = RunaLb.Template.FindName("lbRuna", RunaLb) as TextBlock;
            if (lbl != null) lbl.FontSize = (int)e.NewValue;
            RunaLb.tagFontSize = (int)e.NewValue;
        }
        #endregion
        #region 名称显示位置
        [Designable]
        [DisplayName("显示位置")]
        public string ContentPosition
        {
            get { return (string)GetValue(ContentPositionProperty); }
            set { SetValue(ContentPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NamePosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentPositionProperty =
            DependencyProperty.Register("ContentPosition", typeof(string), typeof(RunaLabel), new UIPropertyMetadata("", new PropertyChangedCallback(ContentPositionChanged)));
        public static void ContentPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RunaLabel RunaLb = sender as RunaLabel;
            TextBlock lbl = RunaLb.Template.FindName("lbRuna", RunaLb) as TextBlock;
            var nameInfo = e.NewValue.ToString().Split('|');

            try
            {
                lbl.HorizontalAlignment = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), nameInfo[0]);
                lbl.VerticalAlignment = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), nameInfo[1]);

            }
            catch (Exception)
            {
            }
        }
        #endregion
        #region 字体
        [Designable]
        [DisplayName("字体")]
        public FontFamily tagFontFamily
        {
            get { return (FontFamily)GetValue(tagFontFamilyProperty); }
            set { SetValue(tagFontFamilyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty tagFontFamilyProperty =
            DependencyProperty.Register("tagFontFamily", typeof(FontFamily), typeof(DesignerItem), new UIPropertyMetadata(new FontFamily("Arial"), new PropertyChangedCallback(tagFontFamilyChanged)));
        public static void tagFontFamilyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RunaLabel RunaLb = sender as RunaLabel;
            TextBlock lbl = RunaLb.Template.FindName("lbRuna", RunaLb) as TextBlock;
            FontFamily ff = (FontFamily)new FontFamilyConverter().ConvertFromString(e.NewValue.ToString());
            if (lbl != null) lbl.FontFamily = ff;
            RunaLb.FontFamily = ff;
        }
        #endregion

        static RunaLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RunaLabel), new FrameworkPropertyMetadata(typeof(RunaLabel)));
        }
        public RunaLabel()
        {
            base._groupName = "常用控件";
            base._displayName = "文本";
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
                var style = FindResource("StyleKeyLableStyle") as Style;
                return style;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OnGetToolBoxStyle方法");
                return null;
            }
        }
       
    }
}
