(function () {
    'use strict';
    window.app.factory('codeService', codeService);
    codeService.$inject = ['$http', 'model', 'alerts'];

    function codeService($http, model, alerts) {
        var details = [];
        return {
            save: save,
            details: loadDetails,
            deleteCode: deleteCode,
            saveDetail: saveDetail,
            deleteDetail: deleteDetail
        };

        function save(code, existCode) {
            return $http.post('/Admin/Codes/Save', code)
                .success(function (data) {
                    angular.extend(existCode, data);
                })
                .error(function (data) {
                    alerts.ajaxError(data);
                });
        }

        function deleteCode(id) {
            return $http.post('/Admin/Codes/DeleteCode/' + id)
                .error(function (data) {
                    alerts.ajaxError(data);
                });
        }

        function loadDetails(id) {
            return $http.get('/Admin/Codes/Details/' + id);
        }

        function saveDetail(updatedDetail, existDetail) {
            return $http.post('/Admin/Codes/SaveDetail', updatedDetail)
                .success(function (data) {
                    angular.extend(existDetail, data);
                });
        }

        function deleteDetail(id) {
            return $http.post('/Admin/Codes/DeleteDetail/' + id)
                .error(function (data) {
                    alerts.ajaxError(data);
                });
        }
    }
})();