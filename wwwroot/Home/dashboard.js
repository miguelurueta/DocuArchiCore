// ===============================================
//  📌 Alias global del ActionsManager
// ===============================================
const AM = window.ActionsManagerApp;
class AppDashboard {
    constructor() {
        this.Spinner = new SpinnerManager();
        this.Notifier = Notifier;
        this.formNotifier = new FormNotifier();
        this.ApiClient = new ApiClientService(ApiConfig);
        this.validator = null;
        this.menuData = null;
        this.dynamicLoader = null;
        this.dashboard = null;
        this.menuManager = null;
        this.pageScopeName = "dashboard";
        // 🔥 Usa el ActionsManager global
        //ActionsManagerApp = window.ActionsManagerApp;

        document.addEventListener("DOMContentLoaded", () => this.init());
    }

    async init() {
        try {
            PluginControles.init();
            await this.initActionManager();   // ahora seguro
            await this.loadDashboard();
            this.initSlider();
        } catch (e) {
            console.error("❌ Error inicializando AppDashboard:", e);
        }
    }

    initSlider() {
        const toggleBtn = document.getElementById("toggleSidebar");
        const body = document.body;

        if (toggleBtn) {
            toggleBtn.addEventListener("click", () => {
                body.classList.toggle("sidebar-hidden");
            });
        }
    }

    /** -----------------------------------------------------
     *  📌 INIT ACTION MANAGER (versión Dashboard segura)
     * ----------------------------------------------------- */
    async initActionManager() {
        try {
            console.log("🟦 AppDashboard → initActionManager()");

            // ⚠ NO instanciamos nuevamente el ActionsManager
            // ya existe como window.ActionsManagerApp
            AM.clearPageScopes(this.NameDefault);
            AM.registerPageScope(this.pageScopeName, {
                boton: {
                    refrescar: async (ctx) => {
                        this.Notifier.show("♻ Refrescando dashboard…", "info");
                        // await this.loadDashboard();
                    },
                    abrirSoporte: async (ctx) => {
                        this.Notifier.show("📞 Abriendo soporte técnico", "info");
                    }
                }
            });
            console.log("🟩 Scopes dashboard añadidos:", AM.scopes);
           
            // ------------------------------------
            // 🔥 Activar el Event Dispatcher Global
            //    (solo la primera vez en toda la app)
            // ------------------------------------
            if (!window.__AM_EVENTS_INITIALIZED__) {
                AM.initializeGlobalEvents();
                window.__AM_EVENTS_INITIALIZED__ = true;
                console.log("🌐 Eventos globales inicializados!");
            }
        } catch (e) {
            console.error("❌ Error en initActionManager:", e);
        }
    }
    _bindDashboardActions() {
        document.addEventListener("click", e => {
            const el = e.target.closest("[data-action]");
            if (!el) return;
            const action = el.dataset.action;
            const scope = el.dataset.scope ;
            const page = el.dataset.page;
            console.log(`🟦 Dashboard ejecuta acción: ${action} (scope: ${scope})`);

            AM.run(scope, action, page, {
                element: el,
                value: el.value,
                ...el.dataset,
                pageScopeName: page
            });
        });
    }


    /** -----------------------------------------------------
     *  📌 Carga inicial del dashboard (menú/cards/sesión)
     * ----------------------------------------------------- */
    async loadDashboard() {
        try {
            let rest = await this.solicitaEstructuraMenuPrincipal();

            if (rest.mensaje !== "OK") {
                this.Notifier.show(rest.mensaje, "danger");
                return;
            }

            this.menuData = typeof rest.data === "string"
                ? JSON.parse(rest.data)
                : rest.data;

            this.dynamicLoader = new DynamicLoader({
                mainContentSelector: "#mainContent",
                legacyFrameSelector: "#legacyFrame"
            });

            // ---- Menú lateral ----
            const menu = new DynamicMenu("#sidebarMenu");
            await menu.buildMenu(this.menuData);

            this.menuManager = new MenuManager(menu.ulRoot, {
                framework: "bootstrap",
                debug: true,
                onItemClick: (link) => {
                    let result = this.onSelectMenu(link.dataset.id);
                    if (result !== "YES") {
                        this.Notifier.show("❌ " + result, "danger");
                    }
                }
            });

            // ---- Cards ----
            const valores = [
                { ValueNode: "WF-CL-01_", Valor: 1234 },
                { ValueNode: "CO-CL-02_", Valor: 89 }
            ];

            this.dashboard = new CardDashboard("contenedorCards");

            this.dashboard.onCardClick = (item, cardEl) => {
                let res = this.onSelectMenu(item.IdMenuPrincipal);
                if (res !== "YES") {
                    this.Notifier.show("❌ " + res, "danger");
                }
                let sel = this.menuManager.getSelectedItem();
                if (sel) {
                    this.menuManager.toggleItemSelection(sel.dataset.id);
                }
                this.menuManager.toggleItemSelection(item.IdMenuPrincipal);
            };

            this.dashboard.cargar(this.menuData, valores);

            // ---- Caracterización del usuario ----
            rest = await this.solicitaCaraterizacionUsuario();
            if (rest.mensaje !== "OK") {
                this.Notifier.show(rest.mensaje, "danger");
                return;
            }

            let config = this.configurarLogin(rest.AuxData, rest.data);
            if (config !== "OK") {
                this.Notifier.show(config, "danger");
            }

            // ---- Sesión ----
            const sessionManager = new SessionManager({
                serviceKey: "Home",
                keepAliveEndpoint: "KeepAlive",
                tiempoRestanteEndpoint: "TiempoRestante",
                logoutUrl: "/"
            });

            document.getElementById("btnCerrarSesion")
                .addEventListener("click", () => sessionManager.mostrarConfirmacionCerrarSesion());

            document.getElementById("btnInicio")
                .addEventListener("click", () => {
                    this.dynamicLoader.swith = 2;
                    this.dynamicLoader.CardView();
                });

        } catch (ex) {
            console.log("Excepción en loadDashboard:", ex);
            this.Notifier.show("❌ Error al cargar la estructura de la empresa.", "danger");
        }
    }

