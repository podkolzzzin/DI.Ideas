using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using DI.Ideas.Rehersal;

public class Container : IContainer
{
    private class Scope : IScope
    {
        private readonly Container _container;
        private readonly ConcurrentDictionary<ServiceDescriptor, object> _scopedInstances = new();
        private readonly ConcurrentStack<object> _disposables = new();

        public Scope(Container container)
        {
            _container = container;
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                if (disposable is IAsyncDisposable ad)
                    ad.DisposeAsync().GetAwaiter().GetResult();
                else
                {
                    ((IDisposable)disposable).Dispose();
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var disposable in _disposables)
            {
                if (disposable is IAsyncDisposable ad)
                    await ad.DisposeAsync();
                else
                {
                    ((IDisposable)disposable).Dispose();
                }
            }
        }

        public object Resolve(Type service)
        {
            var descriptor = _container.FindDescriptor(service);
            if (descriptor == null)
                throw new InvalidOperationException($"Unable to find registration for {service}");

            return ResolveInternal(descriptor);
        }

        internal object ResolveInternal(ServiceDescriptor descriptor)
        {
            if (descriptor.Lifetime == Lifetime.Transient)
                return _container.CreateInstance(this, descriptor);
            else if (descriptor.Lifetime == Lifetime.Scoped || this == _container._root)
                return _scopedInstances.GetOrAdd(descriptor, t => _container.CreateInstance(this, descriptor));
            else
                return _container._root.ResolveInternal(descriptor);
        }
    }

    private readonly ConcurrentDictionary<ServiceDescriptor, Func<IScope, object>> _activators = new();
    private readonly ConcurrentDictionary<Type, ServiceDescriptor> _descriptors = new();
    private readonly IActivationBuilder _activationBuilder;
    private readonly Scope _root;

    public Container(IEnumerable<ServiceDescriptor> descriptors, IActivationBuilder activationBuilder)
    {
        _activationBuilder = activationBuilder;
        _root = new Scope(this);
        var d = (IDictionary<Type, ServiceDescriptor>)_descriptors;
        foreach (var sd in descriptors.GroupBy(x => x.ServiceType))
        {
            var items = sd.ToArray();
            if (items.Length == 1)
                d.Add(sd.Key, items[0]);
            else
            {
                var multiple = new MultipleServicesDescriptor()
                    { Descriptors = items, Lifetime = (Lifetime)int.MaxValue, ServiceType = sd.Key };
                d.Add(sd.Key, multiple);
                var ie = typeof(IEnumerable<>).MakeGenericType(sd.Key);
                d.Add(ie, BuildUsingMultipleDescriptor(ie, multiple));
            }
        }
    }

    private ServiceDescriptor BuildUsingMultipleDescriptor(Type serviceType, ServiceDescriptor descriptor)
    {
        return new FactoryBasedServiceDescriptor()
        {
            Lifetime = Lifetime.Transient,
            ServiceType = serviceType,
            Factory = s =>
            {
                var items = (descriptor as MultipleServicesDescriptor)?.Descriptors ?? new[] { descriptor };
                var scope = (Scope)s;
                var arr = Array.CreateInstance(descriptor.ServiceType, items.Length);
                for (int i = 0; i < arr.Length; i++)
                    arr.SetValue(scope.ResolveInternal(items[i]), i);
                return arr;
            }
        };
    }

    internal ServiceDescriptor? FindDescriptor(Type service)
    {
        if (_descriptors.TryGetValue(service, out var descriptor))
            return descriptor;
        if (service.IsAssignableTo(typeof(IEnumerable)) && service.IsGenericType &&
            service.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            var items = FindDescriptor(service.GetGenericArguments().Single());
            if (items != null)
            {
                return _descriptors.GetOrAdd(service, BuildUsingMultipleDescriptor(service, items));
            }
            else
            {
                return null;
            }
        }

        if (service.IsConstructedGenericType)
        {
            var g = service.GetGenericTypeDefinition();
            var genericDescriptor = FindDescriptor(g);
            if (genericDescriptor == null)
                return null;
            if (genericDescriptor is TypeBasedServiceDescriptor tb)
            {
                var t = tb.ImplementationType.MakeGenericType(service.GetGenericArguments());
                return _descriptors.GetOrAdd(t, new TypeBasedServiceDescriptor()
                {
                    Lifetime = tb.Lifetime,
                    ImplementationType = t,
                    ServiceType = service
                });
            }

            return null;
        }

        return null;
    }

    private object CreateInstance(IScope scope, ServiceDescriptor descriptor)
    {
        return _activators.GetOrAdd(descriptor, _ => BuildActivation(descriptor, _activationBuilder))(scope);
    }

    private Func<IScope, object> BuildActivation(ServiceDescriptor serviceDescriptor,
        IActivationBuilder activationBuilder)
    {
        if (serviceDescriptor is FactoryBasedServiceDescriptor fb)
            return fb.Factory;
        if (serviceDescriptor is InstanceBasedServiceDescriptor ib)
            return _ => ib.Instance;

        var tb = (TypeBasedServiceDescriptor)serviceDescriptor;
        return activationBuilder.BuildActivation(tb, this);
    }

    public IScope CreateScope() => new Scope(this);
}