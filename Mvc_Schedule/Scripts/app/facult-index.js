var lastFocus = null;
$(document).ready(function () {
    SlidePanel($("#list .title").first());
    $(".title").click(function () {
        SlidePanel($(this));
    });
});
function UpdateGroupList(data, div) {
    div.html('');
    if (data != null && data.length != 0) {
        var title = div.prev();
        var id = title.attr("id");
        var ex = title.find("span.export");
        ex.html('');
        ex.append('Excel: <a href="/Schedule/Excel/' + id + '/2" >чётная' +
            '</a><a href="/Schedule/Excel/' + id + '/1" >нечётная</a>');
        $.each(data, function (index, d) {
            div.append('<div class="group_title"><span class="col">' + d.Name + '</span><a href="/Groups/Edit/' + d.Id + '">правка</a>' +
            '<span class="edit"><a href="/plans/' + d.Id + '">План</a> Расписание: <a href="/schedule/create/' + d.Id + '/2">чётная</a>' +
            '<a href="/schedule/create/' + d.Id + '/">нечётная</a></span></div><div class="ending"></div>');
        });
        //$(".group_title:odd").css("border-bottom", "1px solid #FFCCCC");
    } else {
        div.append('<div class="group_title">Групп нет</div>');
    }
}
function SlidePanel(element) {
    if (element.parents('li').hasClass("focus") || element == null) return;
    var facultId = element.attr("id");
    element.parents('li').addClass("focus");
    element.find('span').css('visibility', 'visible');
    var gmenu = element.parents("li").children(".group_menu");
    $.ajax({
        type: "POST",
        contentType: "application/json;charset=utf-8",
        url: 'Default/GetGroups/',
        data: '{"id":"' + facultId + '"}',
        dataType: "json",
        success: function (data) {
            UpdateGroupList(data, gmenu);
            gmenu.stop();
            gmenu.slideDown();
        }
    });
    if (lastFocus != null) {
        lastFocus.parents("li").removeClass();
        lastFocus.parents("li").children(".group_menu").stop().slideUp();
        lastFocus.find('.edit').css('visibility', 'hidden');
    }
    lastFocus = element;
}