class CardDashboard {
    /**
     * Constructor principal
     * @param {string} contenedorId - ID del contenedor donde se insertarán las cards
     */
    constructor(contenedorId) {
        this.contenedor = document.getElementById(contenedorId);
        if (!this.contenedor)
            throw new Error(`❌ Contenedor no encontrado: ${contenedorId}`);

        this.modulos = [];
        this.valores = [];
        this.onCardClick = null; // Propiedad para asignar el evento externamente

        // Delegación de eventos (un solo listener para todos los clics)
        this.contenedor.addEventListener("click", (e) => this._handleCardClick(e));
    }

    /**
     * Carga los módulos y valores en el dashboard
     * @param {Array} modulos - Lista de módulos (con sus etiquetas)
     * @param {Array} valores - Lista de valores [{ValueNode, Valor}]
     */
    cargar(modulos, valores) {
        this.modulos = modulos;
        this.valores = valores;
        this._render();
    }

    /**
     * Renderiza todas las cards filtradas por AcesoDirecto = 1
     * @private
     */
    _render() {
        this.contenedor.innerHTML = "";

        // 🔹 Filtrar módulos que tengan acceso directo
        const modulosFiltrados = this.modulos.filter(
            (m) => m.AcesoDirecto === 1
        );

        // 🔹 Ordenar si existe el campo Orden
        const listaOrdenada = [...modulosFiltrados].sort(
            (a, b) => (a.Orden || 0) - (b.Orden || 0)
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

            const datoValor = this.valores.find((v) => v.ValueNode === ValueNode);
            const valor = datoValor ? datoValor.Valor : "";
            const valorFormateado = valor.toLocaleString("es-CO");
            const NombreCapital = this._toCapitalCase(NombreModulo);
            const div = document.createElement("div");
            div.className = "col-lg-12 col-md-6 col-xl-3";
            div.setAttribute("Value-Node", ValueNode);
            div.setAttribute("IdMenu-Principal", IdMenuPrincipal);
            div.setAttribute("title", ToltipNode);

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
                            <i class="${Icono} fa-2x"></i>
                        </div>
                    </div>
                </div>
            `;

            fragment.appendChild(div);
        });

        this.contenedor.appendChild(fragment);
    }

    /**
     * Maneja el clic en una card (delegación de eventos)
     * @private
     */
    _handleCardClick(e) {
        const card = e.target.closest(".card-click");
        if (card && this.contenedor.contains(card) && typeof this.onCardClick === "function") {
            const valueNode = card.dataset.node;
            const modulo = this.modulos.find((m) => m.ValueNode === valueNode);
            if (modulo) this.onCardClick(modulo, card);
        }
    }
    _toCapitalCase(texto) {
        if (!texto) return "";

        return texto
            .toLowerCase()
            .split(' ')
            .filter(palabra => palabra.trim() !== '')
            .map(palabra => palabra.charAt(0).toUpperCase() + palabra.slice(1))
            .join(' ');
    }
}
