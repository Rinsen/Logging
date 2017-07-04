﻿(function () {
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
            var options = JSON.parse(document.getElementById("selectionOptions").getAttribute("data-json"));

            options.from = new Date(options.from);
            options.to = new Date(options.to);

            return options;
        }
    }
})();