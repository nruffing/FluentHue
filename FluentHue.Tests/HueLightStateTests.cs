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
            };

            var bridge = this.CreateMockBridge();

            var state = new HueLightState(bridge, this.CreateMockLight(bridge), contract);
            Assert.NotNull(state);
            Assert.AreEqual(contract.IsOn, state.IsOn);
            Assert.AreEqual(contract.Brightness, state.Brightness);
        }
    }
}