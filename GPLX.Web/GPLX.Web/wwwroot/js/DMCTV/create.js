// Company: MedcomJsc
// User: HoanNS
// Date: 21-04-2022
// Function: Danh mục cộng tác viên

var DMCTVCreate = function () {
    let base = this;
    let $tableSelector = $('#tblList');
    let $dropStats = $('#requestSearchStats');

    base.$tableDataTable = null;
    base.$searchButton = $('#requestSearchButton');
    base.$exportExcel = $('#btnExportExcel');
    base.$btnCreate = $('#btnCreate');
    base.$btnClose = $('#btnClose');

    base.$txtMaBS = $('#txtMaBS');
    base.$txtTenBS = $('#txtTenBS');
    base.$ddlStatus = $('#ddlStatus option:selected');

    base.$onLoad = true;
    base.$popOverlay = $('#modal-overlay');

    base.recordSelector = null;

    base.recordSelectorIndex = -1;

    base.pageLanguage = languages.vi.DMCTV;

    base.settingPage = {
        pop: {
            url: '',
            title: base.pageLanguage.list.popup.title,
            baseTitle: base.pageLanguage.list.popup.baseTitle,
            viewLabel: base.pageLanguage.list.popup.viewLabel,
            historyLabel: base.pageLanguage.list.popup.historyLabel,
            createLabel: base.pageLanguage.list.popup.createLabel,
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

        //base.$btnSync.bind('click',
        //    function () {
        //        base.onSync(this);
        //    });
    }

    base.searchForm = function (data) {
        if (typeof (data) !== 'undefined') {
            data.MaBS = base.$txtMaBS.val();
            data.Keywords = base.$txtTenBS.val();
            data.Status = $('#ddlStatus option:selected').val();
            base.setParamsAfterSearch(data);
        }
    }

    base.exportExcel = function () {
        let data = {};
        base.searchForm(data);
        costJsBase.ExportExcel('/DMCTV/ExportExcel', data);
    }

    base.setupDataTable = function () {
        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            ajax: {
                url: "/DMCTV/Search",
                data: function (d) {
                    console.log(d);
                    base.searchForm(d);
                }
            },
            columns: [
                {
                    idx: 0,
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return row.tenCN;
                    },
                    width: "5%"
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return row.maBS;
                    },
                    width: "5%"
                }
                ,
                {
                    idx: 3,
                    render: function (data, type, row, meta) {
                        return row.tenBS;
                    },
                    width: "10%"
                },
                {
                    idx: 4,
                    render: function (data, type, row, meta) {
                        return row.userWeb;
                    },
                    width: "5%",
                    data: 'order',
                    class: "text-right"
                },
                {
                    idx: 5,
                    render: function (data, type, row, meta) {
                        return row.passWeb;
                    },
                    width: "5%",
                    data: 'order',
                    class: "text-center"
                },
                {
                    idx: 6,
                    render: function (data, type, row, meta) {
                        return row.tenDTCTV;
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 7,
                    render: function (data, type, row, meta) {
                        return row.tenDVCT;
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 8,
                    render: function (data, type, row, meta) {
                        return row.mobi;
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 9,
                    render: function (data, type, row, meta) {
                        return row.hTCKDoiTuong;
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 10,
                    render: function (data, type, row, meta) {
                        return row.traSau;
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 11,
                    render: function (data, type, row, meta) {
                        return row.cKKH;
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 12,
                    render: function (data, type, row, meta) {
                        return row.dC1;
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

        $.fn.eEnterActions({
            selector: base.$requestSearchKeywords, action: function (elm) {
                base.searching(true);
            }
        });

        base.$btnCreate.bind("click",
            function () {
                base.onCreateDMCTV();
            }
        );

        base.$btnClose.bind("click",
            function () {
                if ($('#hddFrom').val() != '')
                    window.location.href = '/DeXuat/CreateDeXuatCTV?Type=1&MaBS=' + $('#txtMaBS').val();
                else
                    window.location.href = '/DMCTV/List';
            }
        );
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
            Url: '/DMCTV/OnSync',
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

    base.onCreateDMCTV = function (target) {
        costJsBase.Post({
            Url: '/DMCTV/OnCreateDMCTV', Data: {
                MaBS: $('#txtMaBS').val(),
                TenBS: $('#txtTenBS').val(),
                ChiNhanh: $('#ddlUnit').val(),
                NguoiDaiDien: $('#txtNguoiDaiDien').val(),
                MaSap: $('#txtMaSAP').val(),
                Mobi: $('#txtMobi').val(),
                Tel: $('#txtPhoneCoDinh').val(),
                Email: $('#txtEmail').val(),
                MaSoThue: $('#txtTax').val(),
                DC1: $('#txtDC1').val(),
                MaTinh: $('#ddlProvince').val(),
                MaHuyen: $('#ddlDistrict').val(),
                GhiChu: $('#txtGhiChu').val(),
                CMND: $('#txtSoCMND').val(),
                NgayCapCMND: $('#txtNgayCapCMND').val(),
                NoiCapCMND: $('#txtNoiCapCMND').val(),
                NS: $('#txtNgaySinh').val(),
                MaChucDanh: $('#ddlJobTitle').val(),
                ChuyenKhoa: $('#ddlSpecialist').val(),
                DC2: $('#txtDCCT').val(),
                MaTinh2: $('#ddlProvinceCompany').val(),
                MaHuyen2: $('#ddlDistrictCompany').val(),
                CK: $('#txtmadexuat').val(),
                HTCKDoiTuong: $('#txtmadexuat').val(),
                SoTK: $('#txtSoTKNH').val(),
                Bank: $('#txtChiNhanhTKNH').val(),
                TenChuTK: $('#txtChuTKNH').val(),
                SoHD: $('#txtSoHD').val(),
                UserWeb: $('#txtUserWeb').val(),
                PassWeb: $('#txtPassWeb').val(),
                TTDeXuat: $('#txtThongTinDeXuat').val(),
                TenVietTat: $('#txtTenVietTat').val(),
                NgayKyHD: $('#txtNgayKyHD').val(),
                KetThucHD: $('#txtNgayKetThucHD').val(),
                SoPhuLuc: $('#txtSoPL').val(),
                NgayKyPL: $('#txtNgayKyPL').val(),
                NgayKetThucPL: $('#txtNgayKetThucPL').val()
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

                console.log(data.listError);
                costJsBase.ShowValidation(data.listError);
            } else {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
                //base.searching();
                setTimeout(function () {
                    base.$popOverlay.modal('hide');
                }, 1000);

                if ($('#hddFrom').val() != '')
                    window.location.href = '/DeXuat/CreateDeXuatCTV?Type=1&MaBS=' + $('#txtMaBS').val();
                else
                    window.location.href = 'DMCTV/Create?MaBS=' + $('#txtMaBS').val();
                
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
    let c = new DMCTVCreate();
    c.Setup();
    $('[data-toggle="tooltip"]').tooltip();

    $("#txtMaBS").change(function () {
        $("#txtMaBS").removeClass('itemerror');
    });
    $("#txtTenBS").change(function () {
        $("#txtTenBS").removeClass('itemerror');
    });
    $("#ddlUnit").select2();
    $("#ddlPartnerObject").select2();
    $("#ddlProvince").select2();
    $("#ddlDistrict").select2();
    $("#ddlProvinceCompany").select2();
    $("#ddlDistrictCompany").select2();
    $("#ddlJobTitle").select2();
    $("#ddlSpecialist").select2();
    $("#ddlDiscount").select2();
    $("#ddlHinhThucCK").select2();
    $("#ddlHinhThucTT").select2();
    $("#ddlGioiTinh").select2();

    //function ddlHinhThucCK_Change() {
    //    let x = document.getElementById("ddlHinhThucCK").value;
    //    switch (x) {
    //        case "TT":
    //            $("#txtCK").attr("disabled", "disabled");
    //            break;
    //        case "CKS":
    //            // code block
    //            break;
    //        case "TS":

    //            break;
    //        default:
    //        // code block
    //    }
    //};

    $(document).on('change', '#ddlHinhThucCK', function () {
        var _value = $('option:selected', this).val();

        if (_value == "TS") {
            $("#cardThuSau").css('display','block');
            $("#cardChietKhauSau").css('display','none');
            $("#cardKhac").css('display','none');
            $("#cardTrucTiep").css('display','none');
        }
        else if (_value == "CKS") {
            $("#cardThuSau").css('display','none');
            $("#cardChietKhauSau").css('display','block');
            $("#cardKhac").css('display','none');
            $("#cardTrucTiep").css('display','none');
        }
        else if (_value == "KHA") {
            $("#cardThuSau").css('display','none');
            $("#cardChietKhauSau").css('display','none');
            $("#cardKhac").css('display','block');
            $("#cardTrucTiep").css('display','none');
        }
        else if (_value == "TT") {
            $("#cardThuSau").css('display','none');
            $("#cardChietKhauSau").css('display','none');
            $("#cardKhac").css('display','none');
            $("#cardTrucTiep").css('display','block');
        }
        else {
            $("#cardThuSau").css('display','none');
            $("#cardChietKhauSau").css('display','none');
            $("#cardKhac").css('display','none');
            $("#cardTrucTiep").css('display','none');
        }
    });
});