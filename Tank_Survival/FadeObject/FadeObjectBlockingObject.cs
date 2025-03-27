// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class FadeObjectBlockingObject : MonoBehaviour
{
    [SerializeField]
    private LayerMask   layermask;
    [SerializeField]
    private Transform   target;
    [SerializeField]
    private new Camera  camera;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float       fadedAlpha;
    [SerializeField]
    private bool        retainShadows;
    [SerializeField]
    private Vector3     targetPositionOffset;
    [SerializeField]
    private float       fadeSpeed;

    [Header("Read Only Data")]
    [SerializeField]
    private List<FadingObject>                  objectsBlockingView = new List<FadingObject>();
    private Dictionary<FadingObject, Coroutine> runningCoroutines   = new Dictionary<FadingObject, Coroutine>();

    private RaycastHit[] hits = new RaycastHit[10];

    private void Start()
    {
        StartCoroutine(CheckForObjects());
    }

    private IEnumerator CheckForObjects()
    {
        while (true)
        {
            int hits = Physics.RaycastNonAlloc(
                    camera.transform.position,
                    (target.transform.position + targetPositionOffset - camera.transform.position).normalized,
                    this.hits,
                    Vector3.Distance(camera.transform.position, target.transform.position + targetPositionOffset),
                    layermask
                );

            if(hits > 0)
            {
                for (int index = 0; index < hits; index++)
                {
                    FadingObject fadingObject = GetFadingObjectFromHit(this.hits[index]);
                    
                    if (fadingObject != null && !objectsBlockingView.Contains(fadingObject))
                    {
                        if(runningCoroutines.ContainsKey(fadingObject))
                        {
                            if (runningCoroutines[fadingObject] != null)
                            {
                                StopCoroutine(runningCoroutines[fadingObject]);
                            }

                            runningCoroutines.Remove(fadingObject);
                        }

                        runningCoroutines.Add(fadingObject, StartCoroutine(FadeObjectOut(fadingObject)));
                        objectsBlockingView.Add(fadingObject);
                    }
                }
            }

            FadeObjectsNoLongerBeingHit();

            ClearHits();

            yield return null; 
        }
    }

    private void FadeObjectsNoLongerBeingHit()
    {
        // Á¦°ÅÇÒ °´Ã¼
        List<FadingObject> objectsToRemove = new List<FadingObject>(objectsBlockingView.Count);

        foreach(FadingObject fadingObject in objectsBlockingView)
        {
            bool objectIsBeingHit = false;
            for(int index = 0; index < hits.Length; index++)
            {
                FadingObject hitFadingObject = GetFadingObjectFromHit(hits[index]);
                if(hitFadingObject != null && fadingObject == hitFadingObject)
                {
                    objectIsBeingHit = true;
                    break;
                }
            }

            if(!objectIsBeingHit)
            {
                if(runningCoroutines.ContainsKey(fadingObject))
                {
                    if (runningCoroutines[fadingObject] != null)
                    {
                        StopCoroutine(runningCoroutines[fadingObject]); 
                    }
                    runningCoroutines.Remove(fadingObject);
                }

                runningCoroutines.Add(fadingObject, StartCoroutine(FadeObjectIn(fadingObject)));
                objectsToRemove.Add(fadingObject);
            }
        }

        foreach (FadingObject removeObject in objectsToRemove)
        {
            objectsBlockingView.Remove(removeObject);
        }
    }

    private IEnumerator FadeObjectOut(FadingObject fadingObject)
    {
        foreach(Material material in fadingObject.materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcColor);
            material.SetInt("_ZWrite", 0);
            material.SetInt("_Surface", 1);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            material.SetShaderPassEnabled("DepthOnly", false);
            material.SetShaderPassEnabled("SHADOWCASTER", retainShadows);

            material.SetOverrideTag("RenderType", "Transparent");

            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        float time = 0.0f;

        while (fadingObject.materials[0].color.a > fadedAlpha)
        {
            foreach(Material material in fadingObject.materials)
            {
                if(material.HasProperty("_BaseColor"))
                {
                    material.color = new Color(
                        material.color.r,
                        material.color.g,
                        material.color.b,
                        Mathf.Lerp(fadingObject.InitialAlpha, fadedAlpha, time * fadeSpeed)
                    );
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        if(runningCoroutines.ContainsKey(fadingObject))
        {
            StopCoroutine(runningCoroutines[fadingObject]);
            runningCoroutines.Remove(fadingObject);
        }
    }

    private IEnumerator FadeObjectIn(FadingObject fadingObject)
    {
        float time = 0.0f;

        while (fadingObject.materials[0].color.a < fadingObject.InitialAlpha)
        {
            foreach (Material material in fadingObject.materials)
            {
                if (material.HasProperty("_BaseColor"))
                {
                    material.color = new Color(
                        material.color.r,
                        material.color.g,
                        material.color.b,
                        Mathf.Lerp(fadedAlpha, fadingObject.InitialAlpha, time * fadeSpeed)
                    );
                }   
            }

            time += Time.deltaTime;
            yield return null;
        }

        foreach (Material material in fadingObject.materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.SetInt("_Surface", 0);

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

            material.SetShaderPassEnabled("DepthOnly", true);
            material.SetShaderPassEnabled("SHADOWCASTER", true);

            material.SetOverrideTag("RenderType", "Opaque");

            material.DisableKeyword("_SURFACE_TYPE_TRANSPAR     ENT");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        if (runningCoroutines.ContainsKey(fadingObject))
        {
            StopCoroutine(runningCoroutines[fadingObject]);
            runningCoroutines.Remove(fadingObject);
        }
    }

    private void ClearHits()
    {
        System.Array.Clear(hits, 0, hits.Length);
    }
    
    private FadingObject GetFadingObjectFromHit(RaycastHit hit)
    {
        return hit.collider != null ? hit.collider.GetComponent<FadingObject>() : null; 
    }

}
