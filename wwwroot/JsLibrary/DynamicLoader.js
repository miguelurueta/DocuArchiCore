// ==========================================
// Clase: DynamicLoader
// Descripción: Carga vistas dinámicas (MVC o ASPX) en Dashboard
// ==========================================
const recursosPorModulo = {
    Radicacion: {
        css: [
        ],
        js: [ "/wwwroot/Radicacion/Radicacion.js"
        ],
        init: "inicializarRadicacion" // función global que se ejecuta después de cargar
    },
    Recepcion: {
        css: ["/wwwroot/Recepcion/Recepcion.css"],
        js: ["/wwwroot/Recepcion/Recepcion.js"],
        init: "inicializarRecepcion"
    }
};

class DynamicLoader {
    constructor(options) {
        this.sidebarSelector = options.sidebarSelector || "#sidebar a[data-url]";
        this.mainContent = document.querySelector(options.mainContent || "#mainContent");
        this.legacyFrame = document.querySelector(options.legacyFrameSelector || "#legacyFrame");
        this.contenedorCards = document.querySelector(options.contenedorCards || "#contenedorCards");
        this.ContenMain = document.querySelector(options.ContenMain || "#ContenMain");
        this.lblNameModulo = document.querySelector(options.lblNameModulo || "#lblNameModulo");
        this.NameModulo = "Radicacion";
        this.NameDefault ="dashboard"
        this.parametros;
        // 🧭 Guardar última navegación
        this.lastNavigation = null;
        this.timeout=0;
        this.init();
        this.swith = 0;
    }

    init() {
        const links = document.querySelectorAll(this.sidebarSelector);
        links.forEach(link => {
            link.addEventListener("click", e => this.handleLinkClick(e, link));
        });

        window.addEventListener("popstate", e => this.handlePopState(e));
    }

    async handleLinkClick(e, link) {
       
       
        e.preventDefault();
        const url = link.dataset.url;
        const type = link.dataset.type || (url.endsWith(".aspx") ? "aspx" : "mvc");

        // Guardar última navegación
        this.lastNavigation = { url, type };

        this.clearView();

        if (type === "aspx") {
            this.loadIframe(url);
            return;
        }

        await this.loadMvcPage(url);
    }
    async getAppRoot() {
        const path = window.location.pathname;
        const parts = path.split('/');
        if (parts.length > 2) {
            const appSegment = parts[1].toLowerCase();
            if (appSegment !== "views" && appSegment !== "home") {
                return "/" + parts[1] + "/";
            }
        }
        return "/";
    }
   
