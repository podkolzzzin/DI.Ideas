using System.Text;

enum GenerationLifetime
{
    Transient,
    Scoped,
    Singleton,
    Random
}

class CodeGenerator
{
    private readonly List<(LayerCodeGenerator Generator, GenerationLifetime Lifetime)> _layers = new ();
    
    public CodeGenerator()
    {
    }

    public void AddLayer(LayerCodeGenerator generator, GenerationLifetime lifetime)
    {
        _layers.Add((generator, lifetime));
    }

    private string GenerateContainerRegistration()
    {
        return @"static class Registration
{
    public static void Register(IContainerBuilder container)
    {
" + string.Join(Environment.NewLine,
            _layers.Select(x => new LayerRegistrationGenerator(x.Generator, x.Lifetime).Generate())) + @"
    }                           
}";
    }

    private string GenerateAutofacRegistration()
    {
        return @"static class AutofacRegistration
{
    public static void Register(Autofac.ContainerBuilder container)
    {
" + string.Join(Environment.NewLine,
            _layers.Select(x => new AutofacLayerRegistrationGenerator(x.Generator, x.Lifetime).Generate())) + @"
    }                           
}";
    }

    private string GenerateMSDIRegistration()
    {
        return @"static class MSDIRegistration
{
    public static void Register(ServiceCollection container)
    {
" + string.Join(Environment.NewLine,
            _layers.Select(x => new MSDILayerRegistrationGenerator(x.Generator, x.Lifetime).Generate())) + @"
    }                           
}";
    }
    
    public string Generate(string @namespace)
    {
        var builder = new StringBuilder();
        builder
            .AppendLine("using Autofac;")
            .AppendLine("using DI.Ideas.Rehersal;")
            .AppendLine("using Microsoft.Extensions.DependencyInjection;")
            .AppendLine()
            .AppendLine($"namespace {@namespace};");

        foreach (var layer in _layers)
        {
            builder.AppendLine(layer.Generator.Generate());
        }

        builder.AppendLine();
        builder.AppendLine(GenerateContainerRegistration());
        builder.AppendLine(GenerateAutofacRegistration());
        builder.AppendLine(GenerateMSDIRegistration());
        return builder.ToString();
    }
}