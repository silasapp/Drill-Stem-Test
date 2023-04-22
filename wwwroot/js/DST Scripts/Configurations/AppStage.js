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



    // Create Application stage 
    $("#FormCreateAppStage").on('submit', function (event) {
        event.preventDefault();

        if ($("#txtAppStageName").val() === "") {
            ErrorMessage("#CreateAppStageInfo", "Please enter an App Stage");
        }
        else {
            var msg = confirm("Are you sure you want to create this App Stage?");

            if (msg === true) {
                $.getJSON("/ApplicationStages/CreateAppStage",
                    {
                        "AppStageName": $("#txtAppStageName").val(),
                        "AppShortName": $("#txtAppStageShortName").val(),
                        "StageAmount": $("#txtStageAmount").val(),
                        "ServiceCharge": $("#txtServiceCharge").val()
                    },

                    function (response) {
                        if ($.trim(response) === "AppStage Created") {
                            SuccessMessage("#CreateAppStageInfo", "Application stage created successfully");
                            $("#FormCreateAppStage")[0].reset();
                            AppStageTables.ajax.reload();
                        }
                        else {
                            ErrorMessage("#CreateAppStageInfo", response);
                        }
                    });
            }
        }
    });


    // geting list of application stage
    var AppStageTables = $("#TableAppStage").DataTable({

        ajax: {
            url: "/ApplicationStages/GetAppStages",
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
            { data: "appStageID", "visible": false, "searchable": false },
            { data: "stageName" },
            { data: "shortName" },
            { data: "stageAmount" },
            { data: "serviceCharge" },
            { data: "createdAt" },
            { data: "updatedAt" },
            {
                "render": function (data, type, row) {
                    return "<button class=\"btn btn-xs btn-primary app" + row["appStageID"] + "\" id=\"" + row['stageName'] + "|" + row["stageAmount"]+"|"+ row["shortName"] + "|"+ row["serviceCharge"] +"\" onclick=\"getAppStage(" + row["appStageID"] + ")\"> <i class=\"fa fa-edit\"> </i> Edit </button> <button class=\"btn btn-xs btn-danger\" target=\"\" onclick=\"DeleteAppStage(" + row["appStageID"] + ")\"> <i class=\"fa fa-trash\"> </i> Delete </button>";
                }
            }
        ]

    });


    // Editing App Type
    $("#FormEditAppStage").on('submit', function (event) {
        event.preventDefault();

        if ($("#txtEditAppStageID").val() === "") {
            ErrorMessage("#EditAppStageInfo", "Please select an Application stage to edit.");
        }
        else {
            var msg = confirm("Are you sure you want to edit this application stage?");

            if (msg === true) {

                $.getJSON("/ApplicationStages/EditAppStage",
                    {
                        "AppStageName": $("#txtEditAppStageName").val(),
                        "AppStageID": $("#txtEditAppStageID").val(),
                        "AppShortName": $("#txtEditAppStageShortName").val(),
                        "ServiceCharge": $("#txtEditServiceCharge").val(),
                        "StageAmount": $("#txtEditStageAmount").val()
                    },
                    function (response) {
                        if ($.trim(response) === "AppStage Updated") {
                            SuccessMessage("#EditAppStageInfo", "Application stage updated successfully.");
                            $("#FormEditAppStage")[0].reset();
                            AppStageTables.ajax.reload();
                        }
                        else {
                            ErrorMessage("#EditAppStageInfo", response);
                        }
                    });
            }
        }
    });
});


function getAppStage(id) {
    $('html, body').animate({
        scrollTop: $("#FormCreateAppStage").offset().top
    }, 1000);
    var app_id = $("#txtEditAppStageID").val(id);
    var app = $(".app" + id).attr("id").split("|");
    $("#txtEditAppStageName").val(app[0]);
    $("#txtEditStageAmount").val(app[1]);
    $("#txtEditAppStageShortName").val(app[2]);
    $("#txtEditServiceCharge").val(app[3]);
}

/*
 * Removing App Stage.
 */
function DeleteAppStage(id) {
    var msg = confirm("Are you sure you want to remove this Application Stage ?");
    if (msg === true) {
        $.getJSON("/ApplicationStages/DeleteAppStage", { "AppStageID": id }, function (response) {
            if ($.trim(response) === "AppStage Deleted") {
                alert("Application Stage removed successfully...");
                window.location.reload();
            }
            else {
                alert(response);
            }
        });
    }
}