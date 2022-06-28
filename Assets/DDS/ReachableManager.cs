using Rti.Dds.Subscription;
using Rti.Types.Dynamic;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReachableManager : MonoBehaviour
{
    private DDSHandler dDSHandler;
    private protected DataReader<DynamicData> reader { get; private set; }

    public Material rend;
    private bool init = false;
    public GameObject controller;

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
            StructType Reachable = typeFactory.BuildStruct()
                .WithName("Reachability")
                .AddMember(new StructMember("isReachable", typeFactory.GetPrimitiveType<bool>()))
                .Create();

            reader = dDSHandler.SetupDataReader("Reachability_Topic", Reachable);
        }
        ProcessData(reader);
    }

    private void ProcessData(AnyDataReader anyReader)
    {
        var reader = (DataReader<DynamicData>)anyReader;
        using var samples = reader.Take();
        foreach (var sample in samples)
        {
            if (sample.Info.ValidData)
            {
                DynamicData data = sample.Data;

                if (data.GetValue<bool>("isReachable") == true)
                {
                    rend.color = Color.green;
                }
                else
                {
                    rend.color = Color.red;
                }
            }
        }
    }
}
