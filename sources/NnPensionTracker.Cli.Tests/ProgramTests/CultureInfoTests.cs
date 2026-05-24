using System.Globalization;
using FluentAssertions;

namespace NN.Toolkit.Cli.Tests.ProgramTests;

public class CultureInfoTests
{
	[Fact]
	public void ReadCultureInfoFromAppSettings_WhenCultureInfoIsDefined_ReturnsTheConfiguredCulture()
	{
		string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(directory);

		try
		{
			string filePath = Path.Combine(directory, "appsettings.json");
			File.WriteAllText(filePath, "{ \"CultureInfo\": \"fr-FR\" }");

			bool isCultureLoaded = DustInTheWind.NnPensionTracker.Cli.Program.TryReadCultureInfoFromAppSettings(filePath, out CultureInfo cultureInfo);

			isCultureLoaded.Should().BeTrue();
			cultureInfo.Name.Should().Be("fr-FR");
		}
		finally
		{
			Directory.Delete(directory, recursive: true);
		}
	}

	[Fact]
	public void TryReadCultureInfoFromAppSettings_WhenFileIsMissing_ReturnsFalseAndKeepsTheCurrentCulture()
	{
		string filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "appsettings.json");

		bool isCultureLoaded = DustInTheWind.NnPensionTracker.Cli.Program.TryReadCultureInfoFromAppSettings(filePath, out CultureInfo cultureInfo);

		isCultureLoaded.Should().BeFalse();
		cultureInfo.Should().Be(CultureInfo.CurrentCulture);
	}
}



