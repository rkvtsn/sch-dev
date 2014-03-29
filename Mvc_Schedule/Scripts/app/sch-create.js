var schCreate = function (update) {

    if (this.isActivated) return this.model; // 'неадекватный' кусочек синглтона
    var x = this;
    x.editClass = { hash: '#edit', on: '<i title="В обычный режим" class="fa fa-eye"></i>', off: '<i title="В режим редактора" class="fa fa-cogs"></i>', };
    x.isEdit = false;
    var groupId = $("#group-id").val();


    //x.editorBtn = {
    //    hash: '#edit',
    //    content: {
    //        on: '<i title="В обычный режим" class="fa fa-eye"></i>',
    //        off: '<i title="В режим редактора" class="fa fa-cogs"></i>',
    //    },
    //    btn: $('<a class="btn" href="' + this.hash + '">' + this.content.off + '</a>'),
    //    value: false,
    //    on: function () { },
    //    off: function () { },
    //    toggle: function (v) { },
    //};
    //x.checkBtn = {};
    //x.controls = [x.editorBtn, x.checkBtn];


    x.editBtn = $('<a class="btn" href="' + x.editClass.hash + '">' + x.editClass.off + '</a>');
    x.checkBtn = $('<a class="btn" title="Проверить" href="#check"><i class="fa fa-check"></i></a>');

    var ctrlAdd = '<div class="ctrl btn add"><i class="fa fa-plus-square-o"></i></div>';
    var ctrlEdit = '<div class="ctrl btn edit"><i class="fa fa-pencil-square-o"></i></div>';
    var ctrlDel = '<div class="ctrl btn delete"><i class="fa fa-trash-o"></i></div>';

    x.ctrlTileRaw = $('<div class="ctrl-tile">' + ctrlEdit + ctrlDel + ctrlAdd + '</div>');
    x.ctrlTileSubRaw = $('<div class="ctrl-tile">' + ctrlEdit + ctrlDel + '</div>');
    x.ctrlTileAddRaw = $('<div class="ctrl-tile">' + ctrlAdd + '</div>');

    return constructor();


    // Yes Cap that's 'Constructor'!
    function constructor() {
        x.isActivated = true;
        DlgHelper.EnableValidation();

        prepareLayout();

        x.model = { refresh: refresh };
        return x.model;
    }



    function prepareLayout() {
        $("#controls").prepend(x.checkBtn);
        $("#controls").prepend(x.editBtn);

        x.checkBtn.click(function (e) {
            checkLessons();
            e.preventDefault();
            return false;
        });
        x.editBtn.click(function (e) {
            setEditBtn(!x.isEdit);
            refresh();
            e.preventDefault();
            return false;
        });
    }
    function setEditBtn(b) {
        if (b)
            activateEdit();
        else
            deactivateEdit();
        x.isEdit = b;
    }
    function deactivateEdit() {
        window.location.hash = "";
        x.editBtn.html(x.editClass.off);
    }
    function activateEdit() {
        window.location.hash = x.editClass.hash;
        x.editBtn.html(x.editClass.on);
    }
    function rowHtml(row, d) {
        return $('<div class="cell column2" val="' + d.LessonId + '">' + getTime(d.Time) + '</div>' +
                emptyCellHtml(false) +
                emptyCellHtml(true) +
                '<div class="ending"></div>' +
                '<div class="cell column1">&nbsp;</div>');
    }
    function showEditorTime() {
        return $.ajax({
            type: "GET",
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            url: "/lessons/list/",
            success: function (data) {
                $(".row").each(function () {
                    var row = $(this);
                    var firstColumnInRow = row.find(".column2").first();
                    if (!firstColumnInRow.attr("val")) {
                        row.html("");
                        $.each(data, function (i, d) { row.append(rowHtml(row, d)); });
                    } else {
                        var columns = row.find(".column2");
                        var colCounter = 0;
                        $.each(data, function (i, d) {
                            if (!columns[colCounter])
                                row.append(rowHtml(row, d));
                            else
                                if (columns[colCounter].getAttribute("val") != d.LessonId) {
                                    if (i >= colCounter) {
                                        rowHtml(row, d).insertBefore(columns.eq(colCounter));
                                    }
                                    else
                                        row.append(rowHtml(row, d));
                                } else {
                                    colCounter++;
                                }
                        });
                    }
                });
            }
        });
    }
    function bindControls() {
        return showEditorTime().done(function () {
            x.ctrlTileRaw.insertAfter(".subrow1.subject-lector");
            x.ctrlTileSubRaw.insertAfter(".subrow2.subject-lector");
            $(".lesson.empty").append(ctrlTileAddRaw);
        });
    }




    function refresh() {
        if (window.location.hash == x.editClass.hash) setEditBtn(true);
        if (x.isEdit) {
            return update()
                .done(function () {
                    bindControls()
                        .done(function () {
                            DlgHelper.BindOn(generateForm, add, edit, del);
                        });
                });
        } else
            return update();
    }




    function setPointer(caller) {
        window.lastPointer = ($(".lesson:not(.empty)").length == 0) ? defaultHash : '#' + caller.parents(".weekday").attr("id");
    }

    /* main methods */

    //ADD(*, week, lesson-id, weekday-id)
    function add() {
        DlgHelper.AjaxAction("/schedule/add/", "POST", function (data) {

            if (data != null && data != "") {
                refresh().done(function () { DlgHelper.ShowDialogSuccess("Добавлено: " + data, 1000); });
            } else {
                DlgHelper.ShowDialogError("Неверный ввод...", 2000);
            }

        });
    }
    //EDIT(subject-title, auditory, lector, lesson-type, sch-id)
    function edit() {
        DlgHelper.AjaxAction("/schedule/edit/", "POST", function (data) {

            if (data != null && data != "") {
                refresh().done(function () { DlgHelper.ShowDialog("Изменено: " + data); });
            } else {
                DlgHelper.ShowDialogError("Неверный ввод...", 2000);
            }

        });
    }
    //DROP(id)
    function del(caller) {
        getId(caller);
        setPointer(caller);
        DlgHelper.AjaxAction("/schedule/drop/", "POST",
        function (data) {
            if (data != null && data != "") {
                DlgHelper.ShowDialogSuccess("Удалено: " + data, 1000);
                refresh();
            } else {
                DlgHelper.ShowDialogError("Не удалось...", 2000);
            }
        });
    }

    function generateForm(caller, isEdit) {
        setPointer(caller);
        DlgHelper.ClearForm();

        if (x._lock != true) {
            x._lock = true;
            autocompleteHandler($("#subject-title"), "Subjects");
            autocompleteHandler($("#auditory"), "Auditory");
            autocompleteHandler($("#lector"), "Lectors");
            $("#lesson-type").change(function () { getSubjectPlan(); });
        }

        if (isEdit) {
            var id = getId(caller);
            DlgHelper.AjaxActionWithData("/schedule/get/", "GET", function (data) {
                checkByElement("Auditory", $("#auditory").val(data.Auditory));
                checkByElement("Lectors", $("#lector").val(data.LectorName));
                $("#group-sub").val(data.GroupSub);
                $("#subject-title").val(data.SubjectName);
                $("#lesson-type").val(data.LessonType);
            }, { id: id });
        }
        var lesson = caller.parents(".lesson");
        $("#week").val(lesson.attr("week"));
        $("#lesson-id").val(lesson.prevAll(".column2:first").attr("val"));
        $("#weekday-id").val(lesson.parents(".weekday").attr("val"));
    }

    function getId(caller) {
        var id = Number(caller.parents('.lesson').attr("sch-id"));
        $("#sch-id").val(id);
        return id;
    }



    // autocomplete
    function getSubjectPlan(val) {
        var div = $("#is-available");
        var lessonType = $("#lesson-type").val();
        if (lessonType > 3 || lessonType < 1) {
            popupAvail(-1, div, "Subjects");
        } else
            $.ajax({
                url: '/Schedule/GetSubjectPlan',
                type: "GET",
                dataType: "json",
                data: { groupid: groupId, value: val, lessontype: lessonType },
                success: function (data) {
                    popupAvail(data, div, "Subjects");
                }
            });
    }

    function popupAvail(data, div, str) {

        var p = div.find('a[data-type="' + str + '"]');

        var d = Number(data.Owner);
        
        if ((str != "Subjects" && (data == "" ||  data.Owner == groupId || data == groupId || d == 0)) || d < 0) {

            if (p.length != 0) {
                p.remove();
            }

        } else {
            
            var txt = (str == "Lectors") ? 'Преподаватель занят' : (str != "Subjects") ? 'Аудитория занята (' + ((data.Available == null) ? 'доступных нет' : 'доступна ' + data.Available ) + ')' : 'Осталось ' + data + ' ч.';
            if (p.length == 0) {
                if (str == "Subjects") {
                    p = $('<a href="/plans/' + groupId + '" data-type="' + str + '"></a>');
                } else
                    if (str == "Lectors") {
                        p = $('<a href="/schedule/index/' + data + '#edit" data-type="' + str + '"></a>');
                    } else {
                        p = $('<a href="/schedule/index/' + data.Owner + '#edit" data-type="' + str + '"></a>');
                    }
                div.append(p);
            }
            p.html(txt);

        }

    }


    function checkByElement(str, jq) {
        var val = jq.val();
        if (str == "Lectors" || str == "Auditory") {
            var div = $("#is-available");
            $.ajax({
                url: '/schedule/getavailable' + str,
                type: "GET",
                dataType: "json",
                data: {
                    timeId: $("#lesson-id").val(),
                    value: val,
                    week: $("#week").val(),
                    weekdayId: $("#weekday-id").val(),
                    groupId: $("#group-id").val()
        },
                success: function (data) {
                    popupAvail(data, div, str);
                }
            });
        } else {
            getSubjectPlan(val);
        }
    }

    function autocompleteHandler(element, str) {

        element.on('keydown blur', function (event) {
            clearTimeout(x.timer);
            var jq = $(this);
            x.timer = setTimeout(function () {
                checkByElement(str, jq);
                if (event.type === "blur") clearInterval(x.timer);
            }, event.type === "blur" ? 0 : 1000);
        });

        element.autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/schedule/getlist',
                    type: "GET",
                    dataType: "json",
                    data: { letter: request.term, method: str },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item, value: item };
                        }));
                    }
                });
            },
            minLength: 1
        });
    }

    function setValid(l) {
        l.removeClass("not-available");
    }
    function setInvalid(l) {
        l.addClass("not-available");
    }

    function checkLessons() {
        $.ajax({
            type: 'GET',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            url: '/schedule/checklessons/',
            data: { groupId: $('#group-id').val() },
            success: function (data) {
                if (!data || data.length == 0) return;

                $.each(data, function (i, d) {
                    if (!d && d.length == 0 && d.Busy.length == 0) return;

                    var lesson = $('.lesson[sch-id="' + d.Id + '"]');

                    if (d.Busy[0].Auditory == true) {
                        setInvalid(lesson);
                    } else {
                        setValid(lesson);
                    }
                    if (d.Busy[0].Lector == true) {
                        setInvalid(lesson);
                    } else {
                        setValid(lesson);
                    }
                });
            }
        });
    }

};