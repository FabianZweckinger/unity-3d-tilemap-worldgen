using UnityEngine;

public class UIRenderTextureBufferController : MonoBehaviour {

    public Texture uIRenderTextureBuffer;

    

#if UNITY_EDITOR
    private void Update() {
        ResizeUIRenderTextureBuffer(CacheManager.instance.cam.pixelWidth, CacheManager.instance.cam.pixelHeight);
    }
#else
    private void Start() {
        ResizeUIRenderTextureBuffer(CacheManager.instance.cam.pixelWidth, CacheManager.instance.cam.pixelHeight);
        KillInstance();
    }
#endif



    private void ResizeUIRenderTextureBuffer(int newRenderTextureWidth, int newRenderTextureHeight) {
        CacheManager.instance.uicam.targetTexture.Release();
        uIRenderTextureBuffer.width = newRenderTextureWidth;
        uIRenderTextureBuffer.height = newRenderTextureHeight;
    }


    private void KillInstance() { //Used in Standalone Builds to reduce Performance Costs
        Destroy(this);
    }
}
