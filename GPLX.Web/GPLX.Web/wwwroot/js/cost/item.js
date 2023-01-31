var CostElmItemBase = function () {
    let base = this;
    let $tableSelector = $('#manageRequestingCostElementItem');
    let $dropStats = $('#requestSearchStats');
    let $ipKeywords = $('#requestSearchKeywords');
    let $ipDateRange = $('#requestSearchDateRange');
    base.$tableDataTable = null;
    base.$labelStats = $('#lableStats');
    base.$searchButton = $('#requestSearchButton');
    base.$onLoad = true;
    base.$popOverlay = $('#modal-overlay');
    //loại : năm - tuần
    base.rqType = $('#___requestFormRequestType');
    base.ckcAll = $('#ckcAll');
    base.btnApproveMultiple = $('#btnApproveMultiple');
    base.ddlWeek = $('#ddlWeek');
    base.btnDeclineMultiple = $('#btnDeclineMultiple');

    base.recordSelector = null;
    base.recordSelectorIndex = -1;

    base.ckcRecords = [];

    base.pageLanguage = languages.vi.costElementItem;

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
                this.pop.url = uri
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
                this.propertise.contentHtml = `<form id="` +
                    this.formID +
                    `"><div class="form-group text-left">
                        <label class="col-form-label" for="inputError">Lý do từ chối</label>
                        <textarea rows="3" id="___reason" name="___reason" type="text" class="form-control"></textarea>
                  </div></form>`;
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
            base.setupElemsHandle();
            base.bindActions();
            base.setupDataTable();
        });
    }

    base.searchForm = function (data) {
        if (typeof (data) !== 'undefined') {
            data.Keywords = $ipKeywords.val().trim();
            data.Status = $dropStats.find('a[selected]').attr('prop-stats');
            data.FilterWeek = base.ddlWeek.val();
            //let dateData = $ipDateRange.data('daterangepicker');
            //data.FromDate = dateData.startDate.format(base.pageLanguage.all.dateFormat);
            //data.ToDate = dateData.endDate.format(base.pageLanguage.all.dateFormat);


            data.RequestType = base.rqType.val();

            base.setParamsAfterSearch(data);
        }
    }

    base.setupDataTable = function () {
        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            ajax: {
                url: "/CostEstimateItem/Search",
                type: 'post',
                data: function (d) {
                    base.searchForm(d);
                }
            },
            columns: [
                {
                    idx: 0,
                    render: function (data, type, row, meta) {
                        return `<div class="form-check">
                                  <input class="form-check-input position-static" ckcElement type="checkbox" aria-label="...">
                                </div>`;
                        // return meta.row + meta.settings._iDisplayStart + 1;
                    },
                    width: "2%",
                    class: "text-center"
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return row.requestCode || '';
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return row.unitName;
                    },
                    width: "5%"
                },
                {
                    idx: 3,
                    render: function (data, type, row, meta) {
                        return row.departmentName;
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 4,
                    render: function (data, type, row, meta) {
                        return row.requestContent;
                    }
                },
                {
                    idx: 5,
                    render: function (data, type, row, meta) {
                        return costJsBase.FormatMoney(row.cost);
                    },
                    width: "8%",
                    class: "text-right"
                },
                {
                    idx: 6,
                    render: function (data, type, row, meta) {
                        return row.createdDate;
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 7,
                    render: function (data, type, row, meta) {
                        return row.creatorName;
                    },
                    width: "5%",
                    class: "text-center"
                },

                {
                    idx: 8,
                    render: function (data, type, row, meta) {
                        return row.payWeekName;
                    },
                    width: "12%"
                },
                {
                    idx: 9,
                    render: function (data, type, row, meta) {
                        return row.statusName;
                    },
                    width: "5%",
                    class: "text-center"
                },
                {
                    idx: 10,
                    render: function (data, type, row, meta) {
                        let btmHtml = ``;
                        if (typeof (row.editable) !== 'undefined' && row.editable) {
                            btmHtml +=
                                `<a type="button" prop-type="elems.table.edit" prop-type-act="redirect" class="fs-17 text-info mr-2" title="` +
                                base.pageLanguage.list.table.editLabel +
                                `">
                                    <i class="fad fa-pencil"></i>
                                </a>`;
                        }
                        if (typeof (row.approvalable) !== 'undefined' && row.approvalable || typeof (row.viewable) !== 'undefined' && row.viewable) {
                            btmHtml +=
                                `<a type="button" prop-type="elems.table.info" prop-type-act="pop" class="fs-17  text-info mr-2" title="` +
                                base.pageLanguage.list.table.viewLabel +
                                `">
                                    ` +
                                (row.approvalable
                                    ? ' <i class="fas fa-check"></i>'
                                    : ' <i class="fas fa-info-square"></i>') +
                                `
                                </a>`;
                        }
                        if (typeof (row.deleteable) !== 'undefined' && row.deleteable) {
                            btmHtml +=
                                `<a type="button" prop-type="elems.table.delete" prop-type-act="pop" class="fs-17  text-danger mr-2" title="` +
                                base.pageLanguage.list.table.deleteLabel +
                                `"><i class="fas fa-times"></i></a>`;
                        }
                        if (typeof (row.viewable) !== 'undefined' && row.viewable) {
                            btmHtml +=
                                `<a type="button" prop-type="elems.table.history" prop-type-act="pop" class="fs-17  text-info" title="` +
                                base.pageLanguage.list.table.viewHistoryLabel +
                                `">
                                           <i class="fal fa-clock"></i>
                                        </a>`;
                        }

                        return btmHtml;
                    },
                    width: "10%",
                    class: "text-center"
                }
            ],
            drawCallback: function () {
                $('input[ckcElement]').change(function () {
                    let point = $(this).closest('tr');
                    var rc = base.$tableDataTable.row(point).data();
                    base.onRecordSelect(rc, $(this).is(':checked'));
                });
                base.drawInit();
            },
            fixedHeader: true,
            scrollX: true,
            responsive: false,
            fixedColumns: {
                right: 1
            }
        });
    }

    base.setupElemsHandle = function () {
        $.fn.viRangeDateRegister({
            selector: $ipDateRange,
            callbackOnApply: function (ev, picker) {
                base.searching();
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
        $dropStats.find('a').bind("click",
            function (e) {
                base.statsChange(this).then(base.searching);
            });

        $.fn.eEnterActions({
            selector: $ipKeywords,
            action: function (elm) {
                base.searching();
            }
        });

        base.ckcAll.change(function () {
            let isCheck = base.ckcAll.is(":checked");
            let rowsInPage = base.$tableDataTable.rows({ page: 'current' }).data();
            let nodesInPage = base.$tableDataTable.rows({ page: 'current' }).nodes();
            for (var i = 0; i < rowsInPage.length; i++) {
                if (isCheck) {
                    base.onRecordSelect(rowsInPage[i], true);
                    $(nodesInPage[i]).find('input[type="checkbox"]').prop("checked", true);
                } else {
                    base.onRecordSelect(rowsInPage[i], false);
                    $(nodesInPage[i]).find('input[type="checkbox"]').prop("checked", false);
                }
            }
        });

        base.$searchButton.bind("click",
            function () {
                base.searching();
            });

        base.btnApproveMultiple.click(function () {
            Swal.fire({
                title: 'Bạn có muốn phê duyệt các yêu cầu này ?',
                icon: 'success',
                showDenyButton: false,
                showConfirmButton: true,
                showCancelButton: true,
                confirmButtonText: `<i class="fad fa-check"></i> Phê duyệt`,
                cancelButtonText: 'Hủy bỏ',
                showLoaderOnConfirm: true,
                position: 'top',
                preConfirm: function () {
                    var deferred = $.Deferred();
                    base.fncMulti(true).then(data => {
                        if (data.code !== costJsBase.enums.successCode) {
                            Swal.showValidationMessage(data.message);
                            deferred.resolve(false);
                        } else {
                            base.searching();
                            deferred.resolve(data.message);
                        }
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
                            timerInterval = setInterval(() => { }, 100);
                        },
                        willClose: () => {
                            clearInterval(timerInterval);
                        }
                    });
                }
            });
        });

        base.btnDeclineMultiple.click(function () {
            if (!base.ckcRecords.length) {
                costJsBase.EventNotify('warning', 'Bạn chưa chọn yêu cầu cần từ chối!');
                return false;
            }
            let formId = costJsBase.Rad(15);
            Swal.fire({
                title: 'Bạn có muốn từ chối các yêu cầu đã chọn ?',
                icon: 'error',
                showDenyButton: true,
                showConfirmButton: false,
                showCancelButton: true,
                denyButtonText: `<i class="fad fa-ban"></i> Từ chối`,
                cancelButtonText: 'Hủy bỏ',
                position: 'top',
                showLoaderOnDeny: true,
                didOpen: () => {
                    jQuery.fn.validateSetup($('#' + formId),
                        {
                            rules: { ___reason: { required: true } },
                            messages: { ___reason: { required: 'Vui lòng nhập lý do từ chối' } }
                        });
                },
                preDeny: function () {
                    let formValid = $('#' + formId).on().valid();
                    if (formValid) {
                        var deferred = $.Deferred();
                        base.fncMulti(false).then(data => {
                            if (data.code !== costJsBase.enums.successCode) {
                                Swal.showValidationMessage(data.message);
                                deferred.resolve(false);
                            } else {
                                base.searching();
                                deferred.resolve(data.message);
                            }
                        });
                        return deferred.promise();
                    } else
                        return false;
                },
                html: `<form id="` +
                    formId +
                    `"><div class="form-group text-left">
                                                <label class="col-form-label" for="inputError">Lý do từ chối</label>
                                                <textarea rows="3" id="___reason" name="___reason" type="text" class="form-control"></textarea>
                                          </div>
                                      </form>`
            }).then((result) => {
                if (result.isDenied) {
                    Swal.fire({
                        title: result.value,
                        icon: 'success',
                        timer: 1500,
                        timerProgressBar: true,
                        didOpen: () => {
                            Swal.showLoading();
                            timerInterval = setInterval(() => { }, 100);
                        },
                        willClose: () => {
                            clearInterval(timerInterval);
                        }
                    });
                }
            });
        });

        base.ddlWeek.select2({ 'width': '100%', themes: 'boostrap4' }).change(function () {
            base.searching();
        });
    }

    base.onRecordSelect = function (rc, add) {
        if (add) {
            base.ckcRecords.push(rc);
        } else {
            base.ckcRecords = $.grep(base.ckcRecords, function (value) {
                return value !== rc;
            });
        }
        let enableMultipleAct = base.ckcRecords.length > 0;
        base.ckcRecords.forEach(x => {
            if (!x.approvalable) {
                enableMultipleAct = false;
            }
        });

        let pLength = base.$tableDataTable.rows({ page: 'current' }).data().length;

        base.ckcAll.prop('checked', base.ckcRecords.length === pLength);

        base.btnApproveMultiple.prop('disabled', !enableMultipleAct);
        base.btnDeclineMultiple.prop('disabled', !enableMultipleAct);
    }

    base.searching = function () {
        base.$onLoad = false;
        if (base.$tableDataTable) {
            base.$tableDataTable.ajax.reload();
            base.ckcRecords = [];
            base.btnDeclineMultiple.prop('disabled', true);
            base.btnApproveMultiple.prop('disabled', true);
        }
    }

    base.setParamsAfterSearch = function (forms) {
        if (base.$onLoad)
            return false;
        let url = URI(window.location.href);
        let urlBuilder = url.toString();
        urlBuilder = $.fn.buildParamURI(urlBuilder, "keywords", forms.Keywords);
        urlBuilder = $.fn.buildParamURI(urlBuilder, "stats", forms.Status);
        //urlBuilder = $.fn.buildParamURI(urlBuilder, "from", forms.FromDate);
        //urlBuilder = $.fn.buildParamURI(urlBuilder, "to", forms.ToDate);
        urlBuilder = $.fn.buildParamURI(urlBuilder, "w", forms.FilterWeek);
        window.history.pushState("", "", urlBuilder);
    }

    base.readURI = function () {
        var deferred = $.Deferred();
        let params = $.fn.dataURI(window.location.href);
        jQuery.fn.setElementValue('#requestSearchKeywords', params["keywords"]);
        jQuery.fn.setElementValue('#ddlWeek', params["w"]);
        jQuery.fn.setElementValue('#requestSearchStats', params["stats"], function () {
            base.statsChange(params["stats"]);
        });

        //let fromDate = params['from'];
        //let toDate = params['to'];

        //if (typeof (fromDate) !== 'undefined' && typeof (toDate) !== 'undefined') {
        //    $ipDateRange.val(fromDate + ' - ' + toDate);
        //}

        deferred.resolve();
        return deferred.promise();
    }

    base.drawInit = () => $('#manageRequestingCostElementItem [prop-type]').on('click', base.tabRecordFuncHandle);

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
        let openPop = true;

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
                                //duyệt
                                accept: {
                                    visible: record.approvalable, listener: {
                                        event: function () {
                                            $(this).closest('.modal').hide();
                                            base.settingPopConfirm.updateState(true);
                                            costJsBase.ConfirmPopup(base.settingPopConfirm);
                                        }
                                    }
                                },
                                //từ chối
                                decline: {
                                    visible: record.declineable, listener: {
                                        event: function () {
                                            $(this).closest('.modal').hide();
                                            base.settingPopConfirm.updateState(false);
                                            costJsBase.ConfirmPopup(base.settingPopConfirm);
                                        }
                                    }
                                },
                                //đóng
                                close: { visible: true }
                            });
                            base.settingPage.fnURI('/CostEstimateItem/Create?partial=true&viewMode=view&record=' + jQuery.fn.aesToParams(record.id));
                            base.settingPage.overlay = true;
                            break;
                        //xem lịch sử
                        case "history":
                            base.settingPage.fnUPop(base.settingPage.pop.historyLabel);
                            base.settingPage.fnURI('/CostEstimateItem/ViewHistory?record=' + jQuery.fn.aesToParams(record.id));
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
                        // xóa yêu cầu
                        case "delete":
                            openPop = false;
                            Swal.fire({
                                title: 'Bạn có chắc chắn muốn xóa yêu cầu này ?',
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
                                        Url: '/CostEstimateItem/Delete',
                                        Data: {
                                            CostEstimateId: record.id,
                                        }
                                    }).then(data => {
                                        if (data.code !== costJsBase.enums.successCode) {
                                            Swal.showValidationMessage(data.message);
                                            deferred.resolve(false);
                                        } else {
                                            base.$tableDataTable.row(point).remove().draw(false);
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
                                            timerInterval = setInterval(() => { }, 100);
                                        },
                                        willClose: () => {
                                            clearInterval(timerInterval);
                                        }
                                    });
                                }
                            });
                            break;
                        default:
                            break;
                    }
                    if (openPop)
                        costJsBase.OpenModal(base.settingPage.pop);
                    break;
                case "redirect":
                    window.location.href = '/CostEstimateItem/Create?partial=false&record=' + jQuery.fn.aesToParams(record.id);
                    break;
                default:
                    break;
            }
        }
    }
    // function duyệt / từ chối yêu cầu
    base.fncApproval = function (val, opt) {
        let config = {
            Url: '/CostEstimateItem/Approval',
            Data: {
                IsApproval: val,
                Reason: $('#___reason').val(),
                CostEstimateId: base.recordSelector.id,
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
                costJsBase.ButtonState({
                    target: $(opt.target),
                    state: 'normal',
                    disabled: false,
                    text: base.pageLanguage.all.okButtonText,
                    changePropAllButton: false
                });
                costJsBase.EventNotify('error', 'Có lỗi xảy ra, vui lòng thử lại sau!');
            }
        );
        //return false;
    }

    base.fncMulti = function (val) {
        let ids = [];
        base.ckcRecords.forEach(c => {
            ids.push(c.id);
        });
        var deferred = $.Deferred();
        let config = {
            Url: '/CostEstimateItem/Approval',
            Data: {
                IsApproval: val,
                Reason: $('#___reason').val(),
                Records: ids
            },
            async: false,
            beforeSend: function () { },
        };

        //gọi ajax
        costJsBase.PromissePost(config).done(data => {
            deferred.resolve(data);
        }).fail(err => {
            deferred.resolve(false);
        });
        return deferred.promise();
    }
}

//define if page use modal

$(document).ready(function () {
    let c = new CostElmItemBase();
    c.Setup();
});