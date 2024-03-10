using Rti.Dds.Publication;
using Rti.Dds.Subscription;
using Rti.Types.Dynamic;
using UnityEngine;

// The TeleopManager class handles DDS communication for teleoperation in Unity.
public class TeleopManager : MonoBehaviour
{
    public DDSHandler dDSHandler; // Reference to the DDSHandler component for DDS communication.
    private protected DataWriter<DynamicData> writer { get; private set; } // DDS DataWriter for dynamic data.
    private DynamicData sample = null; // DynamicData object for creating and sending DDS samples.
    private bool init = false; // Flag to ensure initialization only occurs once.
    private Transform updater; // Reference to the transform that is being updated.
    private Vector3 tempPos = new Vector3(); // Temporary storage for position information.
    private Vector3 tempRot = new Vector3(); // Temporary storage for rotation information.

    // Called every frame. Sets up DDS data structures on the first frame and updates teleoperation information.
    private void Update()
    {
        if (!init)
        {
            init = true;

            // DynamicType setup for the "OperatorNewPose" data structure.
            var typeFactory = DynamicTypeFactory.Instance;
            var OperatorNewPose = typeFactory.BuildStruct()
                .WithName("OperatorNewPose")
                .AddMember(new StructMember("X", typeFactory.GetPrimitiveType<float>()))
                .AddMember(new StructMember("Y", typeFactory.GetPrimitiveType<float>()))
                .AddMember(new StructMember("Z", typeFactory.GetPrimitiveType<float>()))
                .AddMember(new StructMember("W", typeFactory.GetPrimitiveType<float>()))
                .AddMember(new StructMember("P", typeFactory.GetPrimitiveType<float>()))
                .AddMember(new StructMember("R", typeFactory.GetPrimitiveType<float>()))
                .Create();

            // Setup DDS DataWriter for the "OperatorNewPose_Topic".
            writer = dDSHandler.SetupDataWriter("OperatorNewPose_Topic", OperatorNewPose);
            sample = new DynamicData(OperatorNewPose);
            updater = gameObject.transform;
        }

        // Update teleoperation pose.
        UpdateOperatorPose();
    }

    // Updates teleoperation pose and sends DDS sample if there is a change in position or rotation.
    public void UpdateOperatorPose()
    {
        if (gameObject.transform.localPosition != tempPos || gameObject.transform.localEulerAngles != tempRot)
        {
            // Set values in the DDS sample based on the current position and rotation of the GameObject.
            sample.SetValue("X", -updater.localPosition.x * 1000);
            sample.SetValue("Y", updater.localPosition.y * 1000);
            sample.SetValue("Z", updater.localPosition.z * 1000);
            sample.SetValue("W", CreateFanucWPRFromQuaternion(updater.localRotation).x);
            sample.SetValue("P", CreateFanucWPRFromQuaternion(updater.localRotation).y);
            sample.SetValue("R", CreateFanucWPRFromQuaternion(updater.localRotation).z);

            // Write the DDS sample to the "OperatorNewPose_Topic".
            writer.Write(sample);

            // Update temporary storage with the current position and rotation.
            tempPos = gameObject.transform.localPosition;
            tempRot = gameObject.transform.localEulerAngles;
        }
    }

    // Converts a Quaternion to Fanuc World Position Representation (WPR).
    private Vector3 CreateFanucWPRFromQuaternion(Quaternion q)
    {
        float W = Mathf.Atan2(2 * ((q.w * q.x) + (q.y * q.z)), 1 - 2 * (Mathf.Pow(q.x, 2) + Mathf.Pow(q.y, 2))) * (180 / Mathf.PI);
        float P = Mathf.Asin(2 * ((q.w * q.y) - (q.z * q.x))) * (180 / Mathf.PI);
        float R = Mathf.Atan2(2 * ((q.w * q.z) + (q.x * q.y)), 1 - 2 * (Mathf.Pow(q.y, 2) + Mathf.Pow(q.z, 2))) * (180 / Mathf.PI);

        return new Vector3(W, -P, -R);
    }
}
