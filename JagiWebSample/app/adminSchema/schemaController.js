(function () {
    'use strict';

    window.app.controller('schemaController', schemaController);
    schemaController.$inject = ['model', 'dataService'];

    function schemaController(model, dataService) {
        var vm = this;
        vm.tableName = model.tableName;
        vm.tableNames = model.tableNames;
        vm.list = [];
        vm.change = getSchemaList;

        getSchemaList();

        function getSchemaList(tableName) {
            dataService.get(model.listUrl + tableName, null,
                function (response) {
                    vm.list = response.data;
                }
            );
        }
    }
})();