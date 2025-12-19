
// ===============================================
//  📌 MÓDULO PRINCIPAL DE RADICACIÓN
// ===============================================
class RadicacionModule {
    constructor() {
        // Servicios internos
        this.Spinner = new SpinnerManager();
        this.Notifier = Notifier;
        this.formNotifier = new FormNotifier();
        this.ApiClient = new ApiClientService(ApiConfig);
        this.validator = null;
        this.pageScopeName = "radicacion";
        // Parámetros y estados internos
        this.Parameter = null;
        this.CamposRadicacion = null;
        this.formBuilder = null;
        this.CDeRelacionEstadoRetriccionD = null;
        this.TablePendientes = null;
       
    }
    // =======================================================
    //  🔥 Método principal de inicialización
    // =======================================================
    async init(Parameter) {

        try {
            this.Parameter = Parameter;

            PluginControles.init();
            this.validator = new Validacion(this.formNotifier);
          
            // 1) Cargar estructura y renderizar UI
            await this.loadPage();
            //await this.initTables();
            // 2) Inicializar acciones y scopes
            await this.initActionManager();

            
            

        } catch (ex) {
            console.error("❌ Error inicializando radicación:", ex);
            this.Notifier.show("Error al inicializar Radicación", "danger");
        }
    }



    // =======================================================
    //  🔥 GESTIÓN DE SCOPES Y ACCIONES
    // =======================================================
    async initActionManager() {

        try {
            //console.log("🟦 InitActionManager INICIO");

            // ------------------------------------
            // 🔥 Limpiar su scopes 
            // ------------------------------------
            AM.removePageScope(this.pageScopeName);

            //console.log("🟩 Registrando nuevos scopes...");

            // ------------------------------------
            //  🔥 Registro por lotes
            // ------------------------------------
            AM.registerPageScope(this.pageScopeName, {
                tomselect: {
                    editarRegistro: async ({ id }) => { this.Notifier.show(`Editar registro ${id}`, "info"); },
                    eliminarRegistro: async ({ id }) => { this.Notifier.show(`Eliminar registro ${id}`, "warning"); },
                    onType: async ({ query, element }) => {
                        console.log("Buscando:", query);
                    },
                    onClear: async ({ element }) => {
                        console.log("Se limpió el select");
                    }, selectTipo: async ({ value, element }) => {
                        //this.Notifier.show(`Seleccionaste el ID: ${value}`, "info");
                        if (value !== "" && element.dataset.idescript) {
                            let rest = await this.SolicitaDatosValidacionInterfaz(element.dataset.idescript, value);
                            if (rest !== "YES") {
                                this.Notifier.show(rest, "warning");
                            }
                        }
                        // Aquí llamas LO QUE NECESITES
                        //await this.SolicitaEstructuraRelacionTipoRestriccion(value);
                    }
                },
                selects: {
                    cargargenerico: async ({ element, value }) => {
                        this.CDeRelacionEstadoRetriccionD = null;
                        const destino = element.getAttribute("atrib_campo_drow_destino");
                        let rest = await this.SolicitaListaRelacionTramiteFlujo(element, value, destino);
                        if (rest !== "YES") {
                            this.Notifier.show(rest, "warning");
                        }
                        rest = await this.SolicitaEstructuraRelacionTipoRestriccion(value);
                        if (rest !== "YES") {
                            this.Notifier.show(rest, "warning");
                        }
                    }
                }, boton: {
                    cleaarField: async ({ element, value }) => {
                        if (this.formBuilder) {
                            this.formBuilder.clearFields();
                        }
                        
                    }, ShowPendiente: async ({ element, value }) => {
                       let rest = await this.initTables();
                        if (rest !== "YES") {
                            this.Notifier.show(rest, "warning");
                        }
                    }
                }
            });
            //console.log("🟪 Scopes cargados:", AM.scopes);


            // ------------------------------------
            // 🔥 Registrar eventos personalizados
            // ------------------------------------
            //this._bindTomSelectEvents();
            //this._bindSelectEvents();

            // ------------------------------------
            // 🔥 Activar el Event Dispatcher Global
            //    (solo la primera vez en toda la app)
            // ------------------------------------
            if (!window.__AM_EVENTS_INITIALIZED__) {
                AM.initializeGlobalEvents();
                window.__AM_EVENTS_INITIALIZED__ = true;
                //console.log("🌐 Eventos globales inicializados!");
            }
            // ------------------------------------
            // 🔥 Vincular eventos generales
            // ------------------------------------
            //AM.bindEvents();


            // ------------------------------------
            // 🔥 Actualizar validadores
            // ------------------------------------
            this.validator.refreshBindings(document);

        } catch (e) {
            console.error("⚠️ Error en initActionManager:", e);
            this.Notifier.show("Error vinculando acciones", "danger");
        }
    }

   
    // =======================================================
    //  🔥 Cargar estructura completa
    // =======================================================
    async loadPage() {

        try {
           
            let result = await this.inicializaClienteWorkflow();
            if (!result || result.error || result.data?.Success === false) {
                const msg = result?.message || result?.data?.Message || "Error en la solicitud.";
                this.Notifier?.show(msg, "danger");
                return;
            }
            result = await this.SolicitaEstructuraRadicacion(this.Parameter);
            if (!result || result.error || result.data?.Success === false) {
                const msg = result?.message || result?.data?.Message || "Error en la solicitud.";
                console.warn("Respuesta con error:", result);
                this.Notifier?.show(msg, "danger");
                return;
            }
            this.CamposRadicacion = result.data;
            //console.log(result.data);
            result = await this.LoadCamposRadicacion(this.CamposRadicacion);
            if (!result || result.error || result.data?.Success === false) {
                const msg = result?.message || result?.data?.Message || "Error en la solicitud.";
                this.Notifier?.show(msg, "danger");
                return;
            }

        } catch (ex) {
            console.error("❌ Error en loadPage:", ex);
            this.Notifier.show("Error cargando página Radicación", "danger");
        }
    }



