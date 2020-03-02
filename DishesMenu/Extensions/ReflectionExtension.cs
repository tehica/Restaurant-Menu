using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DishesMenu.Extensions
{
    public static class ReflectionExtension
    {
        public static string GetPropertyValue<T>(this T item, string propertyName)
        {
            return item.GetType().GetProperty(propertyName).GetValue(item, null).ToString();
        }

        // this extension method gets the value of whatever property we pass here
    }
}
