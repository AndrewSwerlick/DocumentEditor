define(["currentModel",
        "libs/diff-match-patch",
        "jquery",
        "libs/rangy-selectionsaverestore",
        "signalR"
    ], function(currentDoc, diff_match_patch, $,rangy,signalR) {
        var controller = function(scope) {
            var dmp = new diff_match_patch();
            window.testDoc = currentDoc;
            scope.doc = currentDoc;

            var connection = signalR("signalR/documents/subscriptions");
            connection.received(function (data) {
                scope.doc.applyRemoteRevision(data);
            });
            
            var selectionPatch;
            var selection;
            scope.doc.on("contentChanging", function() {
                var currentText = $("#doc").html();
                selection = rangy.saveSelection();
                var textWithSelection = $("#doc").html();
                selectionPatch = dmp.patch_make(currentText, textWithSelection);
            });
            scope.doc.on("contentChanged", function() {
                scope.$apply();
                var textAfterEdit = $("#doc").html();
                $("#doc").html(dmp.patch_apply(selectionPatch, textAfterEdit)[0]);
                rangy.restoreSelection(selection);
            });

            scope.doc.on("edited", function (revision) {
                var data = {
                    "DocumentId": scope.doc.id,
                    "RevisionId": revision.id,
                    "Patches": revision.patch,
                    "ParentRevisionId" : revision.revisionAppliedTo
                };
                $.ajax({
                    type: 'PUT',
                    url: "/api/documents/" + scope.doc.id,
                    data: JSON.stringify(data)
                });
            });

            var timer;
            scope.keyup = function(event) {
                if (timer)
                    clearTimeout(timer);
                timer = setTimeout(function() {
                    var currentText = event.target.innerHTML;                   
                    var patch = dmp.patch_make(scope.doc.content, currentText);
                    scope.doc.edit(patch);                   
                }, 1000);
            };
            
        };
        return controller;
    });