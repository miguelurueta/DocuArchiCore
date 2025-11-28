// ActionsManager (v2) — con soporte para page-scopes / namespaces
class ActionsManager {
    constructor(spinner = null, notifier = null, config = {}) {
        // scopes "normales" y "page:" prefijados se almacenan aquí
        this.scopes = {
            global: {}
        };

        // listado de page-scopes registrados (clave -> true)
        this.pageScopeNames = new Set();
        this.currentPageScope = "";
        this.spinner = spinner;
        this.notifier = notifier;
        this.config = Object.assign({
            debug: true,
            errorPosition: "top-right",
            errorPersist: false,
            warningPosition: "bottom-left",
            warningTimeout: 4000,
            blockMode: "local"
        }, config);
    }

    log(...args) {
        if (this.config.debug) console.log(...args);
    }

    // registra una sola acción en scope "normal" (global o cualquier otro)
    register(scope, action, fn) {
        if (!this.scopes[scope]) this.scopes[scope] = {};
        this.scopes[scope][action] = fn;
        //this.log(`📌 Acción registrada: ${scope}.${action}`);
    }

    // registra un batch en scopes "normales"
    registerBatch(batch) {
        this.log("📦 Antes de registrar batch:", batch);
        for (let scope in batch) {
            if (!this.scopes[scope]) this.scopes[scope] = {};
            Object.assign(this.scopes[scope], batch[scope]);
            //this.log(`➡ Registrado batch scope: ${scope}`);
        }
        //this.log("🏁 Scopes cargados:", this.scopes);
    }

    // ============================
    //  PAGE SCOPES (namespace por página)
    // ============================
    // Registra un batch dentro de un scope exclusivo de página.
    // name debe ser único por página (ej: "radicacion_123" o "page_radicacion")
    registerPageScope(name, batch) {
        if (!name) throw new Error("registerPageScope requiere un nombre");
        const key = `page:${name}`;
        if (!this.scopes[key]) this.scopes[key] = {};
        Object.assign(this.scopes[key], batch);
        this.pageScopeNames.add(key);
        //this.log(`🟦 Page-scope registrado: ${key}`, this.scopes[key]);
    }

    // elimina un page scope por nombre
    removePageScope(name) {
        const key = `page:${name}`;
        if (this.scopes[key]) {
            delete this.scopes[key];
            this.pageScopeNames.delete(key);
            //this.log(`🗑️ Page-scope eliminado: ${key}`);
        }
    }

    // limpia TODOS los page-scopes (no toca scopes globales ni compartidos)
    clearPageScopes(name) {

        this.log(`🧹 Limpiando page-scopes excepto: "${name}"`);

        for (const key of Array.from(this.pageScopeNames)) {

            if (key === name) {
                this.log(`✔ Conservado: ${key}`);
                continue;   // 🚫 NO eliminar este
            }

            // borrar el scope completo
            if (this.scopes[key]) {
                delete this.scopes[key];
                //this.log(`❌ Scope eliminado: ${key}`);
            }

            // borrar registro del set
            this.pageScopeNames.delete(key);
        }

        //this.log("📦 Scopes después de limpiar:", this.listScopes());
    }

    // lista scopes (para debug)
    listScopes() {
        return Object.keys(this.scopes);
    }

    // ============================
    // run: primero busca en scopes "normales", luego en page:scope si el scope coincide
    // ============================
   
    setCurrentPageScope(name) {
        this.currentPageScope = name;
        //this.log("📍 Current pageScope actualizado →", name);
    }

    async run(scope, action, page, context = {}) {
        let pageScopeName = context.page; // ← nombre real del módulo
        let pageKey = `page:${pageScopeName}`;

        this.log("📦 run -> scope:", scope, "action:", action, "pageScope:", pageScopeName);
        
        // 1️⃣ Buscar dentro del page-scope REAL
        if (this.scopes[pageKey] &&
            this.scopes[pageKey][scope] &&
            this.scopes[pageKey][scope][action]) {

            return await this.scopes[pageKey][scope][action](context);
        }

        // 2️⃣ Scope normal
        if (this.scopes[scope] && this.scopes[scope][action]) {
            return await this.scopes[scope][action](context);
        }

        console.warn(`⚠️ Acción no encontrada: ${scope}.${action} (pageScope: ${pageScopeName})`);
        this.log("Scopes disponibles:", this.listScopes());
        let json = JSON.stringify(this.scopes, null, 2);
        console.log(json);
    }

