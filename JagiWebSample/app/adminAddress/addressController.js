(function () {
    'use strict';

    window.app.controller("addressController", addressController);

    addressController.$inject = ['$scope', 'model', '$modal', 'dataService'];

    function addressController($scope, model, $modal, dataService) {
        var vm = this;
        vm.modelStatus = dataService;

        var paginationOptions = {  /* Mapping to PageInfo.cs */
            pageNumber: 1,
            pageSize: 25,
            sort: null,
            searchKeyword: null,
            searchField: null
        };

        vm.paginationOptions = paginationOptions;
        vm.openEdit = openEdit;
        vm.deleteDetail = deleteDetail;
        vm.getPage = getPage;
        vm.filters = [
            { field: "Zip", keyword: "" },
            { field: "County", keyword: "" },
            { field: "Realm", keyword: "" }
        ];

        vm.gridOptions = {
            paginationPageSizes: [15, 20, 25, 50],
            paginationPageSize: 25,
            useExternalPagination: true,
            useExternalSorting: true,
            enableFiltering: true,
            columnDefs: [
              {
                  name: 'zip', title: '郵遞區號',
                  filterHeaderTemplate:
                    '<div class="ui-grid-filter-container">' +
                    '<input type="text" ng-model="grid.appScope.vm.filters[0].keyword" class="ui-grid-filter-input" />' +
                    '<span ng-click="grid.appScope.vm.getPage(grid.appScope.vm.filters[0])" class="btn btn-default fa fa-search ui-grid-filter-button"></span>' +
                    '</div>'
              },
              {
                  name: 'county', title: '縣市',
                  filterHeaderTemplate: 
                    '<div class="ui-grid-filter-container">' +
                    '<input type="text" ng-model="grid.appScope.vm.filters[1].keyword" class="ui-grid-filter-input" />' +
                    '<span ng-click="grid.appScope.vm.getPage(grid.appScope.vm.filters[1])" class="btn btn-default fa fa-search ui-grid-filter-button"></span>' +
                    '</div>'
              },
              { name: 'realm', title: '鄉鎮',
                  filterHeaderTemplate: 
                    '<div class="ui-grid-filter-container">' +
                    '<input type="text" ng-model="grid.appScope.vm.filters[2].keyword" class="ui-grid-filter-input" />' +
                    '<span ng-click="grid.appScope.vm.getPage(grid.appScope.vm.filters[2])" class="btn btn-default fa fa-search ui-grid-filter-button"></span>' +
                    '</div>'
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

        function getPage(filter) {
            var url = model.getAddressUrl;

            if (filter) {
                paginationOptions.searchField = filter.field;
                paginationOptions.searchKeyword = filter.keyword;
                for (var i = 0; i < vm.filters.length; i++)
                    if (vm.filters[i].field != filter.field)
                        vm.filters[i].keyword = null;
            }

            dataService.get(url, paginationOptions, function (response) {
                vm.gridOptions.data = response.data.data;
                vm.gridOptions.totalItems = response.data.totalCount;
            });
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

        function getKeywordInput(term, value, row, column) {
            paginationOptions.searchKeyword = term;
        }
    }
})();