var Users = function () {
    let base = this;
    let $tableSelector = $('#tblList');

    base.$tableDataTable = null;
    base.$searchButton = $('#requestSearchButton');
    base.$selectUnits = $('#selectUnits');
    base.$selectDeparts = $('#selectDeparts');
    base.$selectGroups = $('#selectGroups');
    base.$btnSync = $('#btnSync');


    base.$requestSearchKeywords = $('#requestSearchKeywords');
    base.$onLoad = true;
    base.$popOverlay = $('#modal-overlay');

    base.recordSelector = null;

    base.recordSelectorIndex = -1;

    base.pageLanguage = languages.vi.groups;

    base.settingPage = {
        pop: {
            url: '',
            title: base.pageLanguage.list.popup.title,
            baseTitle: base.pageLanguage.list.popup.baseTitle,
            viewLabel: base.pageLanguage.list.popup.viewLabel,
            historyLabel: base.pageLanguage.list.popup.historyLabel,
            editLabel: base.pageLanguage.list.popup.editLabel,
            selector: base.$popOverlay,
            replator: base.pageLanguage.list.popup.replator,
            buttons: {
                accept: {
                    visible: false
                },
                decline: { visible: false },
                close: { visible: false }
            },
            dropContent: true,
            callback: function () {
                base.$createForm = $('#___createForm');

                jQuery.fn.validateSetup(base.$createForm,
                    {
                        rules: {
                            selectTypes: { required: true },
                        },
                        messages: {
                            selectTypes: { required: 'Bạn chưa chọn nhóm đơn vị' }
                        }
                    }
                );

                $('#selectUnitSettings').select2({ 'width': '100%', themes: 'bootstrap4' });
                $('#selectSettingGroups').select2({ 'width': '100%', themes: 'bootstrap4' });
                $('select[ssl=unit-currently]').select2({ 'width': '100%', themes: 'bootstrap4' });
                $('select[ssl=group-currently]').select2({ 'width': '100%', themes: 'bootstrap4' });

                $('[data-toggle="tooltip"]').tooltip();
            }
        },
        redirect: {

        },
        fnUPop: function (act) {
            this.pop.title = this.pop.baseTitle.replace(this.pop.replator, act);
        },
        fnURI: function (uri) {
            if (typeof (uri) == 'string') {
                this.pop.url = uri
            }
        },
        fnButtons: function (btns) {
            this.pop.buttons = btns;
        }
    };

    base.Setup = function () {
        base.bindActions();
        base.setupDataTable();
        base.$btnSync.bind('click',
            function () {
                base.onSync(this);
            });

        $(document).on('click', '#__addUnitCurrently', function () {
            var source = document.getElementById("blockUnitCurrently").innerHTML;
            var template = Handlebars.compile(source);
            var stringID = costJsBase.Rad(20);
            var context = { newID: stringID };
            var html = template(context);

            $('#_boxUnitCurrently').append(html);
            $('#' + stringID).find('select[ssl=unit-currently]').select2({ 'width': '100%', themes: 'bootstrap4' });
            $('#' + stringID).find('select[ssl=group-currently]').select2({ 'width': '100%', themes: 'bootstrap4' });
        });

        $(document).on('click', 'button[ssl="del"]', function () {
            if (confirm('Bạn có chắc chắn muốn xóa dữ liệu này ?')) {
                $(this).closest('.row').remove();
            }
        });
    }

    base.searchForm = function (data) {
        if (typeof (data) !== 'undefined') {
            data.Keywords = base.$requestSearchKeywords.val();
            data.UnitRecord = base.$selectUnits.val();
            data.GroupRecord = base.$selectGroups.val();
            data.DepartmentRecord = base.$selectDeparts.val();
            base.setParamsAfterSearch(data);
        }
    }

    base.setupDataTable = function () {
        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            ajax: {
                url: "/Users/Search",
                data: function (d) {
                    base.searchForm(d);
                },
                type: 'post'
            },
            columns: [
                {
                    idx: 0,
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    },
                    width: "5%"
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return row.userId;
                    },
                    width: "10%"
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return row.userCode || '-';
                    },
                    width: "8%",
                    class: "text-center"
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return row.userPhone || '-';
                    },
                    width: "8%",
                    class: "text-center"
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return row.userEmail || '-';
                    },
                    width: "8%",
                    class: "text-center",
                    data: 'userEmail'
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return row.unitName || '-';
                    },
                    class: "text-left",
                    data: 'unitName'
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return row.departmentName || '-';
                    },
                    width: "8%",
                    class: "text-center",
                    data: 'departmentName'
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return moment(row.createdate).format('DD/MM/YYYY');
                    },
                    class: "text-center"
                },
                {
                    idx: 5,
                    render: function (data, type, row, meta) {
                        let s = '';
                        s += '<a class="text-info mr-2" prop-type="elems.table.edit" prop-type-act="pop" title="Bổ sung thông tin" href="javascript:void(0)"><i class="fad fa-wrench"></i></a>';
                        return s;
                    },
                    class: "text-center"
                }
            ],
            drawCallback: function () {
                base.drawInit();
            },
            scrollX: true,
            responsive: false,
        });
    }


    base.bindActions = function () {
        base.$searchButton.bind("click", function () {
            base.searching();
        });
        $.fn.eEnterActions({
            selector: base.$requestSearchKeywords, action: function (elm) {
                base.searching();
            }
        });
    }

    base.searching = function (reload) {
        if (typeof reload === 'undefined')
            reload = true;
        base.$onLoad = false;
        if (base.$tableDataTable)
            base.$tableDataTable.ajax.reload();
    }

    base.setParamsAfterSearch = function (forms) {
        if (base.$onLoad)
            return false;
        let url = URI(window.location.href);
        let urlBuilder = url.toString();
        urlBuilder = $.fn.buildParamURI(urlBuilder, "stats", forms.Status, typeof (forms.Status) !== 'undefined');
        urlBuilder = $.fn.buildParamURI(urlBuilder, "unit", forms.UserUnit, !(typeof (forms.UserUnit) !== 'undefined'));
        urlBuilder = $.fn.buildParamURI(urlBuilder, "w", forms.ReportForWeek, !(typeof (forms.ReportForWeek) !== 'undefined'));
        window.history.pushState("", "", urlBuilder);
    }

    base.readURI = function () {
        var deferred = $.Deferred();
        deferred.resolve();
        return deferred.promise();
    }

    base.drawInit = () => $('#tblList [prop-type]').on('click', base.tabRecordFuncHandle);

    base.tabRecordFuncHandle = function () {
        let propAct = $(this).attr('prop-type-act');
        let point = $(this).closest('tr');
        var record = base.$tableDataTable.row(point).data();
        base.recordSelectorIndex = base.$tableDataTable.row(point).index();
        base.recordSelector = record;
        if (typeof (propAct) !== 'undefined') {
            switch (propAct) {
                case "pop":
                    base.settingPage.fnUPop('Cấu hình nhóm đơn vị');
                    // cấu hình để tạo các nút trên popup
                    // đăng ký event cho từng nút
                    base.settingPage.fnButtons({
                        //duyệt
                        accept: {
                            visible: true, listener: {
                                event: function () {
                                    base.onCreate(this);
                                }
                            },
                            html: `<button type="button" prop-type="elems.accept" class="btn btn-outline-primary btn-sm">
                                                <i class="fad fa-check mr-2" aria-hidden="true"></i> Cập nhật
                                            </button>`
                        },
                        //đóng
                        close: { visible: true }
                    });
                    base.settingPage.fnURI('/Users/Settings?record=' + jQuery.fn.aesToParams(record.record));
                    base.settingPage.overlay = true;
                    costJsBase.OpenModal(base.settingPage.pop);
                    break;
                default:
                    break;
            }
        }
    }

    base.onCreate = function (target) {
        var fm = new FormData();
        fm.append("Record", $('#___record').val());
        fm.append("GroupRecords", $('#selectSettingGroups').val());
        fm.append("DepartmentRecord", $('#selectSettingDeparts').val());
        fm.append("Units", $('#selectUnitSettings').val());

        let currently = [];
        let $unitCurrently = $('select[ssl="unit-currently"]');
        let anyError = false;
        if ($unitCurrently.length) {
            $unitCurrently.each(function () {
                let $unit = $(this).val();
                let $groups = $(this).closest('.row').find('select[ssl="group-currently"]').val();

                if (!$unit.length || !$groups.length) {
                    anyError = true;
                    $(this).focus();
                }
                currently.push({
                    UnitCode: $unit,
                    GroupId: $groups
                });
            });
        }
        if (anyError) {
            costJsBase.EventNotify('warning', 'Bạn chưa chọn đủ thông tin kiêm nhiệm!')
            return false;
        }

        fm.append("Currently", JSON.stringify(currently));

        costJsBase.JsUploadFile({
            selector: $('#signature'),
            Url: '/Users/OnSetting',
            Data: fm,
            beforeSend: function () {
                costJsBase.ButtonState({
                    target: $(target),
                    state: 'loading',
                    disabled: true,
                    text: 'Đang lưu',
                    changePropAllButton: true
                });
            }
        },
        function (data) {
                if (data.code === costJsBase.enums.successCode) {
                    costJsBase.EventNotify('success', data.message);
                    base.searching(false);
                    setTimeout(function () {
                        base.$popOverlay.modal('hide');
                    },
                        1000);
                } else {
                    costJsBase.ButtonState({
                        target: $(target),
                        state: 'normal',
                        disabled: false,
                        html: '<i class="fad fa-check mr-2" aria-hidden="true"></i> Lưu lại',
                        changePropAllButton: true
                    });
                    costJsBase.EventNotify('error', data.message);
                }
            },
            function () {
                costJsBase.EventNotify('error', 'Lỗi hệ thống, vui lòng thử lại sau!');
                costJsBase.ButtonState({
                    target: $(target),
                    state: 'normal',
                    disabled: false,
                    html: '<i class="fad fa-check mr-2" aria-hidden="true"></i> Lưu lại',
                    changePropAllButton: true
                });
            });
    }

    base.onSync = function (target) {
        costJsBase.Post({
            Url: '/Users/OnSync',
            Data: {},
            beforeSend: function () {
                costJsBase.ButtonState({
                    target: $(target),
                    state: 'loading', //loading, done, normal
                    disabled: true,
                    text: 'Đang đồng bộ ...',
                    spinner: '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> __TEXT__',
                    html: '',
                    changePropAllButton: false
                });
            }
        }, function (data) {
            costJsBase.ButtonState({
                target: $(target),
                state: 'normal',
                disabled: false,
                html: '<i class="fad fa-plus-circle"></i> Đồng bộ dữ liệu',
                changePropAllButton: false
            });
            costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
            base.searching(true);
        }, function (err) {
            costJsBase.ButtonState({
                target: $(target),
                state: 'normal',
                disabled: false,
                html: '<i class="fad fa-plus-circle"></i> Đồng bộ dữ liệu',
                changePropAllButton: false
            });
            costJsBase.EventNotify('error', 'Lỗi hệ thống, vui lòng thử lại sau!');
        });
    }
}

//define if page use modal

$(document).ready(function () {
    let c = new Users();
    c.Setup();
});