var ElementCreate = function () {
    var base = this;
    base.$payWeek = $('#PayWeek');
    base.$costField = $('#Cost');
    base.$billCostField = $('#BillCost');
    base.$billDateField = $('#BillDateDisplay');
    base.$formCreate = $('#__formCreate');
    base.$supplierName = $('#SupplierName');
    base.$fileImage = $('#Image');
    base.$BillCode = $('#BillCode');
    base.$RequestContent = $('#RequestContent');
    base.$Explanation = $('#Explanation');
    base.$PayForm = $('#PayForm');
    base.btnSaving = $('#btnSaving');
    base.$costEstimateItemTypeId = $('#CostEstimateItemTypeId');

    base.setup = function () {
        base.$payWeek.select2({
            theme: 'bootstrap4',
            width: '100%'
        }).change(function () {
            $('#__formCreate').valid();
        });

        base.$costEstimateItemTypeId.select2({
            theme: 'bootstrap4',
            width: '100%'
        }).change(function () {
            $('#__formCreate').valid();
        });

        costJsBase.FormatMoneyInput(base.$costField);
        costJsBase.FormatMoneyInput(base.$billCostField);
        base.$billDateField.datepicker({
            format: 'dd/m/yyyy',
            language: 'vi'
        });

        base.onValidate();

        base.btnSaving.click(function () {
            if (!$('#__formCreate').valid()) {
                e.preventDefault();
                return false;
            } else {
                base.$costField.val(costJsBase.GetValueInputCurrency(base.$costField.val()));
                base.$billCostField.val(costJsBase.GetValueInputCurrency(base.$billCostField.val()));

                let update = costJsBase.ValueFromUrl('record');
                let isUpdate = typeof update !== 'undefined' && update?.length;
                // nếu là update
                // ko bắt up lại chứng từ
                if (!base.$Explanation.val().length && base.$fileImage.get(0).files.length === 0 && !isUpdate) {
                    costJsBase.EventNotify('warning', 'Bạn chưa chọn hình ảnh chứng từ!');
                    return false;
                }

                base.onLiveWarning(false);
                base.onCreate();
            }
        });

        $('#__formCreate').on('submit', function (e) {
            if (!$('#__formCreate').valid()) {
                e.preventDefault();
                return false;
            } else {
                base.$costField.val(costJsBase.GetValueInputCurrency(base.$costField.val()));
                base.$billCostField.val(costJsBase.GetValueInputCurrency(base.$billCostField.val()));

                let update = costJsBase.ValueFromUrl('record');
                let isUpdate = typeof update !== 'undefined' && update?.length;
                // nếu là update
                // ko bắt up lại chứng từ
                if (!base.$Explanation.val().length && base.$fileImage.get(0).files.length === 0 && !isUpdate) {
                    costJsBase.EventNotify('warning', 'Bạn chưa chọn hình ảnh chứng từ!');
                    return false;
                }

                base.onLiveWarning(false);
            }
        });

        costJsBase.ConfigAutocomplete({
            selector: base.$supplierName,
            ajaxURL: '/CostEstimateItem/SupplierSuggestion',
            displayField: 'supplierName',
            valueField: 'supplierName',
            triggerLength: 5
        });

        $('#__formCreate').find('input:not([type=hidden]),select').each(function () {
            $(this).bind('change', base.checkWarning);
        });
    };

    base.onValidate = function () {
        jQuery.fn.validateSetup($('#__formCreate'), {
            ignore: [],
            rules: {
                RequestContent: { required: true },
                Cost: { required: true, minCustom: true },
                CostEstimateItemTypeId: { required: true },
                PayWeek: { required: true },
                Explanation: { customeRules: true },
                PayForm: { required: true },
                BillDate: { required: false, customeBillDateRules: true },
                BillCost: { required: false, customeBillCostRules: true }
            },
            messages: {
                RequestContent: { required: 'Bạn chưa nhập nội dung đề xuất' },
                Cost: { required: 'Bạn chưa nhập số tiền yêu cầu', minCustom: "Tổng số tiền phải lớn hơn 0" },
                CostEstimateItemTypeId: { required: 'Bạn chưa chọn loại chi phí' },
                PayWeek: { required: 'Bạn chưa chọn thời gian đề xuất' },
                Explanation: { customeRules: 'Bạn vui lòng nhập giải trình với các yêu cầu không có thông tin hóa đơn' },
                PayForm: { required: 'Bạn vui lòng chọn hình thức chi' },
                BillDate: { required: false, customeBillDateRules: 'Bạn vui lòng nhập thời gian hóa đơn' },
                BillCost: { required: false, customeBillCostRules: 'Bạn vui lòng nhập giá trị hóa đơn' }
            }
        });
    }

    base.onLiveWarning = function (onWarning) {
        if (onWarning) {
            $(window).on("beforeunload", function (event) {
                localStorage.removeItem(base.$cacheName);
                return '';
            });
        } else {
            $(window).off('beforeunload');
        }
    }

    base.checkWarning = function () {
        let anything = false;
        $('#__formCreate').find('input:not([type=hidden]),select').each(function () {
            if ($(this).val().length) {
                anything = true;
            }
        });
        base.onLiveWarning(anything);
    }

    base.onCreate = function () {
        var frm = new FormData();
        base.$formCreate.find('input:not([type=hidden]),select,textarea').each(function () {
            console.log($(this).attr('name'));
            let fieldType = $(this).attr('type')

            if (fieldType !== 'file') {
                frm.append($(this).attr('name'), $(this).val())
            }
        });
        let rc = costJsBase.ValueFromUrl('record');
        frm.append('Record', rc ?? "");
        let url = '/CostEstimateItem/OnCreate';

        costJsBase.ButtonState({
            target: base.btnSaving,
            state: 'loading',
            disabled: true,
            text: 'Đang lưu lại ...',
            html: '',
            changePropAllButton: true
        });

        costJsBase.JsUploadFile({
            Url: url,
            selector: base.$fileImage,
            Data: frm
        }, base.onSuccess, base.onFail)
    }

    base.onSuccess = function (d) {
        if (d.code === costJsBase.enums.successCode) {
            window.location.href = '/CostEstimateItem/List?type=week';
        } else {
            costJsBase.EventNotify('error', d.message);
            costJsBase.ButtonState({
                target: base.btnSaving,
                state: 'normal',
                disabled: false,
                html: '<i class="fas fa-save mr-2"></i> Lưu lại',
                changePropAllButton: true
            });
        }
    }

    base.onFail = function (d) {
        costJsBase.ButtonState({
            target: base.btnSaving,
            state: 'normal',
            disabled: false,
            html: '<i class="fas fa-save mr-2"></i> Lưu lại',
            changePropAllButton: true
        });

        costJsBase.EventNotify('error', "Lỗi hệ thống, vui lòng thử lại sau!");
    }
}

$(document).ready(function () {
    var c = new ElementCreate();
    c.setup();

    jQuery.validator.addMethod("customeRules", function (value, element) {
        let $billCode = $('#BillCode').val();
        let $billDate = $('#BillDateDisplay').val();
        let $billCost = $('#BillCost').val();

        if ($billCode.length && $billDate.length && $billCost.length)
            return true;
        else if (value.length)
            return true;
        return false;
    }, "Bạn vui lòng nhập giải trình");

    jQuery.validator.addMethod("customeBillDateRules", function (value, element) {
        let $billCode = $('#BillCode').val();
        let $billDate = value;

        if ($billCode.length && $billDate.length)
            return true;
        else if (!$billCode.length)
            return true;

        return false;
    }, "");

    jQuery.validator.addMethod("minCustom", function (value, element) {
        let $cost = value;
        let $costValue = costJsBase.GetValueInputCurrency($cost);
        var fParse = parseFloat($costValue);
        return fParse > 0;
    }, "");

    jQuery.validator.addMethod("customeBillCostRules", function (value, element) {
        let $billCode = $('#BillCode').val();
        let $billCost = value;

        if ($billCode.length && $billCost.length)
            return true;
        else if (!$billCode.length)
            return true;
        return false;
    }, "");
});

