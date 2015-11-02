(function () {
    'use strict';

    window.app.controller("patientsController", patientsController);
    patientsController.$inject = ['$scope', '$location', 'model', 'gridConstants', 'dataService', '$controller'];

    function patientsController($scope, $location, model, gridConstants, dataService, $controller) {
        var vm = this;
        vm.modelStatus = dataService;
        vm.clearFilter = clearFilter;
        vm.openEdit = openEdit;
        $controller('pagedGridController', { $scope: $scope });
        $scope.paginationOptions.url = model.pagedPatientsUrl;
        $scope.paginationOptions.filters = [
            { field: "Name", keyword: "" },
            { field: "ChartId", keyword: "" },
            { field: "idCard", keyword: "" }
        ];

        var headers = model.headers;
        $scope.gridOptions.columnDefs = [
            {
                name: 'edit', displayName: '', enableColumnMenu: false, enableSorting: false, enableFiltering: false,
                headerCellTemplate:
                    '<button ng-click="grid.appScope.vm.clearFilter()" class="btn btn-warning bottom" icon="fa-eraser">清除過濾</button>',
                cellTemplate:
                    '<button ng-click="grid.appScope.vm.openEdit(row.entity)" class="btn btn-primary" icon="fa-edit">編輯</button>'
            },
            {
                name: 'name', title: headers['name'],
                filterHeaderTemplate:
                    gridConstants.filterHeaderTemplate.format("paginationOptions.filters[0]", "getPage")
            },
            {
                name: 'chartId', title: headers['chartId'],
                filterHeaderTemplate:
                    gridConstants.filterHeaderTemplate.format("paginationOptions.filters[1]", "getPage")
            },
            {
                name: 'idCard', title: headers['idCard'], enableSorting: false,
                filterHeaderTemplate:
                    gridConstants.filterHeaderTemplate.format("paginationOptions.filters[2]", "getPage")
            },
            { name: 'birthDay', title: headers['birthDay'], enableFiltering: false },
        ];

        init();

        function init() {
            $scope.gridOptions.data = model.data;
        }

        function openEdit(item) {
            $location.path("/edit/" + item.id);
        }

        function clearFilter() {
            angular.extend($scope.paginationOptions, gridConstants.defaultPaginationOptions);
            for (var i = 0; i < $scope.paginationOptions.filters.length; i++) {
                $scope.paginationOptions.filters[0].keyword = "";
            }
            $scope.getPage();
        }
    }
})();