    // limpia todos los scopes que no sean 'global' (antigua clearDynamicScopes)
    clearDynamicScopes() {
        this.log("🧹 Limpiando scopes dinámicos (no-global)...");
        for (const key of Object.keys(this.scopes)) {
            if (key !== "global") {
                delete this.scopes[key];
                this.log(`❌ Scope eliminado: ${key}`);
            }
        }
        // reset pageScopeNames porque ya fueron borrados
        this.pageScopeNames = new Set();
        this.log("💣 después de limpiar, scopes es:", this.scopes);
    }

    initializeGlobalEvents() {

        // ============================================
        // 1) CLICK universal (excepto SELECT)
        // ============================================
        document.addEventListener("click", e => {
            let el = e.target.closest("[data-action]");
            if (!el) return;

            // ⛔ Si es SELECT → NO se procesa en click
            if (el.tagName === "SELECT") return;

            // Si define data-event="change", tampoco va por click
            if (el.dataset.event === "change") return;

            this._dispatchEvent("click", el, e);
        }, true);


        // ============================================
        // 2) CHANGE universal (solo SELECT por defecto)
        // ============================================
        document.addEventListener("change", e => {
            let el = e.target.closest("[data-action]");
            if (!el) return;

            // ✔ solo SELECT o elementos que pidan explícitamente "change"
            if (el.tagName !== "SELECT" && el.dataset.event !== "change") return;

            this._dispatchEvent("change", el, e);
        });
        document.addEventListener("tomselect:itemadd", e => {
            const { element, value, item } = e.detail;
            const scope = element.dataset.scope || "tomselect";
            const action = element.dataset.action;
            const page = element.dataset.page;
            AM.run(scope, action, page, { value, item, element });
        });

        document.addEventListener("tomselect:itemremove", e => {
            const { element, value } = e.detail;
            const scope = element.dataset.scope || "tomselect";
            const action = element.dataset.action;
            const page = element.dataset.page;
            AM.run(scope, action, page, { value, element });
        });

        document.addEventListener("tomselect:type", e => {
            const { element, query } = e.detail;
            const scope = element.dataset.scope || "tomselect";
            const action = element.dataset.action;
            const page = element.dataset.page;
            AM.run(scope, action, page, { query, element });
        });

        document.addEventListener("tomselect:clear", e => {
            const { element } = e.detail;
            const scope = element.dataset.scope || "tomselect";
            const action = element.dataset.action;
            const page = element.dataset.page;
            AM.run(scope, action, page, { element });
        });

       
    }

    _dispatchEvent(type, el, eventObj) {
        const scope = el.dataset.scope || "global";
        const action = el.dataset.action;
        const page = el.dataset.page;

        this.log(`🎯 Evento → ${type} | scope="${scope}" | action="${action}" | page="${page}"`);

        this.run(scope, action, page, {
            eventType: type,
            event: eventObj,
            element: el,
            value: el.value,
            ...el.dataset,
            pageScopeName: page,

            // extras tomselect
            tomValue: eventObj.detail?.value,
            tomItem: eventObj.detail?.item,
            tomRawEvent: eventObj.detail?.originalEvent,
            tomQuery: eventObj.detail?.query
        });
    }


    // binding global de eventos (compatibilidad)
    bindEvents(root = document) {
        root.addEventListener("click", async (e) => {
            const el = e.target.closest("[data-action]");
            if (!el) return;

            const scope = el.dataset.scope || "global";
            const action = el.dataset.action;
            const blockMode = el.dataset.block || this.config.blockMode;
            const context = { event: e, element: el, ...el.dataset };

            try {
                if (blockMode === "local" && el.id && this.spinner) {
                    this.spinner.showOnButton(el.id, "circle");
                }
                if (blockMode === "global" && this.spinner) {
                    this.spinner.showGlobal("circle");
                }

                await this.run(scope, action, context);

            } catch (ex) {
                if (this.notifier) {
                    this.notifier.show(
                        `❌ Error: ${ex.message}`,
                        "error",
                        this.config.errorPosition,
                        { persistent: this.config.errorPersist }
                    );
                }
            } finally {
                if (blockMode === "local" && el.id && this.spinner) {
                    this.spinner.hideOnButton(el.id);
                }
                if (blockMode === "global" && this.spinner) {
                    this.spinner.hideGlobal();
                }
            }
        });
    }
}

// Crear la instancia global exactamente como antes
window.ActionsManagerApp = new ActionsManager(window.SpinnerInstance || null, window.Notifier || null, { debug: true });
