#if UNITY_6000_3_OR_NEWER
    #define RH_UNITY_FEATURE_ENTITYID
#endif
#if UNITY_2018_3_OR_NEWER
    #define RH_UNITY_FEATURE_SCENE_HANDLE
    #define RH_UNITY_FEATURE_PREFAB_EDIT_MODE
#endif
#if UNITY_5_4_OR_NEWER || (UNITY_5 && !UNITY_5_0)
    #define RH_UNITY_FEATURE_WINDOWTITLE
    #define RH_UNITY_FEATURE_DEBUG_ASSERT
#endif
using UnityEngine;
// Note: This script is only for the editor, but we don't put it into an Editor folder because it needs to be attached to a MonoBehaviour
#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
#if RH_UNITY_FEATURE_SCENE_HANDLE
using UnityEngine.SceneManagement;
using System.Reflection;
#endif

//-----------------------------------------------------------------------------
// Copyright 2017-2022 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.SessionRestore
{
    /// <summary>
    /// Utility functions for getting and setting the expanded state of the Unity hierarchy window
    /// Also has functions for getting and setting the hierarchy window selection
    /// Also has a function to return which scene nodes are expanded
    /// </summary>
    public static class HierarchyUtils
    {
        public static bool IsHierarchyWindowInPrefabMode()
        {
            bool result = false;
#if RH_UNITY_FEATURE_PREFAB_EDIT_MODE
#if UNITY_2021_2_OR_NEWER
            result = (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null);
#else
			result = (UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null);
#endif
#endif
            return result;
        }

        public static string GetHierarchyAsString()
        {
            string result = string.Empty;
            GameObject[] gos = GetHierarchyWindowExpandedObjects();

#if RH_UNITY_FEATURE_SCENE_HANDLE
            result += GetScenesAsString();
#endif

            // Convert transforms into a path list separated by PathListSeparator
            if (gos != null && gos.Length > 0)
            {
                foreach (GameObject go in gos)
                {
                    string path = AnimationUtility.CalculateTransformPath(go.transform, null);

                    // NOTE: We add '/' to the start of the path as without this GameObject.Find will find any GameObject with this name and not start from scene root
                    result += "/" + path + HierarchyRestore.PathListSeparator;
                }
            }
            return result;
        }

        public static string GetSelectionAsString()
        {
            string result = string.Empty;
            // Cache the selection
            if (Selection.activeGameObject != null)
            {
                // Convert transforms into a path list separated by PathListSeparator
                if (Selection.transforms != null && Selection.transforms.Length > 0)
                {
                    for (int i = 0; i < Selection.transforms.Length; i++)
                    {
                        Transform xform = Selection.transforms[i];
                        string path = AnimationUtility.CalculateTransformPath(xform, null);

                        // NOTE: We add '/' to the start of the path as without this GameObject.Find will find any GameObject with this name and not start from scene root
                        result += "/" + path + HierarchyRestore.PathListSeparator;
                    }
                }
            }
            return result;
        }

#if RH_UNITY_FEATURE_SCENE_HANDLE
        private static string GetScenesAsString()
        {
            string result = string.Empty;
            Scene[] scenes = GetHierarchyWindowExpandedScenes();

            // Convert transforms into a path list separated by PathListSeparator
            if (scenes != null && scenes.Length > 0)
            {
                foreach (Scene scene in scenes)
                {
                    result += scene.path + HierarchyRestore.PathListSeparator;
                }
            }
            return result;
        }
#endif

        public static void SetHierarchyFromString(string list, bool logMissingNodes)
        {
            string[] paths = null;
            {
                // Convert the string list to an array
                if (!string.IsNullOrEmpty(list))
                {
                    paths = list.Split(new string[] { HierarchyRestore.PathListSeparator }, System.StringSplitOptions.RemoveEmptyEntries);
                }
            }

            // Convert the paths array into an array of objects
            GameObject[] gos = null;
#if RH_UNITY_FEATURE_ENTITYID
            UnityEngine.EntityId[] sceneHandles = null;
#else
            int[] sceneHandles = null;
#endif
            if (paths != null && paths.Length > 0)
            {
                List<GameObject> goList = new List<GameObject>();
#if RH_UNITY_FEATURE_ENTITYID
                List<UnityEngine.EntityId> sceneList = null;
#else
                List<int> sceneList = null;
#endif
                foreach (string path in paths)
                {
                    bool isFound = false;
                    if (path.StartsWith("/"))
                    {
                        // TODO: GameObject.Find doesn't find disabled objects, so we need to handle this
                        GameObject go = GameObject.Find(path);
                        if (go != null)
                        {
                            goList.Add(go);
                            isFound = true;
                        }
                    }
#if RH_UNITY_FEATURE_SCENE_HANDLE
                    else
                    {
                        Scene scene = SceneManager.GetSceneByPath(path);
                        if (scene.IsValid())
                        {
                            if (sceneList == null)
                            {
#if RH_UNITY_FEATURE_ENTITYID
                                sceneList = new List<UnityEngine.EntityId>();
#else
                                sceneList = new List<int>();
#endif
                            }
#if RH_UNITY_FEATURE_ENTITYID
                            // No automatic conversion as ToEntityID is internal for some reasom
                            // even though SceneHandle is just a ease of use wrapper of entityID.
                            sceneList.Add((int)scene.handle);
#else
                            sceneList.Add(scene.handle);
#endif
                            isFound = true;
                        }
                    }
#endif
                    if (!isFound && logMissingNodes)
                    {
                        Debug.LogWarning("[HierarchyRestore] Can't find node with path: '" + path + "'");
                    }
                }
                gos = goList.ToArray();
                if (sceneList != null)
                {
                    sceneHandles = sceneList.ToArray();
                }
            }

            if ((gos != null && gos.Length > 0) ||
                (sceneHandles != null && sceneHandles.Length > 0))
            {
                HierarchyUtils.SetHierarchyWindowExpandedObjects(gos, sceneHandles);
            }
            else
            {
                HierarchyUtils.SetHierarchyWindowExpandedObjects(new GameObject[0], sceneHandles);
            }
        }

        public static bool SetSelectionFromString(string list, bool logMissingNodes)
        {
            // Get the path list and convert it to an array
            string[] paths = null;
            if (!string.IsNullOrEmpty(list))
            {
                paths = list.Split(new string[] { HierarchyRestore.PathListSeparator }, System.StringSplitOptions.RemoveEmptyEntries);
            }

            // Convert the paths array into a list of objects
            List<Object> gos = new List<Object>();
            if (paths != null && paths.Length > 0)
            {
                foreach (string path in paths)
                {
                    // TODO: GameObject.Find doesn't find disabled objects, so we need to handle this
                    GameObject go = GameObject.Find(path);
                    if (go != null)
                    {
                        gos.Add(go);
                    }
                    else if (logMissingNodes)
                    {
                        Debug.LogWarning("[HierarchyRestore] Can't find node with path: '" + path + "'");
                    }
                }
            }

            if (gos.Count > 0)
            {
                Selection.objects = gos.ToArray();
            }

            return (gos.Count > 0);
        }

        private static bool IsHierarchyWindow(EditorWindow window)
        {
            bool result = false;
            if (window != null)
            {
#if RH_UNITY_FEATURE_WINDOWTITLE
                if (window.titleContent.text == "Hierarchy")
#else
				if (window.title == "UnityEditor.HierarchyWindow")
#endif
                {
                    if (window.GetType().Name == "SceneHierarchyWindow")
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        internal static void FocusHierarchyWindow(bool frameSelection)
        {
            EditorWindow hierarchyWindow = HierarchyUtils.GetHierarchyWindow();
            if (hierarchyWindow)
            {
                hierarchyWindow.Focus();

                if (frameSelection)
                {
                    Event frameEvent = new Event();
                    frameEvent.type = EventType.ExecuteCommand;
                    frameEvent.commandName = "FrameSelected";
                    hierarchyWindow.SendEvent(frameEvent);
                }
            }
        }

        private static EditorWindow GetHierarchyWindow()
        {
            // Search from focused window
            EditorWindow hierarchyWindow = EditorWindow.focusedWindow;
            if (!IsHierarchyWindow(hierarchyWindow))
            {
                hierarchyWindow = null;
            }

            // Search by type (we don't do this because it creates the window if one isn't open...)
            /*if (hierarchyWindow == null)
			{
				if (_hierarchyWindowType == null)
				{
					System.Reflection.Assembly editorAssembly = typeof(EditorWindow).Assembly;
					if (editorAssembly != null)
					{
						_hierarchyWindowType = editorAssembly.GetType("UnityEditor.SceneHierarchyWindow");
					}
				}
				if (_hierarchyWindowType != null)
				{
					hierarchyWindow = EditorWindow.GetWindow(_hierarchyWindowType);
					if (!IsHierarchyWindow(hierarchyWindow))
					{
						hierarchyWindow = null;
					}
				}
			}*/


            // Search by iterating all editor windows (needed?)
            if (hierarchyWindow == null)
            {
                EditorWindow[] windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
                //Debug.Log("searching windows " + windows.Length);
                for (int i = 0; i < windows.Length; i++)
                {
                    if (IsHierarchyWindow(windows[i]))
                    {
                        hierarchyWindow = windows[i];
                        //Debug.Log("found by searching");
                        break;
                    }
                }
            }


            // Old way to find the hierarchy window - the problem is it opens the window if it isn't visible, which can be undesirable
            /*
			EditorApplication.ExecuteMenuItem("Window/Hierarchy");
			EditorWindow hierarchyWindow = EditorWindow.focusedWindow;
			*/

            return hierarchyWindow;
        }

        private static object _cachedTreeViewDataSource = null;
        private static object _cachedTreeView = null;
        private static System.Reflection.MethodInfo _cachedMethodGetExpandedIds = null;
        private static System.Reflection.MethodInfo _cachedMethodSetExpandedIds = null;
        private static System.Reflection.PropertyInfo _cachedPropertyExpandedStateChanged = null;
        private static EditorWindow currentHierarchyWindow;

        public static bool LayoutChanged()
        {
            var hierarchyWindow = GetHierarchyWindow();
            if (hierarchyWindow == null)
                currentHierarchyWindow = null;
            else if (hierarchyWindow != null && currentHierarchyWindow == null)
            {
                currentHierarchyWindow = hierarchyWindow;
                return true;
            }
            return false;
        }

        private static void CacheMethods()
        {
            if (_cachedTreeViewDataSource == null || _cachedMethodSetExpandedIds == null || _cachedMethodGetExpandedIds == null)
            {
                // NOTE: treeView could be TreeView or TreeViewController depending on the Unity version
                object treeView = null;
                {
                    // Get the hierarchy window
                    EditorWindow hierarchyWindow = GetHierarchyWindow();
                    if (hierarchyWindow == null)
                    {
                        return;
                    }

                    // Get the TreeViewState
                    var sceneHierarchywindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
                    if (sceneHierarchywindowType != null)
                    {
                        // Unity 2018.3.0 introduces SceneHierarchy class
                        var sceneHierarchyType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchy");
                        if (sceneHierarchyType != null)
                        {
                            var propInfo = sceneHierarchywindowType.GetProperty("sceneHierarchy", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                            object sceneHierarchy = null;
                            sceneHierarchy = propInfo.GetValue(hierarchyWindow, null);
                            propInfo = sceneHierarchyType.GetProperty("treeView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            treeView = propInfo.GetValue(sceneHierarchy, null);
                        }

                        // If the above failed, then try an alternative approach that should work for Unity < 2018.3.0
                        if (treeView == null)
                        {
                            var propInfo = sceneHierarchywindowType.GetProperty("treeView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            treeView = propInfo.GetValue(hierarchyWindow, null);
                        }
                    }
                }

                object treeViewDataSource = null;
                if (treeView != null)
                {

#if RH_UNITY_FEATURE_ENTITYID
					// (RBN 05/12/2025)
					// This needs to be done like this because with unity 6000.3 their is now two 'data' properties
					// one for the int32 array and a generic that is based on entityID, doing it like this 
					// guarentees that we get the one based on EntityID which we can then use throught to ensure 
					// that we are allways using the correct methods and conversions happening
                    // TreeViewController<T> or old TreeView
                    var runtimeType = treeView.GetType();  
					if (runtimeType != null)
					{
                        var propInfo = runtimeType.GetProperty("data", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        if (propInfo != null)
                        {
                            treeViewDataSource = propInfo.GetValue(treeView);
                        }
                        _cachedPropertyExpandedStateChanged = runtimeType.GetProperty("expandedStateChanged", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty);
                    }
#else
                    // Get the TreeViewState
                    // In newer versions of Unity TreeViewState has moved here:

                    var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.IMGUI.Controls.TreeViewController");
                    if (type == null)
                    {
                        // Fallback for older versions of Unity
                        type = typeof(EditorWindow).Assembly.GetType("UnityEditor.TreeView");
                    }
                    if (type != null)
                    {
                        var propInfo = type.GetProperty("data", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        treeViewDataSource = propInfo.GetValue(treeView, null);
                        _cachedPropertyExpandedStateChanged = type.GetProperty("expandedStateChanged", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty);
                    }
#endif
                }

                // Get the methods to get and set the expanded Ids
                if (treeViewDataSource != null)
                {
                    var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.TreeViewDataSource");
                    if (type == null)
                    {
                        // In newer versions of Unity TreeViewDataSource has moved here:
                        type = typeof(EditorWindow).Assembly.GetType("UnityEditor.IMGUI.Controls.TreeViewDataSource");
                    }

                    _cachedTreeViewDataSource = treeViewDataSource;

#if RH_UNITY_FEATURE_ENTITYID
					// (RBN 05/12/2025)
					// this is done differently as we need the GameobjectTreeViewSource with EntityID to ensure we can actually get the values out
                    _cachedMethodGetExpandedIds = treeViewDataSource.GetType().GetMethod("GetExpandedIDs", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    _cachedMethodSetExpandedIds = treeViewDataSource.GetType().GetMethod("SetExpandedIDs", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
#else
                    _cachedMethodGetExpandedIds = type.GetMethod("GetExpandedIDs", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    _cachedMethodSetExpandedIds = type.GetMethod("SetExpandedIDs", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
#endif
                }

                // Set up the action for expandedStateChanged
                if (treeView != null && _cachedPropertyExpandedStateChanged != null)
                {
                    _cachedTreeView = treeView;
                    System.Action expandAction = _cachedPropertyExpandedStateChanged.GetValue(_cachedTreeView, null) as System.Action;
                    if (expandAction != null)
                    {
                        expandAction -= UnityHierarchyExpandCallback;
                        expandAction += UnityHierarchyExpandCallback;
                        _cachedPropertyExpandedStateChanged.SetValue(_cachedTreeView, expandAction, null);
                    }
                }
            }
        }

        public static System.Action OnHierarchyExpandChanged;

        private static void UnityHierarchyExpandCallback()
        {
            if (OnHierarchyExpandChanged != null)
            {
                OnHierarchyExpandChanged.Invoke();
            }
        }

        // Returns a list of GameObjects that are expanded in the Hierarchy window
        public static GameObject[] GetHierarchyWindowExpandedObjects()
        {
            List<GameObject> result = new List<GameObject>();

            try
            {
                CacheMethods();

#if RH_UNITY_FEATURE_ENTITYID
                UnityEngine.EntityId[] expandedIds = null;
#else
                int[] expandedIds = null;
#endif
                if (_cachedTreeViewDataSource != null && _cachedMethodGetExpandedIds != null)
                {
#if RH_UNITY_FEATURE_ENTITYID
                    expandedIds = (UnityEngine.EntityId[])_cachedMethodGetExpandedIds.Invoke(_cachedTreeViewDataSource, null);
#else
                    expandedIds = (int[])_cachedMethodGetExpandedIds.Invoke(_cachedTreeViewDataSource, null);
#endif
                }

                // Filter the list for GameObjects
                if (expandedIds != null)
                {
                    foreach (int id in expandedIds)
                    {
#if RH_UNITY_FEATURE_ENTITYID
                        GameObject go = EditorUtility.EntityIdToObject(id) as GameObject;
#else
                        GameObject go = EditorUtility.InstanceIDToObject(id) as GameObject;
#endif
                        if (go != null)
                        {
                            result.Add(go);
                        }
                        else
                        {
                            // Scene objects have negative Id
                            //Debug.Log("not found " + id);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("[HierarchyRestore] Can't hierarchy get methods via reflection");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            return result.ToArray();
        }

#if RH_UNITY_FEATURE_SCENE_HANDLE
        // Returns a list of Scenes that are expanded in the Hierarchy window
        public static Scene[] GetHierarchyWindowExpandedScenes()
        {
            List<Scene> result = new List<Scene>();

            try
            {
                CacheMethods();

#if RH_UNITY_FEATURE_ENTITYID
                UnityEngine.EntityId[] expandedIds = null;
#else
                int[] expandedIds = null;
#endif
                if (_cachedTreeViewDataSource != null && _cachedMethodGetExpandedIds != null)
                {
#if RH_UNITY_FEATURE_ENTITYID
                    expandedIds = (UnityEngine.EntityId[])_cachedMethodGetExpandedIds.Invoke(_cachedTreeViewDataSource, null);
#else
                    expandedIds = (int[])_cachedMethodGetExpandedIds.Invoke(_cachedTreeViewDataSource, null);
#endif
                }

                // Filter the list for GameObjects
                if (expandedIds != null)
                {
                    foreach (int id in expandedIds)
                    {
                        // Scene Ids seem to always be negative
                        if (id <= 0)
                        {
                            // Search for a scene that matches this Id
                            for (int i = 0; i < SceneManager.sceneCount; i++)
                            {
                                Scene scene = SceneManager.GetSceneAt(i);
                                if (id == scene.handle)
                                {
                                    result.Add(scene);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("[HierarchyRestore] Can't get methods via reflection");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            return result.ToArray();
        }
#endif

#if RH_UNITY_FEATURE_ENTITYID
        public static void SetHierarchyWindowExpandedObjects(GameObject[] gos, UnityEngine.EntityId[] sceneHandles)
#else
        public static void SetHierarchyWindowExpandedObjects(GameObject[] gos, int[] sceneHandles)
#endif
        {
#if RH_UNITY_FEATURE_DEBUG_ASSERT
            Debug.Assert(gos != null);
#endif
            try
            {
                CacheMethods();

                if (_cachedTreeViewDataSource != null && _cachedMethodSetExpandedIds != null)
                {
                    // Convert array of GameObjects and sceneHandles to array of instanceIDs
                    int itemCount = gos.Length;
                    if (sceneHandles != null)
                    {
                        itemCount += sceneHandles.Length;
                    }
                    int index = 0;

#if RH_UNITY_FEATURE_ENTITYID
                    UnityEngine.EntityId[] ids = new UnityEngine.EntityId[itemCount];
                    // Add the Scene handles first, and then the GameObjects
                    if (sceneHandles != null)
                    {
                        for (int i = 0; i < sceneHandles.Length; i++)
                        {
                            ids[index++] = sceneHandles[i];
                        }
                    }
                    if (gos != null)
                    {
                        for (int i = 0; i < gos.Length; i++)
                        {
                            ids[index++] = gos[i].GetEntityId();
                        }
                    }
#else
                    int[] ids = new int[itemCount];
                    // Add the Scene handles first, and then the GameObjects
                    if (sceneHandles != null)
                    {
                        for (int i = 0; i < sceneHandles.Length; i++)
                        {
                            ids[index++] = sceneHandles[i];
                        }
                    }
                    if (gos != null)
                    {
                        for (int i = 0; i < gos.Length; i++)
                        {
                            ids[index++] = gos[i].GetInstanceID();
                        }
                    }
#endif

                    // Expand the scene hierarchy
                    _cachedMethodSetExpandedIds.Invoke(_cachedTreeViewDataSource, new object[] { ids });
                }
                else
                {
                    Debug.LogWarning("[HierarchyRestore] Can't get methods via reflection");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
#endif