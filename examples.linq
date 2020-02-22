<Query Kind="Program">
  <Reference Relative="FluentHue\bin\Debug\netstandard2.0\FluentHue.dll">R:\git\FluentHue\FluentHue\bin\Debug\netstandard2.0\FluentHue.dll</Reference>
  <Namespace>FluentHue</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

private const string hueUser = "aIKNGKPClUjBOD0hXlwlnPQXDu7fKNhKtavcUtCc";

async Task Main()
{
	// Get all lights
	HueBridgeLocator.SelectFirst()
		.WithUser(hueUser)
		.GetAllLights()
		.Dump();
		
	// Get all lights - async
	var bridge = await HueBridgeLocator.SelectFirstAsync()
		.ConfigureAwait(false);
	var lights = await bridge
		.WithUser(hueUser)
		.GetAllLightsAsync()
		.ConfigureAwait(false);
	lights.Dump();

	// Select light
	HueBridgeLocator.SelectFirst()
		.WithUser(hueUser)
		.SelectLight("Color-Bedroom")
		.Dump();

	// Select light - async
	var light = await bridge
		.WithUser(hueUser)
		.SelectLightAsync("Color-Bedroom")
		.ConfigureAwait(false);
	light.Dump();

	// Get light state
	HueBridgeLocator.SelectFirst()
		.WithUser(hueUser)
		.SelectLight("Bedroom-Closet")
		.GetCurrentState()
		.Dump();

	// Get light state - async
	light = await bridge
		.WithUser(hueUser)
		.SelectLightAsync("Color-Bedroom")
		.ConfigureAwait(false);
	var state = await light.GetCurrentStateAsync()
		.ConfigureAwait(false);
	state.Dump();
	
	// Toggle light state
	HueBridgeLocator.SelectFirst()
		.WithUser(hueUser)
		.SelectLight("Color-Bedroom")
		.GetCurrentState()
			.Toggle()
			.SetBrightness(150)
			.SetColor(1f, 1f)
		.End()
		.GetCurrentState()
		.Dump();

	HueBridgeLocator.SelectFirst()
		.WithUser(hueUser)
		.SelectLight("Bedroom-Closet")
		.GetCurrentState()
			.Toggle()
			.SetBrightness(150)
			.SetColor(.5f, .5f)
		.End()
		.GetCurrentState()
		.Dump();
}