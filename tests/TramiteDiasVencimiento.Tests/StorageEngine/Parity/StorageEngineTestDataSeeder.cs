using MiApp.DTOs.DTOs.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;

namespace TramiteDiasVencimiento.Tests.StorageEngine.Parity;

public sealed class StorageEngineTestDataSeeder
{
    public StorageContext BuildSimpleContext(
        string requestId,
        string gabinete = "gab",
        bool includeWorkflow = true)
    {
        return new StorageContext
        {
            DefaultDbAlias = "db",
            Usuario = "qa.user",
            UsuarioId = 99,
            RequestId = requestId,
            NombreGabinete = gabinete,
            RutaTemporalId = "tmp-batch-01",
            NombreDocumento = "DIG00000011.pdf",
            ArchivosTemporales = ["tmp-001.pdf"],
            IpTrans = "10.10.10.10",
            Command = new AlmacenarDocumentoCommand
            {
                NombreGabinete = gabinete,
                RutaTemporalId = "tmp-batch-01",
                NombreDocumento = "DIG00000011.pdf",
                RequestId = requestId,
                NumeroPaginasDeclaradas = 3,
                CamposIndexacion =
                [
                    new CampoIndexacionDto { NombreCampo = "CAMPO_A", Valor = "VALOR_A" },
                    new CampoIndexacionDto { NombreCampo = "CAMPO_B", Valor = "VALOR_B" }
                ],
                Trd = new TrdStorageDto
                {
                    IdArea = 10,
                    IdSerie = 20,
                    IdSubSerie = 30,
                    IdTipoDocumento = 40,
                    NombreTipoDocumento = "TIPO-40"
                },
                Expediente = new ExpedienteStorageDto
                {
                    IdExpediente = 55,
                    IdClaseDocumento = 7,
                    ClaseDocumento = "ACTA"
                },
                Workflow = includeWorkflow
                    ? new WorkflowStorageDto
                    {
                        IdTareaWorkflow = 3001,
                        IdRutaWorkflow = 11
                    }
                    : null,
                Inventario = new InventarioDocumentalDto
                {
                    IdUsuarioGestion = 99,
                    IdEmpresa = 1,
                    Radicado = "RAD-001",
                    SegundoNombreDocumento = "DOC-ALTERNO.pdf"
                },
                Documentos =
                [
                    new DocumentoEntradaDto
                    {
                        IdDocumento = "1",
                        ArchivoTemporalId = "tmp-001.pdf",
                        NumeroPaginas = 3,
                        Extension = ".pdf"
                    }
                ]
            }
        };
    }
}
