using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GridSpawn : MonoBehaviour{
    //[ExecuteInEditMode]
    public GameObject ObjectToSpawn;
    public float Gap;
    public int Edge;

    void Start() {
        generateGrid();
    }


    void Update() {
    }

    public void generateGrid() {
        Debug.Log("GridSpawn.generateGrid");
        foreach (Transform child in this.transform) {
            //GameObject.DestroyImmediate(child.gameObject);
            Destroy(child.gameObject);
        }

        float initX = this.transform.position.x - Edge * Gap / 2;
        float initY = this.transform.position.y - Edge * Gap / 2;
        float initZ = this.transform.position.z - Edge * Gap / 2;

        for (int z = 0; z < Edge; z++) {
            for (int y = 0; y < Edge; y++) {
                for (int x = 0; x < Edge; x++) {
                    Vector3 pos = new Vector3(initX + x * Gap, initY + y * Gap, initZ + z * Gap);
                    Instantiate(ObjectToSpawn, pos, Quaternion.identity, this.transform);
                }
            }
        }
    }
}
