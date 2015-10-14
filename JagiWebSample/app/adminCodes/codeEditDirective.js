(function () {
    'use strict';

    window.app.directive('codeEdit', codeEdit);

    function codeEdit() {
        return {
            scope: {
                code: '='
            },
            templateUrl: '/adminCodes/templates/codeEdit.tmpl.cshtml',
            controller: controller,
            controllerAs: 'vm'
        }
    }

    controller.$inject = ['$scope'];

    function controller($scope) {
        var vm = this;

        vm.current = $scope.current;
    }
})();