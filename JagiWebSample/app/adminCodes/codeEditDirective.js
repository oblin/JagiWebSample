(function () {
    'use strict';

    window.app.directive('codeEdit', codeEdit);

    function codeEdit() {
        return {
            scope: {
                options: '='        // parent scope 設定並傳入的參數
            },
            templateUrl: '/adminCodes/template/codeEdit.tmpl.cshtml',
            controller: controller,
            controllerAs: 'vm'
        }
    }

    controller.$inject = ['$scope', 'codeService', 'alerts'];

    function controller($scope, codeService, alerts) {
        var vm = this;
        var backup;

        vm.options = $scope.options;

        // Monitor Parent Scope vm.current 
        $scope.$parent.$watch('vm.current', function (value) {
            if (vm.current != value) {
                vm.current = value;
                backup = angular.copy(value);
                $scope.codeFileForm.$setPristine();
                $scope.codeFileForm.$setUntouched();
            }
        })

        $scope.$watch('codeFileForm.$dirty', function (newValue) {
            $scope.$parent.vm.current.isDirty = newValue;
        })

        vm.save = save;
        vm.saving = false;
        vm.create = create;
        vm.cancel = cancel;

        function save(item) {
            vm.saving = true;
            if (item.id && item.id > 0) {
                codeService.update(item)
                    .error(function (data) {
                        alerts.ajaxError(data);
                    })
                    .finally(function () {
                        vm.saving = false;
                        reset();
                    });
            } else {
                codeService.add(item)
                    .success(function (data) {
                        $scope.$parent.vm.pagedList.list.unshift(data);
                    })
                    .finally(function () {
                        vm.saving = false;
                        reset();
                    });
            }
        }

        function create() {
            vm.current = { id: 0 };
        }

        /**
         * 使用 angular.extend() copy backup object to $scope.$parent.vm.current 
         * 讓 two-way binding 可以正確產生
         * 另外使用 $scope.$apply() 才會造成 $scope.$watch('codeFileForm.$dirty') 正確被觸發，否則
         * 不會引起 angular 的內部 event
         */
        function cancel() {
            vm.current = backup;
            angular.extend($scope.$parent.vm.current, backup);
            reset();
        }

        function reset() {
            $scope.codeFileForm.$setPristine();
            $scope.codeFileForm.$setUntouched();
            $scope.$parent.vm.current.isDirty = false;
        }
    }
})();