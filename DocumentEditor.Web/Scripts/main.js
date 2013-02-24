require.config({
    paths: {
        "angular": "//ajax.googleapis.com/ajax/libs/angularjs/1.0.5/angular",
        "jquery": "//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min",
        "signalR" :"jquery.signalR-1.0.0.min.js"
    },
    shim: {
       "angular": {
           exports: "angular"
       },
       "libs/diff-match-patch": {
           exports: 'diff_match_patch'
       },
       "libs/angular-ui" : {
           exports: "angular",
           deps: ["angular"]
       },
       "jquery": {
           exports:"$"
       },
       "libs/rangy-selectionsaverestore": {
           exports: "rangy",
           deps: ["libs/rangy-core"]
       },
       "signalR": {
           exports: "$.connection",
           deps : ["jquery"]
       }
    },
    baseUrl : '/scripts'
});

require([
        "angular",
        "controllers/documentEditorController",
        "libs/angular-ui"
    ], function(angular, controller) {
        var app = angular.module('editor', ['ui']);
        app.controller('documentEditorController', ['$scope', controller]);
        
        angular.bootstrap(document,['editor']);
    });