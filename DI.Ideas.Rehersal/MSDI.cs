using Microsoft.Extensions.DependencyInjection;

namespace DI.Ideas.Rehersal.MSDI;


public class ServiceProvider : IServiceProvider
{
    private readonly IScope _scope;

    public ServiceProvider(IContainer container)
    {
        _scope = container.CreateScope();
    }

    public ServiceProvider(IScope scope)
    {
        _scope = scope;
    }
    
    public object? GetService(Type serviceType)
    {
        return _scope.Resolve(serviceType);
    }
}

public class ServiceProviderFactory : IServiceProviderFactory<IContainerBuilder>
{
    public IContainerBuilder CreateBuilder(IServiceCollection services)
    {
        var builder = new LambdaBasedContainerBuilder();
        foreach (var sd in services.Select(ToServiceDescriptor))
            builder.Register(sd);

        return builder;
    }

    private DI.Ideas.Rehersal.ServiceDescriptor ToServiceDescriptor(Microsoft.Extensions.DependencyInjection.ServiceDescriptor serviceDescriptor)
    {
        var lifetime = ToLifetime(serviceDescriptor.Lifetime);
        if (serviceDescriptor.ImplementationType != null)
            return new TypeBasedServiceDescriptor()
            {
                ServiceType = serviceDescriptor.ServiceType, 
                ImplementationType = serviceDescriptor.ImplementationType!,
                Lifetime = lifetime
            };
        else if (serviceDescriptor.ImplementationFactory != null)
        {
            return new FactoryBasedServiceDescriptor()
            {
                ServiceType = serviceDescriptor.ServiceType,
                Factory = s => serviceDescriptor.ImplementationFactory(new ServiceProvider(s))
            };
        }
        else
        {
            return new InstanceBasedServiceDescriptor(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationInstance!);
        }
    }

    private Lifetime ToLifetime(ServiceLifetime l)
    {
        return l == ServiceLifetime.Singleton ? Lifetime.Singleton
            : l == ServiceLifetime.Scoped ? Lifetime.Scoped : Lifetime.Transient;
    }

    public IServiceProvider CreateServiceProvider(IContainerBuilder containerBuilder)
    {
        return new ServiceProvider(containerBuilder.Build());
    }
}