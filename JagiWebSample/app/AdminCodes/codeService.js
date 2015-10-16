(function () {
    'use strict';
    window.app.factory('codeService', codeService);
    codeService.$inject = ['$http', 'model'];

    function codeService($http, model) {
        var details = [];
        return {
            update: update,
            add: add,
            details: loadDetails,
            saveDetail: saveDetail
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

        function loadDetails(id) {
            return $http.get('/Admin/Codes/Details/' + id);
        }

        function saveDetail(updatedDetail, existDetail) {
            return $http.post('/Admin/Codes/UpdateDetail', updatedDetail)
                .success(function (data) {
                    angular.extend(existDetail, data);
                });
        }
    }
})();