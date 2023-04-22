$(function () {

    // error message
    function ErrorMessage(error_id, error_message) {
        $(error_id).fadeIn('fast')
            .html("<i class=\"fa fa-warning text-danger\"> </i> <span class=\"text-danger\"> " + error_message + " </span>")
            .delay(9000)
            .fadeOut('fast');
        return;
    }

    // success message
    function SuccessMessage(success_id, success_message) {
        $(success_id).fadeIn('fast')
            .html("<i class=\"fa fa-check-circle text-success\"> </i> <span class=\"text-success\">" + success_message + " </span>")
            .delay(10000)
            .fadeOut('fast');
        return;
    }



    // Create Application type 
    $("#FormCreateAppType").on('submit', function (event) {
        event.preventDefault();

        if ($("#txtAppTypeName").val() === "") {
            ErrorMessage("#CreateAppTypeInfo", "Please enter an App Type");
        }
        else {
            var msg = confirm("Are you sure you want to create this App Type?");

            if (msg === true) {
                $.getJSON("/ApplicationTypes/CreateAppType", { "AppTypeName": $("#txtAppTypeName").val() }, function (response) {
                    if ($.trim(response) === "AppType Created") {
                        SuccessMessage("#CreateAppTypeInfo", "Application type created successfully");
                        $("#FormCreateAppType")[0].reset();
                        AppTypeTables.ajax.reload();
                    }
                    else {
                        ErrorMessage("#CreateAppTypeInfo", response);
                    }
                });
            }
        }
    });


    // geting list of application stage
    var AppTypeTables = $("#TableAppType").DataTable({

       
        ajax: {
            url: "/ApplicationTypes/GetAppType",
            type: "POST",
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]]
        },

        dom: 'Bfrtip',
        buttons: [
            'pageLength',
            'copyHtml5',
            'excelHtml5',
            'csvHtml5',
            'pdfHtml5',
            {
                extend: 'print',
                text: 'Print all',
                exportOptions: {
                    modifier: {
                        selected: null
                    }
                }
            },
            {
                extend: 'colvis',
                collectionLayout: 'fixed two-column'
            }

        ],

        language: {
            buttons: {
                colvis: 'Change columns'
            }
        },

        "processing": true,
        "serverSide": true,
        stateSave: true,

        columns: [
            { data: "appTypeID", "visible": false, "searchable": false },
            { data: "typeName" },
            { data: "createdAt" },
            { data: "updatedAt" },
            {
                "render": function (data, type, row) {
                    return "<button class=\"btn btn-sm btn-primary type" + row["appTypeID"] + "\" id=\"" + row['typeName'] + "\" onclick=\"getAppType(" + row["appTypeID"] + ")\"> <i class=\"fa fa-edit\"> </i> Edit </button> <button class=\"btn btn-sm btn-danger\" target=\"\" onclick=\"DeleteAppType(" + row["appTypeID"] + ")\"> <i class=\"fa fa-trash-o\"> </i> Delete </button>";
                }
            }
        ]

    });


    // Editing app type
    $("#FormEditAppType").on('submit', function (event) {
        event.preventDefault();

        if ($("#txtEditAppTypeID").val() === "") {
            ErrorMessage("#EditAppTypeInfo", "Please select an Application type to edit.");
        }
        else {
            var msg = confirm("Are you sure you want to edit this application type?");

            if (msg === true) {
                $.getJSON("/ApplicationTypes/EditAppType", { "AppTypeName": $("#txtEditAppTypeName").val(), "AppTypeID": $("#txtEditAppTypeID").val() }, function (response) {
                    if ($.trim(response) === "AppType Updated") {
                        SuccessMessage("#EditAppTypeInfo", "Application type updated successfully.");
                        $("#FormEditAppType")[0].reset();
                        AppTypeTables.ajax.reload();
                    }
                    else {
                        ErrorMessage("#EditAppTypeInfo", response);
                    }
                });
            }
        }
    });
});


function getAppType(id) {
    $('html, body').animate({
        scrollTop: $("#FormCreateAppType").offset().top
    }, 1000);
    var app_id = $("#txtEditAppTypeID").val(id);
    var app = $(".type" + id).attr("id");
    $("#txtEditAppTypeName").val(app);
}

/*
 * Removing App type.
 */
function DeleteAppType(id) {
    var msg = confirm("Are you sure you want to remove this Application Type ?");
    if (msg === true) {
        $.getJSON("/ApplicationTypes/DeleteAppType", { "AppTypeID": id }, function (response) {
            if ($.trim(response) === "AppType Deleted") {
                alert("Application Type removed successfully...");
                window.location.reload();
            }
            else {
                alert(response);
            }
        });
    }
}