using Wss.FoundationCore.Attributes;
using Wss.FoundationCore.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Common.Package.PublicControl
{
    [DataModel(ModelName = "楼栋")]
    class Building : DesignerItem
    {
        static Building()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Building), new FrameworkPropertyMetadata(typeof(Building)));
        }
        public Building()
        {
            base._groupName = "常用控件";
            base._displayName = "楼栋";
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
                var style = FindResource("StyleBuilding") as Style;
                return style;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OnGetToolBoxStyle方法");
                return null;
            }
        }

        [Designable]
        [Wss.FoundationCore.Attributes.Serializable]
        [DisplayName("楼栋ID")]
        public string BuildingID
        {
            get { return (string)GetValue(BuildingIDProperty); }
            set { SetValue(BuildingIDProperty, value); }
        }

        public static readonly DependencyProperty BuildingIDProperty =
            DependencyProperty.Register("BuildingID", typeof(String), typeof(Building),  new PropertyMetadata((sender, e) =>
            {
                var obj = (Building)sender;
                
            }));
    }
}
