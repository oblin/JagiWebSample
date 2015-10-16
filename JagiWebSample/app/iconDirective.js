(function () {
    'use strict';
    window.app.directive('icon', icon);
    window.app.directive('iconLeft', iconLeft);

    function icon() {
        return {
            restrict: 'A',
            scope: {
                icon: '@'
            },
            link: function (scope, elem, attrs) {
                var iconTmpl = '<i class="fa ' + scope.icon + '"></i>'
                elem.append(iconTmpl);
            }
        }
    }

    function iconLeft() {
        return {
            restrict: 'A',
            scope: {
                iconLeft: '@'
            },
            link: function (scope, elem, attrs) {
                var iconTmpl = '<i class="fa ' + scope.iconLeft + '"></i>'
                elem.prepend(iconTmpl);
            }
        }
    }
})();