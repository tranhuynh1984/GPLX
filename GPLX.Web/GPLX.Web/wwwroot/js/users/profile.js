var Profile = function () {
    let base = this;
    base.$btnUpdate = $('#btnUpdate');
    base.$form = $('#formSettings');

    base.setup = function () {
        var prop = {
            rules: {
                inputAccount: {
                    required: true
                },
                inputPassword: {
                    required: true
                },
                inputRePass: {
                    required: true,
                    equalTo: '#inputPassword'
                }
            },
            messages: {
                inputAccount: {
                    required: 'Bạn chưa nhập tài khoản ký số'
                },
                inputPassword: {
                    required: 'Bạn chưa nhập mật khẩu'
                },
                inputRePass: {
                    required: 'Bạn chưa nhập lại mật khẩu',
                    equalTo: 'Mật khẩu nhập lại không đúng'
                }
            }
        }

        jQuery.fn.validateSetup(base.$form, prop);

        base.$btnUpdate.click(base.onSubmit);
    }

    base.onSubmit = function () {
        if (base.$form.valid()) {
            costJsBase.Post({
                Url: '/Users/OnUpdateProfile',
                Data: {
                    SignatureAcc: $('#inputAccount').val(),
                    SignaturePass: $('#inputPassword').val(),
                    SignatureConfirmPass: $('#inputRePass').val()
                },
                beforeSend: function () {
                    costJsBase.ButtonState({
                        target: $('#btnUpdate'),
                        state: 'loading',
                        disabled: true,
                        text: 'Đang lưu',
                        changePropAllButton: true
                    });
                }
            }, function (data) {
                costJsBase.ButtonState({
                    target: $('#btnUpdate'),
                    state: 'normal',
                    disabled: false,
                    text: 'Lưu lại',
                    changePropAllButton: true
                });

                if (data.code === 200) {
                    setTimeout(function() {
                        window.location.reload();
                    },1500);
                    $('#inputPassword').val('');
                    $('#inputRePass').val('');

                    costJsBase.EventNotify('success', data.msg);
                } else {
                    costJsBase.EventNotify('error', data.msg);
                }
            }, function () {
                costJsBase.ButtonState({
                    target: $('#btnUpdate'),
                    state: 'normal',
                    disabled: false,
                    text: 'Cập nhật',
                    changePropAllButton: true
                });

                costJsBase.EventNotify('error', "Lỗi hệ thống, vui lòng thử lại sau!");

            });
        }
    }
}

$(document).ready(function () {
    var p = new Profile();
    p.setup();
});