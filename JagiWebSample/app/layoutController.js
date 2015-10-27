(function () {
    'use strict';
    window.app.controller("layoutController", layoutController);
    layoutController.$inject = ['dataService'];
    function layoutController(dataService) {
        var vm = this;
        vm.modelStatus = dataService;
    }
})();