using System;
using System.ComponentModel;

namespace Wss.FoundationCore.Attributes
{
    public interface ISerializeProperty
    {
        /// <summary>
        ///     属性名称
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     属性信息
        /// </summary>
        PropertyDescriptor Descriptor { get; }

        /// <summary>
        ///     属性值类型转换器
        /// </summary>
        TypeConverter Converter { get; }

        /// <summary>
        ///     属性所有者
        /// </summary>
        object Owner { get; }

        /// <summary>
        ///     获取属性值
        /// </summary>
        /// <returns></returns>
        object GetValue();

        /// <summary>
        ///     设置属性值
        /// </summary>
        /// <param name="value"></param>
        void SetValue(object value);

        string guid { get; set; }
    }
}