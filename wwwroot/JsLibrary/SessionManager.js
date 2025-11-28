class SessionManager {
    constructor(options = {}) {
        this.api = new ApiClientService();

        this.serviceKey = options.serviceKey || "Home";
        this.keepAliveEndpoint = options.keepAliveEndpoint || "KeepAlive";
        this.tiempoRestanteEndpoint = options.tiempoRestanteEndpoint || "TiempoRestante";
        this.logoutUrl = options.logoutUrl || "/Login";
        this.ValidateEndpoint = options.ValidateEndpoint || "ServiceIsSessionTimedOut";
        this.ValidateEndSesion = options.ValidateEndSesion || "ServiceCerrarSesion";
        // Intervalos (en milisegundos)
        this.intervaloKeepAlive = options.intervaloKeepAlive || 30 * 1000; // 30 seg
        this.intervaloVerificar = options.intervaloVerificar || 1000;      // 1 seg
        this.intervaloValidateEndpoint = options.intervaloValidateEndpoint || 5 * 1000; // 90 seg  

        this.secondsRemaining = 0;
        this.secondsElapsed = 0;
        this.sessionDuration = 0; // duración total real (segundos)
        this.lblTiempoSesion = document.getElementById("lblTiempoSesion");

        this.init();
    }

    init() {
        this.crearMensajeExpiracion();
        this.sincronizarTiempoServidor();

        // 🔁 Mantener sesión viva periódicamente
        //setInterval(() => this.keepAlive(), this.intervaloKeepAlive);

        // ⏱️ Actualizar contador cada segundo
        setInterval(() => this.actualizarContadores(), this.intervaloVerificar);

        setInterval(() => this._ServiceIsSessionTimedOut(), this.intervaloValidateEndpoint);
       
    }

    async keepAlive() {
        try {
            const result = await this.api.use(this.serviceKey).call(this.keepAliveEndpoint);

            // Si solo devuelve 200 OK
            if (result.status === 200 || result.success) {
                await this.sincronizarTiempoServidor();
                console.log("🔄 Sesión mantenida viva");
            }
        } catch (error) {
            console.log("⚠️ Error en keepAlive:", error);
        }
    }

    async _ServiceIsSessionTimedOut() {
        try {
            const Rest = await this.api.use(this.serviceKey).call(this.ValidateEndpoint);
            if (!Rest || Rest.error || Rest.data?.Success === false) {
                const msg = Rest?.message || Rest?.data?.Message || "Error en la solicitud.";  
                this.mostrarMensajeExpiracion(msg);
            }
        } catch (error) {
            console.log("⚠️ Error en _ServiceIsSessionTimedOut:", error);
        }
    }


    async sincronizarTiempoServidor() {
        try {
            const result = await this.api.use(this.serviceKey).call(this.tiempoRestanteEndpoint);
            if (!result.error && result.data?.restanteSegundos) {
                this.secondsRemaining = result.data.restanteSegundos;
                this.actualizarTiempoVisible(this.secondsRemaining);
            }
        } catch (error) {
            console.error("⚠️ Error sincronizando tiempo de sesión:", error);
        }
    }

    actualizarContadores() {
        // Si aún no se ha inicializado el tiempo, salir
        if (this.secondsRemaining <= 0) {
            this.mostrarMensajeExpiracion();
            setTimeout(() => this.cerrarSesion(), 4000);
            return;
        }

        // Incrementa el contador de tiempo transcurrido
        this.secondsElapsed++;

        // Decrementa el tiempo restante, pero solo para detectar timeout
        this.secondsRemaining++;

        // Muestra solo el tiempo transcurrido
        this.actualizarTiempoVisible(this.secondsElapsed);
    }

    actualizarTiempoVisible(transcurridos) {
        if (this.lblTiempoSesion) {
            const horas = Math.floor(transcurridos / 3600);
            const minutos = Math.floor((transcurridos % 3600) / 60);
            const segs = transcurridos % 60;

            // Mostrar formato HH:MM:SS si supera una hora, de lo contrario MM:SS
            this.lblTiempoSesion.textContent =
                horas > 0
                    ? `${horas.toString().padStart(2, "0")}:${minutos
                        .toString()
                        .padStart(2, "0")}:${segs.toString().padStart(2, "0")}`
                    : `${minutos.toString().padStart(2, "0")}:${segs
                        .toString()
                        .padStart(2, "0")}`;
        }

       
    }


    crearMensajeExpiracion() {
        // Crear una sola vez
        const alertHTML = `
        <div id="sessionExpiredAlert" 
             class="alert alert-danger text-center position-fixed top-0 start-50 translate-middle-x shadow"
             role="alert"
             style="display:none; z-index:2000; width:100%; max-width:500px; margin-top:10px;">
            <span id="sessionExpiredMessage">
                <strong>⚠️ Tu sesión ha finalizado.</strong> Serás redirigido al inicio de sesión...
            </span>
        </div>`;

        document.body.insertAdjacentHTML("beforeend", alertHTML);
        this.alert = document.getElementById("sessionExpiredAlert");
        this.alertMessage = document.getElementById("sessionExpiredMessage");
    }


    mostrarMensajeExpiracion(errorMsg = "") {
        if (!this.alert || !this.alertMessage) return;

        let mensajeBase = `<strong>⚠️ Tu sesión ha finalizado.</strong> Serás redirigido al inicio de sesión...`;

        if (errorMsg) {
            mensajeBase += `<br><small class="text-muted">Motivo: ${errorMsg}</small>`;
        }

        this.alertMessage.innerHTML = mensajeBase;
        this.alert.style.display = "block";

        // Desvanecer suavemente antes de redirigir
        setTimeout(() => {
            this.alert.style.transition = "opacity 0.5s ease";
            this.alert.style.opacity = "0";
            setTimeout(() => this.cerrarSesion(), 1500);
        }, 3000);
    }


    cerrarSesion() {
        window.location.href = this.logoutUrl;
    }
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
                // Espera un momento para que el usuario vea el spinner
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

        const overlay = document.createElement("div");
        overlay.id = "logoutOverlay";
        overlay.style.cssText = `
        position: fixed; top: 0; left: 0; width: 100%; height: 100%;
        background-color: rgba(0,0,0,0.4);
        z-index: 2999;
    `;
        document.body.appendChild(overlay);

        // ✅ Confirmar cierre → usa cerrarSesionManual()
        document.getElementById("btnConfirmarLogout").addEventListener("click", () => {
            this.eliminarDialogoLogout();
            this.cerrarSesionManual(); // 👈 Aquí se usa el tuyo
        });

        // ❌ Cancelar
        document.getElementById("btnCancelarLogout").addEventListener("click", () => {
            this.eliminarDialogoLogout();
            
        });
    }
    eliminarDialogoLogout() {
        const dlg = document.getElementById("confirmLogoutDialog");
        const overlay = document.getElementById("logoutOverlay");
        if (dlg) dlg.remove();
        if (overlay) overlay.remove();
    }
}
