using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DocuArchiCore.Controllers
{
    public abstract class BaseController : Controller
    {
        protected static JsonSerializerOptions PascalCaseJsonOptions =>
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                DictionaryKeyPolicy = null
            };
    }
}
