using UnityEngine;

public class CacheManager : MonoBehaviour {

    [HideInInspector] public Camera cam;
    public Camera uicam;
    [HideInInspector] public Quaternion camRotation;

    public static CacheManager instance;



    private void Awake() {
        instance = this;
        cam = Camera.main;
        camRotation = cam.transform.rotation;
    }
}
