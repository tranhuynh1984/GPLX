var CashFollowCompare = function () {
    let base = this;
    base.$tableDataTable = null;
    base.$comparing = $('#comparing');
    base.$lowerDate = $('#mrp-lowerDate');
    base.$upperDate = $('#mrp-upperDate');
    let $tableSelector = $('#comparingTable');
    base.$cashFollowRecord = $('#___cashFollowRecord');
    base.inputTableSearch = $('#__inputTableSearch');
    base.$btnTableSearch = $('#__btnTableSearch');

    base.columnTableConfigs = [
        {
            idx: 0,
            render: function (data, type, row, meta) {
                return row.no;
            },
            width: "auto",
            class: 'align-middle text-center',
            fieldName: 'no',
            data: "no"
        },
        // Nội dung
        {
            idx: 1,
            render: function (data, type, row, meta) {
                return '<span class="' + (row.isBold ? 'font-weight-bold' : '') + '">' + row.content + '</span>';
            },
            width: "auto",
            class: 'align-middle text-left',
            fieldName: 'content',
            data: "content"
        },
        // Ngân sách lũy kế
        {
            idx: 2,
            render: function (data, type, row, meta) {
                return '<span class="' +
                    (row.isBold ? 'font-weight-bold' : '') +
                    '">' +
                    costJsBase.FormatMoney(row.cumulativeBudget, '-') +
                    '</span>'
            },
            width: "auto",
            class: 'text-right align-middle',
            fieldName: 'cumulativeBudget',
            data: 'cumulativeBudget'
        },
        // Thực chi lũy kế
        {
            idx: 3,
            render: function (data, type, row, meta) {
                return '<span class="' +
                    (row.isBold ? 'font-weight-bold' : '') +
                    '">' +
                    costJsBase.FormatMoney(row.cumulativeActuallySpent, '-') +
                    '</span>'
            },
            class: "text-right align-middle",
            width: "auto",
            fieldName: 'cumulativeActuallySpent',
            data: 'cumulativeActuallySpent'

        },
        // Vượt chi
        {
            idx: 4,
            render: function (data, type, row, meta) {
                return '<span class="' +
                    (row.isBold ? 'font-weight-bold' : '') +
                    '">' +
                    costJsBase.FormatMoney(row.overBudget, '-') +
                    '</span>'
            },
            width: "auto",
            class: "align-middle text-right",
            fieldName: 'overBudget',
            data: 'overBudget'

        },
        // Tỉ lệ ngân sách
        {
            idx: 5,
            render: function (data, type, row, meta) {
                return '<span class="' +
                    (row.isBold ? 'font-weight-bold' : '') +
                    '">' +
                    costJsBase.FormatMoney(row.budgetRate) +
                    '%' +
                    '</span>'
            },
            class: "text-right align-middle",
            width: "auto",
            fieldName: 'budgetRate',
            data: 'budgetRate'

        },
        // Tỉ lệ thực tế
        {
            idx: 6,
            render: function (data, type, row, meta) {
                return '<span class="' +
                    (row.isBold ? 'font-weight-bold' : '') +
                    '">' +
                    costJsBase.FormatMoney(row.actuallyRate) +
                    '%' +
                    '</span>'
            },
            class: "text-right align-middle",
            width: "auto",
            fieldName: 'actuallyRate',
            data: 'actuallyRate'
        },
        // Phân loại
        {
            idx: 7,
            render: function (data, type, row, meta) {
                return row.typeName;
            },
            class: "text-right align-middle",
            width: "auto",
            fieldName: 'typeName',
            data: 'typeName'

        },
        // Tỉ lệ vượt chi
        {
            idx: 8,
            render: function (data, type, row, meta) {
                if (row.overBudgetRate > 0) {
                    return '<span class="warning-rate">' + costJsBase.FormatMoney(row.overBudgetRate) + '%' + '</span>';
                } else {
                    return costJsBase.FormatMoney(row.overBudgetRate) + '%';
                }
            },
            class: "text-right align-middle",
            width: "auto",
            fieldName: 'overBudgetRate',
            data: 'overBudgetRate'

        }
    ];

    base.setup = function () {
        base.$tableDataTable = $.fn.jsTableRegister({
            selector: $tableSelector,
            columns: base.columnTableConfigs,
            drawCallback: function () { },
            searching: true,
            scrollX: true,
            responsive: false,
            altEditor: true,
            select: true,
            paging: false,
            autoWidth: true,
            fixedHeader: true,
            serverSide: false,
            fixedColumns: base.fixedColumns,
            iDisplayLength: 100,
            // checkbox search
            initComplete: function () {
                $.fn.dataTable.ext.search.push(
                    function (settings, searchData, index, rowData, counter) {
                        var searchKeys = base.inputTableSearch.val();
                        if (!searchKeys.length)
                            return true;
                        else {
                            let valid = true;
                            if (searchKeys.length) {
                                valid = rowData.content.toLowerCase().indexOf(searchKeys.toLowerCase()) !== -1 || rowData.typeName?.toLowerCase().indexOf(searchKeys.toLowerCase()) !== -1;
                            }
                            return valid;
                        }
                    }
                );
            },
            language: {
                sZeroRecords: 'Chưa chọn thời gian cần so sánh'
            }
        });
        base.$comparing.on('click', base.start);
        base.inputTableSearch.keyup(function () {
            base.$tableDataTable.draw();
        });
        base.$btnTableSearch.on('click',
            function() {
                base.$tableDataTable.draw();
            });
    }

    base.start = function () {
        let lowerValue = base.$lowerDate.val();
        let upperValue = base.$upperDate.val();

        let yearLower = parseInt(lowerValue * 1 / 100);
        let yearUpper = parseInt(upperValue * 1 / 100);

        if (yearLower != yearUpper) {
            costJsBase.EventNotify('warning', 'Vui lòng chọn khoảng thời gian trong cùng một năm!');
            return false;
        }

        let monthLower = lowerValue.substr(4);
        let monthUpper = upperValue.substr(4);
        if (monthLower > monthUpper) {
            costJsBase.EventNotify('warning', 'Vui lòng chọn khoảng thời gian hợp lệ!');
            return false;
        }

        costJsBase.Post({
            Url: '/CashFollow/Comparing',
            Data: {
                Record: base.$cashFollowRecord.val(),
                FromMonth: monthLower,
                ToMonth: monthUpper,
                Year: yearLower
            }
        },
            base.onSuccess,
            base.onFailure);
    }

    base.onSuccess = function (data) {
        console.log(data);
        if (data.code === costJsBase.enums.successCode) {
            base.$tableDataTable.clear();
            base.$tableDataTable.rows.add(data.data).draw(false);
        } else {
            costJsBase.EventNotify('error', data.message);
        }
    }

    base.onFailure = function (err) {
        console.log(err);
        costJsBase.EventNotify('error', 'Lỗi hệ thống, vui lòng thử lại sau!');
    }
}
$(document).ready(function () {
    var com = new CashFollowCompare();
    com.setup();
});