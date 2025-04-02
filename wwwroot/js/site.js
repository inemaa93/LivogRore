// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const express = require('express');
const app = express();
const PORT = 3000;

app.get('/api/test', (req, res) => {
    res.json({ message: "API is working!" });
});

app.listen(PORT, () => {
    console.log(`Server is running on http://localhost:${PORT}`);
});