    // =======================================================
    //  🔥 Render dinámico de controles
    // =======================================================
    async LoadCamposRadicacion(ParameterCampos) {

        try {
            //console.warn("📌 LoadCamposRadicacion recibió:", ParameterCampos);

            const campos = typeof ParameterCampos === "string"
                ? JSON.parse(ParameterCampos)
                : ParameterCampos;

            if (!Array.isArray(campos))
                throw new Error("Lista de campos inválida");


            this.formBuilder = new DynamicFormBuilder();
            this.formBuilder.ApiClient = this.ApiClient;
            this.formBuilder.configArray = campos;
            this.formBuilder.NameModulo = "radicacion";


            const contenedores = {
                CRDEST: document.getElementById("contenedor_CRDEST"),
                CREMIT: document.getElementById("contenedor_CREMIT"),
                CLASIFIC: document.getElementById("contenedor_CLASIFIC"),
                CRECEPCION: document.getElementById("contenedor_CRECEPCION"),
                CESPECIAL: document.getElementById("contenedor_CESPECIAL")
            };

            Object.values(contenedores).forEach(sec =>
                sec && (sec.innerHTML = "")
            );


            const agrupados = campos.reduce((acc, campo) => {
                const tag = campo.TagSesion?.toUpperCase() || "OTROS";
                (acc[tag] ||= []).push(campo);
                return acc;
            }, {});


            for (const tag in agrupados) {

                const zona = contenedores[tag];
                if (!zona) continue;

                agrupados[tag].forEach(campo => {
                    const control = this.formBuilder.createControl(campo);
                    if (control) zona.appendChild(control);
                });
            }

            return { mensaje: "OK" };

        } catch (ex) {
            console.error("Error en LoadCamposRadicacion:", ex);
            return { error: ex.message };
        }
    }

    // =======================================================
    //  🔥 API: Inicializa workflow
    // =======================================================
    async inicializaClienteWorkflow() {

        try {
            const result = await this.ApiClient
                .use("Radicacion")
                .call("ServiceIncializaClienteWokflowRadicador", {});

            if (!result || result.error || !result.data?.Success) {
                return { message: result?.data?.Message || "Error inicializando workflow" };
            }

            return { data: result.data, message: "OK" };

        } catch (ex) {
            return { error: ex.message };
        }
    }


