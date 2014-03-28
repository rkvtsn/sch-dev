(function () {
    var dm = $("#minutes");
    var dh = $("#hours");
    var list = $("#list-elements");

    function updateTimeInputs(caller, isEdit) {
        dm.html('');
        dh.html('');
        if (isEdit == true) {
            var parent = caller.parent();
            $("#lesson-id").val(parent.attr("val"));
            var h = Number(parent.find(".h").attr("val"));
            var m = Number(parent.find(".m").attr("val"));
            //console.log(parent.attr("val") + " " + h + " " + m);
        } else m = h = 0;

        for (var i = 0; i < 60; i++) {
            dm.append('<option value="' + i + '" ' + ((m == i) ? ' selected' : ' ') + '>' + ((i >= 10) ? i : '0' + i) + '</option>');
            if (i < 24)
                dh.append('<option value="' + i + '" ' + ((h == i) ? ' selected' : ' ') + '>' + ((i >= 10) ? i : '0' + i) + '</option>');
        }
    }

    function addZero(str) {
        return (String(str).length == 1) ? "0" + str : str;
    }

    function refresh(onsuccess) {
        DlgHelper.onUpdate(list,
            "/lessons/list",
            "GET",
            function (i, x) {
                var div = $('<li val="' + x.LessonId + '">' +
                    '<a class="delete" href="javascript: void(0)">Удалить</a> ' +
                    '<a class="edit subtitle" href="javascript: void(0)">' +
                    '<span val="' + addZero(x.Hours) + '" class="h">' + addZero(x.Hours) + '</span>:<span val="' + addZero(x.Minutes) + '" class="m">' + addZero(x.Minutes) + '</span>' +
                    '</a></li>');
                list.append(div);
            },
            onsuccess,
            function () { DlgHelper.BindOn(updateTimeInputs, add, edit, del); }
        );
    }

    function add() {
        DlgHelper.AjaxAction("/lessons/add", "POST", function (data) {
            refresh(function () {
                if (data != false) {
                    DlgHelper.ShowDialog("Добавлено время " + data);
                } else {
                    DlgHelper.ShowDialogError("Неудачно: <em>Такое время уже существует...</em>");
                }
            });
        });
    }

    function del(caller) {
        var parent = caller.parent();
        $("#lesson-id").val(parent.attr("val"));
        DlgHelper.AjaxAction("/lessons/drop", "POST",
            function (data) {
                refresh(function () {
                    DlgHelper.ShowDialog("Удалено время " + data);
                });
            });
    }
    function edit() {
        DlgHelper.AjaxAction("/lessons/edit", "POST",
            function (data) {
                refresh(function () {
                    if (data != false) {
                        DlgHelper.ShowDialog("Изменено время " + data);
                    } else {
                        DlgHelper.ShowDialog("Не удалось изменить");
                    }
                });
            });
    }


    $(function () {
        refresh(function () { DlgHelper.HideDialog(); });
    });


})();