var AcutallyCreate = function () {
    let base = this;
    base.$tableDataTable = null;
    base.$btnSelectExcelFile = $('#btnSelectExcelFile');
    base.$inputFileCostEstimateExcelFile = $('#costEstimateExcelFile');
    base.$btnCreate = $('#btnCreate');
    let $tableSelector = $('#createCostEstimateUploadTable');

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

    base.recordSelector = null;
    base.recordSelectorIndex = -1;

    base.excelExts = [".xlsx"]
    base.actOnUpload = function () { }

    base.columnTableConfigs = [
        // TT
        {
            idx: 0,
            render: function (data, type, row, meta) {
                return (typeof meta.row === 'number') ? meta.row + meta.settings._iDisplayStart + 1 : meta.row[0] + 1;
            },
            width: "5%",
            class: "text-center",
            fieldName: '#',
            data: null,
        },
        // Mã dự trù
        {
            idx: 1,
            render: function (data, type, row, meta) {
                return row.requestCode;
            },
            width: "5%",
            class: 'align-middle',
            fieldName: 'requestCode',
            data: "requestCode"
        },
        {
            idx: 2,
            render: function (data, type, row, meta) {
                return row.requestContent;
            },
            width: "5%",
            class: 'align-middle',
            fieldName: 'requestContent',
            data: "requestContent"
        },
        // nội dung
        {
            idx: 3,
            render: function (data, type, row, meta) {
                return row.costEstimateItemTypeName;
            },
            width: "auto",
            class: 'align-middle',
            fieldName: 'costEstimateItemTypeName',
            data: 'costEstimateItemTypeName'
        },
        // Tổng số tiền
        {
            idx: 4,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.cost);
            },
            class: "text-right align-middle",
            width: "6%",
            fieldName: 'cost',
            data: 'cost'

        },
        // Tuần dự trù
        {
            idx: 5,
            render: function (data, type, row, meta) {
                return row.requestPayWeekName;
            },
            width: "8%",
            class: "align-middle",
            fieldName: 'requestPayWeekName',
            data: 'requestPayWeekName'

        },
        // thực chi
        {
            idx: 6,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.actualSpent);
            },
            class: "text-right align-middle",
            width: "6%",
            fieldName: 'actualSpent',
            data: 'actualSpent'

        },
        // còn lại
        {
            idx: 7,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.amountLeft);
            },
            class: "text-right align-middle",
            width: "8%",
            fieldName: 'amountLeft',
            data: 'amountLeft'


        },
        // Tổng chi hiện tại
        {
            idx: 8,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.actualSpentAtTime)
            },
            class: "text-right align-middle",
            width: "10%",
            fieldName: 'actualSpentAtTime',
            data: 'actualSpentAtTime'


        },
        // Thời gian thực chi
        {
            idx: 9,
            render: function (data, type, row, meta) {
                return row.actualPayWeekName;
            },
            class: "text-center align-middle",
            width: "6%",
            fieldName: 'actualPayWeekName',
            data: 'actualPayWeekName'

        },
        // số chứng từ
        {
            idx: 10,
            render: function (data, type, row, meta) {
                return row.accountantCode;
            },
            class: "text-center align-middle",
            width: "6%",
            fieldName: 'accountantCode',
            data: 'accountantCode'

        },
        // giải trình
        {
            idx: 11,
            render: function (data, type, row, meta) {
                return row.explanation;
            },
            class: "text-center align-middle",
            width: "15%",
            fieldName: 'explanation',
            data: 'explanation'

        }
        //},
        //// xóa
        //{
        //    idx: 12,
        //    render: function (data, type, row, meta) {
        //        return `<a class="text-danger" prop-type="trash" href="javascript: void(0);"><i class="far fa-times-circle"></i></a>`;
        //    },
        //    class: "text-center align-middle",
        //    width: "15%",
        //    fieldName: 'Action',
        //    data: null,
        //}
    ]

    base.totals = {
        totalActualSpentAtTime: 0,
        totalAmountLeft: 0,
        totalActualSpent: 0,
        totalCost: 0
    };

    base.sctData = null;

    base.fixedColumns = {
        left: 2,
        right: 1
    };

    base.onLoad = function () {
        var deferred = $.Deferred();
        let columnsEnable = [];
        $('#createCostEstimateUploadTable').find('thead tr th').each(function () {
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
    // khởi tạo datatable
    base.setupDataTable = function (cfgs) {
        let data = null;
        let dataSelectOptions = JSON.parse(dataSelects);
        let options = [];
        if (typeof (dataSrc) !== 'undefined' && dataSrc != null) {
            data = JSON.parse(dataSrc);
        }

        for (var i = 0; i < dataSelectOptions.length; i++) {
            options.push({ label: dataSelectOptions[i].name, value: dataSelectOptions[i].name });
        }

        if ((typeof (partial) == 'undefined' || (typeof (partial) == 'undefined' && !partial)) && base.$btnCreate.length) {
            let editorCfgs = [
                {
                    name: 'requestContent'
                },
                { name: 'amountLeft' },
                { name: 'explanation' },
                { name: 'costEstimateItemTypeName', type: 'select', options: options }];
            let editor = new $.fn.dataTable.Editor({
                table: '#createCostEstimateUploadTable',
                fields: editorCfgs,
                idSrc: 'record'
            });

            $('#createCostEstimateUploadTable').on('click', 'tbody td:not(:first-child)', function (e) {
                editor.inline(this);
            });

        }

        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            columns: cfgs,
            drawCallback: function () {
                base.$btnCreate.prop('disabled', base.$tableDataTable == null ? true : base.$tableDataTable.rows().data().length === 0);
                base.$tableDataTable == null || base.$tableDataTable.rows().data().length === 0 ? $('tfoot tr').hide() : $('tfoot tr').show();
            },
            searching: true,
            scrollX: true,
            responsive: false,
            altEditor: true,
            select: true,
            paging: true,
            autoWidth: true,
            fixedHeader: true,
            serverSide: false,
            fixedColumns: base.fixedColumns,
            data: data,
            iDisplayLength: 25,
            // scroller
            // checkbox search
            initComplete: function () {
                $.fn.dataTable.ext.search.push(
                    function (settings, searchData, index, rowData, counter) {
                        var searchKeys = base.inputTableSearch.val();
                        var typeSearch = base.selectTableSearch.val();

                        if (!searchKeys.length && !typeSearch.length)
                            return true;
                        else {
                            let valid = true;
                            if (searchKeys.length) {
                                valid = rowData.requestContent.toLowerCase().indexOf(searchKeys.toLowerCase()) != -1
                                    || rowData.requestContent.toLowerCase().indexOf(searchKeys.toLowerCase()) != -1
                                    || (rowData.requestCode != null ? rowData.requestCode.toLowerCase().indexOf(searchKeys.toLowerCase()) != -1 : false);
                            }

                            if (typeSearch.length && valid) {
                                valid = rowData.actualSpentType.toLowerCase() === typeSearch.toLowerCase();
                            }
                            return valid;
                        }
                    }
                );
            },


            footerCallback: function (row, data, start, end, display) {
                let api = this.api();

                let cols = ['cost', 'actualSpent', 'amountLeft', 'actualSpentAtTime'];
                let intVal = function (i) {
                    return typeof i === 'string' ?
                        i.replace(/[\$,]/g, '') * 1 :
                        typeof i === 'number' ?
                            i : 0;
                };
                cols.forEach(function (v) {
                    let fieldSettings = base.columnTableConfigs.filter(x => {
                        return x.fieldName === v;
                    });

                    let col = fieldSettings.length ? fieldSettings[0].idx : -1;

                    let calculate = api
                        .column(col, { page: 'current' })
                        .data()
                        .reduce(function (a, b) {
                            let keyVal = 0;
                            if (typeof (b) === 'object') {
                                let valOf = mapField.filter(m => {
                                    return m[0] === v;
                                });

                                keyVal = valOf.length ? valOf[0][1] : 0;
                            } else {
                                keyVal = b;
                            }
                            return intVal(a) + intVal(keyVal);
                        }, 0);

                    let valOf = 0;

                    if (typeof (dataSrc) !== 'undefined') {
                        valOf = api
                            .column(col)
                            .data()
                            .reduce(function (a, b) {
                                let keyVal = 0;
                                if (typeof (b) === 'object') {
                                    let valOf = mapField.filter(m => {
                                        return m[0] === v;
                                    });

                                    keyVal = valOf.length ? valOf[0][1] : 0;
                                } else {
                                    keyVal = b;
                                }
                                return intVal(a) + intVal(keyVal);
                            }, 0);
                    } else {
                        switch (v) {
                            case 'cost':
                                valOf = base.totals.totalCost;
                                break;
                            case 'actualSpent':
                                valOf = base.totals.totalActualSpent;
                                break;
                            case 'amountLeft':
                                valOf = base.totals.totalAmountLeft;
                                break;
                            case 'actualSpentAtTime':
                                valOf = base.totals.totalActualSpentAtTime;
                                break;
                            default:
                                break
                        }
                    }

                    $(api.column(col).footer()).html(
                        costJsBase.FormatMoney(calculate) + '<br />(Tổng: ' + costJsBase.FormatMoney(valOf) + ')'
                    );
                });

                setTimeout(function () {
                    base.$tableDataTable.columns.adjust();
                }, 350);
            }
        });

    }

    // cấu hình các chuỗi string cố định
    base.$formLang = languages.vi.costElement.create;

    // gắn sự kiện cho các nút
    base.bindActions = function () {
        base.$btnSelectExcelFile.bind('click', function () {
            base.excelExts = ['.xlsx'];
            base.$inputFileCostEstimateExcelFile.attr('accept', '.xlsx').click();;
            base.actOnUpload = function () {
                costJsBase.JsUploadFile({
                    selector: base.$inputFileCostEstimateExcelFile,
                    //Url: '/ActuallySpent/OnExcelUpload',
                    Url: '/ActuallySpent/OnSCTDataUpload',
                    async: true,
                    beforeSend: function () {
                        base.$btnStartUpload.html(base.$formLang.pendingUpload);
                        base.showAndClearErrorBox();
                    },
                    complete: function () {
                        base.$btnStartUpload.html(base.$formLang.oldButtonUpload);
                    }
                }, base.onExcelUpload,
                    function (err) {
                        base.$btnStartUpload.html(base.$formLang.oldButtonUpload);
                        costJsBase.EventNotify('error', 'Tải lên không thành công, vui lòng thử lại sau!')
                    }
                )
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
            let files = base.$inputFileCostEstimateExcelFile.get(0).files;
            if (files.length === 0) {
                return false;
            } else {
                base.actOnUpload();
            }
        });

        base.$btnCreate.bind('click', base.onCreate);

        base.$selectReportForWeek.select2({
            theme: 'bootstrap4'
        }).change(function () {
            let week = base.parserInt(base.$selectReportForWeek.val());
            week != -1 ? base.$lblWeekActuallySpent.text('Tổng hợp các khoản dự trù đã duyệt chưa được báo cáo thực chi tính đến hết tuần: ' + (week - 1)).show()
                : base.$lblWeekActuallySpent.hide();
            week != -1 ? base.$weekLable.text(week - 1)
                : base.$weekLable.text('--');
            base.$weekLable
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
    //sự kiện khi tải lên file excel hoàn thành
    base.onExcelUpload = function (data) {
        switch (data.code) {
            case costJsBase.enums.noContentCode:
                let msg = typeof (data.errors) === 'object' ? data.errors[0].message : 'Tệp tải lên không đúng định dạng!';
                costJsBase.EventNotify('error', msg);
                break
            case costJsBase.enums.errorCode:
                costJsBase.EventNotify('error', data.message);
                if (typeof data.excelFileError != 'undefined' && data.excelFileError?.length) {
                    base.showAndClearErrorBox(true);
                    base.$errorCollapse.append('Tệp chi tiết mã lỗi: <a href="' + data.excelFileError + '" target="_blank">Tải xuống</a>');
                }
                break
            case costJsBase.enums.successCode:
                if (data.isValid) {
                    if (!data.specifyFieldValue.data.length) {
                        costJsBase.EventNotify('warning', 'Không có dữ liệu trong danh sách đã nhập!');
                    } else {
                        base.$tableDataTable.clear();
                        base.sctData = [];

                        base.totals.totalActualSpentAtTime += data.specifyFieldValue.totalActualSpentAtTime;
                        base.totals.totalAmountLeft += data.specifyFieldValue.totalAmountLeft;
                        base.totals.totalActualSpent += data.specifyFieldValue.totalActualSpent;
                        base.totals.totalCost += data.specifyFieldValue.totalCost;

                        if (base.sctData == null)
                            base.sctData = [];

                        for (var i = 0; i < data.data.length; i++) {
                            base.sctData.push(data.data[i]);
                        }
                        base.$tableDataTable.rows.add(data.specifyFieldValue.data).draw();

                        //todo
                        base.disableOnDoneUpload(true);
                        base.onLiveWarning(true);
                        base.hideUpploadFile(true);
                        base.clearInput(base.$inputFileCostEstimateExcelFile);
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
                break
            default:
                costJsBase.EventNotify('error', 'Có lỗi xảy ra, tải lên không thành công!');
                break
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

            let anyActionCol = c.filter(x => {
                return x.fieldName === 'Action';
            });

            if (!anyActionCol.length) {
                base.fixedColumns = {
                    left: 2
                };
            }

            base.setupDataTable(c);
            base.$btnCreate.prop('disabled', base.$tableDataTable.rows().data().length === 0);
            base.$tableDataTable == null || base.$tableDataTable.rows().data().length === 0 ? $('tfoot tr').hide() : $('tfoot tr').show();
        })
        base.bindActions();
        $('[data-toggle="tooltip"]').tooltip()

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
        let data = base.$tableDataTable.rows().data();
        let dataCr = [];
        let isValidData = true;
        let week = base.parserInt(base.$selectReportForWeek.val(), -1);
        if (week < 1) {
            costJsBase.EventNotify('warning', 'Vui lòng chọn tuần lập báo cáo');
            return false;
        }

        for (var i = 0; i < data.length; i++) {
            let dataAt = data[i];
            dataCr.push(dataAt);
        }

        if (isValidData) {
            var dataPost = {
                Record: costJsBase.ValueFromUrl('record') || base.$recordID.val(),
                Data: dataCr,
                ReportForWeek: week,
                ReportForWeekName: 'Tuần ' + week,
                SctData: base.sctData
            };

            costJsBase.Post({
                Url: '/ActuallySpent/OnCreate',
                Data: dataPost,
                async: true,
                beforeSend: function () {
                    costJsBase.ButtonState({
                        target: base.$btnCreate,
                        state: 'loading',
                        disabled: true,
                        text: 'Đang lưu lại ...',
                    });
                }
            }, base.onCreateSuccess, base.onCreateError);
        }
    }

    //todo: set for another action
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
            let url = URI(window.location.href);
            setTimeout(function () {
                window.location.href = '/ActuallySpent/List'
                //jQuery.fn.buildParamURI(url, 'record', data.record);
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
                html: ' <i class="fas fa-plus mr-2"></i> Lưu lại',
                target: base.$btnCreate
            });
        }
    }

    base.onCreateError = function (error) {
        base.onLiveWarning(true);
        costJsBase.EventNotify('error', 'Có lỗi xảy ra, vui lòng thử lại sau!');
        costJsBase.ButtonState({
            state: 'normal',
            html: ' <i class="fas fa-plus mr-2"></i> Lưu lại',
            target: base.$btnCreate
        });
    }

    base.onDeleteRow = function (selector) {
        let point = $(selector).closest('tr');
        base.$tableDataTable.row(point).remove().draw();
        base.onLiveWarning(true);
    }

    //todo:
    // pharse 2
    base.autoSavingClientOnChange = function () {

    }
}


$(document).ready(function () {
    let c = new AcutallyCreate();
    // trường hợp mở parital thì phải để delay
    // tránh trường hợp bị lệch header & body
    typeof (partial) != 'undefined' && partial ?
        setTimeout(c.setup, 500) : c.setup();

    $(document).on('click', '[prop-type="trash"]', function () {
        if (confirm('Bạn có chắc chắn muốn xóa thực chi này ?')) {
            c.onDeleteRow(this);
        }
    });
});