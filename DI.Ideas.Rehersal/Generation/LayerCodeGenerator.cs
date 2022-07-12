class LayerCodeGenerator
{
    private static readonly string[] _vocabulary = new[]
    {
        "area",
        "book",
        "business",
        "case",
        "child",
        "company",
        "country",
        "day",
        "eye",
        "fact",
        "family",
        "government",
        "group",
        "hand",
        "home",
        "job",
        "life",
        "lot",
        "man",
        "money",
        "month",
        "mother",
        "Mr",
        "night",
        "number",
        "part",
        "people",
        "place",
        "point",
        "problem",
        "program",
        "question",
        "right",
        "room",
        "school",
        "state",
        "story",
        "student",
        "study",
        "system",
        "thing",
        "time",
        "water",
        "way",
        "week",
        "woman",
        "word",
        "work",
        "world",
        "year",
    };

    private readonly string _name;
    private readonly int _count;
    private readonly Range _depCount;
    private readonly string[] _possibleDependencies;

    public LayerCodeGenerator(string name, int count, Range depCount, IEnumerable<string> possibleDependencies)
    {
        _name = name;
        _count = count;
        _depCount = depCount;
        _possibleDependencies = possibleDependencies.ToArray();
    }

    public IEnumerable<(string Service, string Implementation)> GetContracts()
        => GetClasses().Select(x => (x, GetInterface(x)));

    public IEnumerable<string> GetInterfaces() => GetClasses().Select(GetInterface); 

    private IEnumerable<string> GetClasses()
    {
        int v = 0;
        string postfix = "";
        for (int i = 0; i < _count; i++)
        {
            var name = char.ToUpper(_vocabulary[v][0]) + _vocabulary[v][1..] + char.ToUpper(_name[0]) + _name[1..] + postfix;
            yield return name;
            v++;
            if (v == _vocabulary.Length)
            {
                postfix += "Ex";
                v = 0;
            }
        }
    }

    private string GetInterface(string className) => $"I{className}";

    public string Generate()
    {
        var r = new Random();
        var classNames = GetClasses().ToArray();
        var classes = string.Join(Environment.NewLine, classNames.Select(x => GenerateClass(x, r)));
        var interfaces = string.Join(Environment.NewLine, classNames.Select(GenerateInterface));

        return interfaces + Environment.NewLine + Environment.NewLine + classes;
    }

    private string GenerateInterface(string className)
    {
        var intName = GetInterface(className);
        return $"public interface {intName} {{}}";
    }

    private string GenerateClass(string className, Random r)
    {
        string ToParameterName(string name)
        {
            if (name.StartsWith("I") && char.IsUpper(name[1]))
            {
                name = name[1..];
            }

            return char.ToLower(name[0]) + name[1..];
        }

        string ToFieldName(string name) => $"_{ToParameterName(name)}";
        
        var deps = _possibleDependencies.OrderBy(x => r.Next())
            .Take(r.Next(_depCount.Start.Value, _depCount.End.Value))
            .ToArray();

        const string template = @"
public class {0} : {4}
{{
{2}

  public {0}({1})
  {{
{3}
  }}
}}";
        var paramNames = string.Join(", ", deps.Select(x => $"{x} {ToParameterName(x)}"));
        var fields = string.Join(Environment.NewLine, deps.Select(x => $"  private readonly {x} {ToFieldName(x)};"));
        var assigns = string.Join(Environment.NewLine,
            deps.Select(x => $"    {ToFieldName(x)} = {ToParameterName(x)};"));
        return string.Format(template, className, paramNames, fields, assigns, GetInterface(className));
    }
}