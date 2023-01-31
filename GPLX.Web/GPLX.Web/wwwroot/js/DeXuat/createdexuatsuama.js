var CreateDeXuatSuaMa = function () {
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
    base.$btnNewCTV = $('#btnNewCTV');

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
        $("#ddlDMCP").select2();
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

        base.$btnNewCTV.bind("click", function () {
            window.location.href = "/DMCTV/Create?From=DeXuat";
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
                    window.location.href = '/DeXuat/CreateDeXuatSuaMa?DeXuat=' + $('#txtmadexuat').val();
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
                DeXuatName: $('#txttendexuat').val(),
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
                    window.location.href = '/DeXuat/CreateDeXuatSuaMa?DeXuat=' + $('#txtmadexuat').val();
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
                    window.location.href = '/DeXuat/CreateDeXuatSuaMa?DeXuat=' + $('#txtmadexuat').val();
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

    base.onCreateDXCT = function (_tr) {

        costJsBase.PromissePost({
            Url: '/DeXuat/OnCreateDeXuatChiTiet',
            Data: {
                DeXuatCode: $('#txtmadexuat').val(),
                FieldName: _tr.find('.ddlField').val(),
                ValueOld: _tr.find('.txtoldValue').val(),
                ValueNew: _tr.find('.txtnewValue').val(),
                Note: _tr.find('.txtNote').val()
            },
        }).then(data => {

            if (data.code !== costJsBase.enums.successCode) {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);

            } else {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
            }
        }).fail(e => {
            Swal.showValidationMessage('Lỗi hệ thống, vui lòng thử lại sau!');
            //deferred.resolve(false);
        });
    }

    base.onRemoveDXCT = function (_tr) {

        costJsBase.PromissePost({
            Url: '/DeXuat/OnRemoveDeXuatChiTiet',
            Data: {
                DeXuatCode: $('#txtmadexuat').val(),
                FieldName: _tr.find('.ddlField').val(),
                ValueOld: _tr.find('.txtoldValue').val(),
                ValueNew: _tr.find('.txtnewValue').val(),
                Note: _tr.find('.txtNote').val()
            },
        }).then(data => {

            if (data.code !== costJsBase.enums.successCode) {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);

            } else {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
                _tr.remove();
            }
        }).fail(e => {
            Swal.showValidationMessage('Lỗi hệ thống, vui lòng thử lại sau!');
            //deferred.resolve(false);
        });
    }

}

//define if page use modal

$(document).ready(function () {
    let c = new CreateDeXuatSuaMa();
    c.Setup();
    $('[data-toggle="tooltip"]').tooltip();

    $("#txttenProfileCK").change(function () {
        $("#txttenProfileCK").removeClass('itemerror');
    });
    $("#txtNote").change(function () {
        $("#txtNote").removeClass('itemerror');
    });

    $("#txtmaProfileCK").change(function () {
        $("#txtmaProfileCK").removeClass('itemerror');
    });


    SelectChanged = function (dropdown) {

        var _myTag = dropdown.options[dropdown.selectedIndex].getAttribute('oldValue');
        var _cell = dropdown.closest('.col-md-3');
        _cell.nextElementSibling.firstElementChild.lastElementChild.value = _myTag;
    }

    $(document).on('click', '#btnNew', function () {
        if ($('#txtmadexuat').val() != "") {
            let _rowdefault = '<div class="row">' + $('#rowdefault').html() + '</div>';
            $("#tblList").append(_rowdefault);
        }
        //let _rowdefault = '';
        //_rowdefault = _rowdefault + '<div class="row">';
        //_rowdefault = _rowdefault + '    <div class="col-md-1">';
        //_rowdefault = _rowdefault + '        <div class="form-group">';
        //_rowdefault = _rowdefault + '            <button class="btn btn-sm btn-outline-primary btnRemove" ><i class="fad fa-plus-circle"></i>Bỏ</button>';
        //_rowdefault = _rowdefault + '            <button class="btn btn-sm btn-outline-primary btnSaveNew" ><i class="fad fa-plus-circle"></i>Lưu</button>';
        //_rowdefault = _rowdefault + '        </div>';
        //_rowdefault = _rowdefault + '    </div>';
        //_rowdefault = _rowdefault + '    <div class="col-md-3">';
        //_rowdefault = _rowdefault + '       <div class="form-group">';
        //_rowdefault = _rowdefault + '           <div class="input-group">';
        //_rowdefault = _rowdefault + '               <select class="form-control form-control-sm ddlField" onchange="SelectChanged(this)">';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">TenBS</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">NguoiDaiDien</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">NS</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">MaChucDanh</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">ChuyenKhoa</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">DC1</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">MaTinh</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">MaHuyen</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">DC2</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">MaTinh2</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">MaHuyen2</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">Mobi</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">Tel</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">CK</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">HTCKDoiTuong</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">CMND</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">NgayCapCMND</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">NoiCapCMND</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">Email</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">SoTK</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">Bank</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">TenChuTK</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">SoHD</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">MaDVCT</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">Fax</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">IsActive</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">LyDoIsActive</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">TraSau</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">MaDTCTV</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">UserWeb</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">PassWeb</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">ChiNhanh</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">GhiChu</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">MaSoThue</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">ChungChi_So</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">ChungChi_NgayCap</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">ChungChi_NoiCap</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">NgayKyHD</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">TenVietTat</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">TTDeXuat</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">GT</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">CKKH</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">KetThucHD</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">DaHoanThienHoSo</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">BH_Ma_Khoa</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">ChuKy</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">MaSap</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">SoPhuLuc</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">NgayKyPL</option>';
        //_rowdefault = _rowdefault + '                   <option value="0" oldValue="0">NgayKetThucPL</option>';
        //_rowdefault = _rowdefault + '               </select>';
        //_rowdefault = _rowdefault + '           </div>';
        //_rowdefault = _rowdefault + '       </div>';
        //_rowdefault = _rowdefault + '   </div>';
        //_rowdefault = _rowdefault + '   <div class="col-md-3">';
        //_rowdefault = _rowdefault + '       <div class="form-group">';
        //_rowdefault = _rowdefault + '           <input value="" disabled class="form-control form-control-sm txtoldValue" />';
        //_rowdefault = _rowdefault + '       </div>';
        //_rowdefault = _rowdefault + '   </div>';
        //_rowdefault = _rowdefault + '   <div class="col-md-3">';
        //_rowdefault = _rowdefault + '       <div class="form-group">';
        //_rowdefault = _rowdefault + '           <input id="txttenProfileCK" value="" class="form-control form-control-sm" />';
        //_rowdefault = _rowdefault + '       </div>';
        //_rowdefault = _rowdefault + '   </div>';
        //_rowdefault = _rowdefault + '   <div class="col-md-2">';
        //_rowdefault = _rowdefault + '       <div class="form-group">';
        //_rowdefault = _rowdefault + '           <input id="txttenProfileCK" value="" class="form-control form-control-sm" />';
        //_rowdefault = _rowdefault + '       </div>';
        //_rowdefault = _rowdefault + '   </div>';
        //_rowdefault = _rowdefault + '</div>';

        
    });
    $(document).on('click', '.btnRemove', function () {
        let _row = $(this).closest('.row');
        c.onRemoveDXCT(_tr);
    });

    $(document).on('click', '.btnSaveNewRow', function () {
        let _tr = $(this).closest('.row');
        c.onCreateDXCT(_tr);
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
    $("#txtThoiGianKhoa").change(function () {
        $("#txtThoiGianKhoa").removeClass('itemerror');
    });
});