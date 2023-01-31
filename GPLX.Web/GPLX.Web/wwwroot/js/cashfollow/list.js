var CashFollowList = function () {
    let base = this;
    let $tableSelector = $('#manageCashFollowList');
    let $dropStats = $('#searchStats');
    base.$tableDataTable = null;
    base.$labelStats = $('#lableStats');
    base.$searchButton = $('#requestSearchButton');
    base.$onLoad = true;
    base.$popOverlay = $('#modal-extra-large');
    base.rqType = $('#___requestFormRequestType');
    base.searchYearRequest = $('#__searchYearRequest');

    base.$selectUnits = $('#selectUnits');
    base.$selectYear = $('#__searchYearRequest');

    base.recordSelector = null;
    base.recordSelectorIndex = -1;
    base.pageLanguage = languages.vi.cashFollow;
    base.settingPage = {
        pop: {
            url: '',
            title: base.pageLanguage.list.popup.title,
            baseTitle: base.pageLanguage.list.popup.baseTitle,
            historyLabel: base.pageLanguage.list.table.historyLabel,
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
                this.pop.url = uri
            }
        },
        fnButtons: function (btns) {
            this.pop.buttons = btns;
        }
    };

    // set to elements
    base.Setup = function () {
        // promise -> readURI -> setup handler
        base.readURI().then(function () {
            base.setupElemsHandle();
            base.bindActions();
            base.setupDataTable();
        });
    }

    base.searchForm = function (data) {
        if (typeof (data) !== 'undefined') {
            data.Status = $dropStats.find('a[selected]').attr('prop-stats');

            let onQuery = costJsBase.ValueFromUrl('stats');
            if (onQuery === '-8888')
                data.Status = -8888;

            //let dateData = $ipDateRange.data('daterangepicker');
            //data.FromDate = dateData.startDate.format(base.pageLanguage.all.dateFormat);
            data.UserUnit = typeof base.$selectUnits != 'undefined' ? base.$selectUnits.val() : -100;
            data.Year = base.searchYearRequest.val();

            data.RequestType = base.rqType.val();
            base.setParamsAfterSearch(data);
        }
    }

    base.setupDataTable = function () {
        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            ajax: {
                url: "/CashFollow/Search",
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
                        return row.year || '';
                    },
                    width: "10%",
                    class: "text-center"
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return row.unitName;
                    },
                    class: "text-center"
                },
                {
                    idx: 3,
                    render: function (data, type, row, meta) {
                        return row.creator;
                    },
                    class: "text-center",
                    width: "14%"
                },
                {
                    idx: 4,
                    render: function (data, type, row, meta) {
                        return moment(row.createdDate).format("DD/MM/YYYY HH:mm");
                    },
                    class: "text-center",
                    width: "14%"
                },
                {
                    idx: 5,
                    render: function (data, type, row, meta) {
                        return costJsBase.FormatMoney(row.totalRevenue, '-');
                    },
                    class: "text-center",
                    width: "10%"
                },
                {
                    idx: 6,
                    render: function (data, type, row, meta) {
                        return costJsBase.FormatMoney(row.totalSpending, '-');
                    },
                    class: "text-center",
                    width: "10%"
                },
                {
                    idx: 7,
                    render: function (data, type, row, meta) {
                        return '<a class="text-danger" href="' + row.pathFile + '" target="_blank"><i class="fas fa-file-pdf"></i></a>';
                    },
                    class: "text-center",
                    width: "6%"
                },
                {
                    idx: 8,
                    render: function (data, type, row, meta) {
                        return row.status;
                    },
                    width: "8%",
                    class: "text-center"
                },
                {
                    idx: 9,
                    render: function (data, type, row, meta) {
                        let btmHtml = ``;
                        if (row.approvalable) {
                            btmHtml += `<a href="javascript: void(0);" prop-type="elems.table.approval" prop-type-act="pop" class="fs-17 mr-2 text-info" title="` + base.pageLanguage.list.table.aproveLabel + `">
                                <i class="fad fa-check"></i>
                            </a>`;
                        }

                        if (row.declineable) {
                            btmHtml += `<a href="javascript: void(0);" prop-type="elems.table.decline" prop-type-act="pop" class="fs-17 mr-2 text-danger" title="` + base.pageLanguage.list.table.declineLabel + `">
                            <i class="far fa-ban"></i>
                        </a>`;
                        }
                        if (row.deleteable) {
                            btmHtml += `<a href="javascript: void(0);" prop-type="elems.table.del" prop-type-act="pop" class="fs-17 mr-2 text-danger" title="` + base.pageLanguage.list.table.deleteLabel + `">
                            <i class="far fa-times"></i>
                        </a>`;
                        }
                        if (row.viewable) {
                            //todo

                            //btmHtml += `<a target="_blank" href="/CashFollow/Create?record=` + jQuery.fn.aesToParams(row.record) + `" class="text-info fs-17 mr-2" title="` + base.pageLanguage.list.table.viewLabel + `">
                            //<i class="fal fa-info-square"></i>
                            //</a>`;
                            //    btmHtml += `<a target="_blank" href="/CashFollow/CompareCashFollowActuallySpent?record=` + jQuery.fn.aesToParams(row.record) + `" class="text-info fs-17 mr-2" title="` + base.pageLanguage.list.table.compareLabel + `">
                            //    <i class="fal fa-chart-pie-alt"></i>
                            //</a>`;

                                btmHtml += `<a href="javascript: void(0);" prop-type="elems.table.history" prop-type-act="pop" class="text-info fs-17" title="` + base.pageLanguage.list.table.historyLabel + `">
                            <i class="fal fa-clock"></i>
                        </a>`;
                        }

                       
                        
                        return btmHtml;
                    },
                    width: "12%",
                    class: "text-center"
                }
            ],
            drawCallback: function () {
                base.drawInit();
            },
            scrollX: true,
            //issue: on change resolution --> header not match size with tbody
            responsive: false,
            fixedColumns: {
                right: 1
            },
        });
    }

    base.setupElemsHandle = function () {
        //$.fn.viRangeDateRegister({
        //    selector: $ipDateRange,
        //    callbackOnApply: function (ev, picker) {
        //        base.searching();
        //    }
        //});
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
        $dropStats.find('a').bind("click", function (e) {
            base.statsChange(this).then(base.searching);
        });

        base.$searchButton.bind("click", function () {
            base.searching();
        });
        base.searchYearRequest.bind("change", function () {
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
        urlBuilder = $.fn.buildParamURI(urlBuilder, "year", forms.Year);
        urlBuilder = $.fn.buildParamURI(urlBuilder, "unit", forms.UserUnit);
        window.history.pushState("", "", urlBuilder);
    }

    base.readURI = function () {
        var deferred = $.Deferred();
        let params = $.fn.dataURI(window.location.href);
        jQuery.fn.setElementValue('#searchStats', params["stats"], function () {
            base.statsChange(params["stats"]);
        });

        jQuery.fn.setElementValue('#__searchYearRequest', params["year"]);
        jQuery.fn.setElementValue('#selectUnits', params["unit"]);

        deferred.resolve();
        return deferred.promise();
    }

    base.drawInit = () => $('#manageCashFollowList [prop-type]').on('click', base.tabRecordFuncHandle);

    base.tabRecordFuncHandle = function () {
        let propAct = $(this).attr('prop-type-act');
        let propTypes = $(this).attr('prop-type');
        let separators = propTypes.split('.');
        let action = separators[2];
        let point = $(this).closest('tr');
        var record = base.$tableDataTable.row(point).data();
        base.recordSelectorIndex = base.$tableDataTable.row(point).index();
        base.recordSelector = record;


        if (typeof (propAct) !== 'undefined') {
            switch (propAct) {
                case "pop":
                    switch (action) {
                        case "decline":
                            let formID = costJsBase.Rad(30);
                            Swal.fire({
                                title: 'Bạn có chắc chắn muốn từ chối kế hoạch dòng tiền?',
                                icon: 'error',
                                showDenyButton: true,
                                showConfirmButton: false,
                                showCancelButton: true,
                                denyButtonText: `<i class="fad fa-ban"></i> Từ chối`,
                                cancelButtonText: 'Hủy bỏ',
                                position: 'top',
                                showLoaderOnDeny: true,
                                didOpen: () => {
                                    jQuery.fn.validateSetup($('#' + formID),
                                        {
                                            rules: { ___reason: { required: true } },
                                            messages: { ___reason: { required: 'Vui lòng nhập lý do từ chối' } }
                                        });
                                },
                                preDeny: function() {
                                    let formValid = $('#' + formID).on().valid();
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
                                html: `<form id="` +
                                    formID +
                                    `">
                                           <div class="form-group text-left">
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
                                            timerInterval = setInterval(() => {}, 100);
                                        },
                                        willClose: () => {
                                            clearInterval(timerInterval);
                                        }
                                    });
                                }
                            });
                            break;
                        case 'approval':
                            Swal.fire({
                                title: 'Bạn có chắc chắn muốn phê duyệt kế hoạch dòng tiền?',
                                icon: 'success',
                                showDenyButton: false,
                                showConfirmButton: true,
                                showCancelButton: true,
                                confirmButtonText: `<i class="fad fa-check"></i> Phê duyệt`,
                                cancelButtonText: 'Hủy bỏ',
                                showLoaderOnConfirm: true,
                                position: 'top',
                                preConfirm: function() {
                                    var deferred = $.Deferred();
                                    base.fncApproval(true).then(data => {
                                        if (data.code !== costJsBase.enums.successCode) {
                                            Swal.showValidationMessage(data.message)
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
                                            timerInterval = setInterval(() => {}, 100);
                                        },
                                        willClose: () => {
                                            clearInterval(timerInterval);
                                        }
                                    });
                                }
                            });
                            break;
                        case "history":
                            base.settingPage.fnUPop(base.settingPage.pop.historyLabel);
                            base.settingPage.fnURI('/CashFollow/ViewHistories?record=' + jQuery.fn.aesToParams(record.record));
                            base.settingPage.fnButtons({
                                //duyệt
                                accept: { visible: false },
                                //từ chối
                                decline: { visible: false },
                                //đóng
                                close: { visible: true }
                            });
                            base.settingPage.overlay = true;
                            costJsBase.OpenModal(base.settingPage.pop);
                            break;
                        case "del":
                            base.onDelete();
                            break;
                        default:
                            break;
                    }
                    break;
                default:;
                    break;
            }
        }
    }
    // function duyệt / từ chối yêu cầu
    base.fncApproval = function (val) {
        var deferred = $.Deferred();
        let config = {
            Url: '/CashFollow/Approval',
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
            Url: '/CashFollow/OnDelete',
            Data: {
                Record: base.recordSelector.record
            },
            async: false,
            beforeSend: function () { }
        };

        //gọi ajax
        costJsBase.PromissePost(config).done(data => {
            deferred.resolve(data);
        }).fail(err => {
            deferred.resolve(false);
        });
        return deferred.promise();
    }

    base.onDelete = function () {
        Swal.fire({
            title: 'Bạn có chắc chắn muốn xóa kế hoạch này ?',
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
}

$(document).ready(function () {
    let c = new CashFollowList();
    c.Setup();
});