function zero(m) {
    return (m < 10) ? "0" + m : m;
}

function getTime(o) {
    return zero(o.Hours) + ":" + zero(o.Minutes);
}

var emptyCellHtml = function (isWeekOdd, subrow) {
    return '<div class="cell empty lesson" week="' + isWeekOdd + '"><div class="subrow' + (subrow || 1) + '">&nbsp</div><div class="subrow' + (subrow || 1) + '">&nbsp</div></div>';
};
var defaultHash = "#header";
window.lastPointer = defaultHash;

(function () {

    var list = $("#list-weeks");
    var table = $("#sch");
    var groupId = $("#group-id").val();

    var renderTable = function (data) {
        if (data != null) {
            list.append(renderTableRow(data));
        }
        DlgHelper.ShowDialogSuccess("Готово!", 1000);
        updateSubrows();
        var lessons = table.find(".lesson");
        lessons.stop().hide();
        table.show();
        $.when(lessons.fadeIn()).then(function () {
            if (window.lastPointer == defaultHash)
                $(document.body).scrollTop($(window.lastPointer).offset().top);
            else {
                $(document.body).stop().animate({ 'scrollTop': $(window.lastPointer).offset().top });
            }
        });
    };

    var updatePage = function () {

        table.hide();
        list.html('');
        DlgHelper.ShowDialogWait("Обнавляю...", 5000);

        var d = { id: groupId };
        var u = "/list/";

        if (isSearching()) {
            d = { keyword: keyword };
            u = "/search/";
        }

        return $.when($.ajax({
            type: "GET",
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            url: "/schedule" + u,
            dataType: "json",
            data: d,
            success: function (data) {
                renderTable(data);
                legendBind();
            },
            error: function(t) {
                DlgHelper.ShowDialogError("У нас ошибка <strong>" + t.status + "</b>");
            }
        }));

    };

    var keyword = $("#search-keyword").val();
    var isSearching = function () { return keyword != ""; };

    $(document).ready(function () {

        $(window).bind("resize", methodToFixLayout);

        var model = (typeof schCreate == 'function' && !isSearching()) ? schCreate(updatePage) : { refresh: updatePage };

        model.refresh().done(function () { methodToFixLayout(); });

        $("#update").click(function (e) {
            window.lastPointer = defaultHash;
            model.refresh().done(function () { methodToFixLayout(e, true); });
            e.preventDefault();
            return false;
        });

        $("#save").hide().click(function (e) {
            e.preventDefault();
            return false;
        });
    });


    // На скорую руку..
    // TODO style.class
    // TODO header.style
    function methodToFixLayout(e, v) {
        var x = this;
        var winWidth = $(window).width();
        if (v) setFullScreen(x);

        if (winWidth <= 800) {

            if (x.currentMode == "m") return;
            x.currentMode = "m";

            $(".header1").css({
                "cursor": "pointer",
                "color": "#000",
                "font-weight": ""
            }).addClass("btn")
            .click(function () {
                if ($(this).attr("activated") == "true") return;
                if (!x.last) x.last = $(this).next();

                x.last.css({ "font-weight": "" });
                $(this).css({ "font-weight": "bold" });
                $('[week="' + $(this).attr("val") + '"]').show();
                $('[week="' + x.last.attr("val") + '"]').hide();

                $(this).attr("activated", true);
                x.last.attr("activated", false);
                x.last = $(this);
            });

            $('.header1[val="false"]').click();
            $("#selector-block").css({ "width": '300px', "min-width": '300px' });

        } else {
            if (x.currentMode == "f") return;
            setFullScreen(x);
        }

    }
    function setFullScreen(x) {
        x.currentMode = "f";
        x.last = null;
        $("[week]").show();
        $(".header1").unbind("click")
                     .css({ "cursor": "default", "font-weight": "" })
                     .removeClass("btn");
        $("#selector-block").css({ "width": '', "min-width": '' });
    }

    function updateSubrows() {
        $(".subject-name").each(function () { //TODO!!! inject to Rendering
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

    var emptyRowHtml =
        '<div class="cell column2">&nbsp;</div>' +
        '<div class="ending"></div>' +
        '<div class="cell column1">&nbsp;</div>';

    // @rendering
    function renderTableRow(model) { // TODO: переделать на AngularJS все подобные г*вно коды!
        var row = "";
        $.each(model, function (i, week) {
            row += '<div class="weekday" id="weekday_' + week.Key.WeekdayId + '" val="' + week.Key.WeekdayId + '">' +
                   '<div class="cell column1">' + week.Key.Name + '</div>';

            $.each(week.Group, function (j, lessons) {
                row += '<div class="row">';
                if (lessons.length > 0) {
                    $.each(lessons, function (k, lesson) {
                        row += '<div class="cell column2" val="' + lesson.Key.LessonId + '">' + getTime(lesson.Key) + '</div>';

                        var f = false;

                        var line = "";

                        $.each(lesson.Group, function (x, sc) {
                            var subrow = (!sc.IsWeekOdd) ? lesson.Key.CountEven : lesson.Key.CountOdd;
                            var isSub = false;

                            if (sc.GroupSub >= 1 && subrow == 1) {
                                subrow = 2;
                                isSub = true;
                            }

                            var tempRow =
                                '<div group-sub="' + sc.GroupSub + '" sch-id="' + sc.ScheduleTableId + '" week="' + sc.IsWeekOdd + '" class="cell lesson bg' + (sc.LessonType) + '">' +
                                '<div class="subrow' + (subrow) + '">' +
                                '<span class="subject-name">' + sc.SubjectName + '</span>' +
                                '<span class="subject-auditory">' + sc.Auditory + '</span>' +
                                '</div>' +
                                '<div class="subrow' + subrow + ' subject-lector" >' + sc.LectorName + '</div>' +
                                '</div>';

                            if (isSub) {
                                if (sc.GroupSub == 1) {
                                    tempRow = tempRow + emptyCellHtml(sc.IsWeekOdd, 2);
                                } else if (sc.GroupSub == 2) {
                                    tempRow = emptyCellHtml(sc.IsWeekOdd, 2) + tempRow;
                                }
                            }

                            if (lesson.Key.CountEven == 0 && lesson.Key.CountOdd > 0) {
                                if (lesson.Key.CountOdd == 1) tempRow = emptyCellHtml(!sc.IsWeekOdd) + tempRow;
                                else {
                                    if (!f) tempRow = emptyCellHtml(!sc.IsWeekOdd) + tempRow;
                                    f = !f;
                                }
                            } else if (lesson.Key.CountOdd == 0 && lesson.Key.CountEven > 0) {
                                if (lesson.Key.CountEven == 1) tempRow = tempRow + emptyCellHtml(!sc.IsWeekOdd);
                                else {
                                    if (f) tempRow = tempRow + emptyCellHtml(!sc.IsWeekOdd);
                                    f = !f;
                                }
                            }

                            line += tempRow;
                        });

                        //var groupSubs = line.find("[group-sub]");

                        row +=
                            line +
                            '<div class="ending"></div>' +
                            '<div class="cell column1">&nbsp;</div>';
                    });
                } else {
                    row += emptyRowHtml;
                }
                row += '</div>';
            });
            
            row += '</div><div class="ending"></div>';
        });

        return row;
    }

})();