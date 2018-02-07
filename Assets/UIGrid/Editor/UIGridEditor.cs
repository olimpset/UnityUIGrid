using UnityEngine;
using System.Collections;
using UnityEditor;

//[CanEditMultipleObjects]
[CustomEditor(typeof(UIGrid))]
public class LevelScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        UIGrid uiGrid = (UIGrid)target;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Grid Settings", EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.MaxWidth(Screen.width));
        uiGrid._gridAmount = EditorGUILayout.IntField("Grid Amount", uiGrid._gridAmount);
        uiGrid._goldenRatio = EditorGUILayout.Toggle("Phi Grid", uiGrid._goldenRatio);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.MaxWidth(Screen.width));
        uiGrid._snap = EditorGUILayout.Toggle("Snap", uiGrid._snap);
        uiGrid._snapStrenght = EditorGUILayout.FloatField("Snap Strenght", uiGrid._snapStrenght);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        uiGrid._gridColor = EditorGUILayout.ColorField("Grid Color", uiGrid._gridColor);
        uiGrid._intersectionColor = EditorGUILayout.ColorField("Intersection Color", uiGrid._intersectionColor);
        EditorGUILayout.EndHorizontal();


        uiGrid._rotationalOffset = EditorGUILayout.FloatField("Intersection Color", uiGrid._rotationalOffset);
        EditorGUILayout.EndVertical();
    }
}