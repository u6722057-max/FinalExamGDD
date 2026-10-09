using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

[InitializeOnLoad]
public static class ProjectVisualCheck
{
    const string Request = "/tmp/three-in-one-visual-check.request";
    static ProjectVisualCheck() { EditorApplication.update += Tick; }
    static void Tick()
    {
        if (!File.Exists(Request) || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        if (EditorApplication.isPlayingOrWillChangePlaymode) { EditorApplication.isPlaying = false; return; }
        File.Move(Request, Request + ".processed-" + DateTime.Now.Ticks);
        try { Run(); } catch (Exception e) { File.WriteAllText("/tmp/three-in-one-visual-report.txt", e.ToString()); Debug.LogException(e); }
    }
    [MenuItem("Assignment/Check Game Visuals")]
    public static void Run()
    {
        ThreeInOneMaterialRepair.Repair();
        var report = new StringBuilder();
        report.AppendLine("Pipeline: " + GraphicsSettings.currentRenderPipeline);
        foreach (var guid in AssetDatabase.FindAssets("t:Material", new[]{"Assets"}))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null || mat.shader == null || !mat.shader.isSupported || mat.shader.name.Contains("Error"))
                report.AppendLine("BAD MATERIAL: " + path + " shader=" + (mat == null ? "null" : mat.shader));
        }
        foreach(var guid in AssetDatabase.FindAssets("t:Model", new[]{"Assets"}))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if(importer != null && importer.importTangents == ModelImporterTangents.Import)
            { importer.importTangents = ModelImporterTangents.CalculateMikk; importer.SaveAndReimport(); }
        }
        var setup = EditorSceneManager.GetSceneManagerSetup();
        for(int i=0;i<UnityEngine.SceneManagement.SceneManager.sceneCount;i++)
            if(UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).isDirty)
            { report.AppendLine("Render check deferred: unsaved scene."); File.WriteAllText("/tmp/three-in-one-visual-report.txt",report.ToString());return; }
        Directory.CreateDirectory("/tmp/three-in-one-current-renders");
        try
        {
            foreach(var guid in AssetDatabase.FindAssets("t:Scene",new[]{"Assets"}))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                foreach(var root in scene.GetRootGameObjects())
                  foreach(var renderer in root.GetComponentsInChildren<Renderer>(true))
                    foreach(var mat in renderer.sharedMaterials)
                      if((mat == null && !(renderer is ParticleSystemRenderer)) || (mat != null && (mat.shader == null || !mat.shader.isSupported)))
                        report.AppendLine("BAD RENDERER: " + path + " / " + renderer.name);
                var cam = Camera.main;
                if(cam == null) {report.AppendLine("No main camera: "+path);continue;}
                var rt = new RenderTexture(960,540,24); var tex = new Texture2D(960,540,TextureFormat.RGB24,false);
                var oldTarget = cam.targetTexture; var oldActive = RenderTexture.active;
                cam.targetTexture=rt;cam.Render();RenderTexture.active=rt;
                tex.ReadPixels(new Rect(0,0,960,540),0,0);tex.Apply();
                File.WriteAllBytes("/tmp/three-in-one-current-renders/"+scene.name+".png",tex.EncodeToPNG());
                cam.targetTexture=oldTarget;RenderTexture.active=oldActive;
                UnityEngine.Object.DestroyImmediate(tex);UnityEngine.Object.DestroyImmediate(rt);
                report.AppendLine("Rendered: "+path);
            }
        }
        finally { EditorSceneManager.RestoreSceneManagerSetup(setup);File.WriteAllText("/tmp/three-in-one-visual-report.txt",report.ToString()); }
        Debug.Log("Game visual check complete. Report: /tmp/three-in-one-visual-report.txt");
    }
}
