using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UltimateInjector
{
    public class Container
    {

        private readonly Dictionary<Type, Type> _registeredTypes;
        private readonly Dictionary<Type, ConstructorInfo> _registeredConstructors;
        private readonly Dictionary<Type, IEnumerable<PropertyInfo>> _registeredProperties;

        public Container()
        {
            _registeredTypes = new Dictionary<Type, Type>();
            _registeredConstructors = new Dictionary<Type, ConstructorInfo>();
            _registeredProperties = new Dictionary<Type, IEnumerable<PropertyInfo>>();
        }

        public void AddType(Type type)
        {
            _registeredTypes.Add(type, type);
        }

        public void AddType(Type instanceType, Type contractType)
        {
            _registeredTypes.Add(contractType, instanceType);
        }

        public object CreateInstance(Type requestedType)
        {
            return CreateInstanceWithCheck(requestedType, new List<Type>());
        }

        private object CreateInstanceWithCheck(Type requestedType, List<Type> derivedTypes)
        {
            //Returns instance of class that registered via ImportConstructorAttribute
            if (_registeredConstructors.ContainsKey(requestedType))
            {
                var constructor = _registeredConstructors[requestedType];
                if (constructor.GetParameters()
                    .All(p => _registeredTypes.Keys
                            .Union(_registeredConstructors.Keys)
                            .Union(_registeredProperties.Keys)
                            .Contains(p.ParameterType)) && !derivedTypes.Contains(requestedType))
                {
                    derivedTypes.Add(requestedType);
                    return constructor.Invoke(constructor.GetParameters()
                         .Select(param => CreateInstanceWithCheck(param.ParameterType, derivedTypes))
                         .ToArray());
                }
            }

            //Returns instance of class that registered via ImportAttribute

            if (_registeredProperties.ContainsKey(requestedType))
            {
                var instance = Activator.CreateInstance(requestedType);
                var properties = _registeredProperties[requestedType].ToList();

                if (properties.All(prop => _registeredTypes.Keys
                        .Union(_registeredConstructors.Keys)
                        .Union(_registeredProperties.Keys)
                        .Contains(prop.PropertyType)) && !derivedTypes.Contains(requestedType))
                {
                    derivedTypes.Add(requestedType);
                    foreach (var property in properties)
                    {
                        property.SetValue(instance, CreateInstanceWithCheck(property.PropertyType, derivedTypes));
                    }

                    return instance;
                }
            }

            //Returns instance of class that registered via Export attribute or AddType method
            var targetType = _registeredTypes[requestedType];

            var constructors = targetType.GetConstructors()
                .Where(c =>
                    c.GetParameters()
                        .All(t => _registeredTypes.ContainsKey(t.ParameterType)) && c.GetParameters().Any())
                .OrderByDescending(c => c.GetParameters().Length).ToArray();

            if (constructors.Any())
            {
                var constructor = constructors.First();
                derivedTypes.Add(requestedType);
                var parameters = constructor.GetParameters()
                    .Select(param => CreateInstanceWithCheck(param.ParameterType, derivedTypes))
                    .ToArray();
                return constructor.Invoke(parameters);
            }

            return Activator.CreateInstance(targetType);

        }


        public T CreateInstance<T>()
            where T : class
        {
            return CreateInstance(typeof(T)) as T;
        }

        public void AddAssembly(Assembly assembly)
        {

            foreach (var type in assembly.GetTypes())
            {
                var isRegistered = false;
                foreach (var constructor in type.GetConstructors())
                {
                    var constructorAttribute = constructor.GetCustomAttribute<ImportConstructorAttribute>();
                    if (constructorAttribute == null) continue;

                    _registeredConstructors.Add(type, constructor);
                    isRegistered = true;
                }

                var properties = type.GetProperties().Where(prop => prop.GetCustomAttribute<ImportAttribute>() != null).ToList();
                if (properties.Any())
                {
                    if (isRegistered)
                    {
                        throw new Exception($"Dependency for class {type.FullName} is already registered");
                    }
                    _registeredProperties.Add(type, properties);
                    isRegistered = true;
                }


                var attribute = type.GetCustomAttribute<ExportAttribute>();
                if (attribute == null) continue;
                if (isRegistered)
                {
                    throw new Exception($"Dependency for class {type.FullName} is already registered");
                }
                _registeredTypes.Add(attribute.Contract ?? type, type);
            }

            #region forLogging

            //Console.WriteLine("Export");
            //foreach (var type in assembly.GetTypes())
            //{
            //    var attribute = type.GetCustomAttribute<ExportAttribute>();
            //    if (attribute == null) continue;

            //    Console.WriteLine($"{(attribute.Contract ?? type).FullName} : {type.FullName}");
            //    _registeredTypes.Add(attribute.Contract ?? type, type);
            //}

            //Console.WriteLine("ImportConstructor");
            //foreach (var type in assembly.GetTypes())
            //{
            //    foreach (var constructor in type.GetConstructors())
            //    {
            //        var attribute = constructor.GetCustomAttribute<ImportConstructorAttribute>();
            //        if (attribute == null) continue;

            //        Console.WriteLine($"{constructor}");
            //        _registeredConstructors.Add(type, constructor);
            //    }
            //}

            //Console.WriteLine("Import");
            //foreach (var type in assembly.GetTypes())
            //{
            //    foreach (var property in type.GetProperties())
            //    {
            //        var attribute = property.GetCustomAttribute<ImportAttribute>();
            //        if (attribute == null) continue;

            //        Console.WriteLine($"{property}");
            //        _registeredProperties.Add(type, property);
            //    }
            //}


            #endregion
        }
    }
}
