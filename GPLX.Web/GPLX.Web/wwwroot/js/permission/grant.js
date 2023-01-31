var GrantPermission = function () {
    let base = this;
    base.permission = $('#permission');
    base.$record = $('#___record');
    base.$btnSave = $('#save');
    base.tree = null;
    base.setup = function () {
        base.tree = base.permission.tree({
            primaryKey: 'unique',
            textField: 'funcName',
            uiLibrary: 'bootstrap4',
            dataSource: '/Permission/LoadPerm?record=' + base.$record.val(),
            checkboxes: true,
            //icons: {
            //    expand: '<i class="fad fa-plus"></i>',
            //    collapse: '<i class="fad fa-minus"></i>'
            //}
        });

        base.$btnSave.click(function () {
            var checkedIds = base.tree.getCheckedNodes();
            let config = {
                Url: '/Permission/OnGrant',
                Data: {
                    record: base.$record.val(),
                    data: checkedIds
                },
                async: true,
                beforeSend: function () {
                    costJsBase.ButtonState({
                        target: base.$btnSave,
                        state: 'loading',
                        disabled: true,
                        text: 'Đang lưu',
                        changePropAllButton: true
                    });
                },
                complete: function (data) {
                    costJsBase.ButtonState({
                        target: base.$btnSave,
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
                    }, 1500);
                }, function () {
                    costJsBase.ButtonState({
                        target: base.$btnSave,
                        state: 'normal',
                        disabled: false,
                        text: 'Lưu',
                        changePropAllButton: true
                    });
                    costJsBase.EventNotify('error', 'Có lỗi xảy ra, vui lòng thử lại sau!');
                }
            );
        });
    }

}

$(document).ready(function () {
    var g = new GrantPermission();
    g.setup();
});