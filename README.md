# FluentHue (WIP)
Fluent interface for making REST API calls to a Philips Hue Bridge. Almost all functionality has both synchronous and asynchronous support. This is a work in progress and the following functionality is currently supported.

 * Set, toggle, and retrieve the current state of a light which currently includes whether the light is on or off, the light's brightness, and the light's color (if supported).
 * Rename a light.
 * Initiate the Hue bridge to search for new lights with or without looking for specific serial numbers.

Additional features will continue to be added but if you are looking for something specific please [email me](mailto:nicholasruffing70@gmail.com?subject=FluentHue) or create a new issue.

More information regarding the Philips Hue bridge REST API can be found [here](https://developers.meethue.com/develop/get-started-2/).

## Select Hue Bridge

For cases when you only have one Hue bridge on your local network you can just select the first Hue bridge found using the bridge's broker server discover process.

```c#
var bridge = HueBridgeLocator.SelectFirst();
```
```c#
var bridge = await HueBridgeLocator.SelectFirstAsync().ConfigureAwait(false);
```

Otherwise you can select the bridge using its private IP address. In this case no HTTP calls need to be made so there is no asynchronous option. If you know the private IP address is not going to change or would prefer to setup your local network so it doesn't change you could also directly select the bridge using its private IP address so additional HTTP requests do not need to be made to discover the bridge.

```c#
var bridge = HueBridgeLocator.SelectWithLocalIp("192.168.1.2");
```

## Authentication

In order to communicate with the Hue bridge you will have to first create a user. This can be done following the instructions [here](https://developers.meethue.com/develop/get-started-2/). Eventually we may add support for creating users.

Once you have the username you can specify which user to use when communicating with the Hue birdge using the WithUser method. In this case no HTTP calls need to be made so there is no asynchronous option.

```c#
var bridge = HueBridgeLocator.SelectFirst()
    .WithUser("1028d66426293e821ecfd9ef1a0731df");
```

## Interacting with Lights

To select which light you wish to interact with you can either retrieve all lights that the bridge knows about or select one with a specific name.

### Retrieving All Lights

```c#
var bridge = await HueBridgeLocator.SelectFirstAsync().ConfigureAwait(false);
var lights = await bridge
    .WithUser("1028d66426293e821ecfd9ef1a0731df")
    .GetAllLightsAsync()
    .ConfigureAwait(false);
```

```c#
var lights = HueBridgeLocator.SelectFirst()
    .WithUser("1028d66426293e821ecfd9ef1a0731df")
    .GetAllLights();
```

### Retrieving a Specific Light

```c#
var bridge = await HueBridgeLocator.SelectFirstAsync().ConfigureAwait(false);
var light = await bridge
    .WithUser("1028d66426293e821ecfd9ef1a0731df")
    .SelectLightAsync("Bedroom")
    .ConfigureAwait(false);
```

```c#
var light = HueBridgeLocator.SelectFirst()
    .WithUser("1028d66426293e821ecfd9ef1a0731df")
    .SelectLight("Bedroom");
```

### Retrieve Current State of Light

```c#
var bridge = await HueBridgeLocator.SelectFirstAsync().ConfigureAwait(false);
var light = await bridge
    .WithUser("1028d66426293e821ecfd9ef1a0731df")
    .SelectLightAsync("Bedroom")
    .ConfigureAwait(false);
var state = await light.GetCurrentStateAsync().ConfigureAwait(false);
```

```c#
var state = HueBridgeLocator.SelectFirst()
    .WithUser("1028d66426293e821ecfd9ef1a0731df")
    .SelectLight("Bedroom")
    .GetCurrentState();
```

### Toggle Light On/Off

```c#
var bridge = await HueBridgeLocator.SelectFirstAsync().ConfigureAwait(false);
var light = await bridge
    .WithUser("1028d66426293e821ecfd9ef1a0731df")
    .SelectLightAsync("Bedroom")
    .ConfigureAwait(false);
var state = await light.GetCurrentStateAsync().ConfigureAwait(false);
state.ToggleAsync().ConfigureAwait(false);
```

```c#
var state = HueBridgeLocator.SelectFirst()
    .WithUser("1028d66426293e821ecfd9ef1a0731df")
    .SelectLight("Bedroom")
    .GetCurrentState()
    .Toggle();
```

### Specifically Set Light On/Off

```c#
var bridge = await HueBridgeLocator.SelectFirstAsync().ConfigureAwait(false);
var light = await bridge
    .WithUser("1028d66426293e821ecfd9ef1a0731df")
    .SelectLightAsync("Bedroom")
    .ConfigureAwait(false);
var state = await light.GetCurrentStateAsync().ConfigureAwait(false);
state.SetStateAsync(isOn: true).ConfigureAwait(false);
```

```c#
var state = HueBridgeLocator.SelectFirst()
    .WithUser("1028d66426293e821ecfd9ef1a0731df")
    .SelectLight("Bedroom")
    .GetCurrentState()
    .SetState(isOn: false);
```

### Setting the Brightness of a Light

Brightness is set on a scale of 1 - 254 with 254 being the brightest state of the light possible.

```c#
var bridge = await HueBridgeLocator.SelectFirstAsync().ConfigureAwait(false);
var light = await bridge
    .WithUser("1028d66426293e821ecfd9ef1a0731df")
    .SelectLightAsync("Bedroom")
    .ConfigureAwait(false);
var state = await light.GetCurrentStateAsync().ConfigureAwait(false);
state.SetBrightnessAsync(150).ConfigureAwait(false);
```

```c#
var state = HueBridgeLocator.SelectFirst()
    .WithUser("1028d66426293e821ecfd9ef1a0731df")
    .SelectLight("Bedroom")
    .GetCurrentState()
    .SetBrightness(150);
```

### Setting the Color of a Light

Color is specified using the [CIE 1931 color space](https://en.wikipedia.org/wiki/CIE_1931_color_space) where X and Y are floating point values between 0 and 1.

```c#
var bridge = await HueBridgeLocator.SelectFirstAsync().ConfigureAwait(false);
var light = await bridge
    .WithUser("1028d66426293e821ecfd9ef1a0731df")
    .SelectLightAsync("Bedroom")
    .ConfigureAwait(false);
var state = await light.GetCurrentStateAsync().ConfigureAwait(false);
state.SetColorAsync(x: 0.25, y: 0.5).ConfigureAwait(false);
```

```c#
var state = HueBridgeLocator.SelectFirst()
    .WithUser("1028d66426293e821ecfd9ef1a0731df")
    .SelectLight("Bedroom")
    .GetCurrentState()
    .SetColor(x: 0.25, y: 0.5);
```