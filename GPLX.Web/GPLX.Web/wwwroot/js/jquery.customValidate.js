
jQuery.validator.setDefaults({
    ignore: '',
    errorElement: 'span',
    errorPlacement: function (error, element) {
        error.addClass('invalid-feedback');
        element.closest('.form-group').append(error);
    },
    highlight: function (element, errorClass, validClass) {
        $(element).removeClass('is-valid').addClass('is-invalid');
    },
    unhighlight: function (element) {
        $(element).removeClass('is-invalid').addClass('is-valid');;
    }
});

//==================================================================
//	Description:  		chỉ validate số điện thoại 
//                      nhập vào 09 hoặc 01 và với 09 thì phải nhập 10 ký tự và 01 là 11 ký tự		
//	
//==================================================================
jQuery.validator.addMethod("phone", function (value, element) {
    if (value.length == 0)
        return true;
    var reg = "^[0](9[0-9]{8}|1[0-9]{9}|4[0-9]{8})$";
    reg = new RegExp(reg);
    if (reg.test(value)) {
        return true;
    } else {
        return false;
    };
}, "Số điện thoại không đúng định dạng, vui lòng kiểm tra lại!");

//==================================================================
//	Description:  Kiểm tra số điện thoại đơn giản
//                  chỉ bắt là số và 10-11 ký tự 
//	
//==================================================================
jQuery.validator.addMethod("phonenumber", function (value, element) {
    if (value.length == 0)
        return true;
    var x = value.substring(0, 1);
    if (x == '+') {
        value = value.substring(1, value.length);

    }
    var reg = "^[0-9]{9,12}$";
    reg = new RegExp(reg);
    if (reg.test(value)) {
        return true;
    } else {
        return false;
    };
}, "Số điện thoại không đúng định dạng, gồm 9 hoặc 12 ký tự!");

jQuery.validator.addMethod("phonebase", function (value, element) {
    if (value.length == 0)
        return true;
    var x = value.substring(0, 1);
    if (x == '+') {
        value = value.substring(1, value.length);

    }
    var reg = "^[0-9]{1,20}$";
    reg = new RegExp(reg);
    if (reg.test(value)) {
        return true;
    } else {
        return false;
    };
}, "Số điện thoại không đúng định dạng, gồm 10 hoặc 11 ký tự!");


//==================================================================
//	Description:  Validate số					
//	
//==================================================================
jQuery.validator.addMethod("number", function (value, element) {
    var reg = "^[0-9-]+$";
    reg = new RegExp(reg);
    if (value.length == 0)
        return true;
    else if (reg.test(value)) {
        return true;
    } else {
        return false;
    };
}, "Trường dữ liệu không đúng định dạng, vui lòng kiểm tra lại!");

//==================================================================
//	Description:  validate email đúng định dạng					
//	
//==================================================================
jQuery.validator.addMethod("email", function (value, element) {
    if (value.length == 0)
        return true;
    var reg = "^[a-zA-Z][a-zA-Z0-9\.\_]+@[a-zA-Z][a-zA-Z0-9\.\_]+[a-zA-Z]{2,4}$";

    reg = new RegExp(reg);
    if (reg.test(value)) {
        return true;
    } else {
        return false;
    };
}, "Email không đúng định dạng, vui lòng kiểm tra lại!");

//==================================================================
//	Description:  kiểm tra string UTF8 - Latin
//	
//==================================================================
jQuery.validator.addMethod("utf8string", function (value, element) {
    if (value.length == 0)
        return true;
    var latin = value.latinize().replace(/\s/g, "");
    var regex = "^[a-zA-Z0-9]+$";
    var reg = new RegExp(regex);
    return reg.test(latin);
}, "String không đúng định dạng");

//==================================================================
//	Description:  		validate với select option default = -1
//	
//==================================================================
jQuery.validator.addMethod("selectrequired", function (value, element) {
    if (value == '-1' || value.length == 0) {
        return false;
    } else {
        return true;
    };
}, "Vui lòng chọn!");

//==================================================================
//	Description:  bỏ khoảng trắng	của câu chuyển vào
//              
//	
//==================================================================
jQuery.validator.addMethod("namenotspace", function (value, element) {
    var _value = value.replace(/\s/g, '');

    return _value.length >= 6;


}, "Yêu cầu nhập thông tin ít nhất là 6 đến 250 ký tự !");

//==================================================================
//	Description:  kiểm tra mã All page		
//               không có khoảng trắng, không cho phép nhập ký tự đặc biệt, chấp nhận dấu gạch dưới “_”, .
//	
//==================================================================
jQuery.validator.addMethod("Chosenselection", function (value, element) {
    console.log(typeof value);
    var checkValue = value && Object.keys(value).length > 1;
    //checkValue == obj ko loi
    if (checkValue || (Object.keys(value).length == 1 && value[0].length > 0)) {
        return true;
    } else {
        return false;
    }
}, "Bạn chưa hoàn thiện thông tin, vui lòng kiểm tra lại!");
//==================================================================
//	Description:  từ ngày lớn hơn tới ngày					
//	
//==================================================================
jQuery.validator.addMethod("comparedate", function (value, element, param) {
    try {
        var f = param.format ? param.format : "DD/MM/YYYY";
        var d1 = moment(value, f),
        d2 = moment(param.element.val(), f);
        if (param.type == 1) {
            return d1.valueOf() <= d2.valueOf();
        } else if (param.type == 2) {
            return d1.valueOf() >= d2.valueOf();
        } else {
            return false;
        }
    } catch (e) {
        console.log(e);
        return false;
    }
}, "");

jQuery.validator.addMethod("validAmount", function (value, element) {
    try {
        var amount = parseInt(value.replace(/\./gi, ''));
        //console.log(amount);
        if (amount <= 0 || isNaN(amount)) {
            return false;
        }
        return true;
    } catch (e) {
        return false;
    }
}, "");

$.validator.addMethod('compareWithNow', function (value, element, param) {
    var curent = moment();
    var momentA = moment(value, "DD/MM/YYYY hh:mm:ss");
    return momentA > curent;

}, 'Thời gian bắt đầu phải lớn hơn hiện tại');

$.validator.addMethod('compareDateTime', function (value, element, param) {
    var momentA = moment(value, "DD/MM/YYYY HH:mm:ss");
    var momentB = moment($(param).val(), "DD/MM/YYYY HH:mm:ss");
    return this.optional(element) || momentA > momentB;

}, 'Thời gian bắt đầu phải lớn hơn hiện tại');

jQuery.validator.addMethod("validPassLength", function (value, element) {
    if (value.length < 8)
        return false;
    else {
        return true;
    }
}, "Mật khẩu phải từ 8 ký tự trở lên");

$.validator.addMethod('codeFormat', function (value, element, param) {
    var reg = "[A-Za-z0-9]+$";
    reg = new RegExp(reg);
    if (reg.test(value)) {
        return true;
    } else {
        return false;
    };

}, 'Mã không đúng định dạng');