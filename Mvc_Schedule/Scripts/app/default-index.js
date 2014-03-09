function loadGroups(data) {
	var result = "Выберите группу";
	var groupsDiv = $("#groups ul");
	groupsDiv.html("");
	if (data != null && data.length > 0) {
		$.each(data, function (index, d) {
			var group = '<li><a href="/Schedule/Index/' + d.Id + '">' + d.Name + '</a></li>';
			groupsDiv.append(group);
		});
	} else {
		result = "Извините, данный факультет в разработке";
		$("#groups ul").append('<li><a href="#">Групп нет</a></li>');
	}
	$("#page-title").html(result);
}
var lastId = NaN;
$(document).ready(function () {
	$("#groups").hide();
	$("#facults ul li a").click(function () {
		$.each($("#facults ul li a"), function (index, f) {
			if (f.className == "selected") f.className = "";
		});
		$(this).addClass("selected");
		var facultId = $(this).attr("id");
		if (lastId == facultId) return false;
		lastId = facultId;
		$("#groups").fadeOut(function () {
			$.ajax({
				type: "POST",
				contentType: "application/json;charset=utf-8",
				url: "/Default/GetGroups",
				data: '{"id":"' + facultId + '"}',
				dataType: "json",
				success: function (data) {
					loadGroups(data);
					$("#groups").fadeIn();
				}
			});
		});
		return false;
	});
	$("#facults").fadeIn();
});