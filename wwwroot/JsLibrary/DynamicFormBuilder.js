// =========================================================
// Clase: DynamicFormBuilder
// Descripción: Construye formularios dinámicos con soporte para TomSelectManager
// =========================================================
class DynamicFormBuilder {
    constructor(containerId, tomSelectSettings = {}) {
        this.container = containerId ? document.getElementById(containerId) : null;
        this.tomSelectSettings = tomSelectSettings;
        this.tomSelectInstances = {};
        this.ApiClient;
        this.configArray = [];
        this.NameModulo = "";
        this.controlBuilders = {
            text: this.createTextInput.bind(this),
            date: this.createDateInput.bind(this),
            select: this.createSelect.bind(this),
            textarea: this.createTextArea.bind(this),
            checkbox: this.createCheckbox.bind(this),
            tomselect: this.createTomSelect.bind(this),
            autocomplete: this.createAutocomplete.bind(this) // 🔹 NUEVO
        };
    }

    // Construcción completa desde JSON
    buildFromJson(fieldsJson) {
        if (!Array.isArray(fieldsJson)) throw new Error("El JSON debe ser un array de campos");
        if (this.container) this.container.innerHTML = "";

        fieldsJson.forEach(field => {
            const control = this.createControl(field);
            if (control && this.container) this.container.appendChild(control);
        });
    }

    // Determina tipo y crea control
    createControl(field) {
        const type = this.detectControlType(field);
        const builder = this.controlBuilders[type] || this.controlBuilders["text"];
        return builder(field);
    }

    // =========================================================
    // TIPOS DE CONTROL
    // =========================================================

    createTextInput(field) {
        const div = document.createElement("div");
        div.className = "col-md-6 input-contenedor";

        const label = document.createElement("label");
        label.className = "form-label";
        const requiredMark = field.obligatorio_campo === 1 ? ' <span class="text-danger">*</span>' : "";
        label.innerHTML = `${field.aleas_campo || field.name_campo}${requiredMark}`;
        div.appendChild(label);

        const input = document.createElement("input");
        input.type = "text";
        input.className = "form-control inp-text py-2";
        input.id = field.name_campo;
        input.placeholder = field.Place_Holder || "";
        input.disabled = field.disable_campo === 1;
        this.applyDataClearAttribute(input, field);
        div.appendChild(input);
        return div;
    }

    createDateInput(field) {
        const div = document.createElement("div");
        div.className = "col-md-6 input-contenedor";

        const label = document.createElement("label");
        label.className = "form-label";
        label.innerHTML = `${field.aleas_campo || field.name_campo}`;
        div.appendChild(label);

        const wrapper = document.createElement("div");
        wrapper.className = "d-flex align-items-center";

        const input = document.createElement("input");
        input.type = "date";
        input.className = "form-control me-2 inp-text";
        input.id = field.name_campo;
        input.disabled = field.disable_campo === 1;
        this.applyDataClearAttribute(input, field);
        const button = document.createElement("button");
        button.type = "button";
        button.className = "btn-forms";
        button.innerHTML = '<i class="fa-solid fa-calendar-days"></i>';
        button.addEventListener("click", () => input.showPicker && input.showPicker());

        wrapper.appendChild(input);
        wrapper.appendChild(button);
        div.appendChild(wrapper);
        return div;
    }
    createAutocomplete(field) {
        const div = document.createElement("div");
        div.className = "col-md-6 input-contenedor";

        // Label
        const label = document.createElement("label");
        label.className = "form-label";
        const requiredMark = field.obligatorio_campo === 1 ? ' <span class="text-danger">*</span>' : "";
        label.innerHTML = `${field.aleas_campo || field.name_campo}${requiredMark}`;
        div.appendChild(label);

        // Input
        const input = document.createElement("input");
        input.type = "text";
        input.className = "form-control inp-text py-2";
        input.id = field.name_campo;
        input.placeholder = field.Place_Holder || "";
        input.disabled = field.disable_campo === 1;
        this.applyDataClearAttribute(input, field);
        div.appendChild(input);

        // Inicializa AutocompleteManager
        if (this.ApiClient && field.serviceName && field.apiMethod) {
            new AutocompleteManager(input, {
                apiClient: this.ApiClient,
                serviceName: field.serviceName,
                apiMethod: field.apiMethod,
                extraParams: field.extraParams || {},
                debounce: field.debounce || 300,
                minChars: field.minChars || 2,
                defaultDbAlias: field.dbms_control || "",
                tbl_control: field.tbl_control || "",
                name_campo: field.name_campo|| "",
                onSelect: (item) => {
                    if (field.onChangeAction) {
                        this.App?.execute(field.onChangeAction, { value: item });
                    }
                }
            });
        }

        return div;
    }

    
    createSelect(field) {
        const div = document.createElement("div");
        div.className = "col-md-6 input-contenedor";

        const label = document.createElement("label");
        label.className = "form-label";
        label.innerHTML = `${field.aleas_campo || field.name_campo}`;
        div.appendChild(label);

        const select = document.createElement("select");
        select.className = "form-select inp-text";
        select.id = field.name_campo;
        select.disabled = field.disable_campo === 1;
        this.applyDataClearAttribute(select, field);
        // 👉 atributos para dependencias
        if (field.drow_name_controls_destino) {
            select.setAttribute("atrib_campo_drow_destino", field.drow_name_controls_destino);
        }
        if (field.atrib_name_espace_control) {
            select.setAttribute("atrib_name_espace_control", field.atrib_name_espace_control);
        }
        if (field.atrib_campo_n) {
            select.setAttribute("atrib_campo_n", field.atrib_campo_n);
        }

        // 👉 opcional: acción para ActionsManager
        if (field.onChangeAction) {
            select.dataset.scope = "selects";
            select.dataset.action = field.onChangeAction;
            select.dataset.page = this.NameModulo;
        }

        if (field.ilist_row_drowlist?.length) {
            field.ilist_row_drowlist.forEach(opt => {
                const option = document.createElement("option");
                option.value = opt.id_value;
                option.textContent = opt.value_campo;
                select.appendChild(option);
            });
        }

        div.appendChild(select);
        return div;
    }

