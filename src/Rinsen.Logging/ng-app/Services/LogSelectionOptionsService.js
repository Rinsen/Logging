(function () {
    'use strict';

    angular
        .module('app')
        .factory('LogSelectionOptionsService', LogSelectionOptionsService);

    LogSelectionOptionsService.$inject = ['$http'];

    function LogSelectionOptionsService($http) {
        var service = {
            getOptions: getOptions
        };

        return service;

        function getOptions() {
            return JSON.parse(document.getElementById("selectionOptions").getAttribute("data-json"));
        }
    }
})();