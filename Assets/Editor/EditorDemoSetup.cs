#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
using KaijuSolutions.MultiplayerEngine;
using KaijuSolutions.MultiplayerEngine.NetcodeForGameObjects;
#endif
internal static class EditorDemoSetup
{
    /// <summary>
    /// Ensure the demo is ready for use once Kaiju Multiplayer Engine is installed.
    /// </summary>
    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        // Don't modify anything if in play mode.
        if (Application.isPlaying)
        {
            return;
        }
        
        // Open the scene.
        const string scenePath = "Assets/Level.unity";
        bool open = SceneManager.GetActiveScene().buildIndex == 0;
        if (!open)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            if (scene.buildIndex == 0)
            {
                open = true;
            }
        }
        
        // This should always be true unless the scene itself is deleted.
        if (open)
        {
            bool dirty = false;
            
            // Get the Network Manager.
            NetworkManager manager = Object.FindAnyObjectByType<NetworkManager>();
            if (manager == null)
            {
                GameObject go = new("Network Manager")
                {
                    isStatic = true
                };
                manager = go.AddComponent<NetworkManager>();
                dirty = true;
            }
            
            // Ensure the Network Manager is initialized.
            if (manager.NetworkConfig == null)
            {
                manager.NetworkConfig = new();
                dirty = true;
            }
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
            // Ensure the Kaiju Multiplayer Manager is added.
            if (Object.FindAnyObjectByType<KaijuMultiplayerManager>() == null)
            {
                GameObject go = new("Kaiju Multiplayer Manager")
                {
                    isStatic = true
                };
                go.AddComponent<KaijuMultiplayerManager>();
                go.transform.SetSiblingIndex(0);
                dirty = true;
            }
            
            // Ensure a Kaiju Transport is added and assigned.
            KaijuTransport transport = manager.GetComponent<KaijuTransport>();
            if (transport == null)
            {
                transport = manager.gameObject.AddComponent<KaijuTransport>();
                dirty = true;
            }
            
            if (manager.NetworkConfig.NetworkTransport != transport)
            {
                manager.NetworkConfig.NetworkTransport = transport;
                dirty = true;
            }
            
            // Remove the standard Unity transport.
            UnityTransport utp = manager.GetComponent<UnityTransport>();
            if (utp != null)
            {
                Object.DestroyImmediate(utp);
                dirty = true;
            }
#else
            // Remove missing components.
            foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (root.name == "Kaiju Multiplayer Manager")
                {
                    Object.DestroyImmediate(root);
                    continue;
                }
                
                // Get all child transforms, including inactive ones.
                foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
                {
                    // If something is removed, mark that this is dirty.
                    if (GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject) > 0)
                    {
                        dirty = true;
                    }
                }
            }
            
            // Ensure a Unity Transport is added and assigned.
            UnityTransport transport = manager.GetComponent<UnityTransport>();
            if (transport == null)
            {
                transport = manager.gameObject.AddComponent<UnityTransport>();
                dirty = true;
            }
            
            if (manager.NetworkConfig.NetworkTransport != transport)
            {
                manager.NetworkConfig.NetworkTransport = transport;
                dirty = true;
            }
#endif
            // Save the scene if there are changes.
            if (dirty)
            {
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), scenePath);
            }
        }
        // Get the player prefab.
        const string prefabPath = "Assets/Player.prefab";
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (playerPrefab == null)
        {
            return;
        }
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
        // Ensure the "KaijuUser" and "KaijuUserLink" components are on the player prefab.
        bool needsKaijuUser = playerPrefab.GetComponent<KaijuUser>() == null;
        bool needsKaijuUserLink = playerPrefab.GetComponent<KaijuUserLink>() == null;
        if (!needsKaijuUser && !needsKaijuUserLink)
        {
            return;
        }
        
        using PrefabUtility.EditPrefabContentsScope editingScope = new(prefabPath);
        GameObject prefabRoot = editingScope.prefabContentsRoot;
        
        if (needsKaijuUser)
        {
            prefabRoot.AddComponent<KaijuUser>();
        }
        
        if (needsKaijuUserLink)
        {
            prefabRoot.AddComponent<KaijuUserLink>();
        }
#else
        // Remove missing components from the prefab.
        using PrefabUtility.EditPrefabContentsScope editingScope = new(prefabPath);
        foreach (Transform t in editingScope.prefabContentsRoot.GetComponentsInChildren<Transform>(true))
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
        }
#endif
    }
}
#endif