// ======================================================
// TableManager - Versión AUTO-ADAPTABLE COMPLETA 2025
// ======================================================
class TableManager {

    constructor(options = {}) {

        if (!options.containerId) {
            throw new Error("TableManager: containerId requerido");
        }

        this.container = document.getElementById(options.containerId);
        if (!this.container) {
            throw new Error(`TableManager: No existe contenedor #${options.containerId}`);
        }

        // Configuración general ------------------------------
        this.apiClient = options.apiClient;
        this.serviceName = options.serviceName;
        this.apiMethod = options.apiMethod;

        this.columns = options.columns || [];
        this.actions = options.actions || [];

        this.pageSize = options.pageSize || 10;
        this.scopeName = options.scopeName || "tabla";
        this.idField = options.idField || "id";
        this.filters = options.initialFilters || {};

        this.fullData = [];
        this.currentPage = 1;

        this.tableId = `${options.containerId}_table`;
        this.paginationId = `${this.tableId}_pagination`;

        // Autodetección HTML
        this._detectZones();

        // Render base
        this._renderBase();

        // Fix para dropdown sobrepuesto
       
    }

    // =====================================================
    // Autodetección
    // =====================================================
    _detectZones() {

        this.tableArea =
            this.container.querySelector(".tm-body-scroll") ||
            this.container.querySelector(".tm-table-area") ||
            this.container;

        this.paginationArea =
            this.container.querySelector(".tm-pagination-area") ||
            document.querySelector(".tm-pagination-area") ||
            null;

        if (!this.paginationArea) {
            this.paginationArea = document.createElement("div");
            this.paginationArea.classList.add("tm-pagination-area");
            this.container.insertAdjacentElement("afterend", this.paginationArea);
        }
    }

    // =====================================================
    // Render base (DIBUJA LOS ENCABEZADOS)
    // =====================================================
    _renderBase() {

        const headers = [
            ...this.columns.map(c => `<th>${c.title}</th>`),
            `<th class="text-center">Opciones</th>`
        ].join("");

        this.tableArea.innerHTML = `
            <div class="table-responsive">
                <table id="${this.tableId}" class="table table-hover mb-0 modern-table">
                    <thead class="sticky-header bg-light">
                        <tr>${headers}</tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
        `;

        this.tbody = this.tableArea.querySelector("tbody");
    }

    // =====================================================
    // Carga única desde el servidor
    // =====================================================
    async load() {
        try {
            let result;

            if (typeof this.apiClient.use === "function") {
                result = await this.apiClient.use(this.serviceName).call(this.apiMethod, this.filters);
            } else if (typeof this.apiClient.invoke === "function") {
                result = await this.apiClient.invoke(this.serviceName, this.apiMethod, this.filters);
            } else {
                throw new Error("apiClient inválido");
            }

            const payload = result?.Data ?? result?.data ?? [];
            const success = result?.Success ?? result?.success ?? !result?.Error;

            if (!success) {
                this._renderEmpty("Error cargando datos");
                return;
            }

            this.fullData = Array.isArray(payload) ? payload : [];
            this.totalRecords = this.fullData.length;

            this.renderPage(1);

        } catch (e) {
            console.error("TableManager.load error:", e);
            this._renderEmpty("Error de conexión");
        }
    }

    // =====================================================
    // Paginación local
    // =====================================================
    renderPage(page) {

        this.currentPage = page;

        const start = (page - 1) * this.pageSize;
        const pageData = this.fullData.slice(start, start + this.pageSize);

        this._renderRows(pageData);
        this._renderPagination();
    }

    reload() { this.renderPage(this.currentPage); }

    setFilters(filters = {}) {
        this.filters = filters;
        return this.load();
    }

    // =====================================================
    // Render de filas
    // =====================================================
    _renderRows(rows) {

        if (!rows || rows.length === 0) {
            this._renderEmpty("Sin registros");
            return;
        }

        let html = "";

        for (const row of rows) {

            html += "<tr>";

            for (const col of this.columns) {

                const value = row[col.field] ?? "";

                if (col.type === "custom" && typeof col.render === "function") {
                    html += `<td>${col.render(row)}</td>`;
                    continue;
                }

                if (col.format === "date" && value) {
                    html += `<td>${dayjs(value).format(col.formatMask || "DD/MM/YYYY")}</td>`;
                    continue;
                }

                html += `<td>${value}</td>`;
            }

            html += `<td class="text-center">${this._renderActions(row)}</td>`;
            html += "</tr>";
        }

        this.tbody.innerHTML = html;
        //this._bindActionElements();
    }

