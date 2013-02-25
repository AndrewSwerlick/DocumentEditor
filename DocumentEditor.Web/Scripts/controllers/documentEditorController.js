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

            var connection = signalR("/signalR/documents/subscriptions");
            connection.received(function (data) {
                console.log("recieved data from server:" + JSON.stringify(data));
                if (data.id) {
                    scope.doc.applyRemoteRevision(data);
                    scope.$apply();
                }
            });
            connection.start().then(function() {
                connection.send(scope.doc.id);
            });
            connection.error(function(error) {
                console.warn(error);
            });

            var selectionPatch;
            var selection;
            scope.doc.on("contentChanging", function() {
                var currentText = $("#doc").html();
                selection = rangy.saveSelection();
                console.log("selection " + JSON.stringify(selection.rangeInfos) + " saved");
                var textWithSelection = $("#doc").html();
                selectionPatch = dmp.patch_make(currentText, textWithSelection);
            });
            scope.doc.on("contentChanged", function() {
                scope.$apply();
                var textAfterEdit = $("#doc").html();
                $("#doc").html(dmp.patch_apply(selectionPatch, textAfterEdit)[0]);
                rangy.restoreSelection(selection);
                console.log("selection " + JSON.stringify(selection.rangeInfos) + " restored");
                rangy.removeMarkers(selection);
            });

            scope.doc.on("edited", function (revision) {
                var data = {
                    "DocumentId": scope.doc.id,
                    "RevisionId": revision.id,
                    "Patches": revision.patch,
                    "ParentRevisionId" : revision.revisionAppliedTo
                };
                console.log("preparing to send edit " + JSON.stringify(data));

                $.ajax({
                    type: 'PUT',
                    url: "/api/documents/" + scope.doc.id,
                    data: JSON.stringify(data)                    
                }).done(function() {
                    console.log("revision " + revision.id + " sent");
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