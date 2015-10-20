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

    controller.$inject = ['$scope', 'model', 'validator', 'dataService', 'codeService'];
    function controller($scope, model, validator,dataService, codeService) {
        var vm = this;
        vm.detail = angular.copy($scope.detail);
        if (vm.detail.id == 0)
            vm.detail.codeFileID = $scope.parent.id;

        vm.modelStatus = dataService;
        vm.saving = false;
        vm.save = save;
        vm.cancel = cancel;

        function save() {
            validator.ValidateModel(vm.detail, model.detailValidations);
            vm.modelStatus.isValid = vm.detail.isValid;
            if (!vm.detail.isValid) {
                vm.modelStatus.errors = vm.detail.errors;
                return;
            }

            vm.saving = true;

            dataService.post(model.detailSaveUrl, vm.detail,
                function (response) {
                    angular.extend($scope.detail, response.data);
                    $scope.$parent.$close();                // close the modal
                    if (vm.detail.id == 0) {
                        $scope.list.push($scope.detail);    // 處理新增，將回傳的加入到 list 中
                    }
                }, null,
                function () {
                    vm.saving = false;
                }
            );
        }

        function cancel() {
            $scope.$parent.$dismiss()
            vm.modelStatus.isValid = true;
        }
    }
})();