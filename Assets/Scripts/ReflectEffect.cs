using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectEffect : MonoBehaviour
{
    public Camera mainCamera;
    public MeshRenderer planeMeshRenderer;
    private Camera reflectCamera;
    [SerializeField]
    private RenderTexture reflectTexture;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnDestory()   // 当对象被销毁时，销毁对应的反射相机
    {
        if (reflectCamera)
        {
            Destroy(reflectCamera.gameObject);    
        }
        if (reflectTexture)
        {
            reflectTexture.Release();
            Destroy(reflectTexture);
        }
    }

    void InitReflectCamera()
    {
        reflectTexture = new RenderTexture(512, 512, 24);
        
        reflectCamera = new GameObject("ReflectCamera").AddComponent<Camera>();
        reflectCamera.transform.position = mainCamera.transform.position;
        reflectCamera.transform.rotation = mainCamera.transform.rotation;
        reflectCamera.transform.localScale = mainCamera.transform.localScale;
        // reflectCamera.hideFlags = HideFlags.HideInHierarchy;
        reflectCamera.depth = mainCamera.depth - 1;
        reflectCamera.targetTexture = reflectTexture;
        reflectCamera.cullingMask = LayerMask.GetMask("Default");
        
        planeMeshRenderer.material.SetTexture("_ReflectTex", reflectTexture);
    }

    Vector3 GetMirrorPoint(Vector3 p,Vector3 n,float d)
    {
        Vector3 r = new Vector3();
        var dist = Vector3.Dot(p, n) - d;
        r = p - 2 * dist * n;
        return r;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (reflectCamera == null)
        {
            InitReflectCamera();
        }
        if (reflectCamera == null)
        {
            return;
        }

        var planeNormal = planeMeshRenderer.transform.up;
        var planeDistance = Vector3.Dot(planeMeshRenderer.transform.position, planeNormal);

        reflectCamera.transform.position = GetMirrorPoint(mainCamera.transform.position, planeNormal, planeDistance);

        var refectForward = GetMirrorPoint(mainCamera.transform.forward, planeNormal,0);
        var refectUp = GetMirrorPoint(mainCamera.transform.up, planeNormal, 0);


        reflectCamera.transform.rotation = Quaternion.LookRotation(refectForward, refectUp);
        reflectCamera.fieldOfView = mainCamera.fieldOfView;
        reflectCamera.aspect = ((float)Screen.width) / Screen.height;
            

    }
}