   async resolveAppUrl(relativePath) {
        // Si la ruta ya es absoluta (empieza con / o http), no se toca
        if (/^(\/|https?:)/i.test(relativePath)) {
            return relativePath;
        }

       const base = await this.getAppRoot();
        const baseUrl = new URL(base, window.location.origin);
        const resolved = new URL(relativePath, baseUrl);
        return resolved.pathname;
    }
   async loadMvcPage(url) {
       try {
           if (this.swith === 1) { return true };
           this.showLoading();
           
            url = await this.resolveAppUrl(url);
            const resp = await fetch(url, { method: "GET", cache: "no-store" });
            if (!resp.ok) throw new Error(`Error ${resp.status}: al cargar la página`);

            const html = await resp.text();
            this.mainContent.innerHTML = html;
            history.pushState({ url, type: "mvc" }, "", url);
           let ConteRecurso = await this.cargarRecursos(this.NameModulo, this.parametros);
           if (ConteRecurso != "YES") {
               this.showError(ConteRecurso);
           }
            this.mainContent.style.display = "block";
            //mostrarAlertaBootstrap("✅ Página cargada correctamente", "success", 2000);
            this.hideLoading();

        } catch (err) {
            this.showError(err.message);
        }
    }
   async cargarRecursos(modulo, parametros) {
    try {
    const recursos = recursosPorModulo[modulo];
    if (!recursos) return;
    // Cargar CSS
    recursos.css.forEach((href) => {
        const id = `css-${href}`;
        if (!document.getElementById(id)) {
            const link = document.createElement("link");
            link.id = id;
            link.rel = "stylesheet";
            link.href = href;
            document.head.appendChild(link);
        }
    });

    // Cargar JS secuencialmente
    const cargarScripts = async () => {
        for (const src of recursos.js) {
            const id = `js-${src}`;
            if (!document.getElementById(id)) {
                await new Promise((resolve, reject) => {
                    const script = document.createElement("script");
                    script.id = id;
                    script.src = src;
                    script.onload = resolve;
                    script.onerror = reject;
                    document.body.appendChild(script);
                });
            }
        }

        // Ejecutar función de inicialización si existe
        if (recursos.init && typeof window[recursos.init] === "function") {
            window[recursos.init](parametros);
        }
    };
        await cargarScripts();
        return "YES"
    } catch (err) {
        return err.mensaje;
        //this.showError(err.message);
    }
}

async loadIframe(url) {
       try {
          
           this.showLoading(); // Mostrar el mensaje de carga
            url = await this.resolveAppUrl(url);
            this.lastNavigation = { url, type: "aspx" }; // Guardar referencia
            this.legacyFrame.style.display = "none";
            this.legacyFrame.src = "about:blank";
            this.legacyFrame.setAttribute("scrolling", "no");
            this.legacyFrame.style.overflow = "hidden";
            let loaded = false;
            // Timeout para manejar si el iframe no carga en el tiempo estipulado
            this.timeout = setTimeout(() => {
                if (!loaded) {
                    this.showError(`⏱️ La página tardó demasiado en responder (${url})`);
                    this.legacyFrame.src = "about:blank";
                }
            }, 35000); // 35 segundos para cargar el iframe
            // Usar addEventListener para escuchar el evento 'load' correctamente
           this.legacyFrame.addEventListener('load', () => {
                 if (this.swith === 2) {
                    return true;
                 }
                loaded = true;
                clearTimeout(this.timeout); // Limpiar el timeout
                //console.log("Iframe cargado correctamente");
                // Aquí, se maneja el cambio de visibilidad y el mensaje
                try {
                    const frameDoc = this.legacyFrame.contentDocument || this.legacyFrame.contentWindow.document;
                    if (!frameDoc || !frameDoc.body || frameDoc.body.innerHTML.trim() === "") {
                        throw new Error("El contenido del iframe está vacío.");
                    }

                    // Limpiar y ocultar el contenido anterior
                    this.mainContent.innerHTML = "";
                    if (this.ContenMain) this.ContenMain.innerHTML = "";

                    // Mostrar el iframe cargado con una transición suave
                    this.legacyFrame.style.display = "block";
                    this.legacyFrame.style.opacity = "0";
                    this.legacyFrame.style.transition = "opacity 0.5s ease";
                    setTimeout(() => {
                        this.legacyFrame.style.opacity = "1";
                    }, 50);

                    try {
                        frameDoc.body.style.overflow = "hidden";
                        frameDoc.documentElement.style.overflow = "hidden";
                    } catch (e) {
                        console.log("No se pudo ajustar el contenido del iframe:", e);
                    }

                    this.hideLoading(); // Ocultar mensaje de carga
                } catch (err) {
                    this.showError(`❌ No se pudo cargar correctamente la página (${url}).<br><small>${err.message}</small>`); //aqui 
                }
            });

            this.legacyFrame.onerror = () => {
                clearTimeout(timeout); // Limpiar timeout en caso de error
                this.showError(`❌ Error al cargar el contenido desde: <br><small>${url}</small>`);
                this.legacyFrame.style.display = "none";
            };

            // Establecer la URL y cambiar el estado en el historial
            this.legacyFrame.src = url;
            history.pushState({ url, type: "aspx" }, "", url);
            clearTimeout(this.timeout); // Limpiar el timeout
        } catch (err) {
            console.log("⚠️ Error en loadIframe:", err);
            this.showError(`❌ Error al intentar cargar la página: ${err.message}`);
        }  
    }



    handlePopState(e) {
        if (!e.state) return;
        this.lastNavigation = e.state;

        if (e.state.type === "aspx") {
            this.loadIframe(e.state.url);
        } else {
            this.loadMvcPage(e.state.url);
        }
    }

