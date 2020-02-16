namespace FluentHue.Tests.Contracts
{
    using FluentHue.Contracts;
    using FluentHue.Serializers;
    using GenFu.ValueGenerators.Internet;
    using NUnit.Framework;
    using RestSharp;
    using RestSharp.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public sealed class HueBridgeMetadataTests
    {
        [Test]
        public void DeserializationTest()
        {
            string id = Guid.NewGuid().ToString();
            string ip = NetworkAddress.IPAddress();

            IRestResponse response = new RestResponse();
            response.Content = $"[{{\"id\":\"{id}\",\"internalipaddress\":\"{ip}\"}}]";

            IRestSerializer serializer = new JsonNetSerializer();
            var deserialized = serializer.Deserialize<IEnumerable<HueBridgeMetadata>>(response);
            Assert.NotNull(deserialized);
            Assert.AreEqual(1, deserialized.Count());

            var metadata = deserialized.First();
            Assert.AreEqual(id, metadata.Id);
            Assert.AreEqual(ip, metadata.InternalIpAddress);
        }
    }
}