using Rti.Dds.Publication;
using Rti.Dds.Subscription;
using Rti.Types.Dynamic;
using UnityEngine;

namespace VarjoExample
{
    public class AbortReset : MonoBehaviour
    {
        public DDSHandler dDSHandler;
        private protected DataWriter<DynamicData> writer { get; private set; }
        private DynamicData sample = null;
        private bool init = false;

        public Controller controllerL;
        public Controller controllerR;


        private void Update()
        {
            if (!init)
            {
                init = true;

                var typeFactory = DynamicTypeFactory.Instance;

                StructType AbortReset = typeFactory.BuildStruct()
                   .WithName("AbortReset")
                   .AddMember(new StructMember("Abort", typeFactory.GetPrimitiveType<bool>()))
                   .AddMember(new StructMember("Reset", typeFactory.GetPrimitiveType<bool>()))
                   .Create();

                writer = dDSHandler.SetupDataWriter("AbortReset_Topic", AbortReset);
                sample = new DynamicData(AbortReset);

            }

            UpdateOperatorPose();
        }

        public void UpdateOperatorPose()
        {

            if (controllerL.GripButton || controllerR.GripButton)
            {
                sample.SetValue("Abort", true);
                sample.SetValue("Reset", false);

                writer.Write(sample);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                sample.SetValue("Abort", false);
                sample.SetValue("Reset", true);

                writer.Write(sample);
            }
        }
    }
}