document.getElementById('registerForm').addEventListener('submit', function(event) {
    event.preventDefault();
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    localStorage.setItem(username, password);
    alert('Registrering vellykket!');
});

document.getElementById('loginForm').addEventListener('submit', function(event) {
    event.preventDefault();
    const loginUsername = document.getElementById('loginUsername').value;
    const loginPassword = document.getElementById('loginPassword').value;
    const storedPassword = localStorage.getItem(loginUsername);

    if (storedPassword === loginPassword) {
        alert('Innlogging vellykket!');
    } else {
        alert('Feil brukernavn eller passord.');
    }
});