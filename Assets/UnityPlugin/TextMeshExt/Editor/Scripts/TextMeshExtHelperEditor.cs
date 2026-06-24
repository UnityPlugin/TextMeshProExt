using UnityEditor;
using UnityEngine;
using IMGUIUtils = IMGUI.Utils;

namespace TextMeshExt
{
    [CustomEditor(typeof(TextMeshExtHelper))]
    public class TextMeshExtHelperEditor : Editor
    {
        TextMeshExtHelper _target;

        GUILayoutOption[] TOGGLE_LAYOUT = { GUILayout.Width(20) };

        void OnEnable()
        {
            _target = target as TextMeshExtHelper;
        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            var taregetProperty = serializedObject.FindProperty("target");
            var baseMaterialProperty = serializedObject.FindProperty("baseMaterial");
            var overrideOutlineProperty = serializedObject.FindProperty("overrideOutline");
            var outlineProperty = serializedObject.FindProperty("outline");
            var overrideUnderlayProperty = serializedObject.FindProperty("overrideUnderlay");
            var underlayProperty = serializedObject.FindProperty("underlay");

            IMGUIUtils.Editable(() =>
            {
                EditorGUILayout.PropertyField(taregetProperty);
            }, false);

            IMGUIUtils.Change(
            () =>
            {
                EditorGUILayout.PropertyField(baseMaterialProperty);

                IMGUIUtils.PropertyHorizontal(outlineProperty.displayName, () =>
                {
                    overrideOutlineProperty.boolValue = EditorGUILayout.Toggle(overrideOutlineProperty.boolValue, TOGGLE_LAYOUT);
                    if (overrideOutlineProperty.boolValue)
                    {
                        outlineProperty.colorValue = EditorGUILayout.ColorField(outlineProperty.colorValue);
                    }
                });

                IMGUIUtils.PropertyHorizontal(underlayProperty.displayName, () =>
                {
                    overrideUnderlayProperty.boolValue = EditorGUILayout.Toggle(overrideUnderlayProperty.boolValue, TOGGLE_LAYOUT);
                    if (overrideUnderlayProperty.boolValue)
                    {
                        underlayProperty.colorValue = EditorGUILayout.ColorField(underlayProperty.colorValue);
                    }
                });

                if (baseMaterialProperty.boxedValue == null)
                {
                    var text = IMGUIUtils.GetGUIContent("Base Material is null, change Material Preset or select in Project");
                    if (text.image == null)
                    {
                        var tmp = EditorGUIUtility.TrTextContentWithIcon("", MessageType.Warning);
                        text.image = tmp.image;
                    }
                    EditorGUILayout.HelpBox(text);
                }
            },
            () =>
            {
                serializedObject.ApplyModifiedProperties();

                _target.UpdateMaterial();

                GUIUtility.ExitGUI();
            });

            EditorGUILayout.Space();
            var key = _target.GetKey();
            var matDict = _target.GetAllMaterials();
            matDict.TryGetValue(key, out var mat);

            var count = 0;
            var refDict = _target.GetMaterialRef();
            if (mat) refDict.TryGetValue(mat, out count);

            EditorGUILayout.LabelField(IMGUIUtils.GetGUIContent("Total Mateials"), IMGUIUtils.GetGUIContent(matDict.Count.ToString()));

            IMGUIUtils.Foldout("Current Material", () =>
            {
                EditorGUILayout.ObjectField(IMGUIUtils.GetGUIContent("Material"), mat, typeof(Material), false);
                EditorGUILayout.LabelField(IMGUIUtils.GetGUIContent("Ref"), IMGUIUtils.GetGUIContent(count.ToString()));
            });

            EditorGUILayout.Space();
            if (GUILayout.Button(IMGUIUtils.GetGUIContent("Clear")))
            {
                _target.ClearDict();
            }
        }
    }
}