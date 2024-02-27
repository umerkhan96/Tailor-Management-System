﻿let StaffManager = function () {
    return {
        Datatable: null,
        LoadDataTable: function () {
            var table = $('#dtStaff').DataTable({
                "processing": true,
                "serverSide": true,
                "ajax": {
                    "url": "/Staff/Paginate",
                    "type": "POST",
                    "data": function (d) {
                        d.role = $('#roleFilter').val();
                        d.status = $('#statusFilter').val();
                    }
                },
                "columns": [
                    { "data": "index", "name": "index", "autoWidth": true },
                    { "data": "id", "name": "id", "autoWidth": true },
                    { "data": "firstName", "name": "firstName", "autoWidth": true },
                    { "data": "lastName", "name": "lastName", "autoWidth": true },
                    { "data": "email", "name": "email", "autoWidth": true },
                    { "data": "role", "name": "role", "autoWidth": true },
                    {
                        "data": null,
                        "render": function (data, type, row) {
                            var res = '';
                            if (row.isDeleted) {
                                res += '<a class="btn btn-outline-danger btn-sm" onclick="StaffManager.RestoreStaff(' + row.id + ')"><i class="fa fa-recycle text-sm"></i></a>';
                            } else {
                                res += '<a class="btn btn-outline-info btn-sm" onclick="StaffManager.LoadSaveForm(' + row.id + ')"><i class="fa fa-edit text-sm"></i></a>' +
                                    '&nbsp;';
                                res += '<a class="btn btn-outline-danger btn-sm" onclick="StaffManager.DeleteStaff(' + row.id + ')"><i class="fa fa-trash text-sm"></i></a>';
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
                    var ageFilterDropdown = '<label class=" ml-2"><select id="roleFilter" class="form-control form-control-sm"><option value="">All Roles</option>';
                    ageFilterDropdown += '<option value="Cutter">Cutter</option>';
                    ageFilterDropdown += '<option value="Tailor">Tailor</option>';
                    ageFilterDropdown += '<option value="Other">Other</option>';
                    ageFilterDropdown += '</select></label>';
                    $('#dtStaff_filter').append(ageFilterDropdown);

                    ageFilterDropdown = '<label class=" ml-2"><select id="statusFilter" class="form-control form-control-sm"><option value="">All Status</option>';
                    ageFilterDropdown += '<option value="false">Active</option>';
                    ageFilterDropdown += '<option value="true">In Active</option>';
                    ageFilterDropdown += '</select></label>';
                    $('#dtStaff_filter').append(ageFilterDropdown);

                    // Add event listener to the dropdown for filtering
                    $('#roleFilter, #statusFilter').on('change', function () {
                        table.draw();
                    });
                },
                "columnDefs": [
                    { "orderable": false, "targets": [0, -1], "width": "80px" }

                ]
            });
            StaffManager.Datatable = table;
        },
        LoadSaveForm: function (id) {
            showLoader();
            $.get('/Staff/GetSaveForm', { ID: id }, function (data) {
                $('#modal-lg .modal-content').html(data);
                $('#modal-lg').modal({ backdrop: 'static', keyboard: false }, "show");
                hideLoader();
            });
        },
        DeleteStaff: function (id) {
            showAlert('De-Activation', 'Are you sure, you want to deactivate staff?', "warning", true, "Cancel", "Deactivate", function () {
                showLoader();
                $.ajax({
                    url: '/Staff/DeleteStaff',
                    type: 'post',
                    data: { ID: id },
                    success: function (data) {
                        hideLoader();
                        if (data.status) {
                            showAlert('Success', data.msg, "success", false, "Cancel", "OK", function () {
                                StaffManager.Datatable.draw();
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
        RestoreStaff: function (id) {
            showAlert('Re-Activation', 'Are you sure, you want to re-activate staff?', "warning", true, "Cancel", "Reactivate", function () {
                showLoader();
                $.ajax({
                    url: '/Staff/ActivateStaff',
                    type: 'post',
                    data: { ID: id },
                    success: function (data) {
                        hideLoader();
                        if (data.status) {
                            showAlert('Success', data.msg, "success", false, "Cancel", "OK", function () {
                                StaffManager.Datatable.draw();
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
        SaveStaff: function () {
            if ($('#form-save-user').valid()) {
                showAlert('Confirmation', 'Are you sure, you want to save staff?', "warning", true, "Cancel", "Save", function () {
                    showLoader();
                    $.ajax({
                        url: '/Staff/SaveStaff',
                        type: 'post',
                        data: $('#form-save-user').serialize(),
                        success: function (data) {
                            hideLoader();
                            if (data.status) {
                                showAlert('Success', data.msg, "success", false, "Cancel", "OK", function () {
                                    StaffManager.Datatable.draw();
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
            }
        },
        ViewPassword: function (that) {
            if ($(that).find('i').hasClass('fa-eye')) {
                $(that).parent().prev().attr('type', 'text')
                $(that).find('i').removeClass('fa-eye');
                $(that).find('i').addClass('fa-eye-slash');
            } else {
                $(that).parent().prev().attr('type', 'password')
                $(that).find('i').removeClass('fa-eye-slash');
                $(that).find('i').addClass('fa-eye');
            }
        }
    }
}();