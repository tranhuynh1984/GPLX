var DMDoiTuong = function () {
    let base = this;
    let $tableSelector = $('#tblList');
    let $dropStats = $('#requestSearchStats');

    base.$tableDataTable = null;
    base.$searchButton = $('#requestSearchButton');
    base.$btnSync = $('#btnSync');
    base.$exportExcel = $('#btnExportExcel');
    base.$txtMaDM = $('#txtMaDM');
    base.$txtTenDM = $('#txtTenDM');
    base.$onLoad = true;
    base.$popOverlay = $('#modal-overlay');
    base.$ddlStatus = $('#ddlStatus option:selected');
    base.$btnCreate = $('#btnCreate');

    base.$txtmadoituong = $('#txtmadoituong');

    base.recordSelector = null;

    base.recordSelectorIndex = -1;

    base.pageLanguage = languages.vi.DMDoiTuong;

    base.settingPage = {
        pop: {
            url: '',
            title: base.pageLanguage.list.popup.title,
            baseTitle: base.pageLanguage.list.popup.baseTitle,
            viewLabel: base.pageLanguage.list.popup.viewLabel,
            createLabel: base.pageLanguage.list.popup.createLabel,
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

        base.$exportExcel.bind("click", function () {
            base.exportExcel();
        })

        base.$btnSync.bind('click',
            function () {
                base.onSync(this);
            });
    }

    base.searchForm = function (data) {
        if (typeof (data) !== 'undefined') {
            data.MaDM = base.$txtMaDM.val();
            data.TenDM = base.$txtTenDM.val();
            data.Status = $('#ddlStatus option:selected').val();
            base.setParamsAfterSearch(data);

            $('#txtMaDM').val($.trim(data.MaDM));
            $('#txtTenDM').val($.trim(data.TenDM));
        }
    }

    base.setupDataTable = function () {

        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            ajax: {
                url: "/DMDoiTuong/Search",
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
                        return row.maDM;
                    },
                    width: "5%"
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return row.tenDM;
                    },
                    width: "10%"
                },
                {
                    idx: 3,
                    render: function (data, type, row, meta) {
                        return row.isActiveName;
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 4,
                    render: function (data, type, row, meta) {
                        return row.stt;
                    },
                    width: "10%",
                    data: 'order'
                },
                {
                    idx: 5,
                    render: function (data, type, row, meta) {
                        return row.createby;
                    },
                    width: "5%",
                    data: 'order'
                },
                {
                    idx: 6,
                    render: function (data, type, row, meta) {
                        return moment(row.createdate).format('HH:mm DD/MM/yyyy')
                    },
                    width: "5%",
                    data: 'order'
                },
                {
                    idx: 7,
                    render: function (data, type, row, meta) {
                        return row.updateby;
                    },
                    width: "5%",
                    data: 'order'
                },
                {
                    idx: 8,
                    render: function (data, type, row, meta) {
                        return row.updatedate == null ? "" : moment(row.updatedate).format('HH:mm DD/MM/yyyy')
                    },
                    width: "5%",
                    data: 'order'
                },
                {
                    idx: 9,
                    render: function (data, type, row, meta) {
                        let s = '';
                        s = '<a class="mr-2" href="javascript:void(0)" title="Chỉnh sửa thông tin đối tượng" prop-type="elems.table.edit" prop-type-act="pop" ><i class="fad fa-pencil med-theme-primary-button"></i></a>'
                        s += '<a class="fs-17 text-info mr-2" prop-type="elems.table.info" prop-type-act="pop" title="Chi tiết ĐT" href="javascript:void(0)"><i class="fad fa-info-square"></i></a>'
                        s += '<a class="text-danger mr-2" prop-type="elems.table.delete" prop-type-act="confirm" title="Xóa ĐT" href="javascript:void(0)"><i class="fad fa-ban"></i></a>'
                        return s;
                    },
                    width: "5%",
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

    base.exportExcel = function () {
        let data = {};
        base.searchForm(data);
        costJsBase.ExportExcel('/DMDoiTuong/ExportExcel', data);
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

        base.$btnCreate.bind("click",
            function () {
                base.settingPage.fnUPop(base.settingPage.pop.createLabel);
                // cấu hình để tạo các nút trên popup
                // đăng ký event cho từng nút
                base.settingPage.fnButtons({
                    //duyệt
                    accept: {
                        visible: true,
                        listener: {
                            event: function () {
                                base.onCreate(this);
                            }
                        },
                        html: `<button type="button" prop-type="elems.accept" class="btn btn-outline-primary btn-sm">
                                <i class="fad fa-check mr-2" aria-hidden="true"></i> Lưu lại
                            </button>`,
                    },
                    //đóng
                    close: { visible: true }
                });
                base.settingPage.fnURI('/DMDoiTuong/Create');
                base.settingPage.overlay = true;
                costJsBase.OpenModal(base.settingPage.pop);

            });

        base.$txtmadoituong.bind("active",
            function () {
                console.log(222);
                this.removeClass('itemerror');

            });

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
        let params = $.fn.dataURI(window.location.href);
        jQuery.fn.setElementValue('#requestSearchStats', params["stats"], function () {
            base.statsChange(params["stats"]);
        });
        jQuery.fn.setElementValue('#selectReportForWeek', params["w"]);

        deferred.resolve();
        return deferred.promise();
    }

    base.drawInit = () => $('#tblList [prop-type]').on('click', base.tabRecordFuncHandle);

    base.tabRecordFuncHandle = function () {
        // prop-type [{xyz}.target.action]
        let propAct = $(this).attr('prop-type-act');
        let propTypes = $(this).attr('prop-type');
        let separators = propTypes.split('.');
        let action = separators[2];
        let point = $(this).closest('tr');
        var record = base.$tableDataTable.row(point).data();
        base.recordSelectorIndex = base.$tableDataTable.row(point).index();
        base.recordSelector = record;
        //record.id
        //todo: 
        if (typeof (propAct) !== 'undefined') {
            switch (propAct) {
                case "pop":
                    switch (action) {
                        // xem chi tiết
                        case "info":
                            base.settingPage.fnUPop(base.settingPage.pop.viewLabel);
                            // cấu hình để tạo các nút trên popup
                            // đăng ký event cho từng nút
                            base.settingPage.fnButtons({
                                close: { visible: true }
                            });
                            base.settingPage.fnURI('/DMDoiTuong/Create?viewMode=view&record=' + jQuery.fn.aesToParams(record.maDM));
                            base.settingPage.overlay = true;
                            break;
                        case "edit":
                            base.settingPage.fnUPop(base.settingPage.pop.editLabel);
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
                                                <i class="fad fa-check mr-2" aria-hidden="true"></i> Lưu lại
                                            </button>`
                                },
                                //đóng
                                close: { visible: true }
                            });
                            base.settingPage.fnURI('/DMDoiTuong/Create?viewMode=edit&record=' + jQuery.fn.aesToParams(record.maDM));
                            base.settingPage.overlay = true;
                            break;
                        default:
                            break;
                    }
                    costJsBase.OpenModal(base.settingPage.pop);
                    break;
                case "confirm":
                    Swal.fire({
                        title: 'Bạn có chắc chắn muốn xóa đối tượng này?' + '</br> Bản ghi xóa: ' + record.maDM,
                        icon: 'warning',
                        showDenyButton: false,
                        showConfirmButton: true,
                        showCancelButton: true,
                        confirmButtonText: `<i class="fad fa-check"></i> Xác nhận`,
                        cancelButtonText: 'Hủy bỏ',
                        position: 'top',
                        showLoaderOnConfirm: true,
                        didOpen: () => { },
                        preConfirm: function () {
                            var deferred = $.Deferred();
                            costJsBase.PromissePost({
                                Url: '/DMDoiTuong/OnRemove',
                                Data: {
                                    maDM: record.maDM
                                }
                            }).then(data => {
                                if (data.code !== costJsBase.enums.successCode) {
                                    Swal.showValidationMessage(data.message);
                                    deferred.resolve(false);
                                } else {
                                    base.searching();
                                    deferred.resolve(data.message);
                                }
                            }).fail(e => {
                                Swal.showValidationMessage('Lỗi hệ thống, vui lòng thử lại sau!');
                                deferred.resolve(false);
                            });
                            return deferred.promise();
                        }
                    }).then((result) => {
                        if (result.isConfirmed) {
                            Swal.fire({
                                title: result.value,
                                icon: 'success',
                                timer: 1500,
                                timerProgressBar: true,
                                didOpen: () => {
                                    Swal.showLoading();
                                    timerInterval = setInterval(() => {
                                    },
                                        100);
                                },
                                willClose: () => {
                                    clearInterval(timerInterval)
                                }
                            });
                        }
                    });
                    break;
                default:
                    break;
            }
        }
    }
    // function duyệt / từ chối yêu cầu

    base.onSync = function (target) {
        costJsBase.Post({
            Url: '/DMDoiTuong/OnSync',
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

    base.onCreate = function (target) {
        costJsBase.Post({
            Url: '/DMDoiTuong/OnCreate', Data: {
                Record: $('#___record').val(),
                IsActive: $('#ddlStatusdoituong option:selected').val(),
                MaDM: $('#txtmadoituong').val(),
                TenDM: $('#txttendoituong').val(),
                Stt: $('#txtstt').val()
            },
            beforeSend: function () {
                costJsBase.ButtonState({
                    target: $(target),
                    state: 'loading',
                    disabled: false,
                    text: 'Đang lưu',
                    changePropAllButton: true
                });
            }
        }, function (data) {
            if (data.code !== costJsBase.enums.successCode) {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
                costJsBase.ButtonState({
                    target: $(target),
                    state: 'normal',
                    disabled: false,
                    html: '<i class="fad fa-plus-circle"></i> Lưu lại',
                    changePropAllButton: false
                });

                costJsBase.ShowValidation(data.listError);

            } else {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
                base.searching();
                setTimeout(function () {
                    base.$popOverlay.modal('hide');
                }, 1000);
            }

        }, function () {
            costJsBase.ButtonState({
                target: $(target),
                state: 'normal',
                disabled: false,
                html: '<i class="fad fa-check mr-2" aria-hidden="true"></i> Lưu lại',
                changePropAllButton: true
            });
            costJsBase.EventNotify('error', 'Lỗi hệ thống, vui lòng thử lại sau!')
        })
    }
}

//define if page use modal

$(document).ready(function () {
    let c = new DMDoiTuong();
    c.Setup();
    $('[data-toggle="tooltip"]').tooltip();
});