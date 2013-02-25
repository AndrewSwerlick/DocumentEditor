define([
        "utils/guid",
        "libs/eventEmitter",
        "libs/heir",
        "libs/diff-match-patch"
    ], function(guid, eventEmitter, heir, diff_match_patch) {
        var Document = function(currentRemoteRevId, contents, id) {
            this.currentRevision = {};
            this.currentRemoteRevision = {};
            this.localrevisions = {};
            this.remoteRevisions = {};
            this.content = contents ? contents : "";
            this.id = id;
            this.__revisionCounter = 0;
            if (currentRemoteRevId) {
                this.currentRevision = { id: currentRemoteRevId }
                this.currentRemoteRevision = { id: currentRemoteRevId };
            }
        };
        Document.prototype =
        {
            edit: function(patch) {
                setContents.call(this, new diff_match_patch().patch_apply(patch, this.content)[0]);
                var id = guid.newGuid();
                this.localrevisions[id] = {
                    patch: patch,
                    id: id,
                    order: this.__revisionCounter,
                    revisionAppliedTo: this.currentRevision.id
                };
                this.currentRevision = this.localrevisions[id];
                this.__revisionCounter = this.__revisionCounter + 1;
                this.emit("edited", this.localrevisions[id]);
            },
            applyRemoteRevision: function (remoteRevision) {
                if (remoteRevision.failed)
                    manageFailedRevision.call(this,remoteRevision);
                else 
                    manageSucessfulRevision.call(this, remoteRevision);
                
            },
            getSortedLocalRevisions: function() {

                function mapObjToArray(object) {
                    var array = [];
                    for (var prop in object) {
                        array.push(object[prop]);
                    }
                    return array;
                }

                function sortRevisions(r1, r2) {
                    if (r1.order < r2.order)
                        return -1;
                    if (r1.order > r2.order)
                        return 1;
                    return 0;
                }

                return mapObjToArray(this.localrevisions).sort(sortRevisions);
            }
        };
        Document.inherit(eventEmitter);

        function updateContents() {
            setContents.call(this, this.currentRemoteRevision.content);
            var sortedRevisions = this.getSortedLocalRevisions();
            for (var ind in sortedRevisions) {
                var revision = sortedRevisions[ind];
                this.content = new diff_match_patch().patch_apply(revision.patch, this.content)[0];
            }
        }

        function manageFailedRevision(failureMessage) {
            var dmp = new diff_match_patch();
            var failedLocalRevision = this.localrevisions[failureMessage.id];
            var lastGoodRevision = this.currentRemoteRevision;
            var sortedLocalRevisions = this.getSortedLocalRevisions();
            if (sortedLocalRevisions[failedLocalRevision.order - 1])
                lastGoodRevision = sortedLocalRevisions[failedLocalRevision.order - 1];
            var content = lastGoodRevision ? lastGoodRevision.content : "";
            for (var i = failedLocalRevision.order; i < sortedLocalRevisions.length; i++) {
                var revision = sortedLocalRevisions[i];
                delete this.localrevisions[revision.id];
                this.edit(revision.patch);
            };
        }

        function manageSucessfulRevision(sucessMessage) {
            if (this.remoteRevisions[sucessMessage.id])
                return;
            this.remoteRevisions[sucessMessage.id] = sucessMessage;
            var parentRevisionAlreadyApplied = this.remoteRevisions[sucessMessage.revisionAppliedTo];
            var correspondingLocalRevision = this.localrevisions[sucessMessage.id];
            if (correspondingLocalRevision) {
                var sortedRevisions = this.getSortedLocalRevisions();
                for (var i = 0;
                    sortedRevisions[i] &&
                        sortedRevisions[i].order < correspondingLocalRevision.order + 1;
                    i++) {
                    var revision = sortedRevisions[i];
                    this.remoteRevisions[revision.id] = revision;
                    delete this.localrevisions[revision.id];
                }
            }
            var parentRevisionIsCurrentRemoteRevision = this.currentRemoteRevision.id == sucessMessage.revisionAppliedTo;
            if (parentRevisionAlreadyApplied && !parentRevisionIsCurrentRemoteRevision)
                return;
            this.currentRemoteRevision = sucessMessage;
            this.currentRevision = sucessMessage;
            updateContents.call(this);
        }

        function setContents(value) {
            this.trigger("contentChanging");
            this.content = value;
            this.trigger("contentChanged");
        }

        return Document;
    });