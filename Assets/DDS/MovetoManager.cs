using Rti.Dds.Publication;
using Rti.Dds.Subscription;
using Rti.Types.Dynamic;
using UnityEngine;

// The MovetoManager class handles the DDS communication for the "Moveto" action in Unity.
public class MovetoManager : MonoBehaviour
{
    private DDSHandler dDSHandler; // Reference to the DDSHandler component for DDS communication.
    public DragObject dragObject; // Reference to the DragObject component for tracking object dragging.
    private protected DataWriter<DynamicData> writer { get; private set; } // DDS DataWriter for dynamic data.
    private DynamicData sample = null; // DynamicData object for creating and sending DDS samples.
    private bool init = false; // Flag to ensure initialization only occurs once.

    // Called when the script starts. Initializes the DDSHandler reference.
    private void Start()
    {
        dDSHandler = gameObject.GetComponent<DDSHandler>();
    }

    // Called every frame. Sets up DDS data structures on the first frame and sends data when the object is released.
    private void Update()
    {
        if (!init)
        {
            init = true;

            // DynamicType setup for the "Released" data structure.
            var typeFactory = DynamicTypeFactory.Instance;
            StructType ReleasedChecker = typeFactory.BuildStruct()
               .WithName("Released")
               .AddMember(new StructMember("bool", typeFactory.GetPrimitiveType<bool>()))
               .Create();

            // Setup DDS DataWriter for the "Moveto_Topic".
            writer = dDSHandler.SetupDataWriter("Moveto_Topic", ReleasedChecker);
            sample = new DynamicData(ReleasedChecker);
        }

        // Check if the object has been released.
        if (dragObject.released)
        {
            Debug.Log("Object released: true");

            // Set the value for the "bool" member of the DDS sample to true.
            sample.SetValue("bool", true);

            // Write the DDS sample to the "Moveto_Topic".
            writer.Write(sample);
        }
    }
}
