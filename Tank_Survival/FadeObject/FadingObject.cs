// # System
using System;
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

public class FadingObject : MonoBehaviour, IEquatable<FadingObject>
{
    public List<Renderer>   renderers = new List<Renderer>();
    public List<Material>   materials = new List<Material>();
    public Vector3          position;

    [HideInInspector]
    public float            InitialAlpha;

    private void Awake()
    {
        position = transform.position;

        if(renderers.Count == 0)
        {
            renderers.AddRange(GetComponentsInChildren<Renderer>());
        }
        foreach(Renderer renderer in renderers)
        {
            materials.AddRange(renderer.materials);
        }

        InitialAlpha = materials[0].color.a;
    }

    public bool Equals(FadingObject other)
    {
        return position.Equals(other.position);
    }

    public override int GetHashCode()
    {
        return position.GetHashCode();
    }
}
