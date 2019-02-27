using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDissolver : MonoBehaviour
{
    public MeshRenderer meshRend;
    private Material material;
    private float dissolveRate = 5f;
    public bool amDissolved;
    private bool faderLocked;
    public bool FaderLocked
    {
        set
        {
            if(value == true)
            {
                faderLocked = true;
                CallDissolveMesh();
            }
            else
            {
                faderLocked = false;
                CallReformMesh();
            }
            //faderLocked = value;
        }
        get
        {
            return faderLocked;
        }
    }


    private LayerMask originalLayerMask;

    private void Start()
    {
        originalLayerMask = gameObject.layer;
        meshRend = GetComponent<MeshRenderer>();
        material = meshRend.materials[0];
    }

    public void CallDissolveMesh()
    {
        if (!amDissolved)
        {
            StopCoroutine("ReformMesh");
            StartCoroutine("DissolveMesh");
        }
    }

    public void CallReformMesh()
    {
        if (amDissolved && !faderLocked)
        {
            StopCoroutine("DissolveMesh");
            StartCoroutine("ReformMesh");
        }
    }

    IEnumerator ReformMesh()
    {
        amDissolved = false;
        gameObject.layer = originalLayerMask;

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
        amDissolved = true;
        gameObject.layer = LayerMask.NameToLayer("GameTerrainFade");

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
