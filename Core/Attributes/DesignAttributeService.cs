using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;
using System.Xaml.Schema;
using System.Xml.Linq;
using Wss.FoundationCore.Attributes.Internal;
using OmniLib.Reflection;
using OmniLib.Common;
using Wss.FoundationCore.Controls;

namespace Wss.FoundationCore.Attributes
{
    public class DesignAttributeService
    {
        #region 全局变量
        private readonly Type _type;//要服务的控件类 Type
        private readonly IEnumerable<IDesignProperty> _viewPropertyList;    //控件属性列表
        private readonly IEnumerable<ISerializeProperty> _serializPropertyList;
        private readonly IEnumerable<IModelProperty> _ModelPropertyList;    //model的属性列表
        private const string _DesignerelementStr = "DesignerItemControl";
        private const string _VersionStr = "Version";
        private const string _TypeStr = "Type";
        private static readonly Dictionary<Type, DesignAttributeService> _DicDesignerServiceCache = new Dictionary<Type, DesignAttributeService>();
        private static readonly PropertyInfo _PropertyPathLastAccessor = typeof(PropertyPath).GetProperty("LastAccessor", BindingFlags.Instance | BindingFlags.NonPublic);
        private readonly PropertyDescriptorCollection _properties;

        #endregion

        public DesignAttributeService(Type type)
        {
            _type = type;
            _properties = TypeDescriptor.GetProperties(_type);
            _viewPropertyList = GetViewPropertyList();
            _serializPropertyList = GetSerializPropertyList();
            _ModelPropertyList = GetModelPropertyList();
        }

        public static List<ISerializeProperty> GetSerializPropertys(DependencyObject obj)//获得序列化对象属性
        {
            if (obj == null) return new List<ISerializeProperty>();
            DesignAttributeService dasvc = TryGetService(obj);
            if (dasvc == null) { return new List<ISerializeProperty>(); }
            var result = dasvc._serializPropertyList.Select(x =>
            {
                var pro = new SerializePropertyImpl();//和下面的 DesignPropertyImpl 不同
                pro.CopyFrom(x);
                pro.Owner = obj;
                //pro.guid
                return (ISerializeProperty) pro;
            }).ToList();
            return result;
        }
        public static List<IDesignProperty> GetDesignPropertys(DependencyObject dObj)//获得DesignerItem控件属性 DesignerItem外层封装了一个DesignerContainer 设计时点击控件，显示控件PropertyView时
        {
            if (dObj == null)  return new List<IDesignProperty>();
            DesignAttributeService dasvc = TryGetService(dObj);
            if (dasvc == null)  return new List<IDesignProperty>(); 
            List<IDesignProperty> result = dasvc._viewPropertyList.Select(o =>//将控件属性与控件依赖绑定
            {
                DesignPropertyImpl dpi = new DesignPropertyImpl();
                dpi.CopyFrom(o);
                dpi.Owner = dObj;
                dpi.Binding.Source = dObj;
                //string sss = dpi.
                return (IDesignProperty)dpi;
            }).ToList();

            IDesignerChildable childable = dObj as IDesignerChildable;
            if (childable != null) result.AddRange(GetDesignPropertys(childable.DesignerChild));//递归获得DesignerContainer.Content 里面的控件的详细属性
            return result;
        }
        public static List<IModelProperty> GetModelPropertys(DependencyObject obj)
        {
            if (obj == null) return new List<IModelProperty>();
            DesignAttributeService service = TryGetService(obj);
            if (service == null)  return new List<IModelProperty>(); 
            var result = service._ModelPropertyList.Select(x =>
            {
                var pro = new ModelPropertyImpl();
                pro.CopyFrom(x);
                pro.Owner = obj;
                if (pro.Binding != null)
                {
                    pro.Binding.Source = obj;
                }
                return (IModelProperty) pro;
            }).ToList();

            IDesignerChildable childable = obj as IDesignerChildable;
            if (childable != null)
            {
                result.AddRange(GetModelPropertys(childable.DesignerChild));
            }
            return result;
        }

