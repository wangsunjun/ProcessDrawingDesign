using System;
using System.ComponentModel;

namespace Wss.FoundationCore.Attributes.Internal
{
    internal class SerializePropertyImpl : ISerializeProperty
    {
        private string _name;
        public Type OwnerType { get; set; }

        /// <summary>
        ///     属性所有者
        /// </summary>
        public object Owner { get; set; }

        /// <summary>
        ///     属性信息
        /// </summary>
        public PropertyDescriptor Descriptor { get; set; }

        /// <summary>
        ///     属性值类型转换器
        /// </summary>
        public TypeConverter Converter { get; set; }

        public string Name
        {
            get
            {
                if (_name == null && Descriptor != null && OwnerType != null)
                {
                    _name = Descriptor.ComponentType == OwnerType
                        ? Descriptor.Name
                        : Descriptor.ComponentType.Name + "." + Descriptor.Name;
                }
                return _name;
            }
        }
        private string _guid;
        public string guid
        {
            get
            {
                return _guid;
            }

            set
            {
                _guid = value;
            }
        }

        public object GetValue()
        {
            return Descriptor.GetValue(Owner);
        }

        public void SetValue(object value)
        {
            Descriptor.SetValue(Owner, value);
        }

        /// <summary>
        /// 依赖绑定转移
        /// </summary>
        /// <param name="obj"></param>
        public virtual void CopyFrom(object obj)
        {
            var sp = (SerializePropertyImpl) obj;

            Converter = sp.Converter;
            Descriptor = sp.Descriptor;
            OwnerType = sp.OwnerType;
            Owner = sp.Owner;
        }

        public static SerializePropertyImpl Create(Type ownerType, PropertyDescriptor pd, TypeConverter converter)
        {
            return new SerializePropertyImpl
            {
                OwnerType = ownerType,
                Descriptor = pd,
                Converter = converter
            };
        }
    }
}