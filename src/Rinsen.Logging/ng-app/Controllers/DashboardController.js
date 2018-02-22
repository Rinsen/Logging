(function () {
    'use strict';

    angular
        .module('app')
        .controller('DashboardController', DashboardController);

    DashboardController.$inject = ['$location'];

    function DashboardController($location) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'DashboardController';
        vm.selected = {
            overview: true,
            specific: false,
            otherSpecific: false,
            reset: function () {
                this.overview = false;
                this.specific = false;
                this.otherSpecific = false;
            }
        };

        vm.select = function (id) {
            vm.selected.reset();
            switch (id) {
                case 'overview':
                    vm.selected.overview = true;
                    break;
                case 'specific':
                    vm.selected.specific = true;
                    break;
                case 'otherSpecific':
                    vm.selected.otherSpecific = true;
                    break;
                default:
            } 
        }

        activate();

        function activate() { }
    }
})();
