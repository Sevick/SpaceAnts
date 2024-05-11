using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPheromone : MonoBehaviour, IDetectable {

    public long AntID;
    public long DeathTime;
    public long BirthTime;
    public TargetType PheromoneType;

    [HideInInspector]
    Renderer m_Renderer; // cached on Start(), used to change the ground material
    private Material m_Material; // cached on Start()

    public void Start() {
        m_Renderer = GetComponent<Renderer>(); // Get the ground renderer so we can change the material when a goal is scored       
        m_Material = m_Renderer.material;       // Original ("default") ground material

        Dictionary<TargetType, Material>  pheromoneMaterials = new Dictionary<TargetType, Material>() {
                    { TargetType.Home, null },
                    { TargetType.Food, null }
                };
    }

    public float getWeight() {
        return 1f;
    }

}
