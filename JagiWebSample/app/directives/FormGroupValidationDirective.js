(function () {
    'use strict';

    window.app.directive('formGroupValidation', formGroupValidation);

    function formGroupValidation() {
        return {
            require: '^form',
            replace: true,
            transclude: true,
            template:
				'<div class="has-feedback" ng-class="vm.getValidationClass()" uib-tooltip="{{vm.errorMessages}}">' +
					'<ng-transclude></ng-transclude>' +
					'<input-validation-icons field="vm.field" message="{{vm.errorMessages}}"></input-validation-icons>' +
				'</div>',
            scope: {
                field: '@formGroupValidation'
            },
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
        vm.getValidationClass = getValidationClass;

        function getValidationClass() {
            if (!canBeValidated()) return '';

            if (isValid()) return 'has-success';

            return 'has-error';
        }

        function canBeValidated() {
            //if (!$scope.form[vm.field].$dirty)
            //    return false;
            return ($scope.form[vm.field].$touched || $scope.form.$submitted);
        }

        function isValid() {
            var field = $scope.form[vm.field];
            var isValid = field.$valid;
            if (!isValid) {
                var errorMessages = [];
                var messageString = $("input[name='" + vm.field + "'").attr('message');
                if (!messageString)
                    return;

                var messages = angular.fromJson(messageString);
                var errors = Object.keys(field.$error);
                for (var i = 0; i < errors.length; i++) {
                    var message = messages[errors[i]] ? messages[errors[i]].message : null;
                    if (message)
                        errorMessages.push(message);
                }
                vm.errorMessages = errorMessages.join('\n');
            }
            return isValid;
        }
    }

})();