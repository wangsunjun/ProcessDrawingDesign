using Wss.FoundationCore.Attributes;
using Wss.FoundationCore.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Common.Package.PublicControl
{
    [DataModel(ModelName = "小区")]
    class Village : DesignerItem
    {
        
        static Village()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Village), new FrameworkPropertyMetadata(typeof(Village)));
        }
        public Village()
        {
            base._groupName = "常用控件";
            base._displayName = "小区";
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
                var style = FindResource("StyleVillage") as Style;
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
        [DisplayName("小区ID")]
        public string VillageID
        {
            get { return (string)GetValue(VillageIDProperty); }
            set { SetValue(VillageIDProperty, value); }
        }

        public static readonly DependencyProperty VillageIDProperty =
            DependencyProperty.Register("VillageID", typeof(String), typeof(Village), new PropertyMetadata((sender, e) =>
            {
                var obj = (Village)sender;

            }));
    }
}
