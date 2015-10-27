(function () {
    'use strict';

    window.app.controller("addressController", addressController);

    addressController.$inject = ['$scope', 'model', 'dataService'];

    function addressController($scope, model, dataService) {
        var vm = this;
        vm.modelStatus = dataService;

        var paginationOptions = {  /* Mapping to PageInfo.cs */
            pageNumber: 1,
            pageSize: 15,
            sort: null
        };

        vm.gridOptions = {
            paginationPageSizes: [15, 20, 25, 50],
            paginationPageSize: 25,
            useExternalPagination: true,
            useExternalSorting: true,
            columnDefs: [
              { name: 'zip' },
              { name: 'county' },
              { name: 'realm' },
              { name: 'street', enableSorting: false }
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
            var url = model.getAddressUrl;

            dataService.get(url, paginationOptions, function (response) {
                vm.gridOptions.data = response.data.data;
                vm.gridOptions.totalItems = response.data.totalCount;
            });

            //$http.get(url)
            //.success(function (data) {
            //    $scope.gridOptions.totalItems = 100;
            //    var firstRow = (paginationOptions.pageNumber - 1) * paginationOptions.pageSize;
            //    $scope.gridOptions.data = data.slice(firstRow, firstRow + paginationOptions.pageSize);
            //});
        }
    }
})();