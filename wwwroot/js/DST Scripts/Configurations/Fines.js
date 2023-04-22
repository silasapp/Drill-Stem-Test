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



    // Create Application fine 
    $("#FormCreateFine").on('submit', function (event) {
        event.preventDefault();

        if ($("#txtFineName").val() === "") {
            ErrorMessage("#CreateFineInfo", "Please enter an App Fine");
        }
        else {
            var msg = confirm("Are you sure you want to create this App fine?");

            if (msg === true) {
                $.getJSON("/Fines/CreateFine",
                    {
                        "FineName": $("#txtFineName").val(),
                        "FineDescription": $("#txtFineDescription").val(),
                        "FineAmount": $("#txtFineAmount").val(),
                        "ServiceCharge": $("#txtServiceCharge").val()
                    },

                    function (response) {
                        if ($.trim(response) === "Fine Created") {
                            SuccessMessage("#CreateFineInfo", "Application fine created successfully");
                            $("#FormCreateFine")[0].reset();
                            FineTables.ajax.reload();
                        }
                        else {
                            ErrorMessage("#CreateFineInfo", response);
                        }
                    });
            }
        }
    });


    // geting list of application stage
    var FineTables = $("#FineTables").DataTable({

        ajax: {
            url: "/Fines/GetFines",
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
            { data: "fineID", "visible": false, "searchable": false },
            { data: "fineName" },
            { data: "fineDescription" },
            { data: "fineAmount" },
            { data: "serviceCharge" },
            { data: "createdAt" },
            { data: "updatedAt" },
            {
                "render": function (data, type, row) {
                    return "<button class=\"btn btn-xs btn-primary app" + row["fineID"] + "\" id=\"" + row['fineName'] + "|" + row["fineAmount"] + "|" + row["fineDescription"] + "|" + row["serviceCharge"] + "\" onclick=\"getAppFine(" + row["fineID"] + ")\"> <i class=\"fa fa-edit\"> </i> Edit </button> <button class=\"btn btn-xs btn-danger\" target=\"\" onclick=\"DeleteAppFine(" + row["fineID"] + ")\"> <i class=\"fa fa-trash\"> </i> Delete </button>";
                }
            }
        ]

    });


    // Editing App Type
    $("#FormEditFine").on('submit', function (event) {
        event.preventDefault();

        if ($("#txtEditFineID").val() === "") {
            ErrorMessage("#EditFineInfo", "Please select an Application fine to edit.");
        }
        else {
            var msg = confirm("Are you sure you want to edit this application fine?");

            if (msg === true) {

                $.getJSON("/Fines/EditFine",
                    {
                        "FineName": $("#txtEditFineName").val(),
                        "FineID": $("#txtEditFineID").val(),
                        "FineDescription": $("#txtEditAppFineDescription").val(),
                        "ServiceCharge": $("#txtEditServiceCharge").val(),
                        "FineAmount": $("#txtEditFineAmount").val()
                    },
                    function (response) {
                        if ($.trim(response) === "Fine Updated") {
                            SuccessMessage("#EditFineInfo", "Application fine updated successfully.");
                            $("#FormEditFine")[0].reset();
                            FineTables.ajax.reload();
                        }
                        else {
                            ErrorMessage("#EditFineInfo", response);
                        }
                    });
            }
        }
    });
});


function getAppFine(id) {
    $('html, body').animate({
        scrollTop: $("#FormCreateFine").offset().top
    }, 1000);
    var app_id = $("#txtEditFineID").val(id);
    var app = $(".app" + id).attr("id").split("|");
    $("#txtEditFineName").val(app[0]);
    $("#txtEditFineAmount").val(app[1]);
    $("#txtEditAppFineDescription").val(app[2]);
    $("#txtEditServiceCharge").val(app[3]);
}

/*
 * Removing App Stage.
 */
function DeleteFine(id) {
    var msg = confirm("Are you sure you want to remove this Application Fine ?");
    if (msg === true) {
        $.getJSON("/Fines/DeleteFine", { "FineID": id }, function (response) {
            if ($.trim(response) === "Fine Deleted") {
                alert("Application fine removed successfully...");
                window.location.reload();
            }
            else {
                alert(response);
            }
        });
    }
}
