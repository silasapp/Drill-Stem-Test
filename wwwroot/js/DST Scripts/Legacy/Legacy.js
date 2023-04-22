$(document).ready(function () {

    // error message
    function ErrorMessage(error_id, error_message) {
        $(error_id).fadeIn('fast')
            .html("<div class=\"alert alert-danger\"><i class=\"fa fa-warning\"> </i> <span class=\"\"> " + error_message + " </span> </div>")
            .delay(9000)
            .fadeOut('fast');
        return;
    }

    // success message
    function SuccessMessage(success_id, success_message) {
        $(success_id).fadeIn('fast')
            .html("<div class=\"alert alert-info\"> <i class=\"fa fa-check-circle\"> </i> <span class=\"\">" + success_message + " </span> </div>")
            .delay(10000)
            .fadeOut('fast');
        return;
    }


    $("#CreateLegacyApplication").on('submit', function (e) {
        e.preventDefault();

        if ($("#txtCompanyID").val() === "") {
            ErrorMessage("#CreateLegacyApplicationInfo", "Company not found.");
        }
        else {

            var LegacyDocuments = [];

            var LocalID = document.getElementsByName('txtLocalDocID[]');
            var DocSource = document.getElementsByName('txtDocSource[]');
            var CompDocElpsID = document.getElementsByName('txtCompDocElpsID[]');
            var missingCompDocElpsID = document.getElementsByName('missingCompElpsDocID[]');

            if (missingCompDocElpsID.length > 0) {

                // validating missing documents
                for (var i = 0; i < missingCompDocElpsID.length; i++) {
                    if (missingCompDocElpsID[i].value === "0" || missingCompDocElpsID[i].value === "") {
                        ErrorMessage("#divUploadDocInfo", "Please Upload all required documents");
                        break;
                    }
                    else {
                        alert("Inputs not found...");
                    }
                }
            }
            else {

                for (var j = 0; j < LocalID.length; j++) {

                    LegacyDocuments.push({
                        "LocalDocID": LocalID[j].value.trim(),
                        "CompElpsDocID": CompDocElpsID[j].value.trim(),
                        "DocSource": DocSource[j].value.trim()
                    });
                }

                var msg = confirm("Are you sure you want to create this legacy application?");

                if (msg === true) {

                    var LegacyApplication = {
                        SelfFacility: $("#txtOriginName").val().toUpperCase(),
                        SelfFacilityAddress: $("#txtOriginAddress").val(),
                        DestinationCompany: $("#txtDestinationCompanyName").val().toUpperCase(),
                        DestinationCompanyAddress: $("#txtDestinationCompanyAddress").val(),
                        DestinationFacility: $("#txtDestinationFieldName").val().toUpperCase(),
                        DestinationFacilityAddress: $("#txtDestinationFieldAddress").val(),
                        TransportationMode: $("#txtTransportMode").val(),
                        Qty: $("#txtQuantity").val(),
                        SelfFacilityState: $("#txtOriginState").val(),
                        DestinationFacilityState: $("#txtDestinationFieldState").val(),
                        OriginMesurementInstrument: $("#txtOriginMesurmentInstrument").val(),
                        DestinationMesurementInstrument: $("#txtDestinationMesurmentInstrument").val(),
                        IssuedDate: $("#txtIssuedDate").val(),
                        ExpiryDate: $("#txtExpiryDate").val(),
                        PermitNo: $("#txtLicenceNumber").val().toUpperCase(),
                        AppStageId: $("#txtLStages").val(),
                        IsSubmitted: true,
                        DeletedStatus: false
                    };

                    $("#CreateLegacyLoader").addClass("Submitloader");

                    $.post("/Legacy/CreateLegacyApplication",
                        {
                            "CompanyId": $("#txtCompanyID").val(),
                            "LegacyID": $("#txtLegacyID").val(),
                            "legacy": LegacyApplication,
                            "legacyDocuments": LegacyDocuments
                        }, function (response) {
                            if (response === "Created" || response === "Resubmitted") {
                                SuccessMessage("#CreateLegacyApplicationInfo", "Legacy application " + response + " successfully.");
                                $("#CreateLegacyLoader").removeClass("Submitloader");
                                window.location.href = window.location.origin + "/Legacy/MyLegacy";
                            }
                            else {
                                ErrorMessage("#CreateLegacyApplicationInfo", response);
                                $("#CreateLegacyLoader").removeClass("Submitloader");
                            }
                        });
                }
            }
        }
    });




    $("#btnRejectLegacys").on('click', function (e) {

        if ($("#txtLegacyID").val() === "" ) {
            ErrorMessage("#RejectLegacyModalInfo", "Legacy reference is empty.");
        }
        else if ($("#txtLegacyRejectionComment").val() === "") {
            ErrorMessage("#RejectLegacyModalInfo", "Please enter rejection comment for this application");
        }
        else {

            var msg = confirm("Are you sure you want to reject this legacy application?");

            if (msg === true) {

                $("#RejectLegacyLoader").addClass("Submitloader");

                $.post("/Legacy/RejectLegacy",
                    {
                        "LegacyID": $("#txtLegacyID").val(),
                        "txtComment": $("#txtLegacyRejectionComment").val()
                    }, function (response) {
                        if (response === "Rejected") {
                            SuccessMessage("#RejectLegacyModalInfo", "Legacy application " + response + " successfully.");
                            $("#RejectLegacyLoader").removeClass("Submitloader");
                            window.location.href = window.location.origin + "/Legacy/LegacyDesk";
                        }
                        else {
                            ErrorMessage("#RejectLegacyModalInfo", response);
                            $("#RejectLegacyLoader").removeClass("Submitloader");
                        }
                    });
            }

        }

    });





    $("#btnApproveLegacy").on('click', function (e) {

        if ($("#txtLegacyID").val() === "") {
            ErrorMessage("#ApproveLegacyModalInfo", "Legacy reference is empty.");
        }
        else if ($("#txtApproveLegacyComment").val() === "") {
            ErrorMessage("#ApproveLegacyModalInfo", "Please enter approval comment for this application");
        }
        else {

            var msg = confirm("Are you sure you want to approve this legacy application?");

            if (msg === true) {

                $("#AppApproveLegacyLoader").addClass("Submitloader");

                $.post("/Legacy/AcceptLegacy",
                    {
                        "LegacyID": $("#txtLegacyID").val(),
                        "txtComment": $("#txtApproveLegacyComment").val()
                    }, function (response) {
                        if (response === "Approved") {
                            SuccessMessage("#ApproveLegacyModalInfo", "Legacy application " + response + " successfully.");
                            $("#AppApproveLegacyLoader").removeClass("Submitloader");
                            window.location.href = window.location.origin + "/Legacy/LegacyDesk";
                        }
                        else {
                            ErrorMessage("#ApproveLegacyModalInfo", response);
                            $("#AppApproveLegacyLoader").removeClass("Submitloader");
                        }
                    });
            }

        }

    });

});