var languages = {
    vi: {
        costElementItem: {
            approve: {
                largeText: 'Bạn có chắc chắn muốn phê duyệt yêu cầu này?',
                smallText: '',
                icon: '&#xe876;',
                buttonOkClass: 'btn-success',
                popupType: 'success',
            },
            decline: {
                largeText: 'Bạn có chắc chắn muốn từ chối yêu cầu này ?',
                smallText: '',
                icon: '&#xe611;',
                buttonOkClass: 'btn-success',
                popupType: 'error'
            },
            list: {
                popup: {
                    title: '{___} phiếu yêu cầu',
                    baseTitle: '{___} phiếu yêu cầu',
                    viewLabel: 'Xem chi tiết',
                    historyLabel: 'Xem lịch sử',
                    editLabel: 'Chỉnh sửa',
                    replator: '{___}',
                },
                table: {
                    editLabel: 'Chỉnh sửa yêu cầu',
                    viewLabel: 'Xem chi tiết yêu cầu',
                    viewHistoryLabel: 'Xem lịch sử phê duyệt',
                    deleteLabel: 'Xóa yêu cầu',
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        costElement: {
            create: {
                invalidExcelFileWarning: 'Tệp được chọn không đúng định dạng (.xlsx)',
                maxsizeWarning: 'Tệp được chọn vượt quá dung lượng cho phép!',
                classHidden: 'd-none',
                classInline: 'd-inline-block',
                pendingUpload: '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Đang tải lên ...',
                oldButtonUpload: '<i class="fas fa-arrow-circle-up mr-1"></i><span>Tải lên</span>',
                oldButtonUploadNoContent: '<i class="fas fa-arrow-circle-up"></i>',
                onTableUploadButton: `<button prop-type="table-contract-button" class="btn btn-sm btn-outline-success mr-2 __CLASS__" type="button" title="Tải lên chứng từ"> <i class="fas fa-paperclip"></i></button><input type="file" class="d-none" prop-type="table-contract-file" accept=".xlsx, .doc, .docx, .png, .jpg, .gif, .pdf, .csv"/>`
            }
        },
        actually: {
            approve: {
                largeText: 'Bạn có chắc chắn muốn phê duyệt báo cáo thực chi này?',
                smallText: '',
                icon: '&#xe876;',
                buttonOkClass: 'btn-success',
                popupType: 'success',
            },
            decline: {
                largeText: 'Bạn có chắc chắn muốn từ chối báo cáo thực chi này ?',
                smallText: '',
                icon: '&#xe14c;',
                buttonOkClass: 'btn-success',
                popupType: 'error'
            },
            list: {
                popup: {
                    title: '{___} báo cáo thực chi',
                    baseTitle: '{___} báo cáo thực chi',
                    viewLabel: 'Xem chi tiết',
                    replator: '{___}',
                },
                table: {
                    editLabel: 'Chỉnh sửa báo cáo thực chi',
                    viewLabel: 'Xem chi tiết - phê duyệt báo cáo thực chi',
                    aproveLabel: 'Phê duyệt báo cáo thực chi',
                    viewHistoryLabel: 'Xem lịch sử phê duyệt'
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        all: {
            okButtonText: 'Xác nhận',
            okButtonClass: 'btn-success',
            cancelButtonText: 'Hủy bỏ',
            dateFormat: 'DD/MM/YYYY'
        },
        costEstimate: {
            approve: {
                largeText: 'Bạn có chắc chắn muốn phê duyệt dự trù này?',
                smallText: '',
                icon: '&#xe876;',
                buttonOkClass: 'btn-success',
                popupType: 'success',
            },
            decline: {
                largeText: 'Bạn có chắc chắn muốn từ chối dự trù này ?',
                smallText: '',
                icon: '&#xe611;',
                buttonOkClass: 'btn-success',
                popupType: 'error'
            },
            list: {
                popup: {
                    title: '{___} dự trù',
                    baseTitle: '{___} dự trù',
                    viewLabel: 'Xem chi tiết',
                    historyLabel: 'Xem lịch sử',
                    editLabel: 'Chỉnh sửa',
                    replator: '{___}',
                },
                table: {
                    editLabel: 'Chỉnh sửa dự trù',
                    viewLabel: 'Xem chi tiết dự trù',
                    viewHistoryLabel: 'Xem lịch sử phê duyệt'
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        cashFollow: {
            approve: {
                largeText: 'Bạn có chắc chắn muốn phê duyệt kế hoạch dòng tiền này?',
                smallText: '',
                icon: '&#xe876;',
                buttonOkClass: 'btn-success',
                popupType: 'success',
            },
            decline: {
                largeText: 'Bạn có chắc chắn muốn từ chối kế hoạch dòng tiền này ?',
                smallText: '',
                icon: '&#xe14c;',
                buttonOkClass: 'btn-success',
                popupType: 'error'
            },
            list: {
                popup: {
                    title: '{___} kế hoạch dòng tiền',
                    baseTitle: '{___} kế hoạch dòng tiền',
                    viewLabel: 'Xem chi tiết',
                    historyLabel: '',
                    replator: '{___}',
                },
                table: {
                    editLabel: 'Chỉnh sửa kế hoạch dòng tiền',
                    viewLabel: 'Xem chi tiết kế hoạch dòng tiền',
                    aproveLabel: 'Phê duyệt kế hoạch dòng tiền',
                    deleteLabel: 'Xóa kế hoạch',
                    declineLabel: 'Từ chối kế hoạch dòng tiền',
                    compareLabel: 'So sánh ngân sách & thực chi',
                    historyLabel: 'Lịch sử phê duyệt'
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            },
            create: {
                invalidExcelFileWarning: 'Tệp được chọn không đúng định dạng (.xlsx)',
                maxsizeWarning: 'Tệp được chọn vượt quá dung lượng cho phép!',
                classHidden: 'd-none',
                classInline: 'd-inline-block',
                pendingUpload: '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Đang phân tích ...',
                oldButtonUpload: '<i class="fas fa-arrow-circle-up mr-1"></i><span>Tải lên</span>',
                oldButtonUploadNoContent: '<i class="fas fa-arrow-circle-up"></i>'
            }
        },
        department: {
            list: {
                popup: {
                    title: '{___} phòng ban',
                    baseTitle: '{___} phòng ban',
                    viewLabel: 'Xem chi tiết',
                    editLabel: 'Chỉnh sửa',
                    replator: '{___}',
                },
                table: {
                    editLabel: 'Chỉnh sửa phòng ban',
                    viewLabel: 'Xem chi tiết phòng ban'
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        groups: {
            list: {
                popup: {
                    title: '{___} chức vụ',
                    baseTitle: '{___} chức vụ',
                    viewLabel: 'Xem thông tin chi tiết',
                    editLabel: 'Chỉnh sửa',
                    replator: '{___}',
                },
                table: {
                    editLabel: 'Chỉnh sửa chức vụ',
                    viewLabel: 'Xem thông tin chi tiết chức vụ'
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        units: {
            list: {
                popup: {
                    title: '{___}',
                    baseTitle: '{___}',
                    viewLabel: 'Xem thông tin chi tiết',
                    editLabel: 'Chỉnh sửa',
                    replator: '{___}',
                },
                table: {
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        investment: {
            approve: {
                largeText: 'Bạn có chắc chắn muốn phê duyệt kế hoạch đầu tư này?',
                smallText: '',
                icon: '&#xe876;',
                buttonOkClass: 'btn-success',
                popupType: 'success',
            },
            decline: {
                largeText: 'Bạn có chắc chắn muốn từ chối kế hoạch đầu tư này ?',
                smallText: '',
                icon: '&#xe611;',
                buttonOkClass: 'btn-success',
                popupType: 'error'
            },
            list: {
                popup: {
                    title: '{___} kế hoạch đầu tư',
                    baseTitle: '{___} kế hoạch đầu tư',
                    viewLabel: 'Xem chi tiết',
                    historyLabel: 'Xem lịch sử',
                    editLabel: 'Chỉnh sửa',
                    replator: '{___}',
                },
                table: {
                    editLabel: 'Chỉnh sửa kế hoạch đầu tư',
                    viewLabel: 'Xem chi tiết kế hoạch đầu tư',
                    viewHistoryLabel: 'Xem lịch sử phê duyệt'
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        revenue: {
            approve: {
                largeText: 'Bạn có chắc chắn muốn phê duyệt kế hoạch này?',
                smallText: '',
                icon: '&#xe876;',
                buttonOkClass: 'btn-success',
                popupType: 'success',
            },
            decline: {
                largeText: 'Bạn có chắc chắn muốn từ chối kế hoạch này ?',
                smallText: '',
                icon: '&#xe611;',
                buttonOkClass: 'btn-success',
                popupType: 'error'
            },
            list: {
                popup: {
                    title: '{___} kế hoạch doanh thu khách hàng',
                    baseTitle: '{___} kế hoạch doanh thu khách hàng',
                    viewLabel: 'Xem chi tiết',
                    historyLabel: 'Xem lịch sử',
                    editLabel: 'Chỉnh sửa',
                    replator: '{___}',
                },
                table: {
                    editLabel: 'Chỉnh sửa kế hoạch doanh thu khách hàng',
                    viewLabel: 'Xem chi tiết kế hoạch doanh thu khách hàng',
                    viewHistoryLabel: 'Xem lịch sử phê duyệt'
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        profit: {
            create: {
                invalidExcelFileWarning: 'Tệp được chọn không đúng định dạng (.xlsx)',
                maxsizeWarning: 'Tệp được chọn vượt quá dung lượng cho phép!',
                classHidden: 'd-none',
                classInline: 'd-inline-block',
                pendingUpload: '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Đang tải lên ...',
                oldButtonUpload: '<i class="fas fa-arrow-circle-up mr-1"></i><span>Tải lên</span>',
                oldButtonUploadNoContent: '<i class="fas fa-arrow-circle-up"></i>',
                onTableUploadButton: `<button prop-type="table-contract-button" class="btn btn-sm btn-outline-success mr-2 __CLASS__" type="button" title="Tải lên chứng từ"> <i class="fas fa-paperclip"></i></button><input type="file" class="d-none" prop-type="table-contract-file" accept=".xlsx, .doc, .docx, .png, .jpg, .gif, .pdf, .csv"/>`
            },
            approve: {
                largeText: 'Bạn có chắc chắn muốn phê duyệt kế hoạch này ?',
                smallText: '',
                icon: '&#xe876;',
                buttonOkClass: 'btn-success',
                popupType: 'success',
            },
            decline: {
                largeText: 'Bạn có chắc chắn muốn từ chối kế hoạch này ?',
                smallText: '',
                icon: '&#xe611;',
                buttonOkClass: 'btn-success',
                popupType: 'error'
            },
            list: {
                popup: {
                    title: '{___} kế hoạch lợi nhuận',
                    baseTitle: '{___} kế hoạch lợi nhuận',
                    viewLabel: 'Xem chi tiết',
                    historyLabel: 'Xem lịch sử',
                    editLabel: 'Chỉnh sửa',
                    replator: '{___}',
                },
                table: {
                    editLabel: 'Chỉnh sửa kế hoạch lợi nhuận',
                    viewLabel: 'Xem chi tiết kế hoạch lợi nhuận',
                    viewHistoryLabel: 'Xem lịch sử phê duyệt'
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        DMPN: {
            list: {
                popup: {
                    title: '{___}',
                    baseTitle: '{___}',
                    viewLabel: 'Xem thông tin chi tiết',
                    editLabel: 'Chỉnh sửa',
                    replator: '{___}',
                },
                table: {
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        DMDV: {
            list: {
                popup: {
                    title: '{___}',
                    baseTitle: '{___}',
                    viewLabel: 'Xem thông tin chi tiết',
                    editLabel: 'Chỉnh sửa',
                    replator: '{___}',
                },
                table: {
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        DMHuyen: {
            list: {
                popup: {
                    title: '{___}',
                    baseTitle: '{___}',
                    viewLabel: 'Xem thông tin chi tiết',
                    editLabel: 'Chỉnh sửa',
                    replator: '{___}',
                },
                table: {
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        DMTinh: {
            list: {
                popup: {
                    title: '{___}',
                    baseTitle: '{___}',
                    viewLabel: 'Xem thông tin chi tiết',
                    editLabel: 'Chỉnh sửa',
                    replator: '{___}',
                },
                table: {
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        DM: {
            list: {
                popup: {
                    title: '{___}',
                    baseTitle: '{___}',
                    viewLabel: 'Xem thông tin chi tiết',
                    editLabel: 'Chỉnh sửa',
                    replator: '{___}',
                },
                table: {
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        HDKCB: {
            list: {
                popup: {
                    title: '{___}',
                    baseTitle: '{___}',
                    viewLabel: 'Xem thông tin chi tiết',
                    editLabel: 'Chỉnh sửa',
                    replator: '{___}',
                },
                table: {
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        DMCP: {
            list: {
                popup: {
                    title: '{___}',
                    baseTitle: '{___}',
                    viewLabel: 'Xem thông tin chi tiết',
                    editLabel: 'Chỉnh sửa',
                    replator: '{___}',
                },
                table: {
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        DMBS_ChuyenKhoa: {
            list: {
                popup: {
                    title: '{___}',
                    baseTitle: '{___}',
                    viewLabel: 'Xem thông tin chuyên khoa',
                    createLabel: 'Thêm mới thông tin chuyên khoa',
                    editLabel: 'Chỉnh sửa chuyên khoa',
                    replator: '{___}',
                },
                table: {
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        LoaiDeXuat: {
            list: {
                popup: {
                    title: '{___}',
                    baseTitle: '{___}',
                    viewLabel: 'Xem thông tin chi tiết loại đề xuất',
                    createLabel: 'Thêm mới loại đề xuất',
                    editLabel: 'Chỉnh sửa loại đề xuất',
                    replator: '{___}',
                },
                table: {
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        DMCKDoiTuong: {
            list: {
                popup: {
                    title: '{___}',
                    baseTitle: '{___}',
                    viewLabel: 'Xem thông tin chiết khấu đối tượng',
                    createLabel: 'Thêm mới chiết khấu đối tượng',
                    editLabel: 'Chỉnh sửa chiết khấu đối tượng',
                    replator: '{___}',
                },
                table: {
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        DMDoiTuong: {
            list: {
                popup: {
                    title: '{___}',
                    baseTitle: '{___}',
                    viewLabel: 'Xem thông tin đối tượng',
                    createLabel: 'Thêm mới đối tượng',
                    editLabel: 'Chỉnh sửa đối tượng',
                    replator: '{___}',
                },
                table: {
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        DMChucDanh: {
            list: {
                popup: {
                    title: '{___}',
                    baseTitle: '{___}',
                    viewLabel: 'Xem thông tin chức danh',
                    createLabel: 'Thêm mới chức danh',
                    editLabel: 'Chỉnh sửa chức danh',
                    replator: '{___}',
                },
                table: {
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },

        DMCTV: {

            list: {
                popup: {
                    title: '{___}',
                    baseTitle: '{___}',
                    viewLabel: 'Xem thông tin hợp đồng cộng tác viên',
                    createLabel: 'Thêm mới hợp đồng cộng tác viên',
                    editLabel: 'Chỉnh sửa hợp đồng cộng tác viên',
                    replator: '{___}',
                },
                table: {
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        HDCTV: {

            list: {
                popup: {
                    title: '{___}',
                    baseTitle: '{___}',
                    viewLabel: 'Xem thông tin hợp đồng cộng tác viên',
                    createLabel: 'Thêm mới hợp đồng cộng tác viên',
                    editLabel: 'Chỉnh sửa hợp đồng cộng tác viên',
                    replator: '{___}',
                },
                table: {
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        ProfileCK: {
            list: {
                popup: {
                    title: '{___}',
                    baseTitle: '{___}',
                    viewLabel: 'Xem thông tin chi tiết profile CK',
                    createLabel: 'Thêm mới profile CK',
                    editLabel: 'Chỉnh sửa profile CK',
                    replator: '{___}',
                },
                table: {
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        },
        Relationship: {
            list: {
                popup: {
                    title: '{___}',
                    baseTitle: '{___}',
                    viewLabel: 'Xem thông tin chi tiết quan hệ',
                    createLabel: 'Thêm mới quan hệ',
                    editLabel: 'Chỉnh sửa quan hệ',
                    replator: '{___}',
                },
                table: {
                }
            },
            history: {},
            all: {
                okButtonText: 'Xác nhận',
                okButtonClass: 'btn-success',
                cancelButtonText: 'Hủy bỏ',
                dateFormat: 'DD/MM/YYYY'
            }
        }
    },

    locale: 'vi'
}