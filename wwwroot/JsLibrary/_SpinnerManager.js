class SpinnerManager {
    constructor(minGlobalTime = 800) {
        this.localSpinners = new Map();
        this.globalSpinner = null;
        this.blockedElements = new Set();
        this.globalBlockedButtons = [];
        this.blockGlobalButtons = false;

        // ⏱ Tiempo mínimo de spinner global en ms
        this.minGlobalTime = minGlobalTime;
        this.globalStartTime = null; // Marca de inicio
    }

    // 🔹 Spinner local en botón
    showOnButton(elementId, type = "circle") {
        const el = document.getElementById(elementId);
        if (!el || this.localSpinners.has(elementId)) return;

        if (getComputedStyle(el).position === "static") el.style.position = "relative";
        el.style.overflow = "hidden";

        const overlay = document.createElement("div");
        overlay.style.position = "absolute";
        overlay.style.top = "0";
        overlay.style.left = "0";
        overlay.style.width = "100%";
        overlay.style.height = "100%";
        overlay.style.backgroundColor = "rgba(255,255,255,0.5)";
        overlay.style.display = "flex";
        overlay.style.justifyContent = "center";
        overlay.style.alignItems = "center";
        overlay.style.borderRadius = getComputedStyle(el).borderRadius;
        overlay.style.zIndex = "1000";
        overlay.style.opacity = "0";
        overlay.style.transition = "opacity 0.25s";

        const spinner = document.createElement("div");
        spinner.className = type === "file" ? "spinner-file" : "spinner-circle small";
        overlay.appendChild(spinner);

        el.appendChild(overlay);
        requestAnimationFrame(() => overlay.style.opacity = "1");

        this.localSpinners.set(elementId, { overlay });
        el.disabled = true;
        this.blockedElements.add(el);
    }

    hideOnButton(elementId) {
        const spinnerObj = this.localSpinners.get(elementId);
        if (!spinnerObj) return;
        const { overlay } = spinnerObj;

        overlay.style.opacity = "0";
        overlay.addEventListener("transitionend", () => overlay.remove(), { once: true });

        const el = document.getElementById(elementId);
        if (el && this.blockedElements.has(el)) {
            el.disabled = false;
            this.blockedElements.delete(el);
        }

        this.localSpinners.delete(elementId);
    }

    hideAllButtons() {
        for (const id of Array.from(this.localSpinners.keys())) this.hideOnButton(id);
    }

    // 🔹 Spinner global
    showGlobal(type = "file", options = { blockButtons: false, buttons: [] }) {
        if (this.globalSpinner) return;

        this.globalStartTime = Date.now();
        this.blockGlobalButtons = options.blockButtons;

        if (this.blockGlobalButtons && Array.isArray(options.buttons)) {
            options.buttons.forEach(id => {
                const btn = document.getElementById(id);
                if (btn) {
                    btn.disabled = true;
                    this.globalBlockedButtons.push(btn);
                }
            });
        }

        const loading = document.createElement("div");
        loading.id = "loadingSpinner";
        loading.style.position = "fixed";
        loading.style.top = "0";
        loading.style.left = "0";
        loading.style.width = "100%";
        loading.style.height = "100%";
        loading.style.backgroundColor = "rgba(255,255,255,0.8)";
        loading.style.display = "flex";
        loading.style.justifyContent = "center";
        loading.style.alignItems = "center";
        loading.style.zIndex = "110061";
        loading.style.opacity = "0";
        loading.style.transition = "opacity 0.25s";

        const spinner = document.createElement("div");
        spinner.className = type === "file" ? "spinner-file" : "spinner-circle";
        loading.appendChild(spinner);

        document.body.appendChild(loading);
        requestAnimationFrame(() => loading.style.opacity = "1");

        this.globalSpinner = loading;
    }

    // 🔹 hideGlobal respetando el tiempo mínimo
    async hideGlobal() {
        if (!this.globalSpinner) return;

        const elapsed = Date.now() - (this.globalStartTime || 0);
        if (elapsed < this.minGlobalTime) {
            await new Promise(resolve => setTimeout(resolve, this.minGlobalTime - elapsed));
        }

        const spinner = this.globalSpinner;
        spinner.style.opacity = "0";
        spinner.addEventListener("transitionend", () => {
            if (spinner.parentNode) spinner.remove();
        }, { once: true });

        this.globalSpinner = null;

        if (this.blockGlobalButtons && this.globalBlockedButtons.length > 0) {
            this.globalBlockedButtons.forEach(btn => btn.disabled = false);
            this.globalBlockedButtons = [];
        }
    }
}