    selectAddRow(row, select) {
        row.forEach(opt => {
            const option = document.createElement("option");
            option.value = opt.id_value;
            option.textContent = opt.value_campo;
            select.appendChild(option);
        });

    }
    createTextArea(field) {
        const div = document.createElement("div");
        div.className = "col-md-6 input-contenedor";

        const label = document.createElement("label");
        label.className = "form-label";
        label.innerHTML = `${field.aleas_campo || field.name_campo}`;
        div.appendChild(label);

        const textarea = document.createElement("textarea");
        textarea.className = "form-control inp-text py-2";
        textarea.id = field.name_campo;
        textarea.placeholder = field.Place_Holder || "";
        textarea.disabled = field.disable_campo === 1;
        this.applyDataClearAttribute(textarea, field);
        div.appendChild(textarea);
        return div;
    }

    createCheckbox(field) {
        const div = document.createElement("div");
        div.className = "col-md-6 input-contenedor";

        const wrapper = document.createElement("div");
        wrapper.className = "form-check";

        const input = document.createElement("input");
        input.type = "checkbox";
        input.className = "form-check-input";
        input.id = field.name_campo;
        input.checked = field.value_campo === "1";
        input.disabled = field.disable_campo === 1;
        this.applyDataClearAttribute(input, field);
        const label = document.createElement("label");
        label.className = "form-check-label";
        label.setAttribute("for", field.name_campo);
        label.textContent = field.aleas_campo || field.name_campo;

        wrapper.appendChild(input);
        wrapper.appendChild(label);
        div.appendChild(wrapper);
        return div;
    }

