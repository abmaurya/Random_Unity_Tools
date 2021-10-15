/***************************************************************************************************
 * Licence          : CC BY-SA 4.0(https://creativecommons.org/licenses/by-sa/4.0/)
 * Author           : Abhinav Maurya (https://github.com/abmaurya)
 * Licence Summary  : Use anyway, attribution is necessary(BY) and share as it is(SA)
****************************************************************************************************/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GetDisabledObjects : EditorWindow
{
    //child-parent key-value pair
    Dictionary<Transform, Transform> childParentDict = new Dictionary<Transform, Transform>();
    GameObject tempObject;
    static UnityEngine.SceneManagement.Scene currScene;

    [MenuItem("Tools/Show Object Utility")]
    static void EditorWindowUtility()
    {
        GetDisabledObjects window = new GetDisabledObjects();
        CheckScene();
        window.minSize = new Vector2(200, 100);
        window.maxSize = new Vector2(200, 100);
        window.ShowUtility();
    }

    static void CheckScene()
    {
        UnityEngine.SceneManagement.Scene scn = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (currScene != scn)
        {
            currScene = scn;
        }
    }

    /// <summary>
    /// Shifts disabled objects under a new parent gameobject called DisabledObject_Holder
    /// </summary>
    void GetObjects()
    {
        CheckScene();
        //Get all the root objects in the scene(the parents)
        List<GameObject> sceneObjects = new List<GameObject>(currScene.GetRootGameObjects());

        //Go over each parent added above and add their children to the list 
        for (int j = 0; j < sceneObjects.Count; j++)
        {
            for (int i = 0; i < sceneObjects[j].transform.childCount; i++)
            {
                sceneObjects.Add(sceneObjects[j].transform.GetChild(i).gameObject);
            }
        }

        //Temporary empty gameobject to put the disabled objects under it
        tempObject = new GameObject("DisabledObject_Holder");

        //Make Child-Parent dictionary
        for (int i = 0; i < sceneObjects.Count; i++)
        {
            if (!sceneObjects[i].activeSelf)
            {
                childParentDict[sceneObjects[i].transform] = sceneObjects[i].transform.parent;
                sceneObjects[i].transform.SetParent(tempObject.transform);
            }
        }
        sceneObjects.Clear();

        //Highlight the temporary object in the hierarchy
        EditorGUIUtility.PingObject(tempObject);
    }

    /// <summary>
    /// Sends back gameobjects from under DisabledObject_Holder(as parent) to their original parent
    /// </summary>
    void ResetParents()
    {
        if (childParentDict.Count <= 0)
        {
            Debug.LogError("No object to reset. First use the Show Disabled Objects option.");
            return;
        }

        foreach (var ob in childParentDict)
        {
            ob.Key.SetParent(ob.Value);
        }
        childParentDict.Clear();
        DestroyImmediate(tempObject);
    }

    void OnGUI()
    {
        GUILayout.Space(20);
        if (GUILayout.Button("Show Disabled Objects"))
            GetObjects();
        GUILayout.Space(10);
        if (GUILayout.Button("Reset objects"))
            ResetParents();
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}
