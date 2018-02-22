(function () {
    'use strict';

    angular
        .module('app')
        .controller('SpecificController', SpecificController);

    SpecificController.$inject = ['$location'];

    function SpecificController($location) {
        /* jshint validthis:true */
        var svm = this;
        svm.title = 'SpecificController';

        activate();

        function activate() { }
    }
})();
