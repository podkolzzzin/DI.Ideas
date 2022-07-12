class AutofacLayerRegistrationGenerator : LayerRegistrationGenerator
{
    public AutofacLayerRegistrationGenerator(LayerCodeGenerator layerCodeGenerator, GenerationLifetime lifetime) : base(layerCodeGenerator, lifetime)
    {
    }

    protected override string GetMethod(GenerationLifetime lifetime)
    {
        return lifetime switch
        {
            GenerationLifetime.Transient => "InstancePerDependency()",
            GenerationLifetime.Singleton => "SingleInstance()",
            GenerationLifetime.Scoped => "InstancePerLifetimeScope()",
            _ => base.GetMethod(lifetime)
        };
    }

    protected override string GenerateRegistration((string Service, string Implementation) x)
    {
        return $"container.RegisterType<{x.Service}>().As<{x.Implementation}>().{GetMethod(_lifetime)};";
    }
}

class MSDILayerRegistrationGenerator : LayerRegistrationGenerator
{
    public MSDILayerRegistrationGenerator(LayerCodeGenerator layerCodeGenerator, GenerationLifetime lifetime) : base(layerCodeGenerator, lifetime)
    {
    }

    protected override string GetMethod(GenerationLifetime lifetime)
    {
        if (lifetime != GenerationLifetime.Random)
            return "Add" + lifetime;
        return base.GetMethod(lifetime);
    }

    protected override string GenerateRegistration((string Service, string Implementation) x)
    {
        return $"container.{GetMethod(_lifetime)}<{x.Implementation}, {x.Service}>();";
    }
}