        public static XElement SerializToXElement(DependencyObject obj)
        {
            var pros = GetSerializPropertys(obj);
            if (pros == null) return null;
            var root = new XElement(_DesignerelementStr);
            root.SetAttributeValue(_VersionStr, "1.0");
            root.SetAttributeValue(_TypeStr, Regex.Match(obj.GetType().AssemblyQualifiedName, "^[^,]+,[^,]+").Value);
            foreach (var pro in pros)
            {
                var value = pro.GetValue();
                if (value == null) continue;
                root.SetElementValue(pro.Name, pro.Converter.ConvertToInvariantString(value));
            }
            var childable = obj as IDesignerChildable;
            if (childable != null)  root.Add(SerializToXElementControl(childable.DesignerChild));

            return root;
        }
        public static XElement SerializToXElementControl(DependencyObject obj)
        {
            var pros = GetSerializPropertys(obj);
            if (pros == null) return null;
            var root = new XElement(_DesignerelementStr);
            root.SetAttributeValue(_VersionStr, "1.0");
            root.SetAttributeValue(_TypeStr, Regex.Match(obj.GetType().AssemblyQualifiedName, "^[^,]+,[^,]+").Value);
            foreach (var pro in pros)
            {
                var value = pro.GetValue();
                if (value == null) continue;
                XElement x = new XElement(pro.Name);
                x.Value = pro.Converter.ConvertToInvariantString(value);
                x.SetAttributeValue("ID", Guid.NewGuid().ToString());
                //pro.guid
                root.Add(x);
                //root.SetElementValue(pro.Name, pro.Converter.ConvertToInvariantString(value));
                //root.SetElementValue("ID", Guid.NewGuid().ToString());
            }
            var childable = obj as IDesignerChildable;
            if (childable != null) root.Add(SerializToXElement(childable.DesignerChild));

            return root;
        }
        public static DependencyObject DeSerializFromXElement(XElement xml)
        {
            #region xml
            /*< DesignerElement Version = "1.0" Type = "Wss.Foundation.Controls.DesignerContainer, Wss.Foundation" >
                 < Canvas.Left > 522 </ Canvas.Left >
                 < Canvas.Top > 257 </ Canvas.Top >
                 < Width > 142 </ Width >
                 < Height > 200 </ Height >
                 < Panel.ZIndex > 16 </ Panel.ZIndex >
                 < Angle > 0 </ Angle >
                 < ScaleX > 1 </ ScaleX >
                 < ScaleY > 1 </ ScaleY >
                 < ItemOpacity > 1 </ ItemOpacity >
                 < LockDesign > False </ LockDesign >
                 < WindowTip > False </ WindowTip >
                 < TagVisible > True </ TagVisible >
                 < NamePosition ></ NamePosition >
                 < ID > 97e23167 - 453d - 44fb - 8914 - 1435cd8f5ec1 </ ID >
                < Name ></ Name >
                < ParentID > 00000000 - 0000 - 0000 - 0000 - 000000000000 </ ParentID >
                < IsGroup > False </ IsGroup >
                < DesignerElement Version = "1.0" Type = "Common.Package.HeatExchanger, Common.Package" >
                     < Line.Tag > 模型.换热器 </ Line.Tag >
                   </ DesignerElement >
                 </ DesignerElement >
                 */
            #endregion
            if (!ValidXml(xml)) return null;
            XAttribute typeAttribute = xml.Attribute(_TypeStr);
            DependencyObject dobj = GetItemObject(typeAttribute);
            if (dobj == null) return null;
            IDesignerChildable childable = dobj as IDesignerChildable;
            XElement childelement = xml.Element(_DesignerelementStr);
            if (childable != null && childelement != null)
            {
                DependencyObject child = DeSerializFromXElementControl(childelement);//递归解析DesignerContainer.Content
                childable.DesignerChild = child;
            }
            var pros = GetSerializPropertys(dobj);
            if (pros != null)
            {
                foreach (var pro in pros)
                {
                    XElement element = xml.Element(pro.Name);
                    if (element == null) continue;
                    if (element.Value == "NaN") continue;
                    if (pro.Descriptor.PropertyType == typeof(object)) pro.SetValue(element.Value);
                    else
                    {
                        pro.SetValue(pro.Converter.ConvertFromString(element.Value));
                    }
                }
            }
            return dobj;
        }
        public static DependencyObject DeSerializFromXElementControl(XElement xml)
        {
            if (!ValidXml(xml)) return null;
            XAttribute typeAttribute = xml.Attribute(_TypeStr);
            DesignerItem dobj = GetItemObjectControl(typeAttribute);
            if (dobj == null) return null;
            IDesignerChildable childable = dobj as IDesignerChildable;
            XElement childelement = xml.Element(_DesignerelementStr);
           
            var pros = GetSerializPropertys(dobj);
            dobj._listProperty = pros;
            if (pros != null)
            {
                foreach (var pro in pros)
                {
                    XElement element = xml.Element(pro.Name);
                    if (element == null) continue;
                    if (element.Value == "NaN") continue;
                    try
                    {
                        pro.guid = element.Attribute("ID").Value;
                    }
                    catch { pro.guid = Guid.NewGuid().ToString() ; }

                    if (pro.Descriptor.PropertyType == typeof(object)) pro.SetValue(element.Value);
                    else
                    {
                        pro.SetValue(pro.Converter.ConvertFromString(element.Value));
                    }
                }
            }
            return dobj;
        }
        public static List<ISerializeProperty> DeSerializFromXElementProperty(XElement xml)
        {

            XAttribute typeAttribute = xml.Attribute(_TypeStr);
            DependencyObject dobj = GetItemObject(typeAttribute);
            if (dobj == null) return null;
            var pros = GetSerializPropertys(dobj);
            if (pros != null)
            {
                foreach (var pro in pros)
                {
                    XElement element = xml.Element(pro.Name);
                    if (element == null) continue;
                    if (element.Value == "NaN") continue;
                    pro.guid = element.Attribute("ID").Value;
                }
            }
            return pros;
        }

