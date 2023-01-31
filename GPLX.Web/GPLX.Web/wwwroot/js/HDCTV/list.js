var HDCTV = function () {
    let base = this;
    let $tableSelector = $('#tblList');
    let $dropStats = $('#requestSearchStats');

    base.$tableDataTable = null;
    base.$searchButton = $('#requestSearchButton');
    base.$btnSync = $('#btnSync');
    base.$exportExcel = $('#btnExportExcel');
    base.$txtMaCTV = $('#txtMaCTV');
    base.$txtTenCTV = $('#txtTenCTV');
    base.$onLoad = true;
    base.$popOverlay = $('#modal-overlay');
    base.$ddlStatus = $('#ddlStatus option:selected');
    base.$ddlGroup = $('#ddlGroup option:selected');
    base.$btnCreate = $('#btnCreate');

    base.recordSelector = null;
    base.recordSelectorIndex = -1;
    base.pageLanguage = languages.vi.HDCTV;

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
            data.SubId = base.$txtMaCTV.val();
            data.SubName = base.$txtTenCTV.val();
            data.IsUse = $('#ddlStatus option:selected').val();
            data.CTVGroupID = $('#ddlGroup option:selected').val();
            base.setParamsAfterSearch(data);
        }
    }

    base.setupDataTable = function () {

        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            ajax: {
                url: "/HDCTV/Search",
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
                        return row.subId;
                    },
                    width: "5%"
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return row.subName;
                    },
                    width: "10%"
                },
                {
                    idx: 3,
                    render: function (data, type, row, meta) {
                        return moment(row.fromDate).format('HH:mm DD/MM/yyyy');
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 4,
                    render: function (data, type, row, meta) {
                        return moment(row.toDate).format('HH:mm DD/MM/yyyy');
                    },
                    width: "5%"
                },
                {
                    idx: 5,
                    render: function (data, type, row, meta) {
                        return row.isUseName;
                    },
                    width: "10%"
                },
                {
                    idx: 6,
                    render: function (data, type, row, meta) {
                        return row.createby;
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 7,
                    render: function (data, type, row, meta) {
                        return moment(row.createdate).format('HH:mm DD/MM/yyyy');
                    },
                    width: "5%"
                },
                {
                    idx: 8,
                    render: function (data, type, row, meta) {
                        return row.updateby;
                    },
                    width: "10%"
                },
                {
                    idx: 9,
                    render: function (data, type, row, meta) {
                        return row.updatedate == null ? "" : moment(row.updatedate).format('HH:mm DD/MM/yyyy')
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 10,
                    render: function (data, type, row, meta) {
                        return row.ctvGroupID;
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 11,
                    render: function (data, type, row, meta) {
                        return row.ctvGroupName;
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 12,
                    render: function (data, type, row, meta) {
                        let s = '';
                        s = '<a class="mr-2" href="javascript:void(0)" title="Chỉnh sửa hợp đồng" prop-type="elems.table.edit" prop-type-act="pop" ><i class="fad fa-pencil med-theme-primary-button"></i></a>'
                        s += '<a class="text-danger mr-2" prop-type="elems.table.delete" prop-type-act="confirm" title="Xóa hợp đồng" href="javascript:void(0)"><i class="fad fa-ban"></i></a>'
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
        costJsBase.ExportExcel('/HDCTV/ExportExcel', data);
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
                if ($('#ddlGroup option:selected').val() != -1) {
                    window.location.href = '/HDCTV/Create?GroupId=' + $("#ddlGroup").val();
                }
                else {
                    costJsBase.EventNotify('error', 'Bạn hãy chọn nhóm hợp đồng cộng tác viên!');
                }
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
                            window.location.href = '/HDCTV/Create?GroupId=' + record.ctvGroupID + '&SubId=' + record.subId;
                            break;
                        default:
                            break;
                    }
                    break;
                case "confirm":
                    Swal.fire({
                        title: 'Bạn có chắc chắn muốn xóa hợp đồng cộng tác viên này?' + '</br> Bản ghi xóa: ' + record.subId + ' - ' + record.subName,
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
                                Url: '/HDCTV/OnRemoveSub',
                                Data: {
                                    SubId: record.subId
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
 
}

//define if page use modal

$(document).ready(function () {
    let c = new HDCTV();
    c.Setup();
    $('[data-toggle="tooltip"]').tooltip();

});