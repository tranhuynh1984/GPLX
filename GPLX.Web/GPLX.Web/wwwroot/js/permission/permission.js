
var Permission = function () {
    let base = this;
    let $save = $('#save');
    base.cfgLang = languages.vi.costElementItem;


    base.Save = function () {


        var data = [];
        $(".function_row").each(function () {
            var func = $(this).attr("function-id");

            var total = 0;
            $(this).find("input:checked").each(function () { // iterate through each checked element.
                total += isNaN(parseInt($(this).val())) ? 0 : parseInt($(this).val());
            });

            data.push({
                Id: func,
                Permission: total
            })
        });


        let config = {
            Url: '/Permission/Save',
            Data: {
                record: $("#group_id").val(),
                pers: data
            },
            async: true,
            beforeSend: function () {
                costJsBase.ButtonState({
                    target: $save,
                    state: 'loading',
                    disabled: true,
                    text: 'Đang lưu',
                    changePropAllButton: true
                });
            },
            complete: function (data) {
                costJsBase.ButtonState({
                    target: $save,
                    state: data.code === costJsBase.enums.successCode ? 'done' : 'normal',
                    disabled: data.code === costJsBase.enums.successCode,
                    text: data.code === costJsBase.enums.successCode ? '' : base.cfgLang.all.okButtonText,
                    html: data.code !== costJsBase.enums.successCode ? '' : '<i class="fa fa-check mr-2"></i> Đã lưu',
                    changePropAllButton: true
                });
            },
        };

        costJsBase.Post(config,
            function (data) {
                if (typeof (config.complete) === 'function')
                    config.complete(data);
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
                setTimeout(function () {
                    window.location.reload();
                },1500);
            }, function () {
                costJsBase.ButtonState({
                    target: $save,
                    state: 'normal',
                    disabled: false,
                    text: 'Lưu',
                    changePropAllButton: true
                });
                costJsBase.EventNotify('error', 'Có lỗi xảy ra, vui lòng thử lại sau!');
            }
        );
    }

    base.bindActions = function () {
        $save.bind("click", function (e) {
            base.Save();
        });
    }

    base.Setup = function () {
        base.bindActions();
    }

}

$(document).ready(function () {
    let c = new Permission();
    c.Setup();
});