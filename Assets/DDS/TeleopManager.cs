using Rti.Dds.Publication;
using Rti.Dds.Subscription;
using Rti.Types.Dynamic;
using UnityEngine;

public class TeleopManager : MonoBehaviour
{
    public DDSHandler dDSHandler;
    private protected DataWriter<DynamicData> writer { get; private set; }
    private DynamicData sample = null;
    private bool init = false;
    private Transform updater;
    private Vector3 tempPos = new();
    private Vector3 tempRot = new();

    private void Update()
    {
        if (!init)
        {
            init = true;

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

            writer = dDSHandler.SetupDataWriter("OperatorNewPose_Topic", OperatorNewPose);
            sample = new DynamicData(OperatorNewPose);
            updater = gameObject.transform;

        }

        UpdateOperatorPose();
    }

    public void UpdateOperatorPose()
    {

        if (gameObject.transform.localPosition != tempPos || gameObject.transform.localEulerAngles != tempRot)
        {
            sample.SetValue("X", -updater.localPosition.x * 1000);
            sample.SetValue("Y", updater.localPosition.y * 1000);
            sample.SetValue("Z", updater.localPosition.z * 1000);
            sample.SetValue("W", CreateFanucWPRFromQuaternion(updater.localRotation).x);
            sample.SetValue("P", CreateFanucWPRFromQuaternion(updater.localRotation).y);
            sample.SetValue("R", CreateFanucWPRFromQuaternion(updater.localRotation).z);

            writer.Write(sample);
            tempPos = gameObject.transform.localPosition;
            tempRot = gameObject.transform.localEulerAngles;
        }
    }

    private Vector3 CreateFanucWPRFromQuaternion(Quaternion q)
    {
        float W = Mathf.Atan2(2 * ((q.w * q.x) + (q.y * q.z)), 1 - 2 * (Mathf.Pow(q.x, 2) + Mathf.Pow(q.y, 2))) * (180 / Mathf.PI);
        float P = Mathf.Asin(2 * ((q.w * q.y) - (q.z * q.x))) * (180 / Mathf.PI);
        float R = Mathf.Atan2(2 * ((q.w * q.z) + (q.x * q.y)), 1 - 2 * (Mathf.Pow(q.y, 2) + Mathf.Pow(q.z, 2))) * (180 / Mathf.PI);

        return new Vector3(W, -P, -R);
    }
}