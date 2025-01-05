using UnityEngine;

public class NatureTile : MonoBehaviour {

    public void TryCleanTile() {
        if (transform.childCount == 1) {
            Destroy(gameObject);
        }
    }
}