    showLoading() {
        this.ContenMain.innerHTML = `
    <div id="overlayCargando"
         class="position-fixed top-0 start-0 w-100 h-100 d-flex flex-column justify-content-center align-items-center"
         style="z-index: 2000; background-color: rgba(0, 0, 0, 0.05); opacity: 0; transition: opacity 0.6s ease;">
        <div class="text-center p-4 bg-white rounded-4 shadow-lg"
             style="min-width: 270px; animation: fadeInBox 0.5s ease; border: 1px solid rgba(0,0,0,0.1);">
            <div class="spinner-border text-primary mb-3" style="width: 2.8rem; height: 2.8rem;" role="status"></div>
            <h6 class="fw-semibold text-dark mb-1">Cargando contenido...</h6>
            <p class="text-muted mb-0 small">Por favor, espera un momento.</p>
        </div>
    </div>
    <style>
        @keyframes fadeInBox {
            from { transform: scale(0.97); opacity: 0; }
            to { transform: scale(1); opacity: 1; }
        }
    </style>`;
        requestAnimationFrame(() => {
            const overlay = document.getElementById("overlayCargando");
            if (overlay) overlay.style.opacity = "1";
        });
    }

    hideLoading() {
        const overlay = document.getElementById("overlayCargando");
        if (overlay) {
            overlay.style.opacity = "0";
            overlay.style.transition = "opacity 0.4s ease";
            setTimeout(() => overlay.remove(), 400);
        }
    }

    // 🔴 Nueva versión con botón "Reintentar"
    showError(message) {
        const lastNav = this.lastNavigation;

        this.ContenMain.innerHTML = `
    <div id="overlayError"
         class="position-fixed top-0 start-0 w-100 h-100 d-flex flex-column justify-content-center align-items-center"
         style="z-index: 2000; background-color: rgba(0,0,0,0.05); opacity: 0; transition: opacity 0.6s ease;">
        <div class="text-center p-4 bg-white rounded-4 shadow-lg border border-danger-subtle"
             style="min-width: 280px; animation: fadeInError 0.5s ease;">
            <div class="text-primary mb-2" style="font-size: 2rem;">❌</div>
            <h5 class="fw-bold text-dark mb-2">Ocurrió un error</h5>
            <p class="text-muted mb-3">${message}</p>
            <div class="d-flex justify-content-center gap-2">
                <button class="btn btn-outline-danger btn-sm px-4" id="btnCerrarError">Cerrar</button>
                <button class="btn btn-primary btn-sm px-4" id="btnReintentar">Reintentar</button>
            </div>
        </div>
    </div>
    <style>
        @keyframes fadeInError {
            from { transform: scale(0.95); opacity: 0; }
            to { transform: scale(1); opacity: 1; }
        }
    </style>`;

        requestAnimationFrame(() => {
            const overlay = document.getElementById("overlayError");
            if (overlay) overlay.style.opacity = "1";
        });

        // Botones
        document.getElementById("btnCerrarError")?.addEventListener("click", () => {
            document.getElementById("overlayError")?.remove();
        });

        document.getElementById("btnReintentar")?.addEventListener("click", async () => {
            document.getElementById("overlayError")?.remove();
            if (!lastNav) return;

            this.showLoading();
            if (lastNav.type === "aspx") {
                await this.loadIframe(lastNav.url);
            } else {
                await this.loadMvcPage(lastNav.url);
            }
        });
    }

    clearView() {
        clearTimeout(this.timeout);
        this.mainContent.innerHTML = "";
        this.legacyFrame.style.display = "none";
        this.legacyFrame.src = "about:blank";
        this.contenedorCards.style.display = "none";
        this.ContenMain.innerHTML = "";
    }

    CardView() {
        clearTimeout(this.timeout);
        // Solo mostrar el contenedor de tarjetas, sin interferir con el resto del contenido
        this.mainContent.style.display = "none";  // Hacer invisible el área principal
        this.legacyFrame.style.display = "none";  // Ocultar el iframe
        this.legacyFrame.src = "about:blank";  // Limpiar el contenido del iframe
        this.contenedorCards.style.display = "flex";  // Mostrar las tarjetas
        this.ContenMain.innerHTML = "";  // Limpiar el contenido adicional
        this.lblNameModulo.textContent = "";  // Limpiar el título del módulo
    }
}

// ============================
// Notificación Bootstrap
// ============================
function mostrarAlertaBootstrap(mensaje, tipo = "info", duracion = 3000) {
    const contenedor = document.getElementById("globalAlertContainer");
    if (!contenedor) return;

    const alerta = document.createElement("div");
    alerta.className = `alert alert-${tipo} alert-dismissible fade show shadow`;
    alerta.role = "alert";
    alerta.innerHTML = `
        ${mensaje}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;

    contenedor.appendChild(alerta);

    setTimeout(() => {
        const bsAlert = bootstrap.Alert.getOrCreateInstance(alerta);
        bsAlert.close();
    }, duracion);
}
