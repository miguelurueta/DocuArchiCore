using System.Net.Http.Headers;
using System.Net.Http.Json;
using MiApp.DTOs.DTOs.UI.MuiTable;
using MiApp.DTOs.DTOs.Utilidades;
using MiApp.DTOs.DTOs.Workflow.BandejaCorrespondencia;
using Xunit;
using Xunit.Abstractions;

namespace TramiteDiasVencimiento.Tests;

public sealed class WorkflowInboxApiLiveIntegrationTests
{
    private readonly ITestOutputHelper _output;

    public WorkflowInboxApiLiveIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task SolicitaBandejaWorkflow_LiveApi_SmokeTest_UsuarioGestion144()
    {
        var runLive = Environment.GetEnvironmentVariable("WORKFLOW_INBOX_RUN_LIVE");
        var baseUrl = Environment.GetEnvironmentVariable("WORKFLOW_INBOX_BASE_URL");
        var bearerToken = Environment.GetEnvironmentVariable("WORKFLOW_INBOX_BEARER_TOKEN");

        if (!string.Equals(runLive, "1", StringComparison.Ordinal))
        {
            _output.WriteLine("Smoke test omitido. Define WORKFLOW_INBOX_RUN_LIVE=1 para ejecutarlo.");
            return;
        }

        Assert.False(string.IsNullOrWhiteSpace(baseUrl), "Define WORKFLOW_INBOX_BASE_URL para ejecutar el smoke test.");
        Assert.False(string.IsNullOrWhiteSpace(bearerToken), "Define WORKFLOW_INBOX_BEARER_TOKEN con un token cuyo claim usuarioid sea 144.");

        using var client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl!.TrimEnd('/') + "/")
        };

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        var request = new WorkflowInboxApiRequestDto
        {
            ColumnMode = WorkflowColumnListMode.ListaGestionTramite,
            EstadoTramite = string.Empty,
            SearchType = 1,
            Search = string.Empty,
            SortField = string.Empty,
            SortDir = string.Empty,
            Page = 0,
            PageSize = 0,
            StructuredFilters =
            [
                new WorkflowStructuredFilterDto
                {
                    Field = "string",
                    Operator = "string",
                    Value = "string",
                    ValueFrom = "string",
                    ValueTo = "string"
                }
            ]
        };

        _output.WriteLine($"POST {client.BaseAddress}api/workflowInboxgestion/inboxgestion");
        _output.WriteLine("El token debe resolver usuarioid=144 en el API real.");

        using var response = await client.PostAsJsonAsync("api/workflowInboxgestion/inboxgestion", request);
        var rawBody = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"HTTP {(int)response.StatusCode} {response.ReasonPhrase}");
        _output.WriteLine(rawBody);

        Assert.True(
            response.IsSuccessStatusCode,
            $"La API respondio {(int)response.StatusCode}. Revisa el body capturado en el output del test.");

        var payload = await response.Content.ReadFromJsonAsync<AppResponses<DynamicUiTableDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.success, $"La API retorno success=false. Mensaje: {payload.message}");
        Assert.NotNull(payload.data);
        Assert.Equal("workflowInboxgestion", payload.data.TableId);
        Assert.NotNull(payload.data.Rows);

        _output.WriteLine($"Rows: {payload.data.Rows.Count}");
        if (payload.data.Rows.Count > 0)
        {
            _output.WriteLine($"First row keys: {string.Join(", ", payload.data.Rows[0].Values.Keys)}");
        }
    }
}
