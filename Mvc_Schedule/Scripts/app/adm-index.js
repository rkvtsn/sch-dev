// А далее идёт избыточный, зависимый и нерациональный код на самом лучшем в мире JS
var lastId = NaN, countdown, counter, chgRoot = $("#changepsw").attr("href");

//function ShowDialog_display(msg) {
//    $("#dialog p").html(msg);
//    $("#dialog").show();
//}
//function ShowDialog_iter(msg) {
//    if (counter > 0) {
//        counter--;
//        if (counter > 0) {
//            countdown = setTimeout('ShowDialog_iter("' + msg + '")', 1000);
//        } else {
//            ShowDialog_display(msg);
//        }
//    }
//}
//function ShowDialog(msg, isdebug) {
//    isdebug = false;
//    if (isdebug == false) {
//        ShowDialog_display(msg);
//    } else {
//        counter = 4;
//        HideDialog();
//        ShowDialog_iter(msg);
//    }
//}
//function ShowDialogError(msg) {
//    ShowDialog_display(msg);
//}
//function HideDialog() {
//    clearTimeout(countdown);
//    $("#dialog").hide();
//    $("#dialog p").html('');
//}

// <>Подгрузка ролей<>
function loadRoles(data) {
    var result = "Отметьте доступные разделы";
    var groupsDiv = $("#groups ul");
    var hiddens = $("#hiddens");
    groupsDiv.html("");
    hiddens.html("");
    var i = 0;
    if (data != null && data.RoleChecks.length > 0) {
        var hidInput = '';
        $.each(data.RoleChecks, function (index, d) {
            var group = '<li><a href="javascript:void(0)">' +
                        '<input name="_' + d.RoleCheckId + '" id="_' + d.RoleCheckId + '" type="checkbox" ' + ((d.IsChecked) ? 'checked' : '') + '/>' +
                        '<label for="_' + d.RoleCheckId + '">' + d.Name + '</label></a></li>';
            hidInput += '<input type="hidden" name="' + d.RoleCheckId + '" value="' + d.IsChecked + '" />';
            groupsDiv.append(group);
            i++;
        });
        hiddens.append(hidInput);
        $("#_0").val(i);
        $("#saveChanges").show();
        $("#changepsw").attr("href", chgRoot + "/" + lastId);
        $(".controls").fadeIn();
    } else {
        $("#saveChanges").hide();
        $(".controls").hide();
        result = "Извините, данных нет";
        $("#groups ul").append('<li><a href="javascript:void(0)">Роли не были добавлены</a></li>');
    }

    $("input[type=checkbox]").click(function () {
        var chk = $(this).prop("checked");

        var valName = $(this).attr("name").substr(1);
        $("input[name=" + valName + "]").attr("value", chk);

        if ($(this).attr("name") == "_Admin") {
            $.each($("input[type=checkbox]"), function (index, x) {
                x.checked = false;
                $("input[name=" + x.getAttribute("name").substr(1) + "]").attr("value", false);
            });
            $(this).prop("checked", chk);
            //$("input[name=" + valName + "]").attr('checked', chk);
            $("input[name=" + valName + "]").attr('value', chk);
        } else {
            $("input[name=_Admin]").prop("checked", false);
            $("input[name=Admin]").attr("value", false);
        }
    });

    $("#page-title").html(result);
}
// </>Подгрузка ролей</>

function saveRoles(data) {
    if (data)
        DlgHelper.ShowDialogSuccess("Сохранено");
    else {
        DlgHelper.ShowDialogError("Увы, у нас ошибка");
    }
    $("#groups").hide();
    lastId = NaN;
    $.each($("#facults ul li a"), function (index, f) { if (f.className == "selected") f.className = ""; });
}

// Список пользователей
function getRoles() {
    $.each($("#facults ul li a"), function (index, f) { if (f.className == "selected") f.className = ""; });
    $(this).addClass("selected");
    var usrId = $(this).text();
    if (lastId == usrId) return false;
    DlgHelper.ShowDialog("Ищу доступные роли");
    lastId = usrId;
    $("#groups").fadeOut(function () {
        $.ajax({
            type: "POST",
            contentType: "application/json;charset=utf-8",
            url: "/Admin/GetRoles",
            data: '{"id":"' + usrId + '"}',
            dataType: "json",
            success: function (data) {
                loadRoles(data);
                $("#groups").fadeIn();
                DlgHelper.HideDialog();
            }
        });
    });
    return false;
}
$(document).ready(function () {
    $("#groups").hide();
    $("#saveChanges").hide();
    function refreshUsrs() {
        DlgHelper.ShowDialog("Минуточку... готовлю список пользователей");
        $.ajax({
            type: "POST",
            contentType: "application/json;charset=utf-8",
            url: "/Admin/RefreshUsrs",
            data: '{"id":"admin"}',
            dataType: "json",
            success: function (data) {
                var ul = $("#facults ul");
                ul.html('');
                if (data != null && data.length > 0) {
                    $.each(data, function (index, d) {
                        var li = document.createElement("LI");
                        var a = document.createElement("A");
                        var txt = document.createTextNode(d);
                        a.appendChild(txt);
                        a.setAttribute("HREF", "javascript:void(0);");
                        a.onmousedown = getRoles;
                        li.appendChild(a);
                        ul.append(li);
                    });
                }
                DlgHelper.HideDialog();
            }
        });
    }
    $("#warning_n").click(function () { $(".warning").hide(); });
    $("#deleteusr").click(function () { $("#usrname").html(lastId); $(".warning").show(); });
    $("#warning_y").click(function () {
        $(".warning").hide();
        DlgHelper.ShowDialog("Удаляю пользователя: " + lastId);
        $("#usr").attr("value", lastId);
        var form = $("#formRoles").serialize();
        $.ajax({
            type: 'POST',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            url: '/Admin/Remove',
            data: form,
            success: refreshUsrs
        });
    });


    // Сохранение
    $("#formRoles").submit(function (e) {
        e.preventDefault();
        if ($(".selected").html() == "admin") return DlgHelper.ShowDialogSuccess("Вы видимо перепутали пользователя.. <br/>Я неприкосаемый!");
        DlgHelper.ShowDialog("Сохраняю роли пользователя " + lastId);
        $("#usr").attr("value", lastId);
        var form = $("#formRoles").serialize();
        $.ajax({
            type: 'POST',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            url: '/Admin/SaveRoles',
            data: form,
            success: saveRoles
        });
    });
    
    refreshUsrs();
    $("#facults").fadeIn();
    $("#close_dialog").click(function () { DlgHelper.HideDialog(); });
});