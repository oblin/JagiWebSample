(function () {
    'use strict';

    window.app.controller("patientEditController", patientEditController);
    patientEditController.$inject = ['$scope', 'model', '$location', '$routeParams', '$window', '$timeout', 'dataService'];

    function patientEditController($scope, model, $location, $routeParams, $window, $timeout, dataService) {
        var vm = this;
        vm.dataService = dataService;

        vm.origin;
        vm.current;
        vm.save = save;
        vm.cancel = cancel;
        vm.delete = deleteItem;

        if ($routeParams.id == 0)
            create();
        else
            getPatient($routeParams.id);

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
                    if ($routeParams.id == 0)
                        $location.path("/edit/" + vm.current.id);
                    else {
                        angular.extend(vm.origin, response.data);
                        reset();
                    }
                })
        }

        function deleteItem(id) {
            if (id == 0) return;
            dataService.post(model.deletePatientUrl + id, null,
                function () {
                    $timeout(function () {
                        // 呼叫 parent controller: patientController.js 的 $scope.getPage()，讓 patientController.js refresh 頁面
                        $scope.getPage();
                        $window.history.back();
                    }, 0);
                })
        }

        function create() {
            vm.current = { id: 0 };
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