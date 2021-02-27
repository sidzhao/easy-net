using System;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace EasyNet.Extensions.DependencyInjection
{
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// Sets the value of the property of the object with specified type.
        /// </summary>
        public static void SetValueAndChangeType<TValueType>(this PropertyInfo propertyInfo, object obj, object value)
        {
            propertyInfo.SetValueAndChangeType(obj, value, typeof(TValueType));
        }

        /// <summary>
        /// Sets the value of the property of the object with specified type.
        /// </summary>
        public static void SetValueAndChangeType(this PropertyInfo propertyInfo, object obj, object value, Type valueType)
        {
            Check.NotNull(propertyInfo, nameof(propertyInfo));
            Check.NotNull(obj, nameof(obj));

            if (value == null)
            {
                propertyInfo.SetValue(obj, null);
                return;
            }

            propertyInfo.SetValue(obj, Convert.ChangeType(value, valueType));
        }
    }
}
