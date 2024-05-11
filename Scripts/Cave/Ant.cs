using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{

    public float Speed = 2f;
    public float TurnSpeed = 1f;

    public bool RandomizeMotion = true;

    public int DirectionChangeRandomRange = 500;
    public float RandomDirectionChangeLimit = 0.2f;

    public bool DebugLogEnabled = false;

    private AntDetector detector;
    private Gland gland;
    [SerializeField] private TargetType Target;

    private GameObject carryObject = null;

    private void DebugLog(string msg) {
        if (DebugLogEnabled)
            Debug.Log(this.name + " " + msg);
    }


    void Awake() {
        detector = GetComponentInChildren<AntDetector>(true);
        gland = GetComponentInChildren<Gland>(true);
        SetTarget(TargetType.Food);
    }

    void Start() {

    }


    void OnDrawGizmosSelected() {
        //Gizmos.DrawWireSphere(this.transform.position, DetectRadius);
    }

    // Update is called once per frame
    void Update() {
        float singleStep = TurnSpeed * Time.deltaTime;
        Vector3 newDirection;
        if (detector.nextDirection != Vector3.zero) {
            Vector3 nextDirection = detector.nextDirection;
            newDirection = Vector3.RotateTowards(transform.forward, nextDirection.normalized, singleStep, 0.0f);
        }
        else {
            if (RandomizeMotion && Random.Range(0, DirectionChangeRandomRange) < 1) {
                newDirection = transform.forward + new Vector3(Random.Range(-1 * RandomDirectionChangeLimit, RandomDirectionChangeLimit), 
                                                               Random.Range(-1 * RandomDirectionChangeLimit, RandomDirectionChangeLimit), 
                                                               Random.Range(-1 * RandomDirectionChangeLimit, RandomDirectionChangeLimit)
                                                              );
            }
            else {
                newDirection = transform.forward;
            }
        }
        transform.rotation = Quaternion.LookRotation(newDirection);
        transform.Translate(Speed * Time.deltaTime * Vector3.forward);
    }


    void SetTarget(TargetType newTarget) {
        switch (newTarget) {
            case TargetType.Food:
                gland.SetPheromoneType(TargetType.Home);
                detector.SetPheromoneType(TargetType.Food);
                break;
            case TargetType.Home:
                gland.SetPheromoneType(TargetType.Food);
                detector.SetPheromoneType(TargetType.Home);
                break;
            default:
                throw new System.NotImplementedException();
        }
    }


    void OnCollision() {
        DebugLog("OnCollision");
    }

    void OnParticleCollision(GameObject other) {
        DebugLog("OnParticleCollision ");
    }


    void OnTriggerEnter(Collider other) {
        DebugLog("OnTriggerEnter: " + other.gameObject.name);
        switch (other.gameObject.tag) {
            case "FoodCrop":
                if (!carryObject) {
                    // deactivate collider component to prevent further triggerring
                    other.gameObject.GetComponent<SphereCollider>().enabled = false;
                    // change layer to avoid further detection
                    other.gameObject.layer = this.gameObject.layer;

                    FoodSpot foodSpot = other.gameObject.transform.parent.gameObject.GetComponent<FoodSpot>();
                    foodSpot.pickCrop();

                    carryObject = other.gameObject;
                    // attach crop to agent
                    other.gameObject.transform.SetParent(this.transform, false);
                    other.gameObject.transform.position = gland.gameObject.transform.position;
                    // turn agent around
                    transform.Rotate(0, 180, 0);

                    SetTarget(TargetType.Home);
                }
                break;
            case "Colony":
                if (carryObject) { 
                    GameObject.Destroy(carryObject);
                    carryObject = null;
                    other.gameObject.GetComponent<Colony>().unloadFood();
                }

                //Transform[] ObjectChildrens;
                //ObjectChildrens = gameObject.GetComponentsInChildren<Transform>();

                transform.Rotate(0, 180, 0);
                SetTarget(TargetType.Food);
                break;
        }
        if ( other.gameObject.tag == "FoodCrop") {

        }
    }


    void OnParticleTrigger() {
        DebugLog("OnParticleTrigger: ");
    }

    void OnColliderEnter(Collider other) {
        DebugLog("OnColliderEnter");
    }

}
