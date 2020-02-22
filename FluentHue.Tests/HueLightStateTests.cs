namespace FluentHue.Tests
{
    using FluentHue.Contracts;
    using NUnit.Framework;

    public sealed class HueLightStateTests : TestBase
    {
        [Test]
        public void Construction()
        {
            var contract = new HueLightStateContract()
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

            var state = new HueLightState(bridge, this.CreateMockLight(bridge), contract);
            Assert.NotNull(state);
            Assert.AreEqual(contract.IsOn, state.IsOn);
            Assert.AreEqual(contract.Brightness, state.Brightness);
            Assert.AreEqual(contract.Color[0], state.ColorX);
            Assert.AreEqual(contract.Color[1], state.ColorY);
        }
    }
}