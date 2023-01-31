

var CostJsBase = function () {
    var base = this;
    var numberText = new Array(" không ", " một ", " hai ", " ba ", " bốn ", " năm ", " sáu ", " bảy ", " tám ", " chín ");
    var moneySpeak = new Array("", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ");

    base.$module = "___menu_module___";
    base.$__menu = "___menu_selected___";
    base.$swichingUnit = $("#swichingUnit");
    base.enums = {
        successCode: 200,
        errorCode: 500,
        noContentCode: 204,
        notAuthourize: 403
    };
    //base.allowImageExtension = [".jpg", ".jpeg", ".gif", ".png"];

    base.StartLoading = function () {
        if (!$('.loading-pannel').length)
            $('body').append('<div class="loading-pannel"><div class="lds-dual-ring"></div></dimv>');
        $('.loading-pannel').show();
    }

    base.HideLoading = function () {
        $('.loading-pannel').hide();
    }

    //1. Hàm đọc số có ba chữ số;
    base.NumberToText = function (threeNumber) {
        var result = "";
        var hundreds = parseInt(threeNumber / 100);
        var dozens = parseInt((threeNumber % 100) / 10);
        var units = threeNumber % 10;
        if (hundreds === 0 && dozens === 0 && units === 0) return "";
        if (hundreds !== 0) {
            result += numberText[hundreds] + " trăm ";
            if ((dozens === 0) && (units !== 0)) result += " linh ";
        }
        if ((dozens !== 0) && (dozens !== 1)) {
            result += numberText[dozens] + " mươi";
            if ((dozens === 0) && (units !== 0)) result = result + " linh ";
        }
        if (dozens === 1) result += " mười ";
        switch (units) {
            case 1:
                if ((dozens !== 0) && (dozens !== 1)) {
                    result += " mốt ";
                } else {
                    result += numberText[units];
                }
                break;
            case 5:
                if (dozens === 0) {
                    result += numberText[units];
                } else {
                    result += " lăm ";
                }
                break;
            default:
                if (units !== 0) {
                    result += numberText[units];
                }
                break;
        }
        return result;
    }

    // Function convert number to
    // Vietnamese money speak
    base.MoneyString = function (input, prefix) {
        var counter = 0;
        var i = 0;
        var num = 0;
        var result = "";
        var tmp = "";
        var idx = new Array();
        if (input < 0) return "";
        if (input === 0) return "Không " + prefix;
        if (input > 0) {
            num = input;
        } else {
            num = -input;
        }
        if (input > 8999999999999999) {
            //input = 0;
            return "Số quá lớn!";
        }
        idx[5] = Math.floor(num / 1000000000000000);
        if (isNaN(idx[5]))
            idx[5] = "0";
        num = num - parseFloat(idx[5].toString()) * 1000000000000000;
        idx[4] = Math.floor(num / 1000000000000);
        if (isNaN(idx[4]))
            idx[4] = "0";
        num = num - parseFloat(idx[4].toString()) * 1000000000000;
        idx[3] = Math.floor(num / 1000000000);
        if (isNaN(idx[3]))
            idx[3] = "0";
        num = num - parseFloat(idx[3].toString()) * 1000000000;
        idx[2] = parseInt(num / 1000000);
        if (isNaN(idx[2]))
            idx[2] = "0";
        idx[1] = parseInt((num % 1000000) / 1000);
        if (isNaN(idx[1]))
            idx[1] = "0";
        idx[0] = parseInt(num % 1000);
        if (isNaN(idx[0]))
            idx[0] = "0";
        if (idx[5] > 0) {
            counter = 5;
        } else if (idx[4] > 0) {
            counter = 4;
        } else if (idx[3] > 0) {
            counter = 3;
        } else if (idx[2] > 0) {
            counter = 2;
        } else if (idx[1] > 0) {
            counter = 1;
        } else {
            counter = 0;
        }
        for (i = counter; i >= 0; i--) {
            tmp = base.NumberToText(idx[i]);
            result += tmp;
            if (idx[i] > 0) result += moneySpeak[i];
            if ((i > 0) && (tmp.length > 0)) result += ',';
        }
        if (result.substring(result.length - 1) === ',') {
            result = result.substring(0, result.length - 1);
        }
        result = result.substring(1, 2).toUpperCase() + result.substring(2);
        return result + " " + prefix;
    }

    // Function format input to money format
    // i.e 1.000.000
    base.FormatMoney = function (input, ifZero) {
        if (typeof (input) === 'undefined')
            return ifZero;
        if (typeof (input) === 'number')
            input = input.toString();
        var parseNumber = parseFloat(input.replace(/\,/g, ''));
        if (parseNumber === 0 && typeof ifZero === 'string')
            return ifZero;
        var numFormat = parseNumber.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        return numFormat;
    }

    // Function format input to money format
    // i.e 1,000,000
    base.FormatMoneyComma = function (input) {
        var parseNumber = parseInt(input);
        var numFormat = parseNumber.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        return numFormat;
    }

    // Function bind to selector
    // Allow only number chars
    base.OnlyNumber = function (input) {
        $(input).bind('keyup paste', function () {
            this.value = this.value.replace(/[^0-9,.]/g, '');
        });
    }

    // Function bind to selector
    // Automating format value to money format
    base.FormatMoneyInput = function (input) {
        $(input).bind('keyup paste', function () {
            if (this.value !== '')
                this.value = base.FormatMoney(this.value);
        });
    }

    // Function bind to selector
    // Automating format value to money format

    base.FormatInputCurrency = function (input) {
        $(input).on('input', function () {
            $(this).val(base.FormatMoney(this.value.replace(/[.$]/g, '')));
        }).on('keypress', function (e) {
            if (!$.isNumeric(String.fromCharCode(e.which))) e.preventDefault();
        }).on('paste', function (e) {
            var cb = e.originalEvent.clipboardData || window.clipboardData;
            if (!$.isNumeric(cb.getData('text'))) e.preventDefault();
        });
    }


    // Function convert string to DateTime
    // DateTime return format like 'DD-MM-YYYY'
    // base on moment.js

    base.DateTimeToString = function (value) {
        return value ? moment(new Date(parseInt(value.slice(6, -2)))).format("DD-MM-YYYY") : "";
    }

    // Function get value of currency input
    // remove all special character
    // keep chars is numeric
    base.GetValueInputCurrency = function (text) {
        var sAmount = text;
        var nsAmount = sAmount.replace(/[.,$]/g, '');
        return nsAmount;
    }

    base.ShowMessages = function (type, msg) {
        $.notify({
            message: msg
        },
            {
                type: type,
                showProgressbar: false,
                z_index: 1059,
                timer: 60000
            });
    }
    //refer: https://sweetalert2.github.io/
    base.EventNotify = function (type, msg) {
        Swal.fire({
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 5000,
            icon: type,
            title: msg
        });
    }

    base.ExportExcel = function(exportUrl, data) {
        base.ExportFile(exportUrl, data, 'xlsx');
    }
    
    base.ExportPdf = function(exportUrl, data) {
        base.ExportFile(exportUrl, data, 'pdf');
    }

    base.ExportFile = function(exportUrl, data, fileExtension) {
        let query = new URLSearchParams(data);
        const apiUrl = `${exportUrl}?${query.toString()}`;
        let fileName = '';
        fetch(new Request(apiUrl))
            .then(response => {
                var disposition = response.headers.get('Content-Disposition');
                if (disposition && disposition.indexOf('attachment') !== -1) {
                    var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                    var matches = filenameRegex.exec(disposition);
                    if (matches != null && matches[1]) fileName = matches[1].replace(/['"]/g, '');
                }

                return response.blob();
            })
            .then(result => {
                let objectURL = URL.createObjectURL(result);
                let exportFileName = fileName ? fileName : `default.${fileExtension}`;
                let downloadLink = document.createElement('a');
                if (typeof downloadLink.download === 'undefined') {
                    window.location = objectURL;
                } else {
                    downloadLink.href = objectURL;
                    downloadLink.download = exportFileName;
                    document.body.appendChild(downloadLink);
                    downloadLink.click();
                }
            }).catch(error => {
            console.error(error);
            costJsBase.EventNotify('error', 'Lỗi hệ thống, vui lòng thử lại sau!');
        });
    }

    //Show validate color
    base.ShowValidation = function (listError) {
        for (let i = 0; i < listError.length; ++i) {

            if (listError[i].fieldType == "Dropdown") {
                //Dropdown
                $('#' + listError[i].fieldError).next().find('.select2-selection--single').addClass("itemerror");
            }
            else {
                //Textbox
                $('#' + listError[i].fieldError).addClass("itemerror");
            }
        }
    }
    //Hidden validate color
    $(".itemerror").click(function () {
        this.removeClass('itemerror');
    });

    $(".itemerror").change(function () {
        this.removeClass('itemerror');
    });

    base.ValidInputFileContent = function (maxLength, fileType, $input, $label, $validSize, $size) {
        //append
        var htmlWatermark = '<div class="input-group m-b-10"><div class="input-group-prepend"><span class="input-group-text"><input type="checkbox"></span></div><input type="text" class="form-control" placeholder="Checkbox add on"></div>';
        $(htmlWatermark).insertAfter($($input).closest('p'));
        //end append
        $($input).on("change",
            function () {
                var files = $(this).get(0).files;
                var validExtension;
                //check file extension
                if (files != null && files.length > 0) {
                    for (var i = 0; i < files.length; i++) {
                        var fName = files[i].name;
                        validExtension = base.CheckExtension(fName, fileType);
                        if (!validExtension) {
                            base.EventNotify('error', 'Tệp tải lên không đúng định dạng');
                            base.ClearInput($input, $label);
                            return;
                        } else {
                            //check filesize
                            var kbSize = parseFloat(files[i].size);
                            var mbSize = parseFloat(kbSize / 1024 / 1024); //==> to MB
                            if (mbSize > maxLength) {
                                base.EventNotify('Tệp tải lên vượt quá dung lượng', 'Vui lòng nhập tệp có dung lượng nhỏ hơn ' + maxLength * 1024 + ' kb');
                                base.ClearInput($input, $label);
                                return;
                            } else {
                                if ($validSize && $size.length && files.length === 1) {
                                    var img = new Image();
                                    img.src = window.URL.createObjectURL(files[i]);
                                    img.onload = function () {
                                        var width = img.naturalWidth,
                                            height = img.naturalHeight;
                                        window.URL.revokeObjectURL(img.src);
                                        //console.log(width + " - " + height);
                                        if (width < $size[0] || height < $size[1]) {
                                            base.EventNotify('Kích thước ảnh không phù hợp',
                                                'Vui lòng chọn ảnh kích thước tối thiểu ' + $size[0] + ' x ' + $size[1]);
                                            base.ClearInput($input, $label);
                                            return;
                                        }
                                    };
                                }
                            }
                        }
                    }
                    if (files.length > 1) {
                        var strC = 'Có ' + files.length + ' tệp được chọn';
                        if (typeof ($label) != 'undefined')
                            $($label).textNodes('').replaceWith(strC);
                    } else {
                        var fSigleName = files[0].name;
                        if (typeof ($label) != 'undefined')
                            $($label).textNodes('').replaceWith(fSigleName);
                    }
                }
            });
    }

    // Function reset upload input
    // $input: is input[file]
    // $label: displaying label
    base.ClearInput = function ($input, $label) {
        var input = $($input);
        input.replaceWith(input.val('').clone(true));
        if (typeof ($label) != 'undefined')
            $($label).textNodes('').replaceWith('Chọn tệp');

    }

    // Function validate file extension
    // Allows in extensions array
    base.CheckExtension = function (fileName, arrayExtension) {
        //check file extension
        var validEx = false;
        var fName = fileName;
        for (var j = 0; j < arrayExtension.length; j++) {
            var sCurExtension = arrayExtension[j];
            if (fName.substr(fName.length - sCurExtension.length, sCurExtension.length).toLowerCase() === sCurExtension.toLowerCase()) {
                validEx = true;
                break;
            }
        }
        return validEx;
    }


    // Function create ajax POST request
    // @option: request configuration
    // @fnSuccess: function handler on success request
    // @fnError: function handler on failure request
    base.Post = function (option, fnSuccess, fnError) {
        if (option.Data) {
            option.Data.__RequestFormToken = $("input[name=__RequestVerificationToken]").val();
        }
        else {
            option.Data = { __RequestFormToken: $("input[name=__RequestVerificationToken]").val() }
        }
        $.ajax({
            url: option.Url,
            method: 'POST',
            data: option.Data,
            //todo: optional if request have a file
            //contentType: false,
            //processData: false,
            beforeSend: function (request) {
                request.setRequestHeader("RequestVerificationToken", $("input[name=__RequestVerificationToken]").val());
                //todo: loading optional
                if (typeof option.beforeSend === "function")
                    option.beforeSend();
            },
            async: (option.async == undefined ? true : option.async),
            complete: function () {
                //todo: loading optional

            },
            success: function (rs) {
                //todo: session expire OR not authourize action
                if (rs.code === base.enums.notAuthourize) { }
                else {
                    if (typeof fnSuccess === "function")
                        fnSuccess(rs);
                }
            },
            error: function (e) {
                if (!fnError) { }
                if (typeof fnError === "function")
                    fnError(e);
            }
        });
    }

    base.PostJson = function (option, fnSuccess, fnError) {
        if (option.Data) {
            option.Data.__RequestFormToken = $("input[name=__RequestVerificationToken]").val();
        }
        else {
            option.Data = { __RequestFormToken: $("input[name=__RequestVerificationToken]").val() }
        }
        $.ajax({
            url: option.Url,
            method: 'POST',
            data: option.Data,
            //todo: optional if request have a file
            contentType: "application/json; charset=utf-8;",
            //processData: false,
            beforeSend: function (request) {
                request.setRequestHeader("RequestVerificationToken", $("input[name=__RequestVerificationToken]").val());
                //todo: loading optional
                if (typeof option.beforeSend === "function")
                    option.beforeSend();
            },
            async: (option.async == undefined ? true : option.async),
            complete: function () {
                //todo: loading optional

            },
            success: function (rs) {
                //todo: session expire OR not authourize action
                if (rs.code === base.enums.notAuthourize) { }
                else {
                    if (typeof fnSuccess === "function")
                        fnSuccess(rs);
                }
            },
            error: function (e) {
                if (!fnError) { }
                if (typeof fnError === "function")
                    fnError(e);
            }
        });
    }

    // Function create ajax POST request
    // @option: request configuration
    // @fnSuccess: function handler on success request
    // @fnError: function handler on failure request
    base.Get = function (option, fnSuccess, fnError) {
        if (option.Data) {
            option.Data.__RequestFormToken = $("input[name=__RequestVerificationToken]").val();
        }
        else {
            option.Data = { __RequestFormToken: $("input[name=__RequestVerificationToken]").val() }
        }
        $.ajax({
            url: option.Url,
            method: 'GET',
            data: option.Data,
            //todo: optional if request have a file
            //contentType: false,
            //processData: false,
            beforeSend: function (request) {
                request.setRequestHeader("RequestVerificationToken", $("input[name=__RequestVerificationToken]").val());
                //todo: loading optional
                if (typeof option.beforeSend === "function")
                    option.beforeSend();
            },
            async: (option.async == undefined ? true : option.async),
            complete: function () {
                //todo: loading optional

            },
            success: function (rs) {
                //todo: session expire OR not authourize action
                if (rs.code === base.enums.notAuthourize) { }
                else {
                    if (typeof fnSuccess === "function")
                        fnSuccess(rs);
                }
            },
            error: function (e) {
                if (!fnError) { }
                if (typeof fnError === "function")
                    fnError(e);
            }
        });
    }

    base.PromissePost = function (option) {
        if (option.Data) {
            option.Data.__RequestFormToken = $("input[name=__RequestVerificationToken]").val();
        }
        else {
            option.Data = { __RequestFormToken: $("input[name=__RequestVerificationToken]").val() }
        }
        let ajaxCall = $.ajax({
            data: option.Data,
            url: option.Url,
            method: 'POST',
            beforeSend: function () {
                if (typeof option.beforeSend === "function")
                    option.beforeSend();
            },
            async: (option.async == undefined ? true : option.async)
        });
        return ajaxCall;
    }

    base.JsUploadFile = function (option, fnSuccess, fnError) {
        var files = option.selector.get(0).files;
        if (files.length == 0) {
            try { files = $('#' + $input.attr('id')).get(0).files; } catch (e) {
                console.log('%c Info', 'color: white; background-color: #95B46A', 'JUpload file');
            }
        }
        var formData = option.Data ?? new FormData();
        for (var i = 0; i < files.length; i++) {
            formData.append("files[]", files[i]);
        }

        if (typeof (option.Data) !== 'undefined' && option.Data !== null)
            formData.append("data", option.Data);

        $.ajax({
            url: option.Url,
            cache: false,
            contentType: false,
            processData: false,
            data: formData,
            type: 'post',
            beforeSend: function () {
                //todo: loading optional
                if (typeof option.beforeSend === "function")
                    option.beforeSend();
            },
            async: (option.async == undefined ? true : option.async),
            success: function (response) {
                if (typeof (fnSuccess) == 'function')
                    fnSuccess(response);
            },
            complete: function () {
                if (typeof (option.complete) == 'function')
                    option.complete();
            },
            error: function (err) {
                if (typeof (fnError) == 'function')
                    fnError(err);
            }
        });
    }


    // Function get value from URL

    base.ValueFromUrl = function (param) {
        var url = new URL(window.location.href);
        var c = url.searchParams.get(param);
        return c;
    }

    // function open modal
    /*  setting {
            url: API URL to load content popup
            overlay: display overlay div
            callback: function callback after modal 'load'
            selector: target element
            title: page title
            buttons: customize render buttons
     */
    base.OpenModal = function (setting) {
        let props = {
            url: '',
            overlay: false,
            callback: function () { },
            selector: null,
            title: '',
            dropContent: false,
            /**
             * Cấu hình các nút trên popup
             * Binding sự kiện khi nhấn vào nút
             */
            buttons: {
                accept: {
                    visible: false,
                    html: `<button type="button" prop-type="elems.accept" class="btn btn-outline-success btn-sm">
                                <i class="fad fa-check mr-2" aria-hidden="true"></i> Phê duyệt
                            </button>`,
                    listener: {
                        type: 'click',
                        event: function () { }
                    }
                },
                decline: {
                    visible: false,
                    html: `<button type="button" prop-type="elems.decline" class="btn btn-outline-danger btn-sm">
                                <i class="fad fa-ban mr-2" aria-hidden="true"></i>Từ chối
                            </button>`,
                    listener: {
                        type: 'click',
                        event: function () { }
                    }
                },
                close: {
                    visible: false,
                    html: `<button type="button" prop-type="elems.close" class="btn btn-default btn-sm" data-bs-dismiss="modal" data-dismiss="modal">
                                 Đóng
                            </button>`,
                    listener: {
                        type: 'click',
                        event: function () { }
                    }
                },
            },
            renderButtons: function (me) {
                let $this = me;
                if (!$this.selector) { return false; }
                let buttonsHtml = ``;
                let events = [];
                if ($this.buttons && typeof ($this.buttons) === 'object') {
                    if ($this.buttons.accept.visible) {
                        buttonsHtml += $this.buttons.accept.html;
                        events.push({ elem: '[prop-type="elems.accept"]', type: $this.buttons.accept.listener.type, event: $this.buttons.accept.listener.event })
                    }
                    if ($this.buttons.decline.visible) {
                        buttonsHtml += $this.buttons.decline.html;
                        events.push({ elem: '[prop-type="elems.decline"]', type: $this.buttons.decline.listener.type, event: $this.buttons.decline.listener.event })
                    }
                    if ($this.buttons.close.visible) {
                        buttonsHtml += $this.buttons.close.html;
                        events.push({ elem: '[prop-type="elems.close"]', type: $this.buttons.close.listener.type, event: $this.buttons.close.listener.event })
                    }
                }
                $this.selector.find('[prop-type="elems.buttons"]').html(buttonsHtml);
                for (var i = 0; i < events.length; i++) {
                    let e = events[i];
                    $this.selector.find(e.elem).bind(e.type, e.event);
                }
            },
            initialize: function (po) {
                let $this = po;
                $this.selector.overlay ? $this.selector.find(".overlay").show() : $this.selector.find(".overlay").hide();
                $this.selector.find('.modal-title').html(this.title);
                if ($this.dropContent) {
                    $this.selector.find(".modal-body").html($this.loading());
                    $this.selector.off();
                }

                if ($this.url.length) {
                    $this.selector.off().on('show.bs.modal', function () {
                        let func = function () { };
                        if (typeof ($this.callback) === 'function' && $this.callback !== null) { func = $this.callback };
                        $this.selector.find(".modal-body").load($this.url, func);
                        $this.selector.find("[prop-spinner]").hide();
                    });
                } else if (!$this.dropContent) {
                    $this.selector.find(".modal-body").html($this.loading());
                }
                $this.renderButtons($this);
                $this.show($this);
            },
            show: function (me) {
                let $this = me;
                if ($this.clearContent) {

                }
                $this.selector.modal("show");
            },
            hide: function (me) {
                let $this = me;
                $this.selector.modal("hide");
            },
            loading: function () {
                let html = `<div class="spinner-border spinner-border-sm mr-2" role="status"><span class="sr-only">Loading...</span></div><div class="spinner-grow spinner-grow-sm" role="status"><span class="sr-only">Loading...</span></div>`;
                return html;
            }
        }
        $.fn.extend(true, props, setting);


        props.initialize(props);
    }
    /**
     * Khởi tạo input autocomplete
     * Base trên typeahead bootstrap
     * i.e
     * costJsBase.ConfigAutocomplete({
            selector: selector,
            ajaxURL: '',
            displayField: 'id',
            valueField: 'id',
            onProcess: function (data) {
                if (data.success === false) {
                    return false;
                }
                return data.data;
            }
        });
     *
     *
     **/
    base.ConfigAutocomplete = function (setting) {
        let prop = {
            selector: null,
            ajaxURL: '',
            displayField: '',
            valueField: '',
            triggerLength: 4
        };

        $.extend(true, prop, setting);
        if (typeof (prop.selector) === 'undefined') {
            return false;
        }

        let optionDefault = {
            onSelect: function (item) {
                prop.selector.data("seletectedValue", item.value);
                prop.selector.data("seletectedText", item.text);
                if (typeof (prop.onSelect) == "function")
                    prop.onSelect(item);
            },
            cache: false,
            ajax: {
                url: prop.ajaxURL,
                timeout: 500,
                displayField: prop.displayField,
                valueField: prop.valueField,
                cache: false,
                triggerLength: prop.triggerLength,
                loadingClass: "ax",
                preDispatch: (typeof (prop.onDispatch) === 'undefined' ? function (query) {
                    return {
                        search: query
                    }
                } : prop.onDispatch),
                preProcess: (typeof (prop.onProcess) === 'undefined' ? function (data) {
                    if (data.success === false) {
                        return false;
                    }
                    return data;
                } : prop.onProcess)
            },
            // break grepper
            // searching through in API
            grepper: function (data) { return data },
            scrollBar: true,
            items: 30
        }

        prop.selector.typeahead(optionDefault);
    }

    base.Rad = function (length) {
        let result = '';
        let characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        let charactersLength = characters.length;
        for (let i = 0; i < length; i++) {
            result += characters.charAt(Math.floor(Math.random() * charactersLength));
        }
        return result;
    }

    base.ConfirmPopup = function (setting) {
        let staticPopSelector = $('#__staticConfirmBoxModal')
        let prop = {
            propertise: {
                //refer: https://codepen.io/btn-ninja/pen/YrXmax
                icon: "",
                large_text: "",
                small_text: "",
                contentHtml: '',
                type: 'success'
            },
            selector: staticPopSelector,
            buttons: {
                left: {
                    enable: true,
                    text: 'Cancel',
                    class: 'btn-default',
                    action: function () { },
                    dismiss: true
                },
                right: {
                    enable: true,
                    text: 'Delete',
                    class: 'btn-danger',
                    action: function () { },
                    dismiss: false
                }
            },
            initialize: function () {
                if (this.selector === null || typeof (this.selector) === 'undefined') {
                    return false;
                }
                this.builder();
                this.renderButtons();

                if (this.afterRender !== null && typeof (this.afterRender) == 'function') {
                    this.afterRender();
                }

                this.selector.modal("show");
            },
            renderButtons: function () {
                let buttonsHtml = ``;
                let puts = [];
                if (this.buttons && typeof (this.buttons) === 'object') {
                    if (this.buttons.left) {
                        let radUnique = '__' + base.Rad(10);
                        buttonsHtml += this.buttonCreate(radUnique, this.buttons.left.class, this.buttons.left.dismiss, this.buttons.left.text);
                        puts.push({
                            id: radUnique,
                            event: this.buttons.left.action
                        })
                    }
                    if (this.buttons.right) {
                        let radUnique = '__' + base.Rad(10);
                        buttonsHtml += this.buttonCreate(radUnique, this.buttons.right.class, this.buttons.right.dismiss, this.buttons.right.text);
                        puts.push({
                            id: radUnique,
                            event: this.buttons.right.action
                        })
                    }
                }
                this.selector.find('[prop-type="elems.buttons"]').html(buttonsHtml);
                for (var i = 0; i < puts.length; i++) {
                    let elAt = puts[i];
                    $('#' + elAt.id).off().bind('click', { setting: this }, elAt.event);
                }
            },
            builder: function () {
                this.selector.find('[prop-elem="__small-text"]').text(this.propertise.small_text);
                this.selector.find('[prop-elem="__large-text"]').text(this.propertise.large_text);
                this.selector.find('[prop-elem="__x-icon"]').html(this.propertise.icon);
                this.propertise.type !== 'success' ? this.selector.find('[prop-elem="__x-style"]').removeClass('icon-box-success') : this.selector.find('[prop-elem="__x-style"]').addClass('icon-box-success');
                let smallText = `<p prop-elem="__small-text">` + this.propertise.small_text + `</p>`;
                staticPopSelector.find('.modal-body').html(this.propertise.contentHtml.length ? smallText + this.propertise.contentHtml : smallText);

            },
            buttonCreate: function (uniqID, className, dissmiss, text) {
                let formatButton = '<button id="__ID__" type="button" class="btn __CLASS__" __DISMISS__>__TEXT__</button>';
                formatButton = formatButton.replace('__ID__', uniqID);
                formatButton = formatButton.replace('__CLASS__', className);
                formatButton = formatButton.replace('__TEXT__', text);

                let dissmissString = dissmiss ? 'data-dismiss="modal"' : '';
                formatButton = formatButton.replace('__DISMISS__', dissmissString);

                return formatButton
            },
            afterRender: function () { }
        };

        $.extend(true, prop, setting);
        prop.initialize();
    }

    base.ButtonState = function (setting) {
        var deferred = $.Deferred();

        let prop = {
            target: null,
            state: '', //loading, done, normal
            disabled: false,
            text: 'Đang xử lý ...',
            spinner: '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> __TEXT__',
            html: '',
            changePropAllButton: false
        };

        $.extend(true, prop, setting);

        if (typeof (prop.target) !== 'undefined' && prop.target !== null) {
            if (typeof (prop.target) === 'string') {
                prop.target = $(prop.target);
            }

            if (prop.changePropAllButton) {
                prop.target.parent().find('button').prop('disabled', prop.disabled);
            }

            prop.target.prop('disabled', prop.disabled);
            switch (prop.state) {
                case 'loading':
                    prop.target.html(prop.spinner.replace('__TEXT__', prop.text));
                    break
                case 'done':
                case 'normal':
                    !prop.html.length ? prop.target.html('').text(prop.text) : prop.target.html(prop.html);
                default:
                    break
            }
        }

        deferred.resolve(setting);

        return deferred.promise();
    }

    base.HistoriesFilter = function (state) {
        if (state === 'all') {
            let selector = $('#viewHistories .timeline');
            selector.show();
            !selector.length ? $('#viewHistories p').show() : $('#viewHistories p').hide();
        } else {
            let selector = $('#viewHistories .timeline[prop-stats=' + state + ']');
            selector.show();
            $('#viewHistories .timeline:not([prop-stats=' + state + '])').hide();
            !selector.length ? $('#viewHistories p').show() : $('#viewHistories p').hide();
        }
    }

    base.selectedMenu = function () {
        let cacheMenu = localStorage.getItem(base.$__menu);
        if (typeof cacheMenu !== 'undefined') {
            let specifier = cacheMenu ?? '44';
            if (specifier.length) {
                let li = $('li[specifer="' + specifier + '"]');
                li.find('a').addClass('menu-selected');
                if (li.length) {
                    let liRoot = li.parent().closest('li');
                    base.loopSelected(liRoot.attr('specifer'));
                    if (liRoot.length) {
                        liRoot.addClass('menu-is-opening menu-open');
                    }
                }
            }
        }


        //let path = window.location.pathname;
        //if (path !== '/' && path.length) {
        //    let allLink = $('nav a');
        //    allLink.each(function () {
        //        let thisA = $(this).attr('href');

        //        if (thisA.toLowerCase().startsWith(path.toLowerCase())) {
        //            $(this).addClass('active');
        //            let ul = $(this).closest('ul.nav-treeview');
        //            if (ul.length) {
        //                ul.show().siblings('a').addClass('active');
        //                ul.closest('li').addClass('menu-is-opening menu-open');
        //            } else {
        //                $(this).addClass('active').closest('li').addClass('active');
        //            }
        //        }
        //    });
        //}
    }

    base.loopSelected = function (specifier) {
        if (typeof specifier !== 'undefined' && specifier?.length) {
            if (specifier.length) {
                let li = $('li[specifer="' + specifier + '"]');
                if (li.length) {
                    let liRoot = li.parent().closest('li');
                    //base.loopSelected(liRoot.attr('specifer'));
                    if (liRoot.length) {
                        liRoot.addClass('menu-is-opening menu-open');
                    }
                }
            }
        }
    }

    base.choice = function (module) {
        localStorage.setItem(base.$module, module);
        base.onLoad();
    }

    base.onLoad = function () {
        let module = localStorage.getItem(base.$module) ?? 'GENERAL';
        if (module.length) {
            $('nav li.nav-item[identifier]').each(function () {
                let oLi = $(this);
                let sModule = oLi.attr('identifier');
                if (module.toString().toLowerCase() === sModule.toString().toLowerCase() || sModule === 'GENERAL') {
                    oLi.show();
                } else {
                    oLi.hide();
                }
            });
            $('div[identifier="' + module + '"].bg-success-selected').each(function () {
                $(this).removeClass('bg-success-selected');
            });
            $('div[identifier="' + module + '"]').addClass('bg-success-selected');
        }

    }

    base.onSignout = function () {
        localStorage.removeItem(base.$__menu);
        localStorage.removeItem(base.$module);
    }

    base.onMenuSelect = function () {
        let specifer = $(this).closest('li').attr('specifer');
        localStorage.setItem(base.$__menu, specifer);
    }

    base.switchUnit = function () {
        let ops = {
            Url: '/Users/SwitchUnit',
            Data: {
                UnitCode: base.$swichingUnit.val()
            },
            beforeSend: function () {
                base.$swichingUnit.prop('disabled', true);
                base.$swichingUnit.siblings('.spinner-border').removeClass('d-none');
            }
        };
        base.Post(ops,
            function (data) {
                base.$swichingUnit.prop('disabled', false);
                base.$swichingUnit.siblings('.spinner-border').addClass('d-none');

                if (data.code === base.enums.successCode) {
                    base.EventNotify('success', data.message);
                    setTimeout(function () {
                        localStorage.removeItem(base.$__menu);
                        window.location.href = '/index.html';

                    }, 1000);
                } else {
                    base.EventNotify('success', data.message);
                }

            }, function (err) {
                base.$swichingUnit.prop('disabled', false);
                base.$swichingUnit.siblings('.spinner-border').addClass('d-none');
            });
    }
}


var costJsBase = new CostJsBase();

$(document).ready(function () {
    $('[data-back]').click(function () {
        window.history.back();
    });
    if (typeof $.fn.dataTable != 'undefined')
        $.fn.dataTable.ext.errMode = 'none';
    costJsBase.onLoad();
    costJsBase.selectedMenu();


    let fOps = costJsBase.ValueFromUrl('fOps');
    if (fOps == null || fOps === 'false') {
        let statsOp = $('#lableStats');
        if (statsOp.length) {
            statsOp.closest('div.col-md-4').hide();
        }
    }


    let stats = costJsBase.ValueFromUrl('stats');
    if (stats != null && stats === "-8888") {
        //hide tạo kế hoạch
        $("#___createFormPlan").remove();
    }

    $('nav a.nav-link:not(.without)').click(costJsBase.onMenuSelect);

    costJsBase.$swichingUnit.on('change', costJsBase.switchUnit);
});


jQuery.fn.textNodes = function () {
    return this.contents().filter(function () {
        return (this.nodeType === Node.TEXT_NODE && this.nodeValue.trim() !== "");
    });
}

// register global funtion
// setup input to DataTableJS
// refer: https://legacy.datatables.net/ref#oPaginate
jQuery.fn.jsTableRegister = function (settings) {
    let settingDefault = {
        selector: null,
        language: {
            sProcessing: "Đang xử lý...",
            sLengthMenu: "Xem _MENU_ mục",
            sZeroRecords: "Không tìm thấy dòng nào phù hợp",
            sInfo: "Đang xem _START_ đến _END_ trong tổng số _TOTAL_ mục",
            sInfoEmpty: "Đang xem 0 đến 0 trong tổng số 0 mục",
            sInfoFiltered: "(được lọc từ _MAX_ mục)",
            sInfoPostFix: "",
            sSearch: "Tìm kiếm:",
            oPaginate: {
                sFirst: "Đầu",
                sPrevious: "Trước",
                sNext: "Tiếp",
                sLast: "Cuối"
            }
        },
        processing: true,
        serverSide: true,
        initComplete: function (initSettings, json) {
        },
        drawCallback: function () {
        },
        columns: [],
        ajax: null,
        responsive: true,
        autoWidth: true,
        bStateSave: false,
        paging: true,
        fixedHeader: false,
        dom: "<'row'<'col-sm-12 col-md-6'f>><'row'<'col-sm-12'tr>><'row mt-1'<'col-sm-12 col-md-4'l><'col-sm-12 col-md-4'i><'col-sm-12 col-md-4'p>>",
        //<"float-left"B><"float-right"f>rt<"row"<"col-sm-4"l><"col-sm-4"i><"col-sm-4"p>>
        renderer: "bootstrap",
        searching: false,
        data: null,
        footerCallback: function (row, data, start, end, display) { },
        iDisplayLength: 25,
        select: '',
        altEditor: false
    }

    let extendColumns = [];

    $.extend(true, settingDefault, settings);

    let fnSortCols = function (left, right) {
        if (left.idx < right.idx) return -1;
        else if (left.idx > right.idx) return 1;
        return 0;
    }

    settingDefault.columns.sort(fnSortCols);

    if (settingDefault.ajax) {
        if (!settingDefault.ajax.beforeSend)
            settingDefault.ajax.beforeSend = function (request) {
                request.setRequestHeader("RequestVerificationToken", $("input[name=__RequestVerificationToken]").val());
            }
    }

    if (settingDefault.columns.length) {
        for (let i = 0; i < settingDefault.columns.length; i++) {
            let column = {
                data: null,
                order: false,
                render: function (data) { },
                class: '',
                idx: 0,
                width: "auto",
                searchable: false,
                name: ''
            };
            $.extend(true, column, settingDefault.columns[i]);
            extendColumns.push({
                "data": column.data,
                "orderable": column.order,
                "render": column.render,
                "className": column.class,
                "width": column.width,
                "searchable": column.searchable,
                "name": column.name
            });
        }
    }


    // not set selector
    if (!settingDefault.selector) {
        console.error(`must be set table selector`);
        return false;
    }
    // not set columns
    if (!settingDefault.columns.length) {
        console.error(`must be config table columns`);
        return false;
    }

    settingDefault.columns = extendColumns;
    settingDefault.sDom = 'Rfrtlip';
    settingDefault.ordering = false;

    let $table = $(settingDefault.selector).DataTable(settingDefault);
   
    $(window).resize(function () {
        //$table.columns.adjust().draw();
    });

    $('[data-widget="pushmenu"]').click(function() {
        setTimeout(function() {
            $table.columns.adjust().draw();
        }, 500);
    });

    return $table;
}


// refer: https://www.daterangepicker.com/
// register global funtion
// setup input to daterangepicker
// default: vi-VN date format
jQuery.fn.viRangeDateRegister = function (settings) {
    let prop = {
        locale: {
            applyLabel: "Xác nhận",
            cancelLabel: "Hủy bỏ",
            format: 'DD/MM/YYYY',
            daysOfWeek: [
                "CN",
                "T2",
                "T3",
                "T4",
                "T5",
                "T6",
                "T7"
            ],
            monthNames: [
                "Tháng 1",
                "Tháng 2",
                "Tháng 3",
                "Tháng 4",
                "Tháng 5",
                "Tháng 6",
                "Tháng 7",
                "Tháng 8",
                "Tháng 9",
                "Tháng 10",
                "Tháng 11",
                "Tháng 12"
            ]
        },
        callbackOnApply: function () { },
        selector: null
    }

    $.extend(true, prop, settings);

    if (prop.selector) {
        let dp = prop.selector.daterangepicker({ locale: prop.locale });
        if (typeof prop.callbackOnApply === 'function')
            dp.off('apply.daterangepicker', prop.callbackOnApply).on('apply.daterangepicker', prop.callbackOnApply);
    }
}

// register global funtion
// setup input to handle listener Enter keycode

jQuery.fn.eEnterActions = function (acts) {
    let prop = {
        selector: null,
        keycode: "keyup",
        action: function () { }
    }

    $.extend(true, prop, acts);
    if (prop.selector) {
        prop.selector.bind(prop.keycode, function (act) {
            let keyCode = act.keyCode;
            if (keyCode == 13 && typeof prop.action === 'function') {
                prop.action(this);
            }
        });
    }
}

// function build URI
// append or update param value
// baseURI: is href
// paramName: param name
// paramValue: param value
jQuery.fn.buildParamURI = function (baseURI, paramName, paramValue, remove) {
    var url = URI(baseURI);
    if (typeof paramValue === 'undefined' || paramValue?.length === 0)
        remove = true;

    if (remove) {
        url.removeSearch(paramName);
    } else {
        var queryMap = url.query(true);
        if (queryMap[paramName]) {
            var values = queryMap[paramName];
            if (values !== paramValue) {
                // paramValue not found: add it
                url.setQuery(paramName, paramValue);
            }
        } else {
            // param does not exist yet
            url.setQuery(paramName, paramValue);
        }
    }
    return url.toString();
}




// function get all URI params
// returns map's params
jQuery.fn.dataURI = function (uri) {
    var url = URI(uri);
    var queryMap = url.query(true);
    return queryMap;
}

// function
// set value to element
// and callback trigger func if not undefined
jQuery.fn.setElementValue = function (selector, value, trigger) {
    if (typeof (selector) !== 'undefined' && typeof (value) !== 'undefined') {
        let jSelector = jQuery(selector);
        if (jSelector.length) {
            switch (jSelector.prop("tagName").toLowerCase()) {
                case "input":
                case "textarea":
                case "select":
                    jSelector.val(value);
                    break
                case "div":
                    jSelector.attr("pararm-var", value);
                    break
                default:
                    jSelector.text(value);
            }

            if (typeof (trigger) === 'function') {
                trigger(jSelector);
            }
        }
    }
}

jQuery.fn.aesToParams = function (ase) {
    return ase.replace(/\+/g, '-')
}

// setup form validation
jQuery.fn.validateSetup = function (target, setting) {
    if (typeof (target) == 'undefined' || target == null) {
        return false;
    }
    let prop = {
        rules: {},
        messages: {},
        ignore: ''
    }

    $.extend(prop, setting);

    target.validate({
        ignore: prop.ignore,
        rules: prop.rules,
        messages: prop.messages
    });
}