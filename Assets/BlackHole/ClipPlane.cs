using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ClipPlane : MonoBehaviour
{
    static int _Shader_ClipPlane_Pos = Shader.PropertyToID("_ClipPlane_Pos");
    static int _Shader_ClipPlane_Nor = Shader.PropertyToID("_ClipPlane_Nor");

    // Start is called before the first frame update
    List<Renderer> renderers;
    void Start()
    {
        ResetRenderers();
    }

    public void ResetRenderers()
    {
        if (renderers == null)
        {
            renderers = new List<Renderer>();
        }
        else
        {
            renderers.Clear();
        }

        GetComponentsInChildren<Renderer>(renderers);
        
    }

    void OnTransformChildrenChanged()
    {
        ResetRenderers();
    }

    void OnValidate()
    {
        ResetRenderers();
    }

    void UpdateClipShaderParams()
    {
        if (renderers == null || renderers.Count <= 0)
        {
            return;
        }
        
        var nor = transform.up;
        var pos = transform.position;
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        foreach (var r in renderers)
        {
            if (r == null)
            {
                continue;
            }
            r.GetPropertyBlock(block);
            block.SetVector(_Shader_ClipPlane_Nor,nor);
            block.SetVector(_Shader_ClipPlane_Pos,pos);
            r.SetPropertyBlock(block);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateClipShaderParams();
    }

    void OnDrawGizmos()
    {
        Vector3 size = new Vector3(10, 0.05f, 10f);
        Gizmos.DrawWireCube(transform.position, size);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 size = new Vector3(10, 0.05f, 10f);

        Gizmos.DrawWireCube(transform.position, size);
    }

}
