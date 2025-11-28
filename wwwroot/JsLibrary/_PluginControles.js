// pluginControles.js
const PluginControles = {
    /**
     * Inicialización global — detecta automáticamente los controles
     */
    init() {
        this.initTogglePasswordAuto();
        this.initCustomSelect();
        // Espacio para futuras extensiones
        // this.initSwitches();
        // this.initRangeSliders();
    },

    /**
     * Toggle mostrar/ocultar contraseña (automático por data-* o clase)
     * <input type="checkbox" class="toggle-pwd" data-pwd-id="pwd1" data-icon-id="icon1">
     */
    initTogglePasswordAuto() {
        document.querySelectorAll('[data-toggle-pwd], .toggle-pwd').forEach(toggle => {
            const pwdId = toggle.dataset.pwdId || toggle.getAttribute('data-pwd-id');
            const iconId = toggle.dataset.iconId;
            const showClass = toggle.dataset.showClass || 'fa-eye';
            const hideClass = toggle.dataset.hideClass || 'fa-eye-slash';
            const pwd = document.getElementById(pwdId);
            const icon = iconId ? document.getElementById(iconId) : null;
            if (!pwd) return;

            toggle.addEventListener('change', () => {
                const show = toggle.checked;
                pwd.type = show ? 'text' : 'password';
                if (icon) {
                    icon.classList.toggle(showClass, show);
                    icon.classList.toggle(hideClass, !show);
                }
            });
        });
    },

    /**
     * Inicializa selects personalizados (.custom-select)
     * Compatible con listas dinámicas (usando delegación de eventos)
     */
    initCustomSelect() {
        document.querySelectorAll('.custom-select').forEach(select => {
            const selected = select.querySelector('.selected');
            const options = select.querySelector('.options');
            if (!selected || !options) return;

            // Toggle abrir/cerrar
            selected.addEventListener('click', () => {
                document.querySelectorAll('.custom-select.open').forEach(s => {
                    if (s !== select) s.classList.remove('open');
                });
                select.classList.toggle('open');
            });

            // Delegación de eventos (para listas dinámicas)
            options.addEventListener('click', e => {
                const option = e.target.closest('li');
                if (!option) return;

                const text = option.textContent.trim();
                const value = option.dataset.value ?? "";

                // Actualiza texto y valor
                selected.querySelector('span').textContent = text;
                selected.dataset.value = value;
                select.classList.remove('open');

                // Evento personalizado
                select.dispatchEvent(new CustomEvent('customselect:change', {
                    detail: { value, text },
                    bubbles: true
                }));
            });
        });

        // Cierra todos los selects si se hace clic fuera
        document.addEventListener('click', e => {
            if (!e.target.closest('.custom-select')) {
                document.querySelectorAll('.custom-select.open').forEach(s => s.classList.remove('open'));
            }
        });
    },

    /**
     * Devuelve el valor y texto actual del select personalizado
     * @param {string} selectId
     * @returns {{ value: string, text: string } | null}
     */
    getSelectedValue(selectId) {
        try {
            const select = document.getElementById(selectId);
            if (!select) return null;

            const selected = select.querySelector('.selected');
            if (!selected) return null;

            const value = selected.dataset.value || "";
            const text = selected.querySelector('span')?.textContent?.trim() || "";
            return { value, text };
        } catch (err) {
            console.error("Error en getSelectedValue:", err);
            return null;
        }
    },


    /**
     * Establece programáticamente el valor de un select personalizado
     * @param {string} selectId - ID del .custom-select
     * @param {string|number} value - Valor a seleccionar (según data-value)
     */
    setSelectedValue(selectId, value) {
        const select = document.getElementById(selectId);
        if (!select) return;

        const selected = select.querySelector('.selected span');
        const options = select.querySelectorAll('.options li');
        if (!selected || !options.length) return;

        let match = null;

        options.forEach(opt => {
            if (opt.dataset.value == value) match = opt;
        });

        if (match) {
            const text = match.textContent.trim();

            selected.textContent = text;
            select.querySelector('.selected').dataset.value = value;

            // Dispara el evento de cambio personalizado
            select.dispatchEvent(new CustomEvent('customselect:change', {
                detail: { value, text },
                bubbles: true
            }));
        }
    },

    /**
     * Permite registrar controles adicionales
     */
    registerControl(selector, callback) {
        document.querySelectorAll(selector).forEach(callback);
    }
};

// Exponer globalmente
window.PluginControles = PluginControles;


