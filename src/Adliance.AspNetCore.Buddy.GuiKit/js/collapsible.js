function initCollapsible() {
    document.querySelectorAll("article.collapsible .collapsible-toggle").forEach(toggle => {
        if (toggle.dataset.initialized === "true") return;
        toggle.dataset.initialized = "true";

        toggle.addEventListener("click", () => {
            toggle.closest("article.collapsible").classList.toggle("collapsed");
        });
    });
}

document.addEventListener("DOMContentLoaded", initCollapsible);
document.addEventListener("htmx:afterSettle", initCollapsible);
