var CostEstimateBase = function () {
    let base = this;
    let $tableSelector = $('#tblList');
    let $dropStats = $('#requestSearchStats');

    base.$tableDataTable = null;
    base.$labelStats = $('#lableStats');
    base.$searchButton = $('#requestSearchButton');
    base.$requestSearchKeywords = $('#requestSearchKeywords');
    base.$onLoad = true;
    base.$popOverlay = $('#modal-extra-large');
    base.$btnCreate = $('#btnCreate');
    //loại : năm - tuần
    base.$selectReportForWeek = $('#selectReportForWeek');
    base.$createForm = $('#___createForm');

    base.recordSelector = null;

    base.recordSelectorIndex = -1;

    base.pageLanguage = languages.vi.groups;

    base.settingPage = {
        pop: {
            url: '',
            title: base.pageLanguage.list.popup.title,
            baseTitle: base.pageLanguage.list.popup.baseTitle,
            viewLabel: base.pageLanguage.list.popup.viewLabel,
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
                $('[data-toggle="tooltip"]').tooltip();

                jQuery.fn.validateSetup(base.$createForm,
                    {
                        rules: {
                            groupName: { required: true },
                            groupCode: { required: true, codeFormat: true },
                            order: { required: true, min: 0, number: true}
                        },
                        messages: {
                            groupName: { required: 'Bạn chưa nhập tên chức vụ' },
                            groupCode: { required: 'Bạn vui lòng nhập mã chức vụ', codeFormat: 'Mã chức vụ không đúng định dạng' },
                            order: { required: 'Bạn chưa nhập thứ tự cho nhóm chức vụ', min: 'Số thứ tự tối thiểu là 0', number: 'Thứ tự bắt buộc là định dạng số' }
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

        // promise -> readURI -> setup handler
        base.readURI().then(function () {
            base.bindActions();
            base.setupDataTable();
        });
    }

    base.searchForm = function (data) {
        if (typeof (data) !== 'undefined') {
            data.Status = $dropStats.find('a[selected]').attr('prop-stats');
            data.Keywords = base.$requestSearchKeywords.val();
            base.setParamsAfterSearch(data);
        } else {
            //load from URI
        }
    }

    base.setupDataTable = function () {

        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            ajax: {
                url: "/Groups/Search",
                data: function (d) {
                    base.searchForm(d);
                }
            },
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
                        return row.name;
                    },
                    width: "25%"
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return row.groupCode;
                    },
                    width: "10%",
                    class: "text-center"
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return row.order;
                    },
                    width: "5%",
                    class: "text-center",
                    data: 'order'
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return row.creatorName;
                    },
                    class: "text-center"
                },
                {
                    idx: 3,
                    render: function (data, type, row, meta) {
                        return moment(row.createdDate).format('DD/MM/YYYY')
                    },
                    class: "text-center"
                },
                {
                    idx: 4,
                    render: function (data, type, row, meta) {
                        return row.statusName;
                    },
                    width: "10%",
                    class: "text-center"
                },
                {
                    idx: 5,
                    render: function (data, type, row, meta) {
                        let s = '';
                        s =
                            '<a class="mr-2 fs-17" href="javascript:void(0)" title="Chỉnh sửa thông tin chức vụ" prop-type="elems.table.edit" prop-type-act="pop" ><i class="fad fa-pencil med-theme-primary-button"></i></a>';
                        s +=
                            '<a class="text-info mr-2 fs-17" prop-type="elems.table.info" prop-type-act="pop" title="Chi tiết chức vụ" href="javascript:void(0)"><i class="fal fa-info-square"></i></a>';
                        s +=
                            '<a class="text-info mr-2 fs-17" prop-type="elems.table.delete" prop-type-act="confirm" title="Thay đổi trạng thái" href="javascript:void(0)"><i class="fas fa-recycle"></i></a>';
                        s +=
                            '<a class="text-info mr-2 fs-17" prop-type="elems.table.grant" prop-type-act="redirect" title="Phân quyền" href="javascript:void(0)"><i class="far fa-user-cog"></i></a>';
                        return s;
                    },
                    class: "text-center"
                }
            ],
            drawCallback: function () {
                base.drawInit();
            },
            scrollX: true,
            responsive: false,
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
        $dropStats.find('a').bind("click", function (e) {
            base.statsChange(this).then(base.searching);
        });

        base.$searchButton.bind("click", function () {
            base.searching();
        });

        base.$btnCreate.bind("click", function () {
            base.settingPage.fnUPop(base.settingPage.pop.viewLabel);
            // cấu hình để tạo các nút trên popup
            // đăng ký event cho từng nút
            base.settingPage.fnButtons({
                //duyệt
                accept: {
                    visible: true, listener: {
                        event: function () {
                            if (base.$createForm.on().valid())
                                base.onCreate(this);
                        }
                    },
                    html: `<button type="button" prop-type="elems.accept" class="btn btn-outline-primary btn-sm">
                                <i class="fad fa-check mr-2" aria-hidden="true"></i> Lưu lại
                            </button>`,
                },
                //đóng
                close: { visible: true }
            });
            base.settingPage.fnURI('/Groups/Create');
            base.settingPage.overlay = true;
            costJsBase.OpenModal(base.settingPage.pop);
        })

        $.fn.eEnterActions({
            selector: base.$requestSearchKeywords, action: function (elm) {
                base.searching();
            }
        });
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

    base.tabRecordFuncHandle = function () {
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
                        // xem chi tiết
                        case "info":
                            base.settingPage.fnUPop(base.settingPage.pop.viewLabel);
                            // cấu hình để tạo các nút trên popup
                            // đăng ký event cho từng nút
                            base.settingPage.fnButtons({
                                close: { visible: true }
                            });
                            base.settingPage.fnURI('/Groups/Create?viewMode=view&record=' + jQuery.fn.aesToParams(record.record));
                            base.settingPage.overlay = true;
                            break;
                        case "edit":
                            base.settingPage.fnUPop(base.settingPage.pop.editLabel);
                            // cấu hình để tạo các nút trên popup
                            // đăng ký event cho từng nút
                            base.settingPage.fnButtons({
                                //duyệt
                                accept: {
                                    visible: true, listener: {
                                        event: function () {
                                            if (base.$createForm.on().valid())
                                                base.onCreate(this);
                                        }
                                    },
                                    html: `<button type="button" prop-type="elems.accept" class="btn btn-outline-primary btn-sm">
                                                <i class="fad fa-check mr-2" aria-hidden="true"></i> Lưu lại
                                            </button>`,
                                },
                                //đóng
                                close: { visible: true }
                            });
                            base.settingPage.fnURI('/Groups/Create?viewMode=edit&record=' + jQuery.fn.aesToParams(record.record));
                            base.settingPage.overlay = true;
                            break;
                        default:
                            break;
                    }
                    costJsBase.OpenModal(base.settingPage.pop);
                    break;
                case "confirm":
                    Swal.fire({
                        title: 'Chọn trạng thái bạn muốn thay đổi !',
                        icon: 'warning',
                        showDenyButton: true,
                        showConfirmButton: true,
                        showCancelButton: true,
                        confirmButtonText: `<i class="fad fa-check"></i> Kích hoạt`,
                        denyButtonText: `<i class="fad fa-check"></i> Vô hiệu`,
                        cancelButtonText: 'Hủy bỏ',
                        position: 'top',
                        showLoaderOnConfirm: true,
                        showLoaderOnDeny: true,
                        didOpen: () => { },
                        preConfirm: function () {
                            var deferred = $.Deferred();
                            base.preFunc({ record: record.record, status: 1 }).then((data) => {
                                deferred.resolve(data);
                            });
                            return deferred.promise();
                        },
                        preDeny: function () {
                            var deferred = $.Deferred();
                            base.preFunc({ record: record.record, status: 0 }).then((data) => {
                                deferred.resolve(data);
                            });;
                            return deferred.promise();
                        }
                    }).then((result) => {
                        if (result.isConfirmed || result.isDenied) {
                            Swal.fire({
                                title: result.value,
                                icon: 'success',
                                timer: 1500,
                                timerProgressBar: true,
                                didOpen: () => {
                                    Swal.showLoading();
                                    timerInterval = setInterval(() => {
                                    }, 100);
                                },
                                willClose: () => {
                                    clearInterval(timerInterval);
                                }
                            });
                        }
                    });
                    break;
                case "redirect":
                    window.location.href = '/Permission/GrantTree?record=' + jQuery.fn.aesToParams(record.record);
                    break;
                default:
                    break;
            }
        }
    }
    // function duyệt / từ chối yêu cầu

    base.onCreate = function (target) {
        costJsBase.Post({
            Url: '/Groups/OnCreate', Data: {
                Record: $('#___record').val(),
                Name: $('#groupName').val(),
                GroupCode: $('#groupCode').val(),
                Order: $('#order').val()
            },
            beforeSend: function () {
                costJsBase.ButtonState({
                    target: $(target),
                    state: 'loading',
                    disabled: true,
                    text: 'Đang lưu',
                    changePropAllButton: true
                });
            }
        }, function (data) {
            if (data.code === costJsBase.enums.successCode) {
                costJsBase.EventNotify('success', data.message)
                base.searching();
                setTimeout(function () {
                    base.$popOverlay.modal('hide');
                }, 1000);
            } else {
                costJsBase.ButtonState({
                    target: $(target),
                    state: 'normal',
                    disabled: false,
                    html: '<i class="fad fa-check mr-2" aria-hidden="true"></i> Lưu lại',
                    changePropAllButton: true
                });
                costJsBase.EventNotify('error', data.message)
            }
        }, function () {
            costJsBase.EventNotify('error', 'Lỗi hệ thống, vui lòng thử lại sau!')
            costJsBase.ButtonState({
                target: $(target),
                state: 'normal',
                disabled: false,
                html: '<i class="fad fa-check mr-2" aria-hidden="true"></i> Lưu lại',
                changePropAllButton: true
            });
        })
    }

    base.preFunc = function (v) {
        var deferred = $.Deferred();
        costJsBase.PromissePost({
            Url: '/Groups/OffGroups',
            Data: v
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
}

//define if page use modal

$(document).ready(function () {
    let c = new CostEstimateBase();
    c.Setup();
    $('[data-toggle="tooltip"]').tooltip();
});