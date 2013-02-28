define([
    "libs/diff-match-patch"
    ],
    function (diff_match_patch) {
        var dmp = new diff_match_patch();
        var directive = function () {
            return {
                restrict: 'E',
                replace: true,
                scope:{
                    text: "="
                },
                link: function (scope, elem, attrs) {                    
                    elem.attr("contenteditable", true);
                    var editFn = scope.$parent[attrs.onEdited]
                    elem.html(scope.text);
                    scope.$watch('text', function () {
                        var currentText = elem.html();
                        selection = rangy.saveSelection();
                        console.log("selection " + JSON.stringify(selection.rangeInfos) + " saved");
                        var textWithSelection = elem.html();
                        selectionPatch = dmp.patch_make(currentText, textWithSelection);
                        elem.html(scope.text);
                        var textAfterEdit = elem.html();
                        elem.html(dmp.patch_apply(selectionPatch, textAfterEdit)[0]);
                        rangy.restoreSelection(selection);
                        console.log("selection " + JSON.stringify(selection.rangeInfos) + " restored");
                        rangy.removeMarkers(selection);
                    });                 

                    elem.bind('keyup blur', function () {                      
                        var oldValue = scope.text;
                        editFn(oldValue, elem.html());
                    });                  
                }
            };
        }
        return directive;
    });