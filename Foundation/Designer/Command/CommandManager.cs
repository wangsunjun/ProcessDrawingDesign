using System;
using System.Collections.Generic;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     命令管理者
    /// </summary>
    public class CommandManager
    {
        //恢复队列
        protected Stack<Command> m_RedoList = new Stack<Command>();
        //撤销队列
        protected Stack<Command> m_UndoList = new Stack<Command>();

        public CommandManager(DesignerCanvas dc)
        {
            CurrentCanvas = dc;
        }

        public bool CanUndo
        {
            get { return m_UndoList.Count > 0; }
        }

        public bool CanRedo
        {
            get { return m_RedoList.Count > 0; }
        }

        public DesignerCanvas CurrentCanvas { get; private set; }

        public void Run<T>(params object[] param) where T : Command
        {
            var cmd = Activator.CreateInstance<T>();
            if (cmd.Run(this, param))
            {
                m_UndoList.Push(cmd);//路由器事件往上抛
            }
        }

        public void Undo()
        {
            if (m_UndoList.Count > 0)
            {
                var cmd = m_UndoList.Pop();
                if (cmd.UnRun(this))
                {
                    m_RedoList.Push(cmd);
                }
            }
        }

        public void ReDo()
        {
            if (m_RedoList.Count > 0)
            {
                var cmd = m_RedoList.Pop();
                if (cmd.ReRun(this))
                {
                    m_UndoList.Push(cmd);
                }
            }
        }
    }
}