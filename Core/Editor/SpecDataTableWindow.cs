#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PahlUnity
{
    public class SpecDataTableWindow : EditorWindow
    {
        private struct SpecColumnInfo
        {
            public string Key;
            public SpecModifierType ModifierType;

            public string ColumnName => GetColumnName();

            public string GetColumnName()
            {
                string modifierSuffix = ModifierType == SpecModifierType.Base ? "Add"
                    : (ModifierType == SpecModifierType.Percent ? "Up" : "");
                return Key + modifierSuffix;
            }
        }

        private const string DefaultDataFolder = "Assets/DataFolderPath";
        private const string AllTypesFilter = "All";
        private const float AssetColumnWidth = 110f;
        private const float TypeColumnWidth = 96f;
        private const float SpecColumnWidth = 180f;
        private const float FieldLabelWidth = 56f;
        private const float RowHeight = 20f;
        private const string ShowStepFieldPrefKey = "SpecDataTableWindow.ShowStepField";
        private const string ShowValueTypeFieldPrefKey = "SpecDataTableWindow.ShowValueTypeField";
        private const string ShowRemoveButtonPrefKey = "SpecDataTableWindow.ShowRemoveButton";
        private const string DataFolderPrefKey = "SpecDataTableWindow.DataFolder";
        private const string TypeFilterPrefKey = "SpecDataTableWindow.TypeFilter";

        private float TableContentWidth =>
            AssetColumnWidth + TypeColumnWidth + (mSpecColumns.Count * SpecColumnWidth) + 8f;

        private string mDataFolder = DefaultDataFolder;
        private string mSearchFilter = "";
        private string mTypeFilter = AllTypesFilter;
        private bool mShowStepField = false;
        private bool mShowValueTypeField = false;
        private bool mShowRemoveButton = false;
        private Vector2 mScrollPosition;

        private readonly List<ScriptableObject> mSpecAssetList = new List<ScriptableObject>();
        private readonly List<SerializedObject> mSerializedObjects = new List<SerializedObject>();
        private readonly List<SpecColumnInfo> mSpecColumns = new List<SpecColumnInfo>();
        private readonly List<string> mAvailableTypeFilters = new List<string>();

        [MenuItem("PahlUnity/Spec Data Editor")]
        public static void ShowWindow()
        {
            SpecDataTableWindow window = GetWindow<SpecDataTableWindow>("Spec Data Table");
            window.minSize = new Vector2(720f, 320f);
            window.Show();
        }

        private void OnEnable()
        {
            mShowStepField = EditorPrefs.GetBool(ShowStepFieldPrefKey, false);
            mShowValueTypeField = EditorPrefs.GetBool(ShowValueTypeFieldPrefKey, false);
            mShowRemoveButton = EditorPrefs.GetBool(ShowRemoveButtonPrefKey, false);
            mDataFolder = EditorPrefs.GetString(DataFolderPrefKey, DefaultDataFolder);
            mTypeFilter = EditorPrefs.GetString(TypeFilterPrefKey, AllTypesFilter);
            RefreshAssets();
        }

        private void OnGUI()
        {
            DrawToolbar();

            if (mSpecAssetList.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    $"'{mDataFolder}' 폴더에서 ISpecContainer를 구현한 ScriptableObject 에셋을 찾지 못했습니다. Refresh를 눌러 다시 불러오세요.",
                    MessageType.Info);
                return;
            }

            if (mSpecColumns.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "Specs에 Key가 등록된 데이터가 없습니다. Inspector에서 Spec Key를 추가한 뒤 Refresh하세요.",
                    MessageType.Info);
                return;
            }

            mScrollPosition = EditorGUILayout.BeginScrollView(mScrollPosition);
            DrawMatrixTableHeader();

            for (int i = 0; i < mSpecAssetList.Count; i++)
            {
                ScriptableObject specAsset = mSpecAssetList[i];
                SerializedObject serializedObject = mSerializedObjects[i];

                if (!IsMatchTypeFilter(specAsset) || !IsMatchFilter(serializedObject))
                {
                    continue;
                }

                serializedObject.Update();
                DrawMatrixRow(specAsset, serializedObject);
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            EditorGUILayout.LabelField("Folder", GUILayout.Width(40f));
            string nextDataFolder = EditorGUILayout.TextField(mDataFolder, GUILayout.MinWidth(180f));
            if (!string.Equals(nextDataFolder, mDataFolder, StringComparison.Ordinal))
            {
                mDataFolder = nextDataFolder;
                EditorPrefs.SetString(DataFolderPrefKey, mDataFolder);
            }

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60f)))
            {
                RefreshAssets();
            }

            if (GUILayout.Button("Save All", EditorStyles.toolbarButton, GUILayout.Width(60f)))
            {
                SaveAll();
            }

            EditorGUILayout.LabelField("Type", GUILayout.Width(32f));
            int typeFilterIndex = GetTypeFilterIndex(mTypeFilter);
            int nextTypeFilterIndex = EditorGUILayout.Popup(
                typeFilterIndex,
                mAvailableTypeFilters.ToArray(),
                EditorStyles.toolbarPopup,
                GUILayout.Width(110f));
            if (nextTypeFilterIndex != typeFilterIndex)
            {
                mTypeFilter = mAvailableTypeFilters[nextTypeFilterIndex];
                EditorPrefs.SetString(TypeFilterPrefKey, mTypeFilter);
            }

            EditorGUILayout.LabelField("Filter", GUILayout.Width(36f));
            mSearchFilter = EditorGUILayout.TextField(mSearchFilter, EditorStyles.toolbarSearchField, GUILayout.MinWidth(120f));

            DrawToolbarToggle("Show Step", ref mShowStepField, ShowStepFieldPrefKey, 80f);
            DrawToolbarToggle("Show ValueType", ref mShowValueTypeField, ShowValueTypeFieldPrefKey, 104f);
            DrawToolbarToggle("Show Remove", ref mShowRemoveButton, ShowRemoveButtonPrefKey, 92f);

            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField($"Assets: {mSpecAssetList.Count}  Columns: {mSpecColumns.Count}", GUILayout.Width(170f));
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawToolbarToggle(string label, ref bool value, string prefKey, float width)
        {
            bool nextValue = EditorGUILayout.ToggleLeft(label, value, GUILayout.Width(width));
            if (nextValue == value)
            {
                return;
            }

            value = nextValue;
            EditorPrefs.SetBool(prefKey, value);
        }

        private void DrawMatrixTableHeader()
        {
            EditorGUILayout.BeginHorizontal(
                EditorStyles.toolbar,
                GUILayout.MinWidth(TableContentWidth),
                GUILayout.ExpandWidth(false));
            DrawHeaderCell("Asset", AssetColumnWidth);
            DrawHeaderCell("Type", TypeColumnWidth);

            for (int i = 0; i < mSpecColumns.Count; i++)
            {
                DrawHeaderCell(mSpecColumns[i].ColumnName, SpecColumnWidth);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawMatrixRow(ScriptableObject specAsset, SerializedObject serializedObject)
        {
            SerializedProperty specsProp = serializedObject.FindProperty("_Specs");
            if (specsProp == null)
            {
                return;
            }

            float rowContentHeight = CalculateRowContentHeight(specsProp);

            Color previousColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.92f, 0.92f, 0.92f);
            EditorGUILayout.BeginHorizontal(
                "box",
                GUILayout.MinWidth(TableContentWidth),
                GUILayout.ExpandWidth(false));
            GUI.backgroundColor = previousColor;

            DrawAssetCell(specAsset, AssetColumnWidth, rowContentHeight);
            DrawTypeCell(specAsset, TypeColumnWidth, rowContentHeight);

            for (int columnIndex = 0; columnIndex < mSpecColumns.Count; columnIndex++)
            {
                SpecColumnInfo columnInfo = mSpecColumns[columnIndex];
                DrawSpecColumnCell(specsProp, columnInfo, SpecColumnWidth, rowContentHeight);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(2f);
        }

        private float CalculateRowContentHeight(SerializedProperty specsProp)
        {
            float maxHeight = RowHeight;

            for (int columnIndex = 0; columnIndex < mSpecColumns.Count; columnIndex++)
            {
                SpecColumnInfo columnInfo = mSpecColumns[columnIndex];
                SerializedProperty specProp = FindSpecByColumn(specsProp, columnInfo, out _);
                float cellHeight = specProp == null
                    ? GetEmptySpecCellHeight()
                    : GetFilledSpecCellHeight(specProp);
                maxHeight = Mathf.Max(maxHeight, cellHeight);
            }

            return maxHeight;
        }

        private float GetEmptySpecCellHeight()
        {
            return RowHeight;
        }

        private float GetFilledSpecCellHeight(SerializedProperty specProp)
        {
            float height = 0f;
            SerializedProperty valueTypeProp = FindSpecProperty(specProp, "_ValueType");
            SpecValueType valueType = valueTypeProp != null
                ? (SpecValueType)valueTypeProp.enumValueIndex
                : SpecValueType.Simple;

            if (mShowValueTypeField)
            {
                height += RowHeight;
            }

            height += GetValueFieldsHeight(specProp, valueType);

            if (mShowStepField)
            {
                height += RowHeight;
            }

            if (mShowRemoveButton)
            {
                height += RowHeight;
            }

            return height;
        }

        private static float GetValueFieldsHeight(SerializedProperty specProp, SpecValueType valueType)
        {
            switch (valueType)
            {
                case SpecValueType.Simple:
                    return GetPropertyCellHeight(FindSpecProperty(specProp, "_Value"));
                case SpecValueType.MinMax:
                    return GetPropertyCellHeight(FindSpecProperty(specProp, "_Min"))
                        + GetPropertyCellHeight(FindSpecProperty(specProp, "_Max"));
                case SpecValueType.Curve:
                    return GetPropertyCellHeight(FindSpecProperty(specProp, "_Curve"), true);
                case SpecValueType.Custom:
                    return GetPropertyCellHeight(FindSpecProperty(specProp, "_CustomValue"));
                case SpecValueType.List:
                    return GetPropertyCellHeight(FindSpecProperty(specProp, "_ListValues"), true);
                default:
                    return RowHeight;
            }
        }

        private static float GetPropertyCellHeight(SerializedProperty property, bool includeChildren = false)
        {
            if (property == null)
            {
                return RowHeight;
            }

            if (!includeChildren)
            {
                return RowHeight;
            }

            return EditorGUI.GetPropertyHeight(property, true);
        }

        private static void DrawTypeCell(ScriptableObject specAsset, float width, float rowContentHeight)
        {
            BeginFixedWidthColumn(width, rowContentHeight);
            DrawVerticallyCenteredContent(rowContentHeight, RowHeight, () =>
            {
                EditorGUILayout.LabelField(
                    specAsset.GetType().Name,
                    EditorStyles.miniLabel,
                    GUILayout.Width(width - 4f),
                    GUILayout.Height(RowHeight));
            });
            EndFixedWidthColumn();
        }

        private void DrawSpecColumnCell(
            SerializedProperty specsProp,
            SpecColumnInfo columnInfo,
            float width,
            float rowContentHeight)
        {
            SerializedProperty specProp = FindSpecByColumn(specsProp, columnInfo, out int specIndex);
            bool hasSpec = specProp != null;
            BeginFixedWidthColumn(width, hasSpec ? 0f : rowContentHeight);

            if (!hasSpec)
            {
                DrawEmptySpecCell(specsProp, columnInfo, width, rowContentHeight);
                EndFixedWidthColumn();
                return;
            }

            SerializedProperty valueTypeProp = FindSpecProperty(specProp, "_ValueType");
            if (valueTypeProp != null)
            {
                if (mShowValueTypeField)
                {
                    DrawPropertyCell(valueTypeProp, width);
                }

                SpecValueType valueType = (SpecValueType)valueTypeProp.enumValueIndex;
                DrawValueFieldsByType(specProp, valueType, width);
            }

            if (mShowStepField)
            {
                DrawPropertyCell(FindSpecProperty(specProp, "_Step"), width);
            }

            if (mShowRemoveButton && GUILayout.Button("Remove", GUILayout.Width(width - 4f), GUILayout.Height(RowHeight)))
            {
                specsProp.DeleteArrayElementAtIndex(specIndex);
                EndFixedWidthColumn();
                return;
            }

            EndFixedWidthColumn();
        }

        private void DrawEmptySpecCell(
            SerializedProperty specsProp,
            SpecColumnInfo columnInfo,
            float width,
            float rowContentHeight)
        {
            DrawVerticallyCenteredContent(rowContentHeight, GetEmptySpecCellHeight(), () =>
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(width));
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("-", GUILayout.Width(16f), GUILayout.Height(RowHeight));

                if (GUILayout.Button("+", GUILayout.Width(24f), GUILayout.Height(RowHeight)))
                {
                    AddSpecWithColumn(specsProp, columnInfo);
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            });
        }

        private static void DrawVerticallyCenteredContent(float containerHeight, float contentHeight, Action drawContent)
        {
            float topPadding = Mathf.Max(0f, (containerHeight - contentHeight) * 0.5f);
            if (topPadding > 0f)
            {
                GUILayout.Space(topPadding);
            }

            drawContent();

            float bottomPadding = Mathf.Max(0f, containerHeight - contentHeight - topPadding);
            if (bottomPadding > 0f)
            {
                GUILayout.Space(bottomPadding);
            }
        }

        private static void BeginFixedWidthColumn(float width, float height = 0f)
        {
            if (height > 0f)
            {
                EditorGUILayout.BeginVertical(
                    GUILayout.Width(width),
                    GUILayout.MinWidth(width),
                    GUILayout.MaxWidth(width),
                    GUILayout.Height(height),
                    GUILayout.ExpandHeight(false));
                return;
            }

            EditorGUILayout.BeginVertical(
                GUILayout.Width(width),
                GUILayout.MinWidth(width),
                GUILayout.MaxWidth(width),
                GUILayout.ExpandHeight(false));
        }

        private static void EndFixedWidthColumn()
        {
            EditorGUILayout.EndVertical();
        }

        private static void DrawValueFieldsByType(
            SerializedProperty specProp,
            SpecValueType valueType,
            float width)
        {
            switch (valueType)
            {
                case SpecValueType.Simple:
                    DrawPropertyCell(FindSpecProperty(specProp, "_Value"), width);
                    break;
                case SpecValueType.MinMax:
                    DrawPropertyCell(FindSpecProperty(specProp, "_Min"), width);
                    DrawPropertyCell(FindSpecProperty(specProp, "_Max"), width);
                    break;
                case SpecValueType.Curve:
                    DrawFlexiblePropertyCell(FindSpecProperty(specProp, "_Curve"), width);
                    break;
                case SpecValueType.Custom:
                    DrawPropertyCell(FindSpecProperty(specProp, "_CustomValue"), width);
                    break;
                case SpecValueType.List:
                    DrawFlexiblePropertyCell(FindSpecProperty(specProp, "_ListValues"), width);
                    break;
            }
        }

        private static void AddSpecWithColumn(SerializedProperty specsProp, SpecColumnInfo columnInfo)
        {
            specsProp.InsertArrayElementAtIndex(specsProp.arraySize);
            SerializedProperty newSpecProp = specsProp.GetArrayElementAtIndex(specsProp.arraySize - 1);

            SerializedProperty keyProp = FindSpecProperty(newSpecProp, "_Key");
            if (keyProp != null)
            {
                keyProp.stringValue = columnInfo.Key;
            }

            SerializedProperty modifierTypeProp = FindSpecProperty(newSpecProp, "_ModifierType");
            if (modifierTypeProp != null)
            {
                modifierTypeProp.enumValueIndex = (int)columnInfo.ModifierType;
            }
        }

        private static SerializedProperty FindSpecByColumn(
            SerializedProperty specsProp,
            SpecColumnInfo columnInfo,
            out int specIndex)
        {
            specIndex = -1;

            for (int i = 0; i < specsProp.arraySize; i++)
            {
                SerializedProperty specProp = specsProp.GetArrayElementAtIndex(i);
                SerializedProperty keyProp = FindSpecProperty(specProp, "_Key");
                SerializedProperty modifierTypeProp = FindSpecProperty(specProp, "_ModifierType");

                if (keyProp == null || modifierTypeProp == null)
                {
                    continue;
                }

                string key = keyProp.stringValue;
                SpecModifierType modifierType = (SpecModifierType)modifierTypeProp.enumValueIndex;
                if (key == columnInfo.Key && modifierType == columnInfo.ModifierType)
                {
                    specIndex = i;
                    return specProp;
                }
            }

            return null;
        }

        private static SerializedProperty FindSpecProperty(SerializedProperty specProp, string propertyName)
        {
            SerializedProperty property = specProp.FindPropertyRelative(propertyName);
            if (property != null)
            {
                return property;
            }

            return specProp.FindPropertyRelative(propertyName.TrimStart('_'));
        }

        private static void DrawAssetCell(ScriptableObject specAsset, float width, float rowContentHeight)
        {
            BeginFixedWidthColumn(width, rowContentHeight);
            DrawVerticallyCenteredContent(rowContentHeight, RowHeight, () =>
            {
                if (GUILayout.Button(
                    new GUIContent(specAsset.name, AssetDatabase.GetAssetPath(specAsset)),
                    EditorStyles.linkLabel,
                    GUILayout.Width(width - 4f),
                    GUILayout.Height(RowHeight)))
                {
                    Selection.activeObject = specAsset;
                    EditorGUIUtility.PingObject(specAsset);
                }
            });
            EndFixedWidthColumn();
        }

        private static void DrawPropertyCell(SerializedProperty property, float width)
        {
            if (property == null)
            {
                GUILayout.Label("-", GUILayout.Width(width), GUILayout.Height(RowHeight));
                return;
            }

            float previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = FieldLabelWidth;
            EditorGUILayout.PropertyField(
                property,
                new GUIContent(GetFieldLabelText(property)),
                GUILayout.Width(width),
                GUILayout.Height(RowHeight));
            EditorGUIUtility.labelWidth = previousLabelWidth;
        }

        private static void DrawFlexiblePropertyCell(SerializedProperty property, float width)
        {
            if (property == null)
            {
                GUILayout.Label("-", GUILayout.Width(width));
                return;
            }

            float previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = FieldLabelWidth;
            EditorGUILayout.PropertyField(
                property,
                new GUIContent(GetFieldLabelText(property)),
                true,
                GUILayout.Width(width));
            EditorGUIUtility.labelWidth = previousLabelWidth;
        }

        private static string GetFieldLabelText(SerializedProperty property)
        {
            switch (property.name)
            {
                case "_ValueType":
                    return "ValueType";
                case "_Value":
                    return "Value";
                case "_Min":
                    return "Min";
                case "_Max":
                    return "Max";
                case "_Step":
                    return "Step";
                case "_Curve":
                    return "Curve";
                case "_CustomValue":
                    return "Custom";
                case "_ListValues":
                    return "List";
                default:
                    return property.name.TrimStart('_');
            }
        }

        private static void DrawHeaderCell(string label, float width)
        {
            EditorGUILayout.LabelField(
                label,
                EditorStyles.miniBoldLabel,
                GUILayout.Width(width),
                GUILayout.MinWidth(width),
                GUILayout.MaxWidth(width));
        }

        private bool IsMatchTypeFilter(ScriptableObject specAsset)
        {
            if (mTypeFilter == AllTypesFilter)
            {
                return true;
            }

            return specAsset.GetType().Name == mTypeFilter;
        }

        private bool IsMatchFilter(SerializedObject serializedObject)
        {
            if (string.IsNullOrWhiteSpace(mSearchFilter))
            {
                return true;
            }

            string filter = mSearchFilter.ToLowerInvariant();
            UnityEngine.Object targetObject = serializedObject.targetObject;
            if (targetObject != null && targetObject.name.ToLowerInvariant().Contains(filter))
            {
                return true;
            }

            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (iterator.propertyType != SerializedPropertyType.String)
                {
                    continue;
                }

                string value = iterator.stringValue;
                if (!string.IsNullOrEmpty(value) && value.ToLowerInvariant().Contains(filter))
                {
                    return true;
                }
            }

            return false;
        }

        private void RebuildSpecColumns()
        {
            List<SpecColumnInfo> columnList = new List<SpecColumnInfo>();

            for (int i = 0; i < mSerializedObjects.Count; i++)
            {
                SerializedProperty specsProp = mSerializedObjects[i].FindProperty("_Specs");
                if (specsProp == null)
                {
                    continue;
                }

                for (int specIndex = 0; specIndex < specsProp.arraySize; specIndex++)
                {
                    SerializedProperty specProp = specsProp.GetArrayElementAtIndex(specIndex);
                    SerializedProperty keyProp = FindSpecProperty(specProp, "_Key");
                    SerializedProperty modifierTypeProp = FindSpecProperty(specProp, "_ModifierType");

                    if (keyProp == null || modifierTypeProp == null || string.IsNullOrWhiteSpace(keyProp.stringValue))
                    {
                        continue;
                    }

                    SpecColumnInfo columnInfo = new SpecColumnInfo
                    {
                        Key = keyProp.stringValue,
                        ModifierType = (SpecModifierType)modifierTypeProp.enumValueIndex,
                    };

                    if (!ContainsColumn(columnList, columnInfo))
                    {
                        columnList.Add(columnInfo);
                    }
                }
            }

            columnList.Sort((left, right) =>
                string.Compare(left.ColumnName, right.ColumnName, StringComparison.Ordinal));

            mSpecColumns.Clear();
            mSpecColumns.AddRange(columnList);
        }

        private static bool ContainsColumn(List<SpecColumnInfo> columnList, SpecColumnInfo columnInfo)
        {
            for (int i = 0; i < columnList.Count; i++)
            {
                SpecColumnInfo existingColumn = columnList[i];
                if (existingColumn.Key == columnInfo.Key && existingColumn.ModifierType == columnInfo.ModifierType)
                {
                    return true;
                }
            }

            return false;
        }

        private void RebuildTypeFilters()
        {
            HashSet<string> typeNameSet = new HashSet<string>();

            for (int i = 0; i < mSpecAssetList.Count; i++)
            {
                typeNameSet.Add(mSpecAssetList[i].GetType().Name);
            }

            mAvailableTypeFilters.Clear();
            mAvailableTypeFilters.Add(AllTypesFilter);

            List<string> typeNameList = new List<string>(typeNameSet);
            typeNameList.Sort(StringComparer.Ordinal);
            mAvailableTypeFilters.AddRange(typeNameList);

            if (!mAvailableTypeFilters.Contains(mTypeFilter))
            {
                mTypeFilter = AllTypesFilter;
                EditorPrefs.SetString(TypeFilterPrefKey, mTypeFilter);
            }
        }

        private int GetTypeFilterIndex(string typeFilter)
        {
            for (int i = 0; i < mAvailableTypeFilters.Count; i++)
            {
                if (mAvailableTypeFilters[i] == typeFilter)
                {
                    return i;
                }
            }

            return 0;
        }

        private void RefreshAssets()
        {
            mSpecAssetList.Clear();
            mSerializedObjects.Clear();

            if (string.IsNullOrWhiteSpace(mDataFolder))
            {
                mDataFolder = DefaultDataFolder;
            }

            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { mDataFolder });
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                ScriptableObject scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                if (scriptableObject is not ISpecContainer)
                {
                    continue;
                }

                mSpecAssetList.Add(scriptableObject);
                mSerializedObjects.Add(new SerializedObject(scriptableObject));
            }

            mSpecAssetList.Sort(CompareSpecAssets);
            mSerializedObjects.Sort((left, right) =>
                CompareSpecAssets((ScriptableObject)left.targetObject, (ScriptableObject)right.targetObject));

            RebuildTypeFilters();
            RebuildSpecColumns();
            Repaint();
        }

        private static int CompareSpecAssets(ScriptableObject left, ScriptableObject right)
        {
            int typeCompare = string.Compare(
                left.GetType().Name,
                right.GetType().Name,
                StringComparison.Ordinal);
            if (typeCompare != 0)
            {
                return typeCompare;
            }

            return string.Compare(left.name, right.name, StringComparison.Ordinal);
        }

        private void SaveAll()
        {
            AssetDatabase.SaveAssets();
            ShowNotification(new GUIContent("Saved all spec data assets."));
        }
    }
}
#endif
