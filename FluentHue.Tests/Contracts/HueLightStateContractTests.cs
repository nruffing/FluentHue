namespace FluentHue.Tests.Contracts
{
    using FluentHue.Contracts;
    using FluentHue.Serializers;
    using NUnit.Framework;
    using RestSharp;
    using RestSharp.Serialization;

    public sealed class HueLightStateContractTests : TestBase
    {
        [Test]
        public void SerializationTest()
        {
            var contract = new HueLightStateContract()
            {
                IsOn = this.GetRandomBool(),
                Brightness = this.GetRandomByte(),
                Color = new float[]
                {
                    this.GetRandomFloat(),
                    this.GetRandomFloat(),
                },
            };

            IRestSerializer serializer = new JsonNetSerializer();
            var serialized = serializer.Serialize(contract);

            Assert.AreEqual($"{{\"on\":{contract.IsOn.ToString().ToLower()},\"bri\":{contract.Brightness},\"xy\":[{contract.Color[0]},{contract.Color[1]}]}}", serialized);

            var deserialized = serializer.Deserialize<HueLightStateContract>(new RestResponse()
            {
                Content = serialized,
            });

            Assert.NotNull(deserialized);
            Assert.AreEqual(contract.IsOn, deserialized.IsOn);
            Assert.AreEqual(contract.Brightness, deserialized.Brightness);
            Assert.AreEqual(contract.Color, deserialized.Color);
        }
    }
}