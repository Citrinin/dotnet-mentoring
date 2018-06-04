using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UltimateInjector
{
    /// <summary>
    /// Represents class for IoC container
    /// </summary>
    public class Container
    {
        private readonly Dictionary<Type, Type> _registeredTypes;
        private readonly Dictionary<Type, ConstructorInfo> _registeredConstructors;
        private readonly Dictionary<Type, IEnumerable<PropertyInfo>> _registeredProperties;

        /// <summary>
        /// Initializes a new instance of the UltimateInjector.Container class
        /// </summary>
        public Container()
        {
            _registeredTypes = new Dictionary<Type, Type>();
            _registeredConstructors = new Dictionary<Type, ConstructorInfo>();
            _registeredProperties = new Dictionary<Type, IEnumerable<PropertyInfo>>();
        }

        /// <summary>
        /// Allows to register all dependencies that specified in the given assembly
        /// </summary>
        /// <param name="assembly">Assembly that store dependencies</param>
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
                AddType(type, attribute.Contract ?? type);
            }
        }

        /// <summary>
        /// Registers that the instance of specified type will be returned every time it's requested
        /// </summary>
        /// <param name="type">Type for registration</param>
        public void AddType(Type type)
        {
            AddType(type, type);
        }

        /// <summary>
        /// Registers that the instance of specified instance type will be returned every time an contract type requested
        /// </summary>
        /// <param name="instanceType">Type that will be returned</param>
        /// <param name="contractType">The interface or base type that can be used to retrieve the instances</param>
        public void AddType(Type instanceType, Type contractType)
        {
            if (instanceType == null)
            {
                throw new ArgumentNullException(nameof(instanceType), "Parameter can not be null");
            }

            if (contractType == null)
            {
                throw new ArgumentNullException(nameof(instanceType), "Parameter can not be null");
            }

            if (!contractType.IsAssignableFrom(instanceType))
            {
                throw new TypeIsNotAssignedException($"Type {instanceType} not implement {contractType}");
            }

            if (instanceType.IsAbstract)
            {
                throw new ArgumentException($"Type {instanceType.FullName} can not be an abstract class");
            }

            if (instanceType.IsClass)
            {
                _registeredTypes.Add(contractType, instanceType);
            }
            else
            {
                throw new ArgumentException($"Type {instanceType.FullName} is not a class");
            }
        }

        /// <summary>
        /// Returns instance of the given requested type
        /// </summary>
        /// <param name="requestedType">Type of instance that requested</param>
        /// <returns>Instance of requested type</returns>
        public object CreateInstance(Type requestedType)
        {
            if (requestedType == null)
            {
                throw new ArgumentNullException(nameof(requestedType), "Parameter can not be null");
            }

            return CreateInstanceWithCheck(requestedType, new List<Type>());
        }

        /// <summary>
        /// Returns instance of the given requested type
        /// </summary>
        /// <typeparam name="T">Type of instance that requested</typeparam>
        /// <returns>Instance of requested type</returns>
        public T CreateInstance<T>()
            where T : class
        {
            return CreateInstance(typeof(T)) as T;
        }


        private object CreateInstanceWithCheck(Type requestedType, List<Type> derivedTypes)
        {
            if (!_registeredTypes.Keys
                .Union(_registeredConstructors.Keys)
                .Union(_registeredProperties.Keys)
                .Contains(requestedType))
            {
                throw new ArgumentException($"Type {requestedType.FullName} is not registered");
            }

            if (derivedTypes.Contains(requestedType))
            {
                throw new InjectionException($"Cyclic dependency is found - {requestedType.FullName}");
            }

            //Returns instance of class that registered via ImportConstructorAttribute
            if (_registeredConstructors.ContainsKey(requestedType))
            {
                return CreateInstanceWithCheckByImportConstructorAttribute(requestedType, derivedTypes);
            }

            //Returns instance of class that registered via ImportAttribute

            if (_registeredProperties.ContainsKey(requestedType))
            {
                return CreateInstanceWithCheckByImportAttribute(requestedType, derivedTypes);
            }

            //Returns instance of class that registered via Export attribute or AddType method
            var targetType = _registeredTypes[requestedType];

            var constructors = targetType.GetConstructors()
                .Where(c =>
                    c.GetParameters()
                        .All(t => _registeredTypes.ContainsKey(t.ParameterType)) && c.GetParameters().Any())
                .OrderByDescending(c => c.GetParameters().Length).ToArray();

            if (!constructors.Any()) return Activator.CreateInstance(targetType);

            var constructor = constructors.First();
            derivedTypes.Add(requestedType);
            var parameters = constructor.GetParameters()
                .Select(param => CreateInstanceWithCheck(param.ParameterType, derivedTypes))
                .ToArray();
            return constructor.Invoke(parameters);
        }

        private object CreateInstanceWithCheckByImportConstructorAttribute(Type requestedType, List<Type> derivedTypes)
        {
            var constructor = _registeredConstructors[requestedType];
            if (!constructor.GetParameters()
                    .All(p => _registeredTypes.Keys
                        .Union(_registeredConstructors.Keys)
                        .Union(_registeredProperties.Keys)
                        .Contains(p.ParameterType)) || derivedTypes.Contains(requestedType))
            {
                throw new InjectionException($"In the constructor {constructor.Name} " +
                                            "that registered by the attribute ImportConstructorAttribute " +
                                            "not all parameters are registered in the container");
            }
            derivedTypes.Add(requestedType);
            return constructor.Invoke(constructor.GetParameters()
                .Select(param => CreateInstanceWithCheck(param.ParameterType, derivedTypes))
                .ToArray());
        }

        private object CreateInstanceWithCheckByImportAttribute(Type requestedType, List<Type> derivedTypes)
        {
            var properties = _registeredProperties[requestedType].ToList();

            if (!properties.All(prop => _registeredTypes.Keys
                    .Union(_registeredConstructors.Keys)
                    .Union(_registeredProperties.Keys)
                    .Contains(prop.PropertyType)) || derivedTypes.Contains(requestedType))

            {
                throw new InjectionException($"In the class {requestedType.Name} " +
                                            "that registered by the attribute ImportAttribute " +
                                            "not all necessary properties are registered in the container");
            }
            var instance = Activator.CreateInstance(requestedType);
            derivedTypes.Add(requestedType);
            foreach (var property in properties)
            {
                property.SetValue(instance, CreateInstanceWithCheck(property.PropertyType, derivedTypes));
            }

            return instance;
        }
    }
}
