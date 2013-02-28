require.config({
    paths: {
        "angular": "//ajax.googleapis.com/ajax/libs/angularjs/1.0.5/angular",
        "jquery": "//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min",
        "signalR": "jquery.signalR-1.0.0.min"
    },
    shim: {
        "angular": {
            exports: "angular"
        },
        "libs/diff-match-patch": {
            exports: 'diff_match_patch'
        },        
        "jquery": {
            exports: "$"
        },     
        "libs/rangy-selectionsaverestore": {
            deps: ["libs/rangy-core"],
            exports: "rangy"
        },
        "signalR": {
            exports: "$.connection",
            deps: ["jquery"]
        }
    },
    baseUrl: '/scripts'
});

require([
        "angular",
        "controllers/documentEditorController",
        "view/textEditorDirective",
    ], function(angular, controller,editor) {
        var app = angular.module('editor',[]);
        app.controller('documentEditorController', ['$scope', controller]);
        app.directive("texteditor", editor);
        angular.bootstrap(document,['editor']);
    });