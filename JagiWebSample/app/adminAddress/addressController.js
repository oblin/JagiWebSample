(function () {
    'use strict';

    window.app.controller("addressController", addressController);

    addressController.$inject = ['model', 'dataService'];

    function addressController(model, dataService) {
        var vm = this;

        var paginationOptions = {  /* Mapping to PageInfo.cs */
            pageCount: 15,
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
                vm.gridApi.core.on.sortChanged(vm, function (grid, sortColumns) {
                    if (sortColumns.length == 0) {
                        paginationOptions.sort = null;
                    } else {
                        paginationOptions.sort = sortColumns[0].sort.direction;
                    }
                    getPage();
                });
                gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {
                    paginationOptions.currentPage = newPage;
                    paginationOptions.pageCount = pageSize;
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