var CreateDeXuatKhoaMa = function () {
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
        $("#ddlDMCP").select2();
        //$("#lblCTV").select2();
        $(".lblCTV").select2();

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

        var _processId = $('#hddProcessId').val();
        var _idRole = $('#hddIDRole').val();
        var _lydo = $('#ddlLyDo').val();
        if (_idRole == 1 && _lydo == 1)
            _processId = 7;
        else if (_idRole == 9 && _lydo == 0)
            _processId = 8;
        else if (_idRole == 9 && _lydo == 1)
            _processId = 9;

        costJsBase.Post({
            Url: '/DeXuat/OnCreateDeXuat', Data: {
                DeXuatCode: $('#txtmadexuat').val(),
                DeXuatName: $('#txttendexuat').val(),
                MaBacsi: $('#lblCTV option:selected').val(),
                IDRole: $('#hddIDRole').val(),
                LyDoKhoa: $('#ddlLyDo').val(),
                LoaiDeXuatCode: $('#hddLoaiDeXuatCode').val(),
                ProcessId: _processId,
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
                    window.location.href = '/DeXuat/CreateDeXuatKhoaMa?DeXuat=' + $('#txtmadexuat').val();
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
                //GhiChuStep: $('#txtGhiChu').val(),
                GhiChuStep: "",
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
                    window.location.href = '/DeXuat/CreateDeXuatKhoaMa?DeXuat=' + $('#txtmadexuat').val();
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
                    window.location.href = '/DeXuat/CreateDeXuatKhoaMa?DeXuat=' + $('#txtmadexuat').val();
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

    base.onCreateDXKhoaMaCTV = function (_tr) {

        costJsBase.PromissePost({
            Url: '/DeXuat/OnCreateDeXuatKhoaMaCTV',
            Data: {
                DeXuatCode: $('#txtmadexuat').val(),
                MaCTV: _tr.find('.lblCTV').val(),
                ThoiGianKhoa: _tr.find('.txtTimeBlock').val(),
                Note: _tr.find('.txtNote').val(),
                LyDoKhoa: $('#ddlLyDo').val()
            },
        }).then(data => {

            if (data.code !== costJsBase.enums.successCode) {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);

            } else {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
                
            }
        }).fail(e => {
            //Swal.showValidationMessage('Lỗi hệ thống, vui lòng thử lại sau!');
            //deferred.resolve(false);
        });
    }

    base.onRemoveDXKhoaMaCTV = function (_tr) {
        costJsBase.PromissePost({
            Url: '/DeXuat/OnRemoveDeXuatKhoaMaCTV',
            Data: {
                DeXuatCode: $('#txtmadexuat').val(),
                MaCTV: _tr.find('.lblCTV').val(),
                ThoiGianKhoa: _tr.find('.ThoiGianKhoa').val(),
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
            //Swal.showValidationMessage('Lỗi hệ thống, vui lòng thử lại sau!');
            //deferred.resolve(false);
        });
    }
}

//define if page use modal

$(document).ready(function () {
    let c = new CreateDeXuatKhoaMa();
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

    $(document).on('change', '.lblCTV', function () {
        var option = $('option:selected', this).attr('TagName');
        var _row = $(this).closest(".row");
        _row.find('.txtTenBS').val(option);
    });

    $(document).on('click', '#btnNew', function () {

        if ($('#hddProcessStepId').val() != "0") {
            return;
        }

        if ($('#txtmadexuat').val() != "") {
            var _max = parseInt($("#maxRow").val());
            _max = 1 + _max;
            var _newid = "lblCTV" + _max;

            var dt = new Date();
            var _year = dt.getFullYear();
            var _month = dt.getMonth() + 1;
            if (_month < 10)
                _month = "0" + _month;
            var _date = dt.getDate();
            if (_date < 10)
                _date = "0" + _date;

            var time = _year + "-" + _month + "-" + _date;

            let _row = '';
            _row = _row + '<div class="row">';
            _row = _row + ' <div class="col-md-1">';
            _row = _row + '		<div class="form-group">';
            _row = _row + '			<a class="btnSaveNewRow" title="Lưu thông tin" href="javascript:void(0)"><i class="fa fa-save" style="font-size: 20px;padding-top:5px;"></i></a>';
            _row = _row + '			<a class="text-danger btnRemoveNew" title="Xóa thông tin" href="javascript:void(0)"><i class="fad fa-ban" style="font-size: 20px;padding-top:5px;"></i></a>';
            _row = _row + '		</div>';
            _row = _row + '	</div>';
            _row = _row + '	<div class="col-md-3">';
            _row = _row + '		<div class="form-group">';
            _row = _row + '			<div class="input-group">';
            _row = _row + '				<select id="' + _newid + '" style="width:100%" class="form-control form-control-sm select2-selection select2-hidden-accessible lblCTV">';

            $("#lblCTV").find('option').each(function () {
                var _optionText = $(this).val() + '-' + $(this).text();
                var _optionValue = $(this).val();
                var _optionTag = $(this).text();
                if (_optionValue != "-1")
                    _row = _row + '<option value="' + _optionValue + '" TagName="' + _optionTag + '">' + _optionText + '</option>';
                else
                    _row = _row + '                 <option value="-1" TagName="">Chọn CTV</option>';
            });

            _row = _row + '				</select >';
            _row = _row + '			</div>';
            _row = _row + '		</div>';
            _row = _row + '	</div>';
            _row = _row + '	<div class="col-md-3">';
            _row = _row + '		<div class="form-group">';
            _row = _row + '			<input value="" disabled class="form-control form-control-sm txtTenBS" />';
            _row = _row + '		</div>';
            _row = _row + '	</div>';
            _row = _row + '	<div class="col-md-3">';
            _row = _row + '		<div class="form-group">';
            _row = _row + '			<input type="date" class="form-control form-control-sm txtTimeBlock" value="' + time + '" placeholder="Thời gian khóa" />';
            _row = _row + '		</div>';
            _row = _row + '	</div>';
            _row = _row + '	<div class="col-md-2">';
            _row = _row + '		<div class="form-group">';
            _row = _row + '			<input value="" class="form-control form-control-sm txtNote" />';
            _row = _row + '		</div>';
            _row = _row + '	</div>';
            _row = _row + '</div>';
            $("#tblList").append(_row);

            $("#" + _newid).select2();

            $("#maxRow").val(_max);
        }
    });

    $(document).on('click', '.btnSaveNewRow', function () {
        if ($('#hddProcessStepId').val() != "0") {
            return;
        }

        let _tr = $(this).closest('.row');
        c.onCreateDXKhoaMaCTV(_tr);
    });

    $("#txtmadexuat").change(function () {
        $("#txtmadexuat").removeClass('itemerror');
    });
    $("#txttendexuat").change(function () {
        $("#txttendexuat").removeClass('itemerror');
    });

    if ($('#hddLyDoKhoa').val() != '') {
        $('#ddlLyDo').val($('#hddLyDoKhoa').val());
    }

    $(document).on('click', '.btnRemoveNew', function () {
        let _row = $(this).closest('.row');
        _row.remove();
    });

    $(document).on('click', '.btnRemove', function () {
        if ($('#hddProcessStepId').val() != "0") {
            return;
        }
        let _tr = $(this).closest('.row');
        c.onRemoveDXKhoaMaCTV(_tr);
    });
});