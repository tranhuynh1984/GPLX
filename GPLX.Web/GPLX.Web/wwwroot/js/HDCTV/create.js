var HDCTVCreate = function () {
    let base = this;
    let $tableSelector = $('#tblList');
    let $tableSelectorType2 = $('#tblListType2');
    let $dropStats = $('#requestSearchStats');

    base.$tableDataTable = null;
    base.$tableDataTableType2 = null;
    base.$searchButton = $('#requestSearchButton');
    base.$btnSync = $('#btnSync');
    base.$exportExcel = $('#btnExportExcel');
    base.$errorCollapse = $('#errorCollapse');
    base.$boxExcelErrors = $('#boxExcelErrors');
    base.$txtMaCTV = $('#txtMaCTV');
    base.$txtTenCTV = $('#txtTenCTV');
    base.$onLoad = true;
    base.$popOverlay = $('#modal-overlay');
    base.$ddlStatus = $('#ddlStatus option:selected');
    base.$ddlGroup = $('#ddlGroup option:selected');
    base.$btnCreate = $('#btnCreate');
    base.$btnNew = $('#btnNew');
    base.$btnBack = $('#btnBack');
    base.$btnSaveHD = $('#btnSaveHD');

    base.$btnSelectExcelFile = $('#btnSelectExcelFile');
    base.$inputFileCostEstimateExcelFile = $('#costEstimateExcelFile');
    base.$labelFileName = $('#labelFileSelect');
    base.$btnStartUpload = $('#btnStartUpload');

    // cấu hình các chuỗi string cố định
    base.$formLang = languages.vi.costElement.create;

    base.recordSelector = null;
    base.recordSelectorIndex = -1;
    base.pageLanguage = languages.vi.HDCTV;

    base.settingPage = {
        pop: {
            url: '',
            title: base.pageLanguage.list.popup.title,
            baseTitle: base.pageLanguage.list.popup.baseTitle,
            viewLabel: base.pageLanguage.list.popup.viewLabel,
            createLabel: base.pageLanguage.list.popup.createLabel,
            historyLabel: base.pageLanguage.list.popup.historyLabel,
            editLabel: base.pageLanguage.list.popup.editLabel,
            selector: base.$popOverlay,
            replator: base.pageLanguage.list.popup.replator,
            buttons: {
                accept: {
                    visible: false
                },
                decline: { visible: false },
                close: { visible: false }
            },
            dropContent: true,
            callback: function () {
                base.$createForm = $('#___createForm');

                jQuery.fn.validateSetup(base.$createForm,
                    {
                        rules: {
                            selectTypes: { required: true },
                        },
                        messages: {
                            selectTypes: { required: 'Bạn chưa chọn nhóm đơn vị' }
                        }
                    }
                )
            }
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

    base.Setup = function () {
        //$("#ddlDMCP").select2();
        //$("#ddlDMCP").css('display', 'none');
        $(".MaCP").select2();
        
        base.bindActions();
        base.setupDataTable();
        base.setupDataTableType2();
        base.$exportExcel.bind("click", function () {
            base.exportExcel();
        })

        base.$btnSync.bind('click',
            function () {
                base.onSync(this);
            });
    }

    base.searchForm = function (data) {
        if (typeof (data) !== 'undefined') {
            data.SubId = $('#hddmanhom').val();
            //data.SubName = base.$txtTenCTV.val();
            //data.IsUse = $('#ddlStatus option:selected').val();
            base.setParamsAfterSearch(data);
        }
    }

    base.setupDataTable = function () {
        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            ajax: {
                url: "/HDCTV/SearchCreateType1",
                data: function (d) {
                    base.searchForm(d);
                }
            },
            serverSide: false,
            columns: [
                {
                    idx: 0,
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    },
                    width: "5%",
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return '<label class="lblmaCP" style="display:block">' + row.maCP + '</label>' + '<select class="form-control form-control-sm MaCP" onchange="SelectChanged(this)" style="display:none;width:100%;" >' + $('#ddlDMCP').html().replace('value="' + row.maCP + '"', 'value="' + row.maCP + '" selected') + '</select>';
                    },
                    width: "15%",
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return '<label style="display:block">' + row.tenCP + '</label><input readonly="readonly" class="TenCP" style="display:none;width:100%;text-align:right;" value="' + row.tenCP + '" ></input>';
                        
                    },
                    width: "10%",
                },
                {
                    idx: 3,
                    render: function (data, type, row, meta) {
                        return '<label style="display:block">' + row.bP1?.toLocaleString('en-US', { valute: 'USD', }) + '</label><input maxlength="13" oninput="this.value=this.value.replace(/\\D/g,\''+'\').replace(/^0+(?=\d)/,\''+'\')" class="BP1" style="display:none;width:100%;text-align:right;" value="' + row.bP1 + '" ></input>';
                    },
                    width: "5%",
                    class: "text-right"
                },
                {
                    idx: 4,
                    render: function (data, type, row, meta) {
                        return '<label style="display:block">' + row.bP2?.toLocaleString('en-US', { valute: 'USD', }) + '</label><input maxlength="13" oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP2" style="display:none;width:100%;text-align:right;" value="' + row.bP2 + '" ></input>';
                    },
                    width: "5%",
                    class: "text-right"
                },
                {
                    idx: 5,
                    render: function (data, type, row, meta) {
                        return '<label style="display:block">' + row.bP3?.toLocaleString('en-US', { valute: 'USD', }) + '</label><input maxlength="13" oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP3" style="display:none;width:100%;text-align:right;" value="' + row.bP3 + '" ></input>';
                    },
                    width: "5%",
                    class: "text-right"
                },
                {
                    idx: 6,
                    render: function (data, type, row, meta) {
                        return '<label style="display:block">' + row.bP4?.toLocaleString('en-US', { valute: 'USD', }) + '</label><input maxlength="13" oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP4" style="display:none;width:100%;text-align:right;" value="' + row.bP4 + '" ></input>';
                    },
                    width: "5%",
                    class: "text-right"
                },
                {
                    idx: 7,
                    render: function (data, type, row, meta) {
                        return '<label style="display:block">' + row.bP5?.toLocaleString('en-US', { valute: 'USD', }) + '</label><input maxlength="13" oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP5" style="display:none;width:100%;text-align:right;" value="' + row.bP5 + '" ></input>';
                    },
                    width: "5%",
                    class: "text-right"
                },
                {
                    idx: 8,
                    render: function (data, type, row, meta) {
                        return '<label style="display:block">' + row.bP6?.toLocaleString('en-US', { valute: 'USD', }) + '</label><input maxlength="13" oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP6" style="display:none;width:100%;text-align:right;" value="' + row.bP6 + '" ></input>';
                    },
                    width: "5%",
                    class: "text-right"
                },
                {
                    idx: 9,
                    render: function (data, type, row, meta) {
                        return '<label style="display:block">' + row.bP7?.toLocaleString('en-US', { valute: 'USD', }) + '</label><input maxlength="13" oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP7" style="display:none;width:100%;text-align:right;" value="' + row.bP7 + '" ></input>';
                    },
                    width: "5%",
                    class: "text-right"
                },
                {
                    idx: 10,
                    render: function (data, type, row, meta) {
                        return '<label style="display:block">' + row.bP8?.toLocaleString('en-US', { valute: 'USD', }) + '</label><input maxlength="13" oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP8" style="display:none;width:100%;text-align:right;" value="' + row.bP8 + '" ></input>';
                    },
                    width: "5%",
                    class: "text-right"
                },
                {
                    idx: 11,
                    render: function (data, type, row, meta) {
                        return '<label style="display:block">' + row.bP9?.toLocaleString('en-US', { valute: 'USD', }) + '</label><input maxlength="13" oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP9" style="display:none;width:100%;text-align:right;" value="' + row.bP9 + '" ></input>';
                    },
                    width: "5%",
                    class: "text-right"
                },
                {
                    idx: 12,
                    render: function (data, type, row, meta) {
                        return '<label style="display:block">' + row.bP10?.toLocaleString('en-US', { valute: 'USD', }) + '</label><input maxlength="13" oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP10" style="display:none;width:100%;text-align:right;" value="' + row.bP10 + '" ></input>';
                    },
                    width: "5%",
                    class: "text-right"
                },
                {
                    idx: 13,
                    render: function (data, type, row, meta) {
                        return '<label style="display:block">' + row.bP11?.toLocaleString('en-US', { valute: 'USD', }) + '</label><input maxlength="13" oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP11" style="display:none;width:100%;text-align:right;" value="' + row.bP11 + '" ></input>';
                    },
                    width: "5%",
                    class: "text-right"
                },
                {
                    idx: 14,
                    render: function (data, type, row, meta) {
                        if (row.isActive == 1)
                            return '<label class="lblStatusSub1" style="display:block">' + row.isActiveName + '</label>' + '<select class="form-control form-control-sm ddlStatusSub1" style="display:none" ><option value="0" >Vô hiệu</option><option value="1" selected>Kích hoạt</option></select>';
                        else
                            return '<label class="lblStatusSub1" style="display:block">' + row.isActiveName + '</label>' + '<select class="form-control form-control-sm ddlStatusSub1" style="display:none" ><option value="0" selected>Vô hiệu</option><option value="1" >Kích hoạt</option></select>';
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 15,
                    render: function (data, type, row, meta) {
                        let s = '';
                        s = '<a class="mr-2 btnedit" style="display:inline-block" href="javascript:void(0)" title="Chỉnh sửa thông tin loại đề xuất" prop-type="elems.table.edit" prop-type-act="pop" ><i class="fad fa-pencil med-theme-primary-button"></i></a>'
                        s += '<a class="mr-2 btnsave" style="display:none" prop-type="elems.table.accept" prop-type-act="save" title="Lưu HĐCTV" href="javascript:void(0)" ><i class="fad fa-check mr-2"></i></a>';
                        s += '<a class="mr-2 btnCancel" style="display:none" prop-type="elems.table.cancel" prop-type-act="pop" title="Cancel" href="javascript:void(0)"><i class="fad fa fa-times"></i></a>';
                        s += '<a class="text-danger mr-2 btnremove" prop-type="elems.table.delete" prop-type-act="confirm" title="Xóa loại đề xuất" href="javascript:void(0)"><i class="fad fa-ban"></i></a>'
                        s += '<hidden class="dr_dr" id="dr_' +  meta.row + meta.settings._iDisplayStart + 1+'"></hidden>'
                        return s;
                    },
                    width: "5%",
                    class: "text-center"
                }
            ],
            drawCallback: function () {
                base.drawInit();
            },
            paging: false,
            scrollX: true,
            responsive: false,
        });
    }

    base.setupDataTableType2 = function () {
        base.$tableDataTableType2 = $.fn.jsTableRegister({
            selector: $tableSelectorType2,
            ajax: {
                url: "/HDCTV/SearchCreateType2",
                data: function (d) {
                    base.searchForm(d);
                }
            },
            serverSide: false,
            columns: [
                {
                    idx: 0,
                    render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    },
                    width: "5%"
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return '<label class="lblmaCP" style="display:block">' + row.maCP + '</label>' + '<select class="form-control form-control-sm MaCP" onchange="SelectChanged(this)" style="display:none;width:100%;" >' + $('#ddlDMCP').html().replace('value="' + row.maCP + '"', 'value="' + row.maCP + '" selected') + '</select>';
                    },
                    width: "15%"
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return '<label style="display:block">' + row.tenCP + '</label><input readonly="readonly" class="TenCP" style="display:none;width:100%;text-align:right;" value="' + row.tenCP + '" ></input>';

                    },
                    width: "10%"
                },
                {
                    idx: 3,
                    render: function (data, type, row, meta) {
                        return '<label style="display:block">' + row.fixedPrice.toLocaleString('en-US', { valute: 'USD', }) + '</label><input class="FixedPrice" oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" style="display:none;width:100%;text-align:right;" value="' + row.fixedPrice + '" ></input>';
                    },
                    width: "5%",
                    class: "text-right"
                },
                {
                    idx: 4,
                    render: function (data, type, row, meta) {
                        if (row.isActive == 1)
                            return '<label class="lblStatusSub2" style="display:block">' + row.isActiveName + '</label>' + '<select class="form-control form-control-sm ddlStatusSub2" style="display:none" ><option value="0" >Vô hiệu</option><option value="1" selected>Kích hoạt</option></select>';
                        else
                            return '<label class="lblStatusSub2" style="display:block">' + row.isActiveName + '</label>' + '<select class="form-control form-control-sm ddlStatusSub2" style="display:none" ><option value="0" selected>Vô hiệu</option><option value="1" >Kích hoạt</option></select>';
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 5,
                    render: function (data, type, row, meta) {
                        let s = '';
                        s = '<a class="mr-2 btnedit" style="display:inline-block" href="javascript:void(0)" title="Chỉnh sửa thông tin loại đề xuất" prop-type="elems.table.edit" prop-type-act="pop" ><i class="fad fa-pencil med-theme-primary-button"></i></a>'
                        s += '<a class="mr-2 btnsave" style="display:none" prop-type="elems.table.accept" prop-type-act="save" title="Lưu HĐCTV" href="javascript:void(0)" ><i class="fad fa-check mr-2"></i></a>';
                        s += '<a class="mr-2 btnCancel" style="display:none" prop-type="elems.table.cancel" prop-type-act="pop" title="Cancel" href="javascript:void(0)"><i class="fad fa fa-times"></i></a>';
                        s += '<a class="text-danger mr-2 btnremove" prop-type="elems.table.delete" prop-type-act="confirm" title="Xóa loại đề xuất" href="javascript:void(0)"><i class="fad fa-ban"></i></a>'
                        s += '<hidden class="dr_dr" id="dr_' + meta.row + meta.settings._iDisplayStart + 1 + '"></hidden>'
                        return s;
                    },
                    width: "5%",
                    class: "text-center"
                }
            ],
            drawCallback: function () {
                base.drawInit2();
            },
            paging: false,
            scrollX: true,
            responsive: false,
        });
    }

    base.exportExcel = function () {
        let subId = $('#txtmanhom').val()
        if(subId <= 0) {
            costJsBase.EventNotify('error', 'Bạn cần tạo hợp đồng trước khi thực hiện tính năng này!')
            return;
        }
        let formType = base.$inputFileCostEstimateExcelFile.data('formtype');
        costJsBase.ExportExcel('/HDCTV/ExportExcel', {
            subId: subId,
            formType: formType
        });
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
        base.$btnSelectExcelFile.bind('click', function () {
            let subId = $('#txtmanhom').val()
            if(subId <= 0) {
                costJsBase.EventNotify('error', 'Bạn cần tạo hợp đồng trước')
                return;
            }
            
            base.FileUploadExts = ['.xlsx'];
            let formType = base.$inputFileCostEstimateExcelFile.data('formtype');
            let data = new FormData();
            data.append( 'formType', formType);
            data.append('subId', $('#hddmanhom').val());

            base.$inputFileCostEstimateExcelFile.click();
            base.actOnUpload = function () {
                costJsBase.JsUploadFile({
                    selector: base.$inputFileCostEstimateExcelFile,
                    //Url: '/ActuallySpent/OnExcelUpload',
                    Data: data,
                    Url: '/HDCTV/OnSCTDataUpload',
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

                let validateFileName = costJsBase.CheckExtension(selectFileName, base.FileUploadExts);
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

        base.validFileSize = function (size) {
            var kbSize = parseFloat(size);
            var mbSize = parseFloat(kbSize / 1024 / 1024); //==> to MB
            return mbSize > 10;
        }

        base.$btnStartUpload.bind('click', function () {
            let files = base.$inputFileCostEstimateExcelFile.get(0).files;
            if (files.length === 0) {
                return false;
            } else {
                base.actOnUpload();
            }
        });

        base.$searchButton.bind("click", function () {
            base.searching(true);
        });

        base.$btnCreate.bind("click", function () {
            base.gotoCreate("Create");
        });

        base.$btnNew.bind("click", function () {
            if ($('#hddmanhom').val() != "0") {
                if ($("#hddGroupId").val() == "1") {
                    base.createNewRow();
                }
                else if ($("#hddGroupId").val() == "2") {
                    base.createNewRowType2();
                }
            }
        });

        base.$btnSaveHD.bind("click", function () {
            base.onCreateHD();
        });
        
        base.$btnBack.bind("click",
            function () {
                window.location.href = '/HDCTV/List?GroupId=' + $("#hddGroupId").val();
            });

        $.fn.eEnterActions({
            selector: base.$requestSearchKeywords, action: function (elm) {
                base.searching(true);
            }
        });
    }

    base.searching = function (resetPage) {
        base.$onLoad = false;

        if (base.$tableDataTable)
            base.$tableDataTable.ajax.reload(null, resetPage);

        if (base.$tableDataTableType2)
            base.$tableDataTableType2.ajax.reload(null, resetPage);
    }

    base.gotoCreate = function (type) {
        window.location.href = '/HDCTV/Create?type=' + type + '?GroupId=' + $("#ddlGroup").val();
    }

    base.createNewRow = function () {

        if ($("#tblList tbody").find('.dataTables_empty').length > 0)
            $("#tblList tbody").find('tr').remove();

        var stt = $("#tblList tbody").find('tr').length + 1;
        var _newid = "ddlDMCP" + $(".MaCP").length;

        let dr = '';
        dr = '<tr class="odd">';
        dr += '	<td>' + stt + '</td>';
        dr += '	<td class="text-center"><label class="lblmaCP" style="display: none;"></label>' + '<select id="' + _newid + '" class="form-control form-control-sm MaCP" onchange="SelectChanged(this)" style="display:block;width:100%;" >' + $('#ddlDMCP').html() + '</select>';
        dr += '	<td class="text-center"><label style="display: none;"></label><input class="TenCP" style="display: block; width: 100%;" value="" readonly="readonly" /></td>';
        dr += '	<td class="text-center"><label style="display: none;"></label><input oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP1" style="display: block; width: 100%;text-align:right;" value="" /></td>';
        dr += ' <td class="text-center"><label style="display: none;"></label><input oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP2" style="display: block; width: 100%;text-align:right;" value="" /></td>';
        dr += ' <td class="text-center"><label style="display: none;"></label><input oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP3" style="display: block; width: 100%;text-align:right;" value="" /></td>';
        dr += ' <td class="text-center"><label style="display: none;"></label><input oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP4" style="display: block; width: 100%;text-align:right;" value="" /></td>';
        dr += ' <td class="text-center"><label style="display: none;"></label><input oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP5" style="display: block; width: 100%;text-align:right;" value="" /></td>';
        dr += ' <td class="text-center"><label style="display: none;"></label><input oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP6" style="display: block; width: 100%;text-align:right;" value="" /></td>';
        dr += ' <td class="text-center"><label style="display: none;"></label><input oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP7" style="display: block; width: 100%;text-align:right;" value="" /></td>';
        dr += ' <td class="text-center"><label style="display: none;"></label><input oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP8" style="display: block; width: 100%;text-align:right;" value="" /></td>';
        dr += ' <td class="text-center"><label style="display: none;"></label><input oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP9" style="display: block; width: 100%;text-align:right;" value="" /></td>';
        dr += ' <td class="text-center"><label style="display: none;"></label><input oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP10" style="display: block; width: 100%;text-align:right;" value="" /></td>';
        dr += ' <td class="text-center"><label style="display: none;"></label><input oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="BP11" style="display: block; width: 100%;text-align:right;" value="" /></td>';
        dr += ' <td class="text-center"><label class="lblStatusSub1" style="display:none">Kích hoạt</label><select class="form-control form-control-sm ddlStatusSub1" style="display:block" ><option value="0" >Vô hiệu</option><option value="1" selected>Kích hoạt</option></select></td>';
        dr += ' <td class="text-center">';
        dr += ' 	<a class="mr-2 btnedit" style="display:none" href="javascript:void(0)" title="Chỉnh sửa thông tin HĐCTV" prop-type="elems.table.edit" prop-type-act="pop" ><i class="fad fa-pencil med-theme-primary-button"></i></a>';
        dr += '     <a class="mr-2 btnsave" prop-type="elems.table.accept" prop-type-act="save" title="Lưu HĐCTV" href="javascript:void(0)"><i class="fad fa-check mr-2"></i></a>';
        
        dr += '     <a class="text-danger mr-2 btnremoveNew" prop-type="elems.table.delete" prop-type-act="confirm" title="Xóa HĐCTV" href="javascript:void(0)"><i class="fad fa-ban"></i></a><hidden class="dr_dr" id="dr_001"></hidden>';
        dr += ' </td>';
        dr += '</tr>';

        $("#tblList tbody").append(dr);

        $("#" + _newid).select2();
        
    }

    base.createNewRowType2 = function () {
        if ($("#tblListType2 tbody").find('.dataTables_empty').length > 0)
            $("#tblListType2 tbody").find('tr').remove();

        var stt = $("#tblListType2 tbody").find('tr').length + 1;
        var _newid = "ddlDMCP" + $(".MaCP").length;

        let dr = '';
        dr = '<tr class="odd">';
        dr += '	<td>' + stt + '</td>';
        dr += '	<td class="text-center"><label class="lblmaCP" style="display: none;"></label>' + '<select id="' + _newid + '" class="form-control form-control-sm MaCP" onchange="SelectChanged(this)" style="display:block;width:100%;" >' + $('#ddlDMCP').html() + '</select>';
        dr += '	<td class="text-center"><label style="display: none;"></label><input class="TenCP" style="display: block; width: 100%;" value="" readonly="readonly" /></td>';
        dr += '	<td class="text-center"><label style="display: none;"></label><input oninput="this.value=this.value.replace(/\\D/g,\'' + '\').replace(/^0+(?=\d)/,\'' +'\')" class="FixedPrice" style="display: block; width: 100%;text-align:right;" value="" /></td>';
        dr += ' <td class="text-center"><label class="lblStatusSub2" style="display:none">Kích hoạt</label><select class="form-control form-control-sm ddlStatusSub2" style="display:block" ><option value="0" >Vô hiệu</option><option value="1" selected>Kích hoạt</option></select></td>';
        dr += ' <td class="text-center">';
        dr += ' 	<a class="mr-2 btnedit" style="display:none" href="javascript:void(0)" title="Chỉnh sửa thông tin HĐCTV" prop-type="elems.table.edit" prop-type-act="pop" ><i class="fad fa-pencil med-theme-primary-button"></i></a>';
        dr += '     <a class="mr-2 btnsave" prop-type="elems.table.accept" prop-type-act="save" title="Lưu HĐCTV" href="javascript:void(0)"><i class="fad fa-check mr-2"></i></a>';
        dr += '     <a class="text-danger mr-2 btnremoveNew" prop-type="elems.table.delete" prop-type-act="confirm" title="Xóa HĐCTV" href="javascript:void(0)"><i class="fad fa-ban"></i></a><hidden class="dr_dr" id="dr_001"></hidden>';
        dr += ' </td>';
        dr += '</tr>';

        $("#tblListType2 tbody").append(dr);
        $("#" + _newid).select2();
    }

    base.setParamsAfterSearch = function (forms) {
        if (base.$onLoad)
            return false;
        let url = URI(window.location.href);
        let urlBuilder = url.toString();
        urlBuilder = $.fn.buildParamURI(urlBuilder, "stats", forms.Status, !typeof (forms.Status) !== 'undefined');
        urlBuilder = $.fn.buildParamURI(urlBuilder, "unit", forms.UserUnit, !(typeof (forms.UserUnit) !== 'undefined'));
        urlBuilder = $.fn.buildParamURI(urlBuilder, "w", forms.ReportForWeek, !(typeof (forms.ReportForWeek) !== 'undefined'));
        window.history.pushState("", "", urlBuilder);
    }

    base.readURI = function () {
        var deferred = $.Deferred();
        let params = $.fn.dataURI(window.location.href);
        jQuery.fn.setElementValue('#requestSearchStats', params["stats"], function () {
            base.statsChange(params["stats"]);
        });
        jQuery.fn.setElementValue('#selectReportForWeek', params["w"]);

        deferred.resolve();
        return deferred.promise();
    }

    base.drawInit = () => $('#tblList [prop-type]').on('click', base.tabRecordFuncHandle);
    base.drawInit2 = () => $('#tblListType2 [prop-type]').on('click', base.tabType2RecordFuncHandle);

    base.tabRecordFuncHandle = function () {
        // prop-type [{xyz}.target.action]
        let propAct = $(this).attr('prop-type-act');
        let propTypes = $(this).attr('prop-type');
        let separators = propTypes.split('.');
        let action = separators[2];
        let point = $(this).closest('tr');
        var record = base.$tableDataTable.row(point).data();
        base.recordSelectorIndex = base.$tableDataTable.row(point).index();
        base.recordSelector = record;
        //record.id
        //todo: 
        if (typeof (propAct) !== 'undefined') {
            switch (propAct) {
                case "pop":
                    switch (action) {
                        
                        case "edit":

                            let _tr = $(this).closest('tr');
                            _tr.find('td').earch

                            _tr.find('td').each(function (index) {
                                $(this).find('label').css('display', 'none');
                                $(this).find('input').css('display', 'block');
                                $(this).find('select').css('display', 'block');
                                
                            });

                            _tr.find('.btnsave').css('display', 'inline-block');
                            _tr.find('.btnCancel').css('display', 'inline-block');
                            _tr.find('.btnedit').css('display', 'none');
                            _tr.find('.btnremove').css('display', 'none');

                            _tr.find(".MaCP").select2();

                            var _macp = _tr.find(".lblmaCP").text();
                            _tr.find(".MaCP").val(_macp);
                            break;
                        case "cancel":

                            var _trcancel = $(this).closest('tr');
                            _trcancel.find('td').earch

                            _trcancel.find('td').each(function (index) {
                                $(this).find('label').css('display', 'block');
                                $(this).find('input').css('display', 'none');
                                $(this).find('select').css('display', 'none');

                            });

                            _trcancel.find('.btnsave').css('display', 'none');
                            _trcancel.find('.btnCancel').css('display', 'none');
                            _trcancel.find('.btnedit').css('display', 'inline-block');
                            _trcancel.find('.btnremove').css('display', 'inline-block');

                            _trcancel.find(".MaCP").next(".select2-container").hide();
                          
                            break;
                        default:
                            break;
                    }
                    break;
                case "confirm":
                    Swal.fire({
                        title: 'Bạn có chắc chắn muốn xóa dịch vụ này?' + '</br> Bản ghi xóa: ' + record.maCP,
                        icon: 'warning',
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
                                Url: '/HDCTV/OnRemoveType1',
                                Data: {
                                    maCP: record.maCP,
                                    subId: record.subId
                                }
                            }).then(data => {
                                if (data.code !== costJsBase.enums.successCode) {
                                    Swal.showValidationMessage(data.message);
                                    deferred.resolve(false);
                                } else {
                                    costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
                                    base.searching();
                                    deferred.resolve(data.message);
                                }
                            }).fail(e => {
                                Swal.showValidationMessage('Lỗi hệ thống, vui lòng thử lại sau!');
                                deferred.resolve(false);
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
                                    timerInterval = setInterval(() => {
                                    },
                                        100);
                                },
                                willClose: () => {
                                    clearInterval(timerInterval)
                                }
                            });
                        }
                    });
                    break;
                case "save":
                    let _tr = $(this).closest('tr');

                    //_tr.find('td').earch

                    //_tr.find('td').each(function (index) {
                    //    $(this).find('input').css('display', 'none');
                    //    $(this).find('label').css('display', 'block');
                    //});
                  
                    //_tr.find('.btnsave').css('display', 'none');
                    //_tr.find('.btnedit').css('display', 'inline-block');
                    
                    break;
                default:
                    break;
            }
        }
    }

    base.onFileUpload = function (data) {
        switch (data.code) {
            case costJsBase.enums.successCode:
                base.disableOnDoneUpload(true);
                base.hideUpploadFile(true);
                base.clearInput(base.$inputFileCostEstimateExcelFile);
                costJsBase.EventNotify('success', 'Upload file thành công!');
                break;
            default:
                costJsBase.EventNotify('error', 'Có lỗi xảy ra, tải lên không thành công!');
                break;
        }
    }

        // xóa dữ liệu input file
    base.clearInput = function (selector) {
        selector.val('');
    }
    
    base.renderType1 = function(data) {
        base.$tableDataTable.rows.add(data).draw()
            .nodes()
            .to$()
            .css( 'color', '#007bff' )
            .animate( { color: 'black' } );
    }

    base.renderType2 = function(data) {
        base.$tableDataTableType2.rows.add(data).draw()
            .nodes()
            .to$()
            .css( 'color', '#007bff' )
            .animate( { color: 'black' } );
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
                    if (!data.data.length) {
                        costJsBase.EventNotify('warning', 'Không có dữ liệu hợp lệ trong danh sách đã nhập!');
                        if (typeof data.excelFileError != 'undefined' && data.excelFileError?.length) {
                            base.showAndClearErrorBox(true);
                            base.$errorCollapse.append('Tệp chi tiết mã lỗi: <a href="/' + data.excelFileError + '" target="_blank">Tải xuống</a>');
                        }
                        break;
                    }
                    base.sctData = [];
                    for (let i = 0; i < data.data.length; i++) {
                        base.sctData.push(data.data[i]);
                    }
                    let formType = base.$inputFileCostEstimateExcelFile.data('formtype');
                    if(formType === 1) {
                        base.renderType1(data.data);
                    } else if(formType === 2) {
                        base.renderType2(data.data);
                    }

                    //todo
                    base.disableOnDoneUpload(true);
                    base.hideUpploadFile(true);
                    base.clearInput(base.$inputFileCostEstimateExcelFile);
                    costJsBase.EventNotify('success', 'Nhập dữ liệu thành công!');
                    break;
                }

                costJsBase.EventNotify('warning', 'Dữ liệu không hợp lệ, vui lòng xem chi tiết phía dưới!');
                for (var i = 0; i < data.errors.length; i++) {
                    let fullErr = data.errors[i].column !== null ? '<b>' + data.errors[i].column + '</b>: ' + data.errors[i].message : data.errors[i].message;
                    base.$errorCollapse.append(fullErr);
                    base.$errorCollapse.append('<br />');
                }
                base.showAndClearErrorBox(true);
                break
            default:
                costJsBase.EventNotify('error', 'Có lỗi xảy ra, tải lên không thành công!');
                break
        }
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

    base.tabType2RecordFuncHandle = function () {
        // prop-type [{xyz}.target.action]
        let propAct = $(this).attr('prop-type-act');
        let propTypes = $(this).attr('prop-type');
        let separators = propTypes.split('.');
        let action = separators[2];
        let point = $(this).closest('tr');
        var record = base.$tableDataTableType2.row(point).data();
        base.recordSelectorIndex = base.$tableDataTableType2.row(point).index();
        base.recordSelector = record;
        //record.id
        //todo: 
        if (typeof (propAct) !== 'undefined') {
            switch (propAct) {
                case "pop":
                    switch (action) {
                        
                        case "edit":

                            let _tr = $(this).closest('tr');
                            _tr.find('td').earch

                            _tr.find('td').each(function (index) {
                                $(this).find('label').css('display', 'none');
                                $(this).find('input').css('display', 'block');
                                $(this).find('select').css('display', 'block');
                            });

                            _tr.find('.btnsave').css('display', 'inline-block');
                            _tr.find('.btnCancel').css('display', 'inline-block');
                            _tr.find('.btnedit').css('display', 'none');
                            _tr.find('.btnremove').css('display', 'none');

                            _tr.find(".MaCP").select2();

                            var _macp = _tr.find(".lblmaCP").text();
                            _tr.find(".MaCP").val(_macp);

                            break;

                        case "cancel":

                            var _trcancel = $(this).closest('tr');
                            _trcancel.find('td').earch

                            _trcancel.find('td').each(function (index) {
                                $(this).find('label').css('display', 'block');
                                $(this).find('input').css('display', 'none');
                                $(this).find('select').css('display', 'none');
                            });

                            _trcancel.find('.btnsave').css('display', 'none');
                            _trcancel.find('.btnCancel').css('display', 'none');
                            _trcancel.find('.btnedit').css('display', 'inline-block');
                            _trcancel.find('.btnremove').css('display', 'inline-block');

                            _trcancel.find(".MaCP").next(".select2-container").hide();

                            break;
                        default:
                            break;
                    }
                    //costJsBase.OpenModal(base.settingPage.pop);
                    break;
                case "confirm":
                    Swal.fire({
                        title: 'Bạn có chắc chắn muốn xóa dịch vụ này?' + '</br> Bản ghi xóa: ' + record.maCP,
                        icon: 'warning',
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
                                Url: '/HDCTV/OnRemoveType2',
                                Data: {
                                    maCP: record.maCP,
                                    subId: record.subId
                                }
                            }).then(data => {
                                if (data.code !== costJsBase.enums.successCode) {
                                    Swal.showValidationMessage(data.message);
                                    deferred.resolve(false);
                                } else {
                                    base.searching();
                                    deferred.resolve(data.message);
                                }
                            }).fail(e => {
                                Swal.showValidationMessage('Lỗi hệ thống, vui lòng thử lại sau!');
                                deferred.resolve(false);
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
                                    timerInterval = setInterval(() => {
                                    },
                                        100);
                                },
                                willClose: () => {
                                    clearInterval(timerInterval)
                                }
                            });
                        }
                    });
                    break;
                case "save":
                    let _tr = $(this).closest('tr');

                    break;
                default:
                    break;
            }
        }
    }

    base.onCreateHD = function (target) {
        costJsBase.Post({
            Url: '/HDCTV/OnCreateHD', Data: {
                SubId: $('#txtmanhom').val(),
                CTVGroupID: $('#hddGroupId').val(),
                SubName: $('#txttenHD').val(),
                FromDate: $('#txtfromdate').val(),
                ToDate: $('#txttodate').val(),
                Note: $('#txtNote').val(),
                IsUse: $('#ddlStatusHD').val(),
                WhyNotUse: $('#txtlydo').val(),
                DisCount: $('#txttyle').val(),
                CustomerPrice: $('#txtGiaKH').val()
            },
            beforeSend: function () {
                costJsBase.ButtonState({
                    target: $(target),
                    state: 'loading',
                    disabled: false,
                    text: 'Đang lưu',
                    changePropAllButton: true
                });
            }
        }, function (data) {
            if (data.code !== costJsBase.enums.successCode) {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
                costJsBase.ButtonState({
                    target: $(target),
                    state: 'normal',
                    disabled: false,
                    html: '<i class="fad fa-plus-circle"></i> Lưu lại',
                    changePropAllButton: false
                });
                costJsBase.ShowValidation(data.listError);
            } else {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
                base.searching();
                setTimeout(function () {
                    base.$popOverlay.modal('hide');
                }, 1000);

                $('#txtmanhom').val(data.subId);
                $('#hddmanhom').val(data.subId);
            }
        }, function () {
            costJsBase.ButtonState({
                target: $(target),
                state: 'normal',
                disabled: false,
                html: '<i class="fad fa-check mr-2" aria-hidden="true"></i> Lưu lại',
                changePropAllButton: true
            });
            costJsBase.EventNotify('error', 'Lỗi hệ thống, vui lòng thử lại sau!')
        })
    }
    
    base.onSaveType1 = function (_tr) {

        var _subId = $('#txtmanhom').val();
        

        costJsBase.PromissePost({
            Url: '/HDCTV/OnCreateType1',
            Data: {
                record: _tr.find('.lblmaCP').text(),
                subId: _subId,
                maCP: _tr.find('.MaCP').val(),
                tenCP: _tr.find('.TenCP').val(),
                bP1: _tr.find('.BP1').val(),
                bP2: _tr.find('.BP2').val(),
                bP3: _tr.find('.BP3').val(),
                bP4: _tr.find('.BP4').val(),
                bP5: _tr.find('.BP5').val(),
                bP6: _tr.find('.BP6').val(),
                bP7: _tr.find('.BP7').val(),
                bP8: _tr.find('.BP8').val(),
                bP9: _tr.find('.BP9').val(),
                bP10: _tr.find('.BP10').val(),
                bP11: _tr.find('.BP11').val(),
                isActive: _tr.find('.ddlStatusSub1').val()
            }
        }).then(data => {
            console.log(1111);
            if (data.code !== costJsBase.enums.successCode) {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);

            } else {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
                base.searching();
                setTimeout(function () {
                    base.$popOverlay.modal('hide');
                }, 1000);
                _tr.find('.btnsave').css('display', 'none');
                _tr.find('.btnedit').css('display', 'block');
            }
        }).fail(e => {
            Swal.showValidationMessage('Lỗi hệ thống, vui lòng thử lại sau!');
            //deferred.resolve(false);
        });
    }

    base.onSaveType2 = function (_tr) {

        var _subId = $('#txtmanhom').val();
        

        costJsBase.PromissePost({
            Url: '/HDCTV/OnCreateType2',
            Data: {
                record: _tr.find('.lblmaCP').text(),
                subId: _subId,
                maCP: _tr.find('.MaCP').val(),
                tenCP: _tr.find('.TenCP').val(),
                fixedPrice: _tr.find('.FixedPrice').val(),
                isActive: _tr.find('.ddlStatusSub2').val()
            }
        }).then(data => {

            if (data.code !== costJsBase.enums.successCode) {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);

            } else {
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
                base.searching();
                setTimeout(function () {
                    base.$popOverlay.modal('hide');
                }, 1000);
                _tr.find('.btnsave').css('display', 'none');
                _tr.find('.btnedit').css('display', 'block');
            }
        }).fail(e => {
            Swal.showValidationMessage('Lỗi hệ thống, vui lòng thử lại sau!');
            //deferred.resolve(false);
        });
    }

}

