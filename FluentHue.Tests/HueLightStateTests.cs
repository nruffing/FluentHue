namespace FluentHue.Tests
{
    using FluentHue.Contracts;
    using Moq;
    using NUnit.Framework;
    using RestSharp;
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class HueLightStateTests : TestBase
    {
        [Test]
        public void Construction_NullBridge()
            => Assert.Throws<ArgumentNullException>(() => new HueLightState(
                    null,
                    this.CreateMockLight(this.CreateMockBridge()),
                    new HueLightStateContract()));

        [Test]
        public void Construction_NullLight()
            => Assert.Throws<ArgumentNullException>(() => new HueLightState(
                    this.CreateMockBridge(),
                    null,
                    new HueLightStateContract()));

        [Test]
        public void Construction_NullContract()
        {
            var bridge = this.CreateMockBridge();
            Assert.Throws<ArgumentNullException>(() => new HueLightState(
                    bridge,
                    this.CreateMockLight(bridge),
                    null));
        }

        [Test]
        public void ConstructionAndReverseMapping()
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
            var light = this.CreateMockLight(bridge);

            var state = new HueLightState(bridge, light, expected);
            Assert.NotNull(state);
            Assert.AreEqual(expected.IsOn, state.IsOn);
            Assert.AreEqual(expected.Brightness, state.Brightness);
            Assert.AreEqual(expected.Color[0], state.ColorX);
            Assert.AreEqual(expected.Color[1], state.ColorY);
            Assert.AreEqual(light, state.End());

            var contract = this.GetMapper().Map<HueLightStateContract>(state);
            Assert.NotNull(contract);
            Assert.AreEqual(expected.IsOn, contract.IsOn);
            Assert.AreEqual(expected.Brightness, contract.Brightness);
            Assert.AreEqual(expected.Color, contract.Color);
        }

        [Test]
        public void Toggle_NoUserSet()
        {
            var bridge = CreateMockBridge();
            var light = this.CreateMockLight(bridge);
            var state = this.CreateMockState(bridge, light);

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync(typeof(ArgumentException), async () => await state.ToggleAsync().ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => state.Toggle());
            });
        }

        [Test]
        public void Toggle_NotSuccessfulThrows()
        {
            var bridge = this.CreateMockBridge();
            bridge.WithUser(Guid.NewGuid().ToString());
            var light = this.CreateMockLight(bridge);
            var state = this.CreateMockState(bridge, light);

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
                Assert.ThrowsAsync(typeof(Exception), async () => await state.ToggleAsync().ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => state.Toggle());
            });
        }

        [Test]
        public async Task Toggle()
        {
            var bridge = this.CreateMockBridge();
            bridge.WithUser(Guid.NewGuid().ToString());
            var light = this.CreateMockLight(bridge);
            var state = this.CreateMockState(bridge, light);
            var expected = !state.IsOn;

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
                    Assert.NotNull(r.Parameters[0].Value as HueLightStateContract);
                    Assert.AreEqual(Method.PUT, m);
                }); 

            Assert.AreSame(state, await state.ToggleAsync().ConfigureAwait(false));
            Assert.AreEqual(expected, state.IsOn);

            Assert.AreSame(state, state.Toggle());
            Assert.AreEqual(!expected, state.IsOn);
        }

        [Test]
        public void SetState_NoUserSet()
        {
            var bridge = CreateMockBridge();
            var light = this.CreateMockLight(bridge);
            var state = this.CreateMockState(bridge, light);
            var expected = !state.IsOn;

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync(typeof(ArgumentException), async () => await state.SetStateAsync(expected).ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => state.SetState(!expected));
            });
        }

        [Test]
        public void SetState_NotSuccessfulThrows()
        {
            var bridge = this.CreateMockBridge();
            bridge.WithUser(Guid.NewGuid().ToString());
            var light = this.CreateMockLight(bridge);
            var state = this.CreateMockState(bridge, light);
            var expected = !state.IsOn;

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
                Assert.ThrowsAsync(typeof(Exception), async () => await state.SetStateAsync(expected).ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => state.SetState(!expected));
            });
        }

        [Test]
        public async Task SetState()
        {
            var bridge = this.CreateMockBridge();
            bridge.WithUser(Guid.NewGuid().ToString());
            var light = this.CreateMockLight(bridge);
            var state = this.CreateMockState(bridge, light);
            var expected = !state.IsOn;

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
                    Assert.AreEqual(Method.PUT, m);

                    var contract = r.Parameters[0].Value as HueLightStateContract;
                    Assert.NotNull(contract);
                    Assert.AreEqual(expected, contract.IsOn);
                    Assert.AreEqual(state.Brightness, contract.Brightness);
                    Assert.AreEqual(state.ColorX, contract.Color[0]);
                    Assert.AreEqual(state.ColorY, contract.Color[1]);
                });

            Assert.AreSame(state, await state.SetStateAsync(expected).ConfigureAwait(false));
            Assert.AreEqual(expected, state.IsOn);

            Assert.AreSame(state, state.SetState(expected));
            Assert.AreEqual(expected, state.IsOn);
        }

        [Test]
        public void SetBrightness_NoUserSet()
        {
            var bridge = CreateMockBridge();
            var light = this.CreateMockLight(bridge);
            var state = this.CreateMockState(bridge, light);

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync(typeof(ArgumentException), async () => await state.SetBrightnessAsync(this.GetRandomByte()).ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => state.SetBrightness(this.GetRandomByte()));
            });
        }

        [Test]
        [TestCase(0)]
        [TestCase(255)]
        public void SetBrightness_InvalidValue(byte invalidValue)
        {
            var bridge = CreateMockBridge();
            var light = this.CreateMockLight(bridge);
            var state = this.CreateMockState(bridge, light);

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync(typeof(ArgumentOutOfRangeException), async () => await state.SetBrightnessAsync(invalidValue).ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => state.SetBrightness(invalidValue));
            });
        }

        [Test]
        public void SetBrightness_NotSuccessfulThrows()
        {
            var bridge = this.CreateMockBridge();
            bridge.WithUser(Guid.NewGuid().ToString());
            var light = this.CreateMockLight(bridge);
            var state = this.CreateMockState(bridge, light);

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
                Assert.ThrowsAsync(typeof(Exception), async () => await state.SetBrightnessAsync(this.GetRandomByte()).ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => state.SetBrightness(this.GetRandomByte()));
            });
        }

        [Test]
        public async Task SetBrightness()
        {
            var bridge = this.CreateMockBridge();
            bridge.WithUser(Guid.NewGuid().ToString());
            var light = this.CreateMockLight(bridge);
            var state = this.CreateMockState(bridge, light);
            var expected = this.GetRandomByte();

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
                    Assert.AreEqual(Method.PUT, m);

                    var contract = r.Parameters[0].Value as HueLightStateContract;
                    Assert.NotNull(contract);
                    Assert.AreEqual(state.IsOn, contract.IsOn);
                    Assert.AreEqual(expected, contract.Brightness);
                    Assert.AreEqual(state.ColorX, contract.Color[0]);
                    Assert.AreEqual(state.ColorY, contract.Color[1]);
                });

            Assert.AreSame(state, await state.SetBrightnessAsync(expected).ConfigureAwait(false));
            Assert.AreEqual(expected, state.Brightness);

            Assert.AreSame(state, state.SetBrightness(expected));
            Assert.AreEqual(expected, state.Brightness);
        }

        [Test]
        public void SetColor_NoUserSet()
        {
            var bridge = CreateMockBridge();
            var light = this.CreateMockLight(bridge);
            var state = this.CreateMockState(bridge, light);

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync(typeof(ArgumentException), async () => await state.SetColorAsync(this.GetRandomFloat(), this.GetRandomFloat()).ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => state.SetColor(this.GetRandomFloat(), this.GetRandomFloat()));
            });
        }

        [Test]
        [TestCase(-0.1f)]
        [TestCase(1.1f)]
        public void SetColor_InvalidValue(float invalidValue)
        {
            var bridge = CreateMockBridge();
            var light = this.CreateMockLight(bridge);
            var state = this.CreateMockState(bridge, light);

            Assert.Multiple(() =>
            {
                Assert.ThrowsAsync(typeof(ArgumentOutOfRangeException), async () => await state.SetColorAsync(invalidValue, this.GetRandomFloat()).ConfigureAwait(false));
                Assert.ThrowsAsync(typeof(ArgumentOutOfRangeException), async () => await state.SetColorAsync(this.GetRandomFloat(), invalidValue).ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => state.SetColor(invalidValue, this.GetRandomFloat()));
                Assert.Throws<AggregateException>(() => state.SetColor(this.GetRandomFloat(), invalidValue));
            });
        }

        [Test]
        public void SetColor_NotSuccessfulThrows()
        {
            var bridge = this.CreateMockBridge();
            bridge.WithUser(Guid.NewGuid().ToString());
            var light = this.CreateMockLight(bridge);
            var state = this.CreateMockState(bridge, light);

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
                Assert.ThrowsAsync(typeof(Exception), async () => await state.SetColorAsync(this.GetRandomFloat(), this.GetRandomFloat()).ConfigureAwait(false));
                Assert.Throws<AggregateException>(() => state.SetColor(this.GetRandomFloat(), this.GetRandomFloat()));
            });
        }

        [Test]
        public async Task SetColor()
        {
            var bridge = this.CreateMockBridge();
            bridge.WithUser(Guid.NewGuid().ToString());
            var light = this.CreateMockLight(bridge);
            var state = this.CreateMockState(bridge, light);
            var expectedX = this.GetRandomFloat();
            var expectedY = this.GetRandomFloat();

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
                    Assert.AreEqual(Method.PUT, m);

                    var contract = r.Parameters[0].Value as HueLightStateContract;
                    Assert.NotNull(contract);
                    Assert.AreEqual(state.IsOn, contract.IsOn);
                    Assert.AreEqual(state.Brightness, contract.Brightness);
                    Assert.AreEqual(expectedX, contract.Color[0]);
                    Assert.AreEqual(expectedY, contract.Color[1]);
                });

            Assert.AreSame(state, await state.SetColorAsync(expectedX, expectedY).ConfigureAwait(false));
            Assert.AreEqual(expectedX, state.ColorX);
            Assert.AreEqual(expectedY, state.ColorY);

            Assert.AreSame(state, state.SetColor(expectedX, expectedY));
            Assert.AreEqual(expectedX, state.ColorX);
            Assert.AreEqual(expectedY, state.ColorY);
        }
    }
}