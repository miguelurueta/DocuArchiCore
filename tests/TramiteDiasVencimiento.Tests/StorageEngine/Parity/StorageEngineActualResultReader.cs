using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Metadata;
using System.Collections.Generic;

namespace TramiteDiasVencimiento.Tests.StorageEngine.Parity;

public sealed class StorageEngineActualResultReader
{
    public StorageEngineParitySnapshot ReadCore(
        StorageContext context,
        StorageTransactionResult transaction,
        StorageNamingResult naming,
        StorageDocumentPhysicalMetadata physical)
    {
        return new StorageEngineParitySnapshot
        {
            RequestId = transaction.RequestId,
            NombreGabinete = context.NombreGabinete,
            IdAlmacen = transaction.IdentityReservation.Identity.IdAlmacen,
            Disco = transaction.IdentityReservation.Identity.Disco,
            Carpeta = transaction.IdentityReservation.Identity.Carpeta,
            NumeroPaginas = physical.NumeroPaginas,
            Formato = physical.Formato,
            TamanoLegacy = physical.TamanoLegacy,
            NombreDig = naming.NombreArchivoPrincipal,
            NombreFxl = naming.NombreXml,
            WorkflowActivo = transaction.WorkflowLogInserted,
            IdRegistroProduccionDocumental = transaction.IdRegistroProduccionDocumental
        };
    }

    public StorageEngineParitySnapshot ReadWorkflow(
        StorageEngineParitySnapshot core,
        WorkflowStorageLogModel log)
    {
        return new StorageEngineParitySnapshot
        {
            RequestId = core.RequestId,
            NombreGabinete = core.NombreGabinete,
            IdAlmacen = core.IdAlmacen,
            Disco = core.Disco,
            Carpeta = core.Carpeta,
            NumeroPaginas = core.NumeroPaginas,
            Formato = core.Formato,
            TamanoLegacy = core.TamanoLegacy,
            NombreDig = core.NombreDig,
            NombreFxl = core.NombreFxl,
            WorkflowActivo = true,
            IdRegistroProduccionDocumental = core.IdRegistroProduccionDocumental,
            CamposLog = new Dictionary<string, string?>
            {
                ["id_tran"] = log.IdTran.ToString(),
                ["desc_op"] = log.DescOp,
                ["USER_OPER"] = log.UserOper,
                ["RUT_DOCU"] = log.RutDocu,
                ["MODULO_REGISTRO"] = log.ModuloRegistro,
                ["GABINETE"] = log.Gabinete,
                ["IP_TRANS"] = log.IpTrans,
                ["ID_TAREA_WF"] = log.IdTareaWorkflow.ToString(),
                ["ID_RUTA_WF"] = log.IdRutaWorkflow.ToString(),
                ["TIPOLOGIA_DOCUMENTAL"] = log.TipologiaDocumental
            }
        };
    }
}
