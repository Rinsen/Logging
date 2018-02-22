(function () {
    'use strict';

    angular
        .module('app')
        .controller('OtherSpecificController', OtherSpecificController);

    OtherSpecificController.$inject = ['$location'];

    function OtherSpecificController($location) {
        /* jshint validthis:true */
        var osvm = this;
        osvm.title = 'OtherSpecificController';

        activate();

        function activate() { }
    }
})();
