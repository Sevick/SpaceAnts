using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public GameObject m_camera;
    private Transform cameraTransform;

    // Start is called before the first frame update
    void Start() {
        if (!m_camera) {
            m_camera = GameObject.FindWithTag("MainCamera");
        }
        cameraTransform = m_camera.transform;
    }

    // Update is called once per frame
    void Update() {
        Quaternion wantedRotation = Quaternion.LookRotation(cameraTransform.forward, cameraTransform.up);
        //wantedRotation.
        transform.rotation = wantedRotation;

    }
}
