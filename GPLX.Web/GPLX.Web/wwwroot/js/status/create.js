var CostStatusCreate = function () {
    var base = this;

    base.$inputName = $('#inputName');
    base.$inputValue = $('#inputValue');
    base.$selectSubject = $('#selectSubject');
    base.$selectStatusForCostEstimateType = $('#selectStatusForCostEstimateType');
    base.$selectTypes = $('#selectTypes');
    base.$inputOrder = $('#inputOrder');
    base.$checkApprove = $('#checkApprove');
    base.$checkSigned = $('#checkSigned');
    base.$createForm = $('#___createForm');
    base.$btnSave = $('#btnSave');
    base.$record = $('#___record');

    base.onCreate = function () {
        if (base.$createForm.valid()) {
            costJsBase.PromissePost({
                Url: '/CostStatuses/OnCreate',
                Data: base.formPost(),
                beforeSend: function () {
                    costJsBase.ButtonState({
                        target: base.$btnSave,
                        state: 'loading',
                        disabled: true,
                        text: 'Đang lưu lại ...',
                        changePropAllButton: true
                    });
                }
            }).then((data) => {
                if (data.code === costJsBase.enums.successCode) {
                    var fp = base.formPost();
                    costJsBase.EventNotify('success', data.message);
                    setTimeout(function () {
                        window.location.href = '/CostStatuses/List?t=' + fp.Type + '&sj=' + fp.StatusForSubject + '&ct=' + fp.StatusForCostEstimateType + '';
                    }, 1500);
                } else {
                    base.offLoading();
                    costJsBase.EventNotify('error', data.message);
                }
            }).fail(() => {
                costJsBase.EventNotify('error', 'Lỗi hệ thống, vui lòng thử lại sau!');
                base.offLoading();
            });
        }
    }

    base.offLoading = function () {
        costJsBase.ButtonState({
            target: base.$btnSave,
            state: 'normal',
            disabled: false,
            html: '  <i class="fad fa-save"></i> Lưu lại',
            changePropAllButton: true
        });
    }

    base.formPost = function () {
        return {
            Record: base.$record.val(),
            Value: base.$inputValue.val(),
            Name: base.$inputName.val(),
            Type: base.$selectTypes.val(),
            StatusForCostEstimateType: base.$selectStatusForCostEstimateType.val(),
            StatusForSubject: base.$selectSubject.val(),
            Order: base.$inputOrder.val(),
            IsApprove: base.$checkApprove.is(':checked') ? 1 : 0,
            Signed: base.$checkSigned.is(':checked')
        };
    }

    base.setup = function () {
        jQuery.fn.validateSetup(base.$createForm,
            {
                rules: {
                    inputName: { required: true },
                    inputValue: { required: true },
                    selectSubject: { required: true },
                    selectStatusForCostEstimateType: { required: true },
                    selectTypes: { required: true },
                    inputOrder: { required: true, number: true }
                },
                messages: {
                    inputName: { required: 'Bạn chưa nhập mã trạng thái' },
                    inputValue: { required: 'Bạn chưa nhập giá trị cho trạng thái' },
                    selectSubject: { required: 'Bạn chưa chọn nhóm đối tượng' },
                    selectStatusForCostEstimateType: { required: 'Bạn chưa chọn nhóm thời gian' },
                    selectTypes: { required: 'Bạn chưa chọn nhóm dữ liệu' },
                    inputOrder: { required: 'Bạn chưa chọn nhập thứ tự', number: 'Thứ tự phải là dạng số' }
                }
            }
        );

        base.$btnSave.on('click', base.onCreate);
        $('[data-toggle="tooltip"]').tooltip();
    }
}

$(document).ready(function () {
    var c = new CostStatusCreate();
    c.setup();
});