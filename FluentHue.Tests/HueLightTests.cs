using FluentHue.Contracts;
using Moq;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluentHue.Tests
{
    public sealed class HueLightTests : TestBase
    {
        [Test]
        public void Construction_NullBridge()
            => Assert.Throws<ArgumentNullException>(() => new HueLight(
                null,
                Guid.NewGuid().ToString(),
                new HueLightContract()
                {
                    Name = Guid.NewGuid().ToString(),
                }));

        [Test]
        public void Construction_NullId()
            => Assert.Throws<ArgumentNullException>(() => new HueLight(
                this.CreateMockBridge(),
                null,
                new HueLightContract()
                {
                    Name = Guid.NewGuid().ToString(),
                }));

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        public void Construction_InvalidId(string id)
            => Assert.Throws<ArgumentException>(() => new HueLight(
                this.CreateMockBridge(),
                id,
                new HueLightContract()
                {
                    Name = Guid.NewGuid().ToString(),
                }));

        [Test]
        public void Construction_NullContract()
            => Assert.Throws<ArgumentNullException>(() => new HueLight(
                this.CreateMockBridge(),
                Guid.NewGuid().ToString(),
                null));

        [Test]
        public void Construction_NullName()
            => Assert.Throws<ArgumentNullException>(() => new HueLight(
                this.CreateMockBridge(),
                Guid.NewGuid().ToString(),
                new HueLightContract()
                {
                    Name = null,
                }));

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        public void Construction_InvalidName(string name)
            => Assert.Throws<ArgumentException>(() => new HueLight(
                this.CreateMockBridge(),
                Guid.NewGuid().ToString(),
                new HueLightContract()
                {
                    Name = name,
                }));

        [Test]
        public void Construction()
        {
            var bridge = this.CreateMockBridge();
            var id = Guid.NewGuid().ToString();
            var contract = new HueLightContract()
            {
                Name = Guid.NewGuid().ToString(),
            };

            var light = new HueLight(bridge, id, contract);

            Assert.NotNull(light);
            Assert.AreEqual(id, light.Id);
            Assert.AreEqual(contract.Name, light.Name);
            Assert.AreEqual(bridge, light.End());
        }

        [Test]
        public void Rename_NullName()
        {
            var bridge = this.CreateMockBridge();
            bridge.WithUser(Guid.NewGuid().ToString());
            var light = this.CreateMockLight(bridge);

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync(typeof(ArgumentNullException), async () => await light.RenameAsync(null).ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => light.Rename(null));
            });
        }

        [Test]
        public void Rename_NoUserSet()
        {
            HueBridge bridge = CreateMockBridge();
            var light = this.CreateMockLight(bridge);

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync(typeof(ArgumentException), async () => await light.RenameAsync(Guid.NewGuid().ToString()).ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => light.Rename(Guid.NewGuid().ToString()));
            });
        }

        [Test]
        public void Rename_NotSuccessfulThrows()
        {
            var bridge = this.CreateMockBridge();
            bridge.WithUser(Guid.NewGuid().ToString());
            var light = this.CreateMockLight(bridge);

            Mock.Get(Client)
                .Setup(c => c.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Method>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse>(
                    new RestResponse()
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        ResponseStatus = ResponseStatus.Completed
                    }));

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync(typeof(Exception), async () => await light.RenameAsync(Guid.NewGuid().ToString()).ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => light.Rename(Guid.NewGuid().ToString()));
            });
        }

        [Test]
        public async Task Rename()
        {
            var name = Guid.NewGuid().ToString();
            var bridge = this.CreateMockBridge();
            bridge.WithUser(Guid.NewGuid().ToString());
            var light = this.CreateMockLight(bridge);

            Mock.Get(Client)
                .Setup(c => c.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Method>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse>(
                    new RestResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseStatus = ResponseStatus.Completed
                    }))
                .Callback<IRestRequest, Method, CancellationToken>((r, m, t) =>
                {
                    Assert.True(r.Parameters.Any());
                    Assert.AreEqual(name, ((HueLightContract)r.Parameters[0].Value).Name);
                    Assert.AreEqual(Method.PUT, m);
                });

            Assert.AreSame(light, await light.RenameAsync(name).ConfigureAwait(false));
            Assert.AreSame(light, light.Rename(name));

            Assert.AreEqual(name, light.Name);
        }

        [Test]
        public void GetCurrentState_NoUserSet()
        {
            HueBridge bridge = CreateMockBridge();
            var light = this.CreateMockLight(bridge);

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync(typeof(ArgumentException), async () => await light.GetCurrentStateAsync().ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => light.GetCurrentState());
            });
        }

        [Test]
        public void GetCurrentState_NotSuccessfulThrows()
        {
            var bridge = this.CreateMockBridge();
            bridge.WithUser(Guid.NewGuid().ToString());
            var light = this.CreateMockLight(bridge);

            Mock.Get(Client)
                .Setup(c => c.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Method>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse>(
                    new RestResponse()
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        ResponseStatus = ResponseStatus.Completed
                    }));

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync(typeof(Exception), async () => await light.GetCurrentStateAsync().ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => light.GetCurrentState());
            });
        }

        [Test]
        public async Task GetCurrentState()
        {
            var bridge = this.CreateMockBridge();
            bridge.WithUser(Guid.NewGuid().ToString());
            var light = this.CreateMockLight(bridge);
            var expected = this.CreateMockState(bridge, light);

            Mock.Get(Client)
                .Setup(c => c.ExecuteAsync(
                    It.IsAny<IRestRequest>(),
                    It.IsAny<Method>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IRestResponse>(
                    new RestResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        ResponseStatus = ResponseStatus.Completed,
                        Content = $"{{\"state\":{{\"on\":{expected.IsOn.ToString().ToLower()},\"bri\":{expected.Brightness},\"xy\":[{expected.ColorX},{expected.ColorY}]}}}}",
                    }));

            var state = await light.GetCurrentStateAsync().ConfigureAwait(false);
            Assert.NotNull(state);
            Assert.AreEqual(expected.IsOn, state.IsOn);
            Assert.AreEqual(expected.Brightness, state.Brightness);
            Assert.AreEqual(expected.ColorX, state.ColorX);
            Assert.AreEqual(expected.ColorY, state.ColorY);

            state = light.GetCurrentState();
            Assert.NotNull(state);
            Assert.AreEqual(expected.IsOn, state.IsOn);
            Assert.AreEqual(expected.Brightness, state.Brightness);
            Assert.AreEqual(expected.ColorX, state.ColorX);
            Assert.AreEqual(expected.ColorY, state.ColorY);

        }
    }
}