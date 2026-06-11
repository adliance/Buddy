function initHamburger() {
    document.querySelector("nav > header > .hamburger").addEventListener("click", event => {
        event.target.closest("nav").classList.toggle("open");
    });
}

document.addEventListener("DOMContentLoaded", initHamburger);

