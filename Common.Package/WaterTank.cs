using System;
using System.Collections.Generic;
using System.Windows;
using Wss.FoundationCore.Attributes;
using System.ComponentModel;
using System.Windows.Shapes;
using Wss.FoundationCore.Controls;
using System.Reflection;

namespace Common.Package
{
    [DataModel(ModelName = "水箱")]
    public class WaterTank : DesignerItem
    {
        #region 运行状态
        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [DataModelField]
        [DisplayName("运行状态")]
        public int RunningState
        {
            get { return (int)GetValue(RunningStateProperty); }
            set { SetValue(RunningStateProperty, value); }
        }
        public static DependencyProperty RunningStateProperty = DependencyProperty.Register("RunningState", typeof(int), typeof(WaterTank),
            new PropertyMetadata(
                (sender, e) =>
                {
                    var obj = (WaterTank)sender;
                    if ((int)e.NewValue == 1)//运行
                    {

                        obj.Style = obj.FindResource("StyleKeyWaterTankTag") as Style;
                    }
                    else if ((int)e.NewValue == -1)//故障
                    {
                        obj.Style = obj.FindResource("StyleKeyWaterTankWarnningTag") as Style;
                    }
                    else//正常状态
                    {
                        obj.Style = obj.FindResource("StyleKeyWaterTankTag") as Style;
                    }
        }));
        #endregion

        #region 液位高度
        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [DataModelField]
        [DisplayName("液位高度")]
        public float Liquidlevel
        {
            get { return (float)GetValue(LiquidlevelProperty); }
            set { SetValue(LiquidlevelProperty, value); }
        }
        public static DependencyProperty LiquidlevelProperty = DependencyProperty.Register("Liquidlevel", typeof(float), typeof(WaterTank),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var obj = (WaterTank)sender;
                        Path path1 = obj.Template.FindName("liquidLevelPath", obj) as Path;
                        
                        if(path1!=null) path1.Height = float.Parse(e.NewValue.ToString())/3.0f*100f;//设置液位高度，从Model传值过来
        }));
        #endregion

        #region 液位高报警
        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [DataModelField]
        [DisplayName("液位高报警")]
        public float LiquidHighWarn
        {
            get { return (float)GetValue(LiquidHighWarnProperty); }
            set { SetValue(LiquidHighWarnProperty, value); }
        }
        public static DependencyProperty LiquidHighWarnProperty = DependencyProperty.Register("LiquidHighWarn", typeof(float), typeof(WaterTank),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var obj = (WaterTank)sender;
                        if (e.NewValue.ToString()=="1")
                        {

                        }
                        //Path path1 = obj.Template.FindName("liquidLevelPath", obj) as Path;
                    }));
        #endregion

        #region 液位低报警
        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [DataModelField]
        [DisplayName("液位低报警")]
        public float LiquidLowWarn
        {
            get { return (float)GetValue(LiquidHighLowProperty); }
            set { SetValue(LiquidHighLowProperty, value); }
        }
        public static DependencyProperty LiquidHighLowProperty = DependencyProperty.Register("LiquidLowWarn", typeof(float), typeof(WaterTank),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var obj = (WaterTank)sender;
                        
                    }));
        #endregion

        #region 液位高报警值设定
        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [DataModelField]
        [DisplayName("液位高报警值设定")]
        public float LiquidHighWarnSet
        {
            get { return (float)GetValue(LiquidHighWarnSetProperty); }
            set { SetValue(LiquidHighWarnSetProperty, value); }
        }
        public static DependencyProperty LiquidHighWarnSetProperty = DependencyProperty.Register("LiquidHighWarnSet", typeof(float), typeof(WaterTank),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var obj = (WaterTank)sender;
                        if (e.NewValue!=e.OldValue)
                        {
                            if (obj._DWriteRegister != null) obj._DWriteRegister(obj, "LiquidHighWarnSet", e.NewValue);
                        }
                    }));
        #endregion

        #region 液位低报警值设定
        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [DataModelField]
        [DisplayName("液位低报警值设定")]
        public float LiquidLowWarnSet
        {
            get { return (float)GetValue(LiquidHighLowSetProperty); }
            set { SetValue(LiquidHighLowSetProperty, value); }
        }
        public static DependencyProperty LiquidHighLowSetProperty = DependencyProperty.Register("LiquidLowWarnSet", typeof(float), typeof(WaterTank),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var obj = (WaterTank)sender;
                        if (e.NewValue != e.OldValue)
                        {
                            if (obj._DWriteRegister != null) obj._DWriteRegister(obj, "LiquidLowWarnSet", e.NewValue);
                        }
                    }));

        #endregion

        #region 构造

        static WaterTank()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (WaterTank),
                new FrameworkPropertyMetadata(typeof (WaterTank)));
        }
        public WaterTank()
        {
            base._groupName = "工艺图控件";
            base._displayName = "水箱";
        }
        protected override void OnMergeResource(ref List<Uri> resources)
        {
            base.OnMergeResource(ref resources);
            resources.Add(new Uri(@"/Common.Package;component/WaterTankStyle.xaml", UriKind.Relative));
        }

        protected override Style OnGetToolBoxStyle()
        {
            try
            {
                var style = FindResource("StyleKeyWaterTankTag") as Style;
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