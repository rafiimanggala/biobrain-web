using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

// TODO: Unity IoC container compatibility shim for MAUI migration.
// This wraps Microsoft.Extensions.DependencyInjection to provide the same
// App.Container.Resolve<T>() API that the Xamarin version used.
// Long-term, refactor all call sites to use constructor injection.

namespace Unity
{
    /// <summary>
    /// Minimal Unity IUnityContainer shim backed by MAUI's IServiceProvider.
    /// </summary>
    public interface IUnityContainer
    {
        T Resolve<T>(params ResolverOverride[] overrides);
        T Resolve<T>(string name, params ResolverOverride[] overrides);
        IUnityContainer RegisterType<TFrom, TTo>() where TTo : TFrom;
        IUnityContainer RegisterInstance<T>(T instance);
    }

    /// <summary>
    /// Base class for resolver overrides (parameter injection).
    /// </summary>
    public abstract class ResolverOverride
    {
        public string Name { get; }
        public object Value { get; }

        protected ResolverOverride(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }

    /// <summary>
    /// Service provider-backed Unity container.
    /// </summary>
    public class ServiceProviderContainer : IUnityContainer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<Type, Type> _interfaceToImpl = new();

        public ServiceProviderContainer(IServiceProvider serviceProvider, IServiceCollection descriptors = null)
        {
            _serviceProvider = serviceProvider;
            if (descriptors != null)
            {
                foreach (var d in descriptors)
                {
                    if (d.ImplementationType != null && d.ServiceType != d.ImplementationType)
                    {
                        _interfaceToImpl[d.ServiceType] = d.ImplementationType;
                    }
                }
            }
        }

        public T Resolve<T>(params ResolverOverride[] overrides)
        {
            if (overrides != null && overrides.Length > 0)
            {
                var args = overrides.Select(o => o.Value).ToArray();
                var targetType = typeof(T);

                if (targetType.IsInterface || targetType.IsAbstract)
                {
                    if (_interfaceToImpl.TryGetValue(targetType, out var implType))
                    {
                        return (T)ActivatorUtilities.CreateInstance(_serviceProvider, implType, args);
                    }
                }

                // Try direct creation first
                try
                {
                    return ActivatorUtilities.CreateInstance<T>(_serviceProvider, args);
                }
                catch (InvalidOperationException)
                {
                    // Parameter didn't match T's constructor — cascade to dependencies.
                    // Find constructor parameters that are interfaces and try to resolve
                    // them with the overrides instead.
                    var ctor = targetType.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
                    if (ctor != null)
                    {
                        var ctorParams = ctor.GetParameters();
                        var resolvedArgs = new object[ctorParams.Length];
                        for (int i = 0; i < ctorParams.Length; i++)
                        {
                            var pType = ctorParams[i].ParameterType;
                            if ((pType.IsInterface || pType.IsAbstract) && _interfaceToImpl.ContainsKey(pType))
                            {
                                // Cascade: resolve this dependency with the parameter overrides
                                var implType = _interfaceToImpl[pType];
                                try
                                {
                                    resolvedArgs[i] = ActivatorUtilities.CreateInstance(_serviceProvider, implType, args);
                                    continue;
                                }
                                catch { /* fallback to normal resolution */ }
                            }
                            resolvedArgs[i] = _serviceProvider.GetService(pType)
                                ?? ActivatorUtilities.CreateInstance(_serviceProvider, pType);
                        }
                        return (T)ctor.Invoke(resolvedArgs);
                    }
                    throw;
                }
            }
            return _serviceProvider.GetService<T>()
                ?? ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        }

        public T Resolve<T>(string name, params ResolverOverride[] overrides)
        {
            return Resolve<T>(overrides);
        }

        public IUnityContainer RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            // No-op: registrations are done in MauiProgram.cs
            return this;
        }

        public IUnityContainer RegisterInstance<T>(T instance)
        {
            // No-op: registrations are done in MauiProgram.cs
            return this;
        }
    }
}

namespace Unity.Resolution
{
    /// <summary>
    /// Unity ParameterOverride shim — captures constructor parameter name/value pairs.
    /// In the MAUI DI world these are mostly ignored; refactor to factory pattern long-term.
    /// </summary>
    public class ParameterOverride : Unity.ResolverOverride
    {
        public ParameterOverride(string parameterName, object parameterValue)
            : base(parameterName, parameterValue)
        {
        }
    }
}
