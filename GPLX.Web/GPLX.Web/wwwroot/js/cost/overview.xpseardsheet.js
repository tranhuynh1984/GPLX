var a;
var Overview = function () {
    let base = this;
    let $save = $('#save');
    var xs;
    var edited = [];
    base.cfgLang = languages.vi.costElementItem;


    base.Save = function () {
        if (confirm('Bạn có muốn lưu dự trù này ?')) {
            var data = xs.getData()[0].rows;
            if (data) {
                let config = {
                    Url: '/CostEstimate/OverviewSave',
                    Data: {
                        record,
                        data: rows
                    },
                    async: true,
                    beforeSend: function () {
                        costJsBase.ButtonState({
                            target: $save,
                            state: 'loading',
                            disabled: true,
                            text: 'Đang lưu',
                            changePropAllButton: true
                        });
                    },
                    complete: function (data) {
                        costJsBase.ButtonState({
                            target: $save,
                            state: data.code === costJsBase.enums.successCode ? 'done' : 'normal',
                            disabled: data.code === costJsBase.enums.successCode,
                            text: data.code === costJsBase.enums.successCode ? '' : base.cfgLang.all.okButtonText,
                            html: data.code !== costJsBase.enums.successCode ? '' : '<i class="fa fa-check mr-2"></i> Đã lưu',
                            changePropAllButton: true
                        }).then(function (target) {
                            if (data.code === costJsBase.enums.successCode) {
                                var new_wb = xtos(xs.getData());
                                XLSX.writeFile(new_wb, "SheetJS.xlsx");
                            }
                        });
                    },
                };

                costJsBase.Post(config,
                    function (data) {
                        if (typeof (config.complete) === 'function')
                            config.complete(data);
                        costJsBase.EventNotify(data.code === costJsBase.enums.successCode ? 'success' : 'error', data.message);
                    }, function () {

                        costJsBase.ButtonState({
                            target: $save,
                            state: 'normal',
                            disabled: false,
                            text: 'Lưu',
                            changePropAllButton: true
                        });

                        costJsBase.EventNotify('error', 'Có lỗi xảy ra, vui lòng thử lại sau!');
                    }
                );
            } else {
                costJsBase.EventNotify('error', 'Không lấy được dữ liệu để lưu, vui lòng thử lại!');
            }
        }
    }

    base.bindActions = function () {
        $save.bind("click", function (e) {
            base.Save();
        });
    }

    base.Setup = function () {
        base.bindActions();
        xs = x_spreadsheet('#x-spreadsheet', {
            showToolbar: true,
            showGrid: true,
            showBottomBar: true,
            view: {
                height: () => document.documentElement.clientHeight - 310,
                width: () => document.getElementById("x-spreadsheet").clientWidth,
            },
        }).loadData([{
            name: "sheet1",
            styles: [
                {
                    bgcolor: '#f4b084',
                    textwrap: true,
                    color: '#000000',
                    align: 'center',
                    font: {
                        bold: true,
                    },
                },
                {
                    bgcolor: '#e2efd9',
                    textwrap: true,
                    color: '#000000',
                    align: 'left',
                    font: {
                        bold: true,
                    },
                },
                {
                    textwrap: true,
                    color: '#000000',
                    align: 'left',
                },
                {
                    bgcolor: '#deeaf6',
                    textwrap: true,
                    color: '#000000',
                    align: 'left',
                    font: {
                        bold: true,
                    },
                },
                {
                    bgcolor: '#fef2cb',
                    textwrap: true,
                    color: '#000000',
                    align: 'left',
                    font: {
                        bold: true,
                    },
                },
                {
                    bgcolor: '#e7e6e6',
                    textwrap: true,
                    color: '#000000',
                    align: 'left',
                    font: {
                        bold: true,
                    },
                },
                {
                    bgcolor: '#e7e6e6',
                    textwrap: true,
                    color: '#000000',
                    align: 'left',
                    font: {
                        bold: true,
                    },
                },
            ],
            cols: {
                len: 5,
                0: { width: 150 },
                1: { width: 800 },
                2: { width: 150 },
                3: { width: 250 },
                4: { width: 150 },
            },
            rows: rows,
        }]).change((cdata) => {
            // console.log(cdata);
            //console.log('>>>', xs.getData());
        });

        xs.on('cell-edited', (text, ri, ci) => {
            if (text && !isNaN(text.replace(".", "").replace(",", ""))) {
                var temp = costJsBase.FormatMoneyComma(text.replace(".", "").replace(",", ""));
                edited.push({
                    row: ri,
                    col: ci,
                    text : temp
                })
            }

        });

        xs.on('cell-selected', (cell, ri, ci) => {
            for (var i = 0; i < edited.length; i++) {
                xs.cellText(edited[i].row, edited[i].col, edited[i].text).reRender();
                edited.shift();

            }

        });

        a = xs;
    }

}

$(document).ready(function () {
    let c = new Overview();
    c.Setup();
});