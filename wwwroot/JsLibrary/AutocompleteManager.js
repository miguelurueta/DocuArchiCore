class AutocompleteManager {

    constructor(input, options = {}) {
        this.input = input;
        this.apiClient = options.apiClient;
        this.serviceName = options.serviceName;
        this.apiMethod = options.apiMethod;
        this.defaultDbAlias = options.defaultDbAlias;
        this.tbl_control = options.tbl_control;
        this.extraParams = options.extraParams || {};
        this.onSelect = options.onSelect || (() => { });
        this.minChars = options.minChars || 2;
        this.debounceTime = options.debounce || 300;
        this.name_campo = options.name_campo || "";

        this.resultsBox = null;
        this.spinner = null;
        this.timeout = null;

        // ⚡ Nuevo: delay para evitar parpadeos
        this.spinnerDelay = 250;
        this.spinnerTimeout = null;

        this.#injectStyles();
        this.#wrapInput();
        this.#createResultsBox();
        this.#bindEvents();
    }

    #wrapInput() {
        const wrapper = document.createElement("div");
        wrapper.className = "ac-wrapper";
        wrapper.style.position = "relative";

        this.input.parentNode.insertBefore(wrapper, this.input);
        wrapper.appendChild(this.input);

        this.wrapper = wrapper;
    }

    #createResultsBox() {
        this.resultsBox = document.createElement("div");
        this.resultsBox.className = "ac-results";
        this.resultsBox.style.display = "none";

        // Spinner dentro de resultados
        this.spinner = document.createElement("div");
        this.spinner.className = "ac-spinner";
        this.spinner.style.display = "none";
        this.spinner.innerHTML = `<div class="lds-dual-ring"></div>`;

        this.resultsBox.appendChild(this.spinner);
        this.wrapper.appendChild(this.resultsBox);
    }

    #bindEvents() {
        this.input.addEventListener("input", (e) => {
            const value = e.target.value.trim();
            clearTimeout(this.timeout);

            if (value.length < this.minChars) {
                this.resultsBox.style.display = "none";
                return;
            }

            this.timeout = setTimeout(() => {
                this.search(value);
            }, this.debounceTime);
        });

        this.input.addEventListener("keydown", (e) => this.#handleKeyboard(e));

        document.addEventListener("click", (e) => {
            if (!this.wrapper.contains(e.target)) {
                this.resultsBox.style.display = "none";
            }
        });

        this.input.addEventListener("blur", () => {
            setTimeout(() => this.resultsBox.style.display = "none", 150);
        });
    }

    /** ==========================================================
     *  CONSULTA API + SPINNER
     * ==========================================================*/
    async search(text) {
        try {
            this.#delayedSpinner(); // 🔥 nuevo

            const payload = {
                TextoBuscado: text,
                ...this.extraParams,
                defaultDbAlias: this.defaultDbAlias,
                tbl_control: this.tbl_control,
                name_campo: this.name_campo
            };

            const response = await this.apiClient
                .use(this.serviceName)
                .call(this.apiMethod, payload);

            this.#hideSpinner();

            if (!response || response.error) {
                this.#renderResults([]);
                return;
            }

            this.#renderResults(response.data || []);

        } catch (err) {
            console.error("Autocomplete error:", err);
            this.#hideSpinner();
            this.#renderResults([]);
        }
    }

    #renderResults(list) {
        [...this.resultsBox.querySelectorAll(".ac-item")].forEach(el => el.remove());

        if (!list.length) {
            this.resultsBox.style.display = "none";
            return;
        }

        list.forEach(item => {
            const div = document.createElement("div");
            div.className = "ac-item";
            div.textContent = item.tex_value || item.label || item.nombre || "";
            div.addEventListener("click", () => this.select(item));
            this.resultsBox.appendChild(div);
        });

        this.resultsBox.style.display = "block";
    }

    select(item) {
        this.input.value = item.tex_value || item.label || "";
        this.onSelect(item);
        this.resultsBox.style.display = "none";
    }

    #handleKeyboard(e) {
        const items = [...this.resultsBox.querySelectorAll(".ac-item")];
        if (!items.length) return;

        let idx = items.findIndex(i => i.classList.contains("active"));

        switch (e.key) {
            case "ArrowDown":
                idx = (idx + 1) % items.length;
                break;
            case "ArrowUp":
                idx = (idx - 1 + items.length) % items.length;
                break;
            case "Enter":
                if (idx >= 0) items[idx].click();
                return;
            default:
                return;
        }

        items.forEach(i => i.classList.remove("active"));
        items[idx].classList.add("active");
        items[idx].scrollIntoView({ block: "nearest" });

        e.preventDefault();
    }

    /** ==========================================================
     *  SPINNER CON DELAY (evita parpadeo)
     * ==========================================================*/
    #delayedSpinner() {
        clearTimeout(this.spinnerTimeout);

        // Prepara el contenedor
        this.resultsBox.style.display = "block";

        // Muestra spinner solo si tarda más de 250 ms
        this.spinnerTimeout = setTimeout(() => {
            this.spinner.style.display = "flex";
        }, this.spinnerDelay);
    }

    #hideSpinner() {
        clearTimeout(this.spinnerTimeout);
        this.spinner.style.display = "none";
    }

    /** ==========================================================
     *  ESTILOS AUTO-INYECTADOS
     * ==========================================================*/
    #injectStyles() {
        if (document.getElementById("ac-style-injected")) return;

        const css = `
            .ac-wrapper { position: relative; }
            .ac-results {
                position: absolute;
                top: 100%;
                left: 0;
                right: 0;
                background: #fff;
                border: 1px solid #ccc;
                border-radius: 4px;
                max-height: 240px;
                overflow-y: auto;
                z-index: 99999;
                box-shadow: 0 2px 6px rgba(0,0,0,0.15);
            }
            .ac-item {
                padding: 7px 10px;
                cursor: pointer;
                white-space: nowrap;
            }
            .ac-item:hover,
            .ac-item.active {
                background: #eef2ff;
            }
            .ac-spinner {
                width: 100%;
                display: none;
                justify-content: center;
                padding: 12px 0;
            }
            .lds-dual-ring {
                display: inline-block;
                width: 24px;
                height: 24px;
            }
            .lds-dual-ring:after {
                content: " ";
                display: block;
                width: 24px;
                height: 24px;
                border-radius: 50%;
                border: 3px solid #3f51b5;
                border-color: #3f51b5 transparent #3f51b5 transparent;
                animation: lds-dual-ring 1.2s linear infinite;
            }
            @keyframes lds-dual-ring {
                0% { transform: rotate(0deg); }
                100% { transform: rotate(360deg); }
            }
        `;

        const style = document.createElement("style");
        style.id = "ac-style-injected";
        style.innerHTML = css;
        document.head.appendChild(style);
    }
}
