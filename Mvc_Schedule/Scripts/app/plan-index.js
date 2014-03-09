(function () {
    
    var thisUri = function(action) { return "/plans/" + action + "/" + groupId; };

    var list = $("#list-elements");
    var groupId = $("#group-id").val();

    var isRendered = false;

    var refreshForm = function (caller, isEdit) {
        DlgHelper.ClearForm();
        if (!isRendered) {
            $(".form-inline").each(function () { $(this).find("input").width($(this).find("label").width()); });
            DlgHelper.EnableValidation();
            isRendered = true;
        }
        if (isEdit == true) {
            var parent = caller.parent();
            var id = parent.attr("val");
            $("#plan-id").val(id);
            DlgHelper.AjaxActionWithData(thisUri("get"), "POST", function(data) {
                $("#subject-title").val(data.SubjectName);
                $("#lec").val(data.LecH);
                $("#pr").val(data.PrH);
                $("#lab").val(data.LabH);
            }, { planId : id });
        }
    };

    var add = function () {
        DlgHelper.AjaxAction(thisUri("add"), "POST", function(data) {
            refresh(function() {
                if (data != null) {
                    DlgHelper.ShowDialogSuccess("Добавлено: " + data, 1000);
                } else {
                    DlgHelper.ShowDialogError("План с такой дисциплиной уже существует...", 2000);
                }
            });
        });
    };

    var edit = function () {
        DlgHelper.AjaxAction(thisUri("edit"), "POST", function(data) {
            refresh(function() {
                if (data != null) {
                    DlgHelper.ShowDialog("Изменено: " + data);
                } else {
                    DlgHelper.ShowDialogError("План с такой дисциплиной уже существует...", 2000);
                }
            });
        });
    };

    var del = function (caller) {
        var parent = caller.parent();
        $("#plan-id").val(parent.attr("val"));
        DlgHelper.AjaxAction(thisUri("drop"), "POST",
            function (data) {
                refresh(function () {
                    DlgHelper.ShowDialogSuccess("Удалено: " + data, 1000);
                });
            });
    };
    
    var appendItemToList = function(i, x) {
        var div = $('<li val="' + x.Key + '">' +
                '<a class="delete" href="javascript: void(0)">Удалить</a> ' +
                '<a class="edit subtitle" href="javascript: void(0)">' + x.Value + '</a></li>');
        list.append(div);
    };
    
    function refresh(onsuccess) {
        DlgHelper.onUpdate(list,
            thisUri("list"),
            "POST",
            appendItemToList,
            onsuccess,
            function () { DlgHelper.BindOn(refreshForm, add, edit, del); }
        );
    }

    function autocompleteHandler(element) {
        element.autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: thisUri("GetSubjects"),
                    type: "POST",
                    dataType: "json",
                    data: { letter: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item.Title, value: item.Title };
                        }));
                    }
                });
            },
            minLength: 1
        });
    }
    
    $(function () {
        refresh(function () { DlgHelper.HideDialog(); });
        autocompleteHandler($("#subject-title"));
    });
    
})();