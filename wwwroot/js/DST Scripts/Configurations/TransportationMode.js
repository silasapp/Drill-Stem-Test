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



    // Create Mode
    $("#FormCreateMode").on('submit', function (event) {
        event.preventDefault();

        if ($("#txtTransportMode").val() === "" || $("#txtTransportMode").val() === "") {
            ErrorMessage("#CreateModeInfo", "Please enter a mode");
        }
        else {
            var msg = confirm("Are you sure you want to create this mode?");

            if (msg === true) {
                $.getJSON("/TransportationModes/CreateMode", { "ModeName": $("#txtTransportMode").val(), "ModeAmount": $("#txtModeAmount").val() }, function (response) {
                    if ($.trim(response) === "Mode Created") {
                        SuccessMessage("#CreateModeInfo", "Country created successfully");
                        $("#FormCreateMode")[0].reset();
                        ModeTables.ajax.reload();
                    }
                    else {
                        ErrorMessage("#CreateModeInfo", response);
                    }
                });
            }
        }
    });


    // geting list of countries
    var ModeTables = $("#TableMode").DataTable({

        ajax: {
            url: "/TransportationModes/GetTransportationMode",
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
            { data: "modeID" },
            { data: "modeName" },
            { data: "modeAmount" },
            { data: "createdAt" },
            { data: "updatedAt" },

            {
                "render": function (data, type, row) {
                    return "<button class=\"btn btn-sm btn-primary mode" + row["modeID"] + "\" id=\"" + row['modeName'] + "|" + row['modeAmount'] + "\" onclick=\"getMode(" + row["modeID"] + ")\"> <i class=\"fa fa-edit\"> </i> Edit </button> <button class=\"btn btn-sm btn-danger\" onclick=\"DeleteMode(" + row["modeID"] + ")\"> <i class=\"fa fa-trash\"> </i> Delete </button>";
                }
            }
        ],

        "columnDefs": [
            {
                "targets": [0],
                "visible": false,
                "searchable": false
            },
            {
                "targets": [1],
                "searchable": true,
                "orderable": true
            },
            {
                "targets": [2],
                "orderable": true
            },
            {
                "targets": [3],
                "orderable": true
            }

        ]
    });


    // Editing Countries
    $("#FormEditMode").on('submit', function (event) {
        event.preventDefault();

        if ($("#txtEditTransportMode").val() === "" || $("#txtEditModeAmount").val() === "") {
            ErrorMessage("#EditModeInfo", "Please select a transportation mode to edit.");
        }
        else {
            var msg = confirm("Are you sure you want to edit this transportation mode ?");

            if (msg === true) {
                $.getJSON("/TransportationModes/EditMode", { "ModeName": $("#txtEditTransportMode").val(), "ModeID": $("#txtModeID").val(), "ModeAmount": $("#txtEditModeAmount").val() }, function (response) {
                    if ($.trim(response) === "Mode Updated") {
                        SuccessMessage("#EditModeInfo", "Transportation mode updated successfully.");
                        $("#FormEditMode")[0].reset();
                        
                        ModeTables.ajax.reload();
                    }
                    else {
                        ErrorMessage("#EditModeInfo", response);
                    }
                });
            }
        }
    });
});


function getMode(id) {
    $('html, body').animate({
        scrollTop: $("#FormCreateMode").offset().top
    }, 1000);
    var country_id = $("#txtModeID").val(id);
    var mode = $(".mode" + id).attr("id").split("|");
    $("#txtEditTransportMode").val(mode[0]);
    $("#txtEditModeAmount").val(mode[1]);
}


function DeleteMode(id) {

    var msg = confirm("Are you sure you want to delete this transportation mode?");

    if (msg === true) {
        $.getJSON("/TransportationModes/DeleteMode", { "ModeID": id }, function (response) {
            if ($.trim(response) === "Mode Deleted") {
                alert("Transportation mode removed successfully.");
                location.reload(true);
            }
            else {
                alert(response);
            }
        });
    }
}