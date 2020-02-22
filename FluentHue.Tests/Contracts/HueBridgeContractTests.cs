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

    public sealed class HueBridgeContractTests : TestBase
    {
        [Test]
        public void DeserializationTest()
        {
            string id = Guid.NewGuid().ToString();
            string ip = NetworkAddress.IPAddress();

            IRestResponse response = new RestResponse();
            response.Content = $"[{{\"id\":\"{id}\",\"internalipaddress\":\"{ip}\"}}]";

            IRestSerializer serializer = new JsonNetSerializer();
            var deserialized = serializer.Deserialize<IEnumerable<HueBridgeContract>>(response);
            Assert.NotNull(deserialized);
            Assert.AreEqual(1, deserialized.Count());

            var contract = deserialized.First();
            Assert.AreEqual(id, contract.Id);
            Assert.AreEqual(ip, contract.InternalIpAddress);
        }
    }
}