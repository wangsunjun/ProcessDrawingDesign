using System;
using System.Collections.Generic;
using System.Windows;
using Wss.Foundation.Controls;
using Wss.FoundationCore.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     垂直翻转命令  子类
    /// </summary>
    public class CommandMirrorV : Command
    {
        private List<ISelectable> M_list;
        private List<double> m_newscaleY;
        private List<double> m_oldscaleY;

        public override bool Run(CommandManager mgr, params object[] param)
        {
            M_list = mgr.CurrentCanvas.SelectionService.SelectedDesignerContainer;
            var flag = Convert.ToBoolean(param[0]);
            if (Mirror(flag, mgr))
            {
                return true;
            }
            return false;
        }

        public override bool UnRun(CommandManager mgr)
        {
            if (M_list.Count > 0)
            {
                DesignerContainer di = null;
                for (var j = 0; j < mgr.CurrentCanvas.SelectionService.SelectedDesignerContainer.Count; j++)
                {
                    di = mgr.CurrentCanvas.SelectionService.SelectedDesignerContainer[j] as DesignerContainer;
                    if (m_oldscaleY != null)
                    {
                        for (var i = 0; i <= m_oldscaleY.Count - 1; i++)
                        {
                            di.ScaleY = m_oldscaleY[i];
                        }
                    }
                }


                return true;
            }
            return false;
        }

        public override bool ReRun(CommandManager mgr)
        {
            if (M_list.Count > 0)
            {
                DesignerContainer di = null;
                for (var j = 0; j < mgr.CurrentCanvas.SelectionService.SelectedDesignerContainer.Count; j++)
                {
                    di = mgr.CurrentCanvas.SelectionService.SelectedDesignerContainer[j] as DesignerContainer;
                    if (m_newscaleY != null)
                    {
                        for (var i = 0; i <= m_newscaleY.Count - 1; i++)
                        {
                            di.ScaleY = m_newscaleY[i];
                        }
                    }
                }


                return true;
            }
            return false;
        }

        private Boolean Mirror(bool isHor, CommandManager mgr)
        {
            if (mgr.CurrentCanvas.SelectionService.SelectedDesignerContainer.Count > 0)
            {
                foreach (DesignerContainer item in mgr.CurrentCanvas.SelectionService.SelectedDesignerContainer)
                {
                    if (item != null && item.ParentID == Guid.Empty)
                    {
                        if (isHor)
                        {
                            item.ScaleX = (item.ScaleX < 0 & isHor) ? 1 : -1;
                        }
                        else
                        {
                            if (m_oldscaleY == null)
                            {
                                m_oldscaleY = new List<double>();
                            }
                            m_oldscaleY.Add(item.ScaleY);
                            item.ScaleY = (item.ScaleY < 0 & !isHor) ? 1 : -1;
                            if (m_newscaleY == null)
                            {
                                m_newscaleY = new List<double>();
                            }
                            m_newscaleY.Add(item.ScaleY);
                        }

                        var dt = item.Template.FindName("PART_DragThumb", item) as DragThumb;
                        if (dt != null && item.Content is FrameworkElement)
                        {
                            dt.LayoutTransform = (item.Content as FrameworkElement).LayoutTransform;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public static void Run(CommandManager cmgr, Boolean flag)
        {
            cmgr.Run<CommandMirrorV>(flag);
        }
    }
}