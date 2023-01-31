var CostStatus = function () {
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
    base.$searchSubject = $('#searchSubject');
    base.$searchStatusForCostEstimateType = $('#searchStatusForCostEstimateType');
    base.$searchType = $('#searchType');

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
                $('#grantGroups').select2({ width: '100%', theme: 'bootstrap4' });
                $('#grantUsedGroups').select2({ width: '100%', theme: 'bootstrap4' });
                $('[data-toggle="tooltip"]').tooltip();
            }
        },
        redirect: {

        },
        fnUPop: function (act) {
            this.pop.title = this.pop.baseTitle.replace(this.pop.replator, act);
        },
        fnURI: function (uri) {
            if (typeof (uri) == 'string') {
                this.pop.url = uri;
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
            data.StatusForCostEstimateType = base.$searchStatusForCostEstimateType.val();
            data.StatusForSubject = base.$searchSubject.val();
            data.Type = base.$searchType.val();
            base.setParamsAfterSearch(data);
        }
    }

    base.setupDataTable = function () {
        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            ajax: {
                url: "/CostStatuses/Search",
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
                    width: "10%"
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return row.value;
                    },
                    width: "10%",
                    class: "text-center"
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return row.statusForSubject;
                    },
                    width: "5%",
                    class: "text-center",
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return row.statusForCostEstimateType;
                    },
                    class: "text-center"
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return row.type;
                    },
                    class: "text-center"
                },
                {
                    idx: 3,
                    render: function (data, type, row, meta) {
                        return row.order;
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
                        let s = "";
                        s = '<a class="mr-2" href="javascript:void(0)" title="Chỉnh sửa thông tin trạng thái" prop-type="elems.table.edit" prop-type-act="redirect" ><i class="fad fa-pencil med-theme-primary-button"></i></a>';
                        s += '<a class="text-info mr-2" prop-type="elems.table.stats" prop-type-act="confirm" title="Cập nhật trạng thái" href="javascript:void(0)"><i class="fad fa-repeat-alt"></i></a>';
                        s += '<a class="text-info mr-2" prop-type="elems.table.grant" prop-type-act="pop" title="Gán trạng thái với chức vụ" href="javascript:void(0)"><i class="fad fa-users-cog"></i></a>';
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
        });

        base.$searchButton.bind("click", base.searching);

        $.fn.eEnterActions({
            selector: base.$requestSearchKeywords, action: function (elm) {
                base.searching();
            }
        });

        base.$searchSubject.bind('change', base.searching);
        base.$searchStatusForCostEstimateType.bind('change', base.searching);
        base.$searchType.bind('change', base.searching);
    }

    base.searching = function (reload) {
        if (typeof reload === 'undefined')
            reload = true;
        base.$onLoad = false;
        if (base.$tableDataTable)
            base.$tableDataTable.ajax.reload(null, reload);
    }

    base.setParamsAfterSearch = function (forms) {
        if (base.$onLoad)
            return false;
        let url = URI(window.location.href);
        let urlBuilder = url.toString();
        urlBuilder = $.fn.buildParamURI(urlBuilder, "stats", forms.Status);
        urlBuilder = $.fn.buildParamURI(urlBuilder, "t", forms.Type);
        urlBuilder = $.fn.buildParamURI(urlBuilder, "sj", forms.StatusForSubject);
        urlBuilder = $.fn.buildParamURI(urlBuilder, "ct", forms.StatusForCostEstimateType);
        urlBuilder = $.fn.buildParamURI(urlBuilder, "keywords", forms.Keywords);
        window.history.pushState("", "", urlBuilder);
    }

    base.readURI = function () {
        var deferred = $.Deferred();
        let params = $.fn.dataURI(window.location.href);
        jQuery.fn.setElementValue('#requestSearchStats', params["stats"], function () {
            base.statsChange(params["stats"]);
        });

        jQuery.fn.setElementValue('#searchSubject', params["sj"]);
        jQuery.fn.setElementValue('#searchStatusForCostEstimateType', params["ct"]);
        jQuery.fn.setElementValue('#searchType', params["t"]);
        jQuery.fn.setElementValue('#requestSearchKeywords', params["keywords"]);

        deferred.resolve();
        return deferred.promise();
    }

    base.drawInit = () => $('#tblList [prop-type]').on('click', base.tabRecordFuncHandle);

    base.tabRecordFuncHandle = function () {
        let propAct = $(this).attr('prop-type-act');
        let point = $(this).closest('tr');
        var record = base.$tableDataTable.row(point).data();
        base.recordSelectorIndex = base.$tableDataTable.row(point).index();
        base.recordSelector = record;

        if (typeof (propAct) !== 'undefined') {
            switch (propAct) {
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
                case "pop":
                    base.settingPage.fnUPop('Gán trạng thái cho chức vụ');
                    // cấu hình để tạo các nút trên popup
                    // đăng ký event cho từng nút
                    base.settingPage.fnButtons({
                        //duyệt
                        accept: {
                            visible: true,
                            listener: {
                                event: function () {
                                    base.onGrant(this);
                                }
                            },
                            html: `<button type="button" prop-type="elems.accept" class="btn btn-outline-primary btn-sm">
                                <i class="fad fa-wrench mr-2" aria-hidden="true"></i> Gán quyền
                            </button>`
                        },
                        //đóng
                        close: { visible: true }
                    });
                    base.settingPage.fnURI('/CostStatuses/Grant?record=' + jQuery.fn.aesToParams(record.record));
                    base.settingPage.overlay = true;
                    costJsBase.OpenModal(base.settingPage.pop);
                    break;
                case "redirect":
                    window.location.href = '/CostStatuses/Create?record=' + jQuery.fn.aesToParams(record.record);
                    break;
                default:
                    break;
            }
        }
    }

    base.preFunc = function (v) {
        var deferred = $.Deferred();
        costJsBase.PromissePost({
            Url: '/CostStatuses/ChangeStats',
            Data: v
        }).then(data => {
            if (data.code !== costJsBase.enums.successCode) {
                Swal.showValidationMessage(data.message);
                deferred.resolve(false);
            } else {
                base.searching(false);
                deferred.resolve(data.message);
            }
        }).fail(e => {
            Swal.showValidationMessage('Lỗi hệ thống, vui lòng thử lại sau!');
            deferred.resolve(false);
        });
        return deferred.promise();
    }

    base.onGrant = function (target) {
        costJsBase.Post({
            Url: '/CostStatuses/OnGrant',
            Data: {
                record: $('#___record').val(),
                Grants: $('#grantGroups').val(),
                Used: $('#grantUsedGroups').val()
            },
            beforeSend: function () {
                costJsBase.ButtonState({
                    target: $(target),
                    state: 'loading',
                    disabled: true,
                    text: 'Đang gán quyền ...',
                    changePropAllButton: false
                });
            }
        }, (data) => {
            if (data.code === costJsBase.enums.successCode) {
                setTimeout(function () {
                    base.$popOverlay.modal('hide');
                }, 1500);
                costJsBase.ButtonState({
                    target: $(target),
                    state: 'normal',
                    disabled: false,
                    text: 'Đã gán quyền',
                    changePropAllButton: false
                });
                costJsBase.EventNotify('success', data.message);
            } else {
                costJsBase.ButtonState({
                    target: $(target),
                    state: 'normal',
                    disabled: false,
                    html: '<i class="fad fa-wrench mr-2" aria-hidden="true"></i> Gán quyền',
                    changePropAllButton: false
                });
                costJsBase.EventNotify('error', data.message);
            }
        }, (err) => {
            costJsBase.ButtonState({
                target: $(target),
                state: 'normal',
                disabled: false,
                html: '<i class="fad fa-wrench mr-2" aria-hidden="true"></i> Gán quyền',
                changePropAllButton: false
            });
            costJsBase.EventNotify('error', 'Lỗi hệ thống, vui lòng thử lại sau!');
        });
    }
}

//define if page use modal

$(document).ready(function () {
    let c = new CostStatus();
    c.Setup();
    $('[data-toggle="tooltip"]').tooltip();
});