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

        // for address
        vm.countries = model.countries;
        vm.realms;
        vm.changeZip = changeZip;

        if ($routeParams.id == 0)
            create();
        else
            getPatient($routeParams.id);

        function getPatient(id) {
            var url = model.getPatientUrl;

            dataService.get(url + id, null, function (response) {
                vm.origin = response.data;
                vm.current = angular.copy(vm.origin);
                resetAddress();
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

        function changeZip(oldValue) {
            var currentValue = $scope.patientEditForm.Mailno.$viewValue;
            if (currentValue != oldValue) {
                dataService.get(model.addrZipUrl + currentValue, null, function (response) {
                    vm.current.county = response.data.county;
                    vm.current.realm = response.data.realm;
                    vm.current.village = "";
                    resetAddress(response.data.realm);
                    vm.villages = response.data.villages;
                });
            }
        }

        function resetAddress(realm, village) {
            vm.realms = [];
            realm = realm ? realm : vm.current.realm;
            if (realm)
                vm.realms.push(realm);

            vm.villages = [];
            village = village ? village : vm.current.village;
            if (village)
                vm.villages.push(village);
        }
    }
})();