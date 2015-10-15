var app = angular.module('app', ['ngRoute', 'ui.bootstrap']);

$('.deleteBtn').on('click', function () {
    if (!confirm("確定要刪除此筆資料？")) {
        return false;
    }
});
