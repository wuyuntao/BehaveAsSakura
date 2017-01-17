using BehaveAsSakura.SerializationCompiler.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BehaveAsSakura.SerializationCompiler
{
    static class FlatBuffersSchemaWriter
    {
        public static string ToString(SchemaDef schema)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"namespace {schema.Namespace};");
            builder.AppendLine();

            GenerateEnums(builder, schema);
            GenerateUnions(builder, schema);
            GenerateTables(builder, schema);

            return builder.ToString();
        }

        public static void ToFile(SchemaDef schema, string path)
        {
            File.WriteAllText(path, ToString(schema), Encoding.UTF8);
        }

        private static void GenerateEnums(StringBuilder builder, SchemaDef schema)
        {
            foreach (var e in schema.Enums)
            {
                var typeName = ConvertTypeName(e.Type);
                var underlyingType = Enum.GetUnderlyingType(e.Type);
                var underlyingTypeName = ConvertTypeName(underlyingType);
                builder.AppendLine($"enum {typeName} : {underlyingTypeName} {{");

                var names = Enum.GetNames(e.Type);
                var values = Enum.GetValues(e.Type);
                for (int i = 0; i < names.Length; i++)
                {
                    var name = names.GetValue(i);
                    var value = Convert.ChangeType(values.GetValue(i), underlyingType);
                    builder.AppendLine($"  {name} = {value},");
                }

                builder.AppendLine("}");
                builder.AppendLine();
            }
        }

        private static void GenerateUnions(StringBuilder builder, SchemaDef schema)
        {
            foreach (var e in schema.Unions)
            {
                var typeName = ConvertTypeName(e.UnionType);

                builder.AppendLine($"union {typeName} {{ ");

                for (int i = 0; i < e.IncludedTypes.Count; i++)
                {
                    var includedTypeName = ConvertTypeName(e.IncludedTypes[i].Item1);
                    var includedTypeTag = e.IncludedTypes[i].Item2;
                    builder.AppendLine($"  {includedTypeName} = {includedTypeTag},");
                }

                builder.AppendLine("}");
                builder.AppendLine();

                builder.AppendLine($"table {ConvertUnionWrapper(typeName)} {{ ");
                builder.AppendLine($"  body: {typeName};");
                builder.AppendLine("}");
                builder.AppendLine();
            }
        }

        private static void GenerateTables(StringBuilder builder, SchemaDef schema)
        {
            foreach (var t in schema.Tables)
            {
                var typeName = ConvertTypeName(t.Type);
                builder.AppendLine($"table {typeName} {{");

                foreach (var field in t.Fields)
                {
                    var fieldName = ConvertFieldName(field.Name);
                    var fieldTypeName = ConvertTypeName(field.Type, schema);

                    builder.AppendLine($"  {fieldName}: {fieldTypeName};");
                }

                builder.AppendLine("}");
                builder.AppendLine();
            }
        }

        private static string ConvertFieldName(string name)
        {
            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }

        private static string ConvertIntegerTypeName(Type type)
        {
            switch (type.FullName)
            {
                case "System.Byte":
                    return "ubyte";

                case "System.SByte":
                    return "byte";

                case "System.Int16":
                    return "short";

                case "System.UInt16":
                    return "ushort";

                case "System.Int32":
                    return "int";

                case "System.UInt32":
                    return "uint";

                case "System.Int64":
                    return "long";

                case "System.UInt64":
                    return "ulong";

                default:
                    return null;
            }
        }

        private static string ConvertBuiltInTypeName(Type type)
        {
            var typeName = ConvertIntegerTypeName(type);
            if (typeName != null)
                return typeName;

            switch (type.FullName)
            {
                case "System.Boolean":
                    return "bool";

                case "System.Single":
                    return "float";

                case "System.Double":
                    return "double";

                case "System.String":
                    return "string";

                default:
                    return null;
            }
        }

        private static string ConvertTypeName(Type type, SchemaDef schema = null)
        {
            var typeName = ConvertBuiltInTypeName(type);
            if (typeName != null)
                return typeName;

            if (type.IsArray)
            {
                typeName = ConvertTypeName(type.GetElementType(), schema);
                if (typeName != null)
                {
                    if (schema != null && schema.Unions.Exists(u => u.UnionType == type.GetElementType()))
                        return $"[{ConvertUnionWrapper(typeName)}]";
                    else
                        return $"[{typeName}]";
                }
                else
                    return null;
            }

            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return ConvertBuiltInTypeName(type.GetGenericArguments()[0]);

                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    typeName = ConvertTypeName(type.GetGenericArguments()[0], schema);

                    if (schema != null && schema.Unions.Exists(u => u.UnionType == type.GetElementType()))
                        return $"[{ConvertUnionWrapper(typeName)}]";
                    else
                        return $"[{typeName}]";
                }
            }

            return type.FullName.Replace(".", "__");
        }

        private static string ConvertUnionWrapper(string typeName)
        {
            return $"__UnionWrapper__{typeName}";
        }
    }
}
