(function () {
    var isOddWeek = false;
    var timer;
    var groupId;

    function popupAvail(data, div, str) {

        var p = div.children("a[data-type='" + str + "']");

        var d = Number(data);
        if ((str != "Subjects" && (data == groupId || d == 0)) || d < 0) {

            if (p.length != 0) {
                p.remove();
            }

        } else {

            var txt = (str == "Lectors") ? 'Преподаватель занят' : (str != "Subjects") ? 'Аудитория занята' : 'Осталось ' + data + ' ч.';
            if (p.length == 0) {
                if (str == "Subjects") {
                    p = $('<a href="/plans/' + groupId + '" data-type="' + str + '"></a>');
                } else
                    p = $('<a href="/schedule/create/' + data + '/' + (isOddWeek ? 2 : 1) + '" data-type="' + str + '"></a>');
                div.append(p);
            }
            p.html(txt);

        }

    }

    function getSubjectPlan(jq) {
        var li = jq.closest("li");
        var div = li.find(".is-available");
        var val = li.find(".subjects").val();
        var lessonType = li.find("select").val();
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

    function autocompleteHandler(element, str) {

        element.on('keydown blur', function (event) {
            clearTimeout(timer);
            var jq = $(this);
            timer = setTimeout(function () {
                var val = jq.val();
                var lesson = jq.closest('.lesson');
                var lessonId = lesson.attr("id");
                if (str == "Lectors" || str == "Auditory") {
                    var div = jq.parent("DIV").parent("LI").children(".is-available");
                    $.ajax({
                        url: '/Schedule/GetAvailable' + str,
                        type: "GET",
                        dataType: "json",
                        data: { timeId: lessonId, value: val, week: isOddWeek },
                        success: function (data) {
                            popupAvail(data, div, str);
                        }
                    });
                } else {
                    getSubjectPlan(jq);
                }
                if (event.type === "blur") clearInterval(timer);
            }, event.type === "blur" ? 0 : 1000);
        });

        element.autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Schedule/GetList',
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

    $(document).ready(function () {
        isOddWeek = $("#IsOddWeek").attr("value");
        var index = $("#indexer").val(); // баластовый счётчик, но без него совсем скучно
        groupId = $("#GroupId").val(); //

        $(".add_element").click(function () {
            var counter = $(this).parent("P").prev(".lesson-form").children("LI").length;

            if (counter >= 2) { return; }

            var weekdayId = ($(this).parent("P").parent(".lesson").parent(".weekday").attr("id"));
            var lessonId = ($(this).parent("P").parent(".lesson").attr("id"));
            var htmlPaste = '<li>' +
                '<div class="is-available"></div>' +
                '<div class="form-line cell">' +
                '<label for="ScheduleTableRows_' + index + '__Auditory"> Аудитория </label>' +
                '<div class="ending"></div>' +
                '<input class="auditory" id="ScheduleTableRows_' + index + '__Auditory" name="ScheduleTableRows[' + index + '].Auditory" type="text" value="" />' +
                '</div>' +
                '<div class="form-line cell">' +
                '<label for="ScheduleTableRows_' + index + '__LectorName"> Преподаватель </label>' +
                '<div class="ending"></div>' +
                '<input type="text" class="lectors" id="ScheduleTableRows_' + index + '__LectorName" name="ScheduleTableRows[' + index + '].LectorName" />' +
                '</div>' +
                '<div class="form-line cell">' +
                '<label for="ScheduleTableRows_' + index + '__SubjectName"> Название дисциплины </label>' +
                '<div class="ending"></div>' +
                '<input type="text" class="subjects" id="ScheduleTableRows_' + index + '__SubjectName" name="ScheduleTableRows[' + index + '].SubjectName" />' +
                '</div>' +
                '<div class="form-line cell">' +
                '<label for="ScheduleTableRows_' + index + '__LessonType">Тип занятия</label>' +
                '<div class="ending"></div>' +
                '<select id="ScheduleTableRows_' + index + '__LessonType" name="ScheduleTableRows[' + index + '].LessonType">' +
                '<option value="1" selected="selected">Лекция</option>' +
                '<option value="2">Практика</option>' +
                '<option value="3">Лабораторная</option>' +
                '<option value="4">Зачёт</option>' +
                '</select>' +
                '</div>' +
                '<input id="ScheduleTableRows_' + index + '__LessonId" name="ScheduleTableRows[' + index + '].LessonId" type="hidden" value="' + lessonId + '" />' +
                '<input id="ScheduleTableRows_' + index + '__GroupId" name="ScheduleTableRows[' + index + '].GroupId" type="hidden" value="' + groupId + '" />' +
                '<input id="ScheduleTableRows_' + index + '__WeekdayId" name="ScheduleTableRows[' + index + '].WeekdayId" type="hidden" value="' + weekdayId + '" /></li><div class="ending"></div>';
            $(this).parent("P").prev(".lesson-form").append(htmlPaste);
            autocompleteHandler($(this).parent("P").prev(".lesson-form").find('.subjects'), "Subjects");
            autocompleteHandler($(this).parent("P").prev(".lesson-form").find('.auditory'), "Auditory");
            autocompleteHandler($(this).parent("P").prev(".lesson-form").find('.lectors'), "Lectors");
            $(this).parent("P").prev(".lesson-form").find('select').change(function () { getSubjectPlan($(this)); });
            index++;
        });

        $(".remove_element").click(function () {
            var element = $(this).parent("P").prev(".lesson-form").children("LI").last();
            if (element != null) element.remove();
        });


        $(".weekday h3").click(
            function () {
                if (this.isVisible != true) {
                    $(this).parent(".weekday").children(".lesson").hide();
                    $(this).children("SPAN").html("показать");
                    this.isVisible = true;
                } else {
                    $(this).parent(".weekday").find(".lesson").show();
                    $(this).children("SPAN").html("скрыть");
                    this.isVisible = false;
                }
            }
        );

        //$(".remove").click(function () { $(this).parent().remove(); });
        autocompleteHandler($("input.subjects"), "Subjects");
        autocompleteHandler($("input.auditory"), "Auditory");
        autocompleteHandler($("input.lectors"), "Lectors");
        $(".lesson-form select").change(function () { getSubjectPlan($(this)); });
    });
})();