define([
        "scripts/models/document",
        'scripts/utils/guid',
        'scripts/libs/diff-match-patch',
    ],
    function (Document, guid) {
        describe("Document Tests", function() {
            describe("when a new document is constructed", function() {
                var document = new Document();
                it("has a revisions collection", function() {
                    document.localrevisions.should.exist;
                });
            });
            describe("when we call the documents edit method with a patch", function() {
                var document = new Document();
                var patch = new diff_match_patch().patch_make("", "Test");
                var trackedEdits = [];
                document.on("edited", function(revision) {
                    trackedEdits.push(revision);
                });
                document.edit(patch);
                it("ensure it has a new revision", function() {
                    document.getSortedLocalRevisions().length.should.equal(1);
                });
                it("ensure it has updated contents", function() {
                    document.content.should.equal("Test");
                });
                it("ensure it fires it's edited event", function() {
                    trackedEdits.length.should.equal(1);
                });
            });
            describe("when we call the documents applyRemoteRevision " +
                "method with a remote revision message", function() {
                    var document = new Document();
                    var revision = {
                        id: guid.newGuid(),
                        patches: new diff_match_patch().patch_make("", "Test"),
                        content: "Test"
                    };
                    document.applyRemoteRevision(revision);
                    it("ensure it has a new revision", function() {
                        mapObjToArray(document.remoteRevisions).length.should.equal(1);
                    });
                    it("ensure it has updated contents", function() {
                        document.content.should.equal("Test");
                    });
                    it("ensure it updates the current remote reivsion", function() {
                        document.currentRemoteRevision.id.should.equal(revision.id);
                    });
                });
            describe("given a document with one local revision " +
                "and no remote revisons", function() {

                    function getDocument() {
                        var document = new Document();
                        var patch = new diff_match_patch().patch_make("", "Test");
                        document.edit(patch);
                        return document;
                    }

                    describe("when we call applyRemoteRevision with " +
                        "a remote revision with the same id", function() {
                            var document = getDocument();
                            var revision = {
                                id: document.currentRevision.id,
                                patches: document.currentRevision.patch
                            };
                            document.applyRemoteRevision(revision);
                            it("ensure it removes the local revision", function() {
                                mapObjToArray(document.localrevisions).length.should.equal(0);
                            });
                        });
                    describe("when we call applyRemoteRevision with " +
                        "a different id", function() {
                            var document = getDocument();
                            var remotePatch = new diff_match_patch().patch_make("", "Test2");
                            var revision = {
                                patch: remotePatch,
                                id: guid.newGuid(),
                                content: "Test2"
                            };
                            document.applyRemoteRevision(revision);
                            it("ensure that it applies the local revision on top" +
                                " of the new remote revision", function() {
                                    document.content.should.equal("TestTest2");
                                });
                        });
                });
            describe("given a document with multiple local revisions", function () {
                this.context = function() {
                    var document = new Document();
                    var dmp = new diff_match_patch();
                    document.edit(dmp.patch_make("", "I went to the store to get some milk"));
                    document.edit(dmp.patch_make(
                        "I went to the store to get some milk",
                        "I went to the corner store to get some milk"
                    ));
                    return { document: document };
                };
                describe("when we apply a remote revision corresponding to " +
                    "the first local revision", function () {
                        var document = this.parent.context().document;
                        var firstLocal = document.getSortedLocalRevisions()[0];
                        var remoteRevision = {
                            id: firstLocal.id,
                            patch: firstLocal.patch,
                            content: "I went to the store to get some milk"
                        };
                        document.applyRemoteRevision(remoteRevision);
                        it("ensure that the contents of the document don't change", function () {
                            document.content.should.equal("I went to the corner store to get some milk");
                        });
                    });
                describe("when we apply a remote revision corresponding to " +
                    "the second local revision", function () {
                        this.context = function() {
                            var document = this.parent.context().document;
                            var firstLocal = document.getSortedLocalRevisions()[0];
                            var secondLocal = document.getSortedLocalRevisions()[1];
                            var remoteRevision = {
                                id: secondLocal.id,
                                patch: secondLocal.patch,
                                content: "I went to the corner store to get some milk",
                                revisionAppliedTo: firstLocal.id
                            };
                            document.applyRemoteRevision(remoteRevision);
                            return {
                                document: document,
                                secondLocal: secondLocal,
                                firstLocal: firstLocal,
                            };
                        };
                        it("ensure that contents of the document don't change", function () {
                            var document = this.test.parent.context().document;
                            document.content.should.equal("I went to the corner store to get some milk");
                        });
                        it("ensure that it sets the current remote revision to the second revision", function() {
                            var context = this.test.parent.context();
                            var document = context.document;
                            document.currentRemoteRevision.id.should.equal(context.secondLocal.id);
                        });
                        describe("and then apply the first revision", function() {
                            var context = this.parent.context();
                            var document = context.document;
                            var secondRevisionId = context.secondLocal.id;
                            var firstLocal = context.firstLocal;
                            var remoteRevision = {
                                id: firstLocal.id,
                                patch: firstLocal.patch,
                                content: "I went to the store to get some milk"
                            };
                            document.applyRemoteRevision(remoteRevision);
                            it("ensure that the document contents don't change", function() {
                                document.content.should.equal("I went to the corner store to get some milk");
                            });
                            it("ensure that the currentRemote revision is the second revision", function() {
                                document.currentRemoteRevision.id.should.equal(secondRevisionId);
                            });
                        });
                    });
                describe("when we apply a revision failure message " +
                    "corresponding to the first local revision", function() {
                        var context = this.parent.context();
                        var document = context.document;
                        var firstLocal = document.getSortedLocalRevisions()[0];
                        var failureMessage = {
                            id: firstLocal.id,
                            failed: true
                        };
                        document.applyRemoteRevision(failureMessage);
                        it("ensure it builds two new local revision with the" +
                            "current remote revision as the revision applied to", function() {
                                document.getSortedLocalRevisions().length.should.equal(2);
                                document.getSortedLocalRevisions().map(function(rev) {
                                    return rev.id;
                                }).should.not.include(firstLocal.id);                               
                            });
                    });
            });
            describe("given a document with 1 remote revisions", function () {
                this.context = function() {
                    var document = new Document();
                    var remoteRevision1 = {
                        id: guid.newGuid(),
                        patch: new diff_match_patch()
                            .patch_make(
                                "",
                                "I went to the corner store to buy some milk"),
                        content: "I went to the corner store to buy some milk",
                    };
                    document.applyRemoteRevision(remoteRevision1);
                    return document;
                };
                describe("when a remote revision applied to the current remote " +
                    "revision is applied", function (context) {
                        var document = this.parent.context();
                        var remoteRevision2 = {
                            id: guid.newGuid(),
                            patch: new diff_match_patch()
                                .patch_make(
                                    "I went to the corner store to buy some milk",
                                    "I went to the corner store to buy some oj and milk"),
                            content: "I went to the corner store to buy some oj and milk",
                            revisionAppliedTo: document.currentRemoteRevision.id
                        };
                        document.applyRemoteRevision(remoteRevision2);
                        it("ensure that the documents contents equal the " +
                            "contents of the 2nd revision", function () {
                                document.content.should.equal(remoteRevision2.content);
                            });
                    });
                describe("when two remote revisions applied out of order", function () {
                    var document = this.parent.context();
                    var remoteRevision2 = {
                        id: guid.newGuid(),
                        patch: new diff_match_patch().patch_make(
                            document.currentRemoteRevision.content,
                            "I went to the corner store to buy some oj and milk"
                        ),
                        content: "I went to the corner store to buy some oj and milk",
                        revisionAppliedTo: document.currentRemoteRevision.id
                    };
                    var remoteRevision3 = {
                        id: guid.newGuid(),
                        patch: new diff_match_patch().patch_make(
                            remoteRevision2.content,
                            "I ran to the corner store to buy some oj and milk"
                        ),
                        content: "I ran to the corner store to buy some oj and milk",
                        revisionAppliedTo: remoteRevision2.id
                    };
                    document.applyRemoteRevision(remoteRevision3);
                    document.applyRemoteRevision(remoteRevision2);
                    it("ensure the document has the 3rd revisions content", function () {
                        document.content.should.equal(remoteRevision3.content);
                    });
                });
            });
        });
       
        function mapObjToArray(object) {
            var array = [];
            for (var prop in object) {
                array.push(object[prop]);
            }
            return array;
        }
});