        private static DesignAttributeService TryGetService(DependencyObject obj)//根据传过来控件的类型Type获得其属性Service
        {
            Type type = obj.GetType();
            if (!_DicDesignerServiceCache.ContainsKey(type))
            {
                _DicDesignerServiceCache[type] = new DesignAttributeService(type);
            }
            DesignAttributeService service = _DicDesignerServiceCache[type];
            return service;
        }
        private static bool ValidXml(XElement xml)//判断XML是否符合语法
        {
            if (xml == null) return false;
            if (xml.Attribute(_VersionStr) == null)return false;
            if (xml.Attribute(_TypeStr) == null) return false;
            return true;
        }
        private static DependencyObject GetItemObject(XAttribute typeAttribute)//根据属性类全名称创建类的对象DepedencyObject
        {
            string typeFullName = typeAttribute.Value;
            Type type = null;
            DependencyObject obj;

            //if (typeFullName.Contains("Common.Package"))
            //{
            //    var asm = Assembly.LoadFile(@"E:\ECMS\trunk\Development\TestDesigner\bin\Debug\Package\Common.Package.dll");
            //    type = asm.GetType(typeFullName.Split(',')[0]);
            //}
            //else
                type = type.FastGetType(typeFullName);

            if (type == null) return null;

            var fi = type.FastGetConstructor();
            if (fi == null) return null;
            try
            {
                obj = fi.Invoke() as DependencyObject;
            }
            catch{ return null; }
            FrameworkElement femt = obj as FrameworkElement;
            if (femt != null) femt.ApplyTemplate();
            return obj;
            #region
            //var typeFullName = typeAttribute.Value;
            //Type type = null;
            //type = Type.GetType(typeFullName);// ReSharper disable once ExpressionIsAlwaysNull

            //if (type == null) return null;

            ////IObjcet o = (IObjcet)TypeHelper.CreateObject<object>();

            //var fi = type.GetConstructor(new Type[] { });
            //DependencyObject obj;

            //if (fi == null)
            //{
            //    return null;
            //}
            //try
            //{
            //    //obj = fi[0].Invoke(BindingFlags.Default,null) as DependencyObject;
            //    obj = fi.Invoke(new object[] { }) as DependencyObject;
            //}
            //catch(Exception ex)
            //{
            //    return null;
            //}
            //FrameworkElement frameworkElement = obj as FrameworkElement;
            //if (frameworkElement != null)
            //{
            //    frameworkElement.ApplyTemplate();
            //}
            //return  obj;
            #endregion
        }
        private static DesignerItem GetItemObjectControl(XAttribute typeAttribute)//根据属性类全名称创建控件的对象DesignerItem
        {
            string typeFullName = typeAttribute.Value;
            Type type = null;
            DesignerItem obj;
            var asm = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + "Package\\Common.Package.dll");
            type = asm.GetType(typeFullName.Split(',')[0]);
            //type = type.FastGetType(typeFullName);

