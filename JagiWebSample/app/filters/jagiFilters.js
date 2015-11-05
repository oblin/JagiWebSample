(function () {
    'use strict';

    window.app.filter('codeDesc', function () {
        return function (input, key, codes) {
            if (codes) {
                var details = codes[key];
                if (details) {
                    var desc = details[input];
                    if (desc)
                        return desc;
                }
            }
        }

        return input;
    })
})();