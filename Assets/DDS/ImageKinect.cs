using Rti.Dds.Publication;
using Rti.Dds.Subscription;
using Rti.Types.Dynamic;
using UnityEngine;
using UnityEngine.UI;

public class ImageKinect : MonoBehaviour
{
    private DDSHandler dDSHandler;

    private protected DataReader<DynamicData> reader { get; private set; }
    private DynamicData sample = null;
    private bool init = false;
    public RawImage rawTexture;
    private Texture2D texture2D;


    private void Start()
    {
        texture2D = new Texture2D(320, 288);
        dDSHandler = gameObject.GetComponent<DDSHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!init)
        {
            init = true;
            var typeFactory = DynamicTypeFactory.Instance;
            StructType PointCloud = typeFactory.BuildStruct()
                .WithName("PointCloud")
                .AddMember(new StructMember("sequence_Memory", typeFactory.CreateSequence(typeFactory.GetPrimitiveType<float>(), 2000000)))
                .Create();
            reader = dDSHandler.SetupDataReader("PointCloud_Topic", PointCloud);
        }
        ProcessData(reader);
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

                float[] buffer = data.GetValue<float[]>("sequence_Memory");

                for (int i = 0; i < buffer.Length; i += 6)
                {
                    texture2D.SetPixel((int)(buffer[i]),(int)(-buffer[i + 1]), new Color((byte)buffer[i + 5], (byte)buffer[i + 4], (byte)buffer[i + 3]));
                }

                texture2D.Apply();
                rawTexture.texture = texture2D;
            }
        }
    }
}
