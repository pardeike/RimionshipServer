using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Rimionshipserver.Analyzers
{
	[Generator]
	public class StatsEntityGenerator : ISourceGenerator
	{
		public void Execute(GeneratorExecutionContext context)
		{
			var sourceBuilder = new StringBuilder(@"
// Auto-generated code
using System.Collections.Immutable;

namespace RimionshipServer.Data
{
    public partial class Stats
    {");

			var ns = GetNamespace(context.Compilation.SourceModule.GlobalNamespace, "RimionshipServer", "API");
			if (ns == null)
				return;

			var type = ns.GetTypeMembers().FirstOrDefault(t => t.Name == "StatsRequest");

			if (type == null)
				return;

			var properties = type.GetMembers().OfType<IPropertySymbol>()
				 .Select(ps => (DisplayType: ps.Type.ToDisplayString(), ps.Name))
				 .Where(ps => ps.DisplayType is "float" or "int")
				 .ToList();

			foreach (var (DisplayType, Name) in properties)
			{
				sourceBuilder.AppendLine($@"
        public {DisplayType} {Name} {{ get; set; }}");
			}

			sourceBuilder.AppendLine(@"
        public void UpdateFromRequest(API.StatsRequest stats)
        {");
			foreach (var (DisplayType, Name) in properties)
				sourceBuilder.AppendLine($"this.{Name} = stats.{Name};");

			sourceBuilder.Append(@"
        }

        public static ImmutableArray<string> FieldNames { get; } = ImmutableArray.CreateRange(new[] { ");

			sourceBuilder.Append(string.Join(", ", properties.Select(s => string.Concat('"', s.Name, '"'))));
			sourceBuilder.AppendLine(@" });
    }
}");
			context.AddSource("Stats.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
		}

		public void Initialize(GeneratorInitializationContext context)
		{
		}

		private static IEnumerable<INamedTypeSymbol> GetTypes(INamespaceSymbol symbol)
		{
			foreach (var type in symbol.GetNamespaceMembers().SelectMany(GetTypes))
				yield return type;

			foreach (var type in symbol.GetTypeMembers())
				yield return type;
		}

		private static INamespaceSymbol GetNamespace(INamespaceSymbol root, params string[] names)
		{
			foreach (var name in names)
				root = root.GetNamespaceMembers().FirstOrDefault(ns => ns.Name == name) ?? throw new ArgumentException($"Cannot find namespace {name} in {root.Name}");

			return root;
		}
	}
}