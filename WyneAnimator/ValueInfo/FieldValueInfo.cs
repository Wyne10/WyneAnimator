using System;
using System.Reflection;
using UnityEngine;

namespace WS.WyneAnimator
{
    public class FieldValueInfo : IValueInfo
    {
        public Type GetValueType(MemberInfo memberInfo)
        {
            return ((FieldInfo)memberInfo).FieldType;
        }

        public void SetValue(MemberInfo memberInfo, object obj, object value)
        {
            ((FieldInfo)memberInfo).SetValue(obj, value);
        }

        public object GetValue(MemberInfo memberInfo, object obj)
        {
            return ((FieldInfo)memberInfo).GetValue(obj);
        }

        public bool CheckType(MemberInfo memberInfo)
        {
            if (((FieldInfo)memberInfo).FieldType == typeof(int)) return true;
            else if (((FieldInfo)memberInfo).FieldType == typeof(float)) return true;
            else if (((FieldInfo)memberInfo).FieldType == typeof(long)) return true;
            else if (((FieldInfo)memberInfo).FieldType == typeof(double)) return true;
            else if (((FieldInfo)memberInfo).FieldType == typeof(Color)) return true;
            else if (((FieldInfo)memberInfo).FieldType == typeof(Vector2)) return true;
            else if (((FieldInfo)memberInfo).FieldType == typeof(Vector3)) return true;
            else if (((FieldInfo)memberInfo).FieldType == typeof(bool)) return true;
            return false;
        }
    }
}
