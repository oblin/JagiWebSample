(function () {
    'use strict';

    window.app.directive('clickConfirm', clickConfirm);

    /**
     * 提供顯示 confirm modal 畫面，當使用者按下確認後，才會執行
     * 標準作法：<button class="btn" click-confirm="vm.delete(item)" item="vm.current" >
     * 其中 click-confirm 代表原本要使用在 ng-click function，item 則代表要傳入到該函數的參數
     * @param $modal
     */
    function clickConfirm($modal) {
        var ModalInstanceCtrl = function ($scope, $modalInstance) {
            $scope.ok = function () {
                $modalInstance.close();
            };

            $scope.cancel = function () {
                $modalInstance.dismiss('cancel');
            };
        };

        return {
            restrict: "A",
            scope: {
                clickConfirm: "&",
                item: "="
            },
            link: function (scope, element, attrs) {
                element.bind('click', function () {
                    var message = attrs.confirmMessage || "確定要刪除此筆資料？";
                    var modalHtml =
                        '<div class="modal-body">' + message + '</div>' +
                        '<div class="modal-footer">' +
                        '<button class="btn btn-primary" ng-click="ok()" icon="fa-check"> 確定</button>' +
                        '<button class="btn btn-warning" ng-click="cancel()" icon="fa-close"> 取消</button></div>';

                    var modalInstance = $modal.open({
                        template: modalHtml,
                        controller: ModalInstanceCtrl,
                        windowClass: 'modal-margin-vertical'
                    });
                    modalInstance.result.then(function () {
                        scope.clickConfirm({ item: scope.item }); //raise an error : $digest already in progress
                    }, function () {
                        // Do something while modal dismissed
                    });
                })
            }
        }
    }
})()