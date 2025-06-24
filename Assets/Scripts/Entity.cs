

using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using static CSVReader;

/*
 * Entity Script
 *
 * This script controls the behavior of individual entities instantiated by the CSVReader script.
 * It handles the movement of entities based on position and timestamp data, and also manages visual
 * changes such as color updates when an entity reaches a new position.
 *
 * - The entity's positions and timestamps are stored and updated over time.
 * - The script moves the entity towards target positions based on the provided timestamps.
 * - When the entity reaches a new position, its color changes to visually indicate the movement.
 *
 *
 * Methods:
 * - Start(): Initializes the entity, checking for required components and storing the initial position.
 * - Initialize(): Receives a list of positions and timestamps from the CSVReader and stores them.
 * - Update(): Updates the entity's position over time, moving it towards the next target position.
 */


public class Entity : MonoBehaviour
{

    public GameObject ExclamationPoint;
    public TextMeshProUGUI textfield;

    //public string agentName;

    void Start()
    {

    }


    void Update()
    {

    }
    public void OnHover()
    {
        textfield.enabled = true;
        Debug.Log("Hover On");
    }

    public void OffHover()
    {
        textfield.enabled = false;
        Debug.Log("Hover Off");
    }

    public void EnableExclaim()
    {
        
        this.ExclamationPoint.GetComponent<MeshRenderer>().enabled = true;
    }

    public void DisableExclaim()
    {
        //popout.enabled = false;
        this.ExclamationPoint.GetComponent<MeshRenderer>().enabled = false;
    }

    public void SetMetadata(string metadata)
    {
        //popout.enabled = true;
        this.textfield.SetText(metadata);
    }

    public string GetMetadata()
    {
        //popout.enabled = true;
        return this.textfield.GetParsedText();
    }


}


