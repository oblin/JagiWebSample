(function () {
    'use strict';

    window.app.directive('input', input);

    /**
     * 處理 IE browser 的 input element 後面會有一個 X clear text button
     * 這個 button 不會觸發 angular ngModel，這個 directive 判斷 $input.val() 是否為空
     * 如果是，則將 angular 對應的 $input 值刪除
     */
    function input() {
        return {
            restrict: 'E',
            scope: {},
            link: function (scope, elem, attrs) {

                // Only care about textboxes, not radio, checkbox, etc.
                var validTypes = /^(search|email|url|tel|number|text)$/i;
                if (!validTypes.test(attrs.type)) return;

                // Bind to the mouseup event of the input textbox.  
                elem.bind('mouseup', function () {

                    // Get the old value (before click) and return if it's already empty
                    // as there's nothing to do.
                    var $input = $(this), oldValue = $input.val();
                    if (oldValue === '') return;

                    // Check new value after click, and if it's now empty it means the
                    // clear button was clicked. Manually trigger element's change() event.
                    setTimeout(function () {
                        var newValue = $input.val();
                        if (newValue === '') {
                            angular.element($input).change();
                        }
                    }, 1);
                });
            }
        }
    }
})();