    // =========================================================
    // CONTROL ESPECIAL: TOMSELECT
    // =========================================================
    createTomSelect(field) {
        const div = document.createElement("div");
        div.className = "col-md-11 mb-3";

        const wrapper = document.createElement("div");
        wrapper.className = "d-flex align-items-center";

        const label = document.createElement("label");
        label.className = "form-label me-2 mb-0";
        label.textContent = `${field.aleas_campo || field.name_campo}:`;
        wrapper.appendChild(label);

        const input = document.createElement("input");
        input.type = "text";
        input.className = "form-control me-2 inp-text";
        input.id = field.name_campo;
        input.placeholder = field.Place_Holder || "Buscar...";
        input.disabled = field.disable_campo === 1;
        this.applyDataClearAttribute(input, field);
        if (field.onChangeAction) {
            input.dataset.scope = "tomselect";
            input.dataset.action = field.onChangeAction;
            input.dataset.page = this.NameModulo;
            input.dataset.event = "change";
            input.dataset.idescript = field.id_escript;
        }
        wrapper.appendChild(input);

        const button = document.createElement("button");
        button.type = "button";
        button.className = "btn-forms";
        button.innerHTML = '<i class="fa-solid fa-user-tie"></i>';
        button.addEventListener("click", () => input.focus());
        wrapper.appendChild(button);

        div.appendChild(wrapper);

        // Inicializa TomSelectManager
        setTimeout(() => {
            const tsManager = new TomSelectManager(`#${field.name_campo}`, {
                ...this.tomSelectSettings,
                case_Option: field.case_Option || "destinatario",
                case_Item: field.case_Item || "destinatario",
                onInitialize: this.tomSelectSettings.onInitialize,
                page: this.NameModulo,
                onItemAddAction: field.TomPParameterTomSelelect.onItemAddAction,
                onItemRemoveAction: field.TomPParameterTomSelelect.onItemRemoveAction,
                scopes:"tomselect",
                // 👇 Traducción de acciones enviadas desde backend
                onItemAdd: (val, item) => {
                    if (field.onItemAddAction) {
                        // Ejecuta acción registrada en ActionsManager
                        this.App?.execute(field.onItemAddAction, { id: val });
                    } else if (this.tomSelectSettings.onItemAdd) {
                        this.tomSelectSettings.onItemAdd(val, item);
                    }
                },

                onItemRemove: (val) => {
                    if (field.onItemRemoveAction) {
                        this.App?.execute(field.onItemRemoveAction, { id: val });
                    } else if (this.tomSelectSettings.onItemRemove) {
                        this.tomSelectSettings.onItemRemove(val);
                    }
                },

                onType: this.tomSelectSettings.onType,
                serviceName: field.serviceName,
                apiMethod: field.apiMethod,
                TomPParameterTomSelelect: field.TomPParameterTomSelelect,
                placeholder: field.placeholder,
                maxItems: field.Tom_alow
            });

            tsManager.init();
            this.tomSelectInstances[field.name_campo] = tsManager;
        }, 50);

       

        return div;
    }

   
  
    // =========================================================
    // APLICA dataClear="YES"
    // =========================================================
    applyDataClearAttribute(element, field) {
        if (field?.dataClear === "YES") {
            element.setAttribute("data-clear", "YES");
        }
    }

    // =========================================================
    //  🔥 NUEVO MÉTODO: LIMPIAR CAMPOS
    // =========================================================
    clearFields(rootSelector = null) {

        const root = rootSelector
            ? document.querySelector(rootSelector)
            : document;

        if (!root) return;

        const clearable = root.querySelectorAll("[data-clear='YES']");

        clearable.forEach(el => {

            // ---- INPUTS ----
            if (el.tagName === "INPUT") {
                if (["text", "number", "date"].includes(el.type)) {
                    el.value = "";
                }
                if (el.type === "checkbox") {
                    el.checked = false;
                }

                // TomSelect
                /*if (this.tomSelectInstances[el.id]) {
                    this.tomSelectInstances[el.id].clear();
                }*/
                if (el.tomselect) {
                    try {
                        el.tomselect.clear();
                        el.tomselect.setValue("");
                    } catch { }
                }

            }

            // ---- TEXTAREA ----
            else if (el.tagName === "TEXTAREA") {
                el.value = "";
            }

            // ---- SELECT ----
            else if (el.tagName === "SELECT") {
                el.value = "";
            }
        });

        console.log(`🧽 Limpieza realizada: ${clearable.length} campos.`);
    }
    // =========================================================
    // DETECCIÓN DE TIPO DE CAMPO
    // =========================================================
    detectControlType(field) {
        const type = field.ComportamientoCampo?.toUpperCase?.() || "";
        if (type === "DATE") return "date";
        if (type === "TEXTAREA") return "textarea";
        if (type === "BIT") return "checkbox";
        if (type === "TOMSELECT") return "tomselect";
        if (type === "AUTOCOMPLETE") return "autocomplete"; // 🔹 NUEVO
        if (field.ilist_row_drowlist?.length || type === "SELECCION") return "select";
        return "text";
    }

    getTomSelectManager(name_campo) {
        return this.tomSelectInstances[name_campo] || null;
    }
     // =========================================================
    // EVENTOS CONTROLES select
    // =========================================================
    // Busca configuración de un campo dependiente
    drowConfigService(name_campo) {
        for (let i = 0; i < this.configArray.length; i++) {
            if (this.configArray[i].name_campo === name_campo) {
                return this.configArray[i].config_service_drowlis_destino;
            }
        }
        return null;
    }

    // Limpia las opciones de un select dependiente
    drowDeleteRows(name_campo) {
        for (let i = 0; i < this.configArray.length; i++) {
            if (this.configArray[i].drow_name_padre_control === name_campo) {
                const name_control = this.configArray[i].name_campo;
                const el = document.getElementById(name_control);
                if (el) el.innerHTML = "";
            }
        }
    }
    drowDeleteAllRows(name_control) {
        const el = document.getElementById(name_control);
        if (el) el.innerHTML = "";
    }
}
