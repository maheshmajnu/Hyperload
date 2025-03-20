using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class MeshCombiner : MonoBehaviour
{
    public string savePath = "Assets/CombinedMeshes"; // Path where mesh will be saved

    [ContextMenu("Combine Meshes")]
    public void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        foreach (var mf in meshFilters)
        {
            if (mf.sharedMesh == null) continue;

            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            ci.transform = mf.transform.localToWorldMatrix;
            combineInstances.Add(ci);
        }

        if (combineInstances.Count == 0)
        {
            Debug.LogError("No meshes found under the selected object!");
            return;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

        // Assign the combined mesh to the parent
        MeshFilter mfCombined = gameObject.AddComponent<MeshFilter>();
        MeshRenderer mrCombined = gameObject.AddComponent<MeshRenderer>();
        mfCombined.mesh = combinedMesh;

        // Save the mesh
        SaveMesh(combinedMesh, "CombinedMesh");

        Debug.Log("Combined Mesh created and assigned!");
    }

    private void SaveMesh(Mesh mesh, string name)
    {
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        string fullPath = Path.Combine(savePath, name + ".asset");
        AssetDatabase.CreateAsset(mesh, fullPath);
        AssetDatabase.SaveAssets();
        Debug.Log("Mesh saved at: " + fullPath);
    }
}
