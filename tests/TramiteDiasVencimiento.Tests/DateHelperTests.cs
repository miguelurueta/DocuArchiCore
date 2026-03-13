using MiApp.Services.Service.DateHelper;
using Xunit;

namespace TramiteDiasVencimiento.Tests;

public class DateHelperTests
{
    [Fact]
    public async Task FormateaFechaTimeFrameworkAsync_RetornaYesYFormatoEsperado()
    {
        var service = new DateHelper();

        var result = await service.FormateaFechaTimeFrameworkAsync(new DateTime(2000, 1, 1));

        Assert.Equal("YES", result.Item2);
        Assert.False(string.IsNullOrWhiteSpace(result.Item1));
        Assert.True(
            DateTime.TryParseExact(
                result.Item1,
                "yyyy-MM-dd HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out _));
    }

    [Fact]
    public async Task FormateaFechaTimeFrameworkAsync_IgnoraFechaEntradaYUsaFechaActual()
    {
        var service = new DateHelper();
        var before = DateTime.Now.AddSeconds(-2);

        var result = await service.FormateaFechaTimeFrameworkAsync(new DateTime(1999, 12, 31, 23, 59, 59));

        Assert.Equal("YES", result.Item2);
        var parsed = DateTime.ParseExact(
            result.Item1,
            "yyyy-MM-dd HH:mm:ss",
            System.Globalization.CultureInfo.InvariantCulture);

        Assert.True(parsed >= before);
        Assert.True(parsed <= DateTime.Now.AddSeconds(2));
    }
}
