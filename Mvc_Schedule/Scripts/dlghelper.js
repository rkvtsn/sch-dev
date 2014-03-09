DlgHelper = {
    dialog: $("#dialog"),
    dialog_p: $("#dialog p"),
    warning: $("#warning"),
    formDiv: $("#dialog_form"),
    warnBg: $("#warning_bg"),
    msg: $("#msg"),
    okAction: function() {},
    delAction: function(caller) {},
    GenerateForm: function() {},

    _show: function(obj) {
        obj.stop();
        obj.fadeIn();
        obj.show();
        return obj;
    },
    _hide: function(obj) {
        obj.stop();
        obj.fadeOut();
        obj.hide();
        return obj;
    },

    ShowForm: function() {
        this.HideDialog();
        this._show(DlgHelper.warnBg);
        return this._show(DlgHelper.formDiv);
    },
    HideForm: function() {
        this._hide(DlgHelper.warnBg);
        return this._hide(DlgHelper.formDiv);
    },

    ___oncreate: {
        closeDialogBind: $("#close_dialog").bind("click", function() { DlgHelper.HideDialog(); }),
        warningHideBind: $("#warning_n, #dialog").click(function() { $(".warning").hide(); }),
        cancelBind: $("#cancel").click(function() {
            DlgHelper.HideForm();
            console.log('[Cancel]');
        }),
        okBind:
            $("#ok").click(function() {
                if (DlgHelper.__isValid) {
                    DlgHelper.HideForm();
                    DlgHelper.okAction();
                    console.log('[OK]');
                } else {
                    DlgHelper.ShowDialogError("Неверный ввод!");
                }
            }),
        warningYBind: $("#warning_y").click(function() {
            DlgHelper.warning.hide();
            DlgHelper.warnBg.hide();
            DlgHelper.ShowDialog("Удаляю.. ");
            DlgHelper.delAction(warning._delCaller);
            console.log('Deleting');
        }),
    },


    AjaxAction: function(ajaxUrl, ajaxType, successFn) {
        var form = this.formDiv.serialize();
        this.AjaxActionWithData(ajaxUrl, ajaxType, successFn, form);
        //$.ajax({
        //    type: ajaxType,
        //    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        //    url: ajaxUrl,
        //    data: form,
        //    success: function(data) {
        //        successFn(data);
        //    }
        //});
    },

    AjaxActionWithData: function(ajaxUrl, ajaxType, successFn, d) {
        $.ajax({
            type: ajaxType,
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            url: ajaxUrl,
            data: d,
            success: function(data) {
                successFn(data);
            }
        });
    },

    onUpdate: function(listDiv, ajaxUrl, ajaxType, eachFn, successFn, afterFn) {
        listDiv.html('');
        DlgHelper.ShowDialog("Подготавливаю список...");
        DlgHelper.AjaxAction(ajaxUrl, ajaxType, function(data) {
            if (data != null && data.length > 0) {
                $.each(data, function(i, x) {
                    eachFn(i, x);
                });
                listDiv.show();
                successFn();
            } else {
                DlgHelper.ShowDialog("Данных нет");
            }
            afterFn();
        });
    },

    onUpdateWithData: function(listDiv, ajaxUrl, ajaxType, d, eachFn, successFn, afterFn) {
        listDiv.html('');
        DlgHelper.ShowDialog("Подготавливаю список...");
        DlgHelper.AjaxAction(ajaxUrl, ajaxType, function(data) {
            if (data != null && data.length > 0) {
                $.each(data, function(i, x) {
                    eachFn(i, x);
                });
                listDiv.show();
                successFn();
            } else {
                DlgHelper.ShowDialog("Данных нет");
            }
            afterFn();
        }, d);
    },

    // @валидация
    __isValid: true,
    _failedValidation: function(obj) {
        obj.addClass("input-validation-error");
        if (this.__isValid)
            $('<span class="validation-error">Неверный ввод!</span>').insertAfter(this.msg);
        this.__isValid = false;
    },
    _successValidation: function(obj) {
        obj.removeClass("input-validation-error");
        if (!this.__isValid)
            $('.validation-error').remove();
        this.__isValid = true;
    },
    _isEnableValidation: false,
    EnableValidation: function() {
        console.log("[Validation] is Enabled");
        var x = this;
        this._isEnableValidation = true;
        $("#clearable input").each(function() {
            $(this).blur(function() {
                if ($(this).prop("required"))
                    if ($(this).val().trim() == "")
                        x._failedValidation($(this));
                    else
                        x._successValidation($(this));
            });
        });
        return x;
    },
    ClearForm: function() {
        if (this._isEnableValidation) {
            this.__isValid = true;
            $('.validation-error').remove();
            $('#clearable input').removeClass("input-validation-error");
        }
        this.formDiv.find("#clearable input").each(function() {
            if ($(this).attr("type") == "number") $(this).val($(this).attr('min') || 0);
            else if ($(this).attr("type") == "text") $(this).val('');
            else if ($(this).attr("type") == "checkbox") $(this).prop("checked", false);
        });
        return this;
    },

    BindOn: function(generateForm, addAction, editAction, deleteAction) {
        if (generateForm == null || addAction == null || editAction == null || deleteAction == null) {
            //this.ShowDialogError("Ошибка");
            console.log("error dlg_binding");
            return null;
        }
        var x = this;
        x.GenerateForm = generateForm;
        x.delAction = deleteAction;

        //Form Dialog
        $("#add").unbind('click');
        $("#add, .edit, .add").click(function() {
            console.log('Add OR Edit : click');
            var isEdit = $(this).hasClass("edit");

            if (isEdit) {
                x.msg.html('<i class="fa fa-pencil-square"></i> Изменение:');
                x.okAction = editAction;
                console.log('[Edit]');
            } else {
                x.msg.html('<i class="fa fa-plus-square"></i> Добавление:');
                x.okAction = addAction;
                console.log('[Add]');
            }

            x.ShowForm();
            generateForm($(this), isEdit);
        });

        // Warning [DELETE]
        $(".delete").click(function() {
            $("#title_on_delete").html($(this).parent().find(".subtitle").html());
            DlgHelper.warning.show();
            DlgHelper.warnBg.show();
            warning._delCaller = $(this);
            console.log('[Delete]');
        });

        console.log('[Binding] is On');
        return this;
    },

    // Dialog
    countdown: {},
    cooldown: 3000,
    ShowDialog_display: function (msg, t) {
        if (t) { var x = this; this.countdown = setInterval(function () { x.HideDialog(); }, t); }
        this.dialog_p.html(msg);
        this._show(this.dialog);
    },
    ShowDialog: function (msg, t, icon) {
        if (!t) t = this.cooldown;
        this.HideForm();
        this.HideDialog();
        msg = (icon || '<i class="fa fa-comment-o fa-2x"></i> ') + msg;
        this.ShowDialog_display(msg, t);
    },
    ShowDialogError: function (msg, t) {
        this.ShowDialog(msg, t || this.cooldown, '<i class="fa fa-frown-o fa-2x"></i> ');
    },
    ShowDialogSuccess: function (msg, t) {
        this.ShowDialog(msg, t || this.cooldown, '<i class="fa fa-check-circle-o fa-2x"></i> ');
    },
    ShowDialogWait: function(msg) {
        this.ShowDialog(msg, 10000, '<i class="fa fa-refresh fa-spin fa-2x"></i> ');
    },
    HideDialog: function () {
        clearTimeout(this.countdown);
        this._hide(this.dialog);
        this.dialog_p.html('');
    }
};