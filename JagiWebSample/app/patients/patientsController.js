(function () {
    'use strict';

    window.app.controller("patientsController", patientsController);
    patientsController.$inject = ['$scope', 'model', 'gridConstans', 'dataService', '$controller'];

    function patientsController($scope, model, gridConstans, dataService, $controller) {
        var vm = this;
        vm.modelStatus = dataService;
        $controller('pagedGridController', { $scope: $scope });
        $scope.paginationOptions.url = model.getPatientUrl;
        $scope.paginationOptions.filters = [
            { field: "Name", keyword: "" },
            { field: "ChartId", keyword: "" },
            { field: "idCard", keyword: "" }
        ];

        var headers = model.headers;
        $scope.gridOptions.columnDefs = [
            {
                name: 'name', title: headers['name'],
                filterHeaderTemplate:
                    gridConstans.filterHeaderTemplate.format("paginationOptions.filters[0]", "getPage")
            },
            {
                name: 'chartId', title: headers['chartId'],
                filterHeaderTemplate:
                    gridConstans.filterHeaderTemplate.format("paginationOptions.filters[1]", "getPage")
            },
            {
                name: 'idCard', title: headers['idCard'],
                filterHeaderTemplate:
                    gridConstans.filterHeaderTemplate.format("paginationOptions.filters[2]", "getPage")
            },
            { name: 'birthDay', title: headers['birthDay'], enableFiltering: false },
        ];

        init();

        function init() {
            $scope.gridOptions.data = model.data;
        }
    }
})();