    // =======================================================
    //  🔥 API: Solicita Estructura Radicación
    // =======================================================
    async SolicitaEstructuraRadicacion(ItemMenu) {
        try {
            //console.log(JSON.stringify(ItemMenu));
            const result = await this.ApiClient
                .use("Radicacion")
                .call("ServiceSolicitaEstructuraRadicacion", { ItemMenu });

            if (!result || result.error || result.data?.Success === false) {
                const msg = result?.message || result?.data?.Message || "Error en la solicitud.";
                return { mensaje: msg, data: result?.data || null };
            }
            return { mensaje: "OK", data: result.data };
        } catch (ex) {
            console.error("Error inicializando workflow:", ex.message);
            return { error: ex.message || "Excepción al solicitar estructura de radicación.", data: null };
        }
    }
    /**
     * Solicita la relación del trámite con uno o mas flujo de trabajo
     * Ing Miguel Angel Urueta Miranda
     * 2025-11-18
     * @param {any} element
     * @param {any} value
     * @param {any} destino
     */
    async SolicitaListaRelacionTramiteFlujo(element, value, destino) {
        try {
            if (!destino || destino.trim() === "") {
                return "Destino vacío o inválido";
            }

            // Obtener configuración del dropdown relacionado
            let ArrayDrowConfigList = this.formBuilder.drowConfigService(destino);
            if (!ArrayDrowConfigList || ArrayDrowConfigList.length === 0) {
                return "No existe configuración para el destino: " + destino;
            }

            // Limpiar opciones actuales del control destino
            this.formBuilder.drowDeleteAllRows(destino);

            // Agregar el valor de la condición (lo que envías al backend)
            ArrayDrowConfigList[0].value_condicion = value;

            // Llamado al servicio
            const result = await this.ApiClient
                .use("Radicacion")
                .call("ServiceSolicitaListaRelacionTramiteFlujo", {
                    ClassConfigGeneralService: ArrayDrowConfigList[0]
                });

            if (!result) {
                return "No hubo respuesta del servicio";
            }

            if (result.error) {
                return result.message ?? "Error en la respuesta del servicio";
            }

            // Obtener el control destino
            let SelectDestino = document.getElementById(destino);
            if (!SelectDestino) {
                return "Imposible encontrar el control (" + destino + ")";
            }

            // Cargar los registros
            this.formBuilder.selectAddRow(result.data, SelectDestino);

            return "YES";

        } catch (ex) {
            return "Inconsistencia general en SolicitaListaRelacionTramiteFlujo: " + (ex.message ?? ex);
        }
    }
    
    async SolicitaEstructuraRelacionTipoRestriccion(IdTipoTramite) {
        try {
            // Llamado al servicio
            const result = await this.ApiClient
                .use("Radicacion")
                .call("ServiceSolicitaEstructuraRelacionTipoRestriccion", {
                    IdTipoTramite: IdTipoTramite
                });

            // Validaciones de respuesta
            if (!result) {
                return "No hubo respuesta del servicio";
            }

            if (result.error) {
                return result.message ?? "Error en la respuesta del servicio";
            }

            // Asignar resultado a la variable del módulo
            this.CDeRelacionEstadoRetriccionD = result.data;
            let tomSelec = this.formBuilder.getTomSelectManager("Destinatario_Cor");
            if (
                this.CDeRelacionEstadoRetriccionD &&
                this.CDeRelacionEstadoRetriccionD.MoluloRadicacion == 1

            ) {
                if (tomSelec) {
                    // Resetea sin romper el componente   seting = Object {case_Option: "destinatario", case_Item: "destinatario", onInitialize: undefined, …}
                    tomSelec.reset();
                    tomSelec.seting.TomPParameterTomSelelect.IdRestriccion = this.CDeRelacionEstadoRetriccionD.IdRestriTipoDestInterno;
                    tomSelec.seting.TomPParameterTomSelelect.IdTipoRestriccion = this.CDeRelacionEstadoRetriccionD.IdTipoRestriccion;
                    let listaManual = await tomSelec._loadFromApi("*.*");
                    tomSelec.setOptions(listaManual);
                    if (listaManual.length > 0) {
                        tomSelec.ts.addItem(listaManual[0].id_value);
                    }
                    tomSelec.enableSelectOnlyMode();
                }
            } else {
                if (tomSelec) {
                    tomSelec.reset();
                    tomSelec.seting.TomPParameterTomSelelect.IdRestriccion = 0;
                    tomSelec.seting.TomPParameterTomSelelect.IdTipoRestriccion = 0;
                    tomSelec.restoreNormalMode();
                }
            }

            return "YES";

        } catch (ex) {
            return "Inconsistencia general en SolicitaEstructuraRelacionTipoRestriccion: "
                + (ex.message ?? ex);
        }
    }
    async SolicitaDatosValidacionInterfaz(IdScript, value) {
        try {
            const result = await this.ApiClient
                .use("Radicacion")
                .call("ServiceSolicitaDatosValidacionInterfaz", {
                    IdScript: IdScript, value :value
                });

            // Validaciones de respuesta
            if (!result) {
                return "No hubo respuesta del servicio";
            }

            if (result.error) {
                return result.message ?? "Error en la respuesta del servicio";
            }
            if (result.data) {
                let rest = await this.asignarValoresCampos(result.data, { disable: true, zonaSelector : "contentabElment" });
                if (rest != "YES") { return rest };
            }
            return "YES";
        } catch (ex) {
            return "Inconsistencia general en SolicitaDatosValidacionInterfaz: "
                + (ex.message ?? ex);
        }
    }
    /**
     * Asigna valores a los campos del DOM con soporte opcional para:
     *  - deshabilitar los campos
     *  - buscar dentro de una zona específica
     *
     * @param {Array} listaCampos
     * @param {Object} opciones
     *        opciones.disable -> true/false (por defecto false)
     *        opciones.zonaSelector -> ex: "#contenedorFormulario" (opcional)
     *
     * @returns {Promise<string>}
     */
    async asignarValoresCampos(listaCampos, opciones = {}) {
        const {
            disable = false,
            zonaSelector = null   // puede ser string o array
        } = opciones;

        try {
            // Para evitar race conditions con DOM dinámico
            await new Promise(res => setTimeout(res, 10));

            // Normalizar zonas
            let zonas = [];

            if (!zonaSelector) {
                zonas = [document];
            } else if (Array.isArray(zonaSelector)) {
                zonas = zonaSelector
                    .map(sel => document.querySelector(sel))
                    .filter(z => z !== null);

                if (zonas.length === 0) {
                    console.warn("⚠️ Ninguna zona encontrada, se usará document");
                    zonas = [document];
                }
            } else {
                const root = document.querySelector(zonaSelector);
                zonas = root ? [root] : [document];
                if (!root) {
                    console.warn(`⚠️ Zona no encontrada: ${zonaSelector}. Se usará document.`);
                }
            }

            // Recorrer todos los campos
            for (let item of listaCampos) {
                try {
                    const idCampo = item.NombreCampoDestinoPlaRadicacion;
                    const valor = item.ValorCampoFuentePlaValidacion;

                    if (!idCampo) {
                        console.warn("⚠️ Item sin NombreCampoDestinoPlaRadicacion:", item);
                        continue;
                    }

                    let control = null;

                    // Buscar en cada zona (con soporte para anidado)
                    for (const zona of zonas) {
                        control = zona.querySelector(`#${idCampo}`);
                        if (control) break;
                    }

                    if (!control) {
                        console.warn(`⚠️ Control no encontrado -> id="${idCampo}"`);
                        continue;
                    }

                    // Asignar valor
                    control.value = valor ?? "";

                    // Disparar evento para ActionsManager
                    control.dispatchEvent(new Event("change", { bubbles: true }));

                    // Deshabilitar campo si corresponde
                    if (disable) {
                        control.setAttribute("disabled", "disabled");
                    }

                   

                } catch (errItem) {
                    console.error(`❌ Error procesando campo "${item.NombreCampoDestinoPlaRadicacion}":`, errItem);
                }
            }

            return "YES";

        } catch (errGlobal) {
            console.log("❌ Error global en asignarValoresCampos():", errGlobal);
            return "YES";
        }
    }

