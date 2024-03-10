using Rti.Dds.Core;
using Rti.Dds.Domain;
using Rti.Dds.Publication;
using Rti.Dds.Subscription;
using Rti.Dds.Topics;
using Rti.Types.Dynamic;
using UnityEngine;

// The DDSHandler class manages the setup and configuration of DDS components within a Unity application.
public class DDSHandler : MonoBehaviour
{
    private QosProvider provider;  // Quality of Service provider for configuring DDS settings.
    public DomainParticipant participant;  // The main DDS domain participant for communication.

    // Called when the script starts. Initializes the DDS participant and QoS provider.
    private void Start()
    {
        provider = new QosProvider("TelexistenceRig.xml");  // Loads QoS settings from the "TelexistenceRig.xml" file.
        participant = DomainParticipantFactory.Instance.CreateParticipant(0, provider.GetDomainParticipantQos());
    }

    // Sets up a DataReader for a specified topic with dynamic data type.
    public DataReader<DynamicData> SetupDataReader(string topicName, DynamicType dynamicData)
    {
        if (participant != null)
        {
            DataReader<DynamicData> reader;
            Topic<DynamicData> topic = participant.CreateTopic(topicName, dynamicData);

            // Creates a subscriber with QoS settings from the "RigQoSLibrary::RigQoSProfile".
            var subscriberQos = provider.GetSubscriberQos("RigQoSLibrary::RigQoSProfile");
            Subscriber subscriber = participant.CreateSubscriber(subscriberQos);

            // Creates a DataReader with QoS settings from the "RigQoSLibrary::RigQoSProfile".
            var readerQos = provider.GetDataReaderQos("RigQoSLibrary::RigQoSProfile");
            reader = subscriber.CreateDataReader(topic, readerQos);

            return reader;
        }
        return null;
    }

    // Sets up a DataWriter for a specified topic with dynamic data type.
    public DataWriter<DynamicData> SetupDataWriter(string topicName, DynamicType dynamicData)
    {
        if (participant != null)
        {
            DataWriter<DynamicData> writer;
            Topic<DynamicData> topic = participant.CreateTopic(topicName, dynamicData);

            // Creates a publisher with QoS settings from the "RigQoSLibrary::RigQoSProfile".
            var publisherQos = provider.GetPublisherQos("RigQoSLibrary::RigQoSProfile");
            Publisher publisher = participant.CreatePublisher(publisherQos);

            // Creates a DataWriter with QoS settings from the "RigQoSLibrary::RigQoSProfile".
            var writerQos = provider.GetDataWriterQos("RigQoSLibrary::RigQoSProfile");
            writer = publisher.CreateDataWriter(topic, writerQos);

            return writer;
        }
        return null;
    }

    // Called when the application is quitting. Disposes of the DDS participant.
    private void OnApplicationQuit()
    {
        participant.Dispose();
    }
}
