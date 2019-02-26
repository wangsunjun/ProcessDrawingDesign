using System;

namespace Wss.FoundationCore.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DesignPropertyAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="disName">属性名</param>
        /// <param name="bindPath">绑定路径</param>
        public DesignPropertyAttribute(string disName, string bindPath)
        {
            DisplayName = disName;
            BindPath = bindPath;
        }
        public string Guid { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// 绑定路径
        /// </summary>
        public string BindPath { get; private set; }

        public Type Editer { get; set; }
        public bool ReadOnly { get; set; }

        public override object TypeId
        {
            get { return this; }
        }
    }
}