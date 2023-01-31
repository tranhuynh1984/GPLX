var InvestmentPlan = function () {
    let base = this;
    base.$tableDataTable = null;
    base.$tableDetailDataTable = null;
    base.$btnSelectExcelFile = $('#btnSelectExcelFile');
    base.$inputFileCostEstimateExcelFile = $('#costEstimateExcelFile');
    base.$btnCreate = $('#btnCreate');
    let $tableSelector = $('#createCostEstimateUploadTable');
    let $tableDetailSelector = $('#createInvestExcel');

    base.$labelFileName = $('#labelFileSelect');
    base.$btnStartUpload = $('#btnStartUpload');
    base.$errorCollapse = $('#errorCollapse');
    base.$boxExcelErrors = $('#boxExcelErrors');
    base.$inputSearchOutBox = $('#inputSearchOutBox');
    base.$recordID = $('#__recordCostEstimate');
    base.$checkClearTable = $('#checkClearTable');
    base.$selectReportForWeek = $('#selectReportForWeek');
    base.$lblWeekActuallySpent = $('#lblWeekActuallySpent');
    base.$weekLable = $('#weekLable');
    base.selectTableSearch = $('#__selectTableSearch');
    base.inputTableSearch = $('#__inputTableSearch');
    base.btnTableSearch = $('#__btnTableSearch');
    base.selectYear = $('#selectYear');

    base.recordSelector = null;
    base.recordSelectorIndex = -1;

    base.excelExts = [".xlsx"];
    base.actOnUpload = function () { }

    base.columnTableConfigs = [
        // TT
        {
            idx: 0,
            render: function (data, type, row, meta) {
                return (typeof meta.row === 'number') ? meta.row + meta.settings._iDisplayStart + 1 : meta.row[0] + 1;
            },
            width: "5%",
            class: "text-center align-middle",
            fieldName: '#',
            data: null
        },
        // Nội dung
        {
            idx: 1,
            render: function (data, type, row, meta) {
                return row.investmentPlanContentName;
            },
            width: "auto",
            class: 'align-middle',
            fieldName: 'investmentPlanContentName',
            data: "investmentPlanContentName"
        },
        // số tiền đầu tư dự kiến
        {
            idx: 2,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.expectCostInvestment, '-');
            },
            width: "15%",
            class: 'align-middle text-center',
            fieldName: 'expectCostInvestment',
            data: "expectCostInvestment"
        },
        // vốn tự có
        {
            idx: 3,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.expenditureCapital, '-');
            },
            width: "15%",
            class: 'align-middle text-center',
            fieldName: 'expenditureCapital',
            data: 'expenditureCapital'
        },
        // MG
        {
            idx: 4,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.capitalMedGroup, '-');
            },
            class: "text-center align-middle",
            width: "15%",
            fieldName: 'capitalMedGroup',
            data: 'capitalMedGroup'

        },
        // Vốn vay
        {
            idx: 5,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.spendingLoan, '-');
            },
            width: "15%",
            class: "align-middle text-center",
            fieldName: 'spendingLoan',
            data: 'spendingLoan'

        }
    ];

    base.columnTableDetailConfigs = [
        // TT
        {
            idx: 0,
            render: function (data, type, row, meta) {
                return (typeof meta.row === 'number') ? meta.row + meta.settings._iDisplayStart + 1 : meta.row[0] + 1;
            },
            width: "5%",
            class: "text-center align-middle",
            fieldName: '#',
            data: null
        },
        // Nội dung
        {
            idx: 1,
            render: function (data, type, row, meta) {
                return row.investContent;
            },
            width: "auto",
            class: 'align-middle',
            fieldName: 'investContent',
            data: "investContent"
        },
        // hạng mục
        {
            idx: 2,
            render: function (data, type, row, meta) {
                return row.investmentPlanContentName;
            },
            width: "15%",
            class: 'align-middle text-center',
            fieldName: 'investmentPlanContentName',
            data: "investmentPlanContentName"
        },
        // phòng ban
        {
            idx: 3,
            render: function (data, type, row, meta) {
                return row.departmentName;
            },
            width: "15%",
            class: 'align-middle text-center',
            fieldName: 'departmentName',
            data: 'departmentName'
        },
        //Thời điểm đầu tư
        {
            idx: 4,
            render: function (data, type, row, meta) {
                return moment(row.expectedTime).format('DD/MM/YYYY');
            },
            class: "text-center align-middle",
            width: "15%",
            fieldName: 'expectedTime',
            data: 'expectedTime'

        },
        {
            idx: 5,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.costExpected, '-');
            },
            class: "text-center align-middle",
            width: "15%",
            fieldName: 'costExpected',
            data: 'costExpected'

        },
        //
        {
            idx: 6,
            render: function (data, type, row, meta) {
                return row.capitalPlanName;
            },
            width: "15%",
            class: "align-middle text-center",
            fieldName: 'capitalPlanName',
            data: 'capitalPlanName'
        },
        {
            idx: 7,
            render: function (data, type, row, meta) {
                if (row.spendingLoanPercent === -1)
                    return '-';
                return row.spendingLoanPercent + "%";
            },
            width: "15%",
            class: "align-middle text-center",
            fieldName: 'spendingLoanPercent',
            data: 'spendingLoanPercent'
        },
        {
            idx: 8,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.spendingLoan, '-');
            },
            width: "15%",
            class: "align-middle text-center",
            fieldName: 'spendingLoan',
            data: 'spendingLoan'
        },
        {
            idx: 9,
            render: function (data, type, row, meta) {
                return row.explanation;
            },
            width: "15%",
            class: "align-middle text-center",
            fieldName: 'explanation',
            data: 'explanation'
        }
    ];

    base.investData = null;

    base.onLoad = function () {
        var deferred = $.Deferred();
        let columnsEnable = [];
        $('#createCostEstimateUploadTable').find('thead tr th').each(function () {
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
                base.$tableDataTable == null || base.$tableDataTable.rows().data().length === 0 ? $('tfoot tr').hide() : $('tfoot tr').show();
            },
            searching: false,
            scrollX: true,
            responsive: false,
            altEditor: true,
            select: true,
            paging: false,
            autoWidth: true,
            fixedHeader: false,
            serverSide: false,
            footerCallback: function (row, data, start, end, display) {
                let api = this.api();
                let cols = ['expectCostInvestment', 'expenditureCapital', 'spendingLoan'];
                let medVisible = cfgs.filter(x => {
                    return x.fieldName === 'capitalMedGroup';
                });

                if (medVisible.length)
                    cols.push('capitalMedGroup');

                let intVal = function (i) {
                    return typeof i === 'string' ?
                        i.replace(/[\$,]/g, '') * 1 :
                        typeof i === 'number' ?
                            i : 0;
                };
                cols.forEach(function (v) {
                    let fieldSettings = cfgs.filter(x => {
                        return x.fieldName === v;
                    });

                    let col = fieldSettings.length ? fieldSettings[0].idx : -1;
                    if (v === 'spendingLoan' && !medVisible.length)
                        col = col - 1;

                    if (col >= 0) {
                        let valOf = 0;
                        valOf = api
                            .column(col)
                            .data()
                            .reduce(function (a, b) {
                                let keyVal = 0;
                                if (typeof (b) === 'object' && b !== null) {
                                    let valOf = mapField.filter(m => {
                                        return m[0] === v;
                                    });

                                    keyVal = valOf.length ? valOf[0][1] : 0;
                                } else {
                                    keyVal = b;
                                }
                                return intVal(a) + intVal(keyVal);
                            }, 0);
                        $(api.column(col).footer()).html(
                            costJsBase.FormatMoney(valOf, '-')
                        );
                    }

                });

                setTimeout(function () {
                    base.$tableDataTable.columns.adjust();
                }, 350);
            }
        });

        base.$tableDetailDataTable = $.fn.jsTableRegister({
            selector: $tableDetailSelector,
            columns: base.columnTableDetailConfigs,
            drawCallback: function () { },
            searching: false,
            scrollX: true,
            responsive: false,
            altEditor: true,
            select: true,
            paging: false,
            autoWidth: true,
            fixedHeader: false,
            serverSide: false,
            footerCallback: function (row, data, start, end, display) {
                let api = this.api();
                let cols = ['costExpected', 'spendingLoan'];


                let intVal = function (i) {
                    return typeof i === 'string' ?
                        i.replace(/[\$,]/g, '') * 1 :
                        typeof i === 'number' ?
                            i : 0;
                };
                cols.forEach(function (v) {
                    let fieldSettings = base.columnTableDetailConfigs.filter(x => {
                        return x.fieldName === v;
                    });

                    let col = fieldSettings.length ? fieldSettings[0].idx : -1;
                    if (col >= 0) {
                        let valOf = 0;
                        valOf = api
                            .column(col)
                            .data()
                            .reduce(function (a, b) {
                                let keyVal = 0;
                                if (typeof (b) === 'object' && b !== null) {
                                    let valOf = mapField.filter(m => {
                                        return m[0] === v;
                                    });

                                    keyVal = valOf.length ? valOf[0][1] : 0;
                                } else {
                                    keyVal = b;
                                }
                                return intVal(a) + intVal(keyVal);
                            }, 0);
                        $(api.column(col).footer()).html(
                            costJsBase.FormatMoney(valOf, '-')
                        );
                    }

                });

                setTimeout(function () {
                    base.$tableDetailDataTable.columns.adjust();
                }, 350);
            }
        });

    }

    // cấu hình các chuỗi string cố định
    base.$formLang = languages.vi.costElement.create;

    base.intVal = function (i) {
        return typeof i === 'string' ?
            i.replace(/[\$,]/g, '') * 1 :
            typeof i === 'number' ?
                i : 0;
    }

    // gắn sự kiện cho các nút
    base.bindActions = function () {
        base.$btnSelectExcelFile.bind('click', function () {
            base.excelExts = ['.xlsx'];
            base.$inputFileCostEstimateExcelFile.attr('accept', '.xlsx').click();;
            base.actOnUpload = function () {
                costJsBase.JsUploadFile({
                    selector: base.$inputFileCostEstimateExcelFile,
                    Url: '/InvestmentPlan/OnExcelUpload?year=' + base.selectYear.val(),
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
                        base.$btnStartUpload.html(base.$formLang.oldButtonUpload);
                        costJsBase.EventNotify('error', 'Tải lên không thành công, vui lòng thử lại sau!');
                    }
                );
            };
        });

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
    base.clearAll = function (selector) {
        base.clearInput(selector);
        base.$tableDataTable.clear().draw();
        base.$tableDetailDataTable.clear().draw();
        base.hideUpploadFile(true);
        base.disableOnDoneUpload(true);
        base.$btnStartUpload.html(base.$formLang.oldButtonUpload);
        base.showAndClearErrorBox(false);
    }
    //sự kiện khi tải lên file excel hoàn thành
    base.onExcelUpload = function (data) {
        base.clearAll(base.$inputFileCostEstimateExcelFile);

        switch (data.code) {
            case costJsBase.enums.noContentCode:
                costJsBase.EventNotify('error', data.errors[0].message);
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
                    if (data.specifyFieldValue != null) {
                        //if (!data.specifyFieldValue.investmentPlanDetails.length ||
                        //    !data.specifyFieldValue.investmentPlanAggregates.length) {
                        //    costJsBase.EventNotify('warning', 'Không có dữ liệu trong danh sách đã nhập!');
                        //} else {
                            base.investData = data.specifyFieldValue;
                            base.$tableDetailDataTable.rows.add(data.specifyFieldValue.investmentPlanDetails).draw();
                            base.$tableDataTable.rows.add(data.specifyFieldValue.investmentPlanAggregates).draw();
                            base.onLiveWarning(true);
                            costJsBase.EventNotify('success', 'Nhập dữ liệu thành công!');
                        //}
                    } else {
                        costJsBase.EventNotify('warning', 'Không có dữ liệu trong danh sách đã nhập!');
                    }
                } else {
                    base.showAndClearErrorBox(true);
                    for (var i = 0; i < data.errors.length; i++) {
                        let fullErr = data.errors[i].column !== null ? '<b>' + data.errors[i].column + '</b>: ' + data.errors[i].message : data.errors[i].message;
                        base.$errorCollapse.append(fullErr);
                        base.$errorCollapse.append('<br />');
                    }
                    costJsBase.EventNotify('warning', 'Dữ liệu không đúng định dạng, vui lòng xem chi tiết mã lỗi phía dưới!');
                }
                break;
            default:
                costJsBase.EventNotify('error', data.message);
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
            base.$tableDataTable == null || base.$tableDataTable.rows().data().length === 0
                ? $('tfoot tr').hide()
                : $('tfoot tr').show();
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

    base.parserInt = function (i, def) {
        return typeof i === 'string' ?
            i.replace(/[\$,]/g, '') * 1 :
            typeof i === 'number' ?
                i : def;
    };

    base.onCreate = function () {
        let year = base.intVal(base.selectYear.val());
        if (year === -100) {
            costJsBase.EventNotify('warning', 'Bạn chưa chọn năm tài chính!');
            return false;
        }

        if (base.investData != null && base.investData?.investmentPlan != null) {
            if (base.investData.investmentPlan.year !== year) {
                costJsBase.EventNotify('warning', 'Năm tài chính không khớp với sheet dữ liệu đã tải lên!');
                return false;
            }
        } else {
            costJsBase.EventNotify('warning', 'Không có dữ liệu cần lưu lại!');
            return false;
        }

        costJsBase.Post({
            Url: '/InvestmentPlan/OnCreate',
            Data: base.investData,
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
                window.location.href = '/InvestmentPlan/List?stats=0';
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
    let c = new InvestmentPlan();
    c.setup();
});