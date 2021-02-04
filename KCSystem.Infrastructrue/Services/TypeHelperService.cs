using System;
using System.Reflection;
using KCSystem.Core.Interface;

namespace KCSystem.Infrastructrue.Services
{
    public class TypeHelperService : ITypeHelperService
    {
        public bool TypeHasProperties<T>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();
                
                var indexOfFirstSpace = propertyName.IndexOf(" ", StringComparison.Ordinal);
                    propertyName = indexOfFirstSpace == -1 ? propertyName : propertyName.Remove(indexOfFirstSpace);
                if (string.IsNullOrEmpty(propertyName))
                {
                    continue;
                }

                var propertyInfo = typeof(T)
                    .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
