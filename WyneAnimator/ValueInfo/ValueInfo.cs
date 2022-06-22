using System;
using System.Reflection;

namespace WS.WyneAnimator
{
    public class ValueInfo
    {
        private MemberInfo _memberInfo;
        private IValueInfo _valueInfo;

        public ValueInfo(FieldInfo reflectedField)
        {
            _memberInfo = reflectedField;
            _valueInfo = new FieldValueInfo();
        }

        public ValueInfo(PropertyInfo reflectedProperty)
        {
            _memberInfo = reflectedProperty;
            _valueInfo = new PropertyValueInfo();
        }

        public MemberInfo MemberInfo { get => _memberInfo; }

        public string Name { get { return _memberInfo.Name; } }
        
        public Type Type { get { return _valueInfo.GetValueType(_memberInfo); } }

        public void SetValue(object obj, object value)
        {
            _valueInfo.SetValue(_memberInfo, obj, value);
        }

        public object GetValue(object obj)
        {
            return _valueInfo.GetValue(_memberInfo, obj);
        }

        public bool CheckType()
        {
            return _valueInfo.CheckType(_memberInfo);
        }
    }
}

