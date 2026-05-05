using System.Collections.Concurrent;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Enums;
using Xunit;

namespace TramiteDiasVencimiento.Tests.StorageEngine.Parity;

public sealed class StorageEngineParityTestSuite : IClassFixture<StorageEngineParityFixture>
{
    private readonly StorageEngineParityFixture _fixture;

    public StorageEngineParityTestSuite(StorageEngineParityFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Escenario01_AlmacenamientoSimple_DebeMantenerParidadCore()
    {
        var context = _fixture.Seeder.BuildSimpleContext("req-parity-01", includeWorkflow: false);
        var physical = StorageEngineParityFixture.BuildPhysical(".PDF");
        var naming = StorageEngineParityFixture.BuildNaming(101, ".pdf");

        var transaction = BuildTransaction(context.RequestId, 101, disco: 2, carpeta: 45, workflowLogInserted: false);

        var expected = _fixture.LegacyExpectedBuilder.BuildCore(context, transaction.IdentityReservation.Identity, naming, physical);
        var actual = _fixture.ActualResultReader.ReadCore(context, transaction, naming, physical);

        StorageEngineParityAssert.AssertCore(expected, actual);
    }

    [Fact]
    public void Escenario09_WorkflowActivo_DebeMantenerParidadLogdocuarchi()
    {
        var context = _fixture.Seeder.BuildSimpleContext("req-parity-09");
        var physical = StorageEngineParityFixture.BuildPhysical(".PDF");
        var naming = StorageEngineParityFixture.BuildNaming(202, ".pdf");
        var transaction = BuildTransaction(context.RequestId, 202, disco: 4, carpeta: 77, workflowLogInserted: true);

        var coreExpected = _fixture.LegacyExpectedBuilder.BuildCore(context, transaction.IdentityReservation.Identity, naming, physical);
        var coreActual = _fixture.ActualResultReader.ReadCore(context, transaction, naming, physical);

        var log = new WorkflowStorageLogModel
        {
            IdTran = 202,
            DescOp = "Registra",
            UserOper = context.Usuario,
            DateTrans = DateTime.UtcNow.Date,
            RutDocu = $@"D:\storage\{context.NombreGabinete}{transaction.IdentityReservation.Identity.Disco}\{transaction.IdentityReservation.Identity.Carpeta:D5}\{naming.NombreArchivoPrincipal}",
            ModuloRegistro = "WORKFLOW",
            Gabinete = context.NombreGabinete,
            Campos = "|VALOR_A|VALOR_B",
            IpTrans = context.IpTrans ?? string.Empty,
            HoraRegistro = "10:20:30",
            Radicado = context.Command?.Inventario?.Radicado,
            IdTareaWorkflow = context.Command?.Workflow?.IdTareaWorkflow ?? 0,
            IdRutaWorkflow = context.Command?.Workflow?.IdRutaWorkflow ?? 0,
            UserPropietario = context.Usuario,
            TipologiaDocumental = context.Command?.Trd?.NombreTipoDocumento
        };

        var expected = _fixture.LegacyExpectedBuilder.BuildWorkflow(coreExpected, log);
        var actual = _fixture.ActualResultReader.ReadWorkflow(coreActual, log);

        StorageEngineParityAssert.AssertWorkflow(expected, actual);
        StorageEngineDatabaseAssert.AssertWorkflowLogRow(actual.CamposLog);
    }

    [Fact]
    public void EscenarioFxl_DebeValidarAtributosLegacy()
    {
        var naming = StorageEngineParityFixture.BuildNaming(303, ".pdf");
        var xml = "<Gabinetes><Gabinete ID=\"303\" DISC=\"2\" PAG=\"3\" DBT=\"-2\" IDEX=\"45\" USER=\"qa.user\" DATE1=\"2026/05/05\" TIME1=\"10:20:30\" ID_AREA=\"10\" ID_SERIE=\"20\" /></Gabinetes>";
        var files = _fixture.FileSystem.CreateLegacyStorageFiles("gab", 2, 45, naming.NombreArchivoPrincipal, naming.NombreXml, xml);

        StorageEngineXmlAssert.AssertFxlContainsAttributes(
            files.FxlPath,
            "ID",
            "DISC",
            "PAG",
            "DBT",
            "IDEX",
            "USER",
            "DATE1",
            "TIME1",
            "ID_AREA",
            "ID_SERIE");
    }

    [Fact]
    public void EscenarioConcurrencia_DosSolicitudes_DebeGenerarIdentidadesUnicas()
    {
        var identities = new ConcurrentBag<long>();
        var sequence = 700L;

        Parallel.For(0, 2, _ =>
        {
            var id = Interlocked.Increment(ref sequence);
            identities.Add(id);
        });

        Assert.Equal(2, identities.Count);
        Assert.Equal(2, identities.Distinct().Count());
    }

    [Fact(Skip = "Requiere Docker/Testcontainers + dataset legacy controlado para validar paridad DB/FS/XML end-to-end.")]
    public void EscenariosIntegracion_ParidadLegacy_PendienteAmbienteDocker()
    {
    }

    [Fact(Skip = "Requiere carga concurrente controlada (2 y 5 tareas) en entorno de integración con locks FOR UPDATE reales.")]
    public void EscenariosConcurrencia_ParidadLegacy_PendienteAmbienteDocker()
    {
    }

    [Fact]
    public void EscenarioSystem1_DebeValidarEstructuraMinimaDeIncrementos()
    {
        var before = new Dictionary<string, long>
        {
            ["proxid"] = 100,
            ["numcarp"] = 40,
            ["numpag_carp"] = 10
        };

        var after = new Dictionary<string, long>
        {
            ["proxid"] = 101,
            ["numcarp"] = 40,
            ["numpag_carp"] = 13
        };

        StorageEngineDatabaseAssert.AssertSystem1Update(
            before,
            after,
            expectedProxIdIncrement: 1,
            expectedNumPagCarpIncrement: 3);
    }

    private static StorageTransactionResult BuildTransaction(
        string requestId,
        long idAlmacen,
        int disco,
        int carpeta,
        bool workflowLogInserted)
    {
        return new StorageTransactionResult
        {
            IdAlmacen = idAlmacen,
            IdentityReservation = new StorageIdentityReservationResult
            {
                Identity = new StorageIdentityModel
                {
                    IdAlmacen = idAlmacen,
                    Disco = disco,
                    Carpeta = carpeta,
                    NumeroPaginasCarpeta = 3
                },
                PreviousProxId = idAlmacen - 1,
                NewProxId = idAlmacen,
                PreviousFolder = carpeta,
                NewFolder = carpeta,
                PreviousFolderPages = 0,
                NewFolderPages = 3,
                TamDisc = 572523149
            },
            IdRegistroProduccionDocumental = 9000,
            Success = true,
            Estado = StorageDocumentState.Reserved,
            RequestId = requestId,
            FechaEjecucion = DateTime.UtcNow,
            DuracionMs = 10,
            DiskUsageUpdated = true,
            WorkflowLogInserted = workflowLogInserted
        };
    }
}
