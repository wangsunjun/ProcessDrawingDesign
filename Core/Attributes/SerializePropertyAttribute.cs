using System;

namespace Wss.FoundationCore.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SerializePropertyAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bindPath">绑定路径</param>
        public SerializePropertyAttribute(string bindPath)
        {
            BindPath = bindPath;
        }

        /// <summary>
        /// 绑定路径
        /// </summary>
        public string BindPath { get; private set; }

        public Type Converter { get; set; }

        public override object TypeId
        {
            get { return this; }
        }
    }
}