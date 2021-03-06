// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Design.Utilities;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Migrations.Design
{
    public class CSharpModelCodeGenerator : ModelCodeGenerator
    {
        private const string TableNameAnnotationName = "TableName";
        private const string SchemaAnnotationName = "Schema";
        private const string ColumnNameAnnotationName = "ColumnName";
        private const string KeyNameAnnotationName = "KeyName";

        public override void GenerateModelSnapshotClass(
            string @namespace,
            string className,
            IModel model,
            IndentedStringBuilder stringBuilder)
        {
            Check.NotEmpty(className, "className");
            Check.NotEmpty(@namespace, "namespace");
            Check.NotNull(model, "model");
            Check.NotNull(stringBuilder, "stringBuilder");

            foreach (var ns in GetNamespaces(model).OrderBy(n => n).Distinct())
            {
                stringBuilder
                    .Append("using ")
                    .Append(ns)
                    .AppendLine(";");
            }

            stringBuilder
                .AppendLine()
                .Append("namespace ")
                .AppendLine(@namespace)
                .AppendLine("{");

            using (stringBuilder.Indent())
            {
                stringBuilder
                    .Append("public class ")
                    .Append(className)
                    .AppendLine(" : ModelSnapshot")
                    .AppendLine("{");

                using (stringBuilder.Indent())
                {
                    stringBuilder
                        .AppendLine("public override IModel Model")
                        .AppendLine("{");

                    using (stringBuilder.Indent())
                    {
                        stringBuilder
                            .AppendLine("get")
                            .AppendLine("{");

                        using (stringBuilder.Indent())
                        {
                            Generate(model, stringBuilder);
                        }

                        stringBuilder
                            .AppendLine()
                            .AppendLine("}");
                    }

                    stringBuilder.AppendLine("}");
                }

                stringBuilder.AppendLine("}");
            }

            stringBuilder.Append("}");
        }

        public override void Generate(IModel model, IndentedStringBuilder stringBuilder)
        {
            stringBuilder.Append("var builder = new ModelBuilder()");

            using (stringBuilder.Indent())
            {
                GenerateAnnotations(model.Annotations.ToArray(), stringBuilder);
            }

            stringBuilder.Append(";");

            GenerateEntityTypes(model.EntityTypes, stringBuilder);

            stringBuilder.AppendLine().Append("return builder.Model;");
        }

        protected virtual void GenerateEntityTypes(
            IReadOnlyList<IEntityType> entityTypes, IndentedStringBuilder stringBuilder)
        {
            if (!entityTypes.Any())
            {
                return;
            }

            foreach (var entityType in entityTypes)
            {
                stringBuilder.AppendLine().Append("builder");

                GenerateEntityType(entityType, stringBuilder);

                stringBuilder.Append(";");
                stringBuilder.AppendLine();
            }

            foreach (var entityType in entityTypes.Where(entityType => entityType.ForeignKeys.Count > 0))
            {
                var foreignKeys = entityType.ForeignKeys;

                if (foreignKeys.Any())
                {
                    stringBuilder.AppendLine().Append("builder");

                    GenerateSimpleEntityType(entityType, stringBuilder);

                    using (stringBuilder.Indent())
                    {
                        GenerateForeignKeys(foreignKeys, stringBuilder);
                    }

                    stringBuilder.Append(";");
                    stringBuilder.AppendLine();
                }
            }
        }

        protected virtual void GenerateEntityType(
            [NotNull] IEntityType entityType, [NotNull] IndentedStringBuilder stringBuilder)
        {
            GenerateSimpleEntityType(entityType, stringBuilder);

            using (stringBuilder.Indent())
            {
                GenerateProperties(entityType.Properties, stringBuilder);

                GenerateKey(entityType.GetKey(), stringBuilder);

                GenerateEntityTypeAnnotations(entityType, stringBuilder);
            }
        }

        protected virtual void GenerateProperties(
            [NotNull] IReadOnlyList<IProperty> properties, [NotNull] IndentedStringBuilder stringBuilder)
        {
            if (!properties.Any())
            {
                return;
            }

            stringBuilder.AppendLine().Append(".Properties(");

            if (properties.Count == 1)
            {
                stringBuilder.Append("ps => ");

                GenerateProperty(properties[0], stringBuilder);

                stringBuilder.Append(")");

                return;
            }

            using (stringBuilder.AppendLine().Indent())
            {
                stringBuilder.AppendLine("ps =>");

                using (stringBuilder.Indent())
                {
                    stringBuilder.AppendLine("{");

                    using (stringBuilder.Indent())
                    {
                        foreach (var property in properties)
                        {
                            GenerateProperty(property, stringBuilder);

                            stringBuilder.AppendLine(";");
                        }
                    }

                    stringBuilder.Append("}");
                }
            }

            stringBuilder.Append(")");
        }

        protected virtual void GenerateProperty(
            [NotNull] IProperty property, [NotNull] IndentedStringBuilder stringBuilder)
        {
            stringBuilder
                .Append("ps.Property<")
                .Append(property.PropertyType.GetTypeName())
                .Append(">(")
                .Append(DelimitString(property.Name));

            if (!property.IsClrProperty)
            {
                stringBuilder.Append(", shadowProperty: true");
            }

            if (property.IsConcurrencyToken)
            {
                stringBuilder.Append(", concurrencyToken: true");
            }

            stringBuilder.Append(")");

            GeneratePropertyAnnotations(property, stringBuilder);
        }

        protected virtual void GeneratePropertyAnnotations([NotNull] IProperty property, [NotNull] IndentedStringBuilder stringBuilder)
        {
            var columnName = property[ColumnNameAnnotationName];
            if (columnName != null)
            {
                stringBuilder
                    .Append(".ColumnName(")
                    .Append(DelimitString(columnName))
                    .Append(")");
            }

            var scaffoldableAnnotations = property.Annotations.Where(a => a.Name != ColumnNameAnnotationName).ToArray();
            using (stringBuilder.Indent())
            {
                GenerateAnnotations(scaffoldableAnnotations, stringBuilder);
            }
        }

        protected virtual void GenerateKey(
            [CanBeNull] IKey key, [NotNull] IndentedStringBuilder stringBuilder)
        {
            if (key == null)
            {
                return;
            }

            if (key.Annotations.Any())
            {
                stringBuilder
                    .AppendLine()
                    .Append(".Key(k => k.Properties(")
                    .Append(key.Properties.Select(p => DelimitString(p.Name)).Join())
                    .Append(")");

                var keyName = key[KeyNameAnnotationName];
                var scaffoldableAnnotations = key.Annotations.Where(a => a.Name != KeyNameAnnotationName);
                if (scaffoldableAnnotations.Any())
                {
                    using (stringBuilder.Indent())
                    {
                        GenerateAnnotations(scaffoldableAnnotations.ToArray(), stringBuilder);
                    }
                }

                if (keyName != null)
                {
                    using (stringBuilder.Indent())
                    {
                        stringBuilder
                            .AppendLine()
                            .Append(".KeyName(")
                            .Append(DelimitString(keyName))
                            .Append(")");
                    }
                }

                stringBuilder.Append(")");
            }
            else
            {
                stringBuilder
                    .AppendLine()
                    .Append(".Key(")
                    .Append(key.Properties.Select(p => DelimitString(p.Name)).Join())
                    .Append(")");
            }
        }

        protected virtual void GenerateEntityTypeAnnotations([NotNull] IEntityType entityType, [NotNull] IndentedStringBuilder stringBuilder)
        {
            var tableName = entityType[TableNameAnnotationName];
            var schema = entityType[SchemaAnnotationName];
            if (!string.IsNullOrEmpty(tableName))
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(".TableName(");
                stringBuilder.Append(DelimitString(tableName));
                if (!string.IsNullOrEmpty(schema))
                {
                    stringBuilder.Append(", ");
                    stringBuilder.Append(DelimitString(schema));
                }

                stringBuilder.Append(")");
            }

            var scaffoldableAnnotations = entityType.Annotations.Where(a => a.Name != TableNameAnnotationName && a.Name != SchemaAnnotationName).ToArray();
            GenerateAnnotations(scaffoldableAnnotations, stringBuilder);
        }

        protected virtual void GenerateForeignKeys(
            [NotNull] IReadOnlyList<IForeignKey> foreignKeys, [NotNull] IndentedStringBuilder stringBuilder)
        {
            if (!foreignKeys.Any())
            {
                return;
            }

            stringBuilder.AppendLine().Append(".ForeignKeys(");

            if (foreignKeys.Count == 1)
            {
                stringBuilder.Append("fks => ");

                GenerateForeignKey(foreignKeys[0], stringBuilder);

                stringBuilder.Append(")");

                return;
            }

            using (stringBuilder.AppendLine().Indent())
            {
                stringBuilder.AppendLine("fks =>");

                using (stringBuilder.Indent())
                {
                    stringBuilder.AppendLine("{");

                    using (stringBuilder.Indent())
                    {
                        foreach (var foreignKey in foreignKeys)
                        {
                            GenerateForeignKey(foreignKey, stringBuilder);

                            stringBuilder.AppendLine(";");
                        }
                    }

                    stringBuilder.Append("}");
                }
            }

            stringBuilder.Append(")");
        }

        protected virtual void GenerateForeignKey(
            [NotNull] IForeignKey foreignKey, [NotNull] IndentedStringBuilder stringBuilder)
        {
            stringBuilder
                .Append("fks.ForeignKey(")
                .Append(DelimitString(foreignKey.ReferencedEntityType.Name))
                .Append(", ")
                .Append(foreignKey.Properties.Select(p => DelimitString(p.Name)).Join())
                .Append(")");

            GenerateForeignKeyAnnotations(foreignKey, stringBuilder);
        }

        protected virtual void GenerateForeignKeyAnnotations([NotNull] IForeignKey foreignKey, [NotNull] IndentedStringBuilder stringBuilder)
        {
            var foreignKeyName = foreignKey[KeyNameAnnotationName];
            if (foreignKeyName != null)
            {
                using (stringBuilder.Indent())
                {
                    stringBuilder
                        .AppendLine()
                        .Append(".KeyName(")
                        .Append(DelimitString(foreignKeyName))
                        .Append(")");
                }
            }

            var scaffoldableAnnotations = foreignKey.Annotations.Where(a => a.Name != KeyNameAnnotationName).ToArray();
            using (stringBuilder.Indent())
            {
                GenerateAnnotations(scaffoldableAnnotations, stringBuilder);
            }
        }

        protected virtual void GenerateAnnotations(
            [NotNull] IReadOnlyList<IAnnotation> annotations, [NotNull] IndentedStringBuilder stringBuilder)
        {
            if (!annotations.Any())
            {
                return;
            }

            foreach (var annotation in annotations)
            {
                stringBuilder.AppendLine();

                GenerateAnnotation(annotation, stringBuilder);
            }
        }

        protected virtual void GenerateAnnotation(
            [NotNull] IAnnotation annotation, [NotNull] IndentedStringBuilder stringBuilder)
        {
            stringBuilder
                .Append(".Annotation(")
                .Append(DelimitString(annotation.Name))
                .Append(", ")
                .Append(DelimitString(annotation.Value))
                .Append(")");
        }

        protected virtual void GenerateSimpleEntityType(
            [NotNull] IEntityType entityType, [NotNull] IndentedStringBuilder stringBuilder)
        {
            stringBuilder
                .Append(".Entity(")
                .Append(DelimitString(entityType.Name))
                .Append(")");
        }

        protected virtual string DelimitString([NotNull] string value)
        {
            Check.NotNull(value, "value");

            return "\"" + EscapeString(value) + "\"";
        }

        protected virtual string EscapeString([NotNull] string value)
        {
            Check.NotEmpty(value, "value");

            return value.Replace("\"", "\\\"");
        }
    }
}
