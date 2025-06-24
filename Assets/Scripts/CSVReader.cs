using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using System.Text;
using System.IO;
using UnityEngine;
using TMPro;
using System;
using System.Threading.Tasks;

/*
 * CSVReader Script
 *
 * This script reads entity data from a CSV file and uses the RIDE API to manage and move agents on a NavMesh.
 *
 * - The CSV file is expected to have entity information, where each row represents a data point with
 *   position and timestamp values for an entity.
 *
 * - The script integrates with the RIDE API to instantiate agents and manage their movement on the NavMesh.
 *   It tracks agent positions and updates them in real-time based on CSV timestamps.
 *
 * Methods:
 * - Start(): Initializes the simulation by reading the CSV, instantiating entities, and starting the
 *   coroutine to update entity positions.
 * - ReadCSVFile(): Reads and parses the CSV file to extract entity data.
 * - InstantiateEntities(): Uses the RIDE API to create agents based on CSV data and initializes their
 *   positions on the NavMesh.
 * - PlaceAgentOnNavMesh(): Ensures that agents are placed on a valid position on the NavMesh.
 * - UpdateEntityPositionsCoroutine(): A coroutine that updates the positions of entities over time
 *   based on their timestamps.
 */


public class CSVReader : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    private Dictionary<string, List<EntityInfo>> entityData = new Dictionary<string, List<EntityInfo>>();
    private Dictionary<string, GameObject> instantiatedEntities = new Dictionary<string, GameObject>(); // Track instantiated entities by name

    private GameObject ArcGisMapParent;
    public List<GameObject> entities = new List<GameObject>();
    public GameObject friendlyInfantryPrefab;
    public GameObject insurgentPrefab;
    public GameObject HMMWVPrefab;
    private float elapsedTime;
    private bool fastforwarding = false;
    //public ArcGISMapComponent MapComponent;


    [System.Serializable]
    public struct EntityInfo
    {
        public string Name;
        public Vector3 Position;
        public float Heading;
        public float Timestamp;
    }

    private float simulationStartTime;

    void Start()
    {
        ArcGisMapParent = GameObject.Find("ArcGISMap");
        CreateEntities();
        GenerateCyberEffectData();
        AddMetadata();
        simulationStartTime = Time.time;
        
    }

    private void Update()
    {
        elapsedTime = Time.time - simulationStartTime;
        timerText.text = $"Elapsed Time: {elapsedTime:F2} seconds";
    }

   void AddMetadata()
    {
#if UNITY_EDITOR
        string metadataCSVPath = Path.Combine(Application.streamingAssetsPath, "CyberBOSS_Actors_Devices 1.csv");
#else
        string metadataCSVPath = Path.Combine(Application.persistentDataPath, "CyberBOSS_Actors_Devices 1.csv");
#endif
        string[] metadataLines = File.ReadAllLines(metadataCSVPath);

        foreach (string line in metadataLines)
        {
            string entityName;
            string[] values = line.Split(',');
            //Debug.Log(values);

            entityName = values[0].Replace("7SQ10CAV-1BCT4ID", "7-SQ-10-CAV");
            entityName = entityName.Replace("_1", "");
            entityName = entityName.Replace("/", "-");
            //Debug.Log(entityName);
            var tempGO = GameObject.Find(entityName);
            if (tempGO != null)
            {
                //Debug.Log("Found it");
                tempGO.GetComponent<Entity>().SetMetadata($"Entity Name: {entityName} \n Role Name: {values[1]} \n IP: {values[2]} \n Device Type: {values[3]} \n");
            }




        }
    }

    async void CreateEntities()
    {
#if UNITY_EDITOR
            string mobilityCSVPath = Path.Combine(Application.streamingAssetsPath, "GDC_MobilityData_CyberSim.csv");
#else
        string mobilityCSVPath = Path.Combine(Application.persistentDataPath, "GDC_MobilityData_CyberSim.csv");
#endif



        string[] mobilityLines = File.ReadAllLines(mobilityCSVPath);

        int lineNum = 0;
        bool isFirstLineHeader = true;

        foreach (string line in mobilityLines)
        {
            lineNum++;
            if (isFirstLineHeader)
            {
                isFirstLineHeader = false;
                continue;
            }

            string[] values = line.Split(',');
            //string entityName = values[7];
            string entityName = values[7].Replace("/", "-");
            if (!entityData.ContainsKey(entityName))
            {
                entityData[entityName] = new List<EntityInfo>();
                //Debug.Log(entityName);
                InstantiateEntity(entityName);
            }

            float timestamp = float.Parse(values[0]) / 1000f;

            while (elapsedTime < timestamp)
            {
                if ((timestamp - elapsedTime) > 10 && !fastforwarding)
                {
                    Debug.Log("Playback Mode: Fast Forward");
                    fastforwarding = true;
                    Time.timeScale = 100.0f;
                }
                else if ((timestamp - elapsedTime) < 10 && fastforwarding)
                {
                    fastforwarding = false;
                    Debug.Log("Playback Mode: Normal");
                    Time.timeScale = 10.0f;
                }
                await WaitMilliseconds(10);
                //Debug.Log("Waiting at timestamp: " + elapsedTime + " for " + (timestamp - elapsedTime));

            }
            var tempGO = GameObject.Find(entityName);
            //Debug.Log("Moved: " + entityName);
            tempGO.GetComponent<ArcGISLocationComponent>().Position = new Esri.GameEngine.Geometry.ArcGISPoint(float.Parse(values[21]), float.Parse(values[20]), float.Parse(values[22]), ArcGISSpatialReference.WGS84());
            tempGO.GetComponent<ArcGISLocationComponent>().Rotation = new Esri.ArcGISMapsSDK.Utils.GeoCoord.ArcGISRotation((double)float.Parse(values[8]), 90f, 0);

            Animator animator = tempGO.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("move", true); //Will set animation state to true - In future we can rework this to check previous position and if that matches current position, set move to false to switch to idle animation
            }
        }
        Debug.Log("Mobility CSV Complete");
    }

    async void GenerateCyberEffectData()
    {
        bool isFirstLineHeader = true;

#if UNITY_EDITOR
        string cyberEffectCSVPath = Path.Combine(Application.streamingAssetsPath, "ExtractedCyberBossData-time-formatted.csv");
#else
        string cyberEffectCSVPath = Path.Combine(Application.persistentDataPath, "ExtractedCyberBossData-time-formatted.csv");
#endif



        string[] cyberEffectLines = File.ReadAllLines(cyberEffectCSVPath);

        foreach (string line in cyberEffectLines)
        {
            if (isFirstLineHeader)
            {
                isFirstLineHeader = false;
                continue;
            }

            string[] values = line.Split(',');
            //string entityName = values[4].Replace("_1", "");
            //string entityName = string.Concat(values[2], "_1
            string entityName;
            if (values[4] == "")
            {
                entityName = values[2].Replace("7SQ10CAV-1BCT4ID", "7-SQ-10-CAV");
                entityName = entityName.Replace("_1", "");
            }
            else
            {
                entityName = values[4].Replace("_1", "");
                entityName = entityName.Replace("/", "-");
            }
            //Debug.Log($"{entityName}");
            float timestamp = float.Parse(values[34]) / 1000;
            while (elapsedTime < timestamp)
            {
 
                await WaitMilliseconds(10);

            }
            var tempGO = GameObject.Find(entityName);
            if (tempGO != null) 
            {

                bool DDoS = values[11].Contains("True");
                bool disrupted = values[18].Contains("True");
                bool hacked = values[10].Contains("True");

                if (DDoS)
                {
                    Debug.Log("DDoS at: " + timestamp + " on Entity: " + entityName);
                    tempGO.GetComponent<Entity>().EnableExclaim();
                    string currentText = tempGO.GetComponent<Entity>().GetMetadata();
                    tempGO.GetComponent<Entity>().SetMetadata($"{currentText} \n DDoS at timestamp: {timestamp}!");
                }


                if (disrupted)
                { 
                    Debug.Log("disrupted at: " + timestamp + " on Entity: " + entityName);
                    tempGO.GetComponent<Entity>().EnableExclaim();
                    string currentText = tempGO.GetComponent<Entity>().GetMetadata();
                    tempGO.GetComponent<Entity>().SetMetadata($"{currentText} \n Entity disrupted at timestamp: {timestamp} by cyber attack! \n");
                }

                if (hacked)
                {
                    Debug.Log("hacked at: " + timestamp + " on Entity: " + entityName);
                    tempGO.GetComponent<Entity>().EnableExclaim();
                    string currentText = tempGO.GetComponent<Entity>().GetMetadata();
                    tempGO.GetComponent<Entity>().SetMetadata($"{currentText} \n Hacked at timestamp: {timestamp}!");
                }
            }

            

        }
        Debug.Log("Cyber Effect CSV Complete");
    }

    void InstantiateEntity(string name)
    {

        if (!instantiatedEntities.ContainsKey(name))
        {
            if (name.Contains("Insurgent") || name.Contains("Adversary"))
            {
                GameObject entityObject = Instantiate(insurgentPrefab, Vector3.zero, Quaternion.identity, ArcGisMapParent.transform);
                entityObject.name = name;
                instantiatedEntities[name] = entityObject;


            }
            else if (name.Contains("HMMWV"))
            {
                GameObject entityObject = Instantiate(HMMWVPrefab, Vector3.zero, Quaternion.identity, ArcGisMapParent.transform);
                entityObject.name = name;
                instantiatedEntities[name] = entityObject;

            }
            else
            {
                GameObject entityObject = Instantiate(friendlyInfantryPrefab, Vector3.zero, Quaternion.identity, ArcGisMapParent.transform);
                entityObject.name = name;
                instantiatedEntities[name] = entityObject;
            }


        }


    }

    // Handles the disposing all of the input events.
    void OnDestroy()
    {

    }


    IEnumerator UpdateTimerCoroutine()
    {
        while (true)
        {
            float elapsedTime = Time.time - simulationStartTime;
            timerText.text = $"Time: {elapsedTime:F2} seconds";
            yield return new WaitForSeconds(0.1f);
        }
    }

    private static async Task WaitMilliseconds(int milliseconds)
    {
        await Task.Delay(milliseconds);
    }

}

