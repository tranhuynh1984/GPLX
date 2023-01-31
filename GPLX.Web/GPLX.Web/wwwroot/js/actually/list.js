var ActualleyList = function () {
    let base = this;
    let $tableSelector = $('#manageActuallyList');
    let $dropStats = $('#searchStats');
    let $ipKeywords = $('#requestSearchKeywords');
    base.$tableDataTable = null;
    base.$labelStats = $('#lableStats');
    base.$searchButton = $('#requestSearchButton');
    base.$onLoad = true;
    base.$popOverlay = $('#modal-extra-large');
    base.rqType = $('#___requestFormRequestType');
    base.ddlWeek = $('#ddlWeek');

    base.recordSelector = null;
    base.recordSelectorIndex = -1;

    base.pageLanguage = languages.vi.actually;

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
                    //base.$popOverlay.modal('show');
                    //todo: issue - khi show thì thanh scrollbar ở phía dưới xuất hiện
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

    // set to elements
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
            data.RequestType = base.rqType.val();
            base.setParamsAfterSearch(data);
        } 
    }

    base.setupDataTable = function () {
        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            ajax: {
                url: "/ActuallySpent/Search",
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
                    width: "5%",
                },
                {
                    idx: 1,
                    render: function (data, type, row, meta) {
                        return row.reportForWeekName;
                    },
                    width: "10%"
                },
                {
                    idx: 2,
                    render: function (data, type, row, meta) {
                        return costJsBase.FormatMoney(row.totalEstimateCost);
                    },
                    class: "text-right"
                },
                {
                    idx: 3,
                    render: function (data, type, row, meta) {
                        return costJsBase.FormatMoney(row.totalActuallySpent);
                    },
                    class: "text-right"
                },
                {
                    idx: 4,
                    render: function (data, type, row, meta) {
                        return costJsBase.FormatMoney(row.totalAmountLeft);
                    },
                    class: "text-right"
                },
                {
                    idx: 5,
                    render: function (data, type, row, meta) {
                        return costJsBase.FormatMoney(row.totalActualSpentAtTime);
                    },
                    class: "text-right"
                },
                {
                    idx: 6,
                    render: function (data, type, row, meta) {
                        return row.status;
                    },
                    width: "10%",
                    class: "text-center"
                },
                {
                    idx: 7,
                    render: function (data, type, row, meta) {
                        let btmHtml = ``;
                        if (typeof (row.editable) !== 'undefined' && row.editable) {
                            btmHtml += base.createButton(row.record, 'edit', base.pageLanguage.list.table.editLabel, `<i class="fad fa-pencil"></i>`);
                        }
                        if (row.viewable || row.approvalable) {
                            btmHtml += `<a href="javascript: void(0);"  prop-type="elems.table.info" prop-type-act="pop" class="fs-17 text-info mr-2" title="` + base.pageLanguage.list.table.viewLabel + `">
                                `+ (row.approvalable ? '<i class="fas fa-check"></i>' : '<i class="fas fa-info-square"></i>') + `
                            </a>`;
                        }

                        if (typeof (row.viewable) !== 'undefined' && row.viewable) {
                            btmHtml +=
                                `<a type="button" prop-type="elems.table.history" prop-type-act="pop" class="fs-17 text-info" title="` +
                                base.pageLanguage.list.table.viewHistoryLabel +
                                `"> <i class="fal fa-clock"></i> </a>`;
                        }
                        return btmHtml;
                    },
                    width: "10%",
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
                right: 1
            },
        });
    }

    base.setupElemsHandle = function () {
        
    }

    base.createButton = function (record, mode, label, icon, className) {
        return `<a type="button" target="_blank" href="/ActuallySpent/Create?viewMode=` + mode + `&record=` + jQuery.fn.aesToParams(record) + `" class="text-info fs-17 mr-2 ` + className + `" title="` + label + `">` + icon + `</a>`;
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

        $.fn.eEnterActions({
            selector: $ipKeywords, action: function (elm) {
                base.searching();
            }
        });

        base.$searchButton.bind("click",
            function() {
                base.searching();
            });
        base.ddlWeek.select2({ 'width': '100%', theme: 'bootstrap4' }).change(function() {
            base.searching();
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
        urlBuilder = $.fn.buildParamURI(urlBuilder, "keywords", forms.Keywords);
        urlBuilder = $.fn.buildParamURI(urlBuilder, "stats", forms.Status);
        urlBuilder = $.fn.buildParamURI(urlBuilder, "w", forms.FilterWeek);
        //window.location. = urlBuilder;
        window.history.pushState("", "", urlBuilder);
    }

    base.readURI = function () {
        var deferred = $.Deferred();
        let params = $.fn.dataURI(window.location.href);
        jQuery.fn.setElementValue('#requestSearchKeywords', params["keywords"]);
        jQuery.fn.setElementValue('#searchStats', params["stats"], function () {
            base.statsChange(params["stats"]);
        });
        jQuery.fn.setElementValue('#ddlWeek', params["w"]);

        deferred.resolve();
        return deferred.promise();
    }

    base.drawInit = () => $('#manageActuallyList [prop-type]').on('click', base.tabRecordFuncHandle);

    base.tabRecordFuncHandle = function () {
        let propAct = $(this).attr('prop-type-act');
        let propTypes = $(this).attr('prop-type');
        let separators = propTypes.split('.');
        let action = separators[2];
        let point = $(this).closest('tr');
        var record = base.$tableDataTable.row(point).data();
        base.recordSelectorIndex = base.$tableDataTable.row(point).index();
        base.recordSelector = record;
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
                                            costJsBase.ConfirmPopup(base.settingPopConfirm)
                                        }
                                    }
                                },
                                //đóng
                                close: { visible: true }
                            });
                            base.settingPage.fnURI('/ActuallySpent/Create?partial=true&viewMode=view&record=' + jQuery.fn.aesToParams(record.record));
                            base.settingPage.overlay = true;
                            break
                        case "history":
                            base.settingPage.fnUPop('Xem lịch sử phê duyệt');
                            base.settingPage.fnURI('/ActuallySpent/ViewHistories?record=' + jQuery.fn.aesToParams(record.record));
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
                default:
                    break;
            }
        }
    }
    // function duyệt / từ chối yêu cầu
    base.fncApproval = function (val, opt) {
        let config = {
            Url: '/ActuallySpent/Approval',
            Data: {
                IsApproval: val,
                Reason: $('#___reason').val(),
                Record: base.recordSelector.record
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
    let c = new ActualleyList();
    c.Setup();
});