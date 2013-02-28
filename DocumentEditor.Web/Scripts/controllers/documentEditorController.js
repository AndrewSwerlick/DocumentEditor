define(["currentModel",
        "libs/diff-match-patch",
        "jquery",
        "signalR"
    ], function(currentDoc, diff_match_patch, $,signalR) {
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
            scope.docEdited = function (oldValue, newValue) {
                if (newValue == oldValue)
                    return;
                if (timer)
                    clearTimeout(timer);
                timer = setTimeout(function () {
                    var currentText = newValue;
                    var patch = dmp.patch_make(scope.doc.content, newValue);
                    scope.doc.edit(patch);
                }, 1000);
            };
            
        };
        return controller;
    });