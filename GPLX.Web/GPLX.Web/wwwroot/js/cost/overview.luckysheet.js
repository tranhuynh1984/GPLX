
var Overview = function () {
    let base = this;
    base.chief = new Chief();
    base.mmSelector = $('#modal-extra-large');;
    base.btnSelect = $('#btnSelect');
    base.selectReportForWeek = $('#selectReportForWeek');
    base.btnCreate = $('#btnCreate');
    base.btnDecline = $('#btnDecline');
    base.pageLanguage = languages.vi;

    base.ckcRecords = typeof selectedOlderRequest != 'undefined' ? selectedOlderRequest : [];
    base.firstLoad = true;
    base.isCreate = false;
    base.isPopOnLoad = true;

    // gán sự kiện
    base.bindActions = function () {
        base.btnCreate.click(function () {
            base.onCreate(false);
        });
        //activator
        base.btnSelect.click(base.openModal);

        base.btnDecline.click(function () {
            base.onCreate(true);
        });
    }
    // initialize
    base.Setup = function () {
        base.bindActions();

        $("#luckysheet").css("height", 780);
        $("#luckysheet").css("width", document.getElementById("luckysheet").clientWidth);

        $("#selectReportForWeek").select2({
            'width': '275px',
            'theme': 'bootstrap4'
        })

        var options = {
            container: 'luckysheet',
            showinfobar: false,
            showtoolbar: false
        }
        luckysheet.create(options);

        base.isCreate = costJsBase.ValueFromUrl('type') == null || costJsBase.ValueFromUrl('type') === 'edit';
        base.reload();
    }

    base.reload = function () {
        let data = null;
        if (!base.firstLoad) {
            data = luckysheet.getAllSheets()[0].celldata;
        }

        costJsBase.Post({
            Url: '/CostEstimate/FetchData',
            Data: {
                record: costJsBase.ValueFromUrl('record'),
                data: data,
                request: base.ckcRecords,
                week: base.selectReportForWeek.val(),
                type: base.isCreate ? 'create' : 'approve'
            }
        }, function (data) {
            if (data.code === 200) {
                base.onLiveWarning(true);
                const contentType = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';
                const b64Data = data.message;
                const blob = base.b64toBlob(b64Data, contentType);
                const fileName = costJsBase.Rad(40) + ".xlsx";
                const file = new File([blob],
                    fileName,
                    { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
                LuckyExcel.transformExcelToLucky(file,
                    function (exportJson, luckysheetfile) {
                        if (exportJson.sheets == null || exportJson.sheets.length === 0) {
                            alert(
                                "Failed to read the content of the excel file, currently does not support xls files!");
                            return;
                        }
                        window.luckysheet.destroy();

                        let dataLayer = exportJson.sheets;


                        for (var i = 0; i < dataLayer.length; i++) {
                            if (!base.isCreate) {
                                dataLayer[i].config.authority = {
                                    selectLockedCells: 0,
                                    selectunLockedCells: 0,
                                    formatCells: 0,
                                    formatColumns: 0,
                                    formatRows: 0,
                                    insertColumns: 0,
                                    insertRows: 0,
                                    insertHyperlinks: 0,
                                    deleteColumns: 0,
                                    deleteRows: 0,
                                    sort: 0,
                                    filter: 0,
                                    usePivotTablereports: 0,
                                    editObjects: 0,
                                    editScenarios: 0,
                                    sheet: true,
                                    hintText: "",
                                    algorithmName: "None",
                                    saltValue: null
                                };
                            }
                            if (dataLayer[i].celldata.length > 1) {
                                for (var j = 0; j < dataLayer[i].celldata.length; j++) {
                                    const sAt = dataLayer[i].celldata[j].v.ct?.s;
                                    if (sAt && dataLayer[i].celldata[j].v.ct?.s.length > 1) {
                                        let cellAttribute = {};
                                        let sAll = '';
                                        for (let k = 0; k < dataLayer[i].celldata[j].v.ct.s.length; k++) {
                                            $.extend(cellAttribute, dataLayer[i].celldata[j].v.ct.s[k]);
                                            sAll += dataLayer[i].celldata[j].v.ct.s[k].v;
                                        }
                                        cellAttribute.v = sAll;
                                        cellAttribute.ff = 'Times New Roman';

                                        dataLayer[i].celldata[j].v.ct.s = [cellAttribute];
                                    }
                                }
                            }
                        }


                        window.luckysheet.create({
                            container: 'luckysheet',
                            showinfobar: false,
                            data: dataLayer,
                            title: exportJson.info.name,
                            userInfo: "admin",
                            showtoolbar: false
                        });
                    });
                base.firstLoad = false;
            } else {
                costJsBase.EventNotify('error', data.message);
            }
        }, function (e) {
            costJsBase.EventNotify('error', 'Lỗi hệ thống, vui lòng thử lại sau!');
        });
    }

    base.onLiveWarning = function (onWarning) {
        if (onWarning) {
            $(window).on("beforeunload", function (event) {
                return '';
            });
        } else {
            $(window).off('beforeunload');
        }
    }

    base.b64toBlob = (b64Data, contentType = '', sliceSize = 512) => {
        const byteCharacters = atob(b64Data);
        const byteArrays = [];

        for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
            const slice = byteCharacters.slice(offset, offset + sliceSize);

            const byteNumbers = new Array(slice.length);
            for (let i = 0; i < slice.length; i++) {
                byteNumbers[i] = slice.charCodeAt(i);
            }

            const byteArray = new Uint8Array(byteNumbers);
            byteArrays.push(byteArray);
        }

        const blob = new Blob(byteArrays, { type: contentType });
        return blob;
    }

    base.openModal = function () {
        costJsBase.OpenModal({
            url: '/CostEstimate/ChiefAccountantApprove?record=' + costJsBase.ValueFromUrl('record'),
            overlay: true,
            selector: base.mmSelector,
            title: 'Chọn yêu cầu lập dự trù',
            dropContent: true,
            callback: function () {
                base.chief = new Chief(base);
                base.chief.setup();
                $('[prop-type="elems.accept"]').click(function () {
                    base.chief.onApprove();
                });
            },
            buttons: {
                accept: {
                    visible: true,
                    html: `<button type="button" prop-type="elems.accept" class="btn btn-outline-success btn-sm">
                                <i class="fad fa-check mr-2" aria-hidden="true"></i> Xác nhận
                            </button>`,
                    listener: {
                        type: 'click',
                        event: function () {

                        }
                    }
                },
                close: {
                    visible: true
                }
            }
        });
    }


    base.intVal = function (i) {
        return typeof i === 'string' ?
            i.replace(/[\$,]/g, '') * 1 :
            typeof i === 'number' ?
                i : 0;
    }

    base.onCreate = function (isDecline) {
        const w = base.intVal(base.selectReportForWeek.val());
        const formId = costJsBase.Rad(30);
        let requestUrl = '/CostEstimate/OnCreate';
        if (w <= 0 || w > 53) {
            costJsBase.EventNotify('warning', "Bạn chưa chọn thời gian lập dự trù!");
            return false;
        }

        const requestType = costJsBase.ValueFromUrl('type');
        let popTitle = 'Bạn có chắc chắn muốn lưu thông tin dự trù này ?';
        if (requestType === 'approve') {
            if (isDecline) {
                requestUrl = '/CostEstimate/OnDecline';
                popTitle = 'Bạn có chắc chắn muốn từ chối dự trù này ?';
            } else
                popTitle = 'Bạn có chắc chắn muốn phê duyệt dự trù này ?';
        } else {
            if (base.ckcRecords.length === 0) {
                costJsBase.EventNotify('warning', "Bạn chưa chọn yêu cầu để lên dự trù!");
                return false;
            }
        }

        const ops = {
            title: popTitle,
            icon: 'success',
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
                    Url: requestUrl,
                    Data: {
                        record: costJsBase.ValueFromUrl('record'),
                        data: luckysheet.getAllSheets()[0].celldata,
                        request: base.ckcRecords,
                        week: base.selectReportForWeek.val(),
                        type: requestType,
                        isApprove: !isDecline
                    }
                }).then(data => {
                    if (data.code !== costJsBase.enums.successCode) {
                        Swal.showValidationMessage(data.message);
                        deferred.resolve(false);
                    } else {
                        base.onLiveWarning(false);
                        deferred.resolve(data.message);
                    }
                }).fail(e => {
                    Swal.showValidationMessage('Lỗi hệ thống, vui lòng thử lại sau!');
                    deferred.resolve(false);
                });
                return deferred.promise();
            }
        };

        if (isDecline) {
            ops.didOpen = () => {
                jQuery.fn.validateSetup($('#' + formId), {
                    rules: { ___reason: { required: true } },
                    messages: { ___reason: { required: 'Vui lòng nhập lý do từ chối' } }
                });
            }

            ops.html = `<form id="` + formId + `"><div class="form-group text-left">
                                                <label class="col-form-label" for="inputError">Lý do từ chối</label>
                                                <textarea rows="3" id="___reason" name="___reason" type="text" class="form-control"></textarea>
                                          </div>
                                      </form>`;

            ops.preConfirm = function () {
                let formValid = $('#' + formId).on().valid();
                if (formValid) {
                    var deferred = $.Deferred();
                    costJsBase.PromissePost({
                        Url: requestUrl,
                        Data: {
                            record: costJsBase.ValueFromUrl('record'),
                            data: luckysheet.getAllSheets()[0].celldata,
                            request: base.ckcRecords,
                            week: base.selectReportForWeek.val(),
                            type: requestType,
                            isApprove: !isDecline
                        }
                    }).then(data => {
                        if (data.code !== costJsBase.enums.successCode) {
                            Swal.showValidationMessage(data.message);
                            deferred.resolve(false);
                        } else {
                            base.onLiveWarning(false);
                            deferred.resolve(data.message);
                        }
                    }).fail(e => {
                        Swal.showValidationMessage('Lỗi hệ thống, vui lòng thử lại sau!');
                        deferred.resolve(false);
                    });
                    return deferred.promise();
                } else
                    return false;
            }
        }

        Swal.fire(ops).then((result) => {
            if (result.isConfirmed) {
                Swal.fire({
                    title: result.value,
                    icon: 'success',
                    timer: 1500,
                    timerProgressBar: true,
                    didOpen: () => {
                        Swal.showLoading();
                        timerInterval = setInterval(() => {
                            window.location.href = '/CostEstimate/List?type=week';
                        }, 100);
                    },
                    willClose: () => {
                        clearInterval(timerInterval);
                    }
                });
            }
        });
    }
}

