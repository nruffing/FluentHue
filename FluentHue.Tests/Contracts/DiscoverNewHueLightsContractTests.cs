using FluentHue.Contracts;
using FluentHue.Serializers;
using NUnit.Framework;
using RestSharp;
using RestSharp.Serialization;
using System;

namespace FluentHue.Tests.Contracts
{
    public sealed class DiscoverNewHueLightsContractTests : TestBase
    {
        [Test]
        public void SerializationTest()
        {
            var contract = new DiscoverNewHueLightsContract()
            {
                SerialNumbers = new string[]
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                },
            };

            IRestSerializer serializer = new JsonNetSerializer();
            var serialized = serializer.Serialize(contract);

            Assert.AreEqual($"{{\"deviceid\":[\"{contract.SerialNumbers[0]}\",\"{contract.SerialNumbers[1]}\"]}}", serialized);

            var deserialized = serializer.Deserialize<DiscoverNewHueLightsContract>(new RestResponse()
            {
                Content = serialized,
            });

            Assert.NotNull(deserialized);
            Assert.AreEqual(contract.SerialNumbers, deserialized.SerialNumbers);
        }
    }
}