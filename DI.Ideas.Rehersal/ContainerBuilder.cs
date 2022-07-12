using DI.Ideas.Rehersal;

public abstract class ContainerBuilder : IContainerBuilder
{
    protected readonly List<ServiceDescriptor> _descriptors = new();
    
    public void Register(ServiceDescriptor descriptor)
    {
        _descriptors.Add(descriptor);
    }

    public abstract IContainer Build();
}

public class ReflectionBasedContainerBuilder : ContainerBuilder
{
    public override IContainer Build() => new Container(_descriptors, new ReflectionBasedActivationBuilder());
}

public class LambdaBasedContainerBuilder : ContainerBuilder
{
    public override IContainer Build() => new Container(_descriptors, new LambdaBasedActivationBuilder());
}