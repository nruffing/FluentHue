namespace FluentHue.Tests
{
    using FluentHue.Contracts;
    using GenFu.ValueGenerators.Internet;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public sealed class HueBridgeTests
    {
        [Test]
        public void Construction_NullMetadata()
            => Assert.Throws<ArgumentNullException>(() => new HueBridge((HueBridgeMetadata)null));

        [Test]
        public void Construction_Metadata_NullId()
            => Assert.Throws<ArgumentNullException>(() => new HueBridge(new HueBridgeMetadata()
            {
                Id = null,
                InternalIpAddress = NetworkAddress.IPAddress(),
            }));

        [Test]
        public void Construction_Metadata_NullIp()
            => Assert.Throws<ArgumentNullException>(() => new HueBridge(new HueBridgeMetadata()
            {
                Id = Guid.NewGuid().ToString(),
                InternalIpAddress = null,
            }));

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        public void Contruction_Metadata_InvalidId(string id)
            => Assert.Throws<ArgumentException>(() => new HueBridge(new HueBridgeMetadata()
            {
                Id = id,
                InternalIpAddress = NetworkAddress.IPAddress(),
            }));

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        public void Contruction_Metadata_InvalidIp(string ip)
            => Assert.Throws<ArgumentException>(() => new HueBridge(new HueBridgeMetadata()
            {
                Id = Guid.NewGuid().ToString(),
                InternalIpAddress = ip,
            }));

        [Test]
        public void Construction_Metadata()
        {
            string id = Guid.NewGuid().ToString();
            string ip = NetworkAddress.IPAddress();

            IHueBridge bridge = new HueBridge(new HueBridgeMetadata()
            {
                Id = id,
                InternalIpAddress = ip,
            });

            Assert.NotNull(bridge);
            Assert.AreEqual(id, bridge.Id);
            Assert.AreEqual(ip, bridge.LocalIpAddress);
        }

        [Test]
        public void Construction_Ip()
        {
            string ip = NetworkAddress.IPAddress();

            IHueBridge bridge = new HueBridge(ip);
            
            Assert.NotNull(bridge);
            Assert.AreEqual(ip, bridge.LocalIpAddress);
        }

        [Test]
        public void Contruction_Ip_Null()
            => Assert.Throws<ArgumentNullException>(() => new HueBridge((string)null));

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        public void Contruction_Ip_InvalidIp(string ip)
            => Assert.Throws<ArgumentException>(() => new HueBridge(ip));
    }
}