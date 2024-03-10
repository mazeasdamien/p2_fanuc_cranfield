using Rti.Dds.Subscription;
using Rti.Types.Dynamic;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// The FanucManager class handles the reception and processing of Fanuc robotic system data through DDS.
public class FanucManager : MonoBehaviour
{
    public List<ArticulationBody> joints; // List of ArticulationBody components representing robotic joints.
    public Transform worldPosition; // Transform representing the world position of the robotic system.
    private DDSHandler dDSHandler; // Reference to the DDSHandler component for DDS communication.
    private protected DataReader<DynamicData> reader { get; private set; } // DDS DataReader for dynamic data.

    private bool init = false; // Flag to ensure initialization only occurs once.

    // Called when the script starts. Initializes the DDSHandler reference.
    private void Start()
    {
        dDSHandler = gameObject.GetComponent<DDSHandler>();
    }

    // Called every frame. Sets up DDS data structures on the first frame and processes incoming data.
    private void Update()
    {
        if (!init)
        {
            init = true;

            // DynamicType setup for the "RobotState" data structure.
            var typeFactory = DynamicTypeFactory.Instance;
            StructType RobotState = typeFactory.BuildStruct()
                .WithName("RobotState")
                .AddMember(new StructMember("J1", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("J2", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("J3", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("J4", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("J5", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("J6", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("X", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("Y", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("Z", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("W", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("P", typeFactory.GetPrimitiveType<double>()))
                .AddMember(new StructMember("R", typeFactory.GetPrimitiveType<double>()))
                .Create();

            // Setup DDS DataReader for the "RobotState_Topic".
            reader = dDSHandler.SetupDataReader("RobotState_Topic", RobotState);
        }

        // Process incoming DDS data.
        ProcessData(reader);
    }

    // Creates a Quaternion from Fanuc WPR (Wrist Pitch Roll) angles in degrees.
    public Quaternion CreateQuaternionFromFanucWPR(float W, float P, float R)
    {
        // Conversion from degrees to radians.
        float Wrad = W * (Mathf.PI / 180);
        float Prad = P * (Mathf.PI / 180);
        float Rrad = R * (Mathf.PI / 180);

        // Quaternion calculation based on Fanuc WPR angles.
        float qx = (Mathf.Cos(Rrad / 2) * Mathf.Cos(Prad / 2) * Mathf.Sin(Wrad / 2)) - (Mathf.Sin(Rrad / 2) * Mathf.Sin(Prad / 2) * Mathf.Cos(Wrad / 2));
        float qy = (Mathf.Cos(Rrad / 2) * Mathf.Sin(Prad / 2) * Mathf.Cos(Wrad / 2)) + (Mathf.Sin(Rrad / 2) * Mathf.Cos(Prad / 2) * Mathf.Sin(Wrad / 2));
        float qz = (Mathf.Sin(Rrad / 2) * Mathf.Cos(Prad / 2) * Mathf.Cos(Wrad / 2)) - (Mathf.Cos(Rrad / 2) * Mathf.Sin(Prad / 2) * Mathf.Sin(Wrad / 2));
        float qw = (Mathf.Cos(Rrad / 2) * Mathf.Cos(Prad / 2) * Mathf.Cos(Wrad / 2)) + (Mathf.Sin(Rrad / 2) * Mathf.Sin(Prad / 2) * Mathf.Sin(Wrad / 2));

        return new Quaternion(qx, qy, qz, qw);
    }

    // Processes DDS data received from the DataReader.
    void ProcessData(AnyDataReader anyReader)
    {
        var reader = (DataReader<DynamicData>)anyReader;
        using var samples = reader.Take();
        foreach (var sample in samples)
        {
            if (sample.Info.ValidData)
            {
                DynamicData data = sample.Data;

                // Update the world position based on X, Y, Z coordinates (scaled down by 1000).
                worldPosition.localPosition = new Vector3(-(float)data.GetValue<double>("X") / 1000, (float)data.GetValue<double>("Y") / 1000, (float)data.GetValue<double>("Z") / 1000);

                // Create a Quaternion from Fanuc WPR angles and apply it to localEulerAngles.
                Vector3 eulerAngles = CreateQuaternionFromFanucWPR((float)data.GetValue<double>("W"), (float)data.GetValue<double>("P"), (float)data.GetValue<double>("R")).eulerAngles;
                worldPosition.localEulerAngles = new Vector3(eulerAngles.x, -eulerAngles.y, -eulerAngles.z);

                // Debug.Log statements for debugging purposes (can be uncommented if needed).
                // Debug.Log(worldPosition.localPosition.ToString() + "  " + worldPosition.localEulerAngles.ToString());
                // Debug.Log((float)data.GetValue<double>("J1") + "  " + (float)data.GetValue<double>("J2") + "   " + ((float)data.GetValue<double>("J3") + (float)data.GetValue<double>("J2")) + "   " + (float)data.GetValue<double>("J4") + "   " + (float)data.GetValue<double>("J5") + "   " + (float)data.GetValue<double>("J6"));

                // Update joint positions based on received Fanuc joint angles.
                UpdateJointPositions(data);
            }
        }
    }

    // Updates ArticulationBody joint positions based on Fanuc joint angles.
    void UpdateJointPositions(DynamicData data)
    {
        var J1drive = joints[0].xDrive;
        var J2drive = joints[1].xDrive;
        var J3drive = joints[2].xDrive;
        var J4drive = joints[3].xDrive;
        var J5drive = joints[4].xDrive;
        var J6drive = joints[5].xDrive;

        // Set target angles for each joint based on Fanuc joint angles received.
        J1drive.target = (float)data.GetValue<double>("J1");
        J2drive.target = (float)data.GetValue<double>("J2");
        J3drive.target = (float)data.GetValue<double>("J3") + (float)data.GetValue<double>("J2");
        J4drive.target = (float)data.GetValue<double>("J4");
        J5drive.target = (float)data.GetValue<double>("J5");
        J6drive.target = (float)data.GetValue<double>("J6");

        // Update ArticulationBody joint drives with new target angles.
        joints[0].xDrive = J1drive;
        joints[1].xDrive = J2drive;
        joints[2].xDrive = J3drive;
        joints[3].xDrive = J4drive;
        joints[4].xDrive = J5drive;
        joints[5].xDrive = J6drive;
    }
}

