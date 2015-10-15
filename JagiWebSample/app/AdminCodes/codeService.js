(function () {
    'use strict';
    window.app.factory('codeService', codeService);
    codeService.$inject = ['$http', 'model'];

    function codeService($http, model) {
        var details = [];
        return {
            update: update,
            add: add,
            //loadDetails: loadDetails
        };

        function add(code) {
            return $http.post('/Admin/Codes/Add', code);
        }

        function update(code) {
            return $http.post('/Admin/Codes/Update', code)
                .success(function (data) {
                    angular.extend(code, data);
                });
        }
    }
})();