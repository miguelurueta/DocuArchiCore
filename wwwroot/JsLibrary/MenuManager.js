class MenuManager {
    constructor(ulRoot, options = {}) {
        if (!ulRoot) throw new Error("Se requiere un contenedor válido para MenuManager");

        this.ulRoot = ulRoot;
        this.framework = options.framework || "bootstrap";
        this.debug = options.debug || false;
        this.activeClass = options.activeClass || "active";
        this._clickCallback = options.onItemClick || null;
        this._parentClickCallback = options.onParentClick || null;

        this.storageKey = 'dynamicMenu.activeId';

        // Implementación del campo de búsqueda
        this.searchInput = options.searchInputSelector ? document.querySelector(options.searchInputSelector) : null;
        if (this.searchInput) {
            this.searchInput.addEventListener('input', (evt) => this.searchItems(evt.target.value));
        }

        this.initEvents();
        this.restoreActiveFromStorage();
    }

    initEvents() {
        // Sub-items
        const childLinks = this.ulRoot.querySelectorAll('a.child-link');
        childLinks.forEach(link => {
            link.addEventListener('click', (evt) => {
                if (link.getAttribute('href') === '#') evt.preventDefault();

                this.setActiveLink(link);

                if (this._clickCallback) this._clickCallback(link);
            });
        });

        // Parent links
        const parentLinks = this.ulRoot.querySelectorAll('a.parent-link');
        parentLinks.forEach(plink => {
            plink.addEventListener('click', (evt) => {
                const collapseId = plink.getAttribute('aria-controls');
                const collapseEl = this.ulRoot.querySelector('#' + collapseId);

                setTimeout(() => this.updateParentActiveStateFromChildren(collapseEl), 40);

                if (this._parentClickCallback) this._parentClickCallback(plink);
            });
        });
    }

    setActiveLink(link) {
        // Remover cualquier active previo
        this.ulRoot.querySelectorAll(`a.nav-link.${this.activeClass}`).forEach(a => a.classList.remove(this.activeClass));
        link.classList.add(this.activeClass);

        // Guardar en sessionStorage
        const id = link.dataset.id;
        if (id) sessionStorage.setItem(this.storageKey, id);

        // Marcar padres
        this.clearParentActiveMarks();
        this.markParents(link);
    }

    markParents(childLink) {
        let current = childLink.closest('ul');
        while (current && current !== this.ulRoot) {
            const collapseDiv = current.closest('.collapse');
            if (!collapseDiv) break;

            const parentA = this.ulRoot.querySelector(`a.parent-link[aria-controls="${collapseDiv.id}"]`);
            if (parentA) parentA.classList.add('parent-active');

            current = collapseDiv.closest('ul');
        }
    }

    clearParentActiveMarks() {
        this.ulRoot.querySelectorAll('a.parent-link.parent-active').forEach(a => a.classList.remove('parent-active'));
    }

    updateParentActiveStateFromChildren(collapseEl) {
        if (!collapseEl) return;
        const hasActiveChild = !!collapseEl.querySelector(`a.nav-link.${this.activeClass}`);
        const parentLink = this.ulRoot.querySelector(`a.parent-link[aria-controls="${collapseEl.id}"]`);
        if (parentLink) {
            if (hasActiveChild) parentLink.classList.add('parent-active');
            else parentLink.classList.remove('parent-active');
        }

        const parentLi = parentLink ? parentLink.closest('li.nav-item') : null;
        if (parentLi) {
            const upperCollapse = parentLi.closest('.collapse');
            if (upperCollapse) this.updateParentActiveStateFromChildren(upperCollapse);
        }
    }

    restoreActiveFromStorage() {
        const storedId = sessionStorage.getItem(this.storageKey);
        if (!storedId) return;

        const link = this.ulRoot.querySelector(`a.nav-link[data-id="${storedId}"]`);
        if (link) {
            link.classList.add(this.activeClass);
            this.markParents(link);

            let current = link.closest('ul');
            while (current && current !== this.ulRoot) {
                const collapseDiv = current.closest('.collapse');
                if (!collapseDiv) break;

                const inst = this.framework === "mdb"
                    ? (mdb.Collapse.getInstance(collapseDiv) || new mdb.Collapse(collapseDiv, { toggle: false }))
                    : (bootstrap.Collapse.getInstance(collapseDiv) || new bootstrap.Collapse(collapseDiv, { toggle: false }));
                inst.show();

                current = collapseDiv.closest('ul');
            }
        }
    }

    // 🔹 Métodos públicos
    setActiveById(id) {
        const link = this.ulRoot.querySelector(`a.nav-link[data-id="${id}"]`);
        if (link) this.setActiveLink(link);
    }

    onItemClick(callback) {
        if (typeof callback !== "function") throw new Error("El callback debe ser una función");
        this._clickCallback = callback;
    }

    onParentClick(callback) {
        if (typeof callback !== "function") throw new Error("El callback debe ser una función");
        this._parentClickCallback = callback;
    }

    getActiveItem() {
        return this.ulRoot.querySelector(`a.nav-link.${this.activeClass}`);
    }

    addItem(parentId, newItem) {
        // Puedes implementar la lógica para agregar dinámicamente items
        console.warn("addItem: pendiente implementar actualización dinámica");
    }

    removeItem(itemId) {
        const link = this.ulRoot.querySelector(`a[data-id="${itemId}"]`);
        if (link) {
            const li = link.closest('li.nav-item');
            li?.remove();
        }
    }

    // 🔍 Implementación de búsqueda
    searchItems(query) {
        const links = this.ulRoot.querySelectorAll('a.nav-link');
        links.forEach(link => {
            const text = link.textContent.toLowerCase();
            const isVisible = text.includes(query.toLowerCase());
            link.style.display = isVisible ? 'block' : 'none';
        });
    }

    // 🔲 Seleccionar/Deseleccionar un item por ID
    toggleItemSelection(id) {
        const link = this.ulRoot.querySelector(`a.nav-link[data-id="${id}"]`);
        if (link) {
            if (link.classList.contains(this.activeClass)) {
                link.classList.remove(this.activeClass);  // Desmarcar
            } else {
                link.classList.add(this.activeClass);     // Marcar
            }
        }
    }
    // 📜 Obtener el item seleccionado
    getSelectedItem() {
        const selectedItem = this.ulRoot.querySelector(`a.nav-link.${this.activeClass}`);
        return selectedItem ? selectedItem : null;  // Retorna el item seleccionado o null si no hay ninguno
    }
}
