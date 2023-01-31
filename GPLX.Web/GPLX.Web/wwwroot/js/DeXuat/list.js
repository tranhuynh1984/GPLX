var DeXuat = function () {
    let base = this;
    let $tableSelector = $('#tblList');
    let $dropStats = $('#requestSearchStats');

    base.$tableDataTable = null;
    base.$searchButton = $('#requestSearchButton');
    base.$btnSync = $('#btnSync');
    base.$exportExcel = $('#btnExportExcel');
    //base.$exportPdf = $('#btnExportPdf');
    base.$txtDeXuatCode = $('#txtDeXuatCode');
    base.$txtDeXuatName = $('#txtDeXuatName');
    base.$onLoad = true;
    base.$popOverlay = $('#modal-overlay');
    base.$ddlStatus = $('#ddlStatus option:selected');
    base.$btnCreate = $('#btnCreate');

    base.recordSelector = null;

    base.recordSelectorIndex = -1;

    base.pageLanguage = languages.vi.LoaiDeXuat;

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
        $("#ddlLoaiDeXuat").select2();
        $("#ddlUser").select2();

        base.bindActions();
        base.setupDataTable();

        base.$exportExcel.bind("click", function () {
            base.exportExcel();
        })

        //base.$exportPdf.bind("click", function () {
        //    base.exportPdf();
        //})


        base.$btnSync.bind('click',
            function () {
                base.onSync(this);
            });
    }

    base.searchForm = function (data) {
        if (typeof (data) !== 'undefined') {
            data.DeXuatCode = base.$txtDeXuatCode.val();
            data.DeXuatName = base.$txtDeXuatName.val();
            data.Status = $('#ddlStatus option:selected').val();
            data.ND = $('#txtND').val();
            data.NS = $('#txtNS').val();
            data.LoaiDeXuatCode = $('#ddlLoaiDeXuat option:selected').val();
            data.NguoiTao = $('#ddlUser option:selected').val();

            base.setParamsAfterSearch(data);

            $('#txtDeXuatCode').val($.trim(data.DeXuatCode));
            $('#txtDeXuatName').val($.trim(data.DeXuatName));
        }
    }

    base.setupDataTable = function () {

        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            ajax: {
                url: "/DeXuat/Search",
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
                        return row.deXuatCode;
                    },
                    width: "5%"
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return row.deXuatName;
                    },
                    width: "10%"
                },
                {
                    idx: 3,
                    render: function (data, type, row, meta) {
                        return row.tenDeXuatCode;
                    },
                    width: "10%"
                },
                {
                    idx: 4,
                    render: function (data, type, row, meta) {
                        return '<a class="text-danger exportpdf" href="/DeXuat/ExportPdf?dexuat=' + row.deXuatCode + '" target="_blank"><i class="fas fa-file-pdf"></i></a>';
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 5,
                    render: function (data, type, row, meta) {
                        //return '<a style="width:200px;background-color:red" class="View" href="/Step/List?Dexuat=' + row.deXuatCode + '" >Xem</a> ';

                        if (row.isDone == -1)
                            return '<a href="/Step/List?Dexuat=' + row.deXuatCode + '"  class="link-danger">Tiến Trình</a>';
                        else if (row.isDone == 1)
                            return '<a href="/Step/List?Dexuat=' + row.deXuatCode + '"  class="link-success">Tiến Trình</a>';
                        else if (row.trangThai == "Chờ duyệt")
                            return '<a href="/Step/List?Dexuat=' + row.deXuatCode + '"  class="link-warning">Tiến Trình</a>';
                        else if (row.trangThai == "Quá hạn")
                            return '<a href="/Step/List?Dexuat=' + row.deXuatCode + '"  class="link-secondary">Tiến Trình</a>';
                        else
                            return '<a href="/Step/List?Dexuat=' + row.deXuatCode + '"  class="link-primary">Tiến Trình</a>';
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 6,
                    render: function (data, type, row, meta) {
                        return row.trangThai;
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 7,
                    render: function (data, type, row, meta) {
                        return row.createby;
                    },
                    width: "5%",
                    data: 'order'
                },
                {
                    idx: 8,
                    render: function (data, type, row, meta) {
                        return moment(row.createdate).format('HH:mm DD/MM/yyyy')
                    },
                    width: "5%",
                    data: 'order'
                },
                {
                    idx: 9,
                    render: function (data, type, row, meta) {
                        return row.updateby;
                    },
                    width: "5%",
                    data: 'order'
                },
                {
                    idx: 10,
                    render: function (data, type, row, meta) {
                        return row.updatedate == null ? "" : moment(row.updatedate).format('HH:mm DD/MM/yyyy');
                    },
                    width: "5%",
                    data: 'order'
                },
                {
                    idx: 11,
                    render: function (data, type, row, meta) {
                        let s = '';
                        s = '<a class="mr-2" href="javascript:void(0)" title="Thông tin đề xuất" prop-type="elems.table.edit" prop-type-act="pop" ><i class="fad fa-pencil med-theme-primary-button"></i></a>';
                        if (row.isDone == 0 && ((row.processStepId == row.idRole) || (1>= row.idRole && row.processStepId == 0)))
                            s += '<a class="fs-17 text-info mr-2" prop-type="elems.table.info" prop-type-act="push" title="Đẩy đề xuất" href="javascript:void(0)"><i class="fas fa-recycle"></i></a>';
                        if (row.isDone == 0 && row.processStepId == 0)
                            s += '<a class="text-danger mr-2" prop-type="elems.table.delete" prop-type-act="confirm" title="Xóa đề xuất" href="javascript:void(0)"><i class="fad fa-ban"></i></a>';
                        if (row.isDone == 0 && (row.processStepId == row.idRole &&  row.processStepId > 0))
                            s += '<a class="text-danger mr-2" prop-type="elems.table.reject" prop-type-act="reject" title="Từ chối đề xuất" href="javascript:void(0)"><i class="fad fa-ban"></i></a>';
                        return s;
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 12,
                    render: function (data, type, row, meta) {

                        let s = '';
                        if (row.isDone != 1)
                            s += '<a class="fs-17 text-info mr-2" prop-type="elems.table.done" prop-type-act="done" title="Hoàn thiện" href="javascript:void(0)"><i class="fas fa-recycle"></i></a>';
                        else
                            s += '<a class="mr-2" href="javascript:void(0)" title="Hoàn thiện" prop-type="elems.table.done" prop-type-act="done" ><i class="fad fa-angle-down med-theme-primary-button"></i></a>';
                        
                        return s;
                    },
                    width: "5%",
                    class: "text-center"
                },
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
        costJsBase.ExportExcel('/DeXuat/ExportExcel', data);
    }

    //base.exportPdf = function () {

    //    var _madexuat = $(this).offset('tr').find('td')
    //    costJsBase.ExportPdf('/DeXuat/ExportPdf');
    //}

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
            if ($('#txtND').val() == '') {
                costJsBase.EventNotify('error', 'Bạn hãy chọn thời gian từ!');
                $('#txtND').addClass("itemerror");
            }   
            if ($('#txtNS').val() == '') {
                costJsBase.EventNotify('error', 'Bạn hãy chọn thời gian đến!');
                $('#txtNS').addClass("itemerror");
            }   
            if ($('#txtND').val() > $('#txtNS').val()) {
                costJsBase.EventNotify('error', 'Thời gian từ lớn hơn thời gian đến!');
                $('#txtND').addClass("itemerror");
            }   

            base.searching(true);
        });

        base.$btnCreate.bind("click",
            function () {
                var _loaidexuat = $('#ddlLoaiDeXuat').val();
                if (_loaidexuat == "DeXuatKhoaMa")
                    window.location.href = '/DeXuat/CreateDeXuatKhoaMa';
                else if (_loaidexuat == "DeXuatLuanChuyenMa")
                    window.location.href = '/DeXuat/CreateDeXuatLuanChuyenMa';
                else if (_loaidexuat == "DeXuatMoMa")
                    window.location.href = '/DeXuat/CreateDeXuatMoMaCTV';
                else if (_loaidexuat == "DeXuatTaoMa")
                    window.location.href = '/DeXuat/CreateDeXuatCTV?Type=1';
                else if (_loaidexuat == "DeXuatSuaMa")
                    window.location.href = '/DeXuat/CreateDeXuatCTV';
                else
                    costJsBase.EventNotify('error', 'Bạn hãy chọn loại đề xuất!');
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
                        case "edit":

                            if (record.loaiDeXuatCode == "DeXuatSuaMa")
                                window.location.href = '/DeXuat/CreateDeXuatCTV?DeXuat=' + record.deXuatCode;
                            else if (record.loaiDeXuatCode == "DeXuatMoMa")
                                window.location.href = '/DeXuat/CreateDeXuatMoMaCTV?DeXuat=' + record.deXuatCode;
                            else
                                window.location.href = '/DeXuat/Create' + record.loaiDeXuatCode + '?DeXuat=' + record.deXuatCode;
                            
                        default:
                            break;
                    }
                    break;
                case "push":
                    base.pushDX(record.deXuatCode, record.idRole, "Tôi đồng ý");
                    break;
                case "reject":
                    Swal.fire({
                        title: 'Bạn có chắc chắn muốn từ chối đề xuất này?' + '</br> Bản ghi xóa: ' + record.deXuatCode,
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
                            base.rejectDX(record.deXuatCode, record.idRole, "Tôi từ chối");
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
                case "confirm":
                    Swal.fire({
                        title: 'Bạn có chắc chắn muốn xóa đề xuất này?' + '</br> Bản ghi xóa: ' + record.deXuatCode,
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
                                Url: '/DeXuat/OnRemove',
                                Data: {
                                    deXuatCode: record.deXuatCode
                                }
                            }).then(data => {
                                if (data.code !== costJsBase.enums.successCode) {
                                    costJsBase.EventNotify('error', data.message);
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

    base.pushDX = function (deXuatCode, iDRole, ghiChu ) {
        costJsBase.Post({
            Url: '/DeXuat/PushDeXuat', Data: {
                DeXuatCode: deXuatCode,
                IDRole: iDRole,
                GhiChuStep: ghiChu
            }
        }, function (data) {
            if (data.code !== costJsBase.enums.successCode) {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
                
            } else {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
                base.searching();
            }
        }, function () {
            
            costJsBase.EventNotify('error', 'Lỗi hệ thống, vui lòng thử lại sau!')
        })
    }

    base.rejectDX = function (deXuatCode, iDRole, ghiChu) {
        costJsBase.Post({
            Url: '/DeXuat/RejectDeXuat', Data: {
                DeXuatCode: deXuatCode,
                IDRole: iDRole,
                GhiChuStep: ghiChu
            }
        }, function (data) {
            if (data.code !== costJsBase.enums.successCode) {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);

            } else {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
                base.searching();
            }
        }, function () {

            costJsBase.EventNotify('error', 'Lỗi hệ thống, vui lòng thử lại sau!')
        })
    }

    base.onCreate = function (target) {
        costJsBase.Post({
            Url: '/LoaiDeXuat/OnCreate', Data: {
                Record: $('#___record').val(),
                IsActive: $('#ddlStatusloaidexuat option:selected').val(),
                LoaiDeXuatCode: $('#txtmaloaidexuat').val(),
                LoaiDeXuatName: $('#txttenloaidexuat').val(),
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
    let c = new DeXuat();
    c.Setup();
    $('[data-toggle="tooltip"]').tooltip();

    let _height = $('#FilterSearch').height();
    $('#FilterStatic').height(_height);

    $("#txtND").change(function () {
        $("#txtND").removeClass('itemerror');
    });
    $("#txtNS").change(function () {
        $("#txtNS").removeClass('itemerror');
    });

    //$(document).on('click', '.exportpdf', function () {
    //    c.exportPdf();
    //});
});