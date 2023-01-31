var Chief = function () {
    let base = this;
    base.$tableDataTable = null;
    base.$tableDataTableSelected = null;
    base.$btnSelectExcelFile = $('#btnSelectExcelFile');
    base.$inputFileCostEstimateExcelFile = $('#costEstimateExcelFile');
    let $tableSelector = $('#chiefApprove');

    base.$btnSelectContractFile = $('#btnSelectContractFile');
    base.$recordID = $('#__recordCostEstimate');
    base.$selectReportForWeek = $('#selectReportForWeek');
    base.$requestSearchKeywords = $('#requestSearchKeywords');
    base.$btnSearching = $('#btnSearching');

    base.fixedColumns = {
        left: 3,
        right: 1
    }

    base.columnTableConfigs = [
        {
            idx: 0,
            width: "30px",
            class: "text-center dt-control",
            "defaultContent": '',
            render: function (data, type, row, meta) {
                return '';
            },
        },
        // Ô checkbox
        {
            idx: 1,
            render: function (data, type, row, meta) {
                let checked = '';
                let disabled = '';

                if (typeof selectedOlderRequest != 'undefined') {
                    if (row.isDeleted === 1)
                        disabled = 'disabled';
                    if (row.status === 1)
                        checked = 'checked';
                }
                return '<div class="form-check">' +
                    '<input class="form-check-input position-static" ' + disabled + ' ckcElement type="checkbox" ' + checked + ' />' +
                    '</div>';
            },
            width: "auto",
            class: "text-center"
        },
        // Mã dự trù
        {
            idx: 2,
            render: function (data, type, row, meta) {
                return row.requestCode;
            },
            width: "15%",
            class: 'align-middle'
        },
        // nội dung
        {
            idx: 3,
            render: function (data, type, row, meta) {
                return row.requestContent;
            },
            width: "auto",
            class: 'align-middle'
        },
        // Tổng số tiền
        {
            idx: 4,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.cost);
            },
            class: "text-right align-middle",
            width: "6%"
        },
        // Đơn vị
        {
            idx: 5,
            render: function (data, type, row, meta) {
                return row.unitName;
            },
            width: "6%",
            class: "align-middle"
        },
        // Bộ phận
        {
            idx: 6,
            render: function (data, type, row, meta) {
                return row.departmentName;
            },
            width: "6%",
            class: "align-middle"
        },
        // Người đề xuất
        {
            idx: 7,
            render: function (data, type, row, meta) {
                return row.requesterName;
            },
            class: "text-left align-middle",
            width: "6%"
        },
        // Thời gian đề xuất
        {
            idx: 8,
            render: function (data, type, row, meta) {
                return row.payWeekName;
            },
            class: "text-center align-middle",
            width: "6%"
        }
    ];

    base.ckcRecords = [];

    base.ckcAll = $('#ckcAll');

    // promisse function
    base.onLoad = function () {
        var deferred = $.Deferred();
        let columnsEnable = [];
        $tableSelector.find('thead tr th').each(function () {
            columnsEnable.push(parseInt($(this).attr('position')));
        });
        let columns = [];
        for (var i = 0; i < base.columnTableConfigs.length; i++) {
            for (var j = 0; j < columnsEnable.length; j++) {
                if (base.columnTableConfigs[i].idx === columnsEnable[j]) {
                    columns.push(base.columnTableConfigs[i]);
                    break;
                }
            }
        }

        deferred.resolve(columns);
        return deferred.promise();
    }

    base.setupDataTable = function (cfgs) {
        let settings = {
            selector: $tableSelector,
            columns: cfgs,
            scrollX: true,
            responsive: false,
            fixedColumns: base.fixedColumns,
            drawCallback: function () {
                $('input[ckcElement]').change(function () {
                    let point = $(this).closest('tr');
                    var rc = base.$tableDataTable.row(point).data();
                    base.onRecordSelect(rc, $(this).is(':checked'));
                });

                setTimeout(function () {
                    base.$tableDataTable.columns.adjust();

                    let dataSize = base.$tableDataTable.rows().data().length;
                    let record = costJsBase.ValueFromUrl('record');

                    // trường hợp tạo mới
                    // mặc định checkAll
                    if (record == null && dataSize > 0) {
                        base.ckcAll.prop('checked', true).change();
                    }
                    base.onWindowLoad();
                }, 350);
            }
        };

        if (typeof selectedOlderRequest === 'undefined') {
            settings.ajax = {
                url: "/CostEstimate/SearchRequestList",
                data: function (d) {
                    let rpWeek = base.$selectReportForWeek.val();
                    //d.ReportWeek = rpWeek.length ? parseInt(rpWeek) : 0;
                    d.Keywords = base.$requestSearchKeywords.val();
                    d.RequestType = costJsBase.ValueFromUrl('type');
                },
                type: "post"
            };
            settings.serverSide = true;
            settings.paging = true;
        } else {
            settings.serverSide = false;
            settings.data = selectedOlderRequest;
            settings.initComplete = function() {
                $.fn.dataTable.ext.search.push(
                    function(settings, searchData, index, rowData, counter) {
                        var searchKeys = base.$requestSearchKeywords.val();
                        if (!searchKeys.length)
                            return true;
                        else {
                            let valid = true;
                            if (searchKeys.length) {
                                valid = rowData.requestCode.toLowerCase().indexOf(searchKeys.toLowerCase()) !== -1 || rowData.requestContent?.toLowerCase().indexOf(searchKeys.toLowerCase()) !== -1;
                            }
                            return valid;
                        }
                    }
                );
            };
            settings.searching = true;
        }

        base.$tableDataTable = $.fn.jsTableRegister(settings);
    }

    base.$formLang = languages.vi.costElement.create;

    // setup khi document ready
    base.setup = function () {
        base.onLoad().then(function (c) {
            base.setupDataTable(c);
        });

        if (typeof selectedOlderRequest === 'undefined') {
            $.fn.eEnterActions({
                selector: base.$requestSearchKeywords,
                action: function() {
                    // 
                    base.searching();
                }
            });
            base.$btnSearching.bind('click', base.searching);
        } else {
            base.$requestSearchKeywords.keyup(function () {
                base.$tableDataTable.draw();
            });
            base.$btnSearching.bind('click', function () {
                base.$tableDataTable.draw();
            });
        }
        

        // base.$btnApprove.bind('click', base.onApprove);

        $('[data-toggle="tooltip"]').tooltip();

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

        $('#chiefApprove tbody').on('dblclick', 'tr', function () {
            var tr = $(this).closest('tr');
            var row = base.$tableDataTable.row(tr);

            if (row.child.isShown()) {
                // This row is already open - close it
                row.child.hide();
                tr.removeClass('shown');
            }
            else {
                // Open this row
                row.child(base.showDetail(row.data())).show();
                tr.addClass('shown');
            }
        });

        $('#chiefApprove tbody').on('click', '.dt-control', function () {
            var tr = $(this).closest('tr');
            var row = base.$tableDataTable.row(tr);

            if (row.child.isShown()) {
                row.child.hide();
                tr.removeClass('shown');
            }
            else {
                row.child(base.showDetail(row.data())).show();
                tr.addClass('shown');
            }
        });
    }

    base.onRecordSelect = function (rc, add) {
        if (add) {
            base.ckcRecords.push(rc);
        } else {
            base.ckcRecords = $.grep(base.ckcRecords, function (value) {
                return value !== rc;
            });
        }
        let pLength = base.$tableDataTable.rows({ page: 'current' }).data().length;
        base.ckcAll.prop('checked', base.ckcRecords.length === pLength);
    }

    base.onWindowLoad = function() {
        let inputCheck = $('[ckcElement]:checked').length;
        let pLength = base.$tableDataTable.rows({ page: 'current' }).data().length;
        base.ckcAll.prop('checked', inputCheck === pLength);
    }

    // hiển thị icon theo loại file + link download
    base.displayIconByExtension = function (file) {
        if (!file.length) {
            return '';
        }
        let className = "";
        let extension = '';
        if (!file.length)
            return '';
        let separators = file.split('.');
        extension = '.' + separators[separators.length - 1];
        switch (extension) {
            case '.png':
            case '.jpg':
            case '.gif':
                className = 'fas fa-image';
                break;
            case '.xlsx':
            case '.xls':
                className = 'fas fa-file-excel';
                break;
            case '.doc':
            case '.docx':
                className = 'far fa-file-word';
                break;
            case '.csv':
                className = 'fas fa-file-csv';
                break;
            default:
                className = 'far fa-file';
                break;
        }
        return '<a href="' + file + '" target="_blank"><i class="' + className + ' mr-2"></i>Chi tiết</a>';
    }

    // phê duyệt - từ chối
    base.onApprove = function () {
        let dataApprove = [];

        let weekVal = base.$selectReportForWeek.val();
        let iWeek = 0;
        if (weekVal.length) {
            iWeek = parseInt(weekVal);
        }

        if (iWeek <= 0 || isNaN(iWeek)) {
            costJsBase.EventNotify('warning', 'Bạn chưa chọn tuần lập báo cáo!');
            return false;
        }

        if (base.ckcRecords.length === 0) {
            costJsBase.EventNotify('warning', 'Bạn chưa chọn yêu cầu cần phê duyệt!');
        } else {
            for (var i = 0; i < base.ckcRecords.length; i++) {
                let rec = base.ckcRecords[i];
                rec.record = (base.ckcRecords[i].id);
                rec.accountImage = base.ckcRecords[i].requestImage;
                dataApprove.push(rec);
            }
            costJsBase.Post({
                Url: '/CostEstimate/OnCreate',
                Data: {
                    Data: dataApprove,
                    ReportForWeek: iWeek,
                    Record: $('#__recordCostEstimate').val()
                },
                async: true,
                beforeSend: function () {
                    costJsBase.ButtonState({
                        target: base.$btnApprove,
                        state: 'loading',
                        disabled: true,
                        text: 'Đang xử lý ...'
                    });
                }
            },
                base.onCreateSuccess,
                base.onCreateFailure);
        }
    }
    // tìm kiếm
    base.searching = function () {
        base.$onLoad = false;
        if (base.$tableDataTable)
            base.$tableDataTable.ajax.reload();
    }
    // set warning khi rời trang nếu còn dữ liệu chưa lưu
    base.onLiveWarning = function (onWarning) {
        if (onWarning) {
            $(window).on("beforeunload", function (event) {
                localStorage.removeItem(base.$cacheName);
                return '';
            });
        } else {
            $(window).off('beforeunload');
        }
    }

    base.onCreateSuccess = function (data) {
        if (data.code === 200) {
            base.onLiveWarning(false);
            costJsBase.EventNotify('success', data.message);
            costJsBase.ButtonState({
                target: base.$btnApprove,
                state: 'done',
                disabled: true,
                html: '<i class="fas fa-check mr-2"></i> Đang chuyển hướng ...',
            });

            setTimeout(() => {
                window.location.href = '/CostEstimate/Overview?record=' + jQuery.fn.aesToParams(data.record);
            }, 1500);
        } else {
            costJsBase.ButtonState({
                target: base.$btnApprove,
                state: 'normal',
                disabled: false,
                html: '<i class="fas fa-check mr-2"></i> Phê duyệt'
            });
            costJsBase.EventNotify('error', data.message);
        }
    }

    base.onCreateFailure = function (err) {
        base.onLiveWarning(true);
        costJsBase.EventNotify('error', 'Có lỗi xảy ra, vui lòng thử lại sau!');
        costJsBase.ButtonState({
            target: base.$btnApprove,
            state: 'normal',
            disabled: false,
            html: '<i class="fad fa-check mr-2"></i> Xác nhận',
        });
    }

    base.showDetail = function (d) {
        let par = moment(d.billDate, 'YYYY-MM-DDTHH:mm:ss');
        let sYear = par.year() > 1900 ? par.format('DD/M/YYYY') : '-';

        let decliner = '';
        if (typeof d.isDeleted != 'undefined') {
            if (d.isDeleted === 1) {
                let updaterDate = moment(d.updatedDate, 'YYYY-MM-DDTHH:mm:ss');
                decliner = '<td class="font-weight-bold">Người từ chối:</td>' +
                    '<td>' + (d.updaterName) + '</td>' +
                    '<td class="font-weight-bold">Thời gian từ chối:</td>' +
                    '<td>' + updaterDate.format("DD/MM/YYYY HH:mm") + '</td>';
            }
        }

        // `d` is the original data object for the row
        return '<table cellpadding="8" cellspacing="0" border="0" style="width: 100%;">' +
            '<tr>' +
            '<td class="font-weight-bold">Nhà cung cấp:</td>' +
            '<td>' + (d.supplierName ?? '-') + '</td>' +
            '<td class="font-weight-bold">Số hóa đơn</td>' +
            '<td>' + (d.billCode ?? '-') + '</td>' +
            '<td class="font-weight-bold">Giá trị hóa đơn</td>' +
            '<td>' + costJsBase.FormatMoney(d.billCost, '-') + '</td>' +
            '</tr>' +
            '<tr>' +
            '<td class="font-weight-bold">Ngày hóa đơn:</td>' +
            '<td>' + sYear + '</td>' +
            '<td class="font-weight-bold">Hình thức chi:</td>' +
            '<td>' + d.payForm + '</td>' +
            '<td class="font-weight-bold">Nhóm chi phí:</td>' +
            '<td>' + d.costEstimateItemTypeName + '</td>' +
            '</tr>' +
            '<tr>' +
            '<td class="font-weight-bold">Nhóm dự trù:</td>' +
            '<td>' + d.costEstimateGroupName + '</td>' +
            '<td class="font-weight-bold">Chứng từ:</td>' +
            '<td>' + (d.requestImage.length ? base.displayIconByExtension(d.requestImage) : '-') + '</td>' +
            '<td class="font-weight-bold">Giải trình:</td>' +
            '<td>' + (d.explanation ?? '-') + '</td>' +
            '</tr>' +
            '<tr>' +
            '<td class="font-weight-bold">Người tạo phiếu:</td>' +
            '<td>' + d.creatorName + '</td>' +
            decliner +
            '</tr>' +
            '</table>';
    }
}

$(document).ready(function () {
    let c = new Chief();
    c.setup();
});