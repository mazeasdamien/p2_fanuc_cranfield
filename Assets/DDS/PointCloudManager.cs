using Rti.Dds.Subscription;
using Rti.Types.Dynamic;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

// The PointCloudManager class handles DDS communication and updates a point cloud visualization in Unity.
public class PointCloudManager : MonoBehaviour
{
    public VisualEffect effect; // Reference to the Visual Effect Graph component for point cloud visualization.

    Mesh mesh; // Mesh object for storing point cloud geometry.
    readonly List<Vector3> vertices = new List<Vector3>(); // List to store point cloud vertices.
    readonly List<Color32> colors = new List<Color32>(); // List to store point cloud colors.
    readonly List<int> indices = new List<int>(); // List to store point cloud indices.

    private bool init = false; // Flag to ensure initialization only occurs once.

    private DDSHandler dDSHandler; // Reference to the DDSHandler component for DDS communication.
    private protected DataReader<DynamicData> reader { get; private set; } // DDS DataReader for dynamic data.

    // Called when the script starts. Initializes the DDSHandler reference.
    private void Start()
    {
        dDSHandler = gameObject.GetComponent<DDSHandler>();
    }

    // Called every frame. Sets up DDS data structures on the first frame and updates point cloud visualization.
    private void Update()
    {
        if (!init)
        {
            init = true;

            // DynamicType setup for the "PointCloud" data structure.
            var typeFactory = DynamicTypeFactory.Instance;
            StructType PointCloud = typeFactory.BuildStruct()
                .WithName("PointCloud")
                .AddMember(new StructMember("sequence_Memory", typeFactory.CreateSequence(typeFactory.GetPrimitiveType<float>(), 600000)))
                .Create();

            // Setup DDS DataReader for the "PointCloud_Topic".
            reader = dDSHandler.SetupDataReader("PointCloud_Topic", PointCloud);
        }

        // Process incoming DDS data and update point cloud visualization.
        ProcessData(reader);
    }

    // Processes DDS data received from the DataReader and updates point cloud visualization.
    void ProcessData(AnyDataReader anyReader)
    {
        var reader = (DataReader<DynamicData>)anyReader;
        using var samples = reader.Take();
        foreach (var sample in samples)
        {
            if (sample.Info.ValidData)
            {
                DynamicData data = sample.Data;

                // Clear existing point cloud data.
                vertices.Clear();
                colors.Clear();
                indices.Clear();
                mesh = new Mesh
                {
                    indexFormat = UnityEngine.Rendering.IndexFormat.UInt32,
                };

                // Retrieve point cloud data from DDS sample.
                float[] buffer = data.GetValue<float[]>("sequence_Memory");
                int num = buffer.Length / 6;

                // Create indices for the point cloud mesh.
                for (int i = 0; i < num; i++)
                {
                    indices.Add(i);
                }

                mesh.vertices = new Vector3[num];
                mesh.colors32 = new Color32[num];

                mesh.SetIndices(indices, MeshTopology.Points, 0);

                // Populate vertices and colors from DDS sample data.
                for (int i = 0; i < buffer.Length; i += 6)
                {
                    vertices.Add(new Vector3
                    {
                        x = buffer[i] * 0.001f,
                        y = -buffer[i + 1] * 0.001f,
                        z = buffer[i + 2] * 0.001f
                    });

                    colors.Add(new Color32
                    {
                        b = (byte)buffer[i + 3],
                        g = (byte)buffer[i + 4],
                        r = (byte)buffer[i + 5],
                        a = 255
                    });
                }

                // Update the mesh with the new point cloud data.
                mesh.vertices = vertices.ToArray();
                mesh.colors32 = colors.ToArray();

                // Set the updated mesh in the Visual Effect Graph.
                effect.SetMesh("RemoteData", mesh);
            }
        }
    }
}
