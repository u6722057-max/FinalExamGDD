using UnityEditor;
using UnityEngine;

/// <summary>Upgrades the supplied Built-in Render Pipeline materials for this URP project.</summary>
public static class ThreeInOneMaterialRepair
{
    [MenuItem("Assignment/Repair Legacy Materials")]
    public static void Repair()
    {
        Shader lit = Shader.Find("Universal Render Pipeline/Lit");
        if (lit == null)
        {
            Debug.LogError("URP Lit shader is unavailable; legacy materials were not changed.");
            return;
        }

        int repaired = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:Material", new[] { "Assets" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null || material.shader == null || material.shader.name != "Standard") continue;

            Texture mainTexture = material.GetTexture("_MainTex");
            Vector2 scale = material.GetTextureScale("_MainTex");
            Vector2 offset = material.GetTextureOffset("_MainTex");
            Color color = material.GetColor("_Color");
            float smoothness = material.GetFloat("_Glossiness");
            float metallic = material.GetFloat("_Metallic");
            float mode = material.GetFloat("_Mode");
            Texture normal = material.GetTexture("_BumpMap");

            material.shader = lit;
            material.SetTexture("_BaseMap", mainTexture);
            material.SetTextureScale("_BaseMap", scale);
            material.SetTextureOffset("_BaseMap", offset);
            material.SetColor("_BaseColor", color);
            material.SetFloat("_Smoothness", smoothness);
            material.SetFloat("_Metallic", metallic);
            if (normal != null)
            {
                material.SetTexture("_BumpMap", normal);
                material.EnableKeyword("_NORMALMAP");
            }

            if (mode >= 2f)
            {
                material.SetFloat("_Surface", 1f);
                material.SetFloat("_Blend", mode >= 3f ? 0f : 1f);
                material.SetOverrideTag("RenderType", "Transparent");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }

            EditorUtility.SetDirty(material);
            repaired++;
        }

        if (repaired > 0)
        {
            AssetDatabase.SaveAssets();
            Debug.Log($"Converted {repaired} legacy Standard materials to URP Lit.");
        }
    }
}
