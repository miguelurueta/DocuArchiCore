namespace DocuArchiCore.Tools.DtoJsDocGenerator
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public static class DtoJsDocGenerator
    {
        public static void Generate(string outputPath)
        {
            // Carga segura del ensamblado principal
            var assemblyPath = Path.Combine(AppContext.BaseDirectory, "DocuArchiCore.dll");

            if (!File.Exists(assemblyPath))
                throw new Exception($"❌ No se encontró el assembly en: {assemblyPath}");

            var assembly = Assembly.LoadFrom(assemblyPath);

            // Filtrar DTOs
            var types = assembly.GetTypes()
                .Where(t => t.Name.EndsWith("Dto", StringComparison.OrdinalIgnoreCase))
                .OrderBy(t => t.Name)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("// AUTO-GENERATED FILE — DO NOT EDIT");
            sb.AppendLine("// DTO JSDoc definitions for JavaScript\n");

            foreach (var type in types)
            {
                sb.AppendLine(GenerateJsDocForType(type));
                sb.AppendLine();
            }

            File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
        }

        private static string GenerateJsDocForType(Type type)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"/**");
            sb.AppendLine($" * @typedef {{object}} {type.Name}");
            sb.AppendLine($" * @description Auto-generated DTO from C# class {type.Name}");

            foreach (var prop in type.GetProperties())
            {
                string jsType = ConvertToJsType(prop.PropertyType);
                string camelName = Char.ToLowerInvariant(prop.Name[0]) + prop.Name[1..];

                sb.AppendLine($" * @property {{{jsType}}} {camelName}");
            }

            sb.AppendLine($" */");

            return sb.ToString();
        }

        private static string ConvertToJsType(Type type)
        {
            bool isNullable = Nullable.GetUnderlyingType(type) != null;
            if (isNullable) type = Nullable.GetUnderlyingType(type);

            if (type == typeof(int) || type == typeof(long) ||
                type == typeof(float) || type == typeof(double) ||
                type == typeof(decimal))
                return "number";

            if (type == typeof(string))
                return "string";

            if (type == typeof(bool))
                return "boolean";

            if (type == typeof(DateTime))
                return "string";

            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type) &&
                type != typeof(string))
                return "Array<any>";

            if (type.IsClass)
                return "object";

            return "any";
        }
    }
}
