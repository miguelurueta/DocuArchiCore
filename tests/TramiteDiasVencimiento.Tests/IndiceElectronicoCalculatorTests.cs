using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental;
using MiApp.Models.Models.GestorDocumental.AlmacenamientoDocumental.Exceptions;
using MiApp.Services.Service.GestorDocumental.AlmacenamientoDocumental.Expediente;
using Xunit;

namespace TramiteDiasVencimiento.Tests
{
    public class IndiceElectronicoCalculatorTests
    {
        [Fact]
        public void Calculate_ShouldReturnExpectedValues()
        {
            var calculator = new IndiceElectronicoCalculator();
            var expediente = new ExpedienteInfoModel
            {
                IdExpediente = 10,
                IdUnidadConservacion = 20,
                EstadoExpediente = 1,
                EstadoExpedienteElectronico = 1,
                NumeroFoliosContenidos = 100,
                OrdenIndice = 7,
                UltimaPaginaIndice = 23,
                CodigoUnico = "EXP-1"
            };

            var result = calculator.Calculate(expediente, 5);

            Assert.Equal(8, result.NuevoOrden);
            Assert.Equal(24, result.PaginaInicial);
            Assert.Equal(28, result.PaginaFinal);
            Assert.Equal(5, result.NumeroFolios);
        }

        [Fact]
        public void Calculate_ShouldThrow_WhenNumeroFoliosIsInvalid()
        {
            var calculator = new IndiceElectronicoCalculator();
            var expediente = new ExpedienteInfoModel
            {
                IdExpediente = 10,
                IdUnidadConservacion = 20,
                EstadoExpediente = 1,
                EstadoExpedienteElectronico = 1,
                NumeroFoliosContenidos = 100,
                OrdenIndice = 7,
                UltimaPaginaIndice = 23,
                CodigoUnico = "EXP-1"
            };

            Assert.Throws<StorageTransactionException>(() => calculator.Calculate(expediente, 0));
        }
    }
}
