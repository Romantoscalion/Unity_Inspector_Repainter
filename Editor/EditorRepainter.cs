using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using System.Collections;

public class EditorRepainter : EditorWindow
{
    private const string INSPECTOR_TYPE_NAME = "UnityEditor.InspectorWindow,UnityEditor.dll";
    private const float REFRESH_INTERVAL = 5f;
    private readonly static System.Type INSPECTOR_TYPE = System.Type.GetType(INSPECTOR_TYPE_NAME);

    private List<EditorWindow> allInspectorWindows;
    private bool forceRepaint = false;
    private EditorCoroutine coroutine;

    [MenuItem("Tools/Inspector_Repainter")] 
    public static void OpenWindow() => GetWindow<EditorRepainter>();

    protected void OnEnable() => coroutine = EditorCoroutineUtility.StartCoroutineOwnerless(UpdateInspectorList());
    protected void OnDisable() => EditorCoroutineUtility.StopCoroutine(coroutine);

    public void Update()
    {
        if (forceRepaint) allInspectorWindows.ForEach(x => x.Repaint());
    }

    private void OnGUI() 
    {
        if (GUILayout.Button("Switch")) forceRepaint = !forceRepaint;
        DrawPilotLamp();
    }

    private IEnumerator UpdateInspectorList()
    {
        while (true)
        {
            allInspectorWindows = Resources.FindObjectsOfTypeAll<EditorWindow>().ToList();
            allInspectorWindows = allInspectorWindows.Where(x => x.GetType() == INSPECTOR_TYPE).ToList();
            yield return new EditorWaitForSeconds(REFRESH_INTERVAL);
        }
    }
    
    private GUILayoutOption defaltWidth = GUILayout.Width(100);
    private GUILayoutOption defaltHeight = GUILayout.Height(50);
    public void DrawPilotLamp()
    {
        EditorGUILayout.BeginHorizontal(defaltWidth, defaltHeight);
        EditorGUILayout.LabelField("Refreshing", defaltWidth, defaltHeight);
        var pilotLamp = forceRepaint ? EditorGUIUtility.IconContent("greenLight") : EditorGUIUtility.IconContent("lightOff");
        EditorGUILayout.LabelField(pilotLamp, defaltWidth, defaltHeight);
        EditorGUILayout.EndHorizontal();
    }
}