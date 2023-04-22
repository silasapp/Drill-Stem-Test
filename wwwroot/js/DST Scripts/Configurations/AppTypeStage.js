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


    // selecting all App Types 
    $("#txtTypes").ready(function () {
        var html = "";

        $("#txtTypes").html("");

        $.getJSON("/Helpers/GetAppTypes",
            { "deletedStatus": false },
            function (datas) {

                $("#txtTypes").append("<option disabled selected>--Select App Type--</option>");
                $("#txtEditTypes").append("<option disabled selected>--Select App Type--</option>");

                $.each(datas,
                    function (key, val) {
                        html += "<option value=" + val.appTypeId + ">" + val.typeName + "</option>";
                    });
                $("#txtTypes").append(html);
                $("#txtEditTypes").append(html);
            });
    });


    // selecting all Stages 
    $("#txtStages").ready(function () {
        var html = "";

        $("#txtStages").html("");

        $.getJSON("/Helpers/GetAppStages",
            { "deletedStatus": false },
            function (datas) {

                $("#txtStages").append("<option disabled selected>--Select Stages--</option>");
                $("#txtEditStages").append("<option disabled selected>--Select Stages--</option>");
               
                $.each(datas,
                    function (key, val) {
                        html += "<option value=" + val.appStageId + ">" + val.stageName + "</option>";
                    });
                $("#txtStages").append(html);
                $("#txtEditStages").append(html);
               
            });
    });


    // Create Type => Stage relationship

    $("#FormCreateTypeStage").on('submit', function (event) {

        event.preventDefault();

        var msg = confirm("Are you sure you want to create this Type => Stage relationship?");

        if (msg === true) {
            $.getJSON("/AppTypeWithStage/CreateTypeStage",
                {
                    "TypeID": $("#txtTypes").val(), "StageID": $("#txtStages").val(), "Counter": $("#txtStepCounter").val()
                }, function (response) {
                if ($.trim(response) === "TypeState Created") {
                    SuccessMessage("#TypeStageInfo", "Type and Stage relationship created successfully");
                    $("#FormCreateTypeStage")[0].reset();
                    TypeStageTable.ajax.reload();
                }
                else {
                    ErrorMessage("#TypeStageInfo", response);
                }
            });
        }
    });


    // geting list of Zone => State relationship

    var TypeStageTable = $("#TableTypeStage").DataTable({

        ajax: {
            url: "/AppTypeWithStage/GetTypeStage",
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
            { data: "typeStageID", "visible": false, "searchable": false },
            { data: "typeID", "visible": false, "searchable": false },
            { data: "stageID", "visible": false, "searchable": false },
            { data: "typeName" },
            { data: "stageName" },
            { data: "shortName" },
            { data: "counter" },
            { data: "stageAmount" },
            {data : "serviceCharge"},
            { data: "createdAt" },
            { data: "updatedAt" },
            {
                "render": function (data, type, row) {
                    return "<button class=\"btn btn-xs btn-primary typestage" + row["typeStageID"] + "\" id=\"" + row['typeName'] + "|" + row["stageName"] + "|" + row["counter"] + "|" + row["typeID"] + "|" + row["stageID"] + "\" onclick=\"getTypeStage(" + row["typeStageID"] + ")\"> <i class=\"fa fa-edit\"> </i> Edit </button> <button class=\"btn btn-xs btn-danger\" target=\"\" onclick=\"DeleteTypeStage(" + row["typeStageID"] + ")\"> <i class=\"fa fa-trash-o\"> </i> Delete </button>";
                }
            }
        ]

    });

       
    $("#FormEditTypeStageReset").on('click', function (event) {
        event.preventDefault();
        $("#relationshipStatus").html("");
        $("#FormCreateTypeStage")[0].reset();
    });


    // Editing Zone => State relationship

    $("#FormEditTypeStage").on('submit', function (event) {
        event.preventDefault();

        if ($("#txtEditTypeStageID").val() === "") {
            ErrorMessage("#EditTypeStageInfo", "Please select a relationship to edit.");
        }
        else {
            var msg = confirm("Are you sure you want to edit this relationship?");

            if (msg === true) {
                $.getJSON("/AppTypeWithStage/EditTypeStage",
                    {
                        "Counter": $("#txtEditStepCounter").val(),
                        "TypeID": $("#txtEditTypes").val(),
                        "StageID": $("#txtEditStages").val(),
                        "TypeStageID": $("#txtEditTypeStageID").val()
                    },
                    function (response) {

                    if ($.trim(response) === "TypeStage Updated") {
                        SuccessMessage("#EditTypeStageInfo", "Updated successfully.");
                        $("#FormEditTypeStage")[0].reset();
                        $("#relationshipStatus").html("");
                        TypeStageTable.ajax.reload();
                    }
                    else {
                        ErrorMessage("#EditTypeStageInfo", response);
                    }
                });
            }
        }
    });
});

/*
 * Getting Zone => state info for relationship editing
 */
function getTypeStage(id) {
    $('html, body').animate({
        scrollTop: $("#FormCreateTypeStage").offset().top
    }, 1000);
    var zoneStateId = $("#txtEditTypeStageID").val(id);
    var typeStage = $(".typestage" + id).attr("id").split('|');

    var type = typeStage[0];
    var stage = typeStage[1];
    var counter = typeStage[2];
    var type_id = typeStage[3];
    var stage_id = typeStage[4];

    var html = "";

    $("#relationshipStatus").html("");
    $("#txtEditStepCounter").val("");
    $("#txtEditTypes").change();
    $("#txtEditStages").change();

    html = "<span class=\"ui tiny orange label\" >" + type + "</span> => <span class=\"ui tiny blue label\">" + stage + "</span>";

    $("#relationshipStatus").append(html);
    $("#txtEditStepCounter").val(counter);
    $("#txtEditTypes").val(type_id).change();
    $("#txtEditStages").val(stage_id).change();

}


/*
 * Removing a Type => Stage.
 */
function DeleteTypeStage(id) {

    var msg = confirm("Are you sure you want to remove this Type => Stage?");

    if (msg === true) {
        $.getJSON("/AppTypeWithStage/DeleteTypeStage", { "TypeStageID": id }, function (response) {
            if ($.trim(response) === "TypeStage Deleted") {
                alert("Type => Stage removed successfully...");
                window.location.reload();
            }
            else {
                alert(response);
            }
        });
    }
}