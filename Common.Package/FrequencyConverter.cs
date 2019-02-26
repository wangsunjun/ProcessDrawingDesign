using System;
using System.Collections.Generic;
using System.Windows;
using Wss.FoundationCore.Attributes;
using System.ComponentModel;
using Wss.FoundationCore.Controls;

namespace Common.Package
{
    [DataModel(ModelName = "变频器")]
    public class FrequencyConverter : DesignerItem
    {
        public static DependencyProperty RunningStateProperty =
            DependencyProperty.Register("RunningState", typeof(int), typeof(FrequencyConverter),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var obj = (FrequencyConverter)sender;
                        if ((int)e.NewValue == 1)//运行
                        {

                            obj.Style = obj.FindResource("StyleKeyFrequencyConverterTag") as Style;
                        }
                        else if ((int)e.NewValue == -1)//故障
                        {
                            obj.Style = obj.FindResource("StyleKeyFrequencyConverterWarnningTag") as Style;
                        }
                        else//停止
                        {
                            //obj.Style = obj.FindResource("StyleKeyBackwaterValveStopTag") as Style;
                        }
                    }));


        /// <summary>
        ///     真：运行，假：停止
        /// </summary>
        [DataModelField]
        [DisplayName("运行状态")]
        public int RunningState
        {
            get { return (int)GetValue(RunningStateProperty); }
            set { SetValue(RunningStateProperty, value); }
        }

        #region 构造

        static FrequencyConverter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (FrequencyConverter),
                new FrameworkPropertyMetadata(typeof (FrequencyConverter)));
        }
        public FrequencyConverter()
        {
            base._groupName = "工艺图控件";
            base._displayName = "变频器";
        }
        protected override void OnMergeResource(ref List<Uri> resources)
        {
            base.OnMergeResource(ref resources);
            resources.Add(new Uri(@"/Common.Package;component/FrequencyConverterStyle.xaml", UriKind.Relative));
        }

        protected override Style OnGetToolBoxStyle()
        {
            try
            {
                var style = FindResource("StyleKeyFrequencyConverterTag") as Style;
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