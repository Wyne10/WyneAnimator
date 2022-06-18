using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

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

        public static bool CheckReflectedValue(this FieldInfo field)
        {
            if (field.FieldType == typeof(int)) return true;
            else if (field.FieldType == typeof(float)) return true;
            else if (field.FieldType == typeof(double)) return true;
            else if (field.FieldType == typeof(long)) return true;
            else if (field.FieldType == typeof(Color)) return true;
            else if (field.FieldType == typeof(Vector2)) return true;
            else if (field.FieldType == typeof(Vector3)) return true;
            else if (field.FieldType == typeof(Vector4)) return true;
            else if (field.FieldType == typeof(Quaternion)) return true;
            else if (field.FieldType == typeof(bool)) return true;
            return false;
        }

        public static bool CheckReflectedValue(this PropertyInfo property)
        {
            if (property.PropertyType == typeof(int) && property.CanWrite) return true;
            else if (property.PropertyType == typeof(float) && property.CanWrite) return true;
            else if (property.PropertyType == typeof(double) && property.CanWrite) return true;
            else if (property.PropertyType == typeof(long) && property.CanWrite) return true;
            else if (property.PropertyType == typeof(Color) && property.CanWrite) return true;
            else if (property.PropertyType == typeof(Vector2) && property.CanWrite) return true;
            else if (property.PropertyType == typeof(Vector3) && property.CanWrite) return true;
            else if (property.PropertyType == typeof(Vector4) && property.CanWrite) return true;
            else if (property.PropertyType == typeof(Quaternion) && property.CanWrite) return true;
            else if (property.PropertyType == typeof(bool) && property.CanWrite) return true;
            return false;
        }

        public static void ExcludeTypes(this Type excludeFrom, Type toExclude, out List<FieldInfo> excludedFields, out List<PropertyInfo> excludedProperties)
        {
            excludedFields = new List<FieldInfo>();
            excludedProperties = new List<PropertyInfo>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            
            excludedFields.AddArray(excludeFrom.GetFields(flags).ExcludeTypeMembers(toExclude.GetFields()));
            excludedProperties.AddArray(excludeFrom.GetProperties(flags).ExcludeTypeMembers(toExclude.GetProperties()));
        }
    }
}
