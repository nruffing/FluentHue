namespace FluentHue.Tests.Contracts
{
    using FluentHue.Contracts;
    using FluentHue.Serializers;
    using NUnit.Framework;
    using RestSharp;
    using RestSharp.Serialization;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public sealed class HueBridgeMetadataTests
    {
        [Test]
        public void DeserializationTest()
        {
            IRestResponse response = new RestResponse();
            response.Content = "[{\"id\":\"001788fffe4c2912\",\"internalipaddress\":\"192.168.1.7\"}]";

            IRestSerializer serializer = new JsonNetSerializer();
            var deserialized = serializer.Deserialize<IEnumerable<HueBridgeMetadata>>(response);
            Assert.NotNull(deserialized);
            Assert.AreEqual(1, deserialized.Count());

            var metadata = deserialized.First();
            Assert.AreEqual("001788fffe4c2912", metadata.Id);
            Assert.AreEqual("192.168.1.7", metadata.InternalIpAddress);
        }
    }
}