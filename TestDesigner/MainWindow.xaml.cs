using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wss.Foundation.Controls;
using System.Linq;
using Wss.FoundationCore.Controls;
using System.Windows.Threading;


namespace TestDesigner
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window 
    {
        Dictionary<string, object> _dicPointValue = new Dictionary<string, object>();
        private DispatcherTimer _dTimer = new DispatcherTimer();
        private System.Threading.Thread _td;
        public MainWindow()
        {
            InitializeComponent();
            this.designerArea.MouseDoubleClick += DesignerArea_MouseDoubleClick;
            string path = Directory.GetCurrentDirectory() + "/CustomDiagram/test.xml";
            if (File.Exists(path))
            {
                designerArea.LoadFileToCanvas("实时工况", path);
            }
            foreach (var item in designerArea.CurrentCanvas.Children.OfType<DesignerContainer>())//遍历控件，将控件属性值改变委托注册到此页面的PChanged方法
            {
                DesignerItem ditem = item.Content as DesignerItem;
                if (ditem._DWriteRegister == null) ditem._DWriteRegister = PChanged;
            }


            ////加载ModBus通讯
            //Ecms.MODBUS.LoadObjecSet.CreateCommandGroup();
            //_td = new System.Threading.Thread(new System.Threading.ThreadStart(new Ecms.MODBUS.AsynTcp().ClientConnect));
            //_td.Start();

            //_dTimer.Tick += new EventHandler(dTimer_Tick);
            //_dTimer.Interval = new TimeSpan(0, 0, 10);
            //_dTimer.Start();
        }

        public void PChanged(DesignerItem di,string str, object value)//写点位值
        {
            if (di._isReturn) return;
            string guid = di._listProperty.Where(x => x.Name == str).First().guid;
            //new Ecms.MODBUS.AsynTcp().WritePoint(Ecms.MODBUS.LoadObjecSet.listPoint.Where(x => x._uniqueId.ToString() == guid).First(), value);
            //di._isReturn = false;
        }

        private void dTimer_Tick(object sender, EventArgs e)
        {
            //遍历画布里所有控件，遍历控件里所有依赖属性，并根据ID找到点位值
            foreach (var item in designerArea.CurrentCanvas.Children.OfType<DesignerContainer>())
            {
                DesignerItem ditem = item.Content as DesignerItem;

                for (int i = 0; i < ditem._listProperty.Count; i++)
                {
                    //if (ditem._listProperty[i].guid == null) continue;
                    //string key = ditem._listProperty[i].guid.ToString();
                    //if (Ecms.MODBUS.LoadObjecSet.listPoint.ToArray().Where(x => x._uniqueId.ToString() == key).Count()!=1) continue;

                    //ditem._isReturn = true;
                    //if (ditem._listProperty[i].Descriptor.PropertyType == typeof(object))
                    //{
                    //    ditem._listProperty[i].SetValue(Ecms.MODBUS.LoadObjecSet.listPoint.ToArray().Where(x => x._uniqueId.ToString() == key).First()._realdata);
                    //}
                    //else
                    //{
                    //    ditem._listProperty[i].SetValue(ditem._listProperty[i].Converter.ConvertFromString(
                    //        (Ecms.MODBUS.LoadObjecSet.listPoint.Where(x => x._uniqueId.ToString() == key).First()._realdata).ToString()));

                    //    if(ditem._listProperty[i].Name== "LiquidLowWarnSet")
                    //    {
                    //        Console.WriteLine(("value:"+Ecms.MODBUS.LoadObjecSet.listPoint.Where(x => x._uniqueId.ToString() == key).First()._realdata));
                    //    }
                    //    if (key == "10010035")
                    //    {
                    //        Console.WriteLine(("value5:" + Ecms.MODBUS.LoadObjecSet.listPoint.Where(x => x._uniqueId.ToString() == key).First()._realdata));
                    //    }
                    //}
                    //ditem._isReturn = false;
                    //(item.Content as DesignerItem)._listProperty[i].SetValue(_dicPointValue[key]);
                }
            }
        }
        private void DesignerArea_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            foreach (var item in this.designerArea.CurrentCanvas.Children.OfType<DesignerContainer>())
            {
                //string str = item.DesignerChild.GetType().ToString(); //Type.GetType(item.DesignerChild).ToString();
                //Type type = item.DesignerChild.GetType();
                //var Properties = type.GetProperties();

                //foreach (var p in Properties)//这样遍历所有子控件类中的所有属性
                //{
                //    if (p.CanRead && p.CanWrite)
                //    {
                //        //Propertie.SetValue(item.DesignerChild, Propertie.GetValue(parent, null), null);
                //    }
                //    string strrrr = p.Name;
                //    try
                //    {
                //        if (p.Name == "RunningState") p.SetValue(item.DesignerChild, -1);//给特定属性赋值
                //    }
                //    catch { }
                //}
                //item.DesignerChild.
            }
        }

        #region
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }

        private void btnMng_Click(object sender, RoutedEventArgs e)
        {
            mngMenu.IsOpen = true;
        }
        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnSys_Click(object sender, RoutedEventArgs e)
        {

        }
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void maxButton_Click(object sender, RoutedEventArgs e)
        {

            if (WindowState == WindowState.Normal) WindowState = WindowState.Maximized;
            else WindowState = WindowState.Normal;
        }
        private void mniButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void menuButton_Click(object sender, RoutedEventArgs e)
        {
            mngMenu.IsOpen = true;
        }
        private void mbtnQuit_Click(object sender, RoutedEventArgs e)
        {

        }
        private void mbtnUpdateUInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mbtnRemark_Click(object sender, RoutedEventArgs e)
        {

        }
        private void mconfigTX_Click(object sender, RoutedEventArgs e)
        {

        }
        private void mbtnShengGao_Click(object sender, RoutedEventArgs e)
        {

        }
        private void mbtnGaoJing_Click(object sender, RoutedEventArgs e)
        {

        }
        private void mMngOperateRecorder_Click(object sender, RoutedEventArgs e)
        {

        }
        private void mEnergyMng_Click(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// 系统管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUser_Click(object sender, RoutedEventArgs e)
        {
            UserMenu.IsOpen = true;
        }
        /// <summary>
        /// 报表管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            Report.IsOpen = true;
        }
        /// <summary>
        /// 报警查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAlarm_Click(object sender, RoutedEventArgs e)
        {
            alarmMenu.IsOpen = true;
        }
        /// <summary>
        /// 报警查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miAlarm_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            switch (mi.Name)
            {
                
                default:
                    break;
            }
        }
        /// <summary>
        /// 报表管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miReport_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            switch (mi.Name)
            {
               
                default:
                    break;
            }
        }
        /// <summary>
        /// 系统管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miSystem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            switch (mi.Name)
            {
              
                case "miUserRole"://用户权限管理
                    break;
                case "miRoleMenu"://角色菜单管理
                    break;
                default:
                    break;
            }
        }
        #endregion

        private void btnDesign_Click(object sender, RoutedEventArgs e)
        {
            new DesignWindow().ShowDialog();
        }
    }
}