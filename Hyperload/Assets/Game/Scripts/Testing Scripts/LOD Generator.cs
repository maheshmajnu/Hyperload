using UnityEngine;
using System.Collections.Generic;

public class AutoLODSetup : MonoBehaviour
{
    [ContextMenu("Setup LODs Automatically")]
    void SetupLODs()
    {
        // Ensure an LODGroup component exists
        LODGroup lodGroup = GetComponent<LODGroup>();
        if (lodGroup == null)
        {
            lodGroup = gameObject.AddComponent<LODGroup>();
        }

        // Find all child objects with a MeshRenderer or SkinnedMeshRenderer
        List<Transform> lodObjects = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.GetComponent<MeshRenderer>() || child.GetComponent<SkinnedMeshRenderer>())
            {
                lodObjects.Add(child);
            }
        }

        // If no valid LOD objects found, exit
        if (lodObjects.Count == 0)
        {
            Debug.LogWarning("No valid LOD objects found under " + gameObject.name);
            return;
        }

        // Sort objects based on polycount (highest polycount first)
        lodObjects.Sort((a, b) => GetPolyCount(b).CompareTo(GetPolyCount(a)));

        // Rename objects properly to LOD0, LOD1, LOD2...
        for (int i = 0; i < lodObjects.Count; i++)
        {
            lodObjects[i].name = "LOD" + i;
        }

        // Create LODs dynamically
        LOD[] lods = new LOD[lodObjects.Count];
        for (int i = 0; i < lodObjects.Count; i++)
        {
            Renderer[] renderers = lodObjects[i].GetComponentsInChildren<Renderer>();
            float transition = Mathf.Clamp01(1.0f - (i * 0.3f)); // Adjust transition values
            lods[i] = new LOD(transition, renderers);
        }

        // Assign LODs to LODGroup
        lodGroup.SetLODs(lods);
        lodGroup.RecalculateBounds();

        Debug.Log("LOD Group successfully set up for " + gameObject.name);
    }

    // Function to get polygon count of a mesh
    int GetPolyCount(Transform obj)
    {
        MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
        SkinnedMeshRenderer skinnedMesh = obj.GetComponent<SkinnedMeshRenderer>();

        if (meshFilter && meshFilter.sharedMesh) return meshFilter.sharedMesh.triangles.Length / 3;
        if (skinnedMesh && skinnedMesh.sharedMesh) return skinnedMesh.sharedMesh.triangles.Length / 3;

        return 0;
    }
}
