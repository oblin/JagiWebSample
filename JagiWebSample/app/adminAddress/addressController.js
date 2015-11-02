(function () {
    'use strict';

    window.app.controller("addressController", addressController);

    addressController.$inject = ['$scope', 'model', '$modal', 'gridConstants', 'dataService', '$controller'];

    function addressController($scope, model, $modal, gridConstants, dataService, $controller) {
        var vm = this;
        vm.modelStatus = dataService;

        $controller('pagedGridController', { $scope: $scope });

        vm.openEdit = openEdit;
        vm.deleteDetail = deleteDetail;
        $scope.paginationOptions.url = model.getAddressUrl;
        $scope.paginationOptions.filters = [
            { field: "Zip", keyword: "" },
            { field: "County", keyword: "" },
            { field: "Realm", keyword: "" }
        ];
        $scope.gridOptions.columnDefs = [
              {
                  name: 'zip', title: '郵遞區號',
                  filterHeaderTemplate: gridConstants.filterHeaderTemplate.format("paginationOptions.filters[0]", "getPage")
              },
              {
                  name: 'county', title: '縣市',
                  filterHeaderTemplate: gridConstants.filterHeaderTemplate.format("paginationOptions.filters[1]", "getPage")
              },
              {
                  name: 'realm', title: '鄉鎮',
                  filterHeaderTemplate: gridConstants.filterHeaderTemplate.format("paginationOptions.filters[2]", "getPage")
              },
              { name: 'street', enableColumnMenu: false, enableSorting: false, enableFiltering: false },
              {
                  name: 'eidt', displayName: '', enableColumnMenu: false, enableSorting: false, enableFiltering: false,
                  cellTemplate:
                    '<button ng-click="grid.appScope.vm.openEdit(row.entity)" class="btn btn-primary" icon="fa-edit">編輯</button>  ' +
                    '<button class="btn btn-danger" click-confirm="grid.appScope.vm.deleteDetail(item) "' +
                        'confirm-message="確定要刪除這筆地址資料嗎？" ' +
                         'item="row.entity" icon="fa-trash">刪除</button>',
                  headerCellTemplate:
                      '<button ng-click="grid.appScope.vm.openEdit()" class="btn btn-success" icon="fa-plus-circle">新增地址</button> '
              }
        ];

        $scope.getPage();

        function openEdit(item) {
            if (item == null)
                item = { id: 0 };
            $modal.open({
                template: '<address-edit item="item" page-refresh="pageRefresh()"></address-edit>',
                backdrop: false,
                scope: angular.extend($scope.$new(true),
                    { item: item, pageRefresh: $scope.getPage })
            });

        }

        function deleteDetail(item) {
            dataService.post(model.deleteUrl + item.id, null, function (response) {
                $scope.getPage();
            })
        }

        function getKeywordInput(term, value, row, column) {
            paginationOptions.searchKeyword = term;
        }
    }
})();