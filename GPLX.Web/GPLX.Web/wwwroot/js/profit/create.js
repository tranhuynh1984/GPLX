var RevenuePlan = function () {
    let base = this;
    base.$tableDataTable = null;
    base.$tableDataAggregate = null;
    base.$btnSelectExcelFile = $('#btnSelectExcelFile');
    base.$inputFileCostEstimateExcelFile = $('#costEstimateExcelFile');
    base.$btnCreate = $('#btnCreate');
    let $tableSelector = $('#createRevenueExcel');
    let $tableAggregate = $('#createAggregates');

    base.$labelFileName = $('#labelFileSelect');
    base.$btnStartUpload = $('#btnStartUpload');
    base.$errorCollapse = $('#errorCollapse');
    base.$boxExcelErrors = $('#boxExcelErrors');
    base.$inputSearchOutBox = $('#inputSearchOutBox');
    base.$recordID = $('#__recordCostEstimate');
    base.$checkClearTable = $('#checkClearTable');
    base.$selectReportForWeek = $('#selectReportForWeek');
    base.$lblWeekActuallySpent = $('#lblWeekActuallySpent');

    base.selectTableSearch = $('#__selectTableSearch');
    base.inputTableSearch = $('#__inputTableSearch');
    base.btnTableSearch = $('#__btnTableSearch');
    base.selectYear = $('#selectYear');

    base.recordSelector = null;
    base.recordSelectorIndex = -1;

    base.excelExts = [".xlsx"];
    base.actOnUpload = function () { }
    base.displayPercent = function (row, s) {
        let percent = row.profitPlanGroupName.startsWith('Biên lợi nhuận') ? '%' : '';
        if (s === '-' || !s.length)
            return s;
        if (row.style === 'bold')
            return '<b>' + (s + percent) + '</b>';
        return s + percent;
    }

    base.columnAggregates = [
        {
            idx: 0,
            render: function(data, type, row, meta) {
                return row.no;
            },
            width: "3%",
            class: "text-center align-middle",
            fieldName: 'no',
            data: 'no'
        },
        // Nội dung
        {
            idx: 1,
            render: function(data, type, row, meta) {
                return row.style === 'italic' ? ('<i>' + row.profitPlanGroupName + '</i>') : row.profitPlanGroupName;
            },
            width: "auto",
            class: 'align-middle',
            fieldName: 'profitPlanGroupName',
            data: "profitPlanGroupName"
        },
        // TotalCosh
        {
            idx: 2,
            render: function(data, type, row, meta) {
                return costJsBase.FormatMoney(row.totalCosh, '-');
            },
            width: "20%",
            class: 'align-middle text-center',
            fieldName: 'totalCosh',
            data: "totalCosh"
        },
        {
            idx: 3,
            render: function(data, type, row, meta) {
                let s = costJsBase.FormatMoney(row.proportion, '-');
                return s !== '-' ? s + '%' : s;

            },
            width: "20%",
            class: 'align-middle text-center',
            fieldName: 'proportion',
            data: "proportion"
        }
    ];

    base.columnTableConfigs = [
        // TT
        {
            idx: 0,
            render: function (data, type, row, meta) {
                return row.no;
            },
            width: "3%",
            class: "text-center align-middle",
            fieldName: 'no',
            data: 'no'
        },
        // Nội dung
        {
            idx: 1,
            render: function (data, type, row, meta) {
                return row.style === 'bold' ? ('<b>' + row.profitPlanGroupName + '</b>') : row.profitPlanGroupName;
            },
            width: "auto",
            class: 'align-middle',
            fieldName: 'profitPlanGroupName',
            data: "profitPlanGroupName"
        },
        // T1
        {
            idx: 2,
            render: function (data, type, row, meta) {
                return base.displayPercent(row, costJsBase.FormatMoney(row.m1, '-'));
            },
            width: "5%",
            class: 'align-middle text-center',
            fieldName: 'm1',
            data: "m1"
        },
        {
            idx: 3,
            render: function (data, type, row, meta) {
                return base.displayPercent(row, costJsBase.FormatMoney(row.m2, '-'));
            },
            width: "5%",
            class: 'align-middle text-center',
            fieldName: 'm2',
            data: "m2"
        },
        {
            idx: 4,
            render: function (data, type, row, meta) {
                return base.displayPercent(row, costJsBase.FormatMoney(row.m3, '-'));
            },
            width: "5%",
            class: 'align-middle text-center',
            fieldName: 'm3',
            data: "m3"
        },
        {
            idx: 5,
            render: function (data, type, row, meta) {
                return base.displayPercent(row, costJsBase.FormatMoney(row.m4, '-'));
            },
            width: "5%",
            class: 'align-middle text-center',
            fieldName: 'm4',
            data: "m4"
        },
        {
            idx: 6,
            render: function (data, type, row, meta) {
                return base.displayPercent(row, costJsBase.FormatMoney(row.m5, '-'));
            },
            width: "5%",
            class: 'align-middle text-center',
            fieldName: 'm5',
            data: "m5"

        },
        {
            idx: 7,
            render: function (data, type, row, meta) {
                return base.displayPercent(row, costJsBase.FormatMoney(row.m6, '-'));
            },
            width: "5%",
            class: 'align-middle text-center',
            fieldName: 'm6',
            data: "m6"

        },
        {
            idx: 8,
            render: function (data, type, row, meta) {
                return base.displayPercent(row, costJsBase.FormatMoney(row.m7, '-'));
            },
            width: "5%",
            class: 'align-middle text-center',
            fieldName: 'm7',
            data: "m7"

        },
        {
            idx: 9,
            render: function (data, type, row, meta) {
                return base.displayPercent(row, costJsBase.FormatMoney(row.m8, '-'));
            },
            width: "5%",
            class: 'align-middle text-center',
            fieldName: 'm8',
            data: "m8"
        },
        {
            idx: 10,
            render: function (data, type, row, meta) {
                return base.displayPercent(row, costJsBase.FormatMoney(row.m9, '-'));
            },
            width: "5%",
            class: 'align-middle text-center',
            fieldName: 'm9',
            data: "m9"

        },
        {
            idx: 11,
            render: function (data, type, row, meta) {
                return base.displayPercent(row, costJsBase.FormatMoney(row.m10, '-'));
            },
            width: "5%",
            class: 'align-middle text-center',
            fieldName: 'm10',
            data: "m10"

        },
        {
            idx: 12,
            render: function (data, type, row, meta) {
                return base.displayPercent(row, costJsBase.FormatMoney(row.m11, '-'));
            },
            width: "5%",
            class: 'align-middle text-center',
            fieldName: 'm11',
            data: "m11"

        },
        {
            idx: 13,
            render: function (data, type, row, meta) {
                return base.displayPercent(row, costJsBase.FormatMoney(row.m12, '-'));
            },
            width: "5%",
            class: 'align-middle text-center',
            fieldName: 'm12',
            data: "m12"

        },
        {
            idx: 14,
            render: function (data, type, row, meta) {
                return '<b>' + base.displayPercent(row, costJsBase.FormatMoney(row.total, '-')) + '</b>'
            },
            width: "5%",
            class: 'align-middle text-center',
            fieldName: 'total',
            data: "total"

        },
        {
            idx: 15,
            render: function (data, type, row, meta) {
                // Ngoài y tế
                let s = costJsBase.FormatMoney(row.proPortion, '')
                //if (row.style === 'bold')
                //    return s.length ? s + '%' : '';
                return s.length ? s + '%' : "0%";
            },
            width: "5%",
            class: 'align-middle text-center',
            fieldName: 'proPortion',
            data: "proPortion"
        }
    ];

    base.profitData = null;

    base.onLoad = function () {
        var deferred = $.Deferred();
        let columnsEnable = [];
        $('#createRevenueExcel').find('thead tr th').each(function () {
            columnsEnable.push(parseInt($(this).attr('position')));
        });

        let columns = [];
        for (var i = 0; i < base.columnTableConfigs.length; i++) {
            for (var j = 0; j < columnsEnable.length; j++) {
                if (columnsEnable.indexOf(base.columnTableConfigs[i].idx) !== -1) {
                    columns.push(base.columnTableConfigs[i]);
                    break;
                }
            }
        }

        deferred.resolve(columns);
        return deferred.promise();
    }
    // khởi tạo datatable
    base.setupDataTable = function (cfgs) {
        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            columns: cfgs,
            drawCallback: function () {
                base.$btnCreate.prop('disabled', base.$tableDataTable == null ? true : base.$tableDataTable.rows().data().length === 0);
            },
            searching: false,
            scrollX: true,
            responsive: false,
            altEditor: true,
            select: true,
            paging: false,
            autoWidth: true,
            fixedHeader: true,
            serverSide: false,
            fixedColumns: {
                left: 2,
            }
        });

        base.$tableDataAggregate = $.fn.jsTableRegister({
            selector: $tableAggregate,
            columns: base.columnAggregates,
            drawCallback: function () { },
            searching: false,
            scrollX: true,
            responsive: false,
            altEditor: true,
            select: true,
            paging: false,
            autoWidth: true,
            fixedHeader: true,
            serverSide: false
        });
    }

    base.intVal = function (i) {
        return typeof i === 'string' ?
            i.replace(/[\$,]/g, '') * 1 :
            typeof i === 'number' ?
            i : 0;
    }

    // cấu hình các chuỗi string cố định
    base.$formLang = languages.vi.profit.create;

    // gắn sự kiện cho các nút
    base.bindActions = function () {
        base.$btnSelectExcelFile.bind('click', function () {
            base.excelExts = ['.xlsx'];
            base.$inputFileCostEstimateExcelFile.attr('accept', '.xlsx').click();;
            base.actOnUpload = function () {
                costJsBase.JsUploadFile({
                    selector: base.$inputFileCostEstimateExcelFile,
                    Url: '/ProfitPlan/OnExcelUpload?year=' + base.selectYear.val(),
                    async: true,
                    beforeSend: function () {
                        base.$btnStartUpload.html(base.$formLang.pendingUpload);
                        base.showAndClearErrorBox();
                    },
                    complete: function () {
                        base.$btnStartUpload.html(base.$formLang.oldButtonUpload);
                    }
                },
                    base.onExcelUpload,
                    function (err) {
                        base.clearAll();
                        base.$btnStartUpload.html(base.$formLang.oldButtonUpload);
                        costJsBase.EventNotify('error', 'Tải lên không thành công, vui lòng thử lại sau!');
                    }
                );
            };
        });

        base.$inputFileCostEstimateExcelFile.bind('change', function () {
            let files = $(this).get(0).files;
            if (files.length > 0) {
                let selectFileName = files[0].name;

                let validateFileName = costJsBase.CheckExtension(selectFileName, base.excelExts);
                if (!validateFileName) {
                    costJsBase.EventNotify('error', base.$formLang.invalidExcelFileWarning);
                    base.clearInput(base.$inputFileCostEstimateExcelFile);
                } else {
                    if (base.validFileSize(files[0].size)) {
                        costJsBase.EventNotify('error', base.$formLang.maxsizeWarning);
                        base.clearInput(base.$inputFileCostEstimateExcelFile);
                    } else {
                        base.hideUpploadFile(false, selectFileName);
                    }
                }
            } else {
                base.hideUpploadFile(true);
            }
        });

        base.$btnStartUpload.bind('click', function () {
            let year = base.intVal(base.selectYear.val());
            if (year === -100) {
                costJsBase.EventNotify('warning', 'Bạn chưa chọn năm tài chính');
                return false;
            }

            let files = base.$inputFileCostEstimateExcelFile.get(0).files;
            if (files.length === 0) {
                return false;
            } else {
                base.actOnUpload();
            }
        });

        base.$btnCreate.bind('click', base.onCreate);

        base.selectYear.change(function () {
            var linkRef = $('a[downloadExcel]').attr('href');
            let iYear = parseInt($(this).val());
            if (iYear > 0) {
                let idxOfYear = linkRef.indexOf('&year=');
                linkRef = linkRef.substring(0, idxOfYear);
                linkRef = linkRef + "&year=" + iYear;
                $('a[downloadExcel]').attr('href', linkRef);
                $('a[downloadExcel]').removeClass('disabled');
            }
            else
                $('a[downloadExcel]').addClass('disabled');
        });
    }

    base.validFileSize = function (size) {
        var kbSize = parseFloat(size);
        var mbSize = parseFloat(kbSize / 1024 / 1024); //==> to MB
        return mbSize > 10;
    }
    // xóa dữ liệu input file
    base.clearInput = function (selector) {
        selector.val('');
    }

    base.clearAll = function () {
        base.clearInput(base.$inputFileCostEstimateExcelFile);
        base.$tableDataTable.clear().draw();
        base.$tableDataAggregate.clear().draw();
        base.hideUpploadFile(true);
        base.disableOnDoneUpload(true);
        base.$btnStartUpload.html(base.$formLang.oldButtonUpload);
        base.showAndClearErrorBox(false);
    }

    //sự kiện khi tải lên file excel hoàn thành
    base.onExcelUpload = function (data) {
        base.clearAll();
        switch (data.code) {
            case costJsBase.enums.noContentCode:
                let msg = typeof (data.errors) === 'object' ? data.errors[0].message : 'Tệp tải lên không đúng định dạng!';
                costJsBase.EventNotify('error', msg);
                break;
            case costJsBase.enums.errorCode:
                costJsBase.EventNotify('error', data.message);
                if (typeof data.excelFileError != 'undefined' && data.excelFileError?.length) {
                    base.showAndClearErrorBox(true);
                    base.$errorCollapse.append('Tệp chi tiết mã lỗi: <a href="' + data.excelFileError + '" target="_blank">Tải xuống</a>');
                }
                break;
            case costJsBase.enums.successCode:
                if (data.isValid) {
                    if (!data.specifyFieldValue.profitPlanDetails.length || !data.specifyFieldValue.profitPlanAggregates.length) {
                        costJsBase.EventNotify('warning', 'Không có dữ liệu trong danh sách đã nhập!');
                    } else {
                        base.profitData = data.specifyFieldValue;
                        base.$tableDataTable.rows.add(data.specifyFieldValue.profitPlanDetails).draw();
                        base.$tableDataAggregate.rows.add(data.specifyFieldValue.profitPlanAggregates).draw();
                        costJsBase.EventNotify('success', 'Nhập dữ liệu thành công!');
                    }

                } else {
                    costJsBase.EventNotify('warning', 'Dữ liệu không hợp lệ, vui lòng xem chi tiết phía dưới!');
                    for (var i = 0; i < data.errors.length; i++) {
                        let fullErr = data.errors[i].column !== null ? '<b>' + data.errors[i].column + '</b>: ' + data.errors[i].message : data.errors[i].message;
                        base.$errorCollapse.append(fullErr);
                        base.$errorCollapse.append('<br />');
                    }
                    base.showAndClearErrorBox(true);
                }
                break;
            default:
                costJsBase.EventNotify('error', 'Có lỗi xảy ra, tải lên không thành công!');
                break;
        }
    }

    // xóa box cảnh báo lỗi sai định dạng excel
    base.showAndClearErrorBox = function (show) {
        if (!show) {
            base.$boxExcelErrors.addClass('d-none').removeClass('d-block');
            base.$errorCollapse.html('');
        } else {
            base.$boxExcelErrors.addClass('d-block').removeClass('d-none');
        }
    }

    // on.off disable các nút khi bắt đầu tải lên
    base.disableOnDoneUpload = function (disabled) {
        base.$btnStartUpload.prop('disabled', !disabled);
        base.$btnSelectExcelFile.prop('disabled', !disabled);
    }

    base.hideUpploadFile = function (hide, text) {
        if (hide) {
            base.$labelFileName.text('').removeClass(base.$formLang.classInline).addClass(base.$formLang.classHidden);
            base.$btnStartUpload.removeClass(base.$formLang.classInline).addClass(base.$formLang.classHidden);
        } else {
            base.$labelFileName.text(text).addClass(base.$formLang.classInline).removeClass(base.$formLang.classHidden);
            base.$btnStartUpload.addClass(base.$formLang.classInline).removeClass(base.$formLang.classHidden);
        }
    }

    // setup khi document ready
    base.setup = function () {
        base.onLoad().then(function (c) {
            base.setupDataTable(c);
            base.$btnCreate.prop('disabled', base.$tableDataTable.rows().data().length === 0);
        });
        base.bindActions();
        $('[data-toggle="tooltip"]').tooltip();

        base.inputTableSearch.keyup(function () {
            base.$tableDataTable.draw();
        });

        base.selectTableSearch.change(function () {
            base.$tableDataTable.draw();
        });
    }

    base.onCreate = function () {
        let year = base.intVal(base.selectYear.val());
        if (year === -100) {
            costJsBase.EventNotify('warning', 'Bạn chưa chọn năm tài chính!');
            return false;
        }

        if (base.profitData != null && base.profitData?.profitPlan != null) {
            if (base.profitData.profitPlan.year !== year) {
                costJsBase.EventNotify('warning', 'Năm tài chính không khớp với sheet dữ liệu đã tải lên!');
                return false;
            }
        } else {
            costJsBase.EventNotify('warning', 'Không có dữ liệu cần lưu lại!');
            return false;
        }

        costJsBase.Post({
            Url: '/ProfitPlan/OnCreate',
            Data: base.profitData,
            async: true,
            beforeSend: function () {
                costJsBase.ButtonState({
                    target: base.$btnCreate,
                    state: 'loading',
                    disabled: true,
                    text: 'Đang lưu lại ...'
                });
            }
        }, base.onCreateSuccess, base.onCreateError);
    }

    // cảnh báo khi rời trang
    base.onLiveWarning = function (onWarning) {
        if (onWarning) {
            $(window).on("beforeunload", function (event) {
                return "Are you sure you want to leave?";
            });
        } else {
            $(window).off('beforeunload');
        }
    }

    base.onCreateSuccess = function (data) {
        if (data.code === 200) {
            base.onLiveWarning(false);
            costJsBase.EventNotify('success', data.message || 'Lưu dữ liệu thành công!');
            setTimeout(function () {
                window.location.href = '/ProfitPlan/List?stats=0';
            }, 2000);
            costJsBase.ButtonState({
                state: 'normal',
                html: ' <i class="fas fa-check mr-2"></i> Đã lưu',
                disabled: true,
                target: base.$btnCreate
            });
        } else {
            costJsBase.EventNotify('error', data.message || 'Lưu dữ liệu không thành công!');
            costJsBase.ButtonState({
                state: 'normal',
                html: ' <i class="fas fa-save mr-2"></i> Lưu lại',
                target: base.$btnCreate
            });
        }
    }

    base.onCreateError = function (error) {
        base.onLiveWarning(true);
        costJsBase.EventNotify('error', 'Có lỗi xảy ra, vui lòng thử lại sau!');
        costJsBase.ButtonState({
            state: 'normal',
            html: ' <i class="fas fa-save mr-2"></i> Lưu lại',
            target: base.$btnCreate
        });
    }
}


$(document).ready(function () {
    let c = new RevenuePlan();
    c.setup();
});