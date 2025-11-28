class TomSelectManager {
    constructor(selector, settings = {}, apiClientInstance = null) {
        this.input = typeof selector === "string" ? document.querySelector(selector) : selector;
        this.seting = settings;
        this.apiClient = apiClientInstance || (window.apiClient ? window.apiClient : new ApiClientService());
        this.ts = null;
        this.dependentSelects = [];
        // Config original (importante)
        this._originalState = null;
        this._preventType = null;
        // Para botón externo
        this._buttonSelector = null;
        this._buttonClickHandler = null;
    }

    // =========================================================
    // Inicialización principal
    // =========================================================
    async init() {
        if (!this.input) {
            console.warn("TomSelectManager: No se encontró el elemento");
            return;
        }

        // Evitar doble inicialización
        if (this.ts) {
            console.warn("TomSelectManager: TomSelect ya inicializado");
            return;
        }

        // Crear instancia TomSelect
        this.ts = new TomSelect(this.input, {
            valueField: "id_value",
            labelField: "tex_value",
            searchField: ["tex_value"],
            create: this.seting.create ?? false,
            preload: this.seting.preload ?? false,
            placeholder: this.seting.placeholder ?? "Buscar...",
            loadThrottle: this.seting.delay ?? 350,
            maxItems: this.seting.maxItems ?? 1,
            render: this._renderTemplates(),
            openOnFocus: false,
            mode: this.seting.mode ?? "single",
            page: this.seting.page ?? "",
            onItemAddAction: this.seting.onItemAddAction ?? "",
            onItemRemoveAction: this.seting.onItemRemoveAction ?? "",
            scopes: this.seting.scopes ?? "",
            load: async (query, callback) => {
                try {
                    const data = await this._loadFromApi(query);
                    callback(data);
                } catch (error) {
                    console.error("Error cargando datos TomSelect:", error);
                    callback();
                }
            },
            onInitialize: () => {
                if (typeof this.seting.onInitialize === "function") {
                    this.seting.onInitialize();
                }
            },
            // ================================
            // 🔁 Eventos reemitidos a ActionsManager
            // ================================
            onItemAdd: (value, item) => {
                this._dispatchCustomEvent("tomselect:itemadd", { value, item });
            },

            onItemRemove: (value) => {
                this._dispatchCustomEvent("tomselect:itemremove", { value });
            },

            onType: (query) => {
                this._dispatchCustomEvent("tomselect:type", { query });
            },

            onClear: () => {
                this._dispatchCustomEvent("tomselect:clear", {});
            },

        });

       
        // ===============================================
        // Guardar estado original
        // ===============================================
        this._originalState = this._originalState || {
            searchField: [...this.ts.settings.searchField],
            create: this.ts.settings.create,
            load: this.ts.settings.load,
            controlDisplay: this.ts.control_input?.style?.display ?? ""
        };
        // 🔹 Marcamos como inicializado
        this.input.dataset.tsInitialized = "true";
        // Extras estéticos
        this._applyDynamicWidth();
        this._applyInputVisibilityControl();
    }
    _dispatchCustomEvent(name, detail) {
        const evt = new CustomEvent(name, { detail: { ...detail, element: this.input } });
        this.input.dispatchEvent(evt);
    }
    // =========================================
    // 🔁 Reemitir CHANGE REAL para ActionsManager
    // =========================================
    _emitChange() {
        const evt = new Event("change", { bubbles: true });
        this.input.dispatchEvent(evt);
    }
  

    // =========================================================
    // 🔧 Control dinámico de ancho según maxItems (CSS externo)
    // =========================================================
    _applyDynamicWidth() {
        if (!this.ts) return;
        const wrapper = this.ts.wrapper;
        const maxItems = this.seting.maxItems || 1;
        if (maxItems > 1) {
            wrapper.classList.add("ts-compact-items");
        } else {
            wrapper.classList.remove("ts-compact-items");
        }
    }

    // =========================================================
    // 👁️ Control de visibilidad del input según maxItems
    // =========================================================
    _applyInputVisibilityControl() {
        const tom = this.ts;
        const s = this.seting;
        if (!tom) return;

        const controlInput = tom.control_input;
        if (!controlInput) return;

        const updateVisibility = () => {
            if (s.maxItems === 1) {
                if (tom.items.length >= 1) {
                    controlInput.style.display = "none";
                } else {
                    controlInput.style.display = "";
                }
            } else {
                controlInput.style.display = "";
            }
        };

        // Aplicar al iniciar
        updateVisibility();

        // Escuchar cuando cambia la selección
        tom.on("item_add", updateVisibility);
        tom.on("item_remove", updateVisibility);
        tom.on("clear", updateVisibility);
    }

    // =========================================================
    // Cambia maxItems dinámicamente
    // =========================================================
    setMaxItems(newMax) {
        if (!this.ts) return;
        this.seting.maxItems = newMax;
        this.ts.settings.maxItems = newMax;
        this._applyDynamicWidth();
        this._applyInputVisibilityControl();
    }

    // =========================================================
    // Carga desde API REST
    // =========================================================
    async _loadFromApi(query) {
        if (!this.seting.serviceName) return [];
        const params = {
            q: query,
            case_Option: this.seting.case_Option,
            id_escript: this.seting.id_escript,
            extraParams: this.seting.extraParams || {}
        };

        try {
            const response = await this.apiClient
                .use(this.seting.serviceName)
                .call(this.seting.apiMethod, { Value: query, Parameter: this.seting.TomPParameterTomSelelect });

            if (!response || response.error === true) {
                console.log(response && response.raw ? response.raw : "Respuesta inválida de _loadFromApi");
                return [];
            }

            return Array.isArray(response.data) ? response.data : [];
        } catch (err) {
            console.log("Error en _loadFromApi:", err);
            return [];
        }
    }

    // =========================================================
    // Plantillas personalizadas
    // =========================================================
    _renderTemplates() {
        const s = this.seting;

        // Seguridad: valores siempre presentes
        const scope = s.scopes || "tomselect";
        const page = s.page || "";
        const actionEdit = s.onItemAddAction ?? "edit_item";
        const actionDelete = s.onItemRemoveAction ?? "delete_item";

        return {
            option: (data, escape) => {
                switch (s.case_Option) {

                    case "solicitante":
                        return `
            <div class="py-2 d-flex align-items-center">
                <div><span class="h6">${escape(data.tex_value)}</span></div>
                <div class="ms-auto">

                    <!-- EDITAR -->
                    <a href="javascript:void(0)"
                       class="dropdown-item_ font-weight-light active_show_lista_document_actos"
                       data-scope="${scope}"
                       data-action="${actionEdit}"
                       data-page="${page}"
                       data-id="${escape(data.id_value)}"
                       data-escript="${escape(s.id_escript ?? "")}">
                       <i class="fad fa-edit"></i>
                    </a>

                    <!-- ELIMINAR -->
                    <a href="javascript:void(0)"
                       class="dropdown-item_ font-weight-light active_show_lista_document_actos"
                       data-scope="${scope}"
                       data-action="${actionDelete}"
                       data-page="${page}"
                       data-id="${escape(data.id_value)}"
                       data-escript="${escape(s.id_escript ?? "")}">
                       <i class="fas fa-trash-alt"></i>
                    </a>

                </div>
            </div>`;

                    case "destinatario":
                        return `
            <div class="py-2 d-flex align-items-center">
                <span class="h6">${escape(data.tex_value)} ${escape(data.text_value_descritipo ?? "")}</span>
            </div>`;

                    default:
                        return `
            <div class="py-2 d-flex align-items-center">
                <span class="h6">${escape(data.tex_value || "Elemento sin tipo específico")}</span>
                <div class="ms-auto">
                    <a href="javascript:void(0)" 
                       class="dropdown-item_ ps-2 font-weight-light active_show_lista_document_actos"
                       data-scope="${scope}"
                       data-action="view_item"
                       data-page="${page}"
                       data-id="${escape(data.id_value ?? "")}">
                       <i class="fas fa-eye"></i>
                    </a>
                </div>
            </div>`;
                }
            },

            item: (data, escape) => {
                switch (s.case_Item) {

                    case "solicitante":
                        return `
            <div class="active-tom-item pd-3 d-flex align-items-center">
                <span>${escape(data.tex_value)}</span>
                <div class="ms-auto">

                    <!-- EDITAR -->
                    <a href="javascript:void(0)"
                       class="dropdown-item_ ps-2 font-weight-light active_show_lista_document_actos"
                       data-scope="${scope}"
                       data-action="${actionEdit}"
                       data-page="${page}"
                       data-id="${escape(data.id_value)}"
                       data-escript="${escape(s.id_escript ?? "")}">
                       <i class="fad fa-edit"></i>
                    </a>

                    <!-- ELIMINAR -->
                    <a href="javascript:void(0)"
                       class="dropdown-item_ ps-2 font-weight-light active_show_lista_document_actos"
                       data-scope="${scope}"
                       data-action="${actionDelete}"
                       data-page="${page}"
                       data-id="${escape(data.id_value)}"
                       data-escript="${escape(s.id_escript ?? "")}">
                       <i class="fas fa-trash-alt"></i>
                    </a>

                </div>
            </div>`;

                    case "destinatario":
                        return `
            <div class="active-tom-item pd-3 d-flex align-items-center">
                <span>${escape(data.tex_value)} ${escape(data.text_value_descritipo ?? "")}</span>
            </div>`;

                    default:
                        return `
            <div class="active-tom-item pd-3 d-flex align-items-center">
                <span>${escape(data.tex_value || "Elemento sin tipo específico")}</span>
                <div class="ms-auto">
                    <a href="javascript:void(0)" 
                       class="dropdown-item_ ps-2 font-weight-light active_show_lista_document_actos"
                       data-scope="${scope}"
                       data-action="view_item"
                       data-page="${page}"
                       data-id="${escape(data.id_value ?? "")}">
                       <i class="fas fa-eye"></i>
                    </a>
                </div>
            </div>`;
                }
            },

            option_create: (data, escape) =>
                `<div class="create">Agregue <strong>${escape(data.input)}</strong>&hellip;</div>`,

            no_results: () => `<div class="no-results">No se encontraron resultados</div>`,
            loading: () => `<div class="spinner"></div>`,
            not_loading: () => "",
            optgroup: data => `<div class="optgroup">${data.options}</div>`,
            dropdown: () => `<div></div>`
        };
    }

    // =========================================================
    // Métodos públicos de control
    // =========================================================
    async reload() {
        if (this.ts) await this.ts.load("");
    }

    setOptions(options) {
        if (this.ts && Array.isArray(options)) {
            this.ts.clearOptions();
            this.ts.addOptions(options);
            this.ts.refreshOptions(false);
        }
    }

    getValue() {
        if (!this.ts) return null;
        return this.ts.getValue();
    }

    clearAllTokens() {
        if (!this.ts) return;
        this.ts.clear(true);
    }

    clearTokens(values) {
        if (!this.ts) return;
        if (!values) return;
        const items = Array.isArray(values) ? values : [values];
        items.forEach(v => this.removeToken(v));
    }

    removeToken(value) {
        if (!this.ts) return;
        const currentValues = this.ts.items || [];
        if (currentValues.includes(value)) {
            this.ts.removeItem(value, true);
        }
    }

    // =========================================================
    // Dependencias (TomSelectGroup)
    // =========================================================
    addDependency(childManager) {
        this.dependentSelects.push(childManager);
    }

    async onValueChange(value) {
        for (const dep of this.dependentSelects) {
            dep.ts.clearOptions();
            dep.ts.disable();
            const data = await dep._loadFromApi(value);
            dep.setOptions(data);
            dep.ts.enable();
        }
    }
    // =========================================================
    // 🔥 Reset total del TomSelect
    //    Borra selección + opciones del dropdown
    // =========================================================
    reset() {
        if (!this.ts) return;

        // Limpia los valores seleccionados
        this.ts.clear(true);

        // Limpia TODAS las opciones
        this.ts.clearOptions();

        // Refresca la UI
        this.ts.refreshOptions(false);
    }

    // =========================================================
    // 🔄 Reset + recarga desde API
    // =========================================================
    async clearAndReload() {
        if (!this.ts) return;

        this.ts.clear(true);
        this.ts.clearOptions();

        // Recarga llamando al load("") que dispara el API
        await this.ts.load("");
    }
    // =========================================================
    // Activar modo "solo seleccionar" (estilo <option>)
    // =========================================================
    // ==============================
    // enableSelectOnlyMode mejorada
    // ==============================
    enableSelectOnlyMode() {
        if (!this.ts) return;

        // 1️⃣ Desactivar escritura sin ocultar input
        //this.ts.lock();

        // 2️⃣ Abrir cuando el usuario haga clic
        this.ts.settings.openOnFocus = true;
        this.ts.settings.shouldOpen = "always";

        // 3️⃣ Asegurar que al hacer clic abra la lista
        if (!this._clickToOpen) {
            this._clickToOpen = () => {
                this.ts.focus();
                this.ts.open();
            };
            this.ts.wrapper.addEventListener("click", this._clickToOpen);
        }

        // 4️⃣ Estilo visual
        this.ts.wrapper.classList.add("ts-no-typing");
    }




    // ==============================
    // restoreNormalMode (complemento de limpieza)
    // ==============================
    restoreNormalMode() {
        if (!this.ts) return;

        // 1️⃣ Rehabilitar entrada normal
        this.ts.unlock();

        // 2️⃣ Restaurar comportamiento normal
        this.ts.settings.openOnFocus = false;
        this.ts.settings.shouldOpen = null;

        // 3️⃣ Quitar el listener de abrir con clic
        if (this._clickToOpen) {
            this.ts.wrapper.removeEventListener("click", this._clickToOpen);
            this._clickToOpen = null;
        }

        // 4️⃣ Restaurar visual
        this.ts.wrapper.classList.remove("ts-no-typing");
    }

    initializeExistingTomSelects(root = document) {
        // Buscar todos los inputs tipo TomSelect dentro del root
        const inputs = root.querySelectorAll('input[data-action][data-scope="tomselect"]');

        inputs.forEach(input => {
            // 🔹 Ignorar si ya fue inicializado dinámicamente
            if (input.dataset.tsInitialized === "true") return;

            // Crear instancia de TomSelectManager para este input
            const tsManager = new TomSelectManager(`#${input.id}`, {
                ...this.tomSelectSettings,
                page: this.NameModulo,
                scopes: "tomselect",
                onItemAdd: (val, item) => {
                    const action = input.dataset.action;
                    if (action) {
                        // Ejecuta la acción registrada en ActionsManager
                        this.App?.execute(action, { id: val, element: input });
                    } else if (this.tomSelectSettings.onItemAdd) {
                        this.tomSelectSettings.onItemAdd(val, item);
                    }
                },
                onItemRemove: (val) => {
                    const action = input.dataset.action;
                    if (action) {
                        this.App?.execute(action, { id: val, element: input });
                    } else if (this.tomSelectSettings.onItemRemove) {
                        this.tomSelectSettings.onItemRemove(val);
                    }
                },
                onType: (query) => {
                    const action = input.dataset.action;
                    if (action) {
                        this.App?.execute(action, { value: query, element: input });
                    } else if (this.tomSelectSettings.onType) {
                        this.tomSelectSettings.onType(query);
                    }
                },
                onChange: () => {
                    // Reemitimos el change para ActionsManager
                    const evt = new Event("change", { bubbles: true });
                    input.dispatchEvent(evt);
                },
                onClear: () => {
                    const evt = new Event("change", { bubbles: true });
                    input.dispatchEvent(evt);
                }
            });

            tsManager.init();

            // 🔹 Marcamos como inicializado para no tocarlo de nuevo
            input.dataset.tsInitialized = "true";

            // Guardamos la instancia si quieres manejarla después
            this.tomSelectInstances[input.id] = tsManager;
        });
    }



    // =========================================================
    // Destructor
    // =========================================================
    destroy() {
        if (this.ts) {
            try {
                // asegurar limpieza
                if (this._buttonSelector && this._buttonClickHandler) {
                    const btn = document.querySelector(this._buttonSelector);
                    if (btn) btn.removeEventListener("click", this._buttonClickHandler);
                }
            } catch (e) { /* ignore */ }

            this.ts.destroy();
            this.ts = null;
        }
    }
}
