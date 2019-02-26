using System.ComponentModel;
using System.Windows;
using Wss.Foundation;
using Wss.FoundationCore.Models;
using System;
using System.Configuration;

namespace TestDesigner
{
    /// <summary>
    ///     App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //设置整个项目的资源主题
            Current.LoadResource();
          
            base.OnStartup(e);

          
        }
    }
    public static class GetTheme
    {
        public static void LoadResource(this Application app)
        {
            OnLoadResource(app);
        }
        public static void OnLoadResource(Application app)
        {
            var packUri = String.Format(@"/Wss.Foundation;component/Themes/Resource.xaml");
            var dictionary = Application.LoadComponent(new Uri(packUri, UriKind.Relative)) as ResourceDictionary;
            if (dictionary != null)
            {
                app.Resources.MergedDictionaries.Clear();
                app.Resources.MergedDictionaries.Add(dictionary);
            }
        }
    }
}