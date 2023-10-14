using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QueryX
{
    public static class QueryMappingConfig
    {
        private static readonly Dictionary<Type, ModelMapping> _mappings = new Dictionary<Type, ModelMapping>();
        private static readonly ModelMapping _defaultMapping = new ModelMapping();

        public static ModelMappingConfig<TModel> For<TModel>()
        {
            if (_mappings.TryGetValue(typeof(TModel), out var mapping))
                return new ModelMappingConfig<TModel>(mapping);

            mapping = new ModelMapping();
            _mappings.Add(typeof(TModel), mapping);
            return new ModelMappingConfig<TModel>(mapping);
        }

        public static void Clear<TModel>()
        {
            if (_mappings.ContainsKey(typeof(TModel)))
                _mappings.Remove(typeof(TModel));
        }

        internal static ModelMapping GetMapping<TModel>() =>
            GetMapping(typeof(TModel));

        internal static ModelMapping GetMapping(Type modelType) =>
            _mappings.TryGetValue(modelType, out var mapping)
                    ? mapping
                    : _defaultMapping;
    }

    public class ModelMappingConfig<TModel>
    {
        private readonly ModelMapping _mapping;

        internal ModelMappingConfig(ModelMapping mapping)
        {
            _mapping = mapping;
        }

        public PropertyMappingConfig<TModel> Property<TValue>(Expression<Func<TModel, TValue>> propertyName)
        {
            return !(propertyName.Body is MemberExpression member)
                ? throw new ArgumentException($"Expression '{propertyName}' refers to a method, not a property.")
                : new PropertyMappingConfig<TModel>(_mapping, member.Member.Name);
        }
    }

    public class PropertyMappingConfig<TModel>
    {
        private readonly ModelMapping _mapping;
        private readonly string _propertyName;

        internal PropertyMappingConfig(ModelMapping mapping, string propertyName)
        {
            _mapping = mapping;
            _propertyName = propertyName;
        }

        public PropertyMappingConfig<TModel> MapFrom(string source)
        {
            _mapping.AddPropertyMapping(_propertyName, source);
            return this;
        }

        public void Ignore()
        {
            _mapping.IgnoreProperty(_propertyName);
        }
    }
}
