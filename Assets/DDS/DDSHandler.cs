using Rti.Dds.Core;
using Rti.Dds.Domain;
using Rti.Dds.Publication;
using Rti.Dds.Subscription;
using Rti.Dds.Topics;
using Rti.Types.Dynamic;
using UnityEngine;

public class DDSHandler : MonoBehaviour
{
    private QosProvider provider;
    public DomainParticipant participant;

    private void Start()
    {
        provider = new QosProvider("TelexistenceRig.xml");
        participant = DomainParticipantFactory.Instance.CreateParticipant(0, provider.GetDomainParticipantQos());
    }

    public DataReader<DynamicData> SetupDataReader(string topicName, DynamicType dynamicData)
    {
        if (participant != null)
        {
            DataReader<DynamicData> reader;
            Topic<DynamicData> topic = participant.CreateTopic(topicName, dynamicData);
            var subscriberQos = provider.GetSubscriberQos("RigQoSLibrary::RigQoSProfile");
            Subscriber subscriber = participant.CreateSubscriber(subscriberQos);
            var readerQos = provider.GetDataReaderQos("RigQoSLibrary::RigQoSProfile");
            reader = subscriber.CreateDataReader(topic, readerQos);
            return reader;
        }
        return null;
    }

    public DataWriter<DynamicData> SetupDataWriter(string topicName, DynamicType dynamicData)
    {
        if (participant != null)
        {
            DataWriter<DynamicData> writer;
            Topic<DynamicData> topic = participant.CreateTopic(topicName, dynamicData);
            var publisherQos = provider.GetPublisherQos("RigQoSLibrary::RigQoSProfile");
            Publisher publisher = participant.CreatePublisher(publisherQos);
            var writerQos = provider.GetDataWriterQos("RigQoSLibrary::RigQoSProfile");
            writer = publisher.CreateDataWriter(topic, writerQos);
            return writer;
        }
        return null;
    }

    private void OnApplicationQuit()
    {
        participant.Dispose();
    }
}