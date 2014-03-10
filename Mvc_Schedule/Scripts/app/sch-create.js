var schCreate = function (conf) {

    if (this.isActivated) return this.model; // 'неадекватный' кусочек синглтона
    var x = this;

    // Yes Cap that's 'Constructor'!
    return constructor();

    function constructor() {
        x.isActivated = true;
        x.editClass = { on: '<i class="fa fa-eye"></i> В обычный режим', off: '<i class="fa fa-cogs"></i> В режим редактора', };//x.editClass = (conf && conf.editClass) || { on: "edit-on", off: "edit-off", };
        x.block = $("#legend");
        x.editBtn = $('<a id="sch-edit-btn" class="btn " href="#edit">' + x.editClass.off + '</a>');
        x.isEdit = false;
        x.model = {
            onActivate: [(conf && conf.update)],//|| function() { DlgHelper.ShowDialogError("Обновите! см. консоль."); console.log("не передан загрузчик"); }],
            activate: function () {
                $.each(this.onActivate, function (i, t) { t(); });
            },
        };
        
        prepareLayout();

        return x.model;
    }

    function prepareLayout() {
        x.editBtn.insertAfter(x.block);
        x.editBtn.click(function () {
            if (x.isEdit)
                deactivateEdit();
            else
                activateEdit();
            x.isEdit = !x.isEdit;
        });

        //conf && conf.getList() || console.log("не задан метод получения записей");
    }

    function activateEdit() {
        x.editBtn.html(x.editClass.on);
        
    }

    function deactivateEdit() {
        x.editBtn.html(x.editClass.off);
        
    }


    function bindControls(list) {

        // Добавление новой строки


        /* @lesson_subrow# */

        // Добавление

        // Изменение

        // Удаление

    }
};