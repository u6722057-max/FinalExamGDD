using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public static class Challenge3MaterialRepair
{
    private const string MaterialFolder = "Assets/Challenge 3/Materials";
    private const string ScenePath = "Assets/Challenge 3/Challenge 3.unity";

    [MenuItem("Tools/Challenge 3/Repair URP Materials")]
    public static void Repair()
    {
        Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
        if (litShader == null)
        {
            throw new System.InvalidOperationException("The URP Lit shader could not be found.");
        }

        EnsureFolderExists();

        Material balloonMaterial = CreateConvertedMaterial(
            "Balloon URP",
            "Assets/Course Library/_Source_Files/Materials/PolygonPrototype_Texture_01.mat",
            litShader);
        ApplyBalloonColor(balloonMaterial);
        Material bombMaterial = CreateConvertedMaterial(
            "Bomb URP",
            "Assets/Course Library/_Source_Files/Materials/PolygonCity_Mat_01_A.mat",
            litShader);
        Material moneyMaterial = CreateConvertedMaterial(
            "Money URP",
            "Assets/Course Library/_Source_Files/Materials/SimpleIcons.mat",
            litShader);
        Material groundMaterial = CreateGroundMaterial(litShader);

        AssignPrefabMaterial("Assets/Challenge 3/Prefabs/Player.prefab", balloonMaterial);
        AssignPrefabMaterial("Assets/Challenge 3/Prefabs/Bomb.prefab", bombMaterial);
        AssignPrefabMaterial("Assets/Challenge 3/Prefabs/Money.prefab", moneyMaterial);

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        AssignSceneMaterial(scene, "Player", balloonMaterial);
        AssignSceneMaterial(scene, "Ground", groundMaterial);
        EditorSceneManager.SaveScene(scene);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Challenge 3 URP materials repaired successfully.");
    }

    public static void CapturePreview()
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Camera camera = Camera.main;
        if (camera == null)
        {
            throw new System.InvalidOperationException("The challenge scene has no Main Camera.");
        }

        RenderTexture renderTexture = new RenderTexture(1280, 720, 24);
        Texture2D preview = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        RenderTexture previous = RenderTexture.active;

        camera.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;
        camera.Render();
        preview.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        preview.Apply();
        File.WriteAllBytes("/private/tmp/challenge3-fixed-preview.png", preview.EncodeToPNG());

        camera.targetTexture = null;
        RenderTexture.active = previous;
        Object.DestroyImmediate(preview);
        Object.DestroyImmediate(renderTexture);
        Debug.Log("Challenge 3 preview captured successfully.");
    }

    private static void EnsureFolderExists()
    {
        if (!AssetDatabase.IsValidFolder(MaterialFolder))
        {
            AssetDatabase.CreateFolder("Assets/Challenge 3", "Materials");
        }
    }

    private static Material CreateConvertedMaterial(string name, string sourcePath, Shader shader)
    {
        string targetPath = $"{MaterialFolder}/{name}.mat";
        Material source = AssetDatabase.LoadAssetAtPath<Material>(sourcePath);
        Material target = AssetDatabase.LoadAssetAtPath<Material>(targetPath);

        if (source == null)
        {
            throw new System.InvalidOperationException($"Missing source material: {sourcePath}");
        }

        if (target == null)
        {
            target = new Material(shader) { name = name };
            AssetDatabase.CreateAsset(target, targetPath);
        }
        else
        {
            target.shader = shader;
        }

        Texture mainTexture = source.GetTexture("_MainTex");
        Vector2 textureScale = source.GetTextureScale("_MainTex");
        Vector2 textureOffset = source.GetTextureOffset("_MainTex");
        Color baseColor = source.HasProperty("_Color") ? source.GetColor("_Color") : Color.white;

        target.SetTexture("_BaseMap", mainTexture);
        target.SetTextureScale("_BaseMap", textureScale);
        target.SetTextureOffset("_BaseMap", textureOffset);
        target.SetTexture("_MainTex", mainTexture);
        target.SetTextureScale("_MainTex", textureScale);
        target.SetTextureOffset("_MainTex", textureOffset);
        target.SetColor("_BaseColor", baseColor);
        target.SetColor("_Color", baseColor);
        target.SetFloat("_Smoothness", source.HasProperty("_Glossiness") ? source.GetFloat("_Glossiness") : 0.2f);

        Texture normalMap = source.HasProperty("_BumpMap") ? source.GetTexture("_BumpMap") : null;
        if (normalMap != null)
        {
            target.SetTexture("_BumpMap", normalMap);
            target.EnableKeyword("_NORMALMAP");
        }

        Texture metallicMap = source.HasProperty("_MetallicGlossMap") ? source.GetTexture("_MetallicGlossMap") : null;
        if (metallicMap != null)
        {
            target.SetTexture("_MetallicGlossMap", metallicMap);
            target.EnableKeyword("_METALLICSPECGLOSSMAP");
        }

        EditorUtility.SetDirty(target);
        return target;
    }

    private static Material CreateGroundMaterial(Shader shader)
    {
        string targetPath = $"{MaterialFolder}/Ground URP.mat";
        Material target = AssetDatabase.LoadAssetAtPath<Material>(targetPath);

        if (target == null)
        {
            target = new Material(shader) { name = "Ground URP" };
            AssetDatabase.CreateAsset(target, targetPath);
        }
        else
        {
            target.shader = shader;
        }

        Color groundColor = new Color(0.32f, 0.32f, 0.32f, 1.0f);
        target.SetColor("_BaseColor", groundColor);
        target.SetColor("_Color", groundColor);
        target.SetFloat("_Smoothness", 0.15f);
        EditorUtility.SetDirty(target);
        return target;
    }

    private static void ApplyBalloonColor(Material material)
    {
        Shader unlitShader = Shader.Find("Universal Render Pipeline/Unlit");
        if (unlitShader == null)
        {
            throw new System.InvalidOperationException("The URP Unlit shader could not be found.");
        }

        Color balloonColor = new Color(0.75f, 0.15f, 0.08f, 1.0f);
        material.shader = unlitShader;
        material.SetTexture("_BaseMap", null);
        material.SetTexture("_MainTex", null);
        material.SetColor("_BaseColor", balloonColor);
        material.SetColor("_Color", balloonColor);
        material.SetFloat("_Smoothness", 0.25f);
        EditorUtility.SetDirty(material);
    }

    private static void AssignPrefabMaterial(string prefabPath, Material material)
    {
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
        MeshRenderer renderer = prefabRoot.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            PrefabUtility.UnloadPrefabContents(prefabRoot);
            throw new System.InvalidOperationException($"Prefab has no MeshRenderer: {prefabPath}");
        }

        renderer.sharedMaterial = material;
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
    }

    private static void AssignSceneMaterial(Scene scene, string objectName, Material material)
    {
        GameObject target = null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == objectName)
            {
                target = root;
                break;
            }
        }

        MeshRenderer renderer = target != null ? target.GetComponent<MeshRenderer>() : null;
        if (renderer == null)
        {
            throw new System.InvalidOperationException($"Scene object has no MeshRenderer: {objectName}");
        }

        renderer.sharedMaterial = material;
    }
}
