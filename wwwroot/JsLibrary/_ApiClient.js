/**
 * ApiClient.js
 * Cliente genérico para consumir servicios REST, MVC o ASMX
 * con soporte para cookies de sesión, formularios, JSON, QueryString,
 * validación de campos, select dinámicos y tablas. ASMXClient
 */
// 📌 Configuración global centralizada (no depende de window)
// ===============================
// 🧭 Configuración Global de API
// ===============================
const ApiConfig = (function () {
    const pathSegments = window.location.pathname.split('/').filter(Boolean);
    const appName = pathSegments.length > 0 ? pathSegments[0] : "";
    const BASE = appName ? `${window.location.origin}/${appName}` : window.location.origin;

    // 🌐 Definimos las URLs por entorno
    const environments = {
        DEV: {
            EstruEmpresa: `${BASE}/Account/`,
            Modulos: `${BASE}/Modulos/`,
            Home: `${BASE}/`,
            Radicacion: `/Radicacion/`
        },
        PROD: {
            EstruEmpresa: "https://miapp.com/Account/",
            Modulos: "https://miapp.com/Modulos/"
        }
    };

    // ⚙️ Configuraciones globales adicionales
    let settings = {
        timeout: 10000,
        tooltipPosition: "top",
        headers: {
            "Content-Type": "application/json"
        }
    };

    // ✨ Entorno activo por defecto
    let currentEnv = "DEV";

    // 🧰 Helper interno para limpiar URLs
    function joinUrl(base, path) {
        if (!base.endsWith("/")) base += "/";
        if (path.startsWith("/")) path = path.substring(1);
        return base + path;
    }

    return {
        // ==========================
        // 🌐 ENTORNOS
        // ==========================

        getUrls() {
            return environments[currentEnv];
        },

        setEnv(env) {
            if (environments[env]) {
                currentEnv = env;
                console.info(`✅ Entorno cambiado a: ${env}`);
            } else {
                console.warn(`⚠️ Entorno "${env}" no existe. Se mantiene "${currentEnv}".`);
            }
        },

        getEnv() {
            return currentEnv;
        },

        // ==========================
        // ⚙️ CONFIGURACIONES GLOBALES
        // ==========================

        get(key) {
            return settings[key];
        },

        set(key, value) {
            settings[key] = value;
        },

        getAll() {
            return { ...settings };
        },

        update(newSettings) {
            settings = { ...settings, ...newSettings };
        },

        // ==========================
        // 🧭 URL BUILDER
        // ==========================

        /**
         * Construye una URL completa a partir de la clave base y un endpoint
         * @param {string} baseKey Nombre de la base en environments (Ej: "EstruEmpresa")
         * @param {string} endpoint Nombre o ruta del método/servicio
         * @returns {string} URL final lista para usar
         */
        buildUrl(baseKey, endpoint) {
            const urls = this.getUrls();
            if (!urls[baseKey]) {
                console.error(`❌ No existe la clave base "${baseKey}" en el entorno ${currentEnv}`);
                return null;
            }
            return joinUrl(urls[baseKey], endpoint);
        }
    };
})();
class ApiClientService {
    constructor(config) {
        // 🧭 Ya no necesitas guardar las URLs aquí
        this.serviceKey = null;

        this.globalOptions = {
            delay: 300,
            minChars: 3,
            maxResults: 10,
            urlOverride: null,
            useCredentials: ApiConfig.get("useCredentials") ?? true
        };

        this.validationOptions = {
            tooltipPosition: ApiConfig.get("tooltipPosition") || "right"
        };

        this.validator = typeof Validacion !== "undefined" ? new Validacion(new FormNotifier()) : null;
    }

    // 🧭 Selecciona el servicio por nombre (no URL directa)
    use(serviceName) {
        const urls = ApiConfig.getUrls();
        if (!urls[serviceName]) throw new Error(`❌ Servicio '${serviceName}' no está configurado en ApiConfig`);
        this.serviceKey = serviceName;
        return this;
    }

    // 📡 Llamada genérica a los servicios
    async call(endpoint, parameters = {}, options = {}) {
        if (!this.serviceKey) throw new Error("⚠️ Debes seleccionar un servicio con .use(nombreServicio)");

        const mergedOptions = { ...this.globalOptions, ...options };
        const url = mergedOptions.urlOverride || ApiConfig.buildUrl(this.serviceKey, endpoint);
        //console.log(url);
        try {
            const payload = typeof parameters === "object" ? parameters : { value: String(parameters) };

            if (this.validator) this.validator.clearBackendErrors();
            //console.log(JSON.stringify(payload));
            const response = await fetch(url, {
                method: "POST",
                credentials: mergedOptions.useCredentials ? "include" : "same-origin",
                headers: ApiConfig.get("headers") || { "Content-Type": "application/json" },
                body: JSON.stringify(payload)
            });

            if (!response.ok) return { error: true, status: response.status, message: response.statusText };

            const responsejson = await response.json();
            const result = responsejson?.d || responsejson?.Data || responsejson?.Result || responsejson;
            const AuxData = responsejson?.AuxData;
            const MetaTable = responsejson?.MetaTable;
            if (!result) return { error: true, message: "Respuesta vacía del servicio", raw: responsejson };

            if (responsejson.Meta?.ValidationErrors?.length > 0) {
                if (this.validator) {
                    this.validator.applyBackendErrors(
                        responsejson.Meta.ValidationErrors,
                        this.validationOptions.tooltipPosition
                    );
                }
                return { error: true, message: responsejson.Message, validationErrors: responsejson.Meta.ValidationErrors, raw: responsejson };
            }

            const first = Array.isArray(result) ? result[0] : result;
            const errorMessageGeneral = [
                first?.AppError,
                first?.ErrorMensaje,
                first?.error_gestion,
                first?.error_sistema,
                first?.Error_result,
                first?.ErrorMessage, 
                first?.Message
            ]
                .filter(Boolean) // elimina null, undefined, ""
                .join(" | ");    // une los mensajes con un separador

           
            const errorMessage = first?.Success;
            if (errorMessage === false) {
                return { error: true, message: errorMessageGeneral, data: result, raw: responsejson };
            }
            return { error: false, message: errorMessageGeneral || "YES", data: result, raw: responsejson, AuxData: AuxData, MetaTable: MetaTable  };
        } catch (err) {
            return { error: true, message: err.message };
        }
    }