    // =====================================================
    // Render de acciones
    // =====================================================
    _renderActions(row) {

        if (!this.actions || this.actions.length === 0) return "";

        const id = row[this.idField];

        let html = `<div class="tm-actions d-flex gap-2 justify-content-center">`;

        for (const act of this.actions) {

            const am = `${this.scopeName}.${act.am}`;

            // ICON
            if (act.type === "icon") {
                html += `
                    <i class="fa ${act.icon} text-${act.color || "primary"} tm-pointer"
                       title="${act.tooltip || act.am}"
                       data-bs-toggle="tooltip"
                       data-id="${id}"
                       data-am-click="${am}">
                    </i>`;
                continue;
            }

            // BUTTON
            if (act.type === "button") {
                html += `
                    <button class="btn btn-sm btn-${act.color || "primary"}"
                            data-id="${id}"
                            data-am-click="${am}">
                        ${act.text}
                    </button>`;
                continue;
            }

            // CHECKBOX
            if (act.type === "checkbox") {
                const checked = row[act.field] ? "checked" : "";
                html += `
                    <input type="checkbox" class="form-check-input"
                           ${checked}
                           data-id="${id}"
                           data-am-click="${am}">
                `;
                continue;
            }

            // DROPDOWN
           // DROPDOWN
if (act.type === "dropdown") {
    if (Array.isArray(act.items) && act.items.length > 0) {  // Verificar que act.items sea un array y tenga elementos
        html += `
            <div class="dropdown tm-dropdown-fix">
                <button class="btn btn-sm btn-${act.color || "secondary"} dropdown-toggle"
                        data-bs-toggle="dropdown"
                        data-bs-display="static">
                    <i class="fa ${act.icon || "fa-bars"}"></i>  <!-- Icono de FontAwesome -->
                </button>

                <ul class="dropdown-menu shadow-lg tm-dropdown-menu">
                    ${act.items.map(i => `
                        <li>
                            <a class="dropdown-item"
                               data-id="${id}"
                               data-am-click="${this.scopeName}.${i.am}">
                                ${i.label}
                            </a>
                        </li>
                    `).join("")}
                </ul>
            </div>
        `;
    } else {
        console.warn("Dropdown items is empty or not an array:", act.items);
    }
}

        }

        html += `</div>`;
        return html;
    }

    // =====================================================
    // Paginación
    // =====================================================
    _renderPagination() {

        const totalPages = Math.ceil(this.totalRecords / this.pageSize);

        if (totalPages <= 1) {
            this.paginationArea.innerHTML = "";
            return;
        }

        let html = `<ul class="pagination justify-content-center">`;

        html += `
            <li class="page-item ${this.currentPage === 1 ? "disabled" : ""}">
                <a class="page-link" href="#" data-page="${this.currentPage - 1}">Anterior</a>
            </li>`;

        for (let p = 1; p <= totalPages; p++) {
            html += `
                <li class="page-item ${p === this.currentPage ? "active" : ""}">
                    <a class="page-link" href="#" data-page="${p}">${p}</a>
                </li>`;
        }

        html += `
            <li class="page-item ${this.currentPage === totalPages ? "disabled" : ""}">
                <a class="page-link" href="#" data-page="${this.currentPage + 1}">Siguiente</a>
            </li>`;

        html += `</ul>`;

        this.paginationArea.innerHTML = html;

        this.paginationArea.querySelectorAll("a[data-page]").forEach(a => {
            a.addEventListener("click", e => {
                e.preventDefault();

                const page = parseInt(a.dataset.page);
                if (page >= 1 && page <= totalPages) {
                    this.renderPage(page);
                }
            });
        });
    }

    // =====================================================
    // Sin registros
    // =====================================================
    _renderEmpty(msg) {
        this.tbody.innerHTML = `
            <tr>
                <td colspan="${this.columns.length + 1}" class="text-center text-muted py-4">${msg}</td>
            </tr>
        `;
        this.paginationArea.innerHTML = "";
    }

    // =====================================================
    // Eventos AM
    // =====================================================
    _bindActionElements() {

        const nodes = this.container.querySelectorAll("[data-am-click]");

        nodes.forEach(node => {

            if (node.__tm_bound__) return;
            node.__tm_bound__ = true;

            node.addEventListener("click", e => {
                e.preventDefault();

                const action = node.getAttribute("data-am-click");
                const id = node.getAttribute("data-id");

                this.container.dispatchEvent(new CustomEvent("am:action", {
                    bubbles: true,
                    detail: { action, id, scope: this.scopeName }
                }));
            });
        });
    }
}
