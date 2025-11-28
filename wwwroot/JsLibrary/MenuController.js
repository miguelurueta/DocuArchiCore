class MenuController {
    constructor(containerSelector, options = {}) {
        this.menu = document.querySelector(containerSelector);
        if (!this.menu) throw new Error(`No se encontró el menú: ${containerSelector}`);
        this.activeClass = options.activeClass || "active";
        this.debug = options.debug || false;
        this.stateKey = options.stateKey || "submenuState";
        this.initEvents();
    }

    // 🎯 Eventos básicos
    initEvents() {
        // Activar item
        this.menu.addEventListener("click", (e) => {
            const link = e.target.closest(".nav-link");
            if (!link) return;

            this.menu.querySelectorAll(`.nav-link.${this.activeClass}`).forEach(l => l.classList.remove(this.activeClass));
            link.classList.add(this.activeClass);

            if (this.debug) console.log(`🟢 Click en: ${link.textContent.trim()}`);
        });

        // Guardar estado de submenús
        this.menu.addEventListener("shown.bs.collapse", (e) => this.updateSubMenuState(e.target.id, true));
        this.menu.addEventListener("hidden.bs.collapse", (e) => this.updateSubMenuState(e.target.id, false));
    }

    // 🧾 Guardar/recuperar estado
    updateSubMenuState(id, isOpen) {
        const state = JSON.parse(localStorage.getItem(this.stateKey)) || {};
        state[id] = isOpen;
        localStorage.setItem(this.stateKey, JSON.stringify(state));
    }

    restoreState() {
        const state = JSON.parse(localStorage.getItem(this.stateKey)) || {};
        Object.entries(state).forEach(([id, isOpen]) => {
            const el = document.getElementById(id);
            if (el) {
                const bsCollapse = new bootstrap.Collapse(el, { toggle: false });
                if (isOpen) bsCollapse.show();
            }
        });
    }

    // ➕ Agregar item manualmente
    addItem(label, icon = "fa-solid fa-circle", href = "#") {
        const li = document.createElement("li");
        li.className = "nav-item";
        li.innerHTML = `
            <a class="nav-link d-flex align-items-center px-3" href="${href}">
                <i class="${icon} me-2"></i>${label}
            </a>
            <button class="btn btn-sm btn-danger remove-item-btn">🗑</button>
        `;
        this.menu.appendChild(li);

        // Eliminar
        li.querySelector(".remove-item-btn").addEventListener("click", (e) => {
            e.stopPropagation();
            li.remove();
            if (this.debug) console.log(`❌ Item eliminado: ${label}`);
        });
    }

    // 🔍 Buscar item
    search(label) {
        const items = [...this.menu.querySelectorAll(".nav-link")];
        const found = items.find(i => i.textContent.trim().toLowerCase() === label.toLowerCase());
        if (found) {
            this.menu.querySelectorAll(".bg-warning").forEach(i => i.classList.remove("bg-warning"));
            found.classList.add("bg-warning");
            found.scrollIntoView({ behavior: "smooth", block: "center" });
            if (this.debug) console.log(`✅ Encontrado: ${label}`);
        } else {
            console.warn(`No se encontró el item: ${label}`);
        }
    }

    // 🧹 Limpiar menú
    clear() {
        this.menu.innerHTML = "";
    }

    // 🐢 Carga bajo demanda (ejemplo simple)
    async loadOnDemand(callback) {
        const data = await callback(); // El callback retorna JSON dinámico
        console.log("📥 Datos cargados bajo demanda", data);
        // Podrías reutilizar DynamicMenu aquí para regenerar estructura
    }
}
