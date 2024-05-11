using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class CollisionsDisplay : MonoBehaviour
{

    public int Delay = 150;
    public float DetectRadius = 5;
    public LayerMask DetectLayersMask;
    private Collider[] detectedObjects;

    public int curObject = 0;
    public int curDelay = 0;

    Material defaultMaterial;
    Material selectedMaterial;

    void Start() {
        defaultMaterial = Resources.Load("Materials/Cave/TargetMaterial", typeof(Material)) as Material;
        selectedMaterial = Resources.Load("Materials/Cave/TargetSelectedMaterial", typeof(Material)) as Material;
    }

    void Update() {

        if (detectedObjects == null) {
            detectedObjects = Physics.OverlapSphere(this.transform.position, DetectRadius, DetectLayersMask, QueryTriggerInteraction.Collide);
            curObject = 0;
            return;
        }

        if (curObject >= detectedObjects.Length - 1) {
            GameObject.Find("GridSpawn").GetComponent<GridSpawn>().generateGrid();
            detectedObjects = null;
            return;
        }

        if (curDelay == 0) {
            if (detectedObjects != null && detectedObjects.Length > 0 && curObject == detectedObjects.Length - 1)
                curObject = 0;
            else {
                if (detectedObjects != null) {
                    try {
                        Debug.DrawLine(this.transform.position, detectedObjects[curObject].transform.position, Color.white, 2f);
                        detectedObjects[curObject].transform.gameObject.GetComponent<MeshRenderer>().materials = new Material[] { selectedMaterial };
                    }
                    catch (Exception e) {
                    }                    
                    curObject++;                    
                }
                if (curObject == detectedObjects.Length)
                    curDelay = Delay * 10;
            }
            if (curDelay == 0)
                curDelay = Delay;
        }
        else {
            curDelay--;
        }
    }



    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(this.transform.position, DetectRadius);

        /*
        if (detectedObjects != null) {
            for (int i = 0; i < detectedObjects.Length; i++) {
                Gizmos.DrawLine(this.transform.position, detectedObjects[i].transform.position);
            }
        }
        */
    }

    void OnDrawGizmosSelected() {
        //Gizmos.DrawWireSphere(this.transform.position, DetectRadius);
    }


    void OnParticleCollision(GameObject other) {
        Debug.Log("PheromoneDetector OnParticleCollision ");
    }

    void OnCollisionEnter(Collision other) {
        Debug.Log("PheromoneDetector OnCollisionEnter ");
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("PheromoneDetector OnTriggerEnter ");
    }
}
