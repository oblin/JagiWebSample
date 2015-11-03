(function () {
    'use strict';

    window.app.controller("patientEditController", patientEditController);
    patientEditController.$inject = ['$scope', 'model', '$routeParams', '$window', 'dataService'];

    function patientEditController($scope, model, $routeParams, $window, dataService) {
        var vm = this;
        vm.dataService = dataService;
        vm.id = $routeParams.id;
        vm.origin;
        vm.current;

        vm.save = save;
        vm.cancel = cancel;
        vm.delete = deleteItem;

        getPatient(vm.id);

        function getPatient(id) {
            var url = model.getPatientUrl;

            dataService.get(url + id, null, function (response) {
                vm.origin = response.data;
                vm.current = angular.copy(vm.origin);
            })
        }

        function save(item) {
            dataService.post(model.savePatientUrl, item,
                function (response) {
                    angular.extend(vm.current, response.data);
                    angular.extend(vm.origin, response.data);
                    reset();
                })
        }

        function deleteItem(id) {
            if (id == 0) return;
            dataService.post(model.deletePatientUrl + id,
                function () {
                    $window.history.back();
                })
        }

        function cancel() {
            angular.extend(vm.current, vm.origin);
            reset();
        }

        function reset() {
            $scope.patientEditForm.$setPristine();
            $scope.patientEditForm.$setUntouched();
        }
    }
})();