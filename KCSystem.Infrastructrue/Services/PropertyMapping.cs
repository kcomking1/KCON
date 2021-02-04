using System.Collections.Generic;
using KCSystem.Core.Entities;
using KCSystem.Core.Interface;
using KCSystem.Core.Pages;

namespace KCSystem.Infrastructrue.Services
{
    public abstract class PropertyMapping<TSource, TDestination> : IPropertyMapping 
        where TDestination : Entity
    {
        public Dictionary<string, List<MappedProperty>> MappingDictionary { get; }

        protected PropertyMapping(Dictionary<string, List<MappedProperty>> mappingDictionary)
        {
            MappingDictionary = mappingDictionary;
            MappingDictionary[nameof(Entity.Id)] = new List<MappedProperty>
            {
                new MappedProperty { Name = nameof(Entity.Id), Revert = false}
            };
        }
    }
}
