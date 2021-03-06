﻿namespace FluentHue.Tests
{
    using AutoMapper;
    using FluentHue.Contracts;
    using GenFu.ValueGenerators.Internet;
    using Moq;
    using NUnit.Framework;
    using RestSharp;
    using System;

    [TestFixture]
    public abstract class TestBase : RandomBase
    {
        protected IRestClient Client = Mock.Of<IRestClient>();

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            Container.Initialize();
            Container.Instance.Options.AllowOverridingRegistrations = true;
            Container.Instance.Register<IRestClient>(() => Client, SimpleInjector.Lifestyle.Transient);
        }

        [SetUp]
        public virtual void SetUp()
        {            
            Client = Mock.Of<IRestClient>();
        }

        protected HueBridge CreateMockBridge()
        {
            var bridge = new HueBridge(NetworkAddress.IPAddress());
            Assume.That(bridge != null);
            return bridge;
        }

        protected HueLight CreateMockLight(HueBridge bridge)
        {
            var light = new HueLight(
                bridge,
                Guid.NewGuid().ToString(),
                new HueLightContract()
                {
                    Name = Guid.NewGuid().ToString(),
                });
            Assume.That(light != null);
            return light;
        }

        protected HueLightState CreateMockState(HueBridge bridge, HueLight light)
        {
            var state = new HueLightState(
                bridge,
                light,
                new HueLightStateContract()
                {
                    IsOn = this.GetRandomBool(),
                    Brightness = this.GetRandomByte(),
                    Color = new float[]
                    {
                        GetRandomFloat(),
                        GetRandomFloat()
                    }
                });
            Assume.That(state != null);
            return state;
        }

        protected IMapper GetMapper()
            => Container.Instance.GetInstance<IMapper>();
    }
}