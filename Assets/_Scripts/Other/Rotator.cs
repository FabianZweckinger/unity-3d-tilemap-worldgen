using UnityEngine;

public class Rotator : MonoBehaviour {

    [Header("Settings:")]
    [SerializeField] private float rotateSpeed = 50;
    [SerializeField] private Vector3 rotateDirection = new Vector3(0,1,0);


    private void Update() {
        transform.Rotate(rotateDirection, rotateSpeed * Time.deltaTime);
    }
}