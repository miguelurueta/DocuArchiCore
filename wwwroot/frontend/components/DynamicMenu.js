// =====================
// DynamicMenu: versión final optimizada
// =====================
class DynamicMenu {
    constructor(menuSelector) {
        this.menuContainer = document.querySelector(menuSelector);
        if (!this.menuContainer) return;

        this.ulRoot = this.menuContainer.tagName === "UL"
            ? this.menuContainer
            : (() => {
                const ul = document.createElement("ul");
                ul.className = "nav flex-column mt-2 px-2";
                this.menuContainer.appendChild(ul);
                return ul;
            })();

        this.storageKey = 'dynamicMenu.activeId';
    }

    async buildMenu(menuData) {
        const normalized = this.normalizeData(menuData);
        this.ulRoot.innerHTML = await this.renderMenu(normalized, 0);
        this.initCollapses();
        this.attachNavHandlers();
        this.restoreActiveFromStorage();
    }

    async renderMenu(items, padreId) {
        const hijos = items.filter(m => m.IdPadre === padreId);
        if (!hijos.length) return "";

        let html = "";
        for (const item of hijos) {
            const subHijos = items.filter(x => x.IdPadre === item.IdMenuPrincipal);
            const hasChildren = subHijos.length > 0;
            const submenuId = `submenu-${item.IdMenuPrincipal}`;

            if (item.Tipo === "separador") {
                html += `<li class="nav-item mt-2"><hr></li>`;
                continue;
            }

            if (item.Tipo === "titulo") {
                html += `
                    <li class="nav-item px-1">
                        <small class="text-muted">${item.NombreModulo}</small>
                        <ul class="nav flex-column mt-2">
                            ${await this.renderMenu(items, item.IdMenuPrincipal)}
                        </ul>
                    </li>`;
                continue;
            }

            if (hasChildren) {
                html += `
                    <li class="nav-item">
                        <a class="nav-link d-flex align-items-center px-1 parent-link"
                           data-bs-toggle="collapse" href="#${submenuId}" 
                           role="button" aria-expanded="false" aria-controls="${submenuId}"
                           data-id="${item.IdMenuPrincipal}"
                           data-url="${item.UrlNode || ''}"
                           data-value="${item.ValueNode || ''}"
                           data-tooltip="${item.ToltipNode || ''}">
                            <i class="${item.Icono || 'fa-solid fa-folder-open'} me-2"></i>
                            <span class="link-text">${item.NombreModulo}</span>
                            <i class="fa-solid fa-chevron-down ms-auto"></i>
                        </a>
                        <div class="collapse" id="${submenuId}">
                            <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
                                ${await this.renderMenu(items, item.IdMenuPrincipal)}
                            </ul>
                        </div>
                    </li>`;
            } else {
                html += `
                    <li class="nav-item">
                        <a class="nav-link d-flex align-items-center px-1 child-link ${item.Active ? 'active' : ''}" 
                           href="${'#'}" 
                           data-id="${item.IdMenuPrincipal}"
                           data-value="${item.ValueNode || ''}"
                           data-tooltip="${item.ToltipNode || ''}">
                            ${item.Icono ? `<i class="${item.Icono} me-2"></i>` : ""}
                            <span class="link-text">${item.NombreModulo}</span>
                        </a>
                    </li>`;
            }
        }

        return html;
    }

    normalizeData(data) {
        if (!Array.isArray(data)) return [];
        if (data.length && data[0].Key !== undefined) {
            const obj = {};
            data.forEach(item => obj[item.Key] = item.Value);
            return [obj];
        }
        if (Array.isArray(data[0]) && data[0][0]?.Key !== undefined) {
            return data.map(itemArray => {
                const obj = {};
                itemArray.forEach(pair => obj[pair.Key] = pair.Value);
                return obj;
            });
        }
        return data;
    }

    initCollapses() {
        const collapseElements = this.ulRoot.querySelectorAll('.collapse');
        collapseElements.forEach(el => {
            if (!bootstrap.Collapse.getInstance(el)) {
                new bootstrap.Collapse(el, { toggle: false });
            }
        });
    }

    attachNavHandlers() {
        // Sub-items
        const childLinks = this.ulRoot.querySelectorAll('a.child-link');
        childLinks.forEach(link => {
            link.addEventListener('click', () => {
                this.setActiveLink(link);
            });
        });

        // Parent links
        const parentLinks = this.ulRoot.querySelectorAll('a.parent-link');
        parentLinks.forEach(plink => {
            plink.addEventListener('click', () => {
                const collapseId = plink.getAttribute('aria-controls');
                const collapseEl = this.ulRoot.querySelector('#' + collapseId);
                this.updateParentActiveStateFromChildren(collapseEl);
            });
        });
    }

    setActiveLink(link) {
        this.ulRoot.querySelectorAll('a.nav-link.active').forEach(a => a.classList.remove('active'));
        link.classList.add('active');

        const id = link.getAttribute('data-id');
        if (id) sessionStorage.setItem(this.storageKey, id);

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
        const hasActiveChild = !!collapseEl.querySelector('a.nav-link.active');
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
            link.classList.add('active');
            this.markParents(link);

            let current = link.closest('ul');
            while (current && current !== this.ulRoot) {
                const collapseDiv = current.closest('.collapse');
                if (!collapseDiv) break;
                const inst = bootstrap.Collapse.getInstance(collapseDiv);
                if (inst) inst.show();
                current = collapseDiv.closest('ul');
            }
        }
    }
}
