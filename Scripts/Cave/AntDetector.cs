using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntDetector : MonoBehaviour
{
    public bool CheckVisibility = false;
    public float DetectRadius = 0.5f;
    [Tooltip("Layer to use for raycasts checking targets visibility")]
    public LayerMask VisibilityCheckMask;   // 

    public bool DebugLogEnabled = false;
    public bool DebugDrawEnabled = false;

    [Tooltip("READONLY Set on runtime")]
    public Vector3 nextDirection;   // The resulting direction (Vector2.zero if no object were detected)
    [Tooltip("READONLY Set on runtime")]
    public Vector3 centerPoint;         // READONLY Set on runtime
    [Tooltip("READONLY Set on runtime")]
    public TargetType pheromoneType;    // READONLY Set on runtime
    [Tooltip("READONLY Set on runtime")]
    private int TargetLayer;
    [Tooltip("READONLY Set on runtime")]
    private int TargetPheromoneLayer;
    [Tooltip("READONLY Set on runtime")]
    private Collider[] detectedObjects;

    [System.Serializable]
    public enum DetectModeType {
        Average,
        Closest
    }


    private void DebugLog(string msg) {
        if (DebugLogEnabled)
            Debug.Log("AntDetector " + this.name + " " + msg);
    }


    void Awake() {
        centerPoint = this.transform.parent.transform.position;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        centerPoint = this.transform.parent.transform.position;
        //originalLayerMask |= (1 << layerToAdd);
        nextDirection = GetDirectionForLayer(TargetLayer, DetectModeType.Closest);
        if (nextDirection == Vector3.zero)
            nextDirection = GetDirectionForLayer(TargetPheromoneLayer, DetectModeType.Average);
        if (nextDirection == Vector3.zero)
            nextDirection = CheckWall();
    }

    Vector3 CheckWall() {
        var rayDirection = this.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(centerPoint, rayDirection, out hitInfo, DetectRadius, VisibilityCheckMask)) {
            DebugLog("Wall!!!");
            this.transform.parent.Rotate(0, 180, 0);
            return -rayDirection;
        }
        return Vector3.zero;
    }

    Vector3 GetDirectionForLayer(int pLayer, DetectModeType detectMode) {
        LayerMask DetectLayersMask = (1 << pLayer);
        detectedObjects = Physics.OverlapSphere(this.transform.position, DetectRadius, DetectLayersMask, QueryTriggerInteraction.Collide);
        int detectedCount = 0;
        if (detectedObjects.Length > 0) {
            float totalWeight = 0;
            Vector3 resultVector = new Vector3(0, 0, 0);
            if (detectMode == DetectModeType.Closest)
                resultVector = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);


            for (int objN = 0; objN < detectedObjects.Length; objN++) {
                if (CheckVisibility && CheckTargetVisibility(detectedObjects[objN].gameObject))
                    continue;
                IDetectable pheromone = detectedObjects[objN].gameObject.GetComponent<IDetectable>();
                if (pheromone != null) {
                    Vector3 targetVector = detectedObjects[objN].transform.position - centerPoint;
                    switch (detectMode) {
                        case DetectModeType.Average:
                            totalWeight += pheromone.getWeight();
                            resultVector += pheromone.getWeight() * targetVector;
                            break;
                        case DetectModeType.Closest:
                            if (resultVector.magnitude > targetVector.magnitude)
                                resultVector = targetVector;
                            break;
                        default:
                            throw new System.NotImplementedException("");
                    }

                    detectedCount++;
                }
            }
            DebugLog("Layer " + LayerMask.LayerToName(pLayer) + "  detectedObjects count: " + detectedCount.ToString());
            if (detectedCount != 0)
                return resultVector.normalized;
        }
        return Vector3.zero;
    }

    bool CheckTargetVisibility(GameObject targetObject) {
        //Ray rayTest = new Ray(centerPoint, targetObject.transform.position - transform.position);
        var rayDirection = targetObject.transform.position - centerPoint;

        RaycastHit hitInfo;
        if (Physics.Raycast(centerPoint, rayDirection, out hitInfo, DetectRadius, VisibilityCheckMask)) {
            if (hitInfo.transform.gameObject != targetObject) {
                DebugLog("Raycase hit the " + hitInfo.transform.gameObject.name);
                if (DebugDrawEnabled)
                    Debug.DrawLine(centerPoint, targetObject.transform.position, Color.yellow);
                return true;
            }
        }
        if (DebugDrawEnabled)
            Debug.DrawLine(centerPoint, targetObject.transform.position, Color.green);
        return false;
    }


    void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, DetectRadius);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(centerPoint, 0.5f);
        Gizmos.color = Color.red;
        /*
        if (detectedObjects != null) {
            for (int i = 0; i < detectedObjects.Length; i++) {                
                Gizmos.DrawLine(centerPoint, detectedObjects[i].transform.position);
            }
        }
        */
        Gizmos.color = Color.magenta;
        if (nextDirection !=null && nextDirection != Vector3.zero)
            DrawArrow(centerPoint, 3 * nextDirection);
    }


    public void SetPheromoneType(TargetType newTarget) {
        TargetLayer = LayerMask.NameToLayer(newTarget.ToString());
        TargetPheromoneLayer = LayerMask.NameToLayer("Pheromone" + newTarget.ToString());
        if (TargetLayer < 0 || TargetPheromoneLayer < 0)
            throw new System.Exception("[Target] or Pheromone[Target] Layer not found for: " + newTarget.ToString());
    }

    public static void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f) {
        Gizmos.DrawRay(pos, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;

        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }


    void OnParticleCollision(GameObject other) {
        DebugLog("PheromoneDetector OnParticleCollision ");
    }

    void OnCollisionEnter(Collision other) {
        DebugLog("PheromoneDetector OnCollisionEnter ");
    }

    void OnTriggerEnter(Collider other) {
        DebugLog("PheromoneDetector OnTriggerEnter ");
    }
}
