namespace FluentHue.Tests
{
    using FluentHue.Contracts;
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
                InternalIpAddress = "192.168.1.1",
            }));

        [Test]
        public void Construction_Metadata_NullIp()
            => Assert.Throws<ArgumentNullException>(() => new HueBridge(new HueBridgeMetadata()
            {
                Id = "1234",
                InternalIpAddress = null,
            }));

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        public void Contruction_Metadata_InvalidId(string id)
            => Assert.Throws<ArgumentException>(() => new HueBridge(new HueBridgeMetadata()
            {
                Id = id,
                InternalIpAddress = "192.168.1.1",
            }));

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        public void Contruction_Metadata_InvalidIp(string ip)
            => Assert.Throws<ArgumentException>(() => new HueBridge(new HueBridgeMetadata()
            {
                Id = "1234",
                InternalIpAddress = ip,
            }));

        [Test]
        public void Construction_Metadata()
        {
            string id = "1234";
            string ip = "192.168.1.1";

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
            string ip = "192.168.1.1";

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