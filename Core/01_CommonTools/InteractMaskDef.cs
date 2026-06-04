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
    [CreateAssetMenu(fileName = "InteractMaskDef", menuName = "PahlUnity/Interact Mask Definition")]
    public class InteractMaskDef : ScriptableObject
    {
        public const int MaxMaskCount = 32;

        [SerializeField] private List<string> _Names = new List<string>();

        public IReadOnlyList<string> Names => _Names;

#if UNITY_EDITOR
        [Button("Generate Interact Mask Defines")]
        void GenerateInteractMaskDefines()
        {
            string assetPath = AssetDatabase.GetAssetPath(this);
            string directory = Path.GetDirectoryName(assetPath);
            GenerateInteractMaskDefinesFile(directory);
        }

        static void GenerateInteractMaskDefinesFile(string directory)
        {
            string className = "InteractMask";
            string outputPath = Path.Combine(directory, $"{className}.cs");

            List<string> names = CollectNames();
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"public static class {className}");
            builder.AppendLine("{");
            builder.AppendLine("   public const uint Nothing = 0u;");

            HashSet<string> identifierSet = new HashSet<string>();
            uint everything = 0u;

            for (int i = 0; i < names.Count; i++)
            {
                string maskName = names[i];
                string identifier = GetUniqueIdentifier(ToIdentifier(maskName), identifierSet);
                uint maskValue = GetMaskValue(i);
                everything |= maskValue;

                builder.AppendLine($"   public const uint {identifier} = 1u << {i};");
            }

            builder.AppendLine($"   public const uint Everything = {FormatUIntLiteral(everything)};");
            builder.AppendLine("}");

            File.WriteAllText(outputPath, builder.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();
        }

        public static uint GetMaskValue(int index)
        {
            if (index < 0 || index >= MaxMaskCount)
            {
                return 0u;
            }

            return 1u << index;
        }

        public static List<string> CollectNames()
        {
            string typeName = nameof(InteractMaskDef);
            string[] guids = AssetDatabase.FindAssets($"t:{typeName}", new[] { "Assets" });
            List<string> assetPaths = new List<string>();
            for (int i = 0; i < guids.Length; i++)
            {
                assetPaths.Add(AssetDatabase.GUIDToAssetPath(guids[i]));
            }
            assetPaths.Sort(StringComparer.Ordinal);

            HashSet<string> nameSet = new HashSet<string>();
            List<string> names = new List<string>();

            for (int i = 0; i < assetPaths.Count; i++)
            {
                InteractMaskDef definition = AssetDatabase.LoadAssetAtPath<InteractMaskDef>(assetPaths[i]);
                if (definition == null)
                {
                    continue;
                }

                IReadOnlyList<string> definitionNames = definition.Names;
                for (int nameIndex = 0; nameIndex < definitionNames.Count; nameIndex++)
                {
                    string maskName = definitionNames[nameIndex];
                    if (string.IsNullOrWhiteSpace(maskName))
                    {
                        continue;
                    }

                    string trimmedName = maskName.Trim();
                    if (nameSet.Add(trimmedName))
                    {
                        names.Add(trimmedName);
                        if (names.Count >= MaxMaskCount)
                        {
                            Debug.LogWarning($"{nameof(InteractMaskDef)} supports up to {MaxMaskCount} masks.");
                            return names;
                        }
                    }
                }
            }

            return names;
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

        static string FormatUIntLiteral(uint value)
        {
            return value == uint.MaxValue ? "0xffffffffu" : $"{value}u";
        }
#endif
    }
}