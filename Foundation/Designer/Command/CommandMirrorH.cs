using System;
using System.Collections.Generic;
using System.Windows;
using Wss.Foundation.Controls;
using Wss.FoundationCore.Controls;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     上移一层命令 子类
    /// </summary>
    /// <summary>
    ///     水平翻转命令 子类
    /// </summary>
    public class CommandMirrorH : Command
    {
        private List<ISelectable> M_list;
        private List<double> m_newscaleX;
        private List<double> m_oldscaleX;

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
                    if (m_oldscaleX != null)
                    {
                        for (var i = 0; i <= m_oldscaleX.Count - 1; i++)
                        {
                            di.ScaleX = m_oldscaleX[i];
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
                    if (m_newscaleX != null)
                    {
                        for (var i = 0; i <= m_newscaleX.Count - 1; i++)
                        {
                            di.ScaleX = m_newscaleX[i];
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
                            if (m_oldscaleX == null)
                            {
                                m_oldscaleX = new List<double>();
                            }
                            m_oldscaleX.Add(item.ScaleX);

                            item.ScaleX = (item.ScaleX < 0 & isHor) ? 1 : -1;
                            if (m_newscaleX == null)
                            {
                                m_newscaleX = new List<double>();
                            }
                            m_newscaleX.Add(item.ScaleX);
                        }
                        else
                        {
                            item.ScaleY = (item.ScaleY < 0 & !isHor) ? 1 : -1;
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
            cmgr.Run<CommandMirrorH>(flag);
        }
    }
}