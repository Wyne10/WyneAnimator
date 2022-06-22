using System;
using System.Reflection;
using UnityEngine;

namespace WS.WyneAnimator
{
    public class PropertyValueInfo : IValueInfo
    {
        public Type GetValueType(MemberInfo memberInfo)
        {
            return ((PropertyInfo)memberInfo).PropertyType;
        }

        public void SetValue(MemberInfo memberInfo, object obj, object value)
        {
            ((PropertyInfo)memberInfo).SetValue(obj, value);
        }

        public object GetValue(MemberInfo memberInfo, object obj)
        {
            return ((PropertyInfo)memberInfo).GetValue(obj);
        }

        public bool CheckType(MemberInfo memberInfo)
        {
            if (((PropertyInfo)memberInfo).PropertyType == typeof(int) && ((PropertyInfo)memberInfo).CanWrite) return true;
            else if (((PropertyInfo)memberInfo).PropertyType == typeof(float) && ((PropertyInfo)memberInfo).CanWrite) return true;
            else if (((PropertyInfo)memberInfo).PropertyType == typeof(long) && ((PropertyInfo)memberInfo).CanWrite) return true;
            else if (((PropertyInfo)memberInfo).PropertyType == typeof(double) && ((PropertyInfo)memberInfo).CanWrite) return true;
            else if (((PropertyInfo)memberInfo).PropertyType == typeof(Color) && ((PropertyInfo)memberInfo).CanWrite) return true;
            else if (((PropertyInfo)memberInfo).PropertyType == typeof(Vector2) && ((PropertyInfo)memberInfo).CanWrite) return true;
            else if (((PropertyInfo)memberInfo).PropertyType == typeof(Vector3) && ((PropertyInfo)memberInfo).CanWrite) return true;
            else if (((PropertyInfo)memberInfo).PropertyType == typeof(bool) && ((PropertyInfo)memberInfo).CanWrite) return true;
            return false;
        }
    }
}
