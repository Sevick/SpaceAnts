using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpot : MonoBehaviour
{
    public int cropsLimit = 100;
    public int cropsCount = 0;
    public int respawnThreshold = 30;
    public float SpotRadius = 5f;
    public GameObject FoodCropPrefab;

    private void Start() {
        cropsCount = instantiateCrops(cropsLimit);
    }

    void Update()  {
        if (cropsCount < cropsLimit - respawnThreshold) {
            cropsCount += instantiateCrops(cropsLimit - cropsCount);
        }        
    }

    public void pickCrop() {
        cropsCount--;
    }

    private int instantiateCrops(int newCropsCount) {
        int i;
        for (i = 0; i < newCropsCount; i++) {
            // Generate a random point on the sphere's surface
            Vector3 randomPoint = this.transform.position + Random.insideUnitSphere * SpotRadius;
            // Instantiate the object at the calculated position
            Instantiate(FoodCropPrefab, randomPoint, Quaternion.identity, this.transform);
        }
        return i;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, SpotRadius);
    }
}
