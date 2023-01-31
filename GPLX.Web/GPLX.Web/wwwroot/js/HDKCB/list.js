var HDKCB = function () {
    let base = this;
    let $tableSelector = $('#tblList');
    let $dropStats = $('#requestSearchStats');

    base.$tableDataTable = null;
    base.$serviceDetailTableDataTable = null;
    base.$searchButton = $('#requestSearchButton');
    base.$exportExcel = $('#btnExportExcel');
    base.$btnSync = $('#btnSync');
    base.$txtIDHD = $('#txtIDHD');
    base.$txtMaHD = $('#txtMaHD');
    base.$txtTenHD = $('#txtTenHD');
    base.$txtTenHDTimNhanh = $('#txtTenHDTimNhanh');
    base.$txtMaLoai = $('#txtMaLoai');
    base.$txtND = $('#txtND');
    base.$txtNS = $('#txtNS');
    base.$onLoad = true;
    base.$popOverlay = $('#modal-extra-large');

    base.recordSelector = null;

    base.recordSelectorIndex = -1;

    base.pageLanguage = languages.vi.HDKCB;

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
            data.IDHD = base.$txtIDHD.val();
            data.MaHD = base.$txtMaHD.val();
            data.TenHD = base.$txtTenHD.val();
            data.TenHDTimNhanh = base.$txtTenHDTimNhanh.val();
            data.MaLoai = base.$txtMaLoai.val();
            data.ND = base.$txtND.val();
            data.NS = base.$txtNS.val();
            data.Status = $('#ddlStatus option:selected').val();
            base.setParamsAfterSearch(data);
        }
    }

    base.setupDataTable = function () {
        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            ajax: {
                url: "/HDKCB/Search",
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
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return row.idhd;
                    },
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return row.maHD;
                    },
                    class: "text-center"
                },
                {
                    idx: 3,
                    render: function (data, type, row, meta) {
                        return row.tenHD;
                    }
                    //class: "text-center"
                },
                {
                    idx: 4,
                    render: function (data, type, row, meta) {
                        return row.isActiveName;
                    },
                    class: "text-center"
                },
                {
                    idx: 5,
                    render: function (data, type, row, meta) {
                        return moment(row.nd).format('HH:mm DD/MM/yyyy');
                    },
                    class: "text-center"
                },
                {
                    idx: 6,
                    render: function (data, type, row, meta) {
                        return moment(row.ns).format('HH:mm DD/MM/yyyy');
                    },
                    class: "text-center"
                },
                {
                    idx: 7,
                    render: function (data, type, row, meta) {
                        return row.maLoai;
                    },
                    class: "text-center"
                },
                {
                    idx: 8,
                    render: function (data, type, row, meta) {
                        return row.updateby;
                    },
                    class: "text-center"
                },
                {
                    idx: 9,
                    render: function (data, type, row, meta) {
                        return row.updatedate == null ? "" : moment(row.updatedate).format('HH:MM DD/MM/YYYY')
                    },
                    class: "text-center"
                },
                {
                    idx: 10,
                    render: function (data, type, row, meta) {
                        let s = '';
                        s += '<a class="fs-17 text-info mr-2" prop-type="elems.table.info" prop-type-act="info" title="Chi tiết ĐT" href="javascript:void(0)"><i class="fad fa-info-square"></i></a>'
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

    base.searchingServiceDetail = function (resetPage) {
        base.$onLoad = false;
        if (base.$serviceDetailTableDataTable)
            base.$serviceDetailTableDataTable.ajax.reload(null, resetPage);
    }

    base.setupServiceDetailDataTable = function (idhd) {
        let $serviceDetailTableSelector = $('#tblServiceDetailList');
        base.$serviceDetailTableDataTable = $.fn.jsTableRegister({
            selector: $serviceDetailTableSelector,
            ajax: {
                url: "/HDKCB/ServiceDetail?id=" + idhd,
                data: function (data) {
                    data.keyword = $('#txtSearchServicePopupInput').val();
                }
            },
            columns: [
                {
                    idx: 0,
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    },
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return row.serviceCode;
                    },
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return row.serviceName;
                    },
                },
                {
                    idx: 3,
                    render: function (data, type, row, meta) {
                        return row.unit;
                    },
                    class: "text-center"
                },
                {
                    idx: 4,
                    render: function (data, type, row, meta) {
                        return row.active ? "Kích hoạt": "Vô hiệu";
                    },
                    class: "text-center"
                },
                {
                    idx: 5,
                    render: function (data, type, row, meta) {
                        return costJsBase.FormatMoney(row.servicePrice);
                    },
                    class: "text-right"
                },
                {
                    idx: 6,
                    render: function (data, type, row, meta) {
                        return costJsBase.FormatMoney(row.discountPrice);
                    },
                    class: "text-right"
                },
                {
                    idx: 7,
                    render: function (data, type, row, meta) {
                        return costJsBase.FormatMoney(row.totalPrice);
                    },
                    class: "text-right"
                }
            ],
            scrollX: true,
            responsive: false,
            serverSide: false,
            footerCallback: function (row, data, start, end, display) {
                let api = this.api();
                let responseData =  api.ajax.json();
                let totalPrice = !responseData ? 0 : responseData.totalPrice;
                $(api.column(7).footer()).html(costJsBase.FormatMoney(totalPrice));
            }
        });
    }

    base.drawInit = () => $('#tblList [prop-type]').on('click', base.tabRecordFuncHandle);

    base.tabRecordFuncHandle = function () {
        let settingPage = {
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
                    close: { visible: true }
                },
                dropContent: true,
                callback: function () {
                    base.setupServiceDetailDataTable(record.idhd);
                    $('#txtSearchServicePopupInput').keydown(function(event) {
                        let keycode = (event.keyCode ? event.keyCode : event.which);
                        if(keycode == '13'){
                            base.searchingServiceDetail(true);
                        }
                        event.stopPropagation();
                    })
                }
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
            },
            overlay: true
        };
        
        let propAct = $(this).attr('prop-type-act');
        let point = $(this).closest('tr');
        var record = base.$tableDataTable.row(point).data();
        base.recordSelectorIndex = base.$tableDataTable.row(point).index();
        base.recordSelector = record;
        if (typeof (propAct) !== 'undefined') {
            switch (propAct) {
                // xem chi tiết
                case "info":
                    settingPage.fnUPop(base.settingPage.pop.viewLabel);
                    settingPage.fnURI('/HDKCB/Detail?viewMode=view&id=' + record.idhd);
                    costJsBase.OpenModal(settingPage.pop);
                    break;
                default:
                    break;
            }
        }
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
            var _txtND = base.$txtND.val();
            var _txtNS = base.$txtNS.val();

            if (_txtND == '' && _txtNS != '') {
                let Message = "Hãy chọn ngày ký từ hợp lệ!";
                costJsBase.EventNotify('error', Message);
            }
            else if (_txtND != '' && _txtNS == '') {
                
                let Message = "Hãy chọn ngày ký đến không hợp lệ!";
                costJsBase.EventNotify('error', Message);
            }
            else if (_txtND != '' && _txtNS != '' && _txtNS < _txtND) {
                
                let Message = "Hãy chọn ngày ký từ và ngày ký đến hợp lệ!";
                costJsBase.EventNotify('error', Message);
            }
            else {
                base.searching(true);
            }
        });

        base.$exportExcel.bind("click", function() {
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

    base.exportExcel = function() {
        let data = {};
        base.searchForm(data);
        costJsBase.ExportExcel('/HDKCB/ExportExcel', data);
    }

    base.onSync = function (target) {
        costJsBase.Post({
            Url: '/HDKCB/OnSync',
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
    let c = new HDKCB();
    c.Setup();
    $('[data-toggle="tooltip"]').tooltip();
});