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
            string projectRoot = GetProjectRootFromAssetPath(assetPath);
            string namespaceName = GetNamespaceName(projectRoot);
            GenerateSpecFieldsFile(directory, projectRoot, namespaceName);
        }

        static void GenerateSpecFieldsFile(string directory, string searchRoot, string namespaceName)
        {
            string className = "SpecFields";
            string outputPath = Path.Combine(directory, $"{className}.cs");

            List<string> fields = CollectFields(searchRoot);
            if (fields.Count == 0)
            {
                Debug.LogWarning(
                    $"[{nameof(SpecFieldDefinition)}] No spec fields found under '{searchRoot}'. " +
                    $"Create or select a {nameof(SpecFieldDefinition)} asset under '{searchRoot}', add field names to it, " +
                    $"then click 'Generate Spec Fields' again. " +
                    $"An empty define file will be generated for now.");
            }

            StringBuilder builder = new StringBuilder();

            bool useNamespace = !string.IsNullOrWhiteSpace(namespaceName);
            if (useNamespace)
            {
                builder.AppendLine($"namespace {namespaceName}");
                builder.AppendLine("{");
            }

            string indent = useNamespace ? "    " : "";
            builder.AppendLine($"{indent}public static class {className}");
            builder.AppendLine($"{indent}{{");

            HashSet<string> identifierSet = new HashSet<string>();

            for (int i = 0; i < fields.Count; i++)
            {
                string field = fields[i];
                string identifier = GetUniqueIdentifier(ToIdentifier(field), identifierSet);
                string escapedField = field.Replace("\\", "\\\\").Replace("\"", "\\\"");

                builder.AppendLine($"{indent}   public static readonly int {identifier} = PahlUnity.StableHash.ToInt32(\"{escapedField}\");");
            }

            builder.AppendLine($"{indent}}}");

            if (useNamespace)
            {
                builder.AppendLine("}");
            }

            File.WriteAllText(outputPath, builder.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();
        }

        public static List<string> CollectFields()
        {
            return CollectFields("Assets");
        }

        public static List<string> CollectFields(string searchRoot)
        {
            string typeName = nameof(SpecFieldDefinition);
            string[] guids = AssetDatabase.FindAssets($"t:{typeName}", new[] { searchRoot });
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

        public static string GetProjectRoot(UnityEngine.Object context)
        {
            if (context == null)
            {
                return "Assets";
            }

            string assetPath = AssetDatabase.GetAssetPath(context);
            if (string.IsNullOrEmpty(assetPath))
            {
                if (context is Component component)
                {
                    assetPath = component.gameObject.scene.path;
                }
                else if (context is GameObject gameObject)
                {
                    assetPath = gameObject.scene.path;
                }
            }

            return GetProjectRootFromAssetPath(assetPath);
        }

        public static string GetProjectRootFromAssetPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return "Assets";
            }

            string normalizedPath = assetPath.Replace('\\', '/');
            string[] parts = normalizedPath.Split('/');
            if (parts.Length < 2 || parts[0] != "Assets")
            {
                return "Assets";
            }

            return $"Assets/{parts[1]}";
        }

        static string GetNamespaceName(string projectRoot)
        {
            if (string.IsNullOrEmpty(projectRoot))
            {
                return "";
            }

            string normalizedPath = projectRoot.Replace('\\', '/');
            string[] parts = normalizedPath.Split('/');
            if (parts.Length < 2)
            {
                return "";
            }

            return ToIdentifier(parts[1]);
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