var Chief = function (over) {
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
            render: function (data, type, row, meta) {
                return '    ';
            }
        },
        // Ô checkbox
        {
            idx: 1,
            render: function (data, type, row, meta) {
                let checked = '';
                let disabled = '';

                if (over.isCreate) {
                    let rOnSelect = over.ckcRecords.filter(c => {
                        return c.id === row.id;
                    });
                    if (rOnSelect.length && rOnSelect[0].isDeleted === 0) checked = 'checked';
                    //else if (rOnSelect.length && rOnSelect[0].isDeleted === 1) disabled = 'disabled';
                } else {
                    if (over.ckcRecords.length > 0) {
                        let rOnSelect = over.ckcRecords.filter(c => {
                            return c.id === row.id;
                        });
                        if (row.isDeleted === 1)
                            disabled = 'disabled';
                        else if (rOnSelect.length && rOnSelect[0].isDeleted === 0)
                            checked = 'checked';
                    }
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
        deferred.resolve(base.columnTableConfigs);
        return deferred.promise();
    }

    base.setupDataTable = function (config) {
        let settings = {
            selector: $tableSelector,
            columns: config,
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

                    // trường hợp tạo mới
                    // mặc định checkAll

                    let inputCheck = $('[ckcElement]:checked');

                    inputCheck.each(function () {
                        $(this).prop('checked', true).change();
                    });

                    // lần đầu show popup & chưa bấm nút xác nhận + trường hợp tạo mới
                    // -> mặc định chọn all
                    if (over.isCreate && over.isPopOnLoad && !inputCheck.length && $('input[ckcElement]').length) {
                        base.ckcAll.prop('checked', true).change();
                    }
                }, 350);
            },
            iDisplayLength: 50
        };

        if (over.isCreate) {
            settings.ajax = {
                url: "/CostEstimate/SearchRequestList",
                data: function (d) {
                    d.Keywords = base.$requestSearchKeywords.val();
                    d.RequestType = costJsBase.ValueFromUrl('type');
                    d.CurrentCostEstimateRecord = olderId;
                },
                type: "post"
            };
            settings.serverSide = true;
            settings.paging = true;
        } else {
            debugger
            settings.serverSide = false;
            settings.data = selectedOlderRequest;
            settings.initComplete = function () {
                $.fn.dataTable.ext.search.push(
                    function (settings, searchData, index, rowData, counter) {
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
                action: function () {
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
                    $(nodesInPage[i]).find('input[type="checkbox"]:not(:disabled)').prop("checked", true);
                } else {
                    base.onRecordSelect(rowsInPage[i], false);
                    $(nodesInPage[i]).find('input[type="checkbox"]:not(:disabled)').prop("checked", false);
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
            var checkAny = base.ckcRecords.filter(c => { return c.id === rc.id; });
            if (!checkAny.length)
                base.ckcRecords.push(rc);
        } else {
            base.ckcRecords = $.grep(base.ckcRecords, function (value) {
                return value !== rc;
            });
        }
        let pLength = base.$tableDataTable.rows({ page: 'current' }).data().length;
        let fRc = base.ckcRecords.filter(c => { return c.isDeleted === 1; });
        base.ckcAll.prop('checked', (base.ckcRecords.length - fRc.length) === pLength);
        over.ckcRecords = base.ckcRecords;
    }

    base.onWindowLoad = function () {
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

    // chọn yêu cầu
    base.onApprove = function () {
        const isCreate = costJsBase.ValueFromUrl('record') == null || costJsBase.ValueFromUrl('type') == null;
        if (base.ckcRecords.length === 0 && isCreate) {
            costJsBase.EventNotify('warning', 'Bạn chưa chọn yêu cầu để lên dự trù!');
        } else {
            for (var i = 0; i < base.ckcRecords.length; i++) {
                let rec = base.ckcRecords[i];
                rec.record = (base.ckcRecords[i].id);
                rec.accountImage = base.ckcRecords[i].requestImage;
            }

            if (!over.isCreate) {
                let inputNotChecks = $('[ckcElement]:not(:checked)');
                inputNotChecks.each(function () {
                    let point = $(this).closest('tr');
                    var rc = base.$tableDataTable.row(point).data();
                    rc.isDeleted = 1;

                    over.ckcRecords.push(rc);
                });

            }

            over.reload();
            over.isPopOnLoad = false;
            over.mmSelector.modal('hide');
        }
    }

    // tìm kiếm
    base.searching = function () {
        base.$onLoad = false;
        if (base.$tableDataTable)
            base.$tableDataTable.ajax.reload();
    }

    // ẩn hiện chi tiết
    base.showDetail = function (d) {
        let par = moment(d.billDate, 'YYYY-MM-DDTHH:mm:ss');
        let sYear = par.year() > 1900 ? par.format('DD/M/YYYY') : '-';

        let decliner = '';
        if (typeof d.isDeleted != 'undefined') {
            if (d.isDeleted === 1) {
                decliner = '<td class="font-weight-bold">Người từ chối:</td>' +
                    '<td>' + (d.updaterName) + '</td>' +
                    '<td class="font-weight-bold">Thời gian từ chối:</td>' +
                    '<td>' + d.updatedDate + '</td>';
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
    let c = new Overview();
    c.Setup();
});

