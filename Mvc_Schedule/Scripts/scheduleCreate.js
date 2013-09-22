var isOddWeek = false;
var timer;
    
function popupAvail(data, div, dt) {

    var p = div.children("p[data-type='" + dt + "']");
    
    if (data == "0") {
        
        if (p.length != 0) {
            p.remove();
        }

    } else {
        
        var str = (dt == "Lectors") ? "Лектор занят" : "Аудитория занята";

        if (p.length == 0) {
            p = document.createElement("P");
            p.setAttribute("data-type", dt);
            var text = document.createTextNode(str);
            p.appendChild(text);
            div.append(p);
        } else {
            p.text(str);
        }
        
    }
    
}   

function autocompleteHandler(element, str) {
    if (str == "Lectors" || str == "Auditory") {
        element.on('keydown blur', function (event) {
            clearTimeout(timer);
            var jq = $(this);
            timer = setTimeout(function () {
                var val = jq.val();
                var lesson = jq.closest('.lesson');
                var lessonId = lesson.attr("id");
                
                $.ajax({
                    url: '/Schedule/GetAvailable' + str,
                    type: "POST",
                    dataType: "json",
                    data: { timeId: lessonId, value: val, week: isOddWeek },
                    success: function (data) {
                        var div = jq.parent("DIV").parent("LI").children(".is-available");
                        popupAvail(data, div, str);
                    }
                });
            }, event.type === "blur" ? 0 : 2000);
        });
    }
    element.autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Schedule/GetList',
                type: "POST",
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
    var groupId = $("#GroupId").val(); //
    $(".add_element").click(function () {
        var counter = $(this).parent("P").prev(".lesson-form").children("LI").length;
        if (counter >= 2) { return; }
        var weekdayId = ($(this).parent("P").parent(".lesson").parent(".weekday").attr("id"));
        var lessonId = ($(this).parent("P").parent(".lesson").attr("id"));
        var htmlPaste = '<li>' +
        '<div class="is-available"></div>' +
	    '<div class="form-line cell">' +
		    '<label for="ScheduleTableRows_' + index + '__Auditory"> Аудитория </label>' +
		        '<input class="auditory" id="ScheduleTableRows_' + index + '__Auditory" name="ScheduleTableRows[' + index + '].Auditory" type="text" value="" />' +
        '</div>' +
        '<div class="form-line cell">' +
		    '<label for="ScheduleTableRows_' + index + '__SubjectName"> Название дисциплины </label>' +
		        '<input type="text" class="subjects" id="ScheduleTableRows_' + index + '__SubjectName" name="ScheduleTableRows[' + index + '].SubjectName" />' +
		'</div>' +
        '<div class="form-line cell">' +
		    '<label for="ScheduleTableRows_' + index + '__LectorName"> Преподаватель </label>' +
		        '<input type="text" class="lectors" id="ScheduleTableRows_' + index + '__LectorName" name="ScheduleTableRows[' + index + '].LectorName" />' +
        '</div>' +
                '<input id="ScheduleTableRows_' + index + '__LessonId" name="ScheduleTableRows[' + index + '].LessonId" type="hidden" value="' + lessonId + '" />' +
		        '<input id="ScheduleTableRows_' + index + '__GroupId" name="ScheduleTableRows[' + index + '].GroupId" type="hidden" value="' + groupId + '" />' +
		        '<input id="ScheduleTableRows_' + index + '__WeekdayId" name="ScheduleTableRows[' + index + '].WeekdayId" type="hidden" value="' + weekdayId + '" /></li><div class="ending"></div>';
        $(this).parent("P").prev(".lesson-form").append(htmlPaste);
        autocompleteHandler($(this).parent("P").prev(".lesson-form").find('.subjects'), "Subjects");
        autocompleteHandler($(this).parent("P").prev(".lesson-form").find('.auditory'), "Auditory");
        autocompleteHandler($(this).parent("P").prev(".lesson-form").find('.lectors'), "Lectors");
        index++;
    });
    $(".remove_element").click(function () {
        var element = $(this).parent("P").prev(".lesson-form").children("LI").last();
        if (element != null) element.remove();
    });
    $(".weekday h3").toggle(
		function () {
		    $(this).parent(".weekday").children(".lesson").hide();
		    $(this).children("SPAN").html("показать");
		},
		function () {
		    $(this).parent(".weekday").children(".lesson").show();
		    $(this).children("SPAN").html("скрыть");
		}
	);
    //$(".remove").click(function () { $(this).parent().remove(); });
    autocompleteHandler($("input.subjects"), "Subjects");
    autocompleteHandler($("input.auditory"), "Auditory");
    autocompleteHandler($("input.lectors"), "Lectors");
});