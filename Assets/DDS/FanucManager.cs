using Rti.Dds.Subscription;
using Rti.Types.Dynamic;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FanucManager : MonoBehaviour
{
    public List<ArticulationBody> joints;
    public Transform worldPosition;
    private DDSHandler dDSHandler;
    private protected DataReader<DynamicData> reader { get; private set; }

    private bool init = false;

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

            reader = dDSHandler.SetupDataReader("RobotState_Topic", RobotState);
        }


        ProcessData(reader);
    }

    public Quaternion CreateQuaternionFromFanucWPR(float W, float P, float R)
    {
        float Wrad = W * (Mathf.PI / 180);
        float Prad = P * (Mathf.PI / 180);
        float Rrad = R * (Mathf.PI / 180);

        float qx = (Mathf.Cos(Rrad / 2) * Mathf.Cos(Prad / 2) * Mathf.Sin(Wrad / 2)) - (Mathf.Sin(Rrad / 2) * Mathf.Sin(Prad / 2) * Mathf.Cos(Wrad / 2));
        float qy = (Mathf.Cos(Rrad / 2) * Mathf.Sin(Prad / 2) * Mathf.Cos(Wrad / 2)) + (Mathf.Sin(Rrad / 2) * Mathf.Cos(Prad / 2) * Mathf.Sin(Wrad / 2));
        float qz = (Mathf.Sin(Rrad / 2) * Mathf.Cos(Prad / 2) * Mathf.Cos(Wrad / 2)) - (Mathf.Cos(Rrad / 2) * Mathf.Sin(Prad / 2) * Mathf.Sin(Wrad / 2));
        float qw = (Mathf.Cos(Rrad / 2) * Mathf.Cos(Prad / 2) * Mathf.Cos(Wrad / 2)) + (Mathf.Sin(Rrad / 2) * Mathf.Sin(Prad / 2) * Mathf.Sin(Wrad / 2));

        return new Quaternion(qx, qy, qz, qw);
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

                worldPosition.localPosition = new Vector3(-(float)data.GetValue<double>("X") / 1000, (float)data.GetValue<double>("Y") / 1000, (float)data.GetValue<double>("Z") / 1000);
                Vector3 eulerAngles = CreateQuaternionFromFanucWPR((float)data.GetValue<double>("W"), (float)data.GetValue<double>("P"), (float)data.GetValue<double>("R")).eulerAngles;
                worldPosition.localEulerAngles = new Vector3(eulerAngles.x, -eulerAngles.y, -eulerAngles.z);

                //Debug.Log(worldPosition.localPosition.ToString() + "  " + worldPosition.localEulerAngles.ToString());
                //Debug.Log((float)data.GetValue<double>("J1") + "  " + (float)data.GetValue<double>("J2") + "   " + ((float)data.GetValue<double>("J3") + (float)data.GetValue<double>("J2")) + "   " + (float)data.GetValue<double>("J4") + "   " + (float)data.GetValue<double>("J5") + "   " + (float)data.GetValue<double>("J6"));

                var J1drive = joints[0].xDrive;
                var J2drive = joints[1].xDrive;
                var J3drive = joints[2].xDrive;
                var J4drive = joints[3].xDrive;
                var J5drive = joints[4].xDrive;
                var J6drive = joints[5].xDrive;

                J1drive.target = (float)data.GetValue<double>("J1");
                J2drive.target = (float)data.GetValue<double>("J2");
                J3drive.target = (float)data.GetValue<double>("J3") + (float)data.GetValue<double>("J2");
                J4drive.target = (float)data.GetValue<double>("J4");
                J5drive.target = (float)data.GetValue<double>("J5");
                J6drive.target = (float)data.GetValue<double>("J6");

                joints[0].xDrive = J1drive;
                joints[1].xDrive = J2drive;
                joints[2].xDrive = J3drive;
                joints[3].xDrive = J4drive;
                joints[4].xDrive = J5drive;
                joints[5].xDrive = J6drive;
            }
        }

    }
}