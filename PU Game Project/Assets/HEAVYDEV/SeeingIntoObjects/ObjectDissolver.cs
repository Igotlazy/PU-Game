using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDissolver : MonoBehaviour
{
    public MeshRenderer meshRend;
    private Material material;
    private float dissolveRate = 4f;

    private void Start()
    {
        meshRend = GetComponent<MeshRenderer>();
        material = meshRend.materials[0];
    }

    public void CallDissolveMesh()
    {
        Debug.Log("Dissolve Mesh");
        StopCoroutine("ReformMesh");
        StartCoroutine("DissolveMesh");
    }

    public void CallReformMesh()
    {
        Debug.Log("Reform Mesh");
        StopCoroutine("DissolveMesh");
        StartCoroutine("ReformMesh");
    }

    IEnumerator ReformMesh()
    {
        float currentDissolve = material.GetFloat("_dissolveProgression");
        while(currentDissolve > -1.1f)
        {
            currentDissolve -= dissolveRate * Time.deltaTime;
            material.SetFloat("_dissolveProgression", currentDissolve);
            yield return null;
        }
        material.SetFloat("_dissolveProgression", -1.1f);
    }

    IEnumerator DissolveMesh()
    {
        float currentDissolve = material.GetFloat("_dissolveProgression");
        while (currentDissolve < 0.5f)
        {
            currentDissolve += dissolveRate * Time.deltaTime;
            material.SetFloat("_dissolveProgression", currentDissolve);
            yield return null;
        }
        material.SetFloat("_dissolveProgression", 0.5f);
    }
}
