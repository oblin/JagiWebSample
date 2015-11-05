(function () {
    'use strict';

    window.app.controller("patientsController", patientsController);
    patientsController.$inject = ['$scope', '$location', 'model', 'gridConstants', 'dataService', '$filter', '$controller'];

    function patientsController($scope, $location, model, gridConstants, dataService, $filter, $controller) {
        var vm = this;
        vm.modelStatus = dataService;

        //#region UI Grid Settings
        vm.clearFilter = clearFilter;
        vm.openEdit = openEdit;
        vm.codes = model.codes;
        vm.status = model.status;

        $controller('pagedGridController', { $scope: $scope });
        $scope.paginationOptions.url = model.pagedPatientsUrl;
        $scope.paginationOptions.filters = [
            { field: "Name", keyword: "" },
            { field: "ChartId", keyword: "" },
            { field: "idCard", keyword: "" }
        ];
        $scope.paginationOptions.status = vm.status;

        var headers = model.headers;
        $scope.gridOptions.columnDefs = [
            {
                name: 'edit', displayName: '', width: '10%',
                enableColumnMenu: false, enableSorting: false, enableFiltering: false,
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
                name: 'status', title: headers['status'], enableFiltering: false, width: '20%',
                cellFilter: 'codeDesc: "status": grid.appScope.vm.codes'
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

            // test filter:
            var test = $filter('codeDesc')("2", "status", vm.codes);
        }

        function openEdit(item) {
            if (item)
                $location.path("/edit/" + item.id);
            else
                $location.path("/edit/0");
        }
        
        function clearFilter() {
            angular.extend($scope.paginationOptions, gridConstants.defaultPaginationOptions);
            for (var i = 0; i < $scope.paginationOptions.filters.length; i++) {
                $scope.paginationOptions.filters[0].keyword = "";
            }
            $scope.getPage();
        }
        //#endregion
    }
})();