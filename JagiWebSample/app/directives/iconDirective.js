(function () {
    'use strict';
    window.app.directive('icon', icon);
    window.app.directive('iconLeft', iconLeft);

    function icon() {
        return {
            restrict: 'A',
            // 移除，以避免造成其他 directive: asking for new/isolated scope issue
            //scope: {
            //    icon: '@'
            //},
            link: function (scope, elem, attrs) {
                var iconTmpl = ' <i class="fa ' + attrs.icon + '"></i>'
                elem.append(iconTmpl);
            }
        }
    }

    function iconLeft() {
        return {
            restrict: 'A',
            //scope: {
            //    iconLeft: '@'
            //},
            link: function (scope, elem, attrs) {
                var iconTmpl = '<i class="fa ' + attrs.iconLeft + '"></i> '
                elem.prepend(iconTmpl);
            }
        }
    }
})();