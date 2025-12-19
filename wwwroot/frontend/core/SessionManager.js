class SessionManager {
    constructor(options = {}) {
        this.api = new ApiClientService();

        // Endpoints
        this.serviceKey = options.serviceKey || "Home";
        this.keepAliveEndpoint = options.keepAliveEndpoint || "KeepAlive";
        this.tiempoRestanteEndpoint = options.tiempoRestanteEndpoint || "TiempoRestante";
        this.ValidateEndpoint = options.ValidateEndpoint || "ServiceIsSessionTimedOut";
        this.ValidateEndSesion = options.ValidateEndSesion || "ServiceCerrarSesion";

        // Redirección
        this.logoutUrl = options.logoutUrl || "/Login";

        // Intervalos
        this.intervaloKeepAlive = options.intervaloKeepAlive || 30 * 1000;
        this.intervaloVerificar = options.intervaloVerificar || 1000;
        this.intervaloValidateEndpoint = options.intervaloValidateEndpoint || 30 * 1000;

        // UI elements
        this.lblTiempoSesion = document.getElementById("lblTiempoSesion");
        this.lblTiempoSesionRestante = document.getElementById("lblTiempoSesionRestante");
        this.progressBar = document.getElementById("sessionProgressBar");

        // Tiempo
        this.secondsRemaining = 0;
        this.secondsElapsed = 0;
        this.totalSeconds = 0;          // ← 🔥 NECESARIO PARA LA BARRA

        // Actividad
        this.userIsActive = true;
        this._registrarEventosActividad();

        this.init();
    }

    init() {
        this.crearMensajeExpiracion();
        this.sincronizarTiempoServidor();

        setInterval(() => this.actualizarContadores(), this.intervaloVerificar);
        setInterval(() => this._ServiceIsSessionTimedOut(), this.intervaloValidateEndpoint);
        setInterval(() => this.keepAlive(), this.intervaloKeepAlive);
    }

    // -------------------------------------------
    // ACTIVIDAD
    // -------------------------------------------
    _registrarEventosActividad() {
        const eventos = ["mousemove", "mousedown", "keydown", "scroll", "touchstart"];
        eventos.forEach(ev => {
            document.addEventListener(ev, () => this.userIsActive = true);
        });
    }

    // -------------------------------------------
    // KEEP ALIVE
    // -------------------------------------------
    async keepAlive() {
        if (!this.userIsActive) return;
        this.userIsActive = false;

        try {
            const resp = await this.api.use(this.serviceKey).call(this.keepAliveEndpoint);

            console.group("🔄 KEEPALIVE");
            console.log("Respuesta completa:", resp);
            console.log("Session ID:", resp?.data?.sessionId);
            console.log("LastAccess:", resp?.data?.lastAccess);
            console.groupEnd();

        } catch (e) {
            console.error("⚠️ Error keepAlive", e);
        }
    }

    // -------------------------------------------
    // VALIDACIÓN REMOTA
    // -------------------------------------------
    async _ServiceIsSessionTimedOut() {
        try {
            const resp = await this.api.use(this.serviceKey).call(this.ValidateEndpoint);
            if (!resp || resp.error || resp.data?.Success === false) {
                this.mostrarMensajeExpiracion(resp?.data?.Message || "Sesión expirada");
            }
        } catch { }
    }

    // -------------------------------------------
    // TIEMPO RESTANTE DESDE EL SERVIDOR
    // -------------------------------------------
    async sincronizarTiempoServidor() {
        try {
            const result = await this.api.use(this.serviceKey).call(this.tiempoRestanteEndpoint);

            if (result?.data?.restanteSegundos >= 0) {
                this.secondsRemaining = result.data.restanteSegundos;

                // ⚠️ Definir totalSeconds SOLO una vez
                if (this.totalSeconds === 0) {
                    this.totalSeconds = this.secondsRemaining;
                }

                this._actualizarUI();
            }
        } catch (e) {
            console.warn("Error sincronizando tiempo:", e);
        }
    }

    // -------------------------------------------
    // CONTADOR LOCAL
    // -------------------------------------------
    actualizarContadores() {
        if (this.secondsRemaining <= 0) {
            this.mostrarMensajeExpiracion();
            return;
        }

        this.secondsRemaining--;
        this.secondsElapsed++;

        this._actualizarUI();

        if (this.secondsRemaining % 10 === 0) {
            this.sincronizarTiempoServidor();
        }
    }

    // -------------------------------------------
    // ACTUALIZA UI COMPLETA
    // -------------------------------------------
    _actualizarUI() {

        // Tiempo transcurrido
        if (this.lblTiempoSesion) {
            this.lblTiempoSesion.textContent = this._format(this.secondsElapsed);
        }

        // Tiempo restante
        if (this.lblTiempoSesionRestante) {
            this.lblTiempoSesionRestante.textContent = this._format(this.secondsRemaining);
        }

        // Barra de progreso
        if (this.progressBar && this.totalSeconds > 0) {

            const pct = (this.secondsRemaining / this.totalSeconds) * 100;
            this.progressBar.style.width = pct + "%";

            if (pct > 60) this.progressBar.style.backgroundColor = "#007bff";
            else if (pct > 30) this.progressBar.style.backgroundColor = "#f1c40f";
            else this.progressBar.style.backgroundColor = "#e74c3c";
        }
    }

    // -------------------------------------------
    // FORMATO DE TIEMPO
    // -------------------------------------------
    _format(seg) {
        const m = Math.floor(seg / 60);
        const s = seg % 60;
        return `${m.toString().padStart(2, "0")}:${s.toString().padStart(2, "0")}`;
    }

    // -------------------------------------------
    // MENSAJE EXPIRACIÓN
    // -------------------------------------------
    crearMensajeExpiracion() {
        const html = `
        <div id="sessionExpiredAlert"
             class="alert alert-danger text-center position-fixed top-0 start-50 translate-middle-x shadow"
             role="alert"
             style="display:none; z-index:2000; width:100%; max-width:500px; margin-top:10px;">
            <span id="sessionExpiredMessage">
                <strong>⚠️ Tu sesión ha finalizado.</strong>
            </span>
        </div>`;
        document.body.insertAdjacentHTML("beforeend", html);

        this.alert = document.getElementById("sessionExpiredAlert");
        this.alertMessage = document.getElementById("sessionExpiredMessage");
    }

    mostrarMensajeExpiracion(msg = "") {
        if (!this.alert) return;

        this.alertMessage.innerHTML =
            `<strong>⚠️ Tu sesión ha finalizado.</strong><br><small>${msg}</small>`;

        this.alert.style.display = "block";

        setTimeout(() => {
            this.alert.style.opacity = "0";
            setTimeout(() => this.cerrarSesion(), 1500);
        }, 2500);
    }

    /*cerrarSesion() {
        window.location.href = this.logoutUrl;
    }

    cerrarSesionManual() {
        this.api.use(this.serviceKey).call(this.ValidateEndSesion).finally(() => {
            this.cerrarSesion();
        });
    }*/

    // -------------------------------------------
    // CERRAR SESIÓN (redirección inmediata)
    // -------------------------------------------
    cerrarSesion() {
        window.location.href = this.logoutUrl;
    }

    // -------------------------------------------
    // CERRAR SESIÓN MANUAL (modal con spinner)
    // -------------------------------------------
    cerrarSesionManual() {
        // Crear modal spinner si no existe
        if (!document.getElementById("modalCierreSesion")) {
            const modalHTML = `
        <div class="modal fade" id="modalCierreSesion" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content text-center p-4">
                    <div class="modal-body">
                        <div class="spinner-border text-primary mb-3" style="width: 3rem; height: 3rem;" role="status"></div>
                        <h5 class="fw-bold text-dark">Cerrando sesión...</h5>
                        <p class="text-muted mb-0">Por favor, espera un momento.</p>
                    </div>
                </div>
            </div>
        </div>`;
            document.body.insertAdjacentHTML("beforeend", modalHTML);
        }

        const modalElement = document.getElementById("modalCierreSesion");
        const modal = new bootstrap.Modal(modalElement, {
            backdrop: 'static',
            keyboard: false
        });

        modal.show(); // 🔄 Mostrar spinner modal

        // 🔌 Intentar notificar al backend
        try {
            this.api.use(this.serviceKey).call(this.ValidateEndSesion).finally(() => {
                // Espera breve para transición visual
                setTimeout(() => {
                    modal.hide();
                    this.cerrarSesion();
                }, 1500);
            });
        } catch (error) {
            console.error("⚠️ Error al cerrar sesión manualmente:", error);
            modal.hide();
            setTimeout(() => this.cerrarSesion(), 1000);
        }
    }

    // -------------------------------------------
    // CONFIRMACIÓN ANTES DE CERRAR SESIÓN
    // -------------------------------------------
    mostrarConfirmacionCerrarSesion() {
        const existente = document.getElementById("confirmLogoutDialog");
        if (existente) existente.remove();

        const html = `
        <div id="confirmLogoutDialog"
             class="position-fixed top-50 start-50 translate-middle bg-white border rounded shadow p-4 text-center"
             style="z-index: 3000; width: 90%; max-width: 400px;">
            <h5 class="mb-3 text-primary fw-bold">¿Deseas cerrar tu sesión?</h5>
            <p class="mb-4 text-muted">Serás redirigido a la página principal.</p>
            <div class="d-flex justify-content-center gap-3">
                <button id="btnConfirmarLogout" class="btn btn-primary px-4">
                    <i class="bi bi-box-arrow-right"></i> Sí, cerrar sesión
                </button>
                <button id="btnCancelarLogout" class="btn btn-secondary px-4">
                    <i class="bi bi-x-circle"></i> Cancelar
                </button>
            </div>
        </div>`;

        document.body.insertAdjacentHTML("beforeend", html);

        // Fondo oscuro detrás
        const overlay = document.createElement("div");
        overlay.id = "logoutOverlay";
        overlay.style.cssText = `
        position: fixed; 
        top: 0; left: 0; 
        width: 100%; height: 100%;
        background-color: rgba(0,0,0,0.4);
        z-index: 2999;
    `;
        document.body.appendChild(overlay);

        // Confirmar → cerrar sesión con spinner
        document.getElementById("btnConfirmarLogout").addEventListener("click", () => {
            this.eliminarDialogoLogout();
            this.cerrarSesionManual();
        });

        // Cancelar → cerrar cuadro
        document.getElementById("btnCancelarLogout").addEventListener("click", () => {
            this.eliminarDialogoLogout();
        });
    }

    // -------------------------------------------
    // LIMPIAR DIÁLOGO Y OVERLAY
    // -------------------------------------------
    eliminarDialogoLogout() {
        const dlg = document.getElementById("confirmLogoutDialog");
        const overlay = document.getElementById("logoutOverlay");
        if (dlg) dlg.remove();
        if (overlay) overlay.remove();
    }

}
