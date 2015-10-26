(function () {
    // using to be called whenever on handle error occurs
    window.onerror = function (message) {
        if (window.alerts) {
            window.alerts.error("發生錯誤，請重新載入頁面後重新操作");
        } else {
            console.log("into alert");
            alert("發生無法解決的問題，請關閉瀏覽器後再次操作！");
        }
    }
    
    app = angular.module('app', ['ngRoute', 'ui.bootstrap', 'loadingService', 'common', 'ui.grid']);

    app.config(function ($httpProvider) {
        $httpProvider.interceptors.push('httpInterceptor');
        var spinnerFunction = function (data, headers) {
            $('#loading').modal('show');
            return data;
        };
        $httpProvider.defaults.transformRequest.push(spinnerFunction);
    });

    angular.module('loadingService', [], function ($provide) {
        $provide.factory('httpInterceptor', function ($q, $window) {
            return {
                'response': function (response) {
                    $('#loading').modal('hide');
                    $('.modal-backdrop').remove();
                    return response;
                },
                'responseError': function (rejection) {
                    angular.element('#loading').modal('hide');
                    angular.element('.modal-backdrop').remove();
                    return $q.reject(rejection);
                }
            }
        });
    });
})();
