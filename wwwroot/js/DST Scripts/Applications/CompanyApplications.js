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

    var notifyCount = 0;

    // application on staff desk count
    $.post("/Staffs/MyDeskCount", function (response) {
        $("#MyDeskCount").text(response);
        $("#staffDeskCount").text(response);
    });


    // Get nomination request sent to a staff
    $.post("/NominationRequest/GetNominationRequest", function (response) {
        $("#MyNominationRequestCount").text(response);
    });

    // nomination staff desk count
    $.post("/Staffs/MyNominationCount", function (response) {
        $("#MyNominationCount").text(response);
    });




    // schedules count for staff
    $.post("/Staffs/MySchduleCount", function (response) {
        $("#StaffScheduleCount").text(response);
        $("#StaffScheduleCount2").text(response);
    });

    // legacy application count
    $.post("/Staffs/StaffLegacyAppsCount", function (response) {
        $("#StaffLegacyDeskCount").text(response);
    });

    // legacy application count
    $.post("/Companies/CompanyLegacyAppsCount", function (response) {
        $("#CompanyLegacyCount").text(response);
        $("#CompanyLegacyCount2").text(response);
    });


    $.post("/OutOfOffice/CountRelieveStaff", {}, function (response) {
        $("#OutOfOfficeCout1").text(response);
        $("#OutOfOfficeCout2").text(response);
    });

   


    //Out of office start
    $.post("/OutOfOffice/TriggerOutOfOfficeStart", {}, function (response) {
        console.log(response);
    });


    //Out of office start
    $.post("/OutOfOffice/TriggerOutOfOfficeEnd", {}, function (response) {
        console.log(response);
    });


    // schedule for company
    $.post("/Companies/GetScheduleCount", function (response) {
        notifyCount += response;
        $("#NotifyCount").text(notifyCount);
        $("#ScheduleCount").text(response);
    });

    


    setInterval(function () {

        notifyCount = 0;
        // schedule for company
        $.post("/Companies/GetScheduleCount", function (response) {
            notifyCount += response;
            $("#ScheduleCount").text(response);
        });

       
        // application on staff desk count
        $.post("/Staffs/MyDeskCount", function (response) {
            $("#MyDeskCount").text(response);
            $("#staffDeskCount").text(response);
        });


        // nomination staff desk count
        $.post("/Staffs/MyNominationCount", function (response) {
            $("#MyNominationCount").text(response);
        });
       

        // schedules count for staff
        $.post("/Staffs/MySchduleCount", function (response) {
            $("#StaffScheduleCount").text(response);
            $("#StaffScheduleCount2").text(response);
        });

        // legacy application count
        $.post("/Staffs/StaffLegacyAppsCount", function (response) {
            $("#StaffLegacyDeskCount").text(response);
        });

        // legacy application count
        $.post("/Legacies/LegacyAppsCount", function (response) {
            $("#MyLegacyCount").text(response);
            $("#MyLegacyCount2").text(response);
        });


        $.post("/OutOfOffice/CountRelieveStaff", {}, function (response) {
            $("#OutOfOfficeCout1").text(response);
            $("#OutOfOfficeCout2").text(response);
        });


        //Out of office start
        $.post("/OutOfOffice/TriggerOutOfOfficeStart", {}, function (response) {
            console.log(response);
        });


        //Out of office start
        $.post("/OutOfOffice/TriggerOutOfOfficeEnd", {}, function (response) {
            console.log(response);
        });


        // legacy application count
        $.post("/Companies/CompanyLegacyAppsCount", function (response) {
            $("#CompanyLegacyCount").text(response);
            $("#CompanyLegacyCount2").text(response);
        });


        // Get nomination request sent to a staff
        $.post("/NominationRequest/GetNominationRequest", function (response) {
            $("#MyNominationRequestCount").text(response);
        });

       // $("#NotifyCount").text(notifyCount);

        $.post("/Session/CheckSession", function (response, e, xhr) {

            if ($.trim(response) === "true") {
                var location = window.location.origin + "/Account/ExpiredSession";
                window.location.href = location;
            }

        });

        
    },1000 * 30); // 30 sec




    //Proposed start date for com
    $("#txtStartDate").datetimepicker({
        minDate: new Date().setDate(new Date().getDate() + 1), //- this is tomorrow;  use 0 for toady
        defaultDate: new Date().setDate(new Date().getDate() + 1),
        //onGenerate: function (ct) {
        //    $(this).find('.xdsoft_date.xdsoft_weekend')
        //        .addClass('xdsoft_disabled');
        //}
    });


    //Proposed start date for com
    $("#txtEndDate").datetimepicker({
        minDate: new Date().setDate(new Date().getDate() + 1), //- this is tomorrow;  use 0 for toady
        defaultDate: new Date().setDate(new Date().getDate() + 1),
        //onGenerate: function (ct) {
        //    $(this).find('.xdsoft_date.xdsoft_weekend')
        //        .addClass('xdsoft_disabled');
        //}
    });



    /*
    * Determining what kind of application and show appropraite forms. 
    */
    $("#seltxtAppStage").on('change', function (event) {

        event.preventDefault();

        $("#PermitInfo").html("");
        $("#btnSelectNPMD").val("");
        $("#txtRefNumber").val("");

        if ($("#seltxtAppStage").val() === "") {
            $("#FormSaveDST").slideUp();
            $("#FormCreateNewOtherApplication").slideUp();
        }
        else {

            if ($("#seltxtAppStage option:selected").text().trim() === "TECHNICAL ALLOWABLE RATE") {
                $("#FormSaveDST").slideUp();
                $("#FormCreateNewOtherApplication").slideDown();
            }
            else if ($("#seltxtAppStage option:selected").text().trim() === "OFF CYCLE TECHNICAL ALLOWABLE RATE") {
                $("#FormSaveDST").slideUp();
                $("#FormCreateNewOtherApplication").slideDown();
            }
            else if ($("#seltxtAppStage option:selected").text().trim() === "EXTENSION FOR EWT") {
                $("#FormSaveDST").slideUp();
                $("#FormCreateNewOtherApplication").slideDown();
            }
            else if ($("#seltxtAppStage option:selected").text().trim() === "TECHNICAL ALLOWABLE RATE REVIEW") {
                $("#FormSaveDST").slideUp();
                $("#FormCreateNewOtherApplication").slideDown();
            }
            else {
                $("#FormSaveDST").slideDown();
                $("#FormCreateNewOtherApplication").slideUp();
            }
        }
    });



    $("#UploadedFile").change(function () {
        var name = $('input[type=file]').val().replace(/C:\\fakepath\\/i, '');
        $("#filename").text(name);
    });




    $("#FormSaveDST").on('submit', function (event) {
        event.preventDefault();


        if ($("#seltxtAppStage").val() === "") {

            Notify.alert({
                title: 'Failure',
                text: "Application Stage reference not in correct format."
            });
        }
        else {

            var msg = confirm("Are you sure you want to create this application?");

            var fileUpload = $("#UploadedFile").get(0);
            var files = fileUpload.files;
            var data = new FormData();

            data.append("Files", files[0]);
            data.append("Contractor", $("#txtContractorName").val());
            data.append("RigName", $("#txtRigName").val());
            data.append("RigType", $("#txtRigType").val());
            data.append("TypeStageId", $("#seltxtAppStage").val());

            if (msg === true) {

                $("#AppDiv").addClass("Submitloader");

                $.ajax({
                    url: '/CompanyApplication/CreateApplication',
                    data: data,
                    cache: false,
                    contentType: false,
                    processData: false,
                    type: 'POST',
                    async: false,
                    success: function (result) {
                        var message = result.split("|");
                        if ($.trim(message[0]) === "1") {
                            SuccessMessage("#MessageInfo", "Application created successfully.");
                            window.location.href = "/CompanyApplication/ViewSubmission?id=" + message[1];
                            $("#AppDiv").removeClass("Submitloader");
                        }
                        else{
                            ErrorMessage("#MessageInfo", message[1]);
                            $("#AppDiv").removeClass("Submitloader");
                        }
                    },
                    error: function (result) {
                        var message = result.split("|");
                        if ($.trim(message[0]) === "0") {
                            ErrorMessage("#MessageInfo", message[1]);
                            $("#AppDiv").removeClass("Submitloader");
                        }
                    }
                });
            }
        }
    });





    /*
     * Creating other applicatioins apart for Suitability
     */
    $("#FormCreateNewOtherApplication").on('submit', function (event) {

        event.preventDefault();

        if ($("#txtRefNumber").val() === "") {
            ErrorMessage("#MessageInfo", "Facility not found.");
        }
        else {

            var msg = confirm("Are you sure you want to create this application?");

            if (msg === true) {

                $("#FormCreateNewOtherApplication").addClass("Submitloader");

                $.post("/CompanyApplication/CreateNextApplication",
                    {
                        "LinkId": $("#seltxtAppStage").val(),
                        "txtPermitNo": $("#txtRefNumber").val()
                    },
                    function (response) {

                        var res = $.trim(response).split("|");

                        if (res[0] === "1") {
                            window.location.href = "/CompanyApplication/ViewSubmission?id="+ res[1].trim();
                            $("#FormCreateNewOtherApplication").removeClass("Submitloader");
                        }
                        else {
                            $("#FormCreateNewOtherApplication").removeClass("Submitloader");
                            ErrorMessage("#MessageInfo", response);
                        }
                    });
            }
        }

    });






    $("#FormEditDST").on('submit', function (e) {
        e.preventDefault();

        if ($("#txtAppID").val() === "") {

            Notify.alert({
                title: 'Failure',
                text: "Application reference not in correct format."
            });
        }
        else {

            var msg = confirm("Are you sure you want to update this excel file?");

            var fileUpload = $("#UploadedFile").get(0);
            var files = fileUpload.files;
            var data = new FormData();

            if (msg === true) {

                data.append("Files", files[0]);
                data.append("AppId", $("#txtAppID").val());

                $("#FormEditDST").addClass("Submitloader");

                $.ajax({
                    url: '/CompanyApplication/UpdateFile',
                    data: data,
                    cache: false,
                    contentType: false,
                    processData: false,
                    type: 'POST',
                    async: false,
                    success: function (result) {
                        var message = result.split("|");
                        if ($.trim(message[0]) === "1") {
                            SuccessMessage("#MessageInfo", "Upload done successfully.");
                            location.reload(true);
                            $("#FormEditDST").removeClass("Submitloader");
                        }
                        else {
                            ErrorMessage("#MessageInfo", message[1]);
                            $("#FormEditDST").removeClass("Submitloader");
                        }
                    },
                    error: function (result) {
                        var message = result.split("|");
                        if ($.trim(message[0]) === "0") {
                            ErrorMessage("#MessageInfo", message[1]);
                            $("#FormEditDST").removeClass("Submitloader");
                        }
                    }
                });
            }

        }
    });



    $("#btnSaveEdit").on('click', function (event) {
        event.preventDefault();

        var msg = confirm("Are youu sure you want to make this changes?");

        if (msg == true) {

            $("#AppDiv").addClass("Submitloader");

            $.post("/CompanyApplication/SaveEdit",
                {
                    "AppId": $("#txtAppID").val(),
                    "Contractor": $("#txtContractorName").val(),
                    "RigName": $("#txtRigName").val(),
                    "RigType": $("#txtRigType").val(),
                },

                function (response) {
                    var result = $.trim(response);

                    if (result === "Saved") {
                        SuccessMessage("#MessageInfo2", "Edited successfully...");
                        var location = window.location.origin + "/CompanyApplication/ApplicationPayment/" + $("#txtAppID").val();
                        window.location.href = location;
                    }
                    else {
                        ErrorMessage("#divUploadDocInfo", result);
                        $("#AppDiv").removeClass("Submitloader");
                    }
                }
            );
        }
    });
    


    /*
     * Submitting uploaded documents forms
     */
    $("#btnSubmitDocuments").on('click', function (event) {
        event.preventDefault();

        var LocalID = document.getElementsByName('txtLocalDocID[]');
        var DocSource = document.getElementsByName('txtDocSource[]');
        var CompDocElpsID = document.getElementsByName('txtCompDocElpsID[]');
        var missingCompDocElpsID = document.getElementsByName('missingCompElpsDocID[]');

        var LocalID3 = document.getElementsByName('txtLocalDocID3[]');
        var DocSource3 = document.getElementsByName('txtDocSource3[]');
        var CompDocElpsID3 = document.getElementsByName('txtCompDocElpsID3[]');
        var SubmittedmissingCompDocElpsID = document.getElementsByName('missingCompElpsDocID4[]');
        var AppID = $("#txtAppID").val().trim();

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
        else if (SubmittedmissingCompDocElpsID.length > 0) {

            for (var t = 0; t < SubmittedmissingCompDocElpsID.length; t++) {
                if (SubmittedmissingCompDocElpsID[t].value === "0" || SubmittedmissingCompDocElpsID[t].value === "") {
                    ErrorMessage("#divUploadDocInfo", "Please Upload all addictional documents or remove it if not needed.");
                    break;
                }
                else {
                    alert("Inputs not found...");
                }
            }
        }
        else {

            // saving uploaded documents
            var AppDocuments = [];
            var MissingDocuments = [];

            for (var j = 0; j < LocalID.length; j++) {

                AppDocuments.push({
                    "LocalDocID": LocalID[j].value.trim(),
                    "CompElpsDocID": CompDocElpsID[j].value.trim(),
                    "DocSource": DocSource[j].value.trim()
                });
            }

            for (var k = 0; k < LocalID3.length; k++) {

                AppDocuments.push({
                    "LocalDocID": LocalID3[k].value.trim(),
                    "CompElpsDocID": CompDocElpsID3[k].value.trim(),
                    "DocSource": DocSource3[k].value.trim()
                });
            }

            var msg = confirm("Documents uploaded. Are you sure you want to proceed?");

            $("#divDocumentUpload").addClass("Submitloader");

            if (msg === true) {
                
                $.post("/CompanyApplication/SubmitDocuments",
                    {
                        "AppID": AppID,
                        "SubmittedDocuments": AppDocuments
                    },

                    function (response) {
                        var result = $.trim(response).split('|');

                        if (result[0] === "1") {
                            alert("Documents uploaded successfully... Please submit your application NOW");
                            var location = window.location.origin + "/CompanyApplication/SubmitApplication/" + result[1];
                            window.location.href = location;
                        }
                        else {
                            ErrorMessage("#divUploadDocInfo", result[1]);
                            $("#divDocumentUpload").removeClass("Submitloader");
                        }
                    }
                );
            }
            else {
                $("#divDocumentUpload").removeClass("Submitloader");
            }
        }
    });





    $("#btnCheckPermit").on('click', function (event) {
        event.preventDefault();
        var permitNumber = $("#txtRefNumber");
        var typeId = $("#txtTypeId").val();
        var typeStageId = $("#seltxtAppStage").val();
        $("#PermitInfo").html("");
        var html = "";

        if (permitNumber.val() === "") {
            ErrorMessage("#ErrorDiv", "Please select your permit from the dropdown.");
        }
        else {

            $.post("/Applications/GetPermitInfo",
                {
                    "PermitNO": permitNumber.val(),
                    "typeId": typeId,
                    "TypeStageId": typeStageId
                }, function (response) {

                if ($.isArray(response)) {
                    $.each(response, function (key, val) {
                        html += "<div class='alert alert-info'>";
                        html += "<b> Application Details for : " + val.refNo + "</b> <hr>";
                        html += "<p><b> Ref No : </b>" + val.refNo + " </p>";
                        html += "<p> <b>Permit Number : </b>" + val.permitNO + " </p>";
                        html += "<p> <b>Issue Date : </b>" + val.issuedDate + " </p>";
                        html += "<p> <b>Expiry Date : </b>" + val.expireDate + " </p>";
                        html += "<p> <b>Type : </b>" + val.type + " </p>";
                        html += "<p> <b>Stage : </b>" + val.stage + " </p>";
                        html += "<p> <b>My Company Details : </b>" + val.myCompanyDetails + " </p>";
                        html += "<p> <b>My Facility Details : </b>" + val.facilityDetails + " </p>";
                        html += "<p> <b>Status : </b>" + val.status + " </p>";
                        html += "<p> <b>Applied on : </b>" + val.createdAt + " </p>";
                        html += "</div>";
                    });
                }
                else {
                    html = "<b class='text-danger'> " + response + " </b>";
                }

                $("#PermitInfo").html(html);

            });
        }
    });



    $("#btnSelectNPMD").on("change", function (event) {
        event.preventDefault();

        if ($("#btnSelectNPMD").val() === "YES") {
            $("#ActionButtons").slideDown();
        }
        else {
            $("#ActionButtons").slideUp();
        }
    });



    /*
    * Submitting uploaded documents forms for report
    */
    $("#btnSubmitReportDocuments").on('click', function (event) {
        event.preventDefault();

        var LocalID = document.getElementsByName('txtLocalDocID[]');
        var DocSource = document.getElementsByName('txtDocSource[]');
        var CompDocElpsID = document.getElementsByName('txtCompDocElpsID[]');
        var missingCompDocElpsID = document.getElementsByName('missingCompElpsDocID[]');

        var AppID = $("#txtAppID").val().trim();

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

            // saving uploaded documents
            var AppDocuments = [];
           
            for (var j = 0; j < LocalID.length; j++) {

                AppDocuments.push({
                    "LocalDocID": LocalID[j].value.trim(),
                    "CompElpsDocID": CompDocElpsID[j].value.trim(),
                    "DocSource": DocSource[j].value.trim()
                });
            }

            var msg = confirm("Documents uploaded. Are you sure you want to proceed?");

            $("#divDocumentUpload").addClass("Submitloader");

            if (msg === true) {

                $.post("/CompanyApplication/SubmitReportDocuments",
                    {
                        "AppID": AppID,
                        "SubmittedDocuments": AppDocuments
                    },

                    function (response) {
                        var result = $.trim(response);

                        if (result === "Resubmitted" || result === "Submitted") {
                            alert("Documents uploaded successfully... Application report was successfully " + result);
                            var location = window.location.origin + "/Companies/MyApplications";
                            window.location.href = location;
                        }
                        else {
                            ErrorMessage("#divUploadDocInfo", result[1]);
                            $("#divDocumentUpload").removeClass("Submitloader");
                        }
                    }
                );
            }
            else {
                $("#divDocumentUpload").removeClass("Submitloader");
            }
        }
    });






    /*
     * ReSubmitting uploaded documents and application
     */
    $("#btnReSubmitDocuments").on('click', function (event) {
        event.preventDefault();

        var LocalID = document.getElementsByName('txtLocalDocID[]');
        var DocSource = document.getElementsByName('txtDocSource[]');
        var CompDocElpsID = document.getElementsByName('txtCompDocElpsID[]');
        var missingCompDocElpsID = document.getElementsByName('missingCompElpsDocID[]');

        var LocalID3 = document.getElementsByName('txtLocalDocID3[]');
        var DocSource3 = document.getElementsByName('txtDocSource3[]');
        var CompDocElpsID3 = document.getElementsByName('txtCompDocElpsID3[]');
        var SubmittedmissingCompDocElpsID = document.getElementsByName('missingCompElpsDocID4[]');
        var AppID = $("#txtAppID").val().trim();

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
        else if (SubmittedmissingCompDocElpsID.length > 0) {

            for (var t = 0; t < SubmittedmissingCompDocElpsID.length; t++) {
                if (SubmittedmissingCompDocElpsID[t].value === "0" || SubmittedmissingCompDocElpsID[t].value === "") {
                    ErrorMessage("#divUploadDocInfo", "Please Upload all addictional documents or remove it if not needed.");
                    break;
                }
                else {
                    alert("Inputs not found...");
                }
            }
        }
        else {

            // saving uploaded documents
            var AppDocuments = [];
            var MissingDocuments = [];

            for (var j = 0; j < LocalID.length; j++) {

                AppDocuments.push({
                    "LocalDocID": LocalID[j].value.trim(),
                    "CompElpsDocID": CompDocElpsID[j].value.trim(),
                    "DocSource": DocSource[j].value.trim()
                });
            }

            for (var k = 0; k < LocalID3.length; k++) {

                AppDocuments.push({
                    "LocalDocID": LocalID3[k].value.trim(),
                    "CompElpsDocID": CompDocElpsID3[k].value.trim(),
                    "DocSource": DocSource3[k].value.trim()
                });
            }

            var msg = confirm("Documents uploaded. Are you sure you want to proceed?");

            if (msg === true) {

                $("#divReDocumentUpload").addClass("Submitloader");

                $.post("/CompanyApplication/ReSubmitDocuments",
                    {
                        "AppID": AppID,
                        "ReSubmittedDocuments": AppDocuments
                    },

                    function (response) {
                        var result = response;

                        if (result === "Resubmitted") {
                            alert("Your application have been re-submitted successfully..");
                            var location = window.location.origin + "/Companies/MyApplications";
                            window.location.href = location;
                        }
                        else {
                            ErrorMessage("#divUploadDocInfo", result);
                            $("#divReDocumentUpload").removeClass("Submitloader");
                        }
                    }
                );
            }
            else {
                $("#divReDocumentUpload").removeClass("Submitloader");
            }
        }
    });




    // Submitting Application
    $("#btnSubmitApp").on('click', function (event) {
        event.preventDefault();

        var appid = $("#txtAppID").val();

        if (appid === "") {

            Notify.alert({
                title: 'Failure',
                text: "Application reference not in correct format."
            });
        }
        else {

            var msg = confirm("Are you sure you want to submit your application");

            if (msg === true) {

                $("#SubmitAppDiv").addClass("Submitloader");

                $.post("/CompanyApplication/SubmitApp",
                    {
                        "AppID": appid,
                    },
                    function (response) {
                        var result = $.trim(response);

                        if (result === "Submitted") {
                            alert("Your application has been submitted successfully.");
                            var location = window.location.origin + "/Companies/MyApplications/";
                            window.location.href = location;
                        }
                        else {
                            alert(result);
                            $("#SubmitAppDiv").removeClass("Submitloader");
                        }
                    }
                );
            }
        }
    });




    /*
     * Accepting application schedule
     */
    $("#btnAcceptSchedule").on('click', function (event) {
        event.preventDefault();

        if ($("#txtAcceptComment").val() === "") {
            ErrorMessage("#ScheduleModalInfo", "Please enter comment");
        }
        else {
            var msg = confirm("Are you sure you want to accept this schedule?");

            if (msg === true) {

                $("#DivAcceptSchedule").addClass("Submitloader");

                $.post("/Schedules/CustomerAcceptSchedule",
                    {
                        "ScheduleID": $("#txtScheduleID").val(),
                        "txtComment": $("#txtAcceptComment").val()
                    },
                    function (response) {
                        if ($.trim(response) === "Schedule Accepted") {
                            SuccessMessage("#ScheduleModalInfo", "Schedule approved successfully.");
                            location.reload(true);
                        }
                        else {

                            $("#DivAcceptSchedule").removeClass("Submitloader");
                            ErrorMessage("#ScheduleModalInfo", response);
                        }
                    });
            }
        }
    });


    /*
    * Rejecting application schedule
    */
    $("#btnRejectSchedule").on('click', function (event) {
        event.preventDefault();

        if ($("#txtRejectComment").val() === "") {
            ErrorMessage("#ScheduleRejectModalInfo", "Please enter comment");
        }
        else {

            var msg = confirm("Are you sure you want to reject this schedule?");

            if (msg === true) {

                $("#DivRejectSchedule").addClass("Submitloader");

                $.post("/Schedules/CustomerRejectSchedule",
                    {
                        "ScheduleID": $("#txtScheduleID").val(),
                        "txtComment": $("#txtRejectComment").val()
                    },
                    function (response) {
                        if ($.trim(response) === "Schedule Rejected") {
                            SuccessMessage("#ScheduleRejectModalInfo", "Schedule rejected successfully.");
                            location.reload(true);
                        }
                        else {
                            $("#DivRejectSchedule").removeClass("Submitloader");
                            ErrorMessage("#ScheduleRejectModalInfo", response);
                        }
                    });
            }
        }
    });


});




function WithdrawApp(id) {

    var mg = confirm("Are you sure you want to withdraw this application?");

    if (mg === true) {

        $("#MyAppsTableData").addClass("Submitloader");

        $.post("/CompanyApplication/WithdrawApplication",
            {
                "AppID": id,
            },
            function (response) {
                if ($.trim(response) === "Withdrawn") {
                    alert("Application withdrawn successfully");
                    location.reload(true);
                }
                else {
                    $("#MyAppsTableData").removeClass("Submitloader");
                    alert(response);
                }
            });
    }
}



function ResubmitApp(id) {

    var mg = confirm("Are you sure you want to Resubmit this application?");

    if (mg === true) {

        $("#MyAppsTableData").addClass("Submitloader");

        $.post("/CompanyApplication/ResubmitApplication",
            {
                "AppID": id,
            },
            function (response) {
                if ($.trim(response) === "Resubmitted") {
                    alert("Application Resubmitted successfully");
                    location.reload(true);
                }
                else {
                    $("#MyAppsTableData").removeClass("Submitloader");
                    alert(response);
                }
            });
    }
}



