document.addEventListener("DOMContentLoaded", function () {
    const button = document.getElementById("fontSizeToggle");
    const body = document.body;
    const resizableElements = document.querySelectorAll(".text-resizable");

    // Sjekk om brukeren har valgt stor tekst tidligere
    let isLargeText = localStorage.getItem("largeText") === "true";

    // Funksjon for å oppdatere tekststørrelsen
    function updateTextSize() {
        body.classList.toggle("large-text", isLargeText);
        resizableElements.forEach(el => el.classList.toggle("large-text", isLargeText));

        // Oppdater knappeteksten
        button.textContent = isLargeText ? "Vanlig tekst" : "Større tekst";

        // Lagre valget i localStorage
        localStorage.setItem("largeText", isLargeText);
    }

    // Kjør funksjonen ved lasting for å bruke tidligere valg
    updateTextSize();

    // Legg til event listener på knappen
    button.addEventListener("click", function () {
        isLargeText = !isLargeText;
        updateTextSize();
    });
});
