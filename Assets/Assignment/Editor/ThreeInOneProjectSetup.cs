using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ThreeInOneProjectSetup
{
    private const string SetupPreferencePrefix = "MyLittleKSetupComplete_";

    [InitializeOnLoadMethod]
    private static void SetupOnFirstOpen()
    {
        string preferenceKey = SetupPreferencePrefix + Application.dataPath;
        if (EditorPrefs.GetBool(preferenceKey, false)) return;

        EditorApplication.delayCall += () =>
        {
            if (EditorPrefs.GetBool(preferenceKey, false)) return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += () => SetupOnFirstOpen();
                return;
            }

            BuildProject();
            EditorPrefs.SetBool(preferenceKey, true);
        };
    }

    [MenuItem("Assignment/Build Main Menu and Scene List")]
    public static void BuildProject()
    {
        const string sceneFolder = "Assets/Scenes";
        if (!AssetDatabase.IsValidFolder(sceneFolder)) AssetDatabase.CreateFolder("Assets", "Scenes");

        BuildDrivingScene();

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        scene.name = "MainMenu";
        EditorSceneManager.SaveScene(scene, sceneFolder + "/MainMenu.unity");

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(sceneFolder + "/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Prototype 1.unity", true),
            new EditorBuildSettingsScene("Assets/Challenge 1/Challenge 1.unity", true),
            new EditorBuildSettingsScene("Assets/Challenge 4/Challenge 4.unity", true),
            new EditorBuildSettingsScene("Assets/Challenge 2/Challenge 2.unity", true),
            new EditorBuildSettingsScene("Assets/Challenge 3/Challenge 3.unity", true),
            new EditorBuildSettingsScene("Assets/Challenge 5/Challenge 5.unity", true)
        };
        AssetDatabase.SaveAssets();
        EditorSceneManager.OpenScene(sceneFolder + "/MainMenu.unity", OpenSceneMode.Single);
        Debug.Log("Three-in-One project scenes and build order are ready.");
    }

    private static void BuildDrivingScene()
    {
        const string scenePath = "Assets/Scenes/Prototype 1.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        GameObject vehicle = GameObject.Find("Vehicle");

        if (vehicle == null)
        {
            var carPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Course Library/Vehicles/Veh_Ute_Red_Z.prefab");
            if (carPrefab == null)
            {
                Debug.LogError("Driving car prefab is missing from the Prototype 1 package.");
                return;
            }

            vehicle = (GameObject)PrefabUtility.InstantiatePrefab(carPrefab);
            vehicle.name = "Vehicle";
            vehicle.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            var collider = vehicle.AddComponent<BoxCollider>();
            collider.center = new Vector3(0f, .6f, 0f);
            collider.size = new Vector3(2.5f, 1.2f, 4.5f);
            vehicle.AddComponent<DrivingPlayerController>();
        }

        var cameraObject = GameObject.FindWithTag("MainCamera");
        if (cameraObject != null)
        {
            var follow = cameraObject.GetComponent<DrivingCameraFollow>();
            if (follow == null) follow = cameraObject.AddComponent<DrivingCameraFollow>();
            follow.SetTarget(vehicle.transform);
        }

        string[] obstaclePaths = { "Assets/Course Library/Obstacles/Crate_01.prefab" };
        if (GameObject.Find("Driving Obstacles") == null)
        {
            var group = new GameObject("Driving Obstacles");
            for (int i = 0; i < obstaclePaths.Length; i++)
            {
                var obstaclePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(obstaclePaths[i]);
                if (obstaclePrefab == null) continue;
                var obstacle = (GameObject)PrefabUtility.InstantiatePrefab(obstaclePrefab);
                obstacle.transform.SetParent(group.transform);
                obstacle.name = "Obstacle";
                obstacle.transform.SetPositionAndRotation(new Vector3(0f, 0f, 25f), Quaternion.identity);
            }
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }
}
