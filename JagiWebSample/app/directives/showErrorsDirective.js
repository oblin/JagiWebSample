(function () {
    'use strict';

    window.app.directive('showErrors', showErrors);

    function showErrors() {
        return {
            restrict: "E",
            scope: {
                modelStatus: '='
            },
            replace: true,
            template:
                '<div ng-show="modelStatus.isValid == false" class="alert alert-danger">' +
                '    <h4 class="alert-heading">資料輸入錯誤!</h4>                      ' +
                '    <ul data-ng-repeat="e in modelStatus.errors">                                 ' +
                '        <li>{{ e }}</li>                                              ' +
                '    </ul>                                                             ' +
                '</div>                                                                '
        }
    }
})()