using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gland : MonoBehaviour
{
    public GameObject PheromonePrefab;   
    public int TimeInterval = 50;    
    public int PheromoneLifetime = 1500;

    public TargetType PheromoneType = TargetType.Home;
    int PheromoneLayer;

    public bool DebugLogEnabled = false;

    private long Time = 0;
    private int TimeSinceLastPheromone = 0;

    private Vector3 lastPheromonePos;
    private Transform SpawnParent;

    public Dictionary<TargetType, Material> pheromoneMaterials = new Dictionary<TargetType, Material>();

    private Queue<Tuple<GameObject, IPheromone>> pheromonesQueue = new Queue<Tuple<GameObject, IPheromone>>();

    private void DebugLog(string msg) {
        if (DebugLogEnabled)
            Debug.Log("Gland " + this.name + " " + msg);
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnParent = GameObject.Find("Pheromones").transform;
        if (!SpawnParent)
            SpawnParent = this.transform;

        pheromoneMaterials[TargetType.Food] = Resources.Load("Materials/Cave/PheromoneFood", typeof(Material)) as Material;
        pheromoneMaterials[TargetType.Home] = Resources.Load("Materials/Cave/PheromoneHome", typeof(Material)) as Material;
    }

    // Update is called once per frame
    void Update()
    {
        Time++;
        if (TimeSinceLastPheromone++ > TimeInterval) {
            GameObject newPheromoneGO = Instantiate(PheromonePrefab, this.transform.position, Quaternion.identity, SpawnParent);
            newPheromoneGO.GetComponent<MeshRenderer>().materials = new Material[] { pheromoneMaterials[PheromoneType] };
            IPheromone newPheromone = newPheromoneGO.GetComponent<IPheromone>();
            newPheromone.DeathTime = Time + PheromoneLifetime;
            newPheromone.PheromoneType = PheromoneType;
            newPheromone.gameObject.layer = PheromoneLayer;
            pheromonesQueue.Enqueue(new Tuple<GameObject, IPheromone>(newPheromoneGO, newPheromone));
            TimeSinceLastPheromone = 0;
        }
        if (pheromonesQueue.Count > 0) {
            Tuple<GameObject, IPheromone> oldestPheromone = pheromonesQueue.Peek();

            if (oldestPheromone.Right.DeathTime <= Time) {
                pheromonesQueue.Dequeue();
                GameObject.Destroy(oldestPheromone.Left);
            }
        }
    }

    public void SetPheromoneType(TargetType newPheromoneType) {
        PheromoneType = newPheromoneType;
        PheromoneLayer = LayerMask.NameToLayer("Pheromone" + newPheromoneType.ToString());
        if (PheromoneLayer < 0)
            throw new System.Exception("Pheromone Layer not found for: " + newPheromoneType.ToString());
    }
}
