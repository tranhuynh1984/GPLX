var ManageDashboard = function () {
    let base = this;
    let $tableSelector = $('#mngDashboard');
    let $dropStats = $('#searchStats');
    base.$tableDataTable = null;
    base.$labelStats = $('#lableStats');
    base.$searchButton = $('#requestSearchButton');
    base.$onLoad = true;
    base.rqType = $('#___requestFormRequestType');
    base.searchYearRequest = $('#__searchYearRequest');
    base.$planType = $('#planType');

    base.$selectUnits = $('#selectUnits');
    base.$selectYear = $('#__searchYearRequest');
    base.ckcAll = $('#ckcAll');
    base.btnExport = $('#btnExport');
    base.dropdownMenuButton = $('#dropdownMenuButton');
    base.$pop = $('#modal-extra-large');

    base.ckcRecords = [];
    // set to elements
    base.Setup = function () {
        // promise -> readURI -> setup handler
        base.readURI().then(function () {
            base.bindActions();
            base.setupDataTable();
        });
    }

    base.searchForm = function (data) {
        if (typeof (data) !== 'undefined') {
            data.Status = $dropStats.find('a[selected]').attr('prop-stats');
            data.UnitId = typeof base.$selectUnits != 'undefined' ? base.$selectUnits.val() : -100;
            data.Year = base.searchYearRequest.val();
            data.Type = base.$planType.val();

        }
    }

    base.setupDataTable = function () {
        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            ajax: {
                url: "/Dashboard/Search",
                data: function (d) {
                    base.searchForm(d);
                },
                type: 'post'
            },
            columns: [
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        let check = base.ckcRecords.filter(x => { return x.record === row.record && x.planType === row.planType });
                        let checkedAttr = check.length ? "checked" : "";
                        return `<div class="form-check">
                                    <input class="form-check-input" `+ checkedAttr + ` type="checkbox" ckcElement />
                                </div>`;
                    },
                    width: "2%",
                    class: "text-center"
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return row.year || '';
                    },
                    width: "10%",
                    class: "text-center"
                },
                {
                    idx: 3,
                    render: function (data, type, row, meta) {
                        return row.unitName;
                    },
                    class: "text-center"
                },
                {
                    idx: 4,
                    render: function (data, type, row, meta) {
                        return row.creatorName;
                    },
                    class: "text-center",
                    width: "14%"
                },
                {
                    idx: 5,
                    render: function (data, type, row, meta) {
                        return moment(row.createdDate).format("DD/MM/YYYY HH:mm");
                    },
                    class: "text-center",
                    width: "14%"
                },
                {
                    idx: 6,
                    render: function (data, type, row, meta) {
                        return row.planName;
                    },
                    class: "text-center",
                    width: "10%"
                },
                {
                    idx: 7,
                    render: function (data, type, row, meta) {
                        return '<a class="text-danger" href="' + row.pathPdf + '" target="_blank"><i class="fas fa-file-pdf"></i></a>';
                    },
                    class: "text-center",
                    width: "6%"
                },
                {
                    idx: 8,
                    render: function (data, type, row, meta) {
                        return row.statusName;
                    },
                    width: "8%",
                    class: "text-center"
                },
                {
                    idx: 9,
                    render: function (data, type, row, meta) {
                        let popupURL = '';
                        switch (row.planType) {
                            case 'revenue':
                                popupURL = '/RevenuePlan/ViewHistories?record=' + jQuery.fn.aesToParams(row.record);
                                break;
                            case 'profit':
                                popupURL = '/ProfitPlan/ViewHistories?record=' + jQuery.fn.aesToParams(row.record);
                                break;
                            case 'investment':
                                popupURL = '/InvestmentPlan/ViewHistories?record=' + jQuery.fn.aesToParams(row.record);
                                break;
                            case 'cashfollow':
                                popupURL = '/CashFollow/ViewHistories?record=' + jQuery.fn.aesToParams(row.record);
                                break;
                            default:
                                return '';
                        }
                        return '<a class="fs-17 text-info" prop-type="elems.table.history" data-href="' + popupURL + '" title="Xem lịch sử" href="javascript:void(0)"><i class="fal fa-clock"></i></a>';
                    },
                    width: "3%",
                    class: "text-center"
                }
            ],
            drawCallback: function () {
                $('input[ckcElement]').change(function () {
                    let point = $(this).closest('tr');
                    var rc = base.$tableDataTable.row(point).data();
                    base.onRecordSelect(rc, $(this).is(':checked'));
                });
                //base.ckcAll.prop('checked', false);
                const inputCheckedLength = $('input[ckcElement]:checked').length;
                let pLength = base.$tableDataTable.rows({ page: 'current' }).data().length;
                if (inputCheckedLength === pLength)
                    base.ckcAll.prop('checked', true);
                else
                    base.ckcAll.prop('checked', false);
            },
            scrollX: true,
            //issue: on change resolution --> header not match size with tbody
            responsive: false,
            fixedColumns: {
                right: 1
            }
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
            base.ckcRecords = [];
        });

        base.$searchButton.bind("click", function () {
            base.searching();
        });
        base.searchYearRequest.bind("change", function () {
            base.searching();
            base.ckcRecords = [];
        });
        base.$selectUnits.select2({ theme: 'bootstrap4', width: '100%' }).change(function () {
            base.searching();
            base.ckcRecords = [];
        });

        base.$planType.select2({ theme: 'bootstrap4', width: '100%' }).change(function () {
            base.searching();
            base.ckcRecords = [];
        });

        $('[data-export]').click(function () {
            var exportType = $(this).data('export');
            base.onExport(exportType);
        });
    }

    base.showHistory = function (target) {
        let popHref = $(target).data('href');
        if (popHref.length) {
            costJsBase.OpenModal({
                selector: base.$pop,
                overlay: true,
                dropContent: true,
                title: 'Lịch sử phê duyệt',
                buttons: {
                    close: {
                        visible: true
                    }
                },
                url: popHref
            });
        }
    }

    base.searching = function () {
        base.$onLoad = false;
        if (base.$tableDataTable) {
            base.$tableDataTable.ajax.reload();
            base.dropdownMenuButton.prop('disabled', true);
        }
    }

    base.readURI = function () {
        var deferred = $.Deferred();
        deferred.resolve();
        return deferred.promise();
    }

    base.ckcAll.change(function () {
        let isCheck = base.ckcAll.is(":checked");
        let rowsInPage = base.$tableDataTable.rows({ page: 'current' }).data();
        let nodesInPage = base.$tableDataTable.rows({ page: 'current' }).nodes();
        for (var i = 0; i < rowsInPage.length; i++) {
            if (isCheck) {
                base.onRecordSelect(rowsInPage[i], true);
                $(nodesInPage[i]).find('input[type="checkbox"]').prop("checked", true);
            } else {
                base.onRecordSelect(rowsInPage[i], false);
                $(nodesInPage[i]).find('input[type="checkbox"]').prop("checked", false);
            }
        }
    });

    base.onRecordSelect = function (rc, add) {
        if (add) {
            let check = base.ckcRecords.filter(x => { return x.record === rc.record && x.planType === rc.planType });
            if (!check.length)
                base.ckcRecords.push(rc);
        } else {
            base.ckcRecords = $.grep(base.ckcRecords, function (value) {
                return value !== rc;
            });
        }
        let enableMultipleAct = base.ckcRecords.length > 0;
        let pLength = base.$tableDataTable.rows({ page: 'current' }).data().length;
        setTimeout(function () {
            const inputCheckedLength = $('input[ckcElement]:checked').length;
            if (inputCheckedLength === pLength)
                base.ckcAll.prop('checked', true);
            else
                base.ckcAll.prop('checked', false);
        }, 50);


        base.dropdownMenuButton.prop('disabled', !enableMultipleAct);
    }

    base.onExport = function (exportType) {
        if (base.ckcRecords.length === 0) {
            costJsBase.EventNotify('warning', "Chưa có kế hoạch nào được chọn!");
            return false;
        } else {
            var data = [];
            base.ckcRecords.forEach(x => {
                data.push({
                    Record: x.record,
                    Type: x.planType
                });
            });

            costJsBase.Post({
                Url: '/Dashboard/ExportFile',
                Data: {
                    Data: data,
                    ExportType: exportType
                }
            },
                function (d) {
                    if (d.code === 200) {
                        var win = window.open(d.exportPath, '_blank');
                        if (win) {
                            win.focus();
                        } else {
                            alert('Please allow popups for this website');
                        }

                    } else {
                        costJsBase.EventNotify('error', d.message);
                    }
                },
                function (e) {
                    costJsBase.EventNotify('error', 'Lỗi hệ thống, vui lòng thử lại sau!');
                });
        }
    }
}

$(document).ready(function () {
    let c = new ManageDashboard();
    c.Setup();

    $(document).on('click', '[prop-type="elems.table.history"]', function () {
        c.showHistory(this);
    });
});