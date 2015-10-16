var app = angular.module('app', ['ngRoute', 'ui.bootstrap', 'loadingService']);

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('httpInterceptor');
    var spinnerFunction = function (data, headers) {
        //angular.element('#loading').modal('show');
        $('#loading').modal('show');
        return data;
    };
    $httpProvider.defaults.transformRequest.push(spinnerFunction);
});

angular.module('loadingService', [], function ($provide) {
    $provide.factory('httpInterceptor', function ($q, $window) {
        return {
            'response': function (response) {
                //angular.element('#loading').modal('hide');
                //angular.element('.modal-backdrop').remove();
                $('#loading').modal('hide');
                $('.modal-backdrop').remove();
                return response;
            },
            'responseError': function (rejection) {
                //$('#loading').hide();
                //$('#loading').modal('hide');
                angular.element('#loading').modal('hide');
                angular.element('.modal-backdrop').remove();
                return $q.reject(rejection);
            }
        }
    });
});

$('.deleteBtn').on('click', function () {
    if (!confirm("確定要刪除此筆資料？")) {
        return false;
    }
});
