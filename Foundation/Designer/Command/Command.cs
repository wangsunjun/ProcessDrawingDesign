using System;

namespace Wss.Foundation.Designer.Command
{
    /// <summary>
    ///     命令管理者抽象类
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        ///     命令
        /// </summary>
        /// <param name="mgr">命令管理者</param>
        /// <param name="param">命令参数</param>
        /// <returns>需要撤销的命令返回true，不需要的返回false</returns>
        public abstract Boolean Run(CommandManager mgr, params object[] param);

        /// <summary>
        ///     反向移动命令
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="param"></param>
        /// <returns>需要恢复的命令返回true，不需要的返回false</returns>
        public abstract Boolean UnRun(CommandManager mgr);

        //重做命令
        public abstract Boolean ReRun(CommandManager mgr);
    }
}