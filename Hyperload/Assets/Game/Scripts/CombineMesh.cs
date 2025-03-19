using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class AdvancedMeshCombiner : MonoBehaviour
{
    [Header("Settings")]
    public string savePath = "Assets/MergedMeshes";
    public bool destroyOriginals = false;

    [ContextMenu("Combine and Save Mesh")]
    public void CombineAndSaveMesh()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length == 0)
        {
            Debug.LogError("No Mesh Filters found!");
            return;
        }

        Dictionary<Material, List<CombineInstance>> materialToMeshData = new Dictionary<Material, List<CombineInstance>>();

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.sharedMesh == null || mf.GetComponent<Renderer>() == null)
                continue;

            Material mat = mf.GetComponent<Renderer>().sharedMaterial;
            if (!materialToMeshData.ContainsKey(mat))
                materialToMeshData[mat] = new List<CombineInstance>();

            CombineInstance combineInstance = new CombineInstance
            {
                mesh = mf.sharedMesh,
                transform = mf.transform.localToWorldMatrix
            };

            materialToMeshData[mat].Add(combineInstance);
        }

        GameObject mergedObject = new GameObject("MergedMesh");
        MeshFilter mergedMeshFilter = mergedObject.AddComponent<MeshFilter>();
        MeshRenderer mergedRenderer = mergedObject.AddComponent<MeshRenderer>();

        Mesh finalMesh = new Mesh();
        List<Material> materials = new List<Material>();
        List<CombineInstance> finalCombineInstances = new List<CombineInstance>();

        foreach (var entry in materialToMeshData)
        {
            Mesh subMesh = new Mesh();
            subMesh.CombineMeshes(entry.Value.ToArray(), true, true);
            materials.Add(entry.Key);

            CombineInstance ci = new CombineInstance
            {
                mesh = subMesh,
                transform = Matrix4x4.identity
            };
            finalCombineInstances.Add(ci);
        }

        finalMesh.CombineMeshes(finalCombineInstances.ToArray(), false, false);
        mergedMeshFilter.sharedMesh = finalMesh;
        mergedRenderer.sharedMaterials = materials.ToArray();

        SaveMesh(finalMesh, "MergedMesh");

        if (destroyOriginals)
        {
            foreach (var mf in meshFilters)
            {
                DestroyImmediate(mf.gameObject);
            }
        }

        Debug.Log("Mesh merging completed with materials preserved!");
    }

    private void SaveMesh(Mesh mesh, string meshName)
    {
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        string assetPath = $"{savePath}/{meshName}.asset";
        AssetDatabase.CreateAsset(mesh, assetPath);
        AssetDatabase.SaveAssets();
        Debug.Log($"Mesh saved at: {assetPath}");
    }
}
