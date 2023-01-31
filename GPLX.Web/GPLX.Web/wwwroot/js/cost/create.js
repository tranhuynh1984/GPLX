var CostElementCreate = function () {
    let base = this;
    let maxLength = 10;
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
    base.$requestFormRequestType = $('#___requestFormRequestType');
    base.$btnSelectContractFile = $('#btnSelectContractFile');
    base.$recordID = $('#__recordCostEstimate');

    base.recordSelector = null;
    base.recordSelectorIndex = -1;

    base.excelExts = [".xlsx"]
    base.contractFileExts = [ ".png", ".gif", ".jpg"]

    base.actOnUpload = function () { }
    base.$btnApprove = $('#btnApprove');
    base.$checkClearTable = $('#checkClearTable');
    base.fixedColumns = {
        left: 2,
        right: 1
    }
    base.latestCode = -1;
    base.columnTableConfigs = [
        // Ô checkbox
        {
            idx: 0,
            render: function (data, type, row, meta) {
                return `<div class="form-check">
                                  <input class="form-check-input" type="checkbox">                                 
                                </div>`;
            },
            width: "auto",
            class: "text-center"
        },
        // Mã dự trù
        {
            idx: 1,
            render: function (data, type, row, meta) {
                return row.requestCode;
            },
            width: "auto",
            class: 'align-middle'
        },
        // nội dung
        {
            idx: 2,
            render: function (data, type, row, meta) {
                return row.requestContent;
            },
            width: "auto",
            class: 'align-middle'
        },
        // Tổng số tiền
        {
            idx: 3,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.cost);
            },
            class: "text-right align-middle",
            width: "6%"
        },
        // Đơn vị
        {
            idx: 4,
            render: function (data, type, row, meta) {
                return row.unitName;
            },
            width: "6%",
            class: "align-middle"
        },
        // Bộ phận
        {
            idx: 5,
            render: function (data, type, row, meta) {
                return row.departmentName;
            },
            width: "6%",
            class: "align-middle"
        },
        // Người đề xuất
        {
            idx: 6,
            render: function (data, type, row, meta) {
                return row.requesterName;
            },
            class: "text-left align-middle",
            width: "6%"
        },
        // Thời gian đề xuất
        {
            idx: 7,
            render: function (data, type, row, meta) {
                return row.payWeekName;
            },
            class: "text-center align-middle",
            width: "6%"
        },
        // Nhà cung cấp
        {
            idx: 8,
            render: function (data, type, row, meta) {
                return row.supplierName;
            },
            class: "text-center align-middle",
            width: "6%"
        },
        // Số hóa đơn
        {
            idx: 9,
            render: function (data, type, row, meta) {
                return row.billCode;
            },
            class: "text-center align-middle",
            width: "6%"
        },
        // Ngày hóa đơn
        {
            idx: 10,
            render: function (data, type, row, meta) {
                let d = moment(row.billDate, 'YYYY-MM-DDTHH:mm:ss');
                if (d.year() > 2000) {
                    return moment(row.billDate, 'YYYY-MM-DDTHH:mm:ss').format('M/DD/YYYY');
                }
                return "";
            },
            class: "text-center align-middle",
            width: "6%"
        },
        // Giá trị hóa đơn
        {
            idx: 11,
            render: function (data, type, row, meta) {
                return costJsBase.FormatMoney(row.billCost);
            },
            class: "text-right align-middle",
            width: "6%"
        },
        // Hình thức chi
        {
            idx: 12,
            render: function (data, type, row, meta) {
                return row.payForm;
            },
            class: "text-center align-middle",
            width: "6%"
        },
        // Nhóm chi phí
        {
            idx: 13,
            render: function (data, type, row, meta) {
                return row.costEstimateItemTypeName;
            },
            class: "text-center align-middle",
            width: "6%"
        },
        //Nhóm dự trù
        {
            idx: 14,
            render: function (data, type, row, meta) {
                return row.costEstimateGroupName;
            },
            class: "text-center align-middle",
            width: "5%"
        },
        //Ảnh chứng từ
        {
            idx: 15,
            render: function (data, type, row, meta) {
                let className = row.accountImage && row.accountImage.length ? 'd-none' : '';
                return base.$formLang.onTableUploadButton.replace('__CLASS__', className) + base.displayIconByExtension(row.accountImage);
            },
            class: "text-center align-middle",
            width: "7%"
        },
        // Giải trình
        {
            idx: 16,
            render: function (data, type, row, meta) {
                return row.explanation;
            },
            class: "text-left align-middle",
            width: "9%"
        },
        //Người tạo phiếu
        {
            idx: 17,
            render: function (data, type, row, meta) {
                return row.creatorName;
            },
            class: "text-left align-middle",
            width: "5%"
        },
        //Thao tác
        {
            idx: 18,
            render: function (data, type, row, meta) {
                return `<a class="text-danger" prop-type="trash" href="javascript: void(0);"><i class="far fa-times-circle"></i></a>`;
            },
            class: "text-center align-middle",
            width: "5%",
            fieldName: 'Action'
        }
    ]

    // xóa row được chọn
    base.onDeleteRow = function (selector) {
        let point = $(selector).closest('tr');
        base.$tableDataTable.row(point).remove().draw();
        base.onLiveWarning(true);
    }
    // promisse function
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
        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            columns: cfgs,
            drawCallback: function () {
                base.bindTableOnSelectFile();
                base.$btnCreate.prop('disabled', base.$tableDataTable == null ? true : base.$tableDataTable.rows().data().length === 0);
            },
            scrollX: true,
            responsive: false,
            paging: false,
            fixedHeader: true, //todo:https://github.com/DataTables/FixedHeader/tree/master/css | https://datatables.net/extensions/fixedheader/ 
            serverSide: false,
            fixedColumns: base.fixedColumns
        });
    }

    // cấu hình các chuỗi string cố định
    base.$formLang = languages.vi.costElement.create;

    // gắn sự kiện cho các nút
    base.bindActions = function () {
        base.$btnSelectExcelFile.bind('click', function () {

            let fData = new FormData();
            fData.append('v', base.latestCode);

            base.excelExts = ['.xlsx'];
            base.$inputFileCostEstimateExcelFile.attr('accept', '.xlsx').click();;
            base.actOnUpload = function () {
                costJsBase.JsUploadFile({
                    selector: base.$inputFileCostEstimateExcelFile,
                    Url: '/CostEstimate/OnExcelUpload',
                    Data: fData,
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

        base.$btnSelectContractFile.bind('click', function () {
            base.excelExts = ['.zip'];
            base.$inputFileCostEstimateExcelFile.attr('accept', '.zip').click();
            base.actOnUpload = function () {
                costJsBase.JsUploadFile({
                    selector: base.$inputFileCostEstimateExcelFile,
                    Url: '/CostEstimate/OnUpdateContractFile',
                    async: true,
                    beforeSend: function () {
                        base.$btnStartUpload.html(base.$formLang.pendingUpload);
                    },
                    complete: function () {
                        base.$btnStartUpload.html(base.$formLang.oldButtonUpload);
                        base.clearInput(base.$inputFileCostEstimateExcelFile);
                    }
                }, base.onContractFileUpload,
                    function (err) {
                        base.$btnStartUpload.html(base.$formLang.oldButtonUpload);
                        base.clearInput(base.$inputFileCostEstimateExcelFile);
                        costJsBase.EventNotify('error', 'Tải lên không thành công, vui lòng thử lại sau!')
                    }
                )
            }
        });

        base.$btnCreate.bind('click', base.onCreate);

    }
    // validate kích thước file upload
    base.validFileSize = function (size) {
        var kbSize = parseFloat(size);
        var mbSize = parseFloat(kbSize / 1024 / 1024); //==> to MB
        return mbSize > maxLength;
    }
    // xóa dữ liệu input file
    base.clearInput = function (selector) {
        selector.val('');
    }
    //sự kiện khi tải lên file excel hoàn thành
    base.onExcelUpload = function (data) {
        switch (data.code) {
            case costJsBase.enums.noContentCode:
                costJsBase.EventNotify('error', 'Không có tệp nào được tải lên!');
                break;
            case costJsBase.enums.errorCode:
                costJsBase.EventNotify('error', 'Có lỗi xảy ra, tải lên không thành công!');
                if (typeof data.excelFileError != 'undefined' && data.excelFileError?.length) {
                    base.showAndClearErrorBox(true);
                    base.$errorCollapse.append('Tệp chi tiết mã lỗi: <a href="' + data.excelFileError + '" target="_blank">Tải xuống</a>');
                }
                break;
            case costJsBase.enums.successCode:
                if (data.isValid) {
                    base.$tableDataTable.clear();

                    for (var i = 0; i < data.data.length; i++) {
                        base.$tableDataTable.row.add(data.data[i]).draw(false);
                    }
                    base.$boxExcelErrors.addClass('d-none').removeClass('d-block');
                    base.$errorCollapse.html('');

                    if (typeof (data.specifyFieldValue) !== 'undefined')
                        base.latestCode = data.specifyFieldValue;

                    //todo
                    base.disableOnDoneUpload(true);
                    base.onLiveWarning(true);
                    base.hideUpploadFile(true);

                    base.clearInput(base.$inputFileCostEstimateExcelFile);
                    costJsBase.EventNotify('success', 'Nhập dữ liệu thành công!');
                } else {
                    for (var i = 0; i < data.errors.length; i++) {
                        let fullErr = data.errors[i].column?.length ? '<b>' + data.errors[i].column + '</b>: ' + data.errors[i].message : data.errors[i].message;
                        base.$errorCollapse.append(fullErr);
                        base.$errorCollapse.append('<br />');
                    }
                    base.$boxExcelErrors.addClass('d-block').removeClass('d-none');
                }
                break;
            default:
                break;
        }
    }

    // sự kiện khi upload ảnh chứng từ cho dự trù
    base.onContractFileUpload = function (data) {
        if (data.code === costJsBase.enums.successCode) {
            base.disableOnDoneUpload(true);
            base.onLiveWarning(true);
            base.hideUpploadFile(true);

            base.clearInput(base.$inputFileCostEstimateExcelFile);

            if (data.fileUploadExtension === '.zip') {
                let founding = 0;
                let tableData = base.$tableDataTable.rows().data();
                if (data.data.length) {
                    data.data.forEach((v) => {
                        for (var i = 0; i < tableData.length; i++) {
                            if (v.fileName.startsWith(tableData[i].requestCode)) {
                                founding++;
                                let updateRecord = tableData[i];
                                updateRecord.accountImage = v.viewPath;
                                base.$tableDataTable.row(i).data(updateRecord).draw();
                            }
                        }
                    });
                }

                founding == 0 ?
                    costJsBase.EventNotify('warning', 'Không tìm thấy chứng từ trùng khớp!') :
                    costJsBase.EventNotify('success', 'Cập nhật chứng từ thành công ' + founding + '/' + tableData.length);

            }
            else {
                if (data.data.length) {
                    let f = data.data[0];
                    if (base.recordSelector != null) {
                        base.recordSelector.accountImage = f.viewPath;
                        base.$tableDataTable.row(base.recordSelectorIndex).data(base.recordSelector).draw();
                    }
                }
            }
        } else {
            costJsBase.EventNotify('error', data.message);
        }
    }

    // xóa box cảnh báo lỗi sai định dạng excel
    base.showAndClearErrorBox = function () {
        base.$boxExcelErrors.addClass('d-block').removeClass('d-none');
        base.$errorCollapse.html('');
    }

    // on.off disable các nút khi bắt đầu tải lên
    base.disableOnDoneUpload = function (disabled) {
        base.$btnStartUpload.prop('disabled', !disabled);
        base.$btnSelectExcelFile.prop('disabled', !disabled);
        base.$btnSelectContractFile.prop('disabled', !disabled);
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

    // gán sự kiện cho nút download trên table
    base.bindTableOnSelectFile = function () {
        $('button[prop-type="table-contract-button"]').off().bind('click', function () {
            $(this).siblings('input[prop-type="table-contract-file"]').click();
            let point = $(this).closest('tr');
            var record = base.$tableDataTable.row(point).data();
            base.recordSelectorIndex = base.$tableDataTable.row(point).index();
            base.recordSelector = record;
        });
        $('input[prop-type="table-contract-file"]').off().bind('change', function () {
            let selector = $(this);
            let buttonSelector = $(this).siblings('button[prop-type="table-contract-button"]');
            let files = selector.get(0).files;
            if (files.length > 0) {
                let selectFileName = files[0].name;
                let validateFileName = costJsBase.CheckExtension(selectFileName, base.contractFileExts);
                if (!validateFileName) {
                    costJsBase.EventNotify('error', base.$formLang.invalidExcelFileWarning);
                    base.clearInput(selector);
                } else {
                    if (base.validFileSize(files[0].size)) {
                        costJsBase.EventNotify('error', base.$formLang.maxsizeWarning);
                        base.clearInput(selector);
                    } else {
                        costJsBase.JsUploadFile({
                            selector: selector,
                            Url: '/CostEstimate/OnUpdateContractFile',
                            async: true,
                            beforeSend: function () {
                                buttonSelector.html(base.$formLang.pendingUpload).prop('disabled', true);
                            },
                            complete: function () {
                                buttonSelector.html(base.$formLang.oldButtonUpload);
                                buttonSelector.prop('disabled', false).hide();
                                base.clearInput(selector);
                            }
                        }, base.onContractFileUpload,
                            function (err) {
                                buttonSelector.html(base.$formLang.oldButtonUploadNoContent).prop('disabled', false);
                                costJsBase.EventNotify('error', 'Tải lên không thành công, vui lòng thử lại sau!');
                            }
                        )
                    }
                }
            }
        });
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
        })
        base.bindActions();
        base.$btnApprove.bind('click', base.onApprove);
        $('[data-toggle="tooltip"]').tooltip()
    }
    // sinh icon theo loại file + link download
    base.displayIconByExtension = function (file) {
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
        return '<a href="' + file + '" target="_blank">' +
            '<i class="' + className + ' mr-2"></i>Chi tiết</a>' +
            '<a class="ml-2 text-danger" href="javascript:void(0);" prop-type="table-contract-file-trash" class="">' +
            '<i class="far fa-times-circle"></i></i>' +
            '</a>';
    }
    // action tạo yêu cầu
    base.onCreate = function () {
        let data = base.$tableDataTable.rows().data();
        let dataCr = [];
        let isValidData = true;
        for (var i = 0; i < data.length; i++) {
            let dataAt = data[i];
            if (typeof (dataAt.accountImage) == 'undefined' || dataAt.accountImage.length == 0 && dataAt.explanation.length === 0) {
                isValidData = false;
                costJsBase.EventNotify('warning', 'Vui lòng bổ sung đầy đủ giải trình hoặc tài liệu chứng từ cho dự trù!');
                break;
            }
            //CostEstimateType
            dataCr.push(dataAt);
        }

        if (isValidData) {
            var dataPost = {
                Record: costJsBase.ValueFromUrl('record') || base.$recordID.val(),
                Data: dataCr,
                CostEstimateType: base.$requestFormRequestType.val()
            };

            costJsBase.Post({
                Url: '/CostEstimate/OnCreateBulkItem',
                Data: dataPost,
                async: true,
                beforeSend: function () {
                    costJsBase.ButtonState({
                        target: base.$btnCreate,
                        state: 'loading',
                        disabled: true,
                        text: 'Đang lưu ...',
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
    // lưu thành công
    base.onCreateSuccess = function (data) {
        if (data.code === 200) {
            base.onLiveWarning(false);
            costJsBase.EventNotify('success', data.message);
            costJsBase.ButtonState({
                target: base.$btnCreate,
                state: 'done',
                disabled: true,
                html: '<i class="fas fa-check mr-2"></i> Đã lưu',
            });
            //todo: chuyển sang danh sách
            setTimeout(() => {
                window.location.href = '/CostEstimateItem/List?type=week'
            }, 1500);
        } else {
            costJsBase.ButtonState({
                target: base.$btnCreate,
                state: 'normal',
                disabled: false,
                html: '<i class="fas fa-plus mr-2"></i> Lưu lại',
            });
            costJsBase.EventNotify('error', data.message);
        }
    }
    // lưu không thành công
    base.onCreateError = function (error) {
        base.onLiveWarning(true);
        costJsBase.EventNotify('error', 'Có lỗi xảy ra, vui lòng thử lại sau!');
        costJsBase.ButtonState({
            target: base.$btnCreate,
            state: 'normal',
            disabled: false,
            html: '<i class="fas fa-plus mr-2"></i> Lưu lại',
        });
    }
    // phê duyệt - từ chối
    base.onApprove = function () {
        let dataApprove = [];
        $('#createCostEstimateUploadTable tbody tr input[class="form-check-input"]:checked').each(function () {
            let point = $(this).closest('tr');
            var record = base.$tableDataTable.row(point).data();
            dataApprove.push(record);
        });

        if (dataApprove.length === 0) {
            costJsBase.EventNotify('warning', 'Vui lòng chọn dự trù cần duyệt!');
        }
    }
}

$(document).ready(function () {
    let c = new CostElementCreate();
    c.setup();
    $(document).on('click', '[prop-type="trash"]', function () {
        if (confirm('Bạn có chắc chắn muốn xóa yêu cầu này ?')) {
            c.onDeleteRow(this);
        }
    });
});

$(document).on('click', 'a[prop-type="table-contract-file-trash"]', function () {
    if (confirm('Bạn có chắc muốn xóa tệp này ?')) {
        $(this).siblings('a').remove();
        $(this).siblings('button').removeClass('d-none');
        $(this).remove();
    }
});