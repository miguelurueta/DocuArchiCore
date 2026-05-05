using System;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace TramiteDiasVencimiento.Tests.StorageEngine.Parity;

internal static class StorageEngineXmlAssert
{
    public static void AssertFxlContainsAttributes(string xmlPath, params string[] attributes)
    {
        var doc = XDocument.Load(xmlPath);
        var gabinete = doc.Descendants("Gabinete").FirstOrDefault();
        Assert.NotNull(gabinete);

        foreach (var attribute in attributes)
        {
            var found = gabinete!.Attributes().Any(a => string.Equals(a.Name.LocalName, attribute, StringComparison.OrdinalIgnoreCase));
            Assert.True(found, $"Atributo FXL faltante: {attribute}");
        }
    }

    public static void AssertIndiceContainsDocumentoIndizado(string xmlPath)
    {
        var doc = XDocument.Load(xmlPath);
        var documentos = doc.Descendants().Where(x => x.Name.LocalName == "DocumentoIndizado").ToList();
        Assert.NotEmpty(documentos);
    }
}
