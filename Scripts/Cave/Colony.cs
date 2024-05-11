using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Colony : MonoBehaviour
{
    public GameObject AgentPrefab;
    public int AgentsCountLimit = 5000;
    public int SpawnDelay = 50;
    
    int AgentCount = 0;  // Readonly runtime
    long FoodCount = 0;  // Readonly runtime

    private float ColonySpereRadius;  // initialized on runtime    
    private int NextSpawnDelay = 0;
    private Transform ParentTransform;

    private TMP_Text foodCountText;

    void Awake() {
        foodCountText = this.transform.Find("FoodCounter").gameObject.GetComponent<TMP_Text>();
        foodCountText.text = FoodCount.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        ColonySpereRadius = this.GetComponent<SphereCollider>().radius;
        ParentTransform = this.transform.Find("Ants");
    }

    // Update is called once per frame
    void Update()
    {
        if (NextSpawnDelay <= 0 && AgentCount < AgentsCountLimit) {
            SpawnAgent();
            AgentCount++;
            NextSpawnDelay = SpawnDelay;
        }
        if (NextSpawnDelay > 0)
            NextSpawnDelay--;

        foodCountText.text = FoodCount.ToString();
    }

    private void SpawnAgent() {
        Vector3 posOnSphere = Random.onUnitSphere;

        // Quaternion.identity
        GameObject newAgent = Instantiate(AgentPrefab, this.transform.position + posOnSphere * ColonySpereRadius, Quaternion.FromToRotation(posOnSphere, Vector3.forward) * AgentPrefab.transform.localRotation , ParentTransform); 
    }

    public void unloadFood() {
        FoodCount++;
    }
}
