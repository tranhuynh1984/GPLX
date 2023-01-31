var DMPN = function () {
    let base = this;
    let $tableSelector = $('#tblList');
    let $dropStats = $('#requestSearchStats');

    base.$tableDataTable = null;
    base.$searchButton = $('#requestSearchButton');
    base.$exportExcel = $('#btnExportExcell');
    base.$btnSync = $('#btnSync');
    base.$txtMaDonVi = $('#txtMaDonVi');
    base.$txtTenDonVi = $('#txtTenDonVi');
    base.$onLoad = true;
    base.$popOverlay = $('#modal-overlay');
    base.$ddlStatus = $('#ddlStatus option:selected');
    base.$ddlDMPN = $('#ddlDMPN option:selected');

    base.recordSelector = null;

    base.recordSelectorIndex = -1;

    base.pageLanguage = languages.vi.DMDV;

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
                )
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
            function() {
                base.onSync(this);
            });
    }

    base.searchForm = function (data) {
        if (typeof (data) !== 'undefined') {
            data.MaDonVi = base.$txtMaDonVi.val();
            data.TenDonVi = base.$txtTenDonVi.val();
            data.Status = $('#ddlStatus option:selected').val();
            data.PhapNhanId = $('#ddlDMPN option:selected').val();
            base.setParamsAfterSearch(data);

            $('#txtMaDonVi').val($.trim(data.MaDonVi));
            $('#txtTenDonVi').val($.trim(data.TenDonVi));
        }
    }

    base.setupDataTable = function () {

        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            ajax: {
                url: "/DMDV/Search",
                data: function (d) {
                    base.searchForm(d);
                }
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
                        return row.maDV;
                    },
                    width: "5%"
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return row.tenDV;
                    },
                    width: "10%"
                },
                {
                    idx: 3,
                    render: function (data, type, row, meta) {
                        return row.phapNhanId;
                    },
                    width: "10%"
                },
                {
                    idx: 4,
                    render: function (data, type, row, meta) {
                        return row.phapNhanName;
                    },
                    width: "10%"
                },
                {
                    idx: 5,
                    render: function (data, type, row, meta) {
                        return row.isActiveName;
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 6,
                    render: function (data, type, row, meta) {
                        return row.maSAP;
                    },
                    width: "10%",
                    data: 'order'
                },
                {
                    idx: 7,
                    render: function (data, type, row, meta) {
                        return row.maDVExSap;
                    },
                    width: "10%",
                    class: "text-center",
                    data: 'order'
                }

            ],
            drawCallback: function () {
                base.drawInit();
            },
            scrollX: true,
            responsive: false,
        });
    }

    base.statsChange = function (val) {
        var deferred = $.Deferred();
        $dropStats.find('a').removeAttr('selected');

        if (typeof (val) === 'string') {
            let truth = $dropStats.find('a[prop-stats="' + val + '"]');
            if (truth.length) {
                base.$labelStats.text(truth.text())
                truth.attr("selected", true);
            }
        } else if (typeof (val) !== 'undefined') {
            base.$labelStats.text($(val).text())
            $(val).attr("selected", true);
        }

        deferred.resolve();
        return deferred.promise();
    }

    base.bindActions = function () {

        base.$searchButton.bind("click", function () {
            base.searching(true);
        });

        base.$exportExcel.bind("click", function () {
            base.exportExcel();
        })

        $.fn.eEnterActions({
            selector: base.$requestSearchKeywords, action: function (elm) {
                base.searching(true);
            }
        });
    }

    base.searching = function (resetPage) {
        base.$onLoad = false;
        if (base.$tableDataTable)
            base.$tableDataTable.ajax.reload(null, resetPage);
    }

    base.exportExcel = function () {
        let data = {};
        base.searchForm(data);
        costJsBase.ExportExcel('/DMDV/ExportExcel', data);
    }

    base.setParamsAfterSearch = function (forms) {
        if (base.$onLoad)
            return false;
        let url = URI(window.location.href);
        let urlBuilder = url.toString();
        urlBuilder = $.fn.buildParamURI(urlBuilder, "stats", forms.Status, !typeof (forms.Status) !== 'undefined');
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
                                    //if (base.$createForm.on().valid())
                                    //    base.onCreate(this);
                                }
                            },
                            html: `<button type="button" prop-type="elems.accept" class="btn btn-outline-primary btn-sm">
                                                <i class="fad fa-check mr-2" aria-hidden="true"></i> Cập nhật
                                            </button>`
                        },
                        //đóng
                        close: { visible: true }
                    });
                    base.settingPage.fnURI('/DMPN/SetInOutView?code=' + record.officesCode);
                    base.settingPage.overlay = true;
                    costJsBase.OpenModal(base.settingPage.pop);
                    break;
                default:
                    break;
            }
        }
    }
    // function duyệt / từ chối yêu cầu

    base.onSync = function (target) {
        costJsBase.Post({
            Url: '/DMPN/OnSync',
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
    let c = new DMPN();
    c.Setup();
    $('[data-toggle="tooltip"]').tooltip();

    //$(document).on('click', '#parentId', function () {
    //    var a = this.find('a').first();
    //    a.click();
    //});

});