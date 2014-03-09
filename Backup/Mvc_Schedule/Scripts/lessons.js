function updateTimeInputs() {
	var m = ($("#currentM") != 'undefined') ? $("#currentM").val() : 0;
	var h = ($("#currentH") != 'undefined') ? $("#currentH").val() : 0;
	for (var i = 0; i < 60; i++) {
		$(".minutes").append('<option value="' + i + '" ' + ((m == i) ? ' selected' : ' ') + '>' + ((i >= 10) ? i : '0' + i) + '</option>');
		if (i < 24)
			$(".hours").append('<option value="' + i + '" ' + ((h == i) ? ' selected' : ' ') + '>' + ((i >= 10) ? i : '0' + i) + '</option>');
	}
}
updateTimeInputs();
var countOfelements = 0;
$("#add_element").click(function () {
	var html = '<div class="lesson-time" id="element_' + (++countOfelements) + '" > <div class="editor-label"> <label>Время ' + (countOfelements + 1) + '-го звонка:</label> </div> <select class="hours" id="Lessons_' + countOfelements + '__Hours" name="Lessons[' + countOfelements + '].Hours"> </select> : <select class="minutes" id="Lessons_' + countOfelements + '__Minutes" name="Lessons[' + countOfelements + '].Minutes"> </select> </div>';
	$("#element-fields").append(html);
	updateTimeInputs();
});
$("#remove_element").click(function () {
	if (countOfelements > 0)
		$("#element_" + (countOfelements--)).remove();
});