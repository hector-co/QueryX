using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace QueryX
{
    public class QueryMappingConfig
    {
        private static readonly QueryMappingConfig _instance = new QueryMappingConfig();
        private static readonly ModelMapping _defaultMapping = new ModelMapping();
        private readonly ConcurrentDictionary<Type, ModelMapping> _mappings = new ConcurrentDictionary<Type, ModelMapping>();

        public static QueryMappingConfig Global => _instance;

        public QueryMappingConfig For<TModel>(Action<ModelMappingConfig<TModel>> modelMappingConfig)
        {
            if (_mappings.TryGetValue(typeof(TModel), out var mapping))
                modelMappingConfig(new ModelMappingConfig<TModel>(mapping));

            mapping = new ModelMapping();
            _mappings.TryAdd(typeof(TModel), mapping);
            modelMappingConfig(new ModelMappingConfig<TModel>(mapping));

            return this;
        }

        public void Clear<TModel>()
        {
            if (_mappings.ContainsKey(typeof(TModel)))
                _mappings.TryRemove(typeof(TModel), out _);
        }

        internal ModelMapping GetMapping<TModel>() =>
            GetMapping(typeof(TModel));

        internal ModelMapping GetMapping(Type modelType) =>
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
