using BehaveAsSakura.Attributes;
using BehaveAsSakura.SerializationCompiler.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BehaveAsSakura.SerializationCompiler
{
    static class CSharpSerializerWriter
    {
        public static string ToString(SchemaDef schema, string flatBuffersCodePath)
        {
            var builder = new StringBuilder();

            GenerateFlatbufferObjects(flatBuffersCodePath, builder);
            GenerateUnionSerializers(builder, schema);
            GenerateBaseSerializer(builder, schema);
            GenerateSerializers(builder, schema);
            GenerateBehaviorTreeSerializers(builder, schema);

            return builder.ToString();
        }

        public static void ToFile(SchemaDef schema, string flatBuffersCodePath, string path)
        {
            File.WriteAllText(path, ToString(schema, flatBuffersCodePath), Encoding.UTF8);
        }

        private static void GenerateFlatbufferObjects(string path, StringBuilder builder)
        {
            var code = File.ReadAllText(path, Encoding.UTF8);
            code = Regex.Replace(code, "^public enum", "enum", RegexOptions.Multiline);
            code = Regex.Replace(code, "^public struct", "struct", RegexOptions.Multiline);

            builder.AppendLine(code);
            builder.AppendLine();
        }

        private static void GenerateUnionSerializers(StringBuilder builder, SchemaDef schema)
        {
            builder.AppendLine($@"
namespace {schema.Namespace}
{{
    using FlatBuffers;
    using System;
    using System.Collections.Generic;
");

            foreach (var u in schema.Unions)
            {
                var unionFbName = ConvertTypeName(u.UnionType);

                builder.AppendLine($@"    static class {unionFbName}__UnionSerializer
    {{");

                AddUnionSerializeMethod(builder, u);
                AddUnionWrapperSerializeMethod(builder, u);
                AddUnionWrapperDeserializeMethod(builder, u);

                builder.AppendLine("    }");
                builder.AppendLine();
            }

            builder.AppendLine("}");
            builder.AppendLine();
        }

        private static void AddUnionSerializeMethod(StringBuilder builder, UnionDef union)
        {
            var unionFbName = ConvertTypeName(union.UnionType);
            var unionName = union.UnionType.FullName;

            builder.AppendLine($@"        public static bool Serialize(FlatBufferBuilder fbb, {unionName} obj, out int offset, out {unionFbName} type)
        {{
            if (obj == null)
            {{
                offset = 0;
                type = {unionFbName}.NONE;
                return false;
            }}
");

            foreach (var t in union.IncludedTypes)
            {
                var includedTypeName = t.Item1.FullName;
                var includedTypeSerializerName = ConvertSerializerName(t.Item1);
                var includedFbName = ConvertTypeName(t.Item1);

                builder.AppendLine($@"            if (obj is {includedTypeName})
            {{
                var o = {includedTypeSerializerName}.Instance.Serialize(fbb, ({includedTypeName})obj);
                offset = o.HasValue ? o.Value.Value : 0;
                type = {unionFbName}.{includedFbName};
                return true;
            }}
");
            }

            builder.AppendLine(@"            throw new NotSupportedException(obj.GetType().FullName);
        }");
        }

        private static void AddUnionWrapperSerializeMethod(StringBuilder builder, UnionDef union)
        {
            var unionFbName = ConvertTypeName(union.UnionType);
            var unionName = union.UnionType.FullName;
            var unionWrapperName = ConvertUnionWrapper(union.UnionType);

            builder.AppendLine($@"
        public static Offset<{unionWrapperName}>[] Serialize(FlatBufferBuilder fbb, IList<{unionName}> objects)
        {{
            if (objects == null)
                return null;

            var offsets = new Offset<{unionWrapperName}>[objects.Count];
            for (int i = 0; i < objects.Count; i++)
            {{
                var obj = objects[i];
                if (obj == null)
                {{
                    offsets[i] = {unionWrapperName}.Create{unionWrapperName}(fbb);
                    continue;
                }}
");

            foreach (var t in union.IncludedTypes)
            {
                var includedTypeName = t.Item1.FullName;
                var includedTypeSerializerName = ConvertSerializerName(t.Item1);
                var includedFbName = ConvertTypeName(t.Item1);

                builder.AppendLine($@"                if (obj is {includedTypeName})
                {{
                    var o = {includedTypeSerializerName}.Instance.Serialize(fbb, ({includedTypeName})obj);
                    var offset = o.HasValue ? o.Value.Value : 0;
                    offsets[i] = {unionWrapperName}.Create{unionWrapperName}(fbb,
                                {unionFbName}.{includedFbName},
                                offset);
                    continue;
                }}
");
            }

            builder.AppendLine(@"                throw new NotSupportedException(obj.GetType().FullName);
            }

            return offsets;
        }");
        }

        private static void AddUnionWrapperDeserializeMethod(StringBuilder builder, UnionDef union)
        {
            var unionName = union.UnionType.FullName;
            var unionFbName = ConvertTypeName(union.UnionType);
            var unionWrapperName = ConvertUnionWrapper(union.UnionType);

            builder.AppendLine($@"
        public static {unionName}[] Deserialize(int objectsLength, Func<int, {unionWrapperName}?> getObject)
        {{
            if (objectsLength == 0)
                return null;

            var objects = new {unionName}[objectsLength];
            for (int i = 0; i < objectsLength; i++)
            {{
                var wrapper = getObject(i);
                if (!wrapper.HasValue)
                    continue;

                switch (wrapper.Value.BodyType)
                {{");

            foreach (var t in union.IncludedTypes)
            {
                var includedTypeName = t.Item1.FullName;
                var includedTypeSerializerName = ConvertSerializerName(t.Item1);
                var includedFbName = ConvertTypeName(t.Item1);

                builder.AppendLine($@"                    case {unionFbName}.{includedFbName}:
                        objects[i] = {includedTypeSerializerName}.Instance.Deserialize(wrapper.Value.Body<{includedFbName}>());
                        break;
");
            }

            builder.AppendLine($@"                    case {unionFbName}.NONE:
                        break;

                    default:
                        throw new NotSupportedException(wrapper.Value.BodyType.ToString());
                }}
            }}

            return objects;
        }}");

        }

        private static void GenerateBaseSerializer(StringBuilder builder, SchemaDef schema)
        {
            builder.AppendLine($@"namespace {schema.Namespace}
{{
    using FlatBuffers;
    using System;
    using System.Collections.Generic;

    interface ISerializer
    {{
        byte[] Serialize(object obj);

        object Deserialize(byte[] data);
    }}

    interface ISerializer<TObject, TFlatBufferObject>
        where TFlatBufferObject : struct, IFlatbufferObject
    {{
        Offset<TFlatBufferObject>? Serialize(FlatBufferBuilder fbb, TObject obj);

        Offset<TFlatBufferObject>[] Serialize(FlatBufferBuilder fbb, IList<TObject> objects);

        TObject Deserialize(TFlatBufferObject? obj);

        TObject Deserialize(TFlatBufferObject obj);

        TObject[] Deserialize(int objectsLength, Func<int, TFlatBufferObject?> getObjects);
    }}

    abstract class Serializer<TObject, TFlatBufferObject> : ISerializer<TObject, TFlatBufferObject>, ISerializer
        where TFlatBufferObject : struct, IFlatbufferObject
    {{
        public byte[] Serialize(object obj)
        {{
            var fbb = new FlatBufferBuilder(1024);
            var offset = Serialize(fbb, (TObject)obj);
            if (offset.HasValue)
            {{
                fbb.Finish(offset.Value.Value);

                return fbb.SizedByteArray();
            }}
            else
                return null;
        }}

        public abstract Offset<TFlatBufferObject>? Serialize(FlatBufferBuilder fbb, TObject obj);

        public Offset<TFlatBufferObject>[] Serialize(FlatBufferBuilder fbb, IList<TObject> objects)
        {{
            if (objects == null)
                return null;

            var offsets = new Offset<TFlatBufferObject>[objects.Count];
            for (int i = 0; i < objects.Count; i++)
            {{
                var offset = Serialize(fbb, objects[i]);
                if (offset.HasValue)
                    offsets[i] = offset.Value;
            }}

            return offsets;
        }}

        public static StringOffset[] SerializeString(FlatBufferBuilder fbb, string[] objects)
        {{
            if (objects == null)
                return null;

            var offsets = new StringOffset[objects.Length];
            for (int i = 0; i < objects.Length; i++)
                offsets[i] = fbb.CreateString(objects[i]);

            return offsets;
        }}

        public object Deserialize(byte[] data)
        {{
            if (data == null)
                return default(TObject);

            return Deserialize(GetRootAs(new ByteBuffer(data)));
        }}

        protected abstract TFlatBufferObject GetRootAs(ByteBuffer buffer);

        public TObject Deserialize(TFlatBufferObject? obj)
        {{
            if (!obj.HasValue)
                return default(TObject);

            return Deserialize(obj.Value);
        }}

        public abstract TObject Deserialize(TFlatBufferObject obj);

        public TObject[] Deserialize(int objectsLength, Func<int, TFlatBufferObject?> getObjects)
        {{
            if (objectsLength == 0)
                return null;

            var objects = new TObject[objectsLength];
            for (int i = 0; i < objectsLength; i++)
                objects[i] = Deserialize(getObjects(i));

            return objects;
        }}

        public static T[] DeserializeScalar<T>(int objectsLength, Func<int, T> getObjects)
        {{
            if (objectsLength == 0)
                return null;

            var objects = new T[objectsLength];
            for (int i = 0; i < objectsLength; i++)
                objects[i] = getObjects(i);

            return objects;
        }}
    }}
}}");
        }

        private static void GenerateSerializers(StringBuilder builder, SchemaDef schema)
        {
            builder.AppendLine($@"
namespace {schema.Namespace}
{{
    using FlatBuffers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
");

            foreach (var t in schema.Tables)
            {
                var serializerName = ConvertSerializerName(t.Type);
                var tableName = t.Type.FullName;
                var fbObjectName = ConvertTypeName(t.Type);

                builder.AppendLine($@"
    class {serializerName} : Serializer<{tableName}, {fbObjectName}>
    {{
        public static readonly {serializerName} Instance = new {serializerName}();
");

                AddSerializeMethod(builder, t, schema);
                AddGetRootAddMethod(builder, t);
                AddDeserializeMethod(builder, t, schema);

                builder.AppendLine("    }");
            }

            builder.AppendLine("}");
        }

        private static void AddSerializeMethod(StringBuilder builder, TableDef table, SchemaDef schema)
        {
            var tableName = table.Type.FullName;
            var fbObjectName = ConvertTypeName(table.Type);

            builder.AppendLine($@"        public override Offset<{fbObjectName}>? Serialize(FlatBufferBuilder fbb, {tableName} obj)
        {{
            if (obj == null)
                return null;");

            foreach (var f in table.Fields)
            {
                if (IsCollectionType(f.Type))
                {
                    var elementType = GetElementType(f.Type);
                    var toArray = IsListType(f.Type) ? ".ToArray()" : "";

                    if (elementType == typeof(string))
                    {
                        builder.AppendLine($@"            var vector{f.Name} = default(VectorOffset?);
            if (obj.{f.Name} != null)
            {{
                var offsets{f.Name} = SerializeString(fbb, obj.{f.Name}{toArray});
                vector{f.Name} = {fbObjectName}.Create{f.Name}Vector(fbb, offsets{f.Name});
            }}");
                    }
                    else if (IsScalarType(elementType))
                    {
                        builder.AppendLine($@"            var vector{f.Name} = default(VectorOffset?);
            if (obj.{f.Name} != null)
                vector{f.Name} = {fbObjectName}.Create{f.Name}Vector(fbb, obj.{f.Name}{toArray});");
                    }
                    else if (IsEnumType(elementType))
                    {
                        var underlyingTypeName = Enum.GetUnderlyingType(elementType).FullName;
                        var fbEnumTypeName = ConvertTypeName(elementType);

                        builder.AppendLine($@"            var vector{f.Name} = default(VectorOffset?);
            if (obj.{f.Name} != null)
            {{
                var casted{f.Name} = obj.{f.Name}.Select(e => ({fbEnumTypeName})({underlyingTypeName})e).ToArray();
                vector{f.Name} = {fbObjectName}.Create{f.Name}Vector(fbb, casted{f.Name});
            }}");
                    }
                    else if (IsUnionType(elementType, schema))
                    {
                        var unionSerializerName = $"{ConvertTypeName(elementType)}__UnionSerializer";

                        builder.AppendLine($@"            var vector{f.Name} = default(VectorOffset?);
            if (obj.{f.Name} != null)
            {{
                var offsets{f.Name} = {unionSerializerName}.Serialize(fbb, obj.{f.Name});
                vector{f.Name} = {fbObjectName}.Create{f.Name}Vector(fbb, offsets{f.Name});
            }}");
                    }
                    else
                    {
                        var elementSerializerName = ConvertSerializerName(elementType);

                        builder.AppendLine($@"            var vector{f.Name} = default(VectorOffset?);
            if (obj.{f.Name} != null)
            {{
                var offsets{f.Name} = {elementSerializerName}.Instance.Serialize(fbb, obj.{f.Name});
                vector{f.Name} = {fbObjectName}.Create{f.Name}Vector(fbb, offsets{f.Name});
            }}");
                    }
                }
                else
                {
                    if (f.Type == typeof(string))
                    {
                        builder.AppendLine($@"            var offset{f.Name} = default(StringOffset?);
            if (!string.IsNullOrEmpty(obj.{f.Name}))
                offset{f.Name} = fbb.CreateString(obj.{f.Name});");
                    }
                    else if (IsScalarType(f.Type) || IsEnumType(f.Type))
                    {
                        // Do nothing
                    }
                    else if (IsUnionType(f.Type, schema))
                    {
                        var union = schema.Unions.Find(u => u.UnionType == f.Type);
                        var unionFbName = ConvertTypeName(union.UnionType);

                        builder.AppendLine($@"            int offset{f.Name};
            {unionFbName} type{f.Name};
            {unionFbName}__UnionSerializer.Serialize(fbb, obj.{f.Name}, out offset{f.Name}, out type{f.Name});");
                    }
                    else
                    {
                        var fieldSerializerName = ConvertSerializerName(f.Type);

                        builder.AppendLine($@"            var offset{f.Name} = {fieldSerializerName}.Instance.Serialize(fbb, obj.{ f.Name});");
                    }
                }
            }

            builder.AppendLine($@"            {fbObjectName}.Start{fbObjectName}(fbb);");

            foreach (var f in table.Fields)
            {
                if (IsCollectionType(f.Type))
                {
                    builder.AppendLine($@"            if (vector{f.Name}.HasValue)
                {fbObjectName}.Add{f.Name}(fbb, vector{f.Name}.Value);");
                }
                else
                {
                    if (f.Type == typeof(string))
                    {
                        builder.AppendLine($@"            if (offset{f.Name}.HasValue)
                {fbObjectName}.Add{f.Name}(fbb, offset{f.Name}.Value);");
                    }
                    else if (IsScalarType(f.Type))
                    {
                        builder.AppendLine($"            {fbObjectName}.Add{f.Name}(fbb, obj.{f.Name});");
                    }
                    else if (IsEnumType(f.Type))
                    {
                        var underlyingTypeName = Enum.GetUnderlyingType(f.Type).FullName;
                        var fbEnumTypeName = ConvertTypeName(f.Type);
                        builder.AppendLine($"            {fbObjectName}.Add{f.Name}(fbb, ({fbEnumTypeName})({underlyingTypeName})obj.{f.Name});");
                    }
                    else if (IsUnionType(f.Type, schema))
                    {
                        var union = schema.Unions.Find(u => u.UnionType == f.Type);
                        var unionFbName = ConvertTypeName(union.UnionType);

                        builder.AppendLine($@"            if (type{f.Name} != {unionFbName}.NONE)
            {{
                {fbObjectName}.Add{f.Name}(fbb, offset{f.Name});
                {fbObjectName}.Add{f.Name}Type(fbb, type{f.Name});
            }}");
                    }
                    else
                    {
                        builder.AppendLine($@"            if (offset{f.Name}.HasValue)
                {fbObjectName}.Add{f.Name}(fbb, offset{f.Name}.Value);");
                    }
                }
            }

            builder.AppendLine($@"            return {fbObjectName}.End{fbObjectName}(fbb);");
            builder.AppendLine("        }");
        }

        private static void AddGetRootAddMethod(StringBuilder builder, TableDef table)
        {
            var tableName = table.Type.FullName;
            var fbObjectName = ConvertTypeName(table.Type);

            builder.AppendLine($@"        protected override {fbObjectName} GetRootAs(ByteBuffer buffer)
        {{
            return {fbObjectName}.GetRootAs{fbObjectName}(buffer);
        }}");
        }

        private static void AddDeserializeMethod(StringBuilder builder, TableDef table, SchemaDef schema)
        {
            var tableName = table.Type.FullName;
            var fbObjectName = ConvertTypeName(table.Type);
            var unionFields = new List<Tuple<FieldDef, UnionDef>>();

            builder.AppendLine($@"        public override {tableName} Deserialize({fbObjectName} obj)
        {{
            return new {tableName}()
            {{");

            foreach (var f in table.Fields)
            {
                if (IsCollectionType(f.Type))
                {
                    var elementType = GetElementType(f.Type);

                    if (elementType == typeof(string) || IsScalarType(elementType))
                    {
                        builder.Append($"                {f.Name} = DeserializeScalar<{elementType.FullName}>(obj.{f.Name}Length, obj.{f.Name})");
                    }
                    else if (IsEnumType(elementType))
                    {
                        var toArray = IsArrayType(f.Type) ? ".ToArray()" : "";
                        var enumName = ConvertEnumName(elementType);
                        var underlyingTypeName = Enum.GetUnderlyingType(elementType).FullName;

                        builder.Append($"                {f.Name} = DeserializeScalar(obj.{f.Name}Length, obj.{f.Name}).Select(e => ({enumName})({underlyingTypeName})e){toArray}");
                    }
                    else if (IsUnionType(elementType, schema))
                    {
                        var unionSerializerName = $"{ConvertTypeName(elementType)}__UnionSerializer";

                        builder.Append($"                {f.Name} = {unionSerializerName}.Deserialize(obj.{f.Name}Length, obj.{f.Name})");
                    }
                    else
                    {
                        var elementSerializerName = ConvertSerializerName(elementType);
                        builder.Append($"                {f.Name} = {elementSerializerName}.Instance.Deserialize(obj.{f.Name}Length, obj.{f.Name})");
                    }

                    if (IsListType(f.Type))
                        builder.Append(".ToList()");

                    builder.AppendLine(",");
                }
                else
                {
                    if (f.Type == typeof(string) || IsScalarType(f.Type))
                    {
                        builder.AppendLine($"                {f.Name} = obj.{f.Name},");
                    }
                    else if (IsEnumType(f.Type))
                    {
                        var enumName = ConvertEnumName(f.Type);
                        var underlyingTypeName = Enum.GetUnderlyingType(f.Type).FullName;
                        builder.AppendLine($"                {f.Name} = ({enumName})({underlyingTypeName})obj.{f.Name},");
                    }
                    else if (IsUnionType(f.Type, schema))
                    {
                        var union = schema.Unions.Find(u => u.UnionType == f.Type);
                        unionFields.Add(Tuple.Create(f, union));

                        builder.AppendLine($"                {f.Name} = Deserialize{f.Name}(obj),");
                    }
                    else
                    {
                        var fieldSerializerName = ConvertSerializerName(f.Type);
                        builder.AppendLine($"                {f.Name} = {fieldSerializerName}.Instance.Deserialize(obj.{f.Name}),");
                    }
                }
            }

            builder.AppendLine("            };");
            builder.AppendLine("        }");

            foreach (var f in unionFields)
                AddDeserializeUnionMethod(builder, table, f.Item1, f.Item2);
        }

        private static void AddDeserializeUnionMethod(StringBuilder builder, TableDef table, FieldDef field, UnionDef union)
        {
            var tableFbName = ConvertTypeName(table.Type);
            var unionName = union.UnionType.FullName;
            var unionFbName = ConvertTypeName(union.UnionType);

            builder.AppendLine($@"        private {unionName} Deserialize{field.Name}({tableFbName} obj)
        {{
            switch (obj.{field.Name}Type)
            {{");

            foreach (var t in union.IncludedTypes)
            {
                var includedFbName = ConvertTypeName(t.Item1);
                var includedSerializerName = ConvertSerializerName(t.Item1);

                builder.AppendLine($@"
                case {unionFbName}.{includedFbName}:
                    return {includedSerializerName}.Instance.Deserialize(obj.{field.Name}<{includedFbName}>());");
            }

            builder.AppendLine($@"
                case {unionFbName}.NONE:
                    return null;

                default:
                    throw new NotSupportedException(obj.{field.Name}Type.ToString());
            }}
        }}");
        }

        private static void GenerateBehaviorTreeSerializers(StringBuilder builder, SchemaDef schema)
        {
            var descSerializer = ConvertSerializerName(typeof(BehaviorTreeDesc));
            var propsSerializer = ConvertSerializerName(typeof(BehaviorTreeProps));

            builder.AppendLine($@"
namespace {schema.Namespace}
{{
    public class FlatBuffersSerializer : IBehaviorTreeSerializer
    {{
        public byte[] SerializeDesc(BehaviorTreeDesc desc)
        {{
            return {descSerializer}.Instance.Serialize(desc);
        }}

        public BehaviorTreeDesc DeserializeDesc(byte[] data)
        {{
            return {descSerializer}.Instance.Deserialize(data) as BehaviorTreeDesc;
        }}

        public byte[] SerializeProps(BehaviorTreeProps props)
        {{
            return {propsSerializer}.Instance.Serialize(props);
        }}

        public BehaviorTreeProps DeserializeProps(byte[] data)
        {{
            return {propsSerializer}.Instance.Deserialize(data) as BehaviorTreeProps;
        }}
    }}
}}");
        }

        private static string ConvertEnumName(Type type)
        {
            return type.FullName.Replace("+", "."); ;
        }

        private static string ConvertTypeName(Type type)
        {
            return type.FullName.Replace(".", "__").Replace("+", "___"); ;
        }

        private static string ConvertSerializerName(Type type)
        {
            return $"{ConvertTypeName(type)}__Serializer";
        }

        private static string ConvertUnionWrapper(Type type)
        {
            return $"{ConvertTypeName(type)}__UnionWrapper";
        }

        private static bool IsCollectionType(Type type)
        {
            return IsArrayType(type) || IsListType(type);
        }

        private static bool IsArrayType(Type type)
        {
            return type.IsArray;
        }

        private static bool IsListType(Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>));
        }

        private static Type GetElementType(Type type)
        {
            if (type.IsArray)
                return type.GetElementType();

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                return type.GetGenericArguments()[0];

            return null;
        }

        private static bool IsScalarType(Type type)
        {
            return type == typeof(bool)
                || type == typeof(byte)
                || type == typeof(sbyte)
                || type == typeof(short)
                || type == typeof(ushort)
                || type == typeof(int)
                || type == typeof(uint)
                || type == typeof(long)
                || type == typeof(ulong)
                || type == typeof(float)
                || type == typeof(double);
        }

        private static bool IsEnumType(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(BehaveAsEnumAttribute), false);

            return attrs != null && attrs.Length > 0;
        }

        private static bool IsUnionType(Type type, SchemaDef schema)
        {
            return schema.Unions.Exists(u => u.UnionType == type);
        }
    }
}
