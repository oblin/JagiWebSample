(function () {
    'use strict';

    window.app.directive('waitModal', waitModal);

    function waitModal($compile) {
        var template =
            '<div class="modal top-modal modal-margin-vertical" id="loading" tabindex="10" role="dialog"' +
                'aria-labelledby="loadingLabel" aria-hidden="true">' +
            '<div class="modal-dialog" role="document">' +
                '<div class="modal-content">' +
                    '<div class="modal-body">' +
                        '<h3 style="text-align: center">' +
                            '處理中，請稍候... &nbsp &nbsp   <i class="fa fa-3x fa-spinner fa-pulse"></i>' +
                        '</h3>' +
                    '</div>' +
                '</div>' +
            '</div>' +
            '</div>';

        return {
            restrict: "A, E",
            replace: true,
            link: function (scope, element, attrs) {
                var modal = angular.element(template);
                var linkFn = $compile(modal);
                element.after(modal);
                linkFn(scope);
            }
        }
    }
})()