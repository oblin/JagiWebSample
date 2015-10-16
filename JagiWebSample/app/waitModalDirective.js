(function () {
    'use strict';

    window.app.directive('waitModal', waitModal);

    function waitModal() {
        return {
            restrict: "E",
            replace: true,
            template:
            '<div class="modal fade" id="loading" tabindex="-1" role="dialog"' +
                'aria-labelledby="loadingLabel" aria-hidden="true" style="margin-top: 25%">' +
            '<div class="modal-dialog" role="document">' +
                '<div class="modal-content">' +
                    '<div class="modal-body">' +
                        '<h3 style="text-align: center">' +
                            '處理中，請稍候... &nbsp &nbsp   <i class="fa fa-3x fa-spinner fa-pulse"></i>' +
                        '</h3>' +
                    '</div>' +
                '</div>' +
            '</div>' +
            '</div>'
        }
    }
})()