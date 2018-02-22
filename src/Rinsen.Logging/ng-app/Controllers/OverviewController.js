(function () {
    'use strict';

    angular
        .module('app')
        .controller('OverviewController', OverviewController);

    OverviewController.$inject = ['$location'];

    function OverviewController($location) {
        /* jshint validthis:true */
        var ovm = this;
        ovm.title = 'OverviewController';

        activate();

        function activate() { }
    }
})();
