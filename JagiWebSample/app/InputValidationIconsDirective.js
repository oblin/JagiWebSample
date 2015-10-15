(function () {
    'use strict';

    window.app.directive('inputValidationIcons', inputValidationIcons);

    function inputValidationIcons() {
        return {
            require: '^form',
            scope: {
                field: '='
            },
            template:
				'<span ng-show="vm.canBeValidated() && vm.isValid()" ' +
					'class="fa fa-lg fa-check-square form-control-feedback" style="margin-top: 6px; margin-right: -6px"></span>' +
				'<span ng-show="vm.canBeValidated() && !vm.isValid()" ' +
					'class="fa fa-lg fa-exclamation-triangle form-control-feedback" style="margin-top: 6px; margin-right: -6px"></span>',
            controller: controller,
            controllerAs: 'vm',
            link: function (scope, element, attrs, formCtrl) {
                scope.form = formCtrl;
            }
        }
    }

    controller.$inject = ['$scope'];
    function controller($scope) {
        var vm = this;

        vm.field = $scope.field;
        vm.canBeValidated = canBeValidated;
        vm.isValid = isValid;

        function canBeValidated() {
            if (!$scope.form[vm.field].$dirty)
                return false;
            return ($scope.form[vm.field].$touched || $scope.form.$submitted);
        }

        function isValid() {
            return $scope.form[vm.field].$valid;
        }
    }
})();