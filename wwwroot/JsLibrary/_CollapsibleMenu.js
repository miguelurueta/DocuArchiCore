
   class CollapsibleMenu {
        constructor(selector, options = {}) {
        this.menu = document.querySelector(selector);
    if (!this.menu) throw new Error(`No se encontró el elemento del menú: ${selector}`);

    this.activeClass = options.activeClass || "active";
    this.iconChevronClass = options.iconChevronClass || "fa-solid fa-chevron-down ms-auto";
    this.debug = options.debug || false;
    this.submenusStateKey = options.submenusStateKey || "submenuState"; // Clave para el estado en localStorage
    this.initEvents();
    this.restoreSubMenuState();
    }

    // 🔸 Inicializar eventos principales
    initEvents() {
        this.menu.addEventListener("show.bs.collapse", (e) => {
            this.menu.querySelectorAll(".collapse").forEach(sub => {
                if (sub !== e.target) {
                    const bsCollapse = bootstrap.Collapse.getInstance(sub);
                    if (bsCollapse && sub.classList.contains("show")) {
                        bsCollapse.hide();
                    }
                }
            });
        });

        this.menu.addEventListener("click", (e) => {
            const link = e.target.closest("a.nav-link");
    if (!link) return;

    this.menu.querySelectorAll(`a.nav-link.${this.activeClass}`).forEach(l => l.classList.remove(this.activeClass));
    link.classList.add(this.activeClass);

    if (this.debug) console.log("Clic en:", link.textContent.trim());
        });

        // Persistir estado de submenús
        this.menu.addEventListener("shown.bs.collapse", (e) => {
        this.updateSubMenuState(e.target.id, true);
        });
        
        this.menu.addEventListener("hidden.bs.collapse", (e) => {
        this.updateSubMenuState(e.target.id, false);
        });
    }

    // 🔸 Actualizar el estado de un submenú en localStorage
    updateSubMenuState(submenuId, isOpen) {
        const state = JSON.parse(localStorage.getItem(this.submenusStateKey)) || { };
    state[submenuId] = isOpen;
    localStorage.setItem(this.submenusStateKey, JSON.stringify(state));
    }

    // 🔸 Recuperar el estado de los submenús desde localStorage
    restoreSubMenuState() {
        const state = JSON.parse(localStorage.getItem(this.submenusStateKey)) || { };
    for (const [submenuId, isOpen] of Object.entries(state)) {
            const submenu = document.getElementById(submenuId);
    if (submenu) {
                const bsCollapse = new bootstrap.Collapse(submenu, {toggle: false });
    if (isOpen) bsCollapse.show();
            }
        }
    }

    // 🔸 Agregar un nuevo item al menú
    addMenuItem(label, iconClass, href = "#") {
        const li = document.createElement("li");
    li.className = "nav-item";
    li.innerHTML = `
    <a class="nav-link d-flex align-items-center px-3" href="${href}">
        <i class="${iconClass} me-2"></i> ${label}
    </a>
    <button class="btn btn-sm btn-danger remove-item-btn">Eliminar</button> <!-- Botón de eliminación -->
    `;
    this.menu.appendChild(li);

        // Agregar evento de eliminación
        li.querySelector(".remove-item-btn").addEventListener("click", (e) => {
        e.stopPropagation(); // Evitar el clic en el item
    this.removeItem(label);
        });
    }

    // 🔸 Agregar un submenú a un item existente
    addSubMenu(parentLabel, submenuId, subItems = []) {
        const parentItem = [...this.menu.querySelectorAll(".nav-link")].find(l => l.textContent.trim() === parentLabel);
    if (!parentItem) return console.error("❌ No se encontró el item padre:", parentLabel);

    let collapseDiv = parentItem.nextElementSibling;
    if (!collapseDiv || !collapseDiv.classList.contains("collapse")) {
        collapseDiv = document.createElement("div");
    collapseDiv.className = "collapse";
    collapseDiv.id = submenuId;

    parentItem.setAttribute("data-bs-toggle", "collapse");
    parentItem.setAttribute("href", `#${submenuId}`);
    parentItem.setAttribute("aria-expanded", "false");
    parentItem.setAttribute("aria-controls", submenuId);

    if (!parentItem.querySelector("i.ms-auto")) {
                const iconChevron = document.createElement("i");
    iconChevron.className = this.iconChevronClass;
    parentItem.appendChild(iconChevron);
            }

    const ul = document.createElement("ul");
    ul.className = "btn-toggle-nav list-unstyled fw-normal pb-1 small";
    collapseDiv.appendChild(ul);
    parentItem.parentNode.appendChild(collapseDiv);
        }

    const submenuUl = collapseDiv.querySelector("ul");
        subItems.forEach(item => {
            const li = document.createElement("li");
    li.innerHTML = `<a href="${item.href || '#'}" class="nav-link px-4">${item.label}</a>`;
    submenuUl.appendChild(li);
        });
    }

    // 🔸 Limpiar todo el menú
    clearMenu() {
        this.menu.innerHTML = "";
    }

    // 🔸 Obtener referencia a item por texto
    getItemByLabel(label) {
        return [...this.menu.querySelectorAll(".nav-link")].find(l => l.textContent.trim() === label);
    }

    // 🔸 Activar manualmente un item
    setActive(label) {
        this.menu.querySelectorAll(`a.nav-link.${this.activeClass}`).forEach(l => l.classList.remove(this.activeClass));
    const item = this.getItemByLabel(label);
    if (item) item.classList.add(this.activeClass);
    }

    // 🔸 Eliminar un item por texto
    removeItem(label) {
        const item = this.getItemByLabel(label);
    if (item) {
        item.closest("li").remove();
    if (this.debug) console.log("Item eliminado:", label);
        } else {
        console.warn("No se encontró el item:", label);
        }
    }

    // 🔸 Buscar un item por texto y resaltarlo
    searchItem(label) {
        const item = this.getItemByLabel(label);
    if (item) {
        item.classList.add("bg-warning");
    this.setActive(label);
    if (this.debug) console.log("Item encontrado y resaltado:", label);
        } else {
        console.warn("No se encontró el item:", label);
        }
    }

    // 🔸 Reordenar un item arrastrándolo
    enableDragAndDrop() {
        const items = this.menu.querySelectorAll(".nav-item");
        items.forEach(item => {
        item.setAttribute("draggable", "true");
    item.addEventListener("dragstart", this.dragStart);
    item.addEventListener("dragover", this.dragOver);
    item.addEventListener("drop", this.dropItem);
    item.addEventListener("dragend", this.dragEnd);
        });
    }

    // 🔸 Funciones de drag and drop
    dragStart(e) {
        e.dataTransfer.setData("text/plain", e.target.id);
    e.target.classList.add("dragging");
    }

    dragOver(e) {
        e.preventDefault();
    e.target.classList.add("drag-over");
    }

    dropItem(e) {
        e.preventDefault();
    const draggedId = e.dataTransfer.getData("text/plain");
    const draggedElement = document.getElementById(draggedId);
    const dropTarget = e.target.closest(".nav-item");

    if (draggedElement !== dropTarget) {
        this.menu.insertBefore(draggedElement, dropTarget.nextSibling);
        }

    draggedElement.classList.remove("dragging");
    e.target.classList.remove("drag-over");
    }

    dragEnd(e) {
        e.target.classList.remove("dragging");
    }

    // 🔸 Desplegar todos los submenús
    expandAllSubMenus() {
        const allCollapseElements = this.menu.querySelectorAll('.collapse');
        allCollapseElements.forEach((collapse) => {
            const bsCollapse = new bootstrap.Collapse(collapse, {toggle: false });
    if (!collapse.classList.contains("show")) {
        bsCollapse.show();
            }
        });
    }

    // 🔸 Mostrar tooltip en los items
    addTooltipToItems() {
        const items = this.menu.querySelectorAll('.nav-link');
        items.forEach(item => {
        item.setAttribute('title', item.textContent.trim());
    new bootstrap.Tooltip(item);
        });
    }
}