//define if page use modal

$(document).ready(function () {
    let c = new HDCTVCreate();
    c.Setup();
    $('[data-toggle="tooltip"]').tooltip();

    $("#navinfor").click(function () {
        $("#rowinfor").collapse('toggle');
        $("#rowinforsave").collapse('toggle');
    });

    $(document).on('click', '.btnsave', function () {
        let _tr = $(this).closest('tr');
        if ($("#hddGroupId").val() == "1") {
            c.onSaveType1(_tr)
        }
        else if ($("#hddGroupId").val() == "2") {
            c.onSaveType2(_tr)
        }
    });

    $(document).on('click', '.btnremoveNew', function () {
        let _tr = $(this).closest('tr');
        _tr.remove();
    });

    SelectChanged = function (dropdown) {
        var _value = dropdown.value;
        var _text = dropdown.options[dropdown.selectedIndex].text;
        _text = _text.substring(_value.length + 3);
        var _nexttr = dropdown.closest('td').nextElementSibling;
        _nexttr.firstElementChild.value = _text;
        _nexttr.firstElementChild.nextElementSibling.value = _text;
    }

    $("#txttenHD").change(function () {
        $("#txttenHD").removeClass('itemerror');
    });
    $("#txtNote").change(function () {
        $("#txtNote").removeClass('itemerror');
    });
    $("#txtlydo").change(function () {
        $("#txtlydo").removeClass('itemerror');
    });
    $("#txtfromdate").change(function () {
        $("#txtfromdate").removeClass('itemerror');
    });
    $("#txttodate").change(function () {
        $("#txttodate").removeClass('itemerror');
    });

    $("input[data-type='currency']").on({
        keyup: function () {
            formatCurrency($(this));
        },
        blur: function () {
            formatCurrency($(this), "blur");
        }
    });


    function formatNumber(n) {
        // format number 1000000 to 1,234,567
        return n.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",")
    }


    function formatCurrency(input, blur) {
        // appends $ to value, validates decimal side
        // and puts cursor back in right position.

        // get input value
        var input_val = input.val();

        // don't validate empty input
        if (input_val === "") { return; }

        // original length
        var original_len = input_val.length;

        // initial caret position 
        var caret_pos = input.prop("selectionStart");

        // check for decimal
        if (input_val.indexOf(".") >= 0) {

            // get position of first decimal
            // this prevents multiple decimals from
            // being entered
            var decimal_pos = input_val.indexOf(".");

            // split number by decimal point
            var left_side = input_val.substring(0, decimal_pos);
            var right_side = input_val.substring(decimal_pos);

            // add commas to left side of number
            left_side = formatNumber(left_side);

            // validate right side
            right_side = formatNumber(right_side);

            // On blur make sure 2 numbers after decimal
            if (blur === "blur") {
                right_side += "00";
            }

            // Limit decimal to only 2 digits
            right_side = right_side.substring(0, 2);

            // join number by .
            //input_val = "$" + left_side + "." + right_side;
            input_val = left_side + "." + right_side;

        } else {
            // no decimal entered
            // add commas to number
            // remove all non-digits
            input_val = formatNumber(input_val);
            //input_val = "$" + input_val;

            // final formatting
            //if (blur === "blur") {
            //    input_val += ".00";
            //}
        }

        // send updated string to input
        input.val(input_val);

        // put caret back in the right position
        var updated_len = input_val.length;
        caret_pos = updated_len - original_len + caret_pos;
        input[0].setSelectionRange(caret_pos, caret_pos);
    }

});