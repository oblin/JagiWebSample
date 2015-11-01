(function(){
    'use strict';
    window.app.controller('pagedGridController', pagedGridController);
    pagedGridController.$inject = ['$scope', 'dataService'];

    function pagedGridController($scope, dataService) {
        $scope.paginationOptions = {  
            pageNumber: 1,
            pageSize: 25,
            sort: null,
            searchKeyword: null,
            searchField: null,
            url: null
        };

        $scope.paginationOptions.filters = [];
        $scope.getPage = getPage;

        $scope.gridOptions = {
            paginationPageSizes: [15, 20, 25, 50],
            paginationPageSize: $scope.paginationOptions.pageSize,
            useExternalPagination: true,
            useExternalSorting: true,
            enableFiltering: true,
            columnDefs: [],
            onRegisterApi: function (gridApi) {
                $scope.gridApi = gridApi;
                $scope.gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                    if (sortColumns.length == 0) {
                        $scope.paginationOptions.sortField = null;
                        $scope.paginationOptions.sort = null;
                    } else {
                        $scope.paginationOptions.sort = sortColumns[0].sort.direction;
                        $scope.paginationOptions.sortField = sortColumns[0].field;
                    }
                    getPage();
                });
                gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {
                    $scope.paginationOptions.pageNumber = newPage;
                    $scope.paginationOptions.pageSize = pageSize;
                    getPage();
                });
            }
        };

        function getPage(filter) {
            var url = $scope.paginationOptions.url;

            if (filter) {
                $scope.paginationOptions.searchField = filter.field;
                $scope.paginationOptions.searchKeyword = filter.keyword;
                for (var i = 0; i < $scope.paginationOptions.filters.length; i++)
                    if ($scope.paginationOptions.filters[i].field != filter.field)
                        $scope.paginationOptions.filters[i].keyword = null;
            }

            dataService.get(url, $scope.paginationOptions, function (response) {
                $scope.gridOptions.data = response.data.data;
                $scope.gridOptions.totalItems = response.data.totalCount;
            });
        }
    }
})();