(function () {
    'use strict';

    window.app.controller("patientEditController", patientEditController);
    patientEditController.$inject = ['$scope', 'model', '$routeParams', 'dataService'];

    function patientEditController($scope, model, $routeParams, dataService) {
        var vm = this;
        vm.dataService = dataService;
        vm.id = $routeParams.id;
        vm.current;

        getPatient(vm.id);

        function getPatient(id) {
            var url = model.getPatientUrl;

            dataService.get(url + id, null, function (response) {
                vm.current = response.data;
            })
        }
    }
})();