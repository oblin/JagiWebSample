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
        vm.changeCounty = changeCounty;
        vm.changeZip = changeZip;
        vm.changeRealm = changeRealm;
        vm.changeVillage = changeVillage;

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
                    resetCurrentAddress();
                });
            }
        }

        function changeCounty(county) {
            if (county) {
                dataService.get(model.addrCountyUrl + county, null, function (response) {
                    resetAddress();
                    vm.realms = response.data.realms;
                    //vm.villages = response.data.villages;
                    resetCurrentAddress()
                });
            }
        }

        function changeRealm(realm) {
            if (realm) {
                dataService.get(model.addrRealmUrl + realm, null, function (response) {
                    vm.villages = response.data.villages;
                });
            }
        }

        function changeVillage(village) {
            if (village) {
                var addr = { county: vm.current.county, realm: vm.current.realm, village: vm.current.village };
                dataService.get(model.addrVillageUrl, addr, function (response) {
                    vm.current.mailno = response.data.zip;
                })
            }
        }

        /**
         * 檢查 vm.current address 是否在 vm.realms & vm.villages，如果不是，則將其清空
         */
        function resetCurrentAddress() {
            if (vm.realms.indexOf(vm.current.realm) < 0)
                vm.current.realm = "";
            if (vm.villages.indexOf(vm.current.village) < 0)
                vm.current.village = "";
        }

        /**
         * 將 vm.realms & villages 的選項清空；但如果 vm.current 對應的 address 是有數值，則將
         * 其放入到 vm.realms & villages 中，可以正常顯示
         * @param {type} realm
         * @param {type} village
         */
        function resetAddress() {
            vm.realms = [];
            if (vm.current.realm)
                vm.realms.push(vm.current.realm);

            vm.villages = [];
            if (vm.current.village)
                vm.villages.push(vm.current.village);
        }
    }
})();