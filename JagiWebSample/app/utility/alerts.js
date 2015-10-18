(function () {
    'use strict';

    angular.module('app').factory('alerts', function () {
        return window.alerts;
    })

    var alertService = {
        showAlert: showAlert,
        success: success,
        info: info,
        warning: warning,
        error: error,
        ajaxError: ajaxError
    };
    // set to global elemnet
    window.alerts = alertService;

    var alertContainer = $('.alert-container');

    var template = _.template(
        "<div class='alert <%= alertClass %> alert-dismissable'>"
        + "<button type='button' class='close' data-dismiss='alert'><span aria-hidden='true'>"
        + "&times; </span><span class='sr-only'>Close</span>"
        + "</button><strong><%= message %></strong></div>"
        );

    function showAlert(alert) {
        var alertElment = $(template(alert));
        alertContainer.append(alertElment);

        window.setTimeout(function () {
            alertElment.fadeOut();
        }, 3000);
    }

    function success(message) {
        showAlert({ alertClass: "alert-success", message: message });
    }

    function info(message) {
        showAlert({ alertClass: "alert-info", message: message });
    }

    function warning(message) {
        showAlert({ alertClass: "alert-warning", message: message });
    }

    function error(message) {
        showAlert({ alertClass: "alert-danger", message: message });
    }

    function ajaxError(data) {
        error("資料取得失敗：Status: "
            + data.status + "; StatusText: " + data.statusText);
    }
})();