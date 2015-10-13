(function () {
    'use strict';

    window.app.controller('codeController', codeController);
    codeController.$inject = ['model'];

    function codeController(model) {
        var vm = this;
        vm.codes = model;
    }
})();