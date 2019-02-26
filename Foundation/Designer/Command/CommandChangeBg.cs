using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Wss.Foundation.Controls;
using Wss.FoundationCore.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     更改背景  子类
    /// </summary>
    public class CommandChangeBg : Command
    {
        private string filename = String.Empty;
        private string newfilename = string.Empty;
        private List<DesignerContainer> newpicture;
        private List<DesignerContainer> oldpicture;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            filename = param[0].ToString();
            newfilename = param[0].ToString();
            if (!String.IsNullOrEmpty(filename))
            {
                foreach (var item in mgr.CurrentCanvas.Children.OfType<ISelectable>())
                {
                    var di = item as DesignerContainer;

                    var idx = Panel.GetZIndex(di);

                    if (idx == int.MinValue)
                    {
                        if (oldpicture == null)
                        {
                            oldpicture = new List<DesignerContainer>();
                        }
                        oldpicture.Add(di);
                        mgr.CurrentCanvas.Remove(di);
                        break;
                    }
                }
                #region
                //Image img = new Image();
                //img.Source = new BitmapImage(new Uri(newFileName, UriKind.Relative));
                //img.Stretch = Stretch.Fill;
                //CustomImage ci = new CustomImage();
                //ci.FileName = filename.Substring(filename.LastIndexOf("\\") + 1, filename.Length - filename.LastIndexOf("\\") - 1);
                //ci.Style = mgr.CurrentCanvas.FindResource("ciStyle") as Style;
                //ci.NameVisible = false;

                //ci.Width = (this.Parent as Viewbox).ActualWidth;
                //ci.Height = (this.Parent as Viewbox).ActualHeight;
                #endregion

                var diBg = new DesignerContainer();
                //diBg.Width = AppParams.CanvasWidth;// (this.Parent as Viewbox).ActualWidth;
                //diBg.Height = AppParams.CanvasHeight; //(this.Parent as Viewbox).ActualHeight;
                //diBg.Content = ci;
                diBg.StyleName = "ciStyle";
                diBg.Focusable = false;
                diBg.IsEnabled = false;
                diBg.IsHitTestVisible = false;
                diBg.NameVisible = false;
                Canvas.SetLeft(diBg, 0);
                Canvas.SetTop(diBg, 0);
                Panel.SetZIndex(diBg, int.MinValue);
                if (newpicture == null)
                {
                    newpicture = new List<DesignerContainer>();
                }
                newpicture.Add(diBg);
                mgr.CurrentCanvas.Add(diBg);
                return true;
            }
            return false;
        }

        public override bool UnRun(CommandManager mgr)
        {
            DesignerContainer newdi;
            DesignerContainer olddi;
            if (newpicture != null)
            {
                for (var i = 0; i < newpicture.Count; i++)
                {
                    newdi = newpicture[i];

                    mgr.CurrentCanvas.Remove(newdi);
                }
            }
            if (oldpicture != null)
            {
                for (var i = 0; i < oldpicture.Count; i++)
                {
                    olddi = oldpicture[i];
                    if (!mgr.CurrentCanvas.Children.Contains(oldpicture[i]))
                    {
                        mgr.CurrentCanvas.Add(oldpicture[i]);
                    }
                }
            }
            return true;
        }

        public override bool ReRun(CommandManager mgr)
        {
            foreach (var item in mgr.CurrentCanvas.Children.OfType<ISelectable>())
            {
                var di = item as DesignerContainer;
                if (oldpicture == null)
                {
                    oldpicture = new List<DesignerContainer>();
                }
                oldpicture.Add(di);
                var idx = Panel.GetZIndex(di);

                if (idx == int.MinValue)
                {
                    mgr.CurrentCanvas.Remove(di);

                    break;
                }
            }
            var diBg = new DesignerContainer();
            //diBg.Width = AppParams.CanvasWidth;// (this.Parent as Viewbox).ActualWidth;
            //diBg.Height = AppParams.CanvasHeight; //(this.Parent as Viewbox).ActualHeight;
            //diBg.Content = ci;
            diBg.StyleName = "ciStyle";
            diBg.Focusable = false;
            diBg.IsEnabled = false;
            diBg.IsHitTestVisible = false;
            diBg.NameVisible = false;

            Canvas.SetLeft(diBg, 0);
            Canvas.SetTop(diBg, 0);
            Panel.SetZIndex(diBg, int.MinValue);
            if (newpicture == null)
            {
                newpicture = new List<DesignerContainer>();
            }
            newpicture.Add(diBg);
            mgr.CurrentCanvas.Add(diBg);

            return true;
        }

        public static void Run(CommandManager cmgr, string filename, string newfilename)
        {
            cmgr.Run<CommandChangeBg>(filename, newfilename);
        }
    }
}