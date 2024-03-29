﻿using System;
using System.Reflection;
using System.Collections.Generic;

namespace WS.WyneAnimator
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

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

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

        public static void IncludeProperty(this PropertyInfo toInclude, ref List<ValueInfo> valueInfoList)
        {
            if (toInclude != null)
            {
                valueInfoList.Add(new ValueInfo(toInclude));
            }
        }

        public static void ExcludeProperty(this PropertyInfo toExclude, ref List<ValueInfo> valueInfoList)
        {
            if (toExclude != null)
            {
                foreach (ValueInfo value in valueInfoList)
                {
                    if (value.MemberInfo.MetadataToken == toExclude.MetadataToken)
                    {
                        valueInfoList.Remove(value);
                        return;
                    }
                }
            }
        }
    }
}
