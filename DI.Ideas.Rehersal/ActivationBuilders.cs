using System.Linq.Expressions;
using System.Reflection;

namespace DI.Ideas.Rehersal;

public interface IActivationBuilder
{
    Func<IScope, object> BuildActivation(TypeBasedServiceDescriptor descriptor, Container container);
}

public abstract class BaseActivationBuilder : IActivationBuilder
{
    public Func<IScope, object> BuildActivation(TypeBasedServiceDescriptor descriptor, Container container)
    {
        var ctor = descriptor.ImplementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
            .OrderByDescending(x => x.GetParameters().Length)
            .FirstOrDefault(x => CanActivate(x, container));
        if (ctor == null)
            return s => null;
        var args = ctor.GetParameters();

        return BuildActivationInternal(ctor, args, container, descriptor);
    }

    protected abstract Func<IScope, object> BuildActivationInternal(ConstructorInfo ctor, ParameterInfo[] args,
        Container container, TypeBasedServiceDescriptor descriptor);
    
    private bool CanActivate(ConstructorInfo constructorInfo, Container container)
    {
        return constructorInfo.GetParameters().All(x => container.FindDescriptor(x.ParameterType) != null);
    }
}

public class ReflectionBasedActivationBuilder : BaseActivationBuilder
{
    protected override Func<IScope, object> BuildActivationInternal(ConstructorInfo ctor, ParameterInfo[] args, Container container,
        TypeBasedServiceDescriptor descriptor)
    {
        return s =>
        {
            var argsForCtor = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
                argsForCtor[i] = s.Resolve(args[i].ParameterType);
            return ctor.Invoke(argsForCtor);
        };
    }
}

public class LambdaBasedActivationBuilder : BaseActivationBuilder, IActivationBuilder
{
    private static readonly MethodInfo ResolveMethod = typeof(IScope).GetMethod("Resolve")!;
    
    protected override Func<IScope, object> BuildActivationInternal(ConstructorInfo ctor, ParameterInfo[] args, Container container,
        TypeBasedServiceDescriptor descriptor)
    {
        var scopeParameter = Expression.Parameter(typeof(IScope), "scope");
        var expArgs = args.Select(x => Expression.Convert(Expression.Call(scopeParameter, ResolveMethod, Expression.Constant(x.ParameterType)), x.ParameterType));
        var @new = Expression.New(ctor, expArgs);
        var lambda = Expression.Lambda<Func<IScope, object>>(@new, scopeParameter);
        return lambda.Compile();
    }
}