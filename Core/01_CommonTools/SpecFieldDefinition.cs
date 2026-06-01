using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using UnityEditor;
#endif

namespace PahlUnity
{
    [CreateAssetMenu(fileName = "SpecFieldDefinition", menuName = "PahlUnity/Spec Field Definition")]
    public class SpecFieldDefinition : ScriptableObject
    {
        [SerializeField] private List<string> _Fields = new List<string>();

        public IReadOnlyList<string> Fields => _Fields;

#if UNITY_EDITOR
        [Button("Generate Spec Fields")]
        void GenerateSpecFields()
        {
            string assetPath = AssetDatabase.GetAssetPath(this);
            string directory = Path.GetDirectoryName(assetPath);
            GenerateSpecFieldsFile(directory);
        }

        static void GenerateSpecFieldsFile(string directory)
        {
            string className = "SpecFields";
            string outputPath = Path.Combine(directory, $"{className}.cs");

            List<string> fields = CollectFields();
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"public static class {className}");
            builder.AppendLine("{");

            HashSet<string> identifierSet = new HashSet<string>();

            for (int i = 0; i < fields.Count; i++)
            {
                string field = fields[i];
                string identifier = GetUniqueIdentifier(ToIdentifier(field), identifierSet);
                string escapedField = field.Replace("\\", "\\\\").Replace("\"", "\\\"");

                builder.AppendLine($"   public static readonly int {identifier} = PahlUnity.StableHash.ToInt32(\"{escapedField}\");");
            }

            builder.AppendLine("}");

            File.WriteAllText(outputPath, builder.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();
        }

        static List<string> CollectFields()
        {
            string typeName = nameof(SpecFieldDefinition);
            string[] guids = AssetDatabase.FindAssets($"t:{typeName}", new[] { "Assets" });
            HashSet<string> fieldSet = new HashSet<string>();
            List<string> fields = new List<string>();

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                SpecFieldDefinition definition = AssetDatabase.LoadAssetAtPath<SpecFieldDefinition>(assetPath);

                if (definition == null)
                {
                    continue;
                }

                IReadOnlyList<string> definitionFields = definition.Fields;

                for (int fieldIndex = 0; fieldIndex < definitionFields.Count; fieldIndex++)
                {
                    string field = definitionFields[fieldIndex];

                    if (string.IsNullOrWhiteSpace(field))
                    {
                        continue;
                    }

                    string fieldName = field.Trim();

                    if (fieldSet.Add(fieldName))
                    {
                        fields.Add(fieldName);
                    }
                }
            }

            fields.Sort(StringComparer.Ordinal);
            return fields;
        }

        static string ToIdentifier(string value)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];

                if (char.IsLetterOrDigit(c) || c == '_')
                {
                    builder.Append(c);
                }
            }

            if (builder.Length == 0)
            {
                return "Undefined";
            }

            if (char.IsDigit(builder[0]))
            {
                builder.Insert(0, '_');
            }

            return builder.ToString();
        }

        static string GetUniqueIdentifier(string identifier, HashSet<string> identifierSet)
        {
            if (identifierSet.Add(identifier))
            {
                return identifier;
            }

            int index = 2;
            string uniqueIdentifier = $"{identifier}_{index}";

            while (!identifierSet.Add(uniqueIdentifier))
            {
                index++;
                uniqueIdentifier = $"{identifier}_{index}";
            }

            return uniqueIdentifier;
        }
#endif
    }
}