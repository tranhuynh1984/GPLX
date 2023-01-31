var CashFollowCreate = function () {
    let base = this;
    base.$tableDataTable = null;
    base.$tableDataTableAggregate = null;

    base.$btnSelectExcelFile = $('#btnSelectExcelFile');
    base.$inputFileCostEstimateExcelFile = $('#costEstimateExcelFile');
    base.$btnCreate = $('#btnCreate');
    let $tableSelector = $('#createCostEstimateUploadTable');
    let $tableAggregateSelector = $('#createAggregates');

    base.$labelFileName = $('#labelFileSelect');
    base.$btnStartUpload = $('#btnStartUpload');
    base.$errorCollapse = $('#errorCollapse');
    base.$boxExcelErrors = $('#boxExcelErrors');
    base.$inputSearchOutBox = $('#inputSearchOutBox');
    base.$recordID = $('#__recordCostEstimate');
    base.inputTableSearch = $('#__inputTableSearch');
    base.btnTableSearch = $('#__btnTableSearch');
    base.selectYear = $('#selectYear');

    base.recordSelector = null;
    base.recordSelectorIndex = -1;

    base.excelExts = [".xlsx"];
    base.actOnUpload = function () { }


    base.columnTableAggregates = [
        {
            idx: 0,
            render: function (data, type, row, meta) {
                return row.no;
            },
            width: "5%",
            class: 'align-middle text-center',
            fieldName: 'no',
            data: "no"
        },
        // Nội dung
        {
            idx: 1,
            render: function (data, type, row, meta) {
                return '<span class="' + row.class + '">' + row.cashFollowGroupName + '</span>';

            },
            width: "auto",
            class: 'align-middle text-left',
            fieldName: 'cashFollowGroupName',
            data: "cashFollowGroupName"
        },
        // T1
        {
            idx: 2,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.q1, '-');
            },
            width: "10%",
            class: 'text-right align-middle',
            fieldName: 'q1',
            data: 'q1'
        },
        // T2
        {
            idx: 3,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.q2, '-');
            },
            class: "text-right align-middle",
            width: "10%",
            fieldName: 'q2',
            data: 'q2'

        },
        // T3
        {
            idx: 4,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.q3, '-');
            },
            width: "10%",
            class: "align-middle text-right",
            fieldName: 'q3',
            data: 'q3'

        },
        // T4
        {
            idx: 5,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.q4, '-');
            },
            class: "text-right align-middle",
            width: "10%",
            fieldName: 'q4',
            data: 'q4'
        },
        {
            idx: 5,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.total, '-');
            },
            class: "text-right align-middle",
            width: "10%",
            fieldName: 'total',
            data: 'total'
        }
    ];

    base.columnTableConfigs = [
        {
            idx: 0,
            render: function (data, type, row, meta) {
                return row.no;
            },
            width: "auto",
            class: 'align-middle text-center',
            fieldName: 'no',
            data: "no"
        },
        // Nội dung
        {
            idx: 1,
            render: function (data, type, row, meta) {
                return row.style === 'bold' ? '<b>' + row.cashFollowGroupName + '</b>' : row.cashFollowGroupName;
            },
            width: "auto",
            class: 'align-middle text-left',
            fieldName: 'cashFollowGroupName',
            data: "cashFollowGroupName"
        },
        // T1
        {
            idx: 2,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.m1, '-');
            },
            width: "auto",
            class: 'text-right align-middle',
            fieldName: 'm1',
            data: 'm1'
        },
        // T2
        {
            idx: 3,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.m2, '-');
            },
            class: "text-right align-middle",
            width: "auto",
            fieldName: 'm2',
            data: 'm2'

        },
        // T3
        {
            idx: 4,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.m3, '-');
            },
            width: "auto",
            class: "align-middle text-right",
            fieldName: 'm3',
            data: 'm3'

        },
        // T4
        {
            idx: 5,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.m4, '-');
            },
            class: "text-right align-middle",
            width: "auto",
            fieldName: 'm4',
            data: 'm4'

        },
        // T5
        {
            idx: 6,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.m5, '-');
            },
            class: "text-right align-middle",
            width: "auto",
            fieldName: 'm5',
            data: 'm5'


        },
        // T6
        {
            idx: 7,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.m6, '-');
            },
            class: "text-right align-middle",
            width: "auto",
            fieldName: 'm6',
            data: 'm6'


        },
        // T7
        {
            idx: 8,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.m7, '-');
            },
            class: "text-right align-middle",
            width: "auto",
            fieldName: 'm7',
            data: 'm7'

        },
        // T8
        {
            idx: 9,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.m8, '-');
            },
            class: "text-right align-middle",
            width: "auto",
            fieldName: 'm8',
            data: 'm8'

        },
        // T9
        {
            idx: 10,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.m9, '-');
            },
            class: "text-right align-middle",
            width: "auto",
            fieldName: 'm9',
            data: 'm9'


        },
        // T10
        {
            idx: 11,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.m10, '-');
            },
            class: "text-right align-middle",
            width: "auto",
            fieldName: 'm10',
            data: 'm10'
        },
        // T11
        {
            idx: 12,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.m11, '-');
            },
            class: "text-right align-middle",
            width: "auto",
            fieldName: 'm11',
            data: 'm11'
        },
        // T12
        {
            idx: 13,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.m12, '-');
            },
            class: "text-right align-middle",
            width: "auto",
            fieldName: 'm12',
            data: 'm12'
        },
        // Total
        {
            idx: 14,
            render: function (data, type, row, meta) {
                return '<b>' + costJsBase.FormatMoney(row.total, '-') + '</b>';
            },
            class: "text-right align-middle",
            width: "auto",
            fieldName: 'total',
            data: 'total'
        }
    ];

    base.cashFollowData = null;

    base.fixedColumns = {
        left: 2
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
        let dataAggr = null;
        if (typeof (dataSrc) !== 'undefined' && dataSrc != null) {
            data = dataSrc.cashFollowItemExcels;
            dataAggr = dataSrc.cashFollowAggregateExcels;
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
            paging: false,
            autoWidth: true,
            fixedHeader: true,
            serverSide: false,
            fixedColumns: base.fixedColumns,
            data: data,
            iDisplayLength: 50,
            initComplete: function () {
                $.fn.dataTable.ext.search.push(
                    function (settings, searchData, index, rowData, counter) {
                        var searchKeys = base.inputTableSearch.val();
                        if (!searchKeys.length)
                            return true;
                        else {
                            let valid = true;
                            if (searchKeys.length) {
                                valid = rowData.content.toLowerCase().indexOf(searchKeys.toLowerCase()) != -1;
                            }
                            return valid;
                        }
                    }
                );
            }
        });

        base.$tableDataTableAggregate = $.fn.jsTableRegister({
            selector: $tableAggregateSelector,
            columns: base.columnTableAggregates,
            drawCallback: function () {
            },
            searching: false,
            scrollX: true,
            responsive: false,
            select: true,
            paging: false,
            autoWidth: true,
            fixedHeader: true,
            serverSide: false,
            fixedColumns: base.fixedColumns,
            data: dataAggr,
            iDisplayLength: 50,
        });
    }

    // cấu hình các chuỗi string cố định
    base.$formLang = languages.vi.cashFollow.create;

    // gắn sự kiện cho các nút
    base.bindActions = function () {
        base.$btnSelectExcelFile.bind('click', function () {
            base.excelExts = ['.xlsx'];
            base.$inputFileCostEstimateExcelFile.attr('accept', '.xlsx').click();;
            base.actOnUpload = function () {
                costJsBase.JsUploadFile({
                    selector: base.$inputFileCostEstimateExcelFile,
                    Url: '/CashFollow/OnExcelUpload?year=' + base.selectYear.val(),
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
                        base.clearAll(base.$inputFileCostEstimateExcelFile);
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

    base.clearAll = function (selector) {
        base.clearInput(selector);
        base.$tableDataTable.clear().draw();
        base.$tableDataTableAggregate.clear().draw();
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
                costJsBase.EventNotify('error', data.message);
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
                        if (!data.specifyFieldValue.cashFollowItemExcels.length ||
                            !data.specifyFieldValue.cashFollowAggregateExcels.length) {
                            costJsBase.EventNotify('warning', 'Không có dữ liệu trong danh sách đã nhập!');
                        } else {
                            base.cashFollowData = data.specifyFieldValue;
                            base.$tableDataTable.rows.add(data.specifyFieldValue.cashFollowItemExcels).draw();
                            base.$tableDataTableAggregate.rows.add(data.specifyFieldValue.cashFollowAggregateExcels).draw();
                            base.onLiveWarning(true);
                            costJsBase.EventNotify('success', 'Nhập dữ liệu thành công!');
                        }
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
        });
        base.bindActions();
        base.inputTableSearch.keyup(function () {
            base.$tableDataTable.draw();
        });
    }

    base.intVal = function (i) {
        return typeof i === 'string' ?
            i.replace(/[\$,]/g, '') * 1 :
            typeof i === 'number' ?
                i : 0;
    }

    base.onCreate = function () {
        let year = base.intVal(base.selectYear.val());
        if (year === -100) {
            costJsBase.EventNotify('warning', 'Bạn chưa chọn năm tài chính!');
            return false;
        }


        if (base.cashFollowData != null && base.cashFollowData?.cashFollow != null) {
            if (base.cashFollowData.cashFollow.year !== year) {
                costJsBase.EventNotify('warning', 'Năm tài chính không khớp với sheet dữ liệu đã tải lên!');
                return false;
            }
        } else {
            costJsBase.EventNotify('warning', 'Không có dữ liệu cần lưu lại!');
            return false;
        }

        let data = base.$tableDataTable.rows().data();
        let isValidData = true;
        if (!data.length) {
            costJsBase.EventNotify('warning', 'Không tìm thấy dữ liệu cần lưu!');
            return false;
        }

        if (isValidData) {
            costJsBase.Post({
                Url: '/CashFollow/OnCreate',
                Data: base.cashFollowData,
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
                window.location.href = "/CashFollow/List?stats=0";
            }, 1500);
            costJsBase.ButtonState({
                state: 'normal',
                html: ' <i class="fas fa-check mr-2"></i> Đã lưu',
                disabled: false,
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
            html: ' <i class="fas fa-plus mr-2"></i> Lưu lại',
            target: base.$btnCreate
        });
    }

    base.sorting = function (a, b) {
        return ((a.row < b.row) ? -1 : ((a.row > b.row) ? 1 : 0));
    }
}


$(document).ready(function () {
    let c = new CashFollowCreate();
    typeof (partial) != 'undefined' && partial ?
        setTimeout(c.setup, 500) : c.setup();

    $(document).on('click', '[prop-type="trash"]', function () {
        if (confirm('Bạn có chắc chắn muốn xóa thực chi này ?')) {
            c.onDeleteRow(this);
        }
    });
});