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
	var bridge = await HueBridgeLocator.SelectFirstAsync().ConfigureAwait(false);
	var lights = await bridge
		.WithUser(hueUser)
		.GetAllLightsAsync().ConfigureAwait(false);
	lights.Dump();
}
