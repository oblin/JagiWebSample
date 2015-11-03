(function () {
    'use strict';

    window.app.directive('codeEdit', codeEdit);

    function codeEdit() {
        return {
            scope: {
                options: '=',        // parent scope 設定並傳入的參數
                updateCurrent: "&",
                deleteCurrent: '&'
            },
            templateUrl: '/adminCodes/template/codeEdit.tmpl.cshtml',
            controller: controller,
            controllerAs: 'vm'
        }
    }

    controller.$inject = ['$scope', 'model', '$modal', 'dataService', 'validator', 'alerts'];

    function controller($scope, model, $modal, dataService, validator, alerts) {
        var vm = this;

        vm.options = $scope.options;    // reserved, not use
        vm.parent;                      // $parent 選擇的代碼  
        vm.current;                     // 目前正在編輯的代碼
        vm.details = [];                // details for table list

        // 處理 codeFile 的 CRUD
        vm.save = save;
        vm.create = create;
        vm.delete = deleteCode;
        vm.cancel = cancel;
        vm.saving = dataService.saving;
        // 處理 codeDetail 
        vm.detail = detail;
        vm.deleteDetail = deleteDetail;

        // Monitor Parent Scope vm.current 
        $scope.$parent.$watch('vm.current', function (value) {
            if (vm.parent != value) {
                vm.parent = value;
                dataService.get(model.getCodeUrl + value.id, null,
                    function (response) {
                        vm.details = response.data;
                    });
                vm.current = angular.copy(value);
                $scope.codeFileForm.$setPristine();
                $scope.codeFileForm.$setUntouched();
            }
        })

        /**
         * 當 form inputs 改變時候，設定 $parent.vm.current.isDirty
         * 讓 parent controller 可以判斷是否有被修改，進行如避免操作等行為設定
         * @param newValue 透過 $watch codeFileForm.$dirty 判斷任一input有被修改
         */
        $scope.$watch('codeFileForm.$dirty', function (newValue) {
            $scope.$parent.vm.current.isDirty = newValue;
        })

        /**
         * 提供新增或修改的存檔；其中，新增時候必須要將項目加入到 parent list 中，使用 updateCurrent() 
         * 讓 parent controller 中的 vm.current & list 都可以同時增加
         * @param item
         */
        function save(item) {
            validator.ValidateModel(item, model.codeValidations);
            dataService.isValid = item.isValid;
            if (!item.isValid) {
                dataService.errors = item.errors;
                return;
            }

            dataService.post(model.saveCodeUrl, item,
                function (response) {
                    if (item.id != 0)
                        angular.extend(vm.parent, response.data);
                    if (item.id == 0) {
                        $scope.updateCurrent({ item: response.data });
                    }
                }, null,
                function () {
                    if (!dataService.isValid)
                        angular.extend(vm.current, vm.parent);
                    reset();
                }
            );
        }

        function create() {
            vm.current = { id: 0, itemType: '', typeName: '' };
        }

        function deleteCode(item) {
            if (!item || item.id == 0)
                return;

            dataService.post(model.deleteCodeUrl + item.id, null,
                function () { $scope.deleteCurrent({ item: item }); });
        }

        /**
         * 使用 angular.extend() copy vm.parent object to $scope.$parent.vm.current 
         * 讓 two-way binding 可以正確產生
         * 另外使用 $scope.$apply() 才會造成 $scope.$watch('codeFileForm.$dirty') 正確被觸發，否則
         * 不會引起 angular 的內部 event
         */
        function cancel() {
            vm.current = vm.parent;
            reset();
        }

        function reset() {
            $scope.codeFileForm.$setPristine();
            $scope.codeFileForm.$setUntouched();
            $scope.$parent.vm.current.isDirty = false;
        }

        /**
         * 呼叫 detail modal 並將目前 scope 的內容傳入：
         *  parent: 代表 detail modal 的 parent，因此是目前的 vm.current
         *  detail: 代表目前選擇的 detail item  
         * @param item 使用者點選的 detail item
         */
        function detail(item) {
            if (item == null)
                item = { id: 0 };
            $modal.open({
                template: '<code-detail parent="parent" detail="detail" list="list" />',
                backdrop: false,
                scope: angular.extend($scope.$new(true),
                    { parent: vm.current, detail: item, list: vm.details })
            });
        }

        function deleteDetail(item) {
            if (item == null || item.id == 0)
                return;
            dataService.post(model.deleteDetailUrl + item.id, null,
                function () { vm.details.splice(vm.details.indexOf(item), 1); });
        }
    }
})();