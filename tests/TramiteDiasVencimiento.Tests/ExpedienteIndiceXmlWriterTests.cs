using Microsoft.Extensions.Logging;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.ExpedienteXml;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.ExpedienteXml;
using Moq;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public sealed class ExpedienteIndiceXmlWriterTests
    {
        [Fact]
        public async Task UpdateAsync_ShouldAppendDocumentoIndizado_WhenXmlIsValid()
        {
            var writer = new ExpedienteIndiceXmlWriter(Mock.Of<ILogger<ExpedienteIndiceXmlWriter>>());
            var tempDir = Path.Combine(Path.GetTempPath(), "docuarchi-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            try
            {
                var xmlPath = Path.Combine(tempDir, "000000001.xml");
                await File.WriteAllTextAsync(xmlPath, "<tipoIndiceContenido><tipodocumentoFoliado></tipodocumentoFoliado></tipoIndiceContenido>");

                var model = new ExpedienteIndiceXmlDocumentModel
                {
                    IdRegistroProduccionDocumental = 700,
                    IdAlmacen = 100,
                    IdExpediente = 11,
                    NombreDocumento = "DIG00000011.pdf",
                    SegundoNombreDocumento = "SEGUNDO.pdf",
                    TipologiaDocumental = "Contrato",
                    FechaDeclaracionDocumento = DateTime.UtcNow,
                    FechaIncorporacionDocumento = DateTime.UtcNow,
                    ValorHuella = new string('A', 64),
                    FuncionResumen = "SHA256",
                    OrdenDocumentoExpediente = 1,
                    PaginaInicial = 1,
                    PaginaFinal = 2,
                    Formato = ".PDF",
                    Tamano = "1 Kb",
                    RutaDocumento = @"C:\storage\DIG00000011.pdf",
                    NumeroFolios = 2,
                    Origen = 9
                };

                var result = await writer.UpdateAsync(xmlPath, model);

                Assert.True(result.Updated);
                Assert.Equal("UPDATED", result.Estado);
                var xmlText = await File.ReadAllTextAsync(xmlPath);
                Assert.Contains("DocumentoIndizado", xmlText);
                Assert.Contains("Nombre_Documento", xmlText);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, recursive: true);
                }
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenXmlDoesNotExist()
        {
            var writer = new ExpedienteIndiceXmlWriter(Mock.Of<ILogger<ExpedienteIndiceXmlWriter>>());

            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                writer.UpdateAsync(@"C:\no-existe\indice.xml", new ExpedienteIndiceXmlDocumentModel()));
        }
    }
}

