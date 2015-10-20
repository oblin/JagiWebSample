(function () {
    'use strict';

    window.app.directive('showErrors', showErrors);

    function showErrors() {
        return {
            restrict: "E",
            scope: {
                errors: '='
            },
            replace: true,
            template:
                '<div ng-show="errors && errors.length > 0" class="alert alert-danger">' +
                '    <h4 class="alert-heading">資料輸入錯誤!</h4>                      ' +
                '    <ul data-ng-repeat="e in errors">                                 ' +
                '        <li>{{ e }}</li>                                              ' +
                '    </ul>                                                             ' +
                '</div>                                                                '
        }
    }
})()