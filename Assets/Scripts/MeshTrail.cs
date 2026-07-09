using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour {

    [SerializeField] private float activeTime = 0.2f;
    [SerializeField] private float meshRefreshRate = 0.05f;
    [SerializeField] private float meshDestroyDelay = 0.5f;
    [SerializeField] private Transform positionToSpawn;
    [SerializeField] private Mesh bodyMesh;
    [SerializeField] private Material trailMat;
    [SerializeField] private string shaderVarRef;
    [SerializeField] private float shaderVarRate = 0.05f;
    [SerializeField] private float shaderVarRefreshRate = 0.025f;

    private bool isTrailActive;
    private MeshRenderer[] meshRenderers;
    private MeshFilter[] meshFilters;
    private Mesh combinedMesh;

    private void Start() {
        combinedMesh = new Mesh();
        List<CombineInstance> instances = new List<CombineInstance>();
        for (int i = 0; i < bodyMesh.subMeshCount; i++) {
            CombineInstance ci = new CombineInstance();
            ci.mesh = bodyMesh;
            ci.subMeshIndex = i;
            ci.transform = Matrix4x4.identity;
            instances.Add(ci);
        }
        combinedMesh.CombineMeshes(instances.ToArray(), true);
        //combinedMesh = bodyMesh;
    }


    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && !isTrailActive){
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));
        }
    }

    IEnumerator ActivateTrail(float timeActive) {
        while(timeActive > 0) {
            timeActive -= meshRefreshRate;

            if (meshRenderers == null && meshFilters == null) {
                meshRenderers = GetComponentsInChildren<MeshRenderer>();
                meshFilters = GetComponentsInChildren<MeshFilter>();
            }

            for(int i=0; i<meshRenderers.Length; i++) {
                GameObject gobj = new GameObject();
                gobj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);
                gobj.transform.localScale = Vector3.one * 100;

                MeshRenderer mr = gobj.AddComponent<MeshRenderer>();
                MeshFilter mf = gobj.AddComponent<MeshFilter>();


                mf.mesh = combinedMesh;
                mr.material = trailMat;

                StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));

                Destroy(gobj, meshDestroyDelay);
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }

        isTrailActive = false;
    }

    IEnumerator AnimateMaterialFloat(Material mat, float goal, float rate, float refreshRate) {
        float valueToAnimate = mat.GetFloat(shaderVarRef);

        while(valueToAnimate > goal) {
            valueToAnimate -= rate;
            mat.SetFloat(shaderVarRef, valueToAnimate);

            yield return new WaitForSeconds(refreshRate);
        }
    }

}
