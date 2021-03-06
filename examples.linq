<Query Kind="Program">
  <Reference Relative="FluentHue\bin\Debug\netstandard2.0\FluentHue.dll">R:\git\FluentHue\FluentHue\bin\Debug\netstandard2.0\FluentHue.dll</Reference>
  <Namespace>FluentHue</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

private const string hueUser = "aIKNGKPClUjBOD0hXlwlnPQXDu7fKNhKtavcUtCc";

async Task Main()
{
	// Initiate a search for new lights
	//HueBridgeLocator.SelectFirst()
	//	.WithUser(hueUser)
	//	.DiscoverNewLights("598436");
	
	// Get all lights
	HueBridgeLocator.SelectFirst()
		.WithUser(hueUser)
		.GetAllLights()
		.Dump();
	
	// Rename light
	//HueBridgeLocator.SelectFirst()
	//	.WithUser(hueUser)
	//	.SelectLight("Hue color lamp 1")
	//	.Rename("Bedroom-Closet-Color")
	//	.Dump();
		
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
		.SelectLight("Bedroom-Color")
		.Dump();

	// Select light - async
	var light = await bridge
		.WithUser(hueUser)
		.SelectLightAsync("Bedroom-Color")
		.ConfigureAwait(false);
	light.Dump();

	// Get light state
	HueBridgeLocator.SelectFirst()
		.WithUser(hueUser)
		.SelectLight("Bedroom-Closet-Color")
		.GetCurrentState()
		.Dump();

	// Get light state - async
	light = await bridge
		.WithUser(hueUser)
		.SelectLightAsync("Bedroom-Color")
		.ConfigureAwait(false);
	var state = await light.GetCurrentStateAsync()
		.ConfigureAwait(false);
	state.Dump();
	
	// Toggle light state
	HueBridgeLocator.SelectFirst()
		.WithUser(hueUser)
		.SelectLight("Bedroom-Color")
		.GetCurrentState()
			.Toggle()
			.SetBrightness(180)
			.SetColor(.3f, .25f)
		.End()
		.GetCurrentState()
		.Dump();

	HueBridgeLocator.SelectFirst()
		.WithUser(hueUser)
		.SelectLight("Bedroom-Closet-Color")
		.GetCurrentState()
			.Toggle()
			.SetBrightness(180)
			.SetColor(.3f, .25f)
		.End()
		.GetCurrentState()
		.Dump();
}