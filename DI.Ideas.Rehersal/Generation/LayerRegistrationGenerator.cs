class LayerRegistrationGenerator
{
    private readonly LayerCodeGenerator _layerCodeGenerator;
    protected readonly GenerationLifetime _lifetime;

    public LayerRegistrationGenerator(LayerCodeGenerator layerCodeGenerator, GenerationLifetime lifetime)
    {
        _layerCodeGenerator = layerCodeGenerator;
        _lifetime = lifetime;
    }

    public virtual string Generate()
    {
        return string.Join(Environment.NewLine,
            _layerCodeGenerator.GetContracts().Select(GenerateRegistration));
    }

    protected virtual string GenerateRegistration((string Service, string Implementation) x)
    {
        return $"container.{GetMethod(_lifetime)}<{x.Implementation}, {x.Service}>();";
    }

    private static readonly Random _random = new Random();

    protected virtual string GetMethod(GenerationLifetime lifetime)
    {
        return lifetime switch
        {
            GenerationLifetime.Random =>
                GetMethod(new[] { GenerationLifetime.Scoped, GenerationLifetime.Singleton, GenerationLifetime.Transient }[
                    _random.Next(0, 3)]),
            GenerationLifetime.Transient => "RegisterTransient",
            _ => "Register" + lifetime
        };
    }
}