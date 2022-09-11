#if DEBUG_GENERATOR
using System.Diagnostics;
#endif
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
            var ns   = GetNamespace(context.Compilation.SourceModule.GlobalNamespace, "RimionshipServer", "API");
            var type = ns?.GetTypeMembers().FirstOrDefault(t => t.Name == "StatsRequest");
            
            if (type is null)
                return;

            var properties = type.GetMembers().OfType<IPropertySymbol>()
                                 .Select(ps => (DisplayType: ps.Type.ToDisplayString(), ps.Name))
                                 .Where(ps => ps.DisplayType is "float" or "int")
                                 .ToList();
            var sourceBuilder = new StringBuilder(@"// Auto-generated code
using System.Collections.Immutable;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace RimionshipServer.Data
{
    public partial class Stats
    {
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, double>> _StatToUser = new ();
                                                                                                           
        static partial void InitStatToUser() 
        {
");
            foreach ((_, string Name) in properties)
            {
                sourceBuilder.AppendLine($"            _StatToUser.TryAdd(\"{Name}\", new());");
            }
            sourceBuilder.AppendLine(@"        }
");
                
            foreach ((string DisplayType, string Name) in properties)
            {
                sourceBuilder.AppendLine($@"        public {DisplayType} {Name} {{ get; set; }}");
            }

            sourceBuilder.Append(@"
        public Stats() { }
		public Stats(Stats other)
		{");

            foreach ((_, string Name) in properties)
            {
                sourceBuilder.Append($@"
            this.{Name} = other.{Name};");
            }
            sourceBuilder.Append(@"
		}

        public void UpdateFromRequestNoCache(API.StatsRequest stats)
        {
");
            foreach ((_, string Name) in properties)
                sourceBuilder.AppendLine($"            this.{Name} = stats.{Name};");
            sourceBuilder.Append(@"        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        partial void UpdateFromRequestInternal(API.StatsRequest stats)
        {
");
            foreach ((_, string Name) in properties)
                sourceBuilder.Append(@$"            this.{Name} = stats.{Name};
            _StatToUser[""{Name}""][UserId] = (double) stats.{Name};
");
            sourceBuilder.Append(@"        }

        public static ImmutableArray<string> FieldNames { get; } = ImmutableArray.CreateRange(new[] { ");

            sourceBuilder.Append(string.Join(", ", properties.Select(s => string.Concat('"', s.Name, '"'))));
            sourceBuilder.AppendLine(@" });
    }
}");
            context.AddSource("Stats.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        #if DEBUG_GENERATOR
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
        #endif 
        }

        private static INamespaceSymbol GetNamespace(INamespaceSymbol root, params string[] names)
        {
            foreach (var name in names)
                root = root.GetNamespaceMembers().FirstOrDefault(ns => ns.Name == name) ?? throw new ArgumentException($"Cannot find namespace {name} in {root.Name}");

            return root;
        }
    }
}