    async initTables() {
        try {
            // Asegúrate de tener <div id="contenedorTablaPendientes"></div> en tu modal/body
            this.TablePendientes = new TableManager({
                containerId: "contenedorTablaPendientes",
                apiClient: this.ApiClient,
                serviceName: "Radicacion",
                apiMethod: "ServiceSolicitaListaRadicadosPendientes",
                pageSize: 10,
                scopeName: "tabla", // scope que registraremos en AM
                idField: "id_estado_radicado",
                initialFilters: {
                    // envía filtros iniciales si aplica
                    idUsuarioRadicado: 0,
                    IplantillaRadicado: 0
                },
                columns: [
                    { title: "Número Radicado", field: "consecutivo_radicado", bold: true },
                    { title: "Remitente", field: "remitente" },
                    { title: "Trámite", field: "descripcion_doc" },
                    { title: "Fecha", field: "fecha_registro", format: "date", formatMask: "DD/MM/YYYY" },
                    // puedes añadir otras columnas o una columna actions aquí si prefieres
                ],
                actions: [
                    { type: "icon", icon: "fa-eye", color: "primary", am: "ver" },
                    { type: "icon", icon: "fa-pen", color: "warning", am: "editar" },
                    { type: "button", text: "Eliminar", color: "danger", am: "eliminar" },
                    {
                        type: "dropdown",
                        icon: "fa-ellipsis-v",
                        items: [
                            { label: "Ver", am: "ver" },
                            { label: "Editar", am: "editar" },
                            { label: "Eliminar", am: "eliminar" }
                        ]
                    },
                    { type: "checkbox", field: "activo", am: "toggle" }
                ]
            });

            // render + primera carga
            await this.TablePendientes.load(1);
            let modal = new bootstrap.Modal(document.getElementById('miModal'));
            modal.show();
            return "YES";
        } catch (e) {
            return e.mensaje;
            console.error("initTables error", e);
        }
    }


    


}




// =======================================================
//  🔥 Punto de entrada global desde loadMvcPage()
// =======================================================
window.inicializarRadicacion = async function (params) {

    try {
        const modulo = new RadicacionModule();
        await modulo.init(params);

    } catch (ex) {
        console.error("❌ Error inicializando RadicacionModule:", ex);
    }
};
