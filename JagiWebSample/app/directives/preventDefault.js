(function(){
    'use strict';
    window.app.directive('preventDefault', preventDefault);

    function preventDefault(){
        return function(scope, element, attrs) {
            angular.element(element).bind('click', function(event) {
                event.preventDefault();
                event.stopPropagation();
            });
        }
    }
})();