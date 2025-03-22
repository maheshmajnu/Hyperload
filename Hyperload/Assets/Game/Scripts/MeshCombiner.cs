using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour
{
    public bool deleteOriginals = false; // Option to delete original meshes

    [ContextMenu("Combine Meshes")]
    public void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(); // Get all child MeshFilters
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        if (meshFilters.Length == 0)
        {
            Debug.LogWarning("No meshes found to combine!");
            return;
        }

        Material material = null; // To store the first material

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.sharedMesh != null && mf.gameObject != gameObject) // Ignore parent
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = mf.sharedMesh;
                ci.transform = mf.transform.localToWorldMatrix;
                combineInstances.Add(ci);

                if (material == null) // Assign the first available material
                    material = mf.GetComponent<MeshRenderer>().sharedMaterial;
            }
        }

        if (combineInstances.Count == 0)
        {
            Debug.LogWarning("No valid meshes to combine!");
            return;
        }

        // Create combined mesh
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);
        combinedMesh.name = "Combined_Mesh";

        // Assign mesh to this GameObject
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = combinedMesh;

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;

        // Delete originals if the option is checked
        if (deleteOriginals)
        {
            foreach (MeshFilter mf in meshFilters)
            {
                if (mf.gameObject != gameObject)
                    DestroyImmediate(mf.gameObject);
            }
        }

        Debug.Log("Meshes combined successfully!");
    }

    [ContextMenu("Save Mesh")]
    public void SaveMesh()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null)
        {
            Debug.LogWarning("No combined mesh to save!");
            return;
        }

        string path = EditorUtility.SaveFilePanelInProject("Save Combined Mesh", mf.sharedMesh.name, "asset", "Select save location");
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("Save operation canceled!");
            return;
        }

        Mesh newMesh = Instantiate(mf.sharedMesh); // Create a copy of the mesh before saving
        AssetDatabase.CreateAsset(newMesh, path);
        AssetDatabase.SaveAssets();

        Debug.Log("Mesh saved at: " + path);
    }
}
