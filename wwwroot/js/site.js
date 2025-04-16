// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
console.log('Site.js loaded successfully');

// Add event listeners when the document is ready
document.addEventListener('DOMContentLoaded', function() {
    // Example: Add click handler for a button with id 'testButton'
    const testButton = document.getElementById('testButton');
    if (testButton) {
        testButton.addEventListener('click', function() {
            console.log('Test button clicked');
            // Add your button click handling code here
        });
    }
});