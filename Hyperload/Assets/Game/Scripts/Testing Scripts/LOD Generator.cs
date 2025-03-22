using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;

[RequireComponent(typeof(LODGroup))]
public class AutomaticLODGenerator : MonoBehaviour
{
    [Header("LOD Settings")]
    [Range(2, 4)] public int lodLevels = 3; // Number of LOD levels
    [Range(0.2f, 0.8f)] public float reductionFactor = 0.5f; // How much each LOD is simplified
    public float[] lodThresholds = { 0.6f, 0.3f, 0.1f }; // LOD distances

    [Header("Save Settings")]
    public string savePath = "Assets/GeneratedLODs/"; // Where to save generated meshes
    public bool overwriteExisting = true; // Overwrite existing LOD meshes

    public enum SimplificationMethod { UnityOptimize, ThirdParty }
    [Header("Simplification Options")]
    public SimplificationMethod simplificationMethod = SimplificationMethod.UnityOptimize;

    public void GenerateLODs()
    {
        LODGroup lodGroup = GetComponent<LODGroup>();

        if (!lodGroup)
        {
            Debug.LogError("LODGroup component not found! Adding one now...");
            lodGroup = gameObject.AddComponent<LODGroup>();
        }

        List<LOD> lods = new List<LOD>();
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        if (meshFilters.Length == 0)
        {
            Debug.LogError("No meshes found in children!");
            return;
        }

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        for (int i = 0; i < lodLevels; i++)
        {
            float quality = Mathf.Pow(reductionFactor, i); // Reduce polycount per LOD
            List<Renderer> renderers = new List<Renderer>();

            foreach (MeshFilter mf in meshFilters)
            {
                Mesh newMesh = SimplifyMesh(mf.sharedMesh, quality, i);

                if (newMesh != null)
                {
                    GameObject lodObject = Instantiate(mf.gameObject, mf.transform.position, mf.transform.rotation, transform);
                    lodObject.name = mf.gameObject.name + "_LOD" + i;
                    MeshFilter newMeshFilter = lodObject.GetComponent<MeshFilter>();
                    newMeshFilter.sharedMesh = newMesh;

                    Renderer rend = lodObject.GetComponent<Renderer>();
                    if (rend)
                    {
                        renderers.Add(rend);
                    }
                }
            }

            lods.Add(new LOD(lodThresholds[i], renderers.ToArray()));
        }

        lodGroup.SetLODs(lods.ToArray());
        lodGroup.RecalculateBounds();

        Debug.Log("LOD Generation Complete!");
    }

    private Mesh SimplifyMesh(Mesh originalMesh, float quality, int lodLevel)
    {
        if (originalMesh == null)
        {
            Debug.LogWarning("Mesh is null, skipping simplification.");
            return null;
        }

        Mesh newMesh = Instantiate(originalMesh);

        if (simplificationMethod == SimplificationMethod.UnityOptimize)
        {
            UnityEditor.MeshUtility.Optimize(newMesh);
        }
        else
        {
            Debug.LogWarning("Third-party simplification required!");
            // Here you can integrate Simplygon, InstaLOD, or another mesh reduction tool.
        }

        SaveMeshToDisk(newMesh, originalMesh.name + "_LOD" + lodLevel);
        return newMesh;
    }

    private void SaveMeshToDisk(Mesh mesh, string fileName)
    {
        string path = savePath + fileName + ".asset";

        if (File.Exists(path) && !overwriteExisting)
        {
            Debug.LogWarning($"Mesh {fileName} already exists, skipping save.");
            return;
        }

        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
        Debug.Log($"Saved {fileName} to {path}");
    }
}
