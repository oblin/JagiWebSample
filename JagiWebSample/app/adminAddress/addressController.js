(function () {
    'use strict';

    window.app.controller("addressController", addressController);

    addressController.$inject = ['$scope', 'model', '$modal', 'dataService'];

    function addressController($scope, model, $modal, dataService) {
        var vm = this;
        vm.modelStatus = dataService;

        var paginationOptions = {  /* Mapping to PageInfo.cs */
            pageNumber: 1,
            pageSize: 15,
            sort: null
        };

        vm.openEdit = openEdit;
        vm.deleteDetail = deleteDetail;

        vm.gridOptions = {
            paginationPageSizes: [15, 20, 25, 50],
            paginationPageSize: 25,
            useExternalPagination: true,
            useExternalSorting: true,
            columnDefs: [
              { name: 'zip' },
              { name: 'county' },
              { name: 'realm' },
              { name: 'street', enableColumnMenu: false, enableSorting: false },
              {
                  name: 'eidt', displayName: '', enableColumnMenu: false, enableSorting: false,
                  cellTemplate:
                    '<button ng-click="grid.appScope.vm.openEdit(row.entity)" class="btn btn-primary" icon="fa-edit">編輯</button>  ' +
                    '<button class="btn btn-danger" click-confirm="grid.appScope.vm.deleteDetail(item) "' +
                        'confirm-message="確定要刪除這筆地址資料嗎？" ' +
                         'item="row.entity" icon="fa-trash">刪除</button>',
                  headerCellTemplate: 
                      '<button ng-click="grid.appScope.vm.openEdit()" class="btn btn-success"  icon="fa-plus-circle">新增地址</button> '
            }
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

        function openEdit(item) {
            if (item == null)
                item = { id: 0 };
            $modal.open({
                template: '<address-edit item="item" page-refresh="pageRefresh()"></address-edit>',
                backdrop: false,
                scope: angular.extend($scope.$new(true),
                    { item: item, pageRefresh: getPage })
            });

        }

        function deleteDetail(item) {
            dataService.post(model.deleteUrl + item.id, null, function (response) {
                getPage();
            })
        }
    }
})();