using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    void Start()
    {
        // Get all MeshFilters in children (but not this object’s own one)
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length - 1];

        int index = 0;
        for (int i = 0; i < meshFilters.Length; i++)
        {
            // Skip the MeshFilter on the parent itself
            if (meshFilters[i].gameObject == gameObject)
                continue;

            combine[index].mesh = meshFilters[i].sharedMesh;
            combine[index].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);   // hide original piece
            index++;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine, true, true);

        // Put the new mesh on the parent
        MeshFilter parentFilter = GetComponent<MeshFilter>();
        parentFilter.mesh = combinedMesh;
    }
}
