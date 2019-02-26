using System;
using System.Collections.Generic;
using System.Windows;
using Wss.FoundationCore.Attributes;
using System.ComponentModel;
using Wss.FoundationCore.Controls;

namespace Common.Package
{
    [DataModel(ModelName = "温控阀")]
    public class BackwaterValve2 : DesignerItem
    {
        public static DependencyProperty RunningStateProperty =
            DependencyProperty.Register("RunningState", typeof(int), typeof(BackwaterValve2),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var obj = (BackwaterValve2)sender;
                        if ((int)e.NewValue == 1)//运行
                        {

                            obj.Style = obj.FindResource("StyleKeyBackwaterValveRunningTag2") as Style;
                        }
                        else if ((int)e.NewValue == -1)//故障
                        {
                            obj.Style = obj.FindResource("StyleKeyBackwaterValveWarningTag2") as Style;
                        }
                        else//停止
                        {
                            obj.Style = obj.FindResource("StyleKeyBackwaterValveStopTag2") as Style;
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

        #region 阀开度
        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [DataModelField]
        [DisplayName("阀开度")]
        public float OpenDegreeSet
        {
            get { return (float)GetValue(OpenDegreeProperty); }
            set { SetValue(OpenDegreeProperty, value); }
        }
        public static DependencyProperty OpenDegreeProperty = DependencyProperty.Register("OpenDegreeSet", typeof(float), typeof(BackwaterValve2),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var obj = (BackwaterValve2)sender;
                        if (e.NewValue != e.OldValue)
                        {
                            if (obj._DWriteRegister != null) obj._DWriteRegister(obj, "OpenDegreeSet", e.NewValue);
                            Console.WriteLine(("value3:" + e.NewValue));
                            //e.OldValue = e.NewValue;
                        }
                    }));
        #endregion

        #region 构造

        static BackwaterValve2()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (BackwaterValve2),
                new FrameworkPropertyMetadata(typeof (BackwaterValve2)));
        }
        public BackwaterValve2()
        {
            base._groupName = "工艺图控件";
            base._displayName = "温控阀";
        }
        protected override void OnMergeResource(ref List<Uri> resources)
        {
            base.OnMergeResource(ref resources);
            resources.Add(new Uri(@"/Common.Package;component/BackwaterValve2Style.xaml", UriKind.Relative));
        }

        protected override Style OnGetToolBoxStyle()
        {
            try
            {
                var style = FindResource("StyleKeyBackwaterValveTag2") as Style;
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