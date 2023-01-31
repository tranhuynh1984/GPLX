var ProfitPlanList = function () {
    let base = this;
    let $tableSelector = $('#manageRequestingCostElement');
    let $dropStats = $('#requestSearchStats');

    base.$tableDataTable = null;
    base.$labelStats = $('#lableStats');
    base.$searchButton = $('#requestSearchButton');
    base.$onLoad = true;
    base.$popOverlay = $('#modal-extra-large');
    //loại : năm - tuần
    base.$selectUnits = $('#selectUnits');
    base.$selectYear = $('#__searchYearRequest');
    base.$btnSearch = $('#__btnSearch');


    base.recordSelector = null;
    base.recordSelectorIndex = -1;

    base.pageLanguage = languages.vi.profit;

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
                accept: { visible: false },
                decline: { visible: false },
                close: { visible: false }
            },
            dropContent: true
        },
        redirect: {

        },
        fnUPop: function (act) {
            this.pop.title = this.pop.baseTitle.replace(this.pop.replator, act);
        },
        fnURI: function (uri) {
            if (typeof (uri) == 'string') {
                this.pop.url = uri;
            }
        },
        fnButtons: function (btns) {
            this.pop.buttons = btns;
        }
    };

    base.Setup = function () {

        // promise -> readURI -> setup handler
        base.readURI().then(function () {
            base.bindActions();
            base.setupDataTable();
            base.$btnSearch.click(base.searching);
        });
    }

    base.searchForm = function (data) {
        if (typeof (data) !== 'undefined') {
            data.Status = $dropStats.find('a[selected]').attr('prop-stats');
            data.Year = base.$selectYear.val();

            let onQuery = costJsBase.ValueFromUrl('stats');
            if (onQuery === '-8888')
                data.Status = -8888;

            data.UserUnit = typeof base.$selectUnits != 'undefined' ? base.$selectUnits.val() : -100;
            base.setParamsAfterSearch(data);
        }
    }

    base.setupDataTable = function () {
        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            ajax: {
                url: "/ProfitPlan/Search",
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
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return row.year;
                    },
                    class: "text-center",
                    width: "8%"
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return row.unitName;
                    }
                },
                {
                    idx: 3,
                    render: function (data, type, row, meta) {
                        return row.creatorName;
                    },
                    width: "14%",
                    class: "text-center"
                },
                {
                    idx: 4,
                    render: function (data, type, row, meta) {
                        return moment(row.createdDate).format('DD/MM/YYYY HH:mm');
                    },
                    class: "text-center",
                    width: "12%"
                },
                {
                    idx: 5,
                    render: function (data, type, row, meta) {
                        return costJsBase.FormatMoney(row.totalRevenue, '-');
                    },
                    width: "10%",
                    class: "text-right"
                }, {
                    idx: 6,
                    render: function (data, type, row, meta) {
                        return costJsBase.FormatMoney(row.totalExpense, '-');
                    },
                    width: "8%",
                    class: "text-right"
                }, {
                    idx: 7,
                    render: function (data, type, row, meta) {
                        return costJsBase.FormatMoney(row.totalProfitTax, '-');
                    },
                    width: "8%",
                    class: "text-right"
                },
                // link PDF
                {
                    idx: 8,
                    render: function (data, type, row, meta) {
                        return '<a class="text-danger" href="' + row.pathPdf + '" target="_blank"><i class="fas fa-file-pdf"></i></a>';
                    },
                    class: "text-center",
                    width: "5%"
                },
                {
                    idx: 9,
                    render: function (data, type, row, meta) {
                        return row.statusName;
                    },
                    class: "text-center",
                    width: "8%"
                },
                {
                    idx: 10,
                    render: function (data, type, row, meta) {
                        let s = '';
                        if (row.approvalable)
                            s +=
                                '<a class="fs-17 mr-2 text-info" href="javascript:void(0)" title="Phê duyệt kế hoạch doanh thu khách hàng" prop-type="elems.table.approve" prop-type-act="confirm" >' +
                                '<i class="fad fa-check"></i></a>';
                        if (row.declineable)
                            s +=
                                '<a class="fs-17 mr-2 text-danger" href="javascript:void(0)" title="Từ chối kế hoạch doanh thu khách hàng" prop-type="elems.table.decline" prop-type-act="confirm">' +
                                '<i class="far fa-ban"></i></a>';
                        if (row.deleteable)
                            s +=
                                '<a class="fs-17 mr-2 text-danger" href="javascript:void(0)" title="Xóa kế hoạch doanh thu khách hàng" prop-type="elems.table.delete" prop-type-act="confirm">' +
                                '<i class="far fa-times"></i></a>';
                        if (row.viewable) {
                            s +=
                                '<a class="fs-17 text-info" prop-type="elems.table.history" prop-type-act="pop" title="Xem lịch sử" href="javascript:void(0)"> <i class="fal fa-clock"></i></a>';
                        }
                        return s;
                    },
                    class: "text-center",
                    width: "12%"
                }
            ],
            drawCallback: function () {
                base.drawInit();
            },
            scrollX: true,
            //issue: on change resolution --> header not match size with tbody
            responsive: false
        });
    }

    base.statsChange = function (val) {
        var deferred = $.Deferred();
        $dropStats.find('a').removeAttr('selected');

        if (typeof (val) === 'string') {
            let truth = $dropStats.find('a[prop-stats="' + val + '"]');
            if (truth.length) {
                base.$labelStats.text(truth.text());
                truth.attr("selected", true);
            }
        } else if (typeof (val) !== 'undefined') {
            base.$labelStats.text($(val).text());
            $(val).attr("selected", true);
        }

        deferred.resolve();
        return deferred.promise();
    }

    base.bindActions = function () {
        $dropStats.find('a').bind("click", function (e) {
            base.statsChange(this).then(base.searching);
        });

        base.$searchButton.bind("click",
            function () {
                base.searching();
            });

        base.$selectYear.bind("change",
            function () {
                base.searching();
            });

        base.$selectUnits.select2({ theme: 'bootstrap4', width: '100%' }).change(base.searching);
    }

    base.searching = function () {
        base.$onLoad = false;
        if (base.$tableDataTable)
            base.$tableDataTable.ajax.reload();
    }

    base.setParamsAfterSearch = function (forms) {
        if (base.$onLoad)
            return false;
        let url = URI(window.location.href);
        let urlBuilder = url.toString();
        urlBuilder = $.fn.buildParamURI(urlBuilder, "stats", forms.Status);
        urlBuilder = $.fn.buildParamURI(urlBuilder, "unit", forms.UserUnit);
        urlBuilder = $.fn.buildParamURI(urlBuilder, "year", forms.Year);

        window.history.pushState("", "", urlBuilder);
    }

    base.readURI = function () {
        var deferred = $.Deferred();
        let params = $.fn.dataURI(window.location.href);
        jQuery.fn.setElementValue('#requestSearchStats', params["stats"], function () {
            base.statsChange(params["stats"]);
        });
        jQuery.fn.setElementValue('#__searchYearRequest', params["year"]);
        jQuery.fn.setElementValue('#selectUnits', params["unit"]);

        deferred.resolve();
        return deferred.promise();
    }

    base.drawInit = () => $('#manageRequestingCostElement [prop-type]').on('click', base.tabRecordFuncHandle);

    base.tabRecordFuncHandle = function () {
        let propAct = $(this).attr('prop-type-act');
        let propTypes = $(this).attr('prop-type');
        let separators = propTypes.split('.');
        let action = separators[2];
        let point = $(this).closest('tr');
        var record = base.$tableDataTable.row(point).data();
        base.recordSelectorIndex = base.$tableDataTable.row(point).index();
        base.recordSelector = record;
        //record.id
        if (typeof (propAct) !== 'undefined') {
            switch (propAct) {
                case "pop":
                    switch (action) {
                        //xem lịch sử
                        case "history":
                            base.settingPage.fnUPop(base.settingPage.pop.historyLabel);
                            base.settingPage.fnURI('/ProfitPlan/ViewHistories?record=' + jQuery.fn.aesToParams(record.record));
                            base.settingPage.fnButtons({
                                //duyệt
                                accept: { visible: false },
                                //từ chối
                                decline: { visible: false },
                                //đóng
                                close: { visible: true }
                            });
                            base.settingPage.overlay = true;
                            break;
                        default:
                            break;
                    }
                    costJsBase.OpenModal(base.settingPage.pop);
                    break;
                case "confirm":
                    switch (action) {
                        case 'approve':
                            base.onAprove(point);
                            break;
                        case 'decline':
                            base.onDecline(point);
                            break;
                        case 'delete':
                            base.onDelete(point);
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }
    // function duyệt / từ chối yêu cầu

    base.onAprove = function (point) {
        Swal.fire({
            title: base.pageLanguage.approve.largeText,
            icon: 'success',
            showConfirmButton: true,
            showCancelButton: true,
            confirmButtonText: `<i class="fad fa-check"></i> Phê duyệt`,
            cancelButtonText: 'Hủy bỏ',
            showLoaderOnConfirm: true,
            position: 'top',
            preConfirm: function () {
                var deferred = $.Deferred();
                base.fncApproval(true).then(data => {
                    if (data.code !== costJsBase.enums.successCode) {
                        Swal.showValidationMessage(data.message);
                        deferred.resolve(false);
                    } else {
                        base.$tableDataTable.row(point).data(data.data).draw(false);
                        deferred.resolve(data.message);
                    }
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
                        timerInterval = setInterval(() => { }, 100);
                    },
                    willClose: () => {
                        clearInterval(timerInterval);
                    }
                });
            }
        });
    }

    base.onDelete = function () {
        Swal.fire({
            title: 'Bạn chắc chắn muốn xóa kế hoạch này ?',
            icon: 'success',
            showDenyButton: false,
            showConfirmButton: true,
            showCancelButton: true,
            confirmButtonText: `<i class="fad fa-check"></i> Xác nhận`,
            cancelButtonText: 'Hủy bỏ',
            showLoaderOnConfirm: true,
            position: 'top',
            preConfirm: function () {
                var deferred = $.Deferred();
                base.fncDelete().then(data => {
                    if (data.code !== costJsBase.enums.successCode) {
                        Swal.showValidationMessage(data.message);
                        deferred.resolve(false);
                    } else {
                        base.searching();
                        deferred.resolve(data.message);
                    }
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
                        timerInterval = setInterval(() => { }, 100);
                    },
                    willClose: () => {
                        clearInterval(timerInterval);
                    }
                });
            }
        });
    }

    base.onDecline = function (point) {
        let formId = costJsBase.Rad(30);
        Swal.fire({
            title: base.pageLanguage.decline.largeText,
            icon: 'error',
            showDenyButton: true,
            showConfirmButton: false,
            showCancelButton: true,
            denyButtonText: `<i class="fad fa-ban"></i> Từ chối`,
            cancelButtonText: 'Hủy bỏ',
            position: 'top',
            showLoaderOnDeny: true,
            didOpen: () => {
                jQuery.fn.validateSetup($('#' + formId),
                    {
                        rules: { ___reason: { required: true } },
                        messages: { ___reason: { required: 'Vui lòng nhập lý do từ chối' } }
                    });
            },
            preDeny: function () {
                let formValid = $('#' + formId).on().valid();
                if (formValid) {
                    var deferred = $.Deferred();
                    base.fncApproval(false).then(data => {
                        if (data.code !== costJsBase.enums.successCode) {
                            Swal.showValidationMessage(data.message);
                            deferred.resolve(false);
                        } else {
                            base.$tableDataTable.row(point).data(data.data).draw(false);
                            deferred.resolve(data.message);
                        }
                    });
                    return deferred.promise();
                } else
                    return false;
            },
            html: `<form id="` + formId + `"><div class="form-group text-left">
                                                <label class="col-form-label" for="inputError">Lý do từ chối</label>
                                                <textarea rows="3" id="___reason" name="___reason" type="text" class="form-control"></textarea>
                                          </div>
                                      </form>`
        }).then((result) => {
            if (result.isDenied) {
                Swal.fire({
                    title: result.value,
                    icon: 'success',
                    timer: 1500,
                    timerProgressBar: true,
                    didOpen: () => {
                        Swal.showLoading();
                        timerInterval = setInterval(() => { }, 100);
                    },
                    willClose: () => {
                        clearInterval(timerInterval);
                    }
                });
            }
        });
    }

    base.fncApproval = function (val) {
        var deferred = $.Deferred();
        let config = {
            Url: '/ProfitPlan/OnApproval',
            Data: {
                IsApproval: val,
                Reason: $('#___reason').val(),
                Record: base.recordSelector.record
            },
            async: false,
            beforeSend: function () { },
        };

        //gọi ajax
        costJsBase.PromissePost(config).done(data => {
            deferred.resolve(data);
        }).fail(err => {
            deferred.resolve(false);
        });
        return deferred.promise();
    }

    base.fncDelete = function () {
        var deferred = $.Deferred();
        let config = {
            Url: '/ProfitPlan/OnDelete',
            Data: {
                Record: base.recordSelector.record
            },
            async: false,
            beforeSend: function () { },
        };

        //gọi ajax
        costJsBase.PromissePost(config).done(data => {
            deferred.resolve(data);
        }).fail(err => {
            deferred.resolve(false);
        });
        return deferred.promise();
    }
}

//define if page use modal

$(document).ready(function () {
    let c = new ProfitPlanList();
    c.Setup();
});