    /** -----------------------------------------------------
     *  📌 Navegación de módulos (ASP.NET o MVC)
     * ----------------------------------------------------- */
    onSelectMenu(Id) {
        try {
            const item = this.buscarMenuPorId(Id);
            if (!item) return `No se encontró el menú ${Id}`;

            const url = item.UrlNode + item.PageName;
            const isAspx = url.endsWith(".aspx");

            this.dynamicLoader.swith = 1;
            this.dynamicLoader.clearView();

            if (isAspx) {
                this.dynamicLoader.loadIframe(url);
            } else {
                this.dynamicLoader.swith = 2;
                this.dynamicLoader.parametros = item;
                this.dynamicLoader.loadMvcPage(url);
            }

            document.getElementById("lblNameModulo").textContent = item.NombreModulo;

            return "YES";
        } catch (ex) {
            return ex.message;
        }
    }

    buscarMenuPorId(id) {
        return this.menuData.find(m => Number(m.IdMenuPrincipal) === Number(id)) || null;
    }

    // -----------------------------
    // Servicios REST y Login
    // -----------------------------
    async solicitaEstructuraMenuPrincipal() {
        try {
            const Rest = await this.ApiClient
                .use("Home")
                .call("ServiceSolicitaEstructuraMenuPrincipal", {});

            if (!Rest || Rest.error || Rest.data?.Success === false) {
                const msg = Rest?.message || Rest?.data?.Message || "Error obteniendo menú.";
                return { mensaje: msg, data: Rest?.data || null };
            }

            return { mensaje: "OK", data: Rest.data };

        } catch (e) {
            return { mensaje: e.message, data: null };
        }
    }

    async solicitaCaraterizacionUsuario() {
        try {
            const Rest = await this.ApiClient
                .use("Home")
                .call("ServiceSolicitaCaraterizacionUsuarioLogueado", {});

            if (!Rest || Rest.error || Rest.data?.Success === false) {
                const msg = Rest?.message || Rest?.data?.Message || "Error obteniendo caracterización.";
                return { mensaje: msg, data: Rest?.data };
            }

            return { mensaje: "OK", data: Rest.data, AuxData: Rest.AuxData };

        } catch (e) {
            return { mensaje: e.message, data: null };
        }
    }

    configurarLogin(nombreModulo, datos) {
        try {
            let nombre = "";

            switch (nombreModulo) {
                case "GESTOR DOCUMENTAL":
                    nombre = datos.Nombre_Remitente;
                    break;
                case "WORKFLOW DOCUMENTAL":
                    nombre = datos.Nombre_Usuario;
                    break;
                case "DOCUARCHI CONTENEDOR":
                    nombre = datos.nombre;
                    break;
            }

            return this.actualizarUsuario(nombre);

        } catch (e) {
            return e.message;
        }
    }

    actualizarUsuario(nombre) {
        try {
            const nombreEl = document.querySelector('#userMenu strong');
            const imgEl = document.querySelector('#userMenu img');

            if (nombreEl && imgEl) {
                nombreEl.textContent = this.dashboard._toCapitalCase(nombre);
                imgEl.src = `https://ui-avatars.com/api/?name=${encodeURIComponent(nombre)}&background=0D8ABC&color=fff`;
            }
            return "OK";

        } catch (e) {
            return e.message;
        }
    }
}


// 🔥 Instancia global automática
const App_ = new AppDashboard();




