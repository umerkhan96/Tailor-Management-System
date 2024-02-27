let CustomerManager = function () {
    return {
        Datatable: null,
        LoadDataTable: function () {
            var table = $('#dtCustomer').DataTable({
                "processing": true,
                "serverSide": true,
                "ajax": {
                    "url": "/Customer/Paginate",
                    "type": "POST",
                    "data": function (d) {
                        d.status = $('#statusFilter').val()
                    }
                },
                "columns": [
                    { "data": "index", "name": "index", "autoWidth": true },
                    { "data": "id", "name": "id", "autoWidth": true },
                    { "data": "firstName", "name": "firstName", "autoWidth": true },
                    { "data": "lastName", "name": "lastName", "autoWidth": true },
                    { "data": "mobileNumber", "name": "mobileNumber", "autoWidth": true },
                    {
                        "data": null,
                        "render": function (data, type, row) {
                            var res = '';
                            if (row.isDelete) {
                                res += '<a class="btn btn-outline-danger btn-sm" onclick="CustomerManager.RestoreCustomer(' + row.id + ')"><i class="fa fa-recycle text-sm"></i></a>';
                            } else {
                                res += '<a class="btn btn-outline-info btn-sm" onclick="CustomerManager.LoadSaveForm(' + row.id + ')"><i class="fa fa-edit text-sm"></i></a>' +
                                    '&nbsp;';
                                res += '<a class="btn btn-outline-danger btn-sm" onclick="CustomerManager.DeleteCustomer(' + row.id + ')"><i class="fa fa-trash text-sm"></i></a>';
                            }
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
                    ageFilterDropdown = '<label class=" ml-2"><select id="statusFilter" class="form-control form-control-sm">';
                    ageFilterDropdown += '<option value="false">Active</option>';
                    ageFilterDropdown += '<option value="true">In Active</option>';
                    ageFilterDropdown += '</select></label>';
                    $('#dtCustomer_filter').append(ageFilterDropdown);

                    $('#statusFilter').on('change', function () {
                        table.draw();
                    });

                },
                "columnDefs": [
                    { "orderable": false, "targets": [0, -1], "width": "80px" }

                ]
            });
            CustomerManager.Datatable = table;
        },
        LoadSaveForm: function (id) {
            showLoader();
            $.get('/Customer/GetSaveForm', { ID: id }, function (data) {
                $('#modal-df .modal-content').html(data);
                $('#modal-df').modal({ backdrop: 'static', keyboard: false }, "show");
                hideLoader();
            });
        },
        DeleteCustomer: function (id) {
            showAlert('De-Activation', 'Are you sure, you want to deactivate customer?', "warning", true, "Cancel", "Deactivate", function () {
                showLoader();
                $.ajax({
                    url: '/Customer/DeleteCustomer',
                    type: 'post',
                    data: { ID: id },
                    success: function (data) {
                        hideLoader();
                        if (data.status) {
                            showAlert('Success', data.msg, "success", false, "Cancel", "OK", function () {
                                CustomerManager.Datatable.draw();
                                $('#modal-lg').modal("hide");
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
        RestoreCustomer: function (id) {
            showAlert('Re-Activation', 'Are you sure, you want to re-activate customer?', "warning", true, "Cancel", "Reactivate", function () {
                showLoader();
                $.ajax({
                    url: '/Customer/ActivateCustomer',
                    type: 'post',
                    data: { ID: id },
                    success: function (data) {
                        hideLoader();
                        if (data.status) {
                            showAlert('Success', data.msg, "success", false, "Cancel", "OK", function () {
                                CustomerManager.Datatable.draw();
                                $('#modal-lg').modal("hide");
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
        SaveCustomer: function () {
            if ($('#form-save-customer').valid()) {
                showAlert('Confirmation', 'Are you sure, you want to save customer?', "warning", true, "Cancel", "Save", function () {
                    showLoader();
                    $.ajax({
                        url: '/Customer/SaveCustomer',
                        type: 'post',
                        data: $('#form-save-customer').serialize(),
                        success: function (resp) {
                            if (resp.status) {
                                showAlert('Success', resp.msg, "success", false, "Cancel", "OK", function () {
                                    if ($('#optFromCustomer') && $('#optFromCustomer').val() == 'true') {
                                        CustomerManager.LoadCustomers(function (data) {
                                            if (data && data.length > 0) {
                                                $('#CustomerId').empty();
                                                $('#CustomerId').append('<option value="">Select Customer</option>');
                                                data.forEach(function (e, i) {
                                                    var sel = "";
                                                    if (resp.id == e.id) {
                                                        sel = "selected";
                                                    }
                                                    $('#CustomerId').append('<option value="' + e.id + '" ' + sel + '>' + e.firstName + ' ' + e.lastName + '(' + e.id + ')' + '</option>');
                                                })
                                            }
                                        });
                                    } else {
                                        CustomerManager.Datatable.draw();
                                    }
                                    $('#modal-df').modal("hide");
                                    hideLoader();
                                });
                            } else {
                                showAlert('Oops', resp.msg, "warning", false, "Cancel", "OK");
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
        LoadCustomers: function (callback) {
            $.get('/Customer/GetCustomers', function (data) {
                if (callback) {
                    callback(data);
                }
            });
        }
    }
}();