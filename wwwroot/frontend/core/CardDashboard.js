class CardDashboard {
    /**
     * Constructor principal
     * @param {string} contenedorId - ID del contenedor donde se insertarán las cards
     */
    constructor(contenedorId) {
        this.contenedor = document.getElementById(contenedorId);
        if (!this.contenedor) {
            throw new Error(`❌ Contenedor no encontrado: ${contenedorId}`);
        }

        this.modulos = [];
        this.valores = [];
        this.onCardClick = null;

        // Delegación de eventos
        this.contenedor.addEventListener("click", (e) => this._handleCardClick(e));
    }

    /**
     * Carga los módulos y valores en el dashboard
     * @param {Array} modulos
     * @param {Array} valores
     */
    cargar(modulos, valores) {
        // 🔒 Blindaje de entrada
        this.modulos = Array.isArray(modulos) ? modulos : [];
        this.valores = Array.isArray(valores) ? valores : [];
        this._render();
    }

    /**
     * Renderiza todas las cards filtradas por AcesoDirecto = 1
     * @private
     */
    _render() {
        this.contenedor.innerHTML = "";

        if (!Array.isArray(this.modulos) || this.modulos.length === 0) {
            return;
        }

        // 🔹 Filtro robusto (soporta 1, "1", true)
        const modulosFiltrados = this.modulos.filter(
            m => m && Number(m.AcesoDirecto) === 1
        );

        if (modulosFiltrados.length === 0) {
            console.warn("⚠️ No hay módulos con acceso directo");
            return;
        }

        // 🔹 Ordenar si existe el campo Orden
        const listaOrdenada = [...modulosFiltrados].sort(
            (a, b) => (Number(a.Orden) || 0) - (Number(b.Orden) || 0)
        );

        const fragment = document.createDocumentFragment();

        listaOrdenada.forEach((modulo) => {
            const {
                IdMenuPrincipal,
                NombreModulo,
                ValueNode,
                ToltipNode,
                Icono,
                EtiquetaCard
            } = modulo;

            if (!ValueNode) return; // protección extra

            const datoValor = this.valores.find(
                v => v && v.ValueNode === ValueNode
            );

            const valor = datoValor?.Valor ?? 0;
            const valorFormateado = Number(valor).toLocaleString("es-CO");
            const NombreCapital = this._toCapitalCase(NombreModulo);

            const div = document.createElement("div");
            div.className = "col-lg-12 col-md-6 col-xl-3";
            div.setAttribute("Value-Node", ValueNode);
            div.setAttribute("IdMenu-Principal", IdMenuPrincipal ?? "");
            div.setAttribute("title", ToltipNode ?? "");

            div.innerHTML = `
                <div class="card card-dashboard shadow-lg border-0 gradient-card card-click"
                     data-node="${ValueNode}">
                    <div class="card-body d-flex align-items-center justify-content-between">
                        <div>
                            <h6 class="card-title text-uppercase_ fw-semibold text-muted mb-2">
                                ${NombreCapital}
                            </h6>
                            <p class="mb-0 text-muted small">${EtiquetaCard || ""}</p>
                            <h5 class="fw-bold mb-0">${valorFormateado}</h5>
                        </div>
                        <div class="icon-circle bg-primary-subtle text-primary">
                            <i class="${Icono || ""} fa-2x"></i>
                        </div>
                    </div>
                </div>
            `;

            fragment.appendChild(div);
        });

        this.contenedor.appendChild(fragment);
    }

    /**
     * Maneja el clic en una card (delegación)
     * @private
     */
    _handleCardClick(e) {
        const card = e.target.closest(".card-click");
        if (!card || !this.contenedor.contains(card)) return;

        if (typeof this.onCardClick !== "function") return;

        const valueNode = card.dataset.node;
        if (!valueNode) return;

        const modulo = this.modulos.find(
            m => m && m.ValueNode === valueNode
        );

        if (modulo) {
            this.onCardClick(modulo, card);
        }
    }

    /**
     * Capitaliza texto
     * @private
     */
    _toCapitalCase(texto) {
        if (!texto || typeof texto !== "string") return "";

        return texto
            .toLowerCase()
            .split(" ")
            .filter(p => p.trim() !== "")
            .map(p => p.charAt(0).toUpperCase() + p.slice(1))
            .join(" ");
    }
}


