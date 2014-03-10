(function () {

    var list = $("#list-weeks");
    var table = $("#sch");
    var groupId = $("#group-id").val();

    var renderTable = function (data) {
        console.log("renderTable");

        if (data != null) {
            console.log("appended");
            var div = renderTableRow(data);
            list.append(div);
        }

        DlgHelper.ShowDialogSuccess("Готово!", 1000);
        updateSubrows();
        table.show();
    };


    var updatePage = function () {
        table.hide();
        list.html('');
        DlgHelper.ShowDialog("Обнавляю...", 5000);
        $.ajax({
            type: "GET",
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            url: "/schedule/list/",
            data: { id: groupId },
            success: function (data) {
                renderTable(data);
                legendBind();
            }
        });
    };

    $(document).ready(function () {
        methodToFixLayout();
        $("#update").click(function (e) {
            updatePage();
            e.preventDefault();
            return false;
        });

        $("#save").click(function (e) {
            e.preventDefault();

            var htmlDoc = $("#list-weeks").html();
            console.log(htmlDoc);

            //TODO saving to file
            // a: push on server -> pull from server...
            // b: (isAuth) ? redirect-> {indexDev || index}

            return false;
        });

        var model;
        if (typeof schCreate == 'function')
            model = schCreate({ update: updatePage });
        else
            model = { activate: updatePage };

        model.activate();
    });






    function updateSubrows() {
        $(".subject-name").each(function (i) { //TODO!!! inject to Rendering
            var isSub1 = $(this).parent().hasClass("subrow1");
            var mCell = $(this).parent().parent();
            var lesType = mCell.attr("class")[mCell.attr("class").length - 1];
            var lesName = "Лекция";
            if (lesType == 2) lesName = "Практика";
            else if (lesType == 3) lesName = "Лабораторная";
            else if (lesType == 4) lesName = "Зачёт";
            mCell.attr("title", $(this).text() + " (" + lesName + ")");
            var len = $(this).text().length + $(this).next().text().length; // Длина линейки

            if (isSub1) {
                if (len > 34)
                    $(this).text($(this).text().substr(0, 30) + "...");
            } else
                if (len >= 16) {
                    var words = $(this).text().split(/[ -]+/);
                    if (len <= 15 || words.length == 1)
                        $(this).text($(this).text().substr(0, 10) + "...");
                    else {
                        var name = "";
                        for (var j = 0; j < words.length; j++) {
                            name += words[j].toUpperCase()[0];
                        }
                        $(this).text(name);
                    }
                }
        });
    }

    function methodToFixLayout() {
        var winWidth = $(window).width();
        $("#selector-block").css("width", (winWidth <= 800) ? 'auto' : '');
        $("#selector-block").css("min-width", (winWidth <= 800) ? '300px' : '');
    }
    $(window).bind("resize", methodToFixLayout);

    function legendBind() {
        var legend = $("#legend .sq");
        if (this.isBinded == true) {
            legend.each(function () { this.isVisible = false; });
            legend.unbind("click");
            $("#legend .title").unbind("click");
            legend.css({ opacity: "" });
        }
        this.isBinded = true;

        $("#legend .title").click(function () {
            $($(this).prev()).click();
        });


        legend.click(function () {
            var id = $(this).attr("class")[$(this).attr("class").length - 1];
            var bg = $(".lesson.bg" + id.toString());
            if (this.isVisible != true) {
                $(this).css({
                    opacity: "0.6"
                });
                bg.fadeTo("fast", 0);
                this.isVisible = true;
            } else {
                $(this).css({
                    opacity: ""
                });
                bg.fadeTo("fast", 1);
                this.isVisible = false;
            }
        });
    };

    function zero(m) {
        return (m < 10) ? "0" + m : m;
    }

    function getTime(jsonDt) {
        var dt = new Date(parseInt(jsonDt.substr(6)));
        return zero(dt.getHours()) + ":" + zero(dt.getMinutes());
    }


    // @rendering
    function renderTableRow(model) { // СУПЕР ТУПОЙ МЕТОД !!! TODO: переделать на AngularJS все подобные г*вно коды!
        var row = "";
        $.each(model, function (i, week) {
            row += '<div class="weekday">' +
                   '<div class="cell column1">' + week.Key.Name + '</div>';

            $.each(week.Group, function (j, lessons) {
                row += '<div class="row">';
                if (lessons.length > 0) {
                    $.each(lessons, function (k, lesson) {
                        row += '<div class="cell column2">' + getTime(lesson.Key.Time) + '</div>';
                        $.each(lesson.Group, function (x, sc) {
                            var subrow = (!sc.IsWeekOdd) ? lesson.Key.CountEven : lesson.Key.CountOdd;
                            row +=
                                '<div class="cell lesson bg' + (sc.LessonType) + '">' +
                                '<div class="subrow' + (subrow) + '">' +
                                '<span class="subject-name">' + sc.SubjectName + '</span>' +
                                '<span class="subject-auditory">' + sc.Auditory + '</span>' +
                                '</div>' +
                                '<div class="subrow' + subrow + ' subject-lector">' + sc.LectorName + '</div>' +
                                '</div>';
                        });

                        row +=
                            '<div class="ending"></div>' +
                            '<div class="cell column1">&nbsp;</div>';
                    });
                } else {
                    row +=
                        '<div class="cell column2">&nbsp;</div>' +
                        '<div class="cell subrow2">&nbsp;</div>' +
                        '<div class="cell subrow2">&nbsp;</div>' +
                        '<div class="ending"></div>' +
                        '<div class="cell column1">&nbsp;</div>';
                }
                row += '</div>';
            });
            row += '</div><div class="ending"></div>';
        });
        return row;
    }

})();