            if (type == null) return null;

            var fi = type.FastGetConstructor();
            if (fi == null) return null;
            try
            {
                obj = fi.Invoke() as DesignerItem;
            }
            catch { return null; }
            FrameworkElement femt = obj as FrameworkElement;
            if (femt != null) femt.ApplyTemplate();
            return obj;
        }
        private IEnumerable<IDesignProperty> GetViewPropertyList()//获得控件属性列表
        {
            List<IDesignProperty> listProperties = new List<IDesignProperty>();
            if (_type == null) return listProperties;
            //控件的基本属性,X,Y坐标，名称，是否可见等
            _type.EnumerateAttributes<DesignPropertyAttribute, string>(o =>
            {
                //类上属性的抽象化
                PropertyDescriptor pd = TryToGetDependencyProperty(_type, o.BindPath) ?? _properties[o.BindPath];
                if (pd == null) return;
                listProperties.Add(DesignPropertyImpl.Create(_type, pd, o.DisplayName, o.BindPath, o.ReadOnly));
            }, x => x.DisplayName);
            //根据type获得类中的有DesignableAttribute标记的属性 不同控件的不同属性如 线条的颜色，泵的转速等
            _type.EnumeratePropertiesAttributes<DesignableAttribute>((pro, da) =>
            {
                var displayName = pro.DisplayName;
                if (string.IsNullOrWhiteSpace(displayName))
                {
                    displayName = pro.Name;
                }

                listProperties.Add(DesignPropertyImpl.Create(_type, pro, displayName, pro.Name, da.ReadOnly));
            });
            //_type.EnumeratePropertiesAttributes<PropertyGuidAttribute>((pro,o )=>
            //{
            //    //类上属性的抽象化
            //    PropertyDescriptor pd = TryToGetDependencyProperty(_type, o.PropertyGuid) ?? _properties[o.BindPath];
            //    if (pd == null) return;
            //    //listProperties.Add(DesignPropertyImpl.Create(_type, pd, o.DisplayName, o.BindPath, o.ReadOnly));
            //});

            PropertyInfo[] Properties = _type.GetProperties();
            foreach (PropertyInfo item in Properties)
            {
                object[] objArray = item.GetCustomAttributes(false);
                if (objArray.Length >5)
                {
                    string str = (objArray[4] as PropertyGuidAttribute).PropertyGuid;
                    //if ((objArray[0] as PropertyGuidAttribute).PropertyGuid == "Key")
                    //    Console.WriteLine("属性:{0} 是Key主键", item.Name);
                }
            }

            //_type.FastGetPropertys<PropertyGuidAttribute>((pro, da) =>
            //{
            //    var displayName = pro.DisplayName;
            //    if (string.IsNullOrWhiteSpace(displayName))
            //    {
            //        displayName = pro.Name;
            //    }
            //});
            return listProperties;//返回属性集合
        }
        private IEnumerable<IModelProperty>  GetModelPropertyList()
        {
            var result = new List<IModelProperty>();
            if (_type == null) return result;

            _type.EnumerateAttributes<DataModelFieldAttribute, string>(attribute =>
            {
                if (string.IsNullOrWhiteSpace(attribute.DisplayName))
                {
                    attribute.DisplayName = attribute.FieldName;
                }
                if (string.IsNullOrWhiteSpace(attribute.FieldName))
                {
                    attribute.FieldName = attribute.DisplayName;
                }
                PropertyDescriptor pro = null;
                if (attribute.BindPath != null)
                {
                    pro = TryToGetDependencyProperty(_type, attribute.BindPath) ?? _properties[attribute.BindPath];
                }
                var mpi = ModelPropertyImpl.Create(_type, pro, attribute.DisplayName, attribute.FieldName, attribute.BindPath, attribute.TargetGroup);
                //mpi.UnitMark = attribute.UnitMark;
                result.Add(mpi);
            }, x => x.FieldName);
            _type.EnumeratePropertiesAttributes<DataModelFieldAttribute>((descriptor, attribute) =>
            {
                if (descriptor.SupportsChangeEvents)
                {
                    descriptor = TryToGetDependencyProperty(descriptor.ComponentType, descriptor.Name) ?? descriptor;
                }
                var displayName = descriptor.DisplayName;
                if (string.IsNullOrWhiteSpace(displayName))
                {
                    displayName = descriptor.Name;
                }
                if (string.IsNullOrWhiteSpace(attribute.FieldName))
                {
                    attribute.FieldName = displayName;
                }
                var mpi = ModelPropertyImpl.Create(_type, descriptor, displayName, attribute.FieldName, descriptor.Name, attribute.TargetGroup);
                //mpi.UnitMark = attribute.UnitMark;
                result.Add(mpi);
            });

            return result;
        }
        public IEnumerable<ISerializeProperty> GetSerializPropertyList()
        {
            var result = new List<ISerializeProperty>();
            if (_type == null) return result;

            _type.EnumerateAttributes<SerializePropertyAttribute, string>(o =>
            {
                var pd = TryToGetDependencyProperty(_type, o.BindPath) ?? _properties[o.BindPath];
                if (pd == null) return;
                TypeConverter converter = null;
                if (o.Converter != null)
                {
                    converter = CreateInstance(o.Converter, pd.PropertyType) as TypeConverter;
                }
                if (converter == null)
                {
                    converter = TypeDescriptor.GetConverter(pd.PropertyType);
                }
                result.Add(SerializePropertyImpl.Create(_type, pd, converter));
            }, null);

            _type.EnumeratePropertiesAttributes<SerializableAttribute>((pro, da) => { result.Add(SerializePropertyImpl.Create(_type, pro, pro.Converter)); });

            return result;
        }
        
