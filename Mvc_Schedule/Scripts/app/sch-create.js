var schCreate = function (conf) {
    if (this.isActivated) return null;
    
    var x = this;
    x.isActivated = false;
    var editClass = (conf && conf.editClass) || { on: "edit-on", off: "edit-off", };
    x.block = (conf && conf.after) || $("#legend");
    x.editBtn = $('<a class="' + editClass.off + '" href="#">Редактировать</a>');
    x.isEdit = false;
    x.getList = conf && conf.getList() || function () { return null; };

    prepareLayout();

    return {
        
    };
    
    function prepareLayout() {
        x.editBtn.insertAfter(x.block);
        x.editBtn.click(function () {
            if (x.isEdit)         // toggle begin
                deactivateEdit();
            else
                activateEdit();
            x.isEdit = !x.isEdit; // toggle end
        });

        //conf && conf.getList() || console.log("не задан метод получения записей");
        
        x.isActivated = true;
    }

    function activateEdit() {
        x.editBtn.addClass(editClass.on);
        x.editBtn.removeClass(editClass.off);
        

    }

    function deactivateEdit() {
        x.editBtn.removeClass(editClass.on);
        x.editBtn.addClass(editClass.off);
        

    }
};