let OrdersManager = function () {
    return {
        Datatable: null,
        LoadDataTable: function () {
            var table = $('#dtOrders').DataTable({
                "processing": true,
                "serverSide": true,
                "ajax": {
                    "url": "/Orders/Paginate",
                    "type": "POST",
                    "data": function (d) {
                        d.isCollected = $('#statusCollectedFilter').val();
                        d.dtFrom = $('#dtFromFilter').val();
                        d.dtTo = $('#dtToFilter').val();
                    }
                },
                "columns": [
                    { "data": "index", "name": "index", "autoWidth": true },
                    { "data": "id", "name": "id", "autoWidth": true },
                    { "data": "customerId", "name": "customerId", "autoWidth": true },
                    { "data": "customerName", "name": "customerName", "autoWidth": true },
                    { "data": "orderDateStr", "name": "orderDate", "autoWidth": true },
                    { "data": "returnDateStr", "name": "returnDate", "autoWidth": true },
                    { "data": "totalAmount", "name": "totalAmount", "autoWidth": true },
                    { "data": "paidAmount", "name": "paidAmount", "autoWidth": true },
                    { "data": "balanceAmount", "name": "balanceAmount", "autoWidth": true },
                    {
                        "data": null,
                        "render": function (data, type, row) {
                            var res = '';
                            var total = Number(row.totalAmount);
                            var paid = Number(row.paidAmount);
                            var balance = Number(row.balanceAmount);
                            if (balance == 0)
                                res = "<span class='badge badge-success p-1'>Paid</span>";
                            else if (balance == total)
                                res = "<span class='badge badge-danger p-1'>Not Paid</span>";
                            else
                                res = "<span class='badge badge-warning p-1'>Partial Paid</span>";
                            console.log(total, paid, balance)
                            return res;
                        }
                    },

                    {
                        "data": null,
                        "render": function (data, type, row) {
                            var res = '';
                            res = '<div class="btn-group"><button type="button" class="btn btn-info dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Actions</button>';
                            res += '<div class="dropdown-menu dropdown-menu-right">';

                            if (row.isDeleted) {
                                res += '<button class="dropdown-item" type="button" onclick="OrdersManager.RestoreOrders(' + row.id + ')"><i class="fa fa-recycle text-sm"></i></button>';
                            } else {
                                if (!row.isCollected) {
                                    res += '<button title="Edit" class="dropdown-item" type="button"  onclick="OrdersManager.LoadSaveForm(' + row.id + ')"><i class="fa fa-edit text-sm"></i> Edit Order</button>';
                                }
                                res += '<button title="Send SMS" class="dropdown-item" type="button"  onclick="OrdersManager.SendSms(' + row.id + ')"><i class="fa fa-sms text-sm"></i> Send SMS</button>';
                                res += '<button title="Download Invoice" class="dropdown-item" type="button"  onclick="OrdersManager.DownloadInvoice(' + row.id + ')"><i class="fa fa-print text-sm"></i> Download Invoice</button>';
                                res += '<a title="Preview Invoice" class="dropdown-item" type="button" target="_blank" href="/Orders/GetInvoice?ID=' + row.id + '"><i class="fa fa-eye text-sm"></i> Preview Invoice</a>';
                                res += '<button title="Preview Measurements" class="dropdown-item" type="button"  onclick="OrdersManager.PreviewMeasurements(' + row.id + ')"><i class="fa fa fa-scissors text-sm"></i> Preview Measurements</button>';

                                if (!row.isReady) {
                                    res += '<button title="Mark as Ready" class="dropdown-item" type="button"  onclick="OrdersManager.MarkAsReady(' + row.id + ')"><i class="fa fa-check text-sm"></i> Set as Ready</button>';
                                }

                                if (row.isReady && !row.isCollected) {
                                    res += '<button title="Mark as Collected" class="dropdown-item" type="button"  onclick="OrdersManager.MarkAsCollected(' + row.id + ')"><i class="fa fa-shopping-basket text-sm"></i> Set as Collected</button>';
                                }
                            }
                            res += '</div></div>'
                            return res;
                        }
                    },
                ],
                "language": {
                    "processing": '<i class="fa fa-spinner fa-spin fa-3x fa-fw"></i><span class="sr-only">Loading...</span>',
                    "info": "Showing _START_ to _END_ of _TOTAL_ entries",
                    "infoFiltered": "",
                    "emptyTable": "<img src='../img/no-record-found.png' style='width:150px;'/>"
                },
                "initComplete": function () {

                    var ageFilterDropdown = '<label class=" ml-2"><select id="statusCollectedFilter" class="form-control form-control-sm">';
                    ageFilterDropdown += '<option value="false">In Store</option>';
                    ageFilterDropdown += '<option value="true">Collected</option>';
                    ageFilterDropdown += '</select></label>';
                    $('#dtOrders_filter').append(ageFilterDropdown);

                    var ageFilterDropdown = '<label class=" ml-2">From: <input type="date" id="dtFromFilter" class="form-control form-control-sm"></label>';
                    $('#dtOrders_filter').append(ageFilterDropdown);

                    var ageFilterDropdown = '<label class=" ml-2">To: <input type="date" id="dtToFilter" class="form-control form-control-sm"></label>';
                    $('#dtOrders_filter').append(ageFilterDropdown);

                    // Add event listener to the dropdown for filtering
                    $('#statusCollectedFilter, #dtFromFilter, #dtToFilter').on('change', function () {
                        table.draw();
                    });
                    $('#dtFromFilter, #dtToFilter').on('change', function () {
                        var dtFrom = $('#dtFromFilter').val();
                        var dtTo = $('#dtToFilter').val();
                        if (dtFrom != '' && dtTo != '' && dtFrom > dtTo) {
                            showMessage('Oops!', 'From date must be before end day!', 'warning', null);
                            $('#dtToFilter').val('')
                        } else {
                            table.draw();
                        }
                    });

                    $('#dtOrders_wrapper').children().first().children().first().removeClass('col-md-6').addClass('col-md-3');
                    $('#dtOrders_wrapper').children().first().children().last().removeClass('col-md-6').addClass('col-md-9');
                },
                "columnDefs": [
                    { "orderable": false, "targets": [0, -1], "width": "auto" }

                ]
            });
            OrdersManager.Datatable = table;
        },
        LoadSaveForm: function (id) {
            showLoader();
            window.location.href = '/Orders/GetSaveForm?ID=' + id;
        },
        MarkAsReady: function (id) {
            showAlert('Ready', 'Are you sure, you want to mark order as ready?', "warning", true, "Cancel", "Ready", function () {
                showLoader();
                $.ajax({
                    url: '/Orders/MarkAsReady',
                    type: 'post',
                    data: { ID: id },
                    success: function (data) {
                        hideLoader();
                        if (data.status) {
                            showAlert('Success', data.msg, "success", false, "Cancel", "OK", function () {
                                OrdersManager.Datatable.draw();
                            });
                        } else {
                            showAlert('Oops', data.msg, "warning", false, "Cancel", "OK");
                        }
                    },
                    error: function () {
                        hideLoader();
                        showAlert('Oops', 'Something went wrong! Try again letter', "error", false, "Cancel", "OK");
                    }
                })

            })
        },
        MarkAsCollected: function (id) {
            showAlert('Collection', 'Are you sure, you want to mark as collected?', "warning", true, "Cancel", "Collect", function () {
                showLoader();
                $.ajax({
                    url: '/Orders/MarkAsCollected',
                    type: 'post',
                    data: { ID: id },
                    success: function (data) {
                        hideLoader();
                        if (data.status) {
                            showAlert('Success', data.msg, "success", false, "Cancel", "OK", function () {
                                OrdersManager.Datatable.draw();
                            });
                        } else {
                            showAlert('Oops', data.msg, "warning", false, "Cancel", "OK");
                        }
                    },
                    error: function () {
                        hideLoader();
                        showAlert('Oops', 'Something went wrong! Try again letter', "error", false, "Cancel", "OK");
                    }
                })

            })
        },
        SaveOrders: function () {
            if ($('#form-save-order').valid()) {
                debugger
                var total = Number($('#TotalAmount').val());
                var paid = Number($('#PaidAmount').val());

                if (paid > total) {
                    showMessage('Oops!', 'کل رقم ادا کی گئی رقم سے زیادہ ہونی چاہیے', 'warning');
                    return;
                }

                showAlert('Confirmation', 'Are you sure, you want to take order?', "warning", true, "Cancel", "Save", function () {
                    showLoader();
                    $.ajax({
                        url: '/Orders/SaveOrder',
                        type: 'post',
                        data: $('#form-save-order').serialize(),
                        success: function (data) {
                            hideLoader();
                            if (data.status) {
                                showAlert('Success', data.msg, "success", false, "Cancel", "OK", function () {
                                    window.location.href = "/Orders/GetInvoice?ID=" + data.id;
                                });
                            } else {
                                showAlert('Oops', data.msg, "warning", false, "Cancel", "OK");
                            }
                        },
                        error: function () {
                            hideLoader();
                            showAlert('Oops', 'Something went wrong! Try again letter', "error", false, "Cancel", "OK");
                        }
                    })

                })
            }
        },
        ClearRow: function () {
            $('#qamar').val('');
            $('#chest').val('');
            $('#neck').val('');
            $('#arm').val('');
            $('#tera').val('');
            $('#shirtLength').val('');
            $('#hip').val('');
            $('#shalwar').val('');
            $('#foot').val('');
            $('#shalwarLength').val('');
            $('#otherDetails').val('');
            $('#Description').val('');
            $('#TailorId').val('').trigger('change');
            $('#CutterId').val('').trigger('change');
        },
        RemoveRow: function (that) {
            showAlert('Confirm', 'کیا آپ اس پیمائش کو ہٹانا چاہتے ہیں؟', 'info', true, 'No', 'Yes', function () {
                $(that).parent().parent().remove();
            });
        },
        AddRow: function () {
            let qamar = $('#qamar').val();
            let chest = $('#chest').val();
            let neck = $('#neck').val();
            let arm = $('#arm').val();
            let tera = $('#tera').val();
            let shirtLength = $('#shirtLength').val();
            let hip = $('#hip').val();
            let shalwar = $('#shalwar').val();
            let foot = $('#foot').val();
            let shalwarLength = $('#shalwarLength').val();
            let otherDetails = $('#otherDetails').val();
            let TailorId = $('#TailorId').val();
            let CutterId = $('#CutterId').val();
            let Description = $.trim($('#Description').val());

            if (qamar == '' || qamar == '0') {
                $('#qamar').focus();
                showMessage('Oops!', 'کمر کی پیمائش درج کریں', 'warning');
                return;
            }
            if (chest == '' || chest == '0') {
                $('#chest').focus();
                showMessage('Oops!', 'سینے کی پیمائش درج کریں', 'warning');
                return;
            }
            if (neck == '' || neck == '0') {
                $('#neck').focus();
                showMessage('Oops!', 'گردن کی پیمائش درج کریں', 'warning');
                return;
            }
            if (arm == '' || arm == '0') {
                $('#arm').focus();
                showMessage('Oops!', 'بازو کی پیمائش درج کریں', 'warning');
                return;
            }
            if (tera == '' || tera == '0') {
                $('#tera').focus();
                showMessage('Oops!', 'ٹیرا پیمائش درج کریں', 'warning');
                return;
            }
            if (shirtLength == '' || shirtLength == '0') {
                $('#shirtLength').focus();
                showMessage('Oops!', 'قمیض کی لمبائی کی پیمائش درج کریں', 'warning');
                return;
            }
            if (hip == '' || hip == '0') {
                $('#hip').focus();
                showMessage('Oops!', 'ہپ کی پیمائش درج کریں', 'warning');
                return;
            }
            if (shalwar == '' || shalwar == '0') {
                $('#shalwar').focus();
                showMessage('Oops!', 'شلوار کی پیمائش درج کریں', 'warning');
                return;
            }
            if (foot == '' || foot == '0') {
                $('#foot').focus();
                showMessage('Oops!', 'پنچا پیمائش درج کریں', 'warning');
                return;
            }
            if (shalwarLength == '' || shalwarLength == '0') {
                $('#shalwarLength').focus();
                showMessage('Oops!', 'شلوار کی لمبائی کی پیمائش درج کریں', 'warning');
                return;
            }
            if (TailorId == '') {
                $('#TailorId').focus();
                showMessage('Oops!', 'درزی کو منتخب کریں', 'warning');
                return;
            }
            if (CutterId == '') {
                $('#CutterId').focus();
                showMessage('Oops!', 'کپڑا کاٹنے والا کو منتخب کریں', 'warning');
                return;
            }
            if (Description == '') {
                $('#Description').focus();
                showMessage('Oops!', 'کپڑا کاٹنے والا کو منتخب کریں', 'warning');
                return;
            }

            var item = `<tr>
                <td class="text-right">
                    <button type='button' class='btn btn-sm btn-outline-danger' onclick='OrdersManager.RemoveRow(this)'><i class='fa fa-trash'></i></button>
                </td>
                <td class="text-right">${otherDetails}</td>
                <td class="text-right">${shalwarLength}</td>
                <td class="text-right">${foot}</td>
                <td class="text-right">${chest}</td>
                <td class="text-right">${qamar}</td>
                <td class="text-right">${hip}</td>
                <td class="text-right">${shalwar}</td>
                <td class="text-right">${neck}</td>
                <td class="text-right">${arm}</td>
                <td class="text-right">${tera}</td>
                <td class="text-right">${shirtLength}</td>
                <td class="text-right">${TailorId}</td>
                <td class="text-right">${CutterId}</td>
                <td class="text-right">${Description}</td>
            </tr>`;

            $('#tblOrderDetails tbody').append(item);
            OrdersManager.ClearRow();
        },
        SendSms: function (id) {
            showAlert('Confirmation', 'Are you sure, you want to send sms?', "warning", true, "Cancel", "Save", function () {
                showLoader();
                $.get('/Orders/SendSms', { OrderID: id }, function (data) {
                    hideLoader();
                    if (data.status) {
                        showMessage('Success', data.msg, 'success')
                    } else {
                        showMessage('Oops!', data.msg, 'warning')
                    }
                });
            })

        },
        DownloadInvoice: function (id) {
            if (!id) {
                id = $.trim($('#txtInvNumber').val());
                if (id == '' || id == '0') {
                    $('#txtInvNumber').focus();
                    showMessage('Oops!', 'Enter Order Number', 'warning');
                    return;
                }
            }
            showAlert('Confirmation', 'Are you sure, you want to download invoice?', "warning", true, "Cancel", "<i class='fa fa-download'></i> Download", function () {
                window.location.href = "/Orders/DownloadInvoice?OrderID=" + id;
            })
        },
        GetInvoice: function () {
            var id = $.trim($('#txtInvNumber').val());
            if (id == '' || id == '0') {
                $('#txtInvNumber').focus();
                showMessage('Oops!', 'Enter Order Number', 'warning');
                return;
            }
            showLoader();
            $.get('/Orders/GetInvoice', { ID: id }, function (data) {
                $('#dvInvoice').html(data);
                $('#btnDownloadPrint,#btnDownloadBack').addClass('d-none');
                if ($('#dvInvoice').find('.invoice-5').length > 0) {
                    $('#btnDownload').removeClass('d-none');
                } else {
                    $('#btnDownload').addClass('d-none');
                }
                hideLoader();
            });
        },
        PreviewMeasurements: function (id) {
            showLoader();
            $.get('/Orders/GetMeasurements', { ID: id }, function (data) {
                $('#modal-lg .modal-content').html(data);
                $('#modal-lg').modal({ backdrop: 'static', keyboard: false }, "show");
                hideLoader();
            });
        },
        PrintMeasurements: function (id) {
            window.open('/Orders/PrintMeasurements?ID=' + id, '_blank');
        },
        DownloadBalanceSheet: function () {
            var cid = $('#ddlCustomer').val();
            var oid = $('#txtInvNumber').val();
            var dtFrom = $('#dtFrom').val();
            var dtTo = $('#dtTo').val();
            if (dtTo < dtFrom) {
                showMessage('Oops!', 'From date must be before to date!', 'warning')
                return;
            }
            var url = `/Orders/DownloadBalanceSheet?Cid=${cid}&Oid=${oid}&dtFrom=${dtFrom}&dtTo=${dtTo}`;
            window.open(url, '_blank');
        },
        DownloadBalancePdf: function () {
            var cid = $('#ddlCustomer').val();
            var oid = $('#txtInvNumber').val();
            var dtFrom = $('#dtFrom').val();
            var dtTo = $('#dtTo').val();
            if (dtTo < dtFrom) {
                showMessage('Oops!', 'From date must be before to date!', 'warning')
                return;
            }
            var url = `/Orders/DownloadBalancePdf?Cid=${cid}&Oid=${oid}&dtFrom=${dtFrom}&dtTo=${dtTo}`;
            window.open(url, '_blank');
        },

    }
}();