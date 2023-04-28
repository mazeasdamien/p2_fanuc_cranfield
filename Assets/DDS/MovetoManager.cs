using Rti.Dds.Publication;
using Rti.Dds.Subscription;
using Rti.Types.Dynamic;
using UnityEngine;

public class MovetoManager : MonoBehaviour
{
    private DDSHandler dDSHandler;
    private protected DataWriter<DynamicData> writer { get; private set; }
    private DynamicData sample = null;
    private bool init = false;

    public bool sendDropped;

    private void Start()
    {
        dDSHandler = gameObject.GetComponent<DDSHandler>();
    }

    private void Update()
    {
        if (!init)
        {
            init = true;

            var typeFactory = DynamicTypeFactory.Instance;

            StructType ReleasedChecker = typeFactory.BuildStruct()
               .WithName("Released")
               .AddMember(new StructMember("bool", typeFactory.GetPrimitiveType<bool>()))
               .Create();

            writer = dDSHandler.SetupDataWriter("Moveto_Topic", ReleasedChecker);
            sample = new DynamicData(ReleasedChecker);

        }

        if (sendDropped == true)
        {
            Debug.Log("true");
            sample.SetValue("bool", true);
            writer.Write(sample);
            sendDropped = false;
        }

    }
}
