(function () {
    'use strict';

    angular
        .module('app')
        .controller('LoggerController', LoggerController);

    LoggerController.$inject = ['$location', 'LogSelectionOptionsService'];

    function LoggerController($location, logSelectionOptionsService) {
        /* jshint validthis:true */
        var vm = this;

        activate();

        function activate() {
            vm.options = logSelectionOptionsService.getOptions();
        }
    }
})();