    // 🪝 fillSelect
    async fillSelect(serviceName, methodName, selectId, extraParams = {}, valueField = "id", textField = "name") {
        const selectEl = document.getElementById(selectId);
        if (!selectEl) return console.error(`❌ No se encontró <select> con id="${selectId}"`);

        const result = await this.use(serviceName).call(methodName, extraParams);
        if (result.error) return console.error("⚠️ Error en fillSelect:", result.message);

        selectEl.innerHTML = "";
        const optDefault = document.createElement("option");
        optDefault.value = "";
        optDefault.textContent = "Seleccione...";
        selectEl.appendChild(optDefault);

        result.data.forEach(item => {
            const option = document.createElement("option");
            option.value = item[valueField] ?? "";
            option.textContent = item[textField] ?? "";
            selectEl.appendChild(option);
        });
    }

    // 🪝 fillCustomSelectKeyValue
    async fillCustomSelectKeyValue(serviceName, methodName, selectId, extraParams = {}, keyId = "ID_MODULO", keyText = "NOMBRE_MODULO") {
        try {
            const result = await this.use(serviceName).call(methodName, extraParams);
            const container = document.getElementById(selectId);
            const optionsList = container?.querySelector(".options");
            if (!optionsList) return console.error(`❌ No se encontró .options dentro de #${selectId}`);

            optionsList.innerHTML = "";
            const data = Array.isArray(result.data) ? result.data : [];
            if (!data.length) {
                optionsList.innerHTML = `<li data-value="">Sin módulos disponibles</li>`;
                return;
            }

            const normalizados = data.map(arr => {
                const obj = {};
                arr.forEach(pair => {
                    if (pair.Key && pair.Value !== undefined) obj[pair.Key] = pair.Value;
                });
                return obj;
            });

            normalizados.forEach(item => {
                const li = document.createElement("li");
                li.dataset.value = item[keyId];
                li.textContent = item[keyText];
                optionsList.appendChild(li);
            });
        } catch (ex) {
            console.error("💥 Error en fillCustomSelectKeyValue:", ex);
        }
    }
    async fillCustomSelectKeyValue_(_data, selectId, extraParams = {}, keyId = "ID_MODULO", keyText = "NOMBRE_MODULO") {
        try {
            //const result = await this.use(serviceName).call(methodName, extraParams);
            const container = document.getElementById(selectId);
            const optionsList = container?.querySelector(".options");
            if (!optionsList) return console.error(`❌ No se encontró .options dentro de #${selectId}`);

            optionsList.innerHTML = "";
            const data = Array.isArray(_data) ? _data : [];
            if (!data.length) {
                optionsList.innerHTML = `<li data-value="">Sin módulos disponibles</li>`;
                return;
            }
            console.log(data);
            const normalizados = data.map(item => {
                const obj = {};
                Object.keys(item).forEach(key => {
                    obj[key] = item[key]; // Aquí puedes agregar lógica si necesitas transformar los valores
                });
                return obj;
            });

            normalizados.forEach(item => {
                const li = document.createElement("li");
                li.dataset.value = item[keyId];
                li.textContent = item[keyText];
                optionsList.appendChild(li);
            });
        } catch (ex) {
            console.error("💥 Error en fillCustomSelectKeyValue_:", ex);
        }
    }
    // 🪝 fillTable
    async fillTable(serviceName, methodName, tableId, extraParams = {}, columns = []) {
        const tableEl = document.getElementById(tableId);
        if (!tableEl) return console.error(`❌ No se encontró tabla con id="${tableId}"`);

        const result = await this.use(serviceName).call(methodName, extraParams);
        if (result.error) return console.error("⚠️ Error en fillTable:", result.message);

        const data = Array.isArray(result.data) ? result.data : [result.data];
        tableEl.innerHTML = "";

        const thead = document.createElement("thead");
        const headerRow = document.createElement("tr");

        if (columns.length === 0 && data.length > 0) {
            columns = Object.keys(data[0]).map(k => ({ header: k, field: k }));
        }

        columns.forEach(col => {
            const th = document.createElement("th");
            th.textContent = col.header;
            headerRow.appendChild(th);
        });

        thead.appendChild(headerRow);
        tableEl.appendChild(thead);

        const tbody = document.createElement("tbody");
        data.forEach(item => {
            const row = document.createElement("tr");
            columns.forEach(col => {
                const td = document.createElement("td");
                td.textContent = item[col.field] ?? "";
                row.appendChild(td);
            });
            tbody.appendChild(row);
        });

        tableEl.appendChild(tbody);
    }
}

