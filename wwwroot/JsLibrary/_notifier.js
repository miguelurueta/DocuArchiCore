class Notifier {
    static container = null;

    static #createContainer() {
        if (!this.container) {
            this.container = document.createElement("div");
            this.container.className = "notifier-container";
            document.body.appendChild(this.container);
        }
    }

    /**
     * Muestra una notificación
     * @param {string} message - Texto del mensaje
     * @param {string} type - success | error | warning | info
     * @param {string} position - top-left | top-right | bottom-left | bottom-right | center
     * @param {object} options - { persistent: boolean, timeout: number }
     */
    static show(message, type = "info", position = "top-right", options = {}) {
        this.#createContainer();

        const { persistent = false, timeout = 3000 } = options;

        // contenedor por posición
        let containerPos = document.querySelector(`.notifier-${position}`);
        if (!containerPos) {
            containerPos = document.createElement("div");
            containerPos.className = `notifier-position notifier-${position}`;
            this.container.appendChild(containerPos);
        }

        // crear notificación
        const notif = document.createElement("div");
        notif.className = `notifier ${type}`;
        notif.innerHTML = `
      <span class="message">${message}</span>
      <button class="close-btn">&times;</button>
    `;

        // evento de cierre manual
        notif.querySelector(".close-btn").addEventListener("click", () => notif.remove());

        containerPos.appendChild(notif);

        // si no es persistente, cerramos después de timeout
        if (!persistent && timeout > 0) {
            setTimeout(() => notif.remove(), timeout);
        }

        return notif;
    }

    /** Oculta TODAS las notificaciones activas */
    static hide() {
        if (this.container) {
            this.container.innerHTML = "";
        }
    }
}

// Exportamos en global
window.Notifier = Notifier;

class FormNotifier extends Notifier {
    constructor() {
        super();
    }

    showError(fieldId, message) {
        const field = document.getElementById(fieldId);
        const errorSpan = document.getElementById(`error-${fieldId}`);

        if (field) field.classList.add("error-control");
        if (errorSpan) errorSpan.textContent = message;
    }

    clearError(fieldId) {
        const field = document.getElementById(fieldId);
        const errorSpan = document.getElementById(`error-${fieldId}`);

        if (field) field.classList.remove("error-control");
        if (errorSpan) errorSpan.textContent = "";
    }

    clearAll() {
        document.querySelectorAll(".error-control").forEach(el => el.classList.remove("error-control"));
        document.querySelectorAll(".error-message").forEach(span => span.textContent = "");
    }
}
