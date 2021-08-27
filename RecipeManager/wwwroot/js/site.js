// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var recipeDetailPostBackURL = '/Home/Details';
$(function () {
    $('#NewIngredientModal').on('shown.bs.modal', function (e) {
        $('#recipeIngredientName').trigger('focus');
    });

    $('#NewStepModal').on('shown.bs.modal', function (e) {
        $('#recipeStepName').trigger('focus');
    });

    $('.addStepTrigger').click(function (e) {
    });

    $('.addIngredientTrigger').click(function (e) {
    });
});