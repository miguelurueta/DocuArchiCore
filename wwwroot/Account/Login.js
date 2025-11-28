let App;
let Spinner = new SpinnerManager();
let NotifierInstance =  Notifier;            //⚡ Crea una instancia de la clase notificaciones
let formNotifier = new FormNotifier();
let ApiClient = new ApiClientService(ApiConfig);
let validator;                             // ⚡ la instanciamos después de que el DOM esté listo
let IdEmpresa = 0;
let NombreEmpresa = "";

document.addEventListener("DOMContentLoaded", async () => {
     PluginControles.init();
    // ✅ Crear instancia solo cuando el DOM está cargado
    validator = new Validacion(formNotifier);
    await InitActionManager();
    await JLoadFormInicioSesion();
});

const InitActionManager = async () => {
    try {
        App = new ActionsManager(Spinner, NotifierInstance, {
            errorPosition: "center",
            errorPersist: false,
            warningPosition: "bottom-left",
            warningTimeout: 7000,
            blockMode: "local"
        });

        App.registerBatch({
            login: {
                validar: async ({ element }) => {
                    await ValidaUserAplicacion();
                }
            }
        });

        // ⚡ Vinculamos eventos automáticamente
        App.bindEvents();
        validator.refreshBindings(document);

    } catch (e) {
        alert(e);
    }
}


const ValidaUserAplicacion = async () => {
    try {
        // Limpiar notificaciones previas
        formNotifier.clearAll();

        const form = document.getElementById("login_usuario");
        validator.refreshBindings(form);

        // 1️⃣ Validación local
        const valid = validator.validarFormulario(form, { tooltipPosition: "bottom" });
        if (!valid) return; // ❌ Validación fallida → no mostrar spinner ni bloquear botón

        // 2️⃣ Mostrar spinner global y bloquear botón
        Spinner.showGlobal("file", { blockButtons: true, buttons: ["boton_sesioon"] });

        // Guardar marca de inicio en SpinnerManager ya se hace automáticamente
        // No necesitamos await aquí, SpinnerManager manejará minGlobalTime

        // 3️⃣ Obtener valores del formulario
        const ValueUsuario = document.getElementById("usuario").value;
        const ValuePassword = document.getElementById("password").value;
        const IdModulo = PluginControles.getSelectedValue("selectModulos")?.value || "0";

        // 4️⃣ Llamada al API
        const Rest = await ApiClient
            .use("EstruEmpresa")
            .call("ValidaUserAplicacion", {
                IdEmpresa: IdEmpresa,
                IdModulo: IdModulo,
                User: ValueUsuario,
                Pasword: ValuePassword
            });

        // 5️⃣ Validar respuesta del backend
        if (!Rest || Rest.error) {
            const msg = Rest?.message || Rest?.data?.Message || "Error en la solicitud.";
            NotifierInstance.show(`⚠️ ${msg}`, "info");
            await Spinner.hideGlobal(); // liberar spinner respetando minGlobalTime
            return;
        }

        // Retardo opcional para transición suave (ej. 0.25s)
        await new Promise(resolve => setTimeout(resolve, 250));

        // 6️⃣ Navegación a la página destino
        // 🔹 No liberamos el spinner aquí, bloquea la página hasta que cargue
        window.location.replace('../Home/Home/');
        //window.location.replace('@Url.Action("Home", "Home")');

    } catch (err) {
        NotifierInstance.show(`❌ Error: ${err.message}`, "error");
        console.error(err);
        await Spinner.hideGlobal(); // liberar spinner en caso de error respetando minGlobalTime
    }
};

/* 
 * Carga el inicio de sesión del formulario de inicio
 * */
const JLoadFormInicioSesion = async () => {
    try {
        const rest = await JSolicitaEstructuraEmpresa();
        if (!rest || !rest.data) {
            NotifierInstance.show("⚠️ No se pudo obtener la estructura de la empresa.", "warning");
            return;
        }
        // Si la API devuelve error o no éxito
        if (rest.data.Success === false) {
            NotifierInstance.show("⚠️ " + (rest.data.Message || "Error en la solicitud."), "warning");
            console.warn("Respuesta con error:", rest);
            return;
        }
        console.log("Estructura de empresa obtenida:", rest.data);
        IdEmpresa = rest.data.ID_EMPRESA;
        NombreEmpresa = rest.data.RAZON_SOCIAL_EMPRESA;
        let restMOD = await JSolicitaModulosEmpresa(IdEmpresa, "selectModulos");
        if (restMOD.mensaje !== "OK") {
            NotifierInstance.show(restMOD.mensaje, "danger");
        }

    } catch (ex) {
        console.error("Excepción en JLoadFormInicioSesion:", ex.message);
        NotifierInstance.show("❌ Error al cargar la estructura de la empresa.", "danger");
    }
};
/**
 * Solicita la estructura de la empresa activa
 * para los modulos
 * */
async function JSolicitaEstructuraEmpresa() {
    try {
        const Rest = await ApiClient
            .use("EstruEmpresa")
            .call("SolicitaEstructuraEmpresa", {});

        // Validar respuesta del servidor
        if (!Rest || Rest.error || Rest.data?.Success === false) {
            const msg = Rest?.message || Rest?.data?.Message || "Error en la solicitud.";
            console.warn("Respuesta con error:", Rest);
            return { mensaje: msg, data: Rest?.data || null };
        }

        // Retorno exitoso
        return { mensaje: "OK", data: Rest.data };

    } catch (ex) {
        console.error("Error JSolicitaEstructuraEmpresa:", ex.message);
        return { mensaje: ex.message || "Excepción al solicitar estructura de empresa.", data: null };
    }
}

/**
 * Consulta módulos de una empresa y llena un select
 * @param {number} idEmpresa
 * @param {string} selectId
 * @returns {Promise<{mensaje: string, data: any}>}
 */
async function JSolicitaModulosEmpresa(idEmpresa, selectId) {
    try {
       

        if (!idEmpresa) {
            return { mensaje: "⚠️ Debe indicar un ID de empresa válido.", data: null };
        }

        /** @type {EmpresaGestionDocumentalDto} */
        const payload = { idEmpresa };

        const Rest = await ApiClient
            .use("Modulos")
            .call("SolicitaModulosEmpresa", payload);

        if (!Rest || Rest.error || Rest.data?.Success === false) {
            return { mensaje: Rest?.message || "Error al solicitar módulos", data: Rest?.data ?? null };
        }

        await ApiClient.fillCustomSelectKeyValue_(
            Rest.data,
            selectId,
            payload,
            "ID_MODULO",
            "NOMBRE_MODULO"
        );

        return { mensaje: "OK", data: Rest.data };

    } catch (ex) {
        return { mensaje: ex.message, data: null };
    }
}


