using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wss.FoundationCore
{
    public interface IObjcet
    {
        void Put();
        void Put(string plus);
    }
    public class PlugPut : IObjcet
    {
        private string plugName = "my plugName value is default!";
        public string PlugName
        {
            get { return plugName; }
            set { plugName = value; }
        }

        public PlugPut() { }
        public PlugPut(string plusName)
        {
            this.PlugName = plusName;
        }

        public void Put()
        {
            Console.WriteLine("Default plug value is:" + plugName);
        }
        public void Put(string plus)
        {
            Console.WriteLine("Put plus value is:" + plus);
        }
    }

    public class TypeHelper
    {
        public static object CreateObject<T>(params object[] args) where T : IObjcet
        {
            try
            {
                Type myType = typeof(T);
                int lenght = 0;
                if (args != null)
                {
                    lenght = args.Length;
                }

                Type[] types = new Type[lenght];
                for (int i = 0; i < args.Length; i++)
                {
                    types[i] = args[i].GetType();
                }

                object[] param = new object[lenght];
                for (int i = 0; i < args.Length; i++)
                {
                    param[i] = args[i];
                }

                object obj = null;

                // Get the constructor that takes an integer as a parameter.
                ConstructorInfo constructorInfoObj = myType.GetConstructor(types);
                if (constructorInfoObj != null)
                {
                    Console.WriteLine("The constructor of PlugPut that takes an integer as a parameter is: " + constructorInfoObj.ToString());
                    //调用指定参数的构造函数
                    obj = constructorInfoObj.Invoke(param);
                }
                else
                {
                    Console.WriteLine("The constructor of PlugPut that takes an integer as a parameter is not available.");
                    obj = Activator.CreateInstance(myType, null);
                }
                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class CustomBinder : Binder
    {
        public override MethodBase BindToMethod(  BindingFlags bindingAttr, MethodBase[] match, ref object[] args,  ParameterModifier[] modifiers, CultureInfo culture, string[] names,out object state)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
            // Arguments are not being reordered.
            state = null;
            // Find a parameter match and return the first method with
            // parameters that match the request.
            foreach (MethodBase mb in match)
            {
                ParameterInfo[] parameters = mb.GetParameters();

                if (ParametersMatch(parameters, args))
                {
                    return mb;
                }
            }
            return null;
        }

        public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
            foreach (FieldInfo fi in match)
            {
                if (fi.GetType() == value.GetType())
                {
                    return fi;
                }
            }
            return null;
        }

        public override MethodBase SelectMethod(  BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            // Find a parameter match and return the first method with
            // parameters that match the request.
            foreach (MethodBase mb in match)
            {
                ParameterInfo[] parameters = mb.GetParameters();
                if (ParametersMatch(parameters, types))
                {
                    return mb;
                }
            }

            return null;
        }

        public override PropertyInfo SelectProperty( BindingFlags bindingAttr, PropertyInfo[] match, Type returnType, Type[] indexes, ParameterModifier[] modifiers)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
            foreach (PropertyInfo pi in match)
            {
                if (pi.GetType() == returnType &&
                    ParametersMatch(pi.GetIndexParameters(), indexes))
                {
                    return pi;
                }
            }
            return null;
        }

        public override object ChangeType( object value, Type myChangeType, CultureInfo culture)
        {
            try
            {
                object newType;
                newType = Convert.ChangeType(value, myChangeType);
                return newType;
            }
            // Throw an InvalidCastException if the conversion cannot
            // be done by the Convert.ChangeType method.
            catch (InvalidCastException)
            {
                return null;
            }
        }

        public override void ReorderArgumentArray(ref object[] args, object state)
        {
            // No operation is needed here because BindToMethod does not
            // reorder the args array. The most common implementation
            // of this method is shown below.

            // ((BinderState)state).args.CopyTo(args, 0);
        }

        // Returns true only if the type of each object in a matches
        // the type of each corresponding object in b.
        private bool ParametersMatch(ParameterInfo[] a, object[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].ParameterType != b[i].GetType())
                {
                    return false;
                }
            }
            return true;
        }

        // Returns true only if the type of each object in a matches
        // the type of each corresponding entry in b.
        private bool ParametersMatch(ParameterInfo[] a, Type[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].ParameterType != b[i])
                {
                    return false;
                }
            }
            return true;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            #region
            Console.Write("Put plus value is:");
            string strPlus = Console.ReadLine();

            //无参构造函数
            IObjcet obj = (IObjcet)TypeHelper.CreateObject<PlugPut>();
            obj.Put();
            obj.Put(strPlus);

            //定义构造函数所需参数
            object[] param = new object[1];
            param[0] = strPlus;

            //带参数的构造函数
            obj = (IObjcet)TypeHelper.CreateObject<PlugPut>(param);
            obj.Put();
            obj.Put(strPlus);

            #endregion

            Console.ReadLine();
        }
    }

}
