namespace FluentHue.Tests.Contracts
{
    using FluentHue.Contracts;
    using FluentHue.Serializers;
    using NUnit.Framework;
    using RestSharp;
    using RestSharp.Serialization;
    
    public sealed class HueLightContractTests : TestBase
    {
        /* 
            {
                "state": {
                        "on": false,
                    "bri": 254,
                    "hue": 48016,
                    "sat": 102,
                    "effect": "none",
                    "xy": [
                        0.2999,
                        0.2498
                    ],
                    "ct": 153,
                    "alert": "select",
                    "colormode": "xy",
                    "mode": "homeautomation",
                    "reachable": true
                },
                "swupdate": {
                    "state": "noupdates",
                    "lastinstall": "2017-12-03T15:49:47"
                },
                "type": "Extended color light",
                "name": "Color-Bedroom",
                "modelid": "LCT014",
                "manufacturername": "Philips",
                "productname": "Hue color lamp",
                "capabilities": {
                    "certified": true,
                    "control": {
                        "mindimlevel": 1000,
                        "maxlumen": 800,
                        "colorgamuttype": "C",
                        "colorgamut": [
                            [
                                0.6915,
                                0.3083
                            ],
                            [
                                0.1700,
                                0.7000
                            ],
                            [
                                0.1532,
                                0.0475
                            ]
                        ],
                        "ct": {
                            "min": 153,
                            "max": 500
                        }
                    },
                    "streaming": {
                        "renderer": true,
                        "proxy": true
                    }
                },
                "config": {
                    "archetype": "sultanbulb",
                    "function": "mixed",
                    "direction": "omnidirectional",
                    "startup": {
                        "mode": "powerfail",
                        "configured": true
                    }
                },
                "uniqueid": "00:17:88:01:02:4c:de:b8-0b",
                "swversion": "1.46.13_r26312",
                "swconfigid": "56D25448",
                "productid": "Philips-LCT014-1-A19ECLv4"
            }
        */

        [Test]
        public void DeserializationTest()
        {
            IRestResponse response = new RestResponse();
            response.Content = "{\"state\":{ \"on\":false,\"bri\":254,\"hue\":48016,\"sat\":102,\"effect\":\"none\",\"xy\":[0.2999,0.2498],\"ct\":153,\"alert\":\"select\",\"colormode\":\"xy\",\"mode\":\"homeautomation\",\"reachable\":true},\"swupdate\":{\"state\":\"noupdates\",\"lastinstall\":\"2017-12-03T15:49:47\"},\"type\":\"Extended color light\",\"name\":\"Color-Bedroom\",\"modelid\":\"LCT014\",\"manufacturername\":\"Philips\",\"productname\":\"Hue color lamp\",\"capabilities\":{\"certified\":true,\"control\":{\"mindimlevel\":1000,\"maxlumen\":800,\"colorgamuttype\":\"C\",\"colorgamut\":[[0.6915,0.3083],[0.1700,0.7000],[0.1532,0.0475]],\"ct\":{\"min\":153,\"max\":500}},\"streaming\":{\"renderer\":true,\"proxy\":true}},\"config\":{\"archetype\":\"sultanbulb\",\"function\":\"mixed\",\"direction\":\"omnidirectional\",\"startup\":{\"mode\":\"powerfail\",\"configured\":true}},\"uniqueid\":\"00:17:88:01:02:4c:de:b8-0b\",\"swversion\":\"1.46.13_r26312\",\"swconfigid\":\"56D25448\",\"productid\":\"Philips-LCT014-1-A19ECLv4\"}";

            IRestSerializer serializer = new JsonNetSerializer();
            var deserialized = serializer.Deserialize<HueLightContract>(response);
            Assert.NotNull(deserialized);

            Assert.AreEqual("Color-Bedroom", deserialized.Name);
        }
    }
}