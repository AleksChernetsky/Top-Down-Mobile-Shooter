using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefence.Core
{
    public sealed class ServiceLocator : IServiceLocator
    {
        private readonly Dictionary<Type, IService> _services = new Dictionary<Type, IService>();
        private readonly Dictionary<Type, Func<IService>> _factories = new Dictionary<Type, Func<IService>>();

        public static IServiceLocator Instance { get; private set; }

        public ServiceLocator()
        {
            Instance = this;
        }

        public void Init()
        {
            // ServiceLocator itself doesn't need initialization
        }

        public void RegisterLazy<TInterface, TImplementation>()
            where TInterface : class, IService
            where TImplementation : class, TInterface, new()
        {
            var interfaceType = typeof(TInterface);
            _factories[interfaceType] = () =>
            {
                var service = new TImplementation();
                service.Init();
                return service;
            };
        }

        public void RegisterLazy<TInterface>(Func<TInterface> factory)
            where TInterface : class, IService
        {
            var interfaceType = typeof(TInterface);
            _factories[interfaceType] = () =>
            {
                var service = factory();
                service.Init();
                return service;
            };
        }

        public void Register<T>(T service) where T : class, IService
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            // Auto-register for all implemented interfaces (excluding IService itself)
            var serviceType = service.GetType();
            var interfaces = serviceType.GetInterfaces();

            foreach (var interfaceType in interfaces)
            {
                if (interfaceType == typeof(IService))
                {
                    continue;
                }

                if (typeof(IService).IsAssignableFrom(interfaceType))
                {
                    if (!_services.TryAdd(interfaceType, service))
                    {
                        Debug.LogWarning($"Service {interfaceType.Name} is already registered.");
                    }
                }
            }

            // Also register as concrete type if needed
            var concreteType = typeof(T);
            if (concreteType.IsClass && !concreteType.IsAbstract)
            {
                _services.TryAdd(concreteType, service);
            }
        }

        public void Register(Type serviceType, IService service)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (!_services.TryAdd(serviceType, service))
            {
                throw new InvalidOperationException($"Service of type {serviceType.Name} is already registered.");
            }
        }

        public T Resolve<T>() where T : class, IService
        {
            var type = typeof(T);

            if (_services.TryGetValue(type, out var service))
            {
                return service as T;
            }

            if (!_factories.TryGetValue(type, out var factory))
            {
                throw new InvalidOperationException($"Service of type {type.Name} is not registered.");
            }

            var newService = factory() as T;
            _services[type] = newService;
            return newService;
        }

        public IService Resolve(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (_services.TryGetValue(serviceType, out var service))
            {
                return service;
            }

            if (!_factories.TryGetValue(serviceType, out var factory))
            {
                throw new InvalidOperationException($"Service of type {serviceType.Name} is not registered.");
            }

            var newService = factory();
            _services[serviceType] = newService;
            return newService;
        }

        public bool TryResolve<T>(out T service) where T : class, IService
        {
            try
            {
                service = Resolve<T>();
                return service != null;
            }
            catch
            {
                service = null;
                return false;
            }
        }

        public void Unregister<T>() where T : class, IService
        {
            var type = typeof(T);
            _services.Remove(type);
            _factories.Remove(type);
        }

        public void Clear()
        {
            _services.Clear();
            _factories.Clear();
            Instance = null;
        }

        public IEnumerable<Type> GetRegisteredTypes()
        {
            return _services.Keys.Concat(_factories.Keys).Distinct();
        }
    }
}
