// See https://aka.ms/new-console-template for more information

using DI.Ideas.Rehersal;
using Microsoft.Extensions.DependencyInjection;
using ServiceDescriptor = Microsoft.Extensions.DependencyInjection.ServiceDescriptor;

var builder = new LambdaBasedContainerBuilder();
 builder.RegisterTransient(typeof(IGenericInterface<>), typeof(GenericService<>));
 var container = builder.Build();
 var scope = container.CreateScope();
 var scope2 = container.CreateScope();
 var ofInt = scope.Resolve<IGenericInterface<int>>();
 var ofDouble = scope.Resolve<IGenericInterface<double>>();
 var ofIntAgain = scope2.Resolve<IGenericInterface<int>>();
 builder.RegisterTransient<IMultiple, M1>();
 builder.RegisterScoped<IMultiple, M2>();
 builder.RegisterSingleton<IMultiple, M3>();
//
// var container = builder.Build();
// var scope1 = container.CreateScope();
// var scope2 = container.CreateScope();
// var multiples = scope1.Resolve<IEnumerable<IMultiple>>();
// var mArr = multiples.ToArray();
// var mArr2 = scope1.Resolve<IEnumerable<IMultiple>>().ToArray();


var dalLayer = new LayerCodeGenerator("repository", 30, ..0, Enumerable.Empty<string>());
var serviceLayer = new LayerCodeGenerator("service", 200, 20..30, dalLayer.GetInterfaces());
var presentationLayer = new LayerCodeGenerator("controller", 5, 99..100, serviceLayer.GetInterfaces());

var generator = new CodeGenerator();
generator.AddLayer(dalLayer, GenerationLifetime.Transient);
generator.AddLayer(serviceLayer, GenerationLifetime.Transient);
generator.AddLayer(presentationLayer, GenerationLifetime.Transient);

File.WriteAllText(@"C:\Users\andriipodkolzin\source\repos\DI.Ideas.Rehersal\DI.Ideas.Rehersal\Samples.SuperClass.cs", generator.Generate("Samples.SuperClass"));


Console.WriteLine("Hello, World!");

interface IMultiple
{
    
}

class M1 : IMultiple
{
    
}

class M2 : IMultiple
{
    
}

class M3 : IMultiple
{
    
}

interface IGenericInterface<T>
{
    
}
class GenericService<T> : IGenericInterface<T> {}
