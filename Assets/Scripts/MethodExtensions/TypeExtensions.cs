using System;
using System.Reflection;
using System.Collections.Generic;

namespace WyneAnimator
{
    public static class TypeExtensions
    {
        public static TMembersType[] ExcludeTypeMembers<TMembersType>(this MemberInfo[] excludeFrom, TMembersType[] toExclude) where TMembersType : MemberInfo
        {
            List<TMembersType> excludedTypeMembers = new List<TMembersType>();

            foreach (TMembersType member in (TMembersType[])excludeFrom)
            {
                bool exclude = false;

                foreach (TMembersType excludeMember in toExclude)
                {
                    if (member.MetadataToken == excludeMember.MetadataToken)
                    {
                        exclude = true;
                        break;
                    }
                }

                if (!exclude)
                    excludedTypeMembers.Add(member);
            }

            return excludedTypeMembers.ToArray();
        }

        public static List<ValueInfo> ExcludeType(this Type excludeFrom, Type toExclude)
        {
            List<ValueInfo> excludedValues = new List<ValueInfo>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (FieldInfo field in excludeFrom.GetFields(flags).ExcludeTypeMembers(toExclude.GetFields()))
            {
                excludedValues.Add(new ValueInfo(field));
            }

            foreach (PropertyInfo property in excludeFrom.GetProperties(flags).ExcludeTypeMembers(toExclude.GetProperties()))
            {
                excludedValues.Add(new ValueInfo(property));
            }

            return excludedValues;
        }
    }
}