        private static DependencyPropertyDescriptor TryToGetDependencyProperty(Type type, string path)//获得依赖项属性
        {
            var bindProperty = new MarkupPropertyPath(new PropertyPath(path)).GetAccessor(type);

            if (bindProperty != null)
            {
                return DependencyPropertyDescriptor.FromProperty(bindProperty, type);
            }
            var descriptor = DependencyPropertyDescriptor.FromName(path, type, type); 
            return descriptor;
        }
        public object CreateInstance(Type type, Type protype)
        {
            Type[] typeArray =
            {
                typeof (Type)
            };
            if (!(type.GetConstructor(typeArray) != null)) return TypeDescriptor.CreateInstance(null, type, null, null);

            return TypeDescriptor.CreateInstance(null, type, typeArray, new object[]{  protype  });
        }
    }

    public class MarkupPropertyPath
    {
        private PropertyPath _path;
        private static readonly PropertyInfo PathLastAccessor = typeof(PropertyPath).GetProperty("LastAccessor", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo ResolvePropertyName = typeof(PropertyPath).GetMethod("ResolvePropertyName",BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(object), typeof(Type), typeof(object), typeof(bool) }, null);
        public MarkupPropertyPath(PropertyPath path)
        {
            _path = path;
        }
        public DependencyProperty GetAccessor(Type ownerType)
        {
            object[] objs = new object[] { _path.Path, null, ownerType, new DependencyObject(), false } ;
            return ResolvePropertyName.Invoke(_path, objs) as DependencyProperty;
        }
    }
}