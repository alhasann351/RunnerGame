/*using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SimpleMeshCombiner : MonoBehaviour
{
    [Header("Source parent যেই group combine করবেন")]
    public Transform sourceParent;

    [Header("Combine করার পর original renderer off হবে")]
    public bool disableOriginalRenderers = true;

    [ContextMenu("Combine Meshes")]
    public void CombineMeshes()
    {
        if (sourceParent == null)
        {
            Debug.LogError("Source Parent set করা হয়নি!");
            return;
        }

        MeshFilter[] meshFilters = sourceParent.GetComponentsInChildren<MeshFilter>();

        List<CombineInstance> combineInstances = new List<CombineInstance>();
        Material firstMaterial = null;

        foreach (MeshFilter meshFilter in meshFilters)
        {
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

            if (meshRenderer == null)
                continue;

            if (meshFilter.sharedMesh == null)
                continue;

            if (firstMaterial == null)
                firstMaterial = meshRenderer.sharedMaterial;

            if (meshRenderer.sharedMaterial != firstMaterial)
            {
                Debug.LogWarning(meshFilter.name + " skipped কারণ material আলাদা");
                continue;
            }

            CombineInstance combineInstance = new CombineInstance();
            combineInstance.mesh = meshFilter.sharedMesh;
            combineInstance.transform = transform.worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;

            combineInstances.Add(combineInstance);

            if (disableOriginalRenderers)
            {
                meshRenderer.enabled = false;
            }
        }

        if (combineInstances.Count == 0)
        {
            Debug.LogError("Combine করার মতো mesh পাওয়া যায়নি!");
            return;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.name = sourceParent.name + "_CombinedMesh";

        combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

        MeshFilter myMeshFilter = GetComponent<MeshFilter>();
        MeshRenderer myMeshRenderer = GetComponent<MeshRenderer>();

        myMeshFilter.sharedMesh = combinedMesh;
        myMeshRenderer.sharedMaterial = firstMaterial;

        Debug.Log("Combined " + combineInstances.Count + " meshes from " + sourceParent.name);
    }
}*/

using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SimpleMeshCombiner : MonoBehaviour
{
    public Transform sourceParent;
    public bool disableOriginalRenderers = true;

    [ContextMenu("Combine Meshes")]
    public void CombineMeshes()
    {
        if (sourceParent == null)
        {
            Debug.LogError("Source Parent set করা হয়নি!");
            return;
        }

        MeshFilter[] meshFilters = sourceParent.GetComponentsInChildren<MeshFilter>();

        List<CombineInstance> combineInstances = new List<CombineInstance>();
        Material firstMaterial = null;

        foreach (MeshFilter meshFilter in meshFilters)
        {
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

            if (meshRenderer == null || meshFilter.sharedMesh == null)
                continue;

            if (firstMaterial == null)
                firstMaterial = meshRenderer.sharedMaterial;

            if (meshRenderer.sharedMaterial != firstMaterial)
            {
                Debug.LogWarning(meshFilter.name + " skipped কারণ material আলাদা");
                continue;
            }

            CombineInstance combineInstance = new CombineInstance();
            combineInstance.mesh = meshFilter.sharedMesh;
            combineInstance.transform = transform.worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;

            combineInstances.Add(combineInstance);

            if (disableOriginalRenderers)
                meshRenderer.enabled = false;
        }

        if (combineInstances.Count == 0)
        {
            Debug.LogError("Combine করার মতো mesh পাওয়া যায়নি!");
            return;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.name = sourceParent.name + "_CombinedMesh";

        combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);
        combinedMesh.RecalculateBounds();

        MeshFilter myMeshFilter = GetComponent<MeshFilter>();
        MeshRenderer myMeshRenderer = GetComponent<MeshRenderer>();

        myMeshFilter.sharedMesh = combinedMesh;
        myMeshRenderer.sharedMaterial = firstMaterial;

#if UNITY_EDITOR
        SaveMeshAsAsset(combinedMesh);
#endif

        Debug.Log("Combined " + combineInstances.Count + " meshes from " + sourceParent.name);
    }

#if UNITY_EDITOR
    private void SaveMeshAsAsset(Mesh mesh)
    {
        string folderPath = "Assets/RunnerGame/Art/CombinedMeshes";

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/RunnerGame/Art", "CombinedMeshes");
        }

        string meshPath = folderPath + "/" + gameObject.name + "_Mesh.asset";
        meshPath = AssetDatabase.GenerateUniqueAssetPath(meshPath);

        AssetDatabase.CreateAsset(mesh, meshPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        GetComponent<MeshFilter>().sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);

        Debug.Log("Combined mesh saved: " + meshPath);
    }
#endif
}