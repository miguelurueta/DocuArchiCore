using Microsoft.AspNetCore.Mvc;
using DocuArchiCore.Abstractions.Security;
using System.Linq;
using System.Text;

namespace MiApp.Web.Controllers
{
    [Route("debug")]
    public class DebugController : Controller
    {
        private readonly ISesionActual _sesion;

        public DebugController(ISesionActual sesion)
        {
            _sesion = sesion;
        }

        // ===========================================================
        //  🔍 PANEL HTML COMPLETO DE SESIÓN
        // ===========================================================
        [HttpGet("sesion/html")]
        public IActionResult SesionHtml()
        {
            var props = _sesion.GetType()
                .GetProperties()
                .Select(p => new
                {
                    Name = p.Name,
                    Value = p.GetValue(_sesion),
                    Group = GetGroupName(p.Name),
                    Css = GetCssClass(p.GetValue(_sesion)),
                    Icon = GetIcon(p.GetValue(_sesion))
                })
                .OrderBy(p => p.Group)
                .ThenBy(p => p.Name)
                .ToList();

            var html = new StringBuilder();

            html.Append("<html><head><meta charset='UTF-8'><title>Debug Sesión Actual</title>");

            html.Append(@"
<style>
    body { font-family: Arial; padding: 20px; background: #f5f5f5; }
    table { border-collapse: collapse; width: 100%; margin-bottom: 30px; }
    th, td { padding: 10px; border: 1px solid #ccc; font-size: 14px; }
    th { background: #222; color: white; }
    tr:hover { background: #f1f1f1; }

    .ok { background: #c8f7c5 !important; }
    .off { background: #f7c5c5 !important; }
    .warn { background: #fff3a3 !important; }

    h1 { margin-bottom: 10px; }
    h2 { background: #333; color: #fff; padding: 10px; margin-top:40px; }

    #search { width: 100%; padding: 12px; font-size: 16px; margin-bottom: 20px;
               border-radius:6px; border:1px solid #999; }

    .excel-btn { padding:10px 20px; background:#007bff; 
                 color:white; border:none; border-radius:6px; 
                 margin-bottom:20px; cursor:pointer; font-size:15px; }
</style>

<script>
function searchPermissions() {
    var input = document.getElementById('search').value.toLowerCase();
    var rows = document.getElementsByClassName('perm-row');
    for (var i = 0; i < rows.length; i++) {
        var name = rows[i].getAttribute('data-name').toLowerCase();
        rows[i].style.display = name.includes(input) ? '' : 'none';
    }
}

function exportToExcel() {
    const file = new Blob(
        ['\ufeff' + document.documentElement.innerHTML],
        { type: 'application/vnd.ms-excel;charset=utf-8;' }
    );

    const url = URL.createObjectURL(file);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'SesionActual_Debug.xls';
    a.click();
    URL.revokeObjectURL(url);
}
</script>
");

            html.Append("</head><body>");

            html.Append("<h1>🔍 Panel de Sesión – Vista Completa</h1>");

            html.Append("<button class='excel-btn' onclick='exportToExcel()'>📥 Exportar a Excel</button>");
            html.Append("<input type='text' id='search' onkeyup='searchPermissions()' placeholder='Buscar variable o permiso...'>");

            var groups = props.GroupBy(p => p.Group);
            foreach (var group in groups)
            {
                html.Append($"<h2>{group.Key}</h2>");
                html.Append("<table>");
                html.Append("<tr><th>Variable</th><th>Valor</th><th>Estado</th></tr>");

                foreach (var p in group)
                {
                    html.Append($@"
<tr class='perm-row {p.Css}' data-name='{p.Name}'>
<td>{p.Name}</td>
<td>{p.Value}</td>
<td style='font-size:18px;text-align:center'>{p.Icon}</td>
</tr>");
                }

                html.Append("</table>");
            }

            html.Append("</body></html>");

            return Content(html.ToString(), "text/html");
        }

        // ===========================================================
        //  🎨 LÓGICA DE COLORES
        // ===========================================================
        private string GetCssClass(object value)
        {
            if (value == null) return "off";

            return value switch
            {
                int i when i == 1 => "ok",
                int i when i > 1 => "warn",
                int _ => "off",

                string s when !string.IsNullOrWhiteSpace(s) => "ok",
                string _ => "off",

                _ => "ok"
            };
        }

        // ===========================================================
        //  🎯 ÍCONOS VISUALES
        // ===========================================================
        private string GetIcon(object value)
        {
            if (value == null) return "&#10060;"; // ❌

            return value switch
            {
                int i when i == 1 => "&#10004;",   // ✔️
                int i when i > 1 => "&#9888;",    // ⚠️
                int _ => "&#10060;",              // ❌

                string s when !string.IsNullOrWhiteSpace(s) => "&#10004;",
                string _ => "&#10060;",

                _ => "&#10004;"
            };
        }

        // ===========================================================
        //  🔥 AGRUPACIÓN POR PREFIJOS Y CONTENIDOS
        // ===========================================================
        private string GetGroupName(string prop)
        {
            prop = prop.ToUpper();

            if (prop.StartsWith("GA_")) return "Gestión Documental";
            if (prop.StartsWith("DA_")) return "DocuArchi";
            if (prop.StartsWith("RA_")) return "Radicación";
            if (prop.StartsWith("WF_")) return "Workflow";

            if (prop.Contains("DOCUMENTO")) return "Documentos";
            if (prop.Contains("EXPEDIENTE")) return "Expedientes";
            if (prop.Contains("CONSERVACION")) return "Unidades de Conservación";
            if (prop.Contains("VISOR") || prop.Contains("EXPRESS")) return "Visor Express";
            if (prop.Contains("PRODUC")) return "Producción Documental";
            if (prop.Contains("MIGRA")) return "Migración de Documentos";
            if (prop.Contains("ADMINISTRACION") || prop.Contains("GESTION")) return "Administración";
            if (prop.Contains("FIRMA")) return "Firmas Digitales";

            return "General";
        }
    }
}
