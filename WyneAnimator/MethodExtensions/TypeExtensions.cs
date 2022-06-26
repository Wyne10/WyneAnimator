using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace WS.WyneAnimator
{
    public static class TypeExtensions
    {
        public static PropertyInfo[] ExcludeTypeProperties(this PropertyInfo[] excludeFrom, PropertyInfo[] toExclude)
        {
            List<PropertyInfo> excludedProperties = new List<PropertyInfo>();

            foreach (PropertyInfo property in excludeFrom)
            {
                bool exclude = false;

                foreach (PropertyInfo excludeProperty in toExclude)
                {
                    if (property.MetadataToken == excludeProperty.MetadataToken)
                    {
                        exclude = true;
                        break;
                    }
                }

                if (!exclude)
                    excludedProperties.Add(property);
            }

            return excludedProperties.ToArray();
        }

        public static List<PropertyInfo> ExcludeType(this Type excludeFrom, Type toExclude)
        {
            List<PropertyInfo> excludedProperties = new List<PropertyInfo>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (PropertyInfo property in excludeFrom.GetProperties(flags).ExcludeTypeProperties(toExclude.GetProperties()))
            {
                excludedProperties.Add(property);
            }

            return excludedProperties;
        }

        public static bool CheckPropertyType(this PropertyInfo property)
        {
            if (property.PropertyType == typeof(int) && property.CanWrite) return true;
            else if (property.PropertyType == typeof(float) && property.CanWrite) return true;
            else if (property.PropertyType == typeof(long) && property.CanWrite) return true;
            else if (property.PropertyType == typeof(double) && property.CanWrite) return true;
            else if (property.PropertyType == typeof(Color) && property.CanWrite) return true;
            else if (property.PropertyType == typeof(Vector2) && property.CanWrite) return true;
            else if (property.PropertyType == typeof(Vector3) && property.CanWrite) return true;
            else if (property.PropertyType == typeof(bool) && property.CanWrite) return true;
            return false;
        }

        public static void IncludeProperty(this PropertyInfo toInclude, ref List<PropertyInfo> propertyInfoList)
        {
            if (toInclude != null)
            {
                propertyInfoList.Add(toInclude);
            }
        }

        public static void ExcludeProperty(this PropertyInfo toExclude , ref List<PropertyInfo> propertyInfoList)
        {
            if (toExclude != null)
            {
                propertyInfoList.Remove(toExclude);
            }
        }
    }
}
