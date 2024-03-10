using Rti.Dds.Subscription;
using Rti.Types.Dynamic;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// The ReachableManager class handles DDS communication and updates the reachability visualization in Unity.
public class ReachableManager : MonoBehaviour
{
    private DDSHandler dDSHandler; // Reference to the DDSHandler component for DDS communication.
    private protected DataReader<DynamicData> reader { get; private set; } // DDS DataReader for dynamic data.

    public Material rend; // Reference to the material used for visualization.
    public GameObject controller; // Reference to the controller object.
    private bool init = false; // Flag to ensure initialization only occurs once.

    // Called when the script starts. Initializes the DDSHandler reference.
    private void Start()
    {
        dDSHandler = gameObject.GetComponent<DDSHandler>();
    }

    // Called every frame. Sets up DDS data structures on the first frame and updates reachability visualization.
    private void Update()
    {
        if (!init)
        {
            init = true;

            // DynamicType setup for the "Reachability" data structure.
            var typeFactory = DynamicTypeFactory.Instance;
            StructType Reachable = typeFactory.BuildStruct()
                .WithName("Reachability")
                .AddMember(new StructMember("isReachable", typeFactory.GetPrimitiveType<bool>()))
                .Create();

            // Setup DDS DataReader for the "Reachability_Topic".
            reader = dDSHandler.SetupDataReader("Reachability_Topic", Reachable);
        }

        // Process incoming DDS data and update reachability visualization.
        ProcessData(reader);
    }

    // Processes DDS data received from the DataReader and updates reachability visualization.
    private void ProcessData(AnyDataReader anyReader)
    {
        var reader = (DataReader<DynamicData>)anyReader;
        using var samples = reader.Take();
        foreach (var sample in samples)
        {
            if (sample.Info.ValidData)
            {
                DynamicData data = sample.Data;

                // Update visualization based on reachability information.
                if (data.GetValue<bool>("isReachable") == true)
                {
                    rend.color = Color.green; // Set color to green if reachable.
                }
                else
                {
                    rend.color = Color.red; // Set color to red if not reachable.
                }
            }
        }
    }
}
