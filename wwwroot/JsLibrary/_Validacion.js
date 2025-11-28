class Validacion {
    constructor(formNotifier = new FormNotifier(), config = {}) {
        if (!formNotifier) throw new Error("Se requiere FormNotifier");
        this.notifier = formNotifier;

        this.config = Object.assign({
            tooltipPosition: "right"
        }, config);

        this.validators = {
            fecha: this.validarFecha.bind(this),
            numero: this.validarNumero.bind(this),
            email: this.validarEmail.bind(this),
            required: this.validarRequerido.bind(this),
        };

        this._bindEvents(document);
    }

    /* -------------------- ENLACE AUTOMÁTICO -------------------- */
    _bindEvents(root = document) {
        const inputs = root.querySelectorAll("[data-validate], [data-required='true'], .custom-select");
        inputs.forEach(input => {
            if (input.dataset.__validacionBound === "1") return;
            input.dataset.__validacionBound = "1";

            input.addEventListener("focus", (e) => this.#clearError(e.currentTarget));

            if (input.dataset.validate === "fecha") {
                input.addEventListener("keypress", (e) => {
                    const char = String.fromCharCode(e.which);
                    if (!/[0-9-]/.test(char)) e.preventDefault();
                });
                input.addEventListener("input", (e) => this.#formatearFecha(e.target));
                input.addEventListener("blur", (e) => this.#completarFecha(e.target));
            }

            if (input.dataset.validate === "numero") {
                input.addEventListener("input", (e) => {
                    e.target.value = e.target.value.replace(/\D+/g, "");
                });
                input.addEventListener("keypress", (e) => {
                    const char = String.fromCharCode(e.which);
                    if (!/[0-9]/.test(char)) e.preventDefault();
                });
            }

            if (input.classList.contains("custom-select")) {
                input.addEventListener("click", () => this.#clearError(input));
                input.addEventListener("customselect:change", () => this.#clearError(input));
            }
        });
    }

    refreshBindings(root = document) {
        this._bindEvents(root);
    }

    /* -------------------- VALIDADORES -------------------- */
    validarInput(control) {
        if (!control) return "YES";

        if (control.classList.contains("custom-select")) {
            if (control.dataset.required === "true")
                return this.validarCustomSelect(control);
            return "YES";
        }

        const tipo = control.dataset.validate || (control.dataset.required === "true" ? "required" : null);

        if (control.dataset.required === "true" && tipo && tipo !== "required") {
            const req = this.validarRequerido(control);
            if (req !== "YES") return req;
        }

        if (!tipo) return "YES";
        if (this.validators[tipo]) return this.validators[tipo](control);
        return "YES";
    }

    validarFecha(control) {
        const value = (control.value || "").trim();

        if (value === "") {
            if (control.dataset.required === "true")
                return this.#setError(control, "Este campo es obligatorio.");
            return this.#clearError(control), "YES";
        }

        const regex = /^\d{4}-\d{2}-\d{2}$/;
        if (!regex.test(value)) return this.#setError(control, "Formato inválido (YYYY-MM-DD).");

        const [year, month, day] = value.split("-").map(Number);
        const actual = new Date().getFullYear();

        if (year > actual) return this.#setError(control, "El año no es válido.");
        if (month < 1 || month > 12) return this.#setError(control, "El mes no es válido.");
        if (day < 1 || day > 31) return this.#setError(control, "El día no es válido.");

        const fecha = new Date(year, month - 1, day);
        if (fecha.getFullYear() !== year || fecha.getMonth() + 1 !== month || fecha.getDate() !== day)
            return this.#setError(control, "La fecha no es válida.");

        this.#clearError(control);
        return "YES";
    }

    validarNumero(control) {
        const value = (control.value || "").trim();

        if (value === "") {
            if (control.dataset.required === "true")
                return this.#setError(control, "Este campo es obligatorio.");
            return this.#clearError(control), "YES";
        }

        if (/\D/.test(value))
            return this.#setError(control, "Solo se permiten números.");
        return this.#clearError(control), "YES";
    }

    validarEmail(control) {
        const value = (control.value || "").trim();

        if (value === "") {
            if (control.dataset.required === "true")
                return this.#setError(control, "Este campo es obligatorio.");
            return this.#clearError(control), "YES";
        }

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(value))
            return this.#setError(control, "Email no válido");
        return this.#clearError(control), "YES";
    }

    validarRequerido(control) {
        const value = (control.value || "").trim();
        if (value === "")
            return this.#setError(control, "Este campo es obligatorio.");
        return this.#clearError(control), "YES";
    }

    validarCustomSelect(control) {
        const selected = control.querySelector(".selected span");
        const value = selected ? selected.textContent.trim() : "";
        if (value === "" || value.toLowerCase().includes("seleccione"))
            return this.#setError(control, "Seleccione una opción.");
        this.#clearError(control);
        return "YES";
    }

    /* -------------------- FORMATEADOR DE FECHA -------------------- */
    #formatearFecha(control) {
        let val = control.value.replace(/\D/g, "");
        if (val.length > 8) val = val.substring(0, 8);

        if (val.length > 4 && val.length <= 6)
            val = val.replace(/^(\d{4})(\d{1,2})/, "$1-$2");
        else if (val.length > 6)
            val = val.replace(/^(\d{4})(\d{2})(\d{1,2})/, "$1-$2-$3");

        control.value = val;
    }

    #completarFecha(control) {
        const val = control.value.trim();
        const regex = /^(\d{4})-(\d{1,2})-(\d{1,2})$/;

        if (regex.test(val)) {
            let [, y, m, d] = val.match(regex);
            m = m.padStart(2, "0");
            d = d.padStart(2, "0");
            control.value = `${y}-${m}-${d}`;
        }
    }

    /* -------------------- TOOLTIP DE ERROR -------------------- */
    #setError(control, message) {
        control.classList.add("input-warning");
        const existing = document.querySelector(`.tooltip-error[data-for="${control.id}"]`);
        if (existing) existing.remove();

        const tooltip = document.createElement("div");
        tooltip.className = "tooltip-error";
        tooltip.dataset.for = control.id;
        tooltip.textContent = message;

        control.parentElement.style.position = "relative";
        control.parentElement.appendChild(tooltip);

        const pos = this.config.tooltipPosition || "right";
        const offset = 8;
        const finalPos = control.classList.contains("custom-select") ? "bottom" : pos;

        switch (finalPos) {
            case "bottom":
                tooltip.style.top = `${control.offsetTop + control.offsetHeight + offset}px`;
                tooltip.style.left = `${control.offsetLeft}px`;
                tooltip.style.zIndex = "1";
                break;
            case "left":
                tooltip.style.top = `${control.offsetTop + (control.offsetHeight / 2) - 12}px`;
                tooltip.style.left = `${control.offsetLeft - tooltip.offsetWidth - offset}px`;
                break;
            case "top":
                tooltip.style.top = `${control.offsetTop - control.offsetHeight}px`;
                tooltip.style.left = `${control.offsetLeft}px`;
                break;
            default:
                tooltip.style.top = `${control.offsetTop + (control.offsetHeight / 2) - 12}px`;
                tooltip.style.left = `${control.offsetLeft + control.offsetWidth + offset}px`;
                break;
        }

        if (this.notifier?.showError)
            this.notifier.showError(control.id, message);

        return "NO";
    }

    #clearError(control) {
        if (!control) return;
        control.classList.remove("input-warning");
        const tooltip = document.querySelector(`.tooltip-error[data-for="${control.id}"]`);
        if (tooltip) tooltip.remove();
        if (this.notifier?.clearError)
            this.notifier.clearError(control.id);
    }

    /* -------------------- VALIDAR FORMULARIO -------------------- */
    validarFormulario(root = document, options = {}) {
        let rootEl = root;
        if (typeof root === "string") rootEl = document.getElementById(root);
        if (!rootEl) return false;

        const oldConfig = { ...this.config };
        this.config = Object.assign({}, this.config, options);

        const inputs = rootEl.querySelectorAll("[data-validate], [data-required='true'], .custom-select[data-required='true']");
        let allValid = true;

        inputs.forEach(input => {
            if (this.validarInput(input) !== "YES") allValid = false;
        });

        this.config = oldConfig;
        return allValid;
    }

    setOptions(newOptions) {
        this.options = { ...this.options, ...newOptions };
    }
    /* -------------------- ERRORES DEL BACKEND -------------------- */
    applyBackendErrors(errors = [], _tooltipPosition) {
        if (!Array.isArray(errors)) return;

        // 📌 Guardamos la posición original
        const originalPosition = this.config.tooltipPosition;

        // 📌 Forzamos posición para errores del backend (ej. "bottom") bottom
        this.config.tooltipPosition = _tooltipPosition;

        errors.forEach(err => {
            const field = err.Field || err.field;
            const message = err.Message || err.message;
            if (!field) return;

            let control = document.getElementById(field);
            if (!control) control = document.querySelector(`[name="${field}"]`);

            if (control) {
                this.#setError(control, message || "Error de validación");
            }
        });

        // 📌 Restauramos posición original
        this.config.tooltipPosition = originalPosition;
    }

    clearBackendErrors(root = document) {
        const inputs = root.querySelectorAll("[data-validate], [data-required='true'], .custom-select");
        inputs.forEach(input => this.#clearError(input));
    }

}
