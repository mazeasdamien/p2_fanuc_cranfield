using Rti.Dds.Publication;
using Rti.Dds.Subscription;
using Rti.Types.Dynamic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace VarjoExample
{
    public class GeneralViewKinect : MonoBehaviour
    {
        private DDSHandler dDSHandler;

        private protected DataReader<DynamicData> reader { get; private set; }
        private DynamicData sample = null;
        private bool init = false;
        public List<RawImage> rawTextureList;

        public RawImage rawTexture;
        public Texture2D texture2D;

        public Controller controllerRight;
        public int count = 0;
        bool hasbeenchanged;

        private void Start()
        {
            hasbeenchanged = true;
            texture2D = new Texture2D(1, 1);
            dDSHandler = gameObject.GetComponent<DDSHandler>();
            rawTexture = rawTextureList[count];
        }

        // Update is called once per frame
        void Update()
        {
            if (!init)
            {
                init = true;
                var typeFactory = DynamicTypeFactory.Instance;
                StructType RobotImage = typeFactory.BuildStruct()
                    .WithName("Video")
                    .AddMember(new StructMember("Memory", typeFactory.CreateSequence(typeFactory.GetPrimitiveType<byte>(), 2000000)))
                    .Create();
                reader = dDSHandler.SetupDataReader("GeneralView_Topic", RobotImage);
            }
            ProcessData(reader);

            if (controllerRight.Primary2DAxisClick)
            {

                if (hasbeenchanged == true)
                {
                    count++;
                    texture2D = new Texture2D(1, 1);
                    rawTexture = rawTextureList[count];
                    hasbeenchanged = false;
                }
            }
            if (!controllerRight.Primary2DAxisClick)
            {
                hasbeenchanged = true;
            }
        }

        void ProcessData(AnyDataReader anyReader)
        {
            var reader = (DataReader<DynamicData>)anyReader;
            using var samples = reader.Take();
            foreach (var sample in samples)
            {
                if (sample.Info.ValidData)
                {
                    DynamicData data = sample.Data;

                    texture2D.LoadImage(data.GetValue<byte[]>("Memory"));
                    texture2D.Apply();
                    rawTexture.texture = texture2D;
                }
            }
        }
    }
}
