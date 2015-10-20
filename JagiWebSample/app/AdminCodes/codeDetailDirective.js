(function () {
    'use strict';

    window.app.directive('codeDetail', codeDetail);

    function codeDetail() {
        return {
            scope: {
                detail: '=',
                parent: '=',
                list: '='
            },
            templateUrl: '/adminCodes/template/detailEdit.tmpl.cshtml',
            controller: controller,
            controllerAs: 'vm'
        }
    }

    controller.$inject = ['$scope', 'detailValidations', 'validator', 'codeService'];
    function controller($scope, detailValidations, validator, codeService) {
        var vm = this;
        vm.detail = angular.copy($scope.detail);
        if (vm.detail.id == 0)
            vm.detail.codeFileID = $scope.parent.id;

        vm.errorMessage;
        vm.saving = false;
        vm.save = save;

        function save() {
            validator.ValidateModel(vm.detail, detailValidations);
            if (!vm.detail.isValid) {
                vm.errorMessage = vm.detail.errors;
                return;
            }

            vm.saving = true;
            codeService.saveDetail(vm.detail, $scope.detail)
                .success(function () {
                    // close the modal
                    $scope.$parent.$close();
                    if (vm.detail.id == 0) {
                        // 處理新增，將回傳的加入到 list 中
                        $scope.list.push($scope.detail);
                    }
                })
                .error(function (data) {
                    vm.errorMessage = data.errorMessages;
                })
                .finally(function(){
                    vm.saving = false;
                })
        }
    }
})();