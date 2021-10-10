/* global bootstrap: false */
(function () {
    'use strict'
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    tooltipTriggerList.forEach(function (tooltipTriggerEl) {
        new bootstrap.Tooltip(tooltipTriggerEl)
    })
})()

// auto import file html in index.js
$(document).ready(function () {
    $("div[data-includeHTML]").each(function () {
        $(this).load(`src/${$(this).attr("data-includeHTML")}.html`);
    });
});