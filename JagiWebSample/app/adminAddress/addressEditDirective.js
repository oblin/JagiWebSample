(function () {
    'use strict';

    window.app.directive('addressEdit', addressEdit);

    function addressEdit() {
        return {
            scope: {
                item: '=',
                pageRefresh: '&'
            },
            templateUrl: '/adminAddress/template/addressEdit.tmpl.cshtml',
            controller: controller,
            controllerAs: 'vm'
        }
    }

    controller.$inject = ['$scope', 'model', '$modal', 'dataService', 'alerts'];

    function controller($scope, model, $modal, dataService, alerts) {
        var vm = this;

        vm.current = angular.copy($scope.item);
        vm.save = save;
        vm.cancel = cancel;
        vm.saving = dataService.saving;

        function save() {
            var url = model.updateUrl;

            dataService.post(url, vm.current,
                function (response) {
                    angular.extend($scope.item, response.data);
                }, null, function () {
                    cancel();
            })
        }

        function cancel() {
            $scope.$parent.$dismiss();
        }
    }
})();