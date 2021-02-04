using System.Collections.Generic;
using KCSystem.Core.Pages;

namespace KCSystem.Core.Interface
{
    public interface IPropertyMapping
    {
        Dictionary<string, List<MappedProperty>> MappingDictionary { get; }
    }
}