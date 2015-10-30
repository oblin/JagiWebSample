(function () {
    'use strict';

    window.app.controller("patientsController", patientsController);
    patientsController.$inject = ['$scope', 'model', 'dataService'];

    function patientsController($scope, model, dataService) {
        var vm = this;
        var paginationOptions = model.paginationOptions;
        var headers = model.headers;
        vm.modelStatus = dataService;

        vm.gridOptions = {
            paginationPageSizes: [15, 20, 25, 50],
            paginationPageSize: paginationOptions.pageSize,
            useExternalPagination: true,
            useExternalSorting: true,
            enableFiltering: true,
            columnDefs: [
                { name: 'name', title: headers['name'] },
                { name: 'chartId', title: headers['chartId'] },
                { name: 'idCard', title: headers['idCard'] },
                { name: 'birthDay', title: headers['birthDay'] },
            ],
            onRegisterApi: function (gridApi) {
                vm.gridApi = gridApi;
                vm.gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                    if (sortColumns.length == 0) {
                        paginationOptions.sort = null;
                    } else {
                        paginationOptions.sort = sortColumns[0].sort.direction;
                        paginationOptions.sortField = sortColumns[0].field;
                    }
                    getPage();
                });
                gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {
                    paginationOptions.pageNumber = newPage;
                    paginationOptions.pageSize = pageSize;
                    getPage();
                });
            }
        };

        getPage();

        function getPage() {
            vm.gridOptions.data = model.data;
        }
    }
})();