(function () {
    'use strict';

    angular
        .module('app')
        .controller('LoggerController', LoggerController);

    LoggerController.$inject = ['$location', 'LogSelectionOptionsService'];

    function LoggerController($location, logSelectionOptionsService) {
        /* jshint validthis:true */
        var vm = this;
        vm.show = show;

        function show() {
            var searchModel = {
                from: vm.options.from,
                to: vm.options.to,
                logLevels: [],
                logEnvironments: [],
                logApplications: []
            };

            vm.options.logLevels.forEach(function (logLevel) {
                if (logLevel.selected) {
                    searchModel.logLevels.push(logLevel.level)
                }
            });

            vm.options.logEnvironments.forEach(function (logEnvironment) {
                if (logEnvironment.selected) {
                    searchModel.logEnvironments.push(logEnvironment.id)
                }
            });

            vm.options.logApplications.forEach(function (logApplication) {
                if (logApplication.selected) {
                    searchModel.logApplications.push(logApplication.id)
                }
            });

            logSelectionOptionsService.getLogs(searchModel);
        }
        

        activate();

        function activate() {
            vm.options = logSelectionOptionsService.getOptions();
        }
    }
})();
