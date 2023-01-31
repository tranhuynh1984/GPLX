var CreateDeXuatMoMaCTV = function () {
    let base = this;
    let $dropStats = $('#requestSearchStats');

    base.$exportExcel = $('#btnExportExcel');
    base.$ProfileMa = $('#txtmaProfileCK');
    base.$ProfileTen = $('#txttenProfileCK');
    base.$onLoad = true;
    base.$popOverlay = $('#modal-overlay');
    base.$ddlStatus = $('#ddlStatus option:selected');
    base.$ddlDMChuyenKhoa = $('#ddlDMChuyenKhoa option:selected');
    //base.$btnNew = $('#btnNew');
    base.$btnBack = $('#btnBack');
    base.$btnSaveDX = $('#btnSaveDX');
    base.$btnPush = $('#btnPush');
    base.$btnReject = $('#btnReject');
    base.$btnSearch = $('#btnSearch');

    base.recordSelector = null;

    base.recordSelectorIndex = -1;

    base.pageLanguage = languages.vi.ProfileCK;

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
        $("#lblCTV").select2();
        base.bindActions();

        base.$exportExcel.bind("click", function () {
            base.exportExcel();
        })
    }

    base.searchForm = function (data) {
        if (typeof (data) !== 'undefined') {
            data.ProfileCKMa = $('#txtmaProfileCK').val();
            data.Keywords = $('#txtSearch').val();
            base.setParamsAfterSearch(data);
        }
    }
   
    base.exportExcel = function () {
        let data = {};
        base.searchForm(data);
        costJsBase.ExportExcel('/ProfileCK/ExportExcel', data);
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
        $.fn.eEnterActions({
            selector: base.$requestSearchKeywords, action: function (elm) {
                //base.searching(true);
            }
        });


        base.$btnBack.bind("click", function () {
            window.location.href = '/DeXuat/List';
        });

        base.$btnSaveDX.bind("click", function () {
            base.onCreateDX();
        });

        base.$btnPush.bind("click", function () {
            base.pushDX();
        });

        base.$btnReject.bind("click", function () {
            base.rejectDX();
        });
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

    base.onCreateDX = function (target) {
        costJsBase.Post({
            Url: '/DeXuat/OnCreateDeXuat', Data: {
                DeXuatCode: $('#txtmadexuat').val(),
                DeXuatName: $('#txttendexuat').val(),
                MaBacsi: $('#lblCTV option:selected').val(),
                TenBacsi: $('#txttenCTV').val(),
                IDRole: $('#hddIDRole').val(),
                ThoiGianKhoa: $('#txtThoiGianKhoa').val(),
                LoaiDeXuatCode: $('#hddLoaiDeXuatCode').val(),
                ProcessId: $('#hddProcessId').val(),
                Note: $('#txtGhiChu').val(),
                Record: $('#___record').val()
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
                //base.searching();
                setTimeout(function () {
                    base.$popOverlay.modal('hide');
                    window.location.href = '/DeXuat/CreateDeXuatMoMaCTV?DeXuat=' + $('#txtmadexuat').val();
                }, 1500);
                
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

    base.pushDX = function (target) {
        costJsBase.Post({
            Url: '/DeXuat/PushDeXuat', Data: {
                DeXuatCode: $('#txtmadexuat').val(),
                IDRole: $('#hddIDRole').val(),
                GhiChuStep: "",
                //GhiChuStep: $('#txtGhiChu').val(),
                LoaiDeXuatCode: $('#hddLoaiDeXuatCode').val()
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
            } else {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
                //base.searching();
                setTimeout(function () {
                    base.$popOverlay.modal('hide');
                    window.location.href = '/DeXuat/CreateDeXuatMoMaCTV?DeXuat=' + $('#txtmadexuat').val();
                }, 1500);
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

    base.rejectDX = function (target) {
        costJsBase.Post({
            Url: '/DeXuat/RejectDeXuat', Data: {
                DeXuatCode: $('#txtmadexuat').val(),
                IDRole: $('#hddIDRole').val(),
                GhiChuStep: $('#txtGhiChu').val()
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
                //base.searching();
                setTimeout(function () {
                    base.$popOverlay.modal('hide');
                    window.location.href = '/DeXuat/CreateDeXuatMoMaCTV?DeXuat=' + $('#txtmadexuat').val();
                }, 1500);
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
    let c = new CreateDeXuatMoMaCTV();
    c.Setup();
    $('[data-toggle="tooltip"]').tooltip();

    $("#txttenProfileCK").change(function () {
        $("#txttenProfileCK").removeClass('itemerror');
    });
    $("#txtThoiGianKhoa").change(function () {
        $("#txtThoiGianKhoa").removeClass('itemerror');
    });
    $("#txtNote").change(function () {
        $("#txtNote").removeClass('itemerror');
    });

    $("#txtmaProfileCK").change(function () {
        $("#txtmaProfileCK").removeClass('itemerror');
    });

    $("#lblCTV").change(function () {
        var option = $('option:selected', this).attr('TagName');
        $('#txttenCTV').val(option);
        $('#lblCTV').next().find('.select2-selection--single').removeClass('itemerror');
    });

    $("#txtmadexuat").change(function () {
        $("#txtmadexuat").removeClass('itemerror');
    });
    $("#txttendexuat").change(function () {
        $("#txttendexuat").removeClass('itemerror');
    });
});