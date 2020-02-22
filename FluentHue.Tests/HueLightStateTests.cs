namespace FluentHue.Tests
{
    using FluentHue.Contracts;
    using NUnit.Framework;

    public sealed class HueLightStateTests : TestBase
    {
        [Test]
        public void ContractMappingTest()
        {
            var expected = new HueLightStateContract()
            {
                IsOn = this.GetRandomBool(),
                Brightness = this.GetRandomByte(),
                Color = new float[]
                {
                    GetRandomFloat(),
                    GetRandomFloat()
                }
            };

            var bridge = this.CreateMockBridge();

            var state = new HueLightState(bridge, this.CreateMockLight(bridge), expected);
            Assert.NotNull(state);
            Assert.AreEqual(expected.IsOn, state.IsOn);
            Assert.AreEqual(expected.Brightness, state.Brightness);
            Assert.AreEqual(expected.Color[0], state.ColorX);
            Assert.AreEqual(expected.Color[1], state.ColorY);

            var contract = this.GetMapper().Map<HueLightStateContract>(state);
            Assert.NotNull(contract);
            Assert.AreEqual(expected.IsOn, contract.IsOn);
            Assert.AreEqual(expected.Brightness, contract.Brightness);
            Assert.AreEqual(expected.Color, contract.Color);
        }
    }
}