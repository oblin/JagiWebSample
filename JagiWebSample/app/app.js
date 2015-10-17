(function () {
    app = angular.module('app', ['ngRoute', 'ui.bootstrap', 'loadingService']);

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
