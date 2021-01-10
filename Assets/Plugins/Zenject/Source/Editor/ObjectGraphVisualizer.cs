using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModestTree;

namespace Zenject
{
    // Responsibilities:
    // - Output a file specifying the full object graph for a given root dependency
    // - This file uses the DOT language with can be fed into GraphViz to generate an image
    // - http://www.graphviz.org/
    public static class ObjectGraphVisualizer
    {
        public static void OutputObjectGraphToFile(
            DiContainer container, string outputPath,
            IEnumerable<Type> externalIgnoreTypes, IEnumerable<Type> contractTypes)
        {
            // Output the entire object graph to file
            Dictionary<Type, List<Type>> graph = CalculateObjectGraph(container, contractTypes);

            List<Type> ignoreTypes = new List<Type>
            {
                typeof(DiContainer),
                typeof(InitializableManager)
            };

            ignoreTypes.AddRange(externalIgnoreTypes);

            string resultStr = "digraph { \n";

            resultStr += "rankdir=LR;\n";

            foreach (KeyValuePair<Type, List<Type>> entry in graph)
            {
                if (ShouldIgnoreType(entry.Key, ignoreTypes))
                {
                    continue;
                }

                foreach (Type dependencyType in entry.Value)
                {
                    if (ShouldIgnoreType(dependencyType, ignoreTypes))
                    {
                        continue;
                    }

                    resultStr += GetFormattedTypeName(entry.Key) + " -> " + GetFormattedTypeName(dependencyType) + "; \n";
                }
            }

            resultStr += " }";

            File.WriteAllText(outputPath, resultStr);
        }

        static bool ShouldIgnoreType(Type type, List<Type> ignoreTypes)
        {
            return ignoreTypes.Contains(type);
        }

        static Dictionary<Type, List<Type>> CalculateObjectGraph(
            DiContainer container, IEnumerable<Type> contracts)
        {
            Dictionary<Type, List<Type>> map = new Dictionary<Type, List<Type>>();

            foreach (Type contractType in contracts)
            {
                List<Type> depends = GetDependencies(container, contractType);

                if (depends.Any())
                {
                    map.Add(contractType, depends);
                }
            }

            return map;
        }

        static List<Type> GetDependencies(
            DiContainer container, Type type)
        {
            List<Type> dependencies = new List<Type>();

            foreach (Type contractType in container.GetDependencyContracts(type))
            {
                List<Type> dependTypes;

                if (contractType.FullName.StartsWith("System.Collections.Generic.List"))
                {
                    Type[] subTypes = contractType.GenericArguments();
                    Assert.IsEqual(subTypes.Length, 1);

                    Type subType = subTypes[0];
                    dependTypes = container.ResolveTypeAll(subType);
                }
                else
                {
                    dependTypes = container.ResolveTypeAll(contractType);
                    Assert.That(dependTypes.Count <= 1);
                }

                foreach (Type dependType in dependTypes)
                {
                    dependencies.Add(dependType);
                }
            }

            return dependencies;
        }

        static string GetFormattedTypeName(Type type)
        {
            string str = type.PrettyName();

            // GraphViz does not read names with <, >, or . characters so replace them
            str = str.Replace(">", "_");
            str = str.Replace("<", "_");
            str = str.Replace(".", "_");

            return str;
        }
    }
}

