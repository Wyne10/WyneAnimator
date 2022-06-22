using System;
using System.Reflection;

namespace WS.WyneAnimator
{
    public interface IValueInfo
    {
        Type GetValueType(MemberInfo memberInfo);
        void SetValue(MemberInfo memberInfo, object obj, object value);
        object GetValue(MemberInfo memberInfo, object obj);
        bool CheckType(MemberInfo memberInfo);
    }
}
