var CostEstimateBase = function () {
    let base = this;
    let $tableSelector = $('#manageRequestingCostElement');
    let $dropStats = $('#requestSearchStats');

    base.$tableDataTable = null;
    base.$labelStats = $('#lableStats');
    base.$searchButton = $('#requestSearchButton');
    base.$onLoad = true;
    base.$popOverlay = $('#modal-extra-large');
    //loại : năm - tuần
    base.rqType = $('#___requestFormRequestType');
    base.$selectReportForWeek = $('#selectReportForWeek');
    base.$selectUnits = $('#selectUnits');
    base.$btnSearch = $('#__btnSearch');


    base.recordSelector = null;
    base.recordSelectorIndex = -1;

    base.pageLanguage = languages.vi.costEstimate;

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
                accept: { visible: false },
                decline: { visible: false },
                close: { visible: false }
            },
            dropContent: true
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

    base.settingPopConfirm = {
        propertise: {},

        buttons: {
            left: {
                text: base.pageLanguage.all.cancelButtonText,
                action: function () {
                    base.$popOverlay.show();
                }
            },
            right: {
                text: base.pageLanguage.all.okButtonText,
                class: base.pageLanguage.all.okButtonClass,
                action: function () { }
            }
        },
        // cập nhật thông tin đối tượng
        // theo trạng thái
        updateState: function (changeStatus) {
            this.state = changeStatus;
            this.propertise.large_text = this.state ? base.pageLanguage.approve.largeText : base.pageLanguage.decline.largeText;
            this.propertise.icon = this.state ? base.pageLanguage.approve.icon : base.pageLanguage.decline.icon;
            this.buttons.right.class = this.state ? base.pageLanguage.approve.buttonOkClass : base.pageLanguage.decline.buttonOkClass;
            this.propertise.type = this.state ? base.pageLanguage.approve.popupType : base.pageLanguage.decline.popupType;
            this.propertise.contentHtml = '';
            this.formID = this.state ? '' : costJsBase.Rad(15);

            if (!this.state) {
                this.propertise.contentHtml = `<form id="` + this.formID + `"><div class="form-group text-left">
                        <label class="col-form-label" for="inputError">Lý do từ chối</label>
                        <textarea rows="3" id="___reason" name="___reason" type="text" class="form-control"></textarea>
                  </div></form>`
            }
            this.buttons.right.action = this.state ? this.onApproved : this.onDecline
            this.afterRender = this.state ? null : function () {
                jQuery.fn.validateSetup($('#' + this.formID), {
                    rules: { ___reason: { required: true } },
                    messages: { ___reason: { required: 'Vui lòng nhập lý do từ chối' } }
                });
            };
        },
        // handle nút duyệt
        onApproved: function (e) {
            base.fncApproval(true, e);
        },
        // handle nút từ chối
        onDecline: function (e) {
            let rootSetting = e.data.setting;
            let ok = $('#' + rootSetting.formID).on().valid();
            if (ok) {
                base.fncApproval(false, e);
            }
        },

        //trạng thái của thao tác
        state: false,
        // ID sinh ngẫu nhiên, dùng để validate
        formID: '',
        afterRender: null
    };

    base.Setup = function () {

        // promise -> readURI -> setup handler
        base.readURI().then(function () {
            base.bindActions();
            base.setupDataTable();
            base.$btnSearch.click(base.searching);
        });
    }

    base.searchForm = function (data) {
        if (typeof (data) !== 'undefined') {
            data.Status = $dropStats.find('a[selected]').attr('prop-stats');
            data.ReportForWeek = base.$selectReportForWeek.val();
            data.RequestType = base.rqType.val();
            data.UserUnit = typeof base.selectUnits != 'undefined' ? base.$selectUnits.val() : -100;
            base.setParamsAfterSearch(data);
        }
    }

    base.setupDataTable = function () {
        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            ajax: {
                url: "/CostEstimate/SearchEstimate",
                data: function (d) {
                    base.searchForm(d);
                },
                type: 'post'
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
                        return row.unitName;
                    }
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return row.reportForWeekName || '';
                    },
                    width: "5%"
                },
                {
                    idx: 3,
                    render: function (data, type, row, meta) {
                        return row.userName;
                    },
                    class: "text-center"
                },
                {
                    idx: 4,
                    render: function (data, type, row, meta) {
                        return costJsBase.FormatMoney(row.expectRevenue || 0);
                    },
                    class: "text-right"
                },
                {
                    idx: 5,
                    render: function (data, type, row, meta) {
                        return costJsBase.FormatMoney(row.estimatedCost || 0);
                    },
                    class: "text-right"
                },
                // link PDF
                {
                    idx: 6,
                    render: function (data, type, row, meta) {
                        return '<a class="text-danger" href="' + row.pathPdf + '" target="_blank"><i class="fas fa-file-pdf"></i></a>';
                    },
                    class: "text-center"
                },
                {
                    idx: 7,
                    render: function (data, type, row, meta) {
                        return row.status;
                    },
                    class: "text-center"
                },
                {
                    idx: 8,
                    render: function (data, type, row, meta) {
                        let s = '';
                        if (row.editable)
                            s =
                                '<a class="fs-17 mr-2 text-info" href="javascript:void(0)" title="Chỉnh sửa dự trù" prop-type="elems.table.edit" prop-type-act="redirect" >' +
                                '<i class="fad fa-pencil"></i></a>';
                        if (row.approveAble)
                            s +=
                                '<a class="fs-17 mr-2 text-info" href="javascript:void(0)" title="Phê duyệt dự trù" prop-type="elems.table.approve" prop-type-act="redirect" >' +
                                '<i class="fad fa-check"></i></a>';
                        if (row.viewable) {
                            s +=
                                '<a class="fs-17 text-info" prop-type="elems.table.history" prop-type-act="pop" title="Xem lịch sử" href="javascript:void(0)"> <i class="fal fa-clock"></i></a>';
                        }
                        return s;
                    },
                    class: "text-center"
                }
            ],
            drawCallback: function () {
                base.drawInit();
            },
            scrollX: true,
            //issue: on change resolution --> header not match size with tbody
            responsive: false,
            fixedColumns: {
                right: 2,
                left: 2
            }
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

        base.$searchButton.bind("click",
            function () {
                base.searching();
            });

        base.$selectReportForWeek.select2({ theme: 'bootstrap4', width: '100%' }).change(base.searching);
        base.$selectUnits.select2({ theme: 'bootstrap4', width: '100%' }).change(base.searching);
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
        urlBuilder = $.fn.buildParamURI(urlBuilder, "stats", forms.Status);
        urlBuilder = $.fn.buildParamURI(urlBuilder, "unit", forms.UserUnit);
        urlBuilder = $.fn.buildParamURI(urlBuilder, "w", forms.ReportForWeek);
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

    base.drawInit = () => $('#manageRequestingCostElement [prop-type]').on('click', base.tabRecordFuncHandle);

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
        if (typeof (propAct) !== 'undefined') {
            switch (propAct) {
                case "pop":
                    switch (action) {
                        //xem lịch sử
                        case "history":
                            base.settingPage.fnUPop(base.settingPage.pop.historyLabel);
                            base.settingPage.fnURI('/CostEstimate/ViewHistories?record=' + jQuery.fn.aesToParams(record.record));
                            base.settingPage.fnButtons({
                                //duyệt
                                accept: { visible: false },
                                //từ chối
                                decline: { visible: false },
                                //đóng
                                close: { visible: true }
                            });
                            base.settingPage.overlay = true;
                            break;
                        default:
                            break;
                    }
                    costJsBase.OpenModal(base.settingPage.pop);
                    break;
                case "confirm":
                    let isApprove = action === 'approve';
                    Swal.fire({
                        title: isApprove ? 'Bạn có chắc chắn muốn phê duyệt dự trù này ?' : 'Bạn có chắc chắn muốn từ chối dự trù này ?',
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
                                Url: '/CostEstimate/OnApproval',
                                Data: {
                                    record: record.record,
                                    IsApproval: isApprove
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
                                    clearInterval(timerInterval);
                                }
                            });
                        }
                    });
                    break;
                case "redirect":
                    switch (action) {
                        case "approve":
                            window.location.href = '/CostEstimate/Overview?type=approve&record=' + jQuery.fn.aesToParams(record.record);
                            break;
                        default:
                            window.location.href = '/CostEstimate/Overview?record=' + jQuery.fn.aesToParams(record.record);
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }
    // function duyệt / từ chối yêu cầu
    base.fncApproval = function (val, opt) {
        let config = {
            Url: '/CostEstimate/OnApproval',
            Data: {
                IsApproval: val,
                Reason: $('#___reason').val(),
                Record: base.recordSelector.record,
                Type: base.recordSelector.type
            },
            async: true,
            beforeSend: function () {
                costJsBase.ButtonState({
                    target: $(opt.target),
                    state: 'loading',
                    disabled: true,
                    text: 'Đang lưu',
                    changePropAllButton: true
                });
            },
            complete: function (data) {
                costJsBase.ButtonState({
                    target: $(opt.target),
                    state: data.code === costJsBase.enums.successCode ? 'done' : 'normal',
                    disabled: data.code === costJsBase.enums.successCode,
                    text: data.code === costJsBase.enums.successCode ? '' : base.pageLanguage.all.okButtonText,
                    html: data.code !== costJsBase.enums.successCode ? '' : '<i class="fa fa-check mr-2"></i> Đã lưu',
                    changePropAllButton: true
                }).then(function (target) {
                    if (data.code === costJsBase.enums.successCode) {
                        base.$tableDataTable.row(base.recordSelectorIndex).data(data.data).draw();
                        setTimeout(function () {
                            target.target.closest('.modal').modal('hide');
                            base.$popOverlay.modal('hide');
                        }, 1000);
                    }
                });
            }
        };
        $.extend(config, opt);

        //gọi ajax
        costJsBase.Post(
            config,
            function (data) {
                if (typeof (config.complete) === 'function')
                    config.complete(data);
                costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
            }, function () {
                if (typeof (config.complete) === 'function')
                    config.complete(data);
                costJsBase.EventNotify('error', 'Có lỗi xảy ra, vui lòng thử lại sau!');
            }
        );
        //return false;
    }
}

//define if page use modal

$(document).ready(function () {
    let c = new CostEstimateBase();
    c.Setup();
});