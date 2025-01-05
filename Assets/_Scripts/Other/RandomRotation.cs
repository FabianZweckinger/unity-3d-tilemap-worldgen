using UnityEngine;

public class RandomRotation : MonoBehaviour {

    private void Start() {
        transform.Rotate(new Vector3(0,1,0), Random.Range(0f, 360f));
    }
}
