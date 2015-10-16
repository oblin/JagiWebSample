(function () {
    'use strict';

    window.app.directive('codeDetail', codeDetail);

    function codeDetail() {
        return {
            scope: {
                detail: '=',
                parent: '='
            },
            templateUrl: '/adminCodes/template/detailEdit.tmpl.cshtml',
            controller: controller,
            controllerAs: 'vm'
        }
    }

    controller.$inject = ['$scope', 'codeService'];
    function controller($scope, codeService) {
        var vm = this;
        vm.detail = angular.copy($scope.detail);
        vm.errorMessage;
        vm.save = save;

        function save(item) {
            codeService.saveDetail(vm.detail, $scope.detail)
                .success(function () {
                    // close the modal
                    $scope.$parent.$close();
                })
                .error(function (data) {
                    vm.errorMessage = data;
                });
        }
    }
})();