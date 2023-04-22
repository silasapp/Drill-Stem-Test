
$(function () {

    // error message
    function ErrorMessage(error_id, error_message) {
        $(error_id).fadeIn('fast')
            .html("<div class=\"alert alert-danger\"><i class=\"fa fa-warning\"> </i> <span class=\"text-dark\"> " + error_message + " </span> </div>")
            .delay(9000)
            .fadeOut('fast');
        return;
    }

    // success message
    function SuccessMessage(success_id, success_message) {
        $(success_id).fadeIn('fast')
            .html("<div class=\"alert alert-success\"> <i class=\"fa fa-check-circle\"> </i> <span class=\"text-dark\">" + success_message + " </span> </div>")
            .delay(10000)
            .fadeOut('fast');
        return;
    }

    


    var OtherDocs = $("#OtherDocsTable").DataTable({
        
        "paging": false,
        "ordering": false,
        "info": false,
        "searching": false,

        "columnDefs": [
            {
                "orderable": false,
                "targets": 0,
                "width": "20%",
                'checkboxes': {
                    'selectRow': true
                }
            }
        ],

        "select": {
            'style': 'multi'
        }

    });


    var apps = [];
    var myDeskApps = [];
    var staffApps = [];


    $("#btnPushApp").on('click', function (event) {
        event.preventDefault();

        apps.length = 0;

        var rows_selected = AppDropTable.column(0).checkboxes.selected();

        if (rows_selected.length !== 0) {

            $.each(rows_selected, function (index, rowId) {
                apps.push(rowId);
            });

            var str = apps.join(",");
            $("#showpush").val(str);

            $("#btnPushApp2").click();
            pushStaff.ajax.reload();
        }
        else {
            alert("Please select applications to assign");
        }
    });




    /*
     * Displaying all application on a particular staff desk for distribution
     */ 
    var staffDeskApps = $("#MyDeskAppTable").DataTable({

        'initComplete': function (settings) {
            var api = this.api();

            api.cells(
                api.rows(function (idx, data, node) {
                    
                    return (data[8] === '<b>YES</b>') ? true : false;
                }).indexes(),
                0
            ).checkboxes.disable();
        },

        'columnDefs': [
            {
                'targets': 0,
                'checkboxes': {
                    'selectRow': true
                },
                'createdCell': function (td, cellData, rowData, row, col) {

                    if ($.trim(rowData["hasPushed"]) === 'true') {
                        this.api().cell(td).checkboxes.disable();
                    }
                }
            }
        ],

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

        'deferRender': true,
        "select": {
            'style': 'multi'
        }
    });



    var staffPustTable = $("#StaffPushTable").DataTable({

        ajax: {
            url: "/Applications/GetPushStaff?id=" + $("#txtFromStaffID").val(),
            type: "POST",
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]]
        },


        "processing": true,
        "serverSide": true,
      
        'columnDefs': [
            {
                'targets': 0,
                'checkboxes': {
                    'selectRow': true
                },
                'createdCell': function (td, cellData, rowData, row, col) {

                }
            }
        ],

        'deferRender': true,
        "select": {
            'style': 'multi'
        },

        columns: [
            {
                data: "staffID"
            },
            { data: "staffName" },
            { data: "staffEmail" },
            { data: "roleName" },
            { data: "officeName" },
            { data: "zonalOffice" }
        ]
    });



    /*
    * Getting applications on staffs desk for processing
    * 
    */
    var MyDeskTable = $("#MyDeskAppsTable").DataTable({

        'deferRender': true,
        "select": {
            'style': 'multi'
        },

        ajax: {
            url: "/Applications/GetMyDeskApps",
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

        columns: [
            //{
            //    data: "deskID"
            //},
            {
                "render": function (data, type, row) {
                    return "<b class=\"text-danger\"> " + row["refNo"] + " </b>";
                }
            },

            {
                "render": function (data, type, row) {
                    return "<b class=\"text-primary\"> " + row["companyName"] + " </b>";
                }
            },
            
            {
                "render": function (data, type, row) {
                    return "<b class=\"text-dark\"> " + row["stage"] + " </b>";
                }
            },
            {
                "render": function (data, type, row) {
                    return "<b class=\"text-dark\"> " + row["proposalApproved"] + " </b>";
                }
            },
            {
                "render": function (data, type, row) {
                    return "<b class=\"text-dark\"> " + row["reportApproved"] + " </b>";
                }
            },

            {
                "render": function (data, type, row) {
                    return "<button class=\"btn btn-sm btn-warning\"> " + row["status"] + " </button>";
                }
            },
            { data: "dateApplied" },
            { data: "dateSubmitted" },
            { data: "updatedAt" },
            {
                "render": function (data, type, row) {
                    return "<b class=\"text-primary\"> " + row["activity"] + " </b>";
                }
            },
            {
                "render": function (data, type, row) {
                    return "<button class=\"btn btn-sm btn-info\" onclick=\"ViewApplication(" + row["deskID"] + "," + row["processID"] + ")\"> <i class=\"fa fa-eye\"> </i> view Application </button>";
                }
            }
        ]
    });



    $("#btnSinglePush").on('click', function (event) {

        var id = $("#txtSinglePush");

        if (id.val() !== "") {
            $("#btnPushSingleApp").click();
        }
        else {
            ErrorMessage("#AppViewInfo", "Application link was broken and cannot be Assign");
        }

    });



    $("#btnInsPushApp").on('click', function (event) {
        event.preventDefault();

        myDeskApps.length = 0;

        var rows_selected = MyDeskTable.column(0).checkboxes.selected();

        if (rows_selected.length !== 0) {

            $.each(rows_selected, function (index, rowId) {
                myDeskApps.push(rowId);
            });

            var str = myDeskApps.join(",");
            $("#Insshowpush").val(str);

            $("#btnInsPushApp2").click();
            MyDeskTable.ajax.reload();
        }
        else {

            alert("Please select applications to assign");
        }
    });



    /*
     * This is to rerout all application from the desk of one staff to another
     */
    $("#btnStaffRerouteApps").on('click', function (event) {
        event.preventDefault();

        staffApps.length = 0;

       // var rows_select = staffDeskApps.column(0).checkboxes.select();
        var rows_selected = staffDeskApps.column(0).checkboxes.selected();

        if (rows_selected.length !== 0) {

            $.each(rows_selected, function (index, rowId) {
                staffApps.push(rowId);
            });

            var str = staffApps.join(",");
            $("#Insshowpush").val(str);

            $("#btnGetStaffs").click();
        }
        else {

            alert("Please select staff and applications to Re-route");
        }
    });




    $("#btnGetStaffs").on('click', function (event) {

        var RouteApp = $("#TableDistributeApps").DataTable({

            ajax: {
                url: "/Deskes/GetRouteStaff?staff=" + $("#txtOriginalStaffID").val(),
                type: "POST",
                lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]]
            },

            "destroy": true,
            "processing": true,
            "serverSide": true,
            "searching": false,
            "info": false,
            "paging": true,
            "lengthChange": false,

            columns: [
                { data: "lastName" },
                { data: "firstName" },

                { data: "staffEmail" },
                {
                    "render": function (data, type, row) {
                        return "<button class=\"btn btn-sm btn-info push\" onclick=\"RerouteApps(" + row["staffId"] + ")\"> Re-Route Application </button>";
                    }
                }
            ]
        });

    });




    /*
     * Push application to inspector
     */
    var pushToInspector = $("#InsStaffPush").DataTable({

        ajax: {
            url: "/Applications/GetPushStaff",
            type: "POST",
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]]
        },

        "processing": true,
        "serverSide": true,
        "searching": false,
        "info": false,
        "paging": true,
        "lengthChange": false,

        columns: [
            { data: "lastName" },
            { data: "firstName" },

            { data: "staffEmail" },
            {
                "render": function (data, type, row) {
                    return "<button class=\"btn btn-sm btn-info push\" onclick=\"InsPushApp(" + row["staffId"] + ")\"> Assign Application </button>";
                }
            }
        ]
    });


    /*
     * Table for list of staff for single push 
     */
    var pushSingleApp = $("#SinglePushTable").DataTable({

        ajax: {
            url: "/Applications/GetPushStaff",
            type: "POST",
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]]
        },

        "processing": true,
        "serverSide": true,
        "searching": false,
        "info": false,
        "paging": true,
        "lengthChange": false,

        columns: [
            { data: "lastName" },
            { data: "firstName" },

            { data: "staffEmail" },
            {
                "render": function (data, type, row) {
                    return "<button class=\"btn btn-sm btn-info push\" onclick=\"SinglePushApp(" + row["staffId"] + ")\"> Assign Application </button>";
                }
            }
        ]
    });





    /*
     * Saving an application Report
     */
    $("#btnSaveReport").on('click', function (event) {

        var txtReport = $("#txtAppReport");
        var txtReportTitle = $("#txtReportTitle");

        if (txtReport.val() === "" || $("#txtAppId").val() === "" || $("#txtReportTitle").val() === "") {
            ErrorMessage("#ReportModalInfo", "Please enter the report title and comment for this report");
        }
        else {

            // saving uploaded documents
            var AppDocuments = [];

            var mssg = confirm("Click OK if you added a file for upload, Click Cancle if you didn't");

            if (mssg === true) {

                var LocalID = document.getElementsByName('txtLocalDocID[]');
                var DocSource = document.getElementsByName('txtDocSource[]');
                var CompDocElpsID = document.getElementsByName('txtCompDocElpsID[]');
                var missingCompDocElpsID = document.getElementsByName('missingCompElpsDocID[]');

                for (var j = 0; j < LocalID.length; j++) {

                    AppDocuments.push({
                        "LocalDocID": LocalID[j].value.trim(),
                        "CompElpsDocID": CompDocElpsID[j].value.trim(),
                        "DocSource": DocSource[j].value.trim()
                    });
                }
            }
            else {
                AppDocuments.push({
                    "LocalDocID": 0,
                    "CompElpsDocID": 0,
                    "DocSource": "NILL"
                });
            }


            var msg = confirm("Are you sure you want to save this report");

            if (msg === true) {

                $("#AppReportLoader").addClass("Submitloader");

                $.post("/Applications/SaveReport",
                    {
                        "AppID": $("#txtAppId").val(),
                        "txtReport": txtReport.val(),
                        "txtReportTitle": $("#txtReportTitle").val(),
                        "SubmittedDocuments": AppDocuments
                    },
                    function (response) {

                        if ($.trim(response) === "Report Saved") {
                            $("#AppReportLoader").removeClass("Submitloader");
                            txtReport.val("");
                            txtReportTitle.val("")
                            SuccessMessage("#ReportModalInfo", "Report successfully saved.");
                            $("#divReport").load(location.href + " #divReport");

                        }
                    else {
                        ErrorMessage("#ReportModalInfo", response);
                        $("#AppReportLoader").removeClass("Submitloader");
                    }
                });
            }
            else {
                $("#AppReportLoader").removeClass("Submitloader");
            }
        }
    });



    /*
    * Editing an application Report
    */
    $("#btnEditReport").on('click', function (event) {

        var txtReport = $("#txtEditAppReport");
        var txtReportTitle = $("#txtEditReportTitle");

        if (txtReport.val() === "" || $("#txtEditReportID").val() === "" || txtReportTitle.val() === "") {
            ErrorMessage("#EditReportModalInfo", "Please enter a comment and title for this report");
        }
        else {


            // saving uploaded documents
            var AppDocuments = [];

            var mssg = confirm("Click OK if you added a file for upload, Click Cancle if you didn't");

            if (mssg === true) {

                var LocalID = document.getElementsByName('txtELocalDocID[]');
                var DocSource = document.getElementsByName('txtEDocSource[]');
                var CompDocElpsID = document.getElementsByName('txtECompDocElpsID[]');
                var missingCompDocElpsID = document.getElementsByName('missingCompElpsDocID[]');

                for (var j = 0; j < LocalID.length; j++) {

                    AppDocuments.push({
                        "LocalDocID": LocalID[j].value.trim(),
                        "CompElpsDocID": CompDocElpsID[j].value.trim(),
                        "DocSource": DocSource[j].value.trim()
                    });
                }
            }
            else {
                AppDocuments.push({
                    "LocalDocID": 0,
                    "CompElpsDocID": 0,
                    "DocSource": "NILL"
                });
            }


            var msg = confirm("Are you sure you want to edit this report");

            if (msg === true) {

                $("#AppEditReportLoadder").addClass("Submitloader");

                $.post("/Applications/EditReport",
                    {
                        "ReportID": $("#txtEditReportID").val(),
                        "txtReport": txtReport.val(),
                        "txtReportTitle": txtReportTitle.val(),
                        "SubmittedDocuments": AppDocuments
                    },
                    function (response) {
                        if ($.trim(response) === "Report Edited") {
                            SuccessMessage("#EditReportModalInfo", "Report successfully edited.");
                            txtReport.val("");
                            txtReportTitle.val("");
                            $("#divReport").load(location.href + " #divReport");
                            $("#AppEditReportLoadder").removeClass("Submitloader");
                        }
                        else {
                            ErrorMessage("#EditReportModalInfo", response);
                            $("#AppEditReportLoadder").removeClass("Submitloader");
                        }
                    });
            }
            else {
                $("#AppEditReportLoadder").removeClass("Submitloader");
            }
        }
    });



    /*
     * App schdule datetimepicker
     */
    $("#txtSchduleDate").datetimepicker({
        minDate: new Date().setDate(new Date().getDate() + 1), //- this is tomorrow;  use 0 for toady
        defaultDate: new Date().setDate(new Date().getDate() + 1),
        onGenerate: function (ct) {
            $(this).find('.xdsoft_date.xdsoft_weekend')
                .addClass('xdsoft_disabled');
        }
    });



    /*
     * edit App schdule datetimepicker
     */
    $("#txtEditSchduleDate").datetimepicker({
        minDate: new Date().setDate(new Date().getDate() + 1), //- this is tomorrow;  use 0 for toady
        defaultDate: new Date().setDate(new Date().getDate() + 1),
        onGenerate: function (ct) {
            $(this).find('.xdsoft_date.xdsoft_weekend')
                .addClass('xdsoft_disabled');
        }
    });




    /*
     * Create a schdule
     */
    $("#btnSaveSchdule").on('click', function (event) {
        event.preventDefault();

        if ($("#txtSchduleComment").val() === "" || $("#txtSchduleDate").val() === "") {
            ErrorMessage("#SchduleModalInfo", "Please enter schedule date and comment");
        }
        else {
            msg = confirm("Are you sure you want to create this schedule?");
            $("#AppSchduleLoadder").addClass("Submitloader");

            if (msg === true) {
                $.post("/Applications/CreateSchdule",
                    {
                        "SchduleDate": $("#txtSchduleDate").val(),
                        "SchduleComment": $("#txtSchduleComment").val(),
                        "AppID": $("#txtAppId").val(),
                        "DeskID": $("#txtDeskID").val(), // dask id
                        "SchduleLocation": $("#txtSchduleLocation").val(),
                        "SchduleType": $("#txtSchduleType").val()
                    },
                    function (response) {
                        if ($.trim(response) === "Schdule Created") {
                            $("#divSchdules").load(location.href + " #divSchdules");
                            SuccessMessage("#SchduleModalInfo", "Schedule created successfully");
                            $("#txtSchduleComment").val('');
                            $("#txtSchduleDate").val('');
                            $("#AppSchduleLoadder").removeClass("Submitloader");
                        }
                        else {
                            ErrorMessage("#SchduleModalInfo", response);
                            $("#AppSchduleLoadder").removeClass("Submitloader");
                        }
                    });
            }
            else {
                $("#AppSchduleLoadder").removeClass("Submitloader");
            }
        }
    });



    /*
     * Edit a schdule 
     */
    $("#btnEditSchdule").on('click', function (event) {
        event.preventDefault();

        if ($("#txtEditSchduleDate").val() === "" || $("#txtEditSchduleID").val() === "" || $("#txtEditSchduleComment").val() === "") {
            ErrorMessage("#EditSchduleModalInfo", "Please enter a comment annd date for this schedule.");
        }
        else {
            var msg = confirm("Are you sure you want to edit this schedule");
            $("#AppEditSchduleLoadder").addClass("Submitloader");
            if (msg === true) {

                $.post("/Applications/EditSchdule",
                    {
                        "schduleID": $("#txtEditSchduleID").val(),
                        "txtComment": $("#txtEditSchduleComment").val(),
                        "txtSchduleDate": $("#txtEditSchduleDate").val(),
                        "txtSchduleLoaction": $("#txtEditSchduleLocation").val(),
                        "txtSchduleType": $("#txtEditSchduleType").val()
                    },
                    function (response) {
                        if ($.trim(response) === "Schdule Edited") {
                            SuccessMessage("#EditSchduleModalInfo", "Schedule successfully edited.");
                            $("#txtEditSchduleDate").val("");
                            $("#txtEditSchduleComment").val("");
                            $("#divSchdules").load(location.href + " #divSchdules");
                            $("#AppEditSchduleLoadder").removeClass("Submitloader");
                        }
                        else {
                            ErrorMessage("#EditSchduleModalInfo", response);
                            $("#AppEditSchduleLoadder").removeClass("Submitloader");
                        }
                    });
            }
            else {
                $("#AppEditSchduleLoadder").removeClass("Submitloader");
            }
        }

    });



    /*
     * Approval
     */
    $("#btnApprove").on('click', function (event) {
        event.preventDefault();

        var txtComment = $("#txtAppApproveComment");
        var txtDeskID = $("#txtDeskID");
        var txtAppID = $("#txtAppId");
        var txtVolume = $("#txtVolume");
        var appstage = $("#txtAppStage")
        
        var location = window.location.origin + "/Applications/MyDesk";

        if (txtComment.val() === "") {
            ErrorMessage("#ApproveModalInfo", "Please enter a comment.");
        }
        else if (txtVolume.val() === "" && appstage.val().indexOf("MER") < 0) {
            ErrorMessage("#ApproveModalInfo", "Please enter the proposed volume");
        }
        else if (txtDeskID.val() === "") {
            ErrorMessage("#ApproveModalInfo", "Error... Missing application. Refresh the page.");
        }
        else {
            var msg = confirm("Are you sure you want to Approve this application?");

            if (msg === true) {

                $("#AppApproveLoadder").addClass("Submitloader");

                $.post("/Applications/Approval",
                    {
                        "txtComment": txtComment.val(),
                        "txtDeskID": txtDeskID.val(),
                        "txtAppID": txtAppID.val(),
                        "txtVolume": txtVolume.val()
                    },
                    function (response) {

                        if ($.trim(response) === "Approved Next") {
                            SuccessMessage("#ApproveModalInfo", "Application approved and moved to the next processing officer.");
                            alert("Application approved and moved to the next processing officer.");
                            window.location.href = location;
                            $("#AppApproveLoadder").removeClass("Submitloader");
                        }
                        else if ($.trim(response) === "Approved") {
                            SuccessMessage("#ApproveModalInfo", "SUCCESSFUL FINAL APPROVAL.");
                            alert("FINAL APPLICATION APPROVED.");
                            window.location.href = location;

                            $("#AppApproveLoadder").removeClass("Submitloader");
                        }
                        else if ($.trim(response) === "Report Approved") {
                            SuccessMessage("#ApproveModalInfo", "FINAL DST REPORT APPROVED.");
                            alert("FINAL DST REPORT APPROVED.");
                            window.location.href = location;

                            $("#AppApproveLoadder").removeClass("Submitloader");
                        }
                        else {
                            ErrorMessage("#ApproveModalInfo", response);
                            $("#AppApproveLoadder").removeClass("Submitloader");
                        }
                    });
            }
            else {
                $("#AppApproveLoadder").removeClass("Submitloader");
            }
        }
    });




    $("#btnDisApprove").on('click', function (event) {
        event.preventDefault();

        var txtComment = $("#txtAppDisApproveComment");
        var txtDeskID = $("#txtDeskID");

        if (txtComment.val() === "") {
            ErrorMessage("#DisApproveModalInfo", "Please enter a comment for your disapprove.");
        }
        else {

            var msg = confirm("Are you sure you want to disapprove this application? Please not that this applicant will have to re-apply.");

            $("#AppDisapproveLoadder").addClass("Submitloader");

            if (msg === true) {

                $.post("/Applications/DenyApplication", { "DeskID": txtDeskID.val(), "txtComment": txtComment.val() }, function (response) {

                    if (response === "Denied") {
                        alert("Application disapprove Successfully.");
                        SuccessMessage("#DisApproveModalInfo", "Application disapprove Successfully.");
                        var location = window.location.origin + "/Applications/MyDesk";
                        window.location.href = location;

                        $("#AppDisapproveLoadder").removeClass("Submitloader");
                    }
                    else {
                        $("#AppDisapproveLoadder").removeClass("Submitloader");
                        ErrorMessage("#DisApproveModalInfo", response);
                    }
                });
            }
            else {
                $("#AppDisapproveLoadder").removeClass("Submitloader");
            }
        }
    });




    /*
     * Button for external rejection
     */
    $("#btnReject").on('click', function (event) {

        var txtComment = $("#txtAppExRejectComment");
        var txtDeskID = $("#txtDeskID");
        var docs = [];

        var rows_selected = OtherDocs.column(0).checkboxes.selected();

        if (rows_selected.length !== 0) {
            $.each(rows_selected, function (index, rowId) {
                docs.push(rowId);
            });
        }

        if (txtComment.val() === "") {
            ErrorMessage("#ExRejectModalInfo", "Please enter a comment for rejection.");
        }
        else {

            var msg = confirm("Are you sure you want to reject this application?");

            $("#AppExternalRejectionLoadder").addClass("Submitloader");

            var location = window.location.origin + "/Applications/MyDesk";

            if (msg === true) {

                $.post("/Applications/Rejection", { "DeskID": txtDeskID.val(), "RequiredDocs": docs, "txtComment": txtComment.val() }, function (response) {

                    if ($.trim(response) === "External Rejection") {
                        alert("Application has been rejected back to customer.");
                        SuccessMessage("#ExRejectModalInfo", "Application has been rejected back to the company.");
                        
                        window.location.href = location;

                        $("#AppExternalRejectionLoadder").removeClass("Submitloader");
                    }
                    else if ($.trim(response) === "Internal Rejection") {
                        alert("Application has been internally rejected back to the previous processing staff.");
                        SuccessMessage("#ExRejectModalInfo", "Application has been internally rejected back to the previous processing staff.");
                       
                        window.location.href = location;
                    }
                    else {
                        $("#AppExternalRejectionLoadder").removeClass("Submitloader");
                        ErrorMessage("#ExRejectModalInfo", response);
                    }
                });
            }
            else {
                $("#AppExternalRejectionLoadder").removeClass("Submitloader");
            }
        }
    });



    /*
    * Saving an nominated staff
    */

    $("#btnSendNominationRequest").on('click', function (event) {

        var txtNominatedStaff = $("#txtNominatedStaff");
        //var split = txtNominatedStaff.val().split('|');
        var split = txtNominatedStaff.val();


        //if (txtNominatedStaff.val() === "" || $("#txtAppId").val() === "" || split[2] === "") {
        if (txtNominatedStaff.val() === "" || $("#txtAppId").val() === "" || split === "") {
            ErrorMessage("#AddStaffModalInfo", "Please select a staff");
        }
        else {

            Notify.confirm({
                title: 'Nomination Request',
                text: 'Are you sure you want to send this nomination request',
                ok: function () {
                    $("#AddStaffLoader").addClass("Submitloader");
                    $.post("/Applications/SaveNominatedStaff",
                        {
                            "txtAppId": $("#txtAppId").val(),
                            "StaffId": split,
                            "txtComment": $("#txtRequestComment").val(),
                        },
                        function (response) {
                            if ($.trim(response) === "Saved") {
                                SuccessMessage("#AddStaffModalInfo", "Nomination request(s) sent.");
                                txtNominatedStaff.val("");
                                $("#txtComment").val("");
                                $("#divNominatedStaff").load(location.href + " #divNominatedStaff");
                                $("#AddStaffLoader").removeClass("Submitloader");
                            }
                            else {
                                ErrorMessage("#AddStaffModalInfo", response);
                                $("#AddStaffLoader").removeClass("Submitloader");
                            }
                        });
                }
            });

        }
    });




    /*
     * Reseting application step
     *
     */
    $("#btnResetStep").on('click', function (event) {
        var app_id = $("#btnResetStep").val();

        var msg = confirm("Are you sure you want to reset all step for this application?");

        if (msg === true) {
            $.post("/Applications/ResetStep", { "AppID": app_id }, function (response) {
                if ($.trim(response) === "Reset Done") {
                    SuccessMessage("#ResetStepInfo", "All reset done for this application.");
                }
                else {
                    ErrorMessage("#ResetStepInfo", response);
                }
            });
        }
    });


    $("#RecordView").on('click', function (event) {
        $("#CalendarDiv").slideUp();
        $("#SchduleDiv").slideDown();
        $("#CalendarDiv").addClass("hide");
    });

    $("#CalendarView").on('click', function (event) {
        $("#SchduleDiv").slideUp();
        $("#CalendarDiv").slideDown();
        $("#CalendarDiv").removeClass("hide");
    });



    /*
    * Saving an application nomination Report
    */
    $("#btnNSaveReport").on('click', function (event) {

        var txtReport = $("#txtNAppReport");
        var txtReportTitle = $("#txtNReportTitle");

        if (txtReport.val() === "" || $("#txtNormID").val() === "" || $("#txtNReportTitle").val() === "") {
            ErrorMessage("#NReportModalInfo", "Please enter the report title and comment for this report");
        }
        else {

            // saving uploaded documents
            var AppDocuments = [];

            var mssg = confirm("Click OK if you added a file for upload, Click Cancle if you didn't");

            if (mssg === true) {

                var LocalID = document.getElementsByName('txtLocalDocID[]');
                var DocSource = document.getElementsByName('txtDocSource[]');
                var CompDocElpsID = document.getElementsByName('txtCompDocElpsID[]');
                var missingCompDocElpsID = document.getElementsByName('missingCompElpsDocID[]');

                for (var j = 0; j < LocalID.length; j++) {

                    AppDocuments.push({
                        "LocalDocID": LocalID[j].value.trim(),
                        "CompElpsDocID": CompDocElpsID[j].value.trim(),
                        "DocSource": DocSource[j].value.trim()
                    });
                }
            }
            else {
                AppDocuments.push({
                    "LocalDocID": 0,
                    "CompElpsDocID": 0,
                    "DocSource": "NILL"
                });
            }


            var msg = confirm("Are you sure you want to save this report");

            if (msg === true) {

                $("#NReportLoader").addClass("Submitloader");

                $.post("/Applications/SaveNominationReport",
                    {
                        "NormID": $("#txtNormID").val(),
                        "txtReport": txtReport.val(),
                        "txtReportTitle": txtReportTitle.val(),
                        "SubmittedDocuments": AppDocuments
                    },
                    function (response) {

                        if ($.trim(response) === "Report Saved") {
                            
                            txtReport.val("");
                            txtReportTitle.val("");
                            alert("Report successfully saved.");
                            SuccessMessage("#NReportModalInfo", "Report successfully saved.");
                            var location = window.location.origin + "/Applications/NominatedStaff";
                            window.location.href = location;
                            $("#NReportLoader").removeClass("Submitloader");
                        }
                        else {
                            ErrorMessage("#NReportModalInfo", response);
                            $("#NReportLoader").removeClass("Submitloader");
                        }
                    });
            }
            else {
                $("#NReportLoader").removeClass("Submitloader");
            }
        }
    });



    /*
   * update an application nomination Report
   */
    $("#btnNEditReport").on('click', function (event) {

        var txtReport = $("#txtNEditAppReport");
        var txtReportTitle = $("#txtNEditReportTitle");

        if (txtReport.val() === "" || $("#txtNormID").val() === "" || txtReportTitle.val() === "") {
            ErrorMessage("#NEditReportModalInfo", "Please enter the report title and comment for this report");
        }
        else {

            // saving uploaded documents
            var AppDocuments = [];

            var mssg = confirm("Click OK if you added a file for upload, Click Cancle if you didn't");

            if (mssg === true) {

                var LocalID = document.getElementsByName('txtLocalDocID[]');
                var DocSource = document.getElementsByName('txtDocSource[]');
                var CompDocElpsID = document.getElementsByName('txtCompDocElpsID[]');
                var missingCompDocElpsID = document.getElementsByName('missingCompElpsDocID[]');

                for (var j = 0; j < LocalID.length; j++) {

                    AppDocuments.push({
                        "LocalDocID": LocalID[j].value.trim(),
                        "CompElpsDocID": CompDocElpsID[j].value.trim(),
                        "DocSource": DocSource[j].value.trim()
                    });
                }
            }
            else {
                AppDocuments.push({
                    "LocalDocID": 0,
                    "CompElpsDocID": 0,
                    "DocSource": "NILL"
                });
            }


            var msg = confirm("Are you sure you want to update this report");

            if (msg === true) {

                $("#NReportLoader").addClass("Submitloader");

                $.post("/Applications/EditNominationReports",
                    {
                        "NormID": $("#txtNormID").val(),
                        "txtReport": txtReport.val(),
                        "txtReportTitle": txtReportTitle.val(),
                        "SubmittedDocuments": AppDocuments
                    },
                    function (response) {

                        if ($.trim(response) === "Report Edited") {

                            txtReport.val("");
                            txtReportTitle.val("");
                            alert("Report successfully updated.");
                            SuccessMessage("#NEditReportModalInfo", "Report successfully updated.");
                            var location = window.location.origin + "/Applications/NominatedStaff";
                            window.location.href = location;
                            $("#NEditReportLoader").removeClass("Submitloader");
                        }
                        else {
                            ErrorMessage("#NEditReportModalInfo", response);
                            $("#NEditReportLoader").removeClass("Submitloader");
                        }
                    });
            }
            else {
                $("#NEditReportLoader").removeClass("Submitloader");
            }
        }
    });


});








/*
 * Push application to supervisor
 */ 
function PushApp(id) {

    var apps = $("#showpush").val().split(",");

    var msg = confirm("Are you sure you want to assign application(s) to this staff?");

    if (msg === true) {

        $(".push").attr("disable", "disabled");

        $.post("/Applications/PushApplications", { "staffID": id, "Application": apps }, function (respons) {
            if (respons.trim() === "Pushed") {

                alert("Application(s) Assign successfully...");
                location.reload(true);
            }
            else {
                $("#AppDropInfo").text(respons);
                $(".push").removeAttr("disabled");
            }
        });
    }
    
}


/*
 * Push application to inspector
 */ 
function InsPushApp(id) {

    var apps = $("#Insshowpush").val().split(",");

    var msg = confirm("Are you sure you want to assign application(s) to this staff?");

    if (msg === true) {

        $(".push").attr("disable", "disabled");

        $.post("/Applications/PushInsApplications", { "staffID": id, "DeskID": apps }, function (respons) {
            if (respons.trim() === "Pushed") {
                alert("Application(s) Assigned successfully...");
                location.reload(true);
            }
            else {
                $("#InsAppDropInfo").text(respons);

                $(".push").removeAttr("disabled");
            }
        });
    }

}


/*
 * Push single application to inspector
 */
function SinglePushApp(id) {

    var apps = $("#txtSinglePush").val().split(",");
    var txtPushComment = $("#txtPushComment").val();

    var msg = confirm("Are you sure you want to assign application(s) to this staff?");

    if (msg === true) {

        $(".push").attr("disable", "disabled");

        $.post("/Applications/PushInsApplications", { "staffID": id, "DeskID": apps, "PushComment": txtPushComment }, function (respons) {
            if (respons.trim() === "Pushed") {
                alert("Application(s) Assigned successfully...");
                var location = window.location.origin + "/Applications/MyDesk";
                window.location.href = location;
            }
            else {
                $("#SinglePushInfo").text(respons);

                $(".push").removeAttr("disabled");
            }
        });
    }

}


/*
 * View application
 */ 
function ViewApplication(app_id, process_id) {

    $.getJSON("/Helpers/GetEncrypt", { "app_id": app_id, "process_id": process_id }, function (response) {
        var r = response.split("|");
        var location = window.location.origin + "/Applications/ViewApplication/" + r[0] + "/" + r[1];
        window.location.href = location;
    });
}



/*
 * Get applicatioon report for edit
 */
function GetReport(ReportID) {

    $("#txtEditReportID").val(ReportID);
   
    $.getJSON("/Applications/GetReport", { "ReportID": ReportID }, function (response) {
        var res = $.trim(response).split("|");

        if (res[0] === "1") {
            $("#txtEditAppReport").val(res[1]);
            $("#txtEditReportTitle").val(res[2]);
        }
        else {
            ErrorMessage("#ReportModalInfo", res[1]);
        }
    });
}


/*
 * Deleteing an application report
 */
function DeleteReport(reportID) {
    var msg = confirm("Are you sure you want to delete this report?");

    if (msg === true) {

        $.getJSON("/Applications/DeleteReport", { "ReportID": reportID }, function (response) {

            if (response === "Report Deleted") {
                alert("Application report removed successfully.");
                $("#divReport").load(location.href + " #divReport");
            }
            else {
                ErrorMessage("#AppViewInfo", response);
            }
        });
    }
}



/*
 * Getting schdule
 */
function GetSchdule(schduleID) {

    $("#txtEditSchduleID").val(schduleID);

    $.getJSON("/Applications/GetSchdule", { "schduleID": schduleID }, function (response) {
        var res = $.trim(response).split("|");

        if (res[0] === "1") {
            $("#txtEditSchduleComment").val(res[1]);
            $("#txtEditSchduleDate").val(res[2]);
        }
        else {
            ErrorMessage("#EditSchduleModalInfo", res[1]);
        }
    });
}


/*
 * Delete a schdule
 */
function DeleteSchdule(schduleID) {

    var msg = confirm("Are you sure you want to delete this schedule?");

    if (msg === true) {

        $.getJSON("/Applications/DeleteSchdule", { "schduleID": schduleID }, function (response) {

            if (response === "Schdule Deleted") {
                alert("Application schedule removed successfully.");
               
                $("#divSchdules").load(location.href + " #divSchdules");
            }
            else {
                ErrorMessage("#AppViewInfo", response);
            }
        });
    }
}






/*
 * Re route applicationn to another staff
 */
function RerouteApps(id) {

    var apps = $("#Insshowpush").val().split(",");

    var previousStaff = $("#txtOriginalStaffID").val();

    var msg = confirm("Are you sure you want to re-route application(s) to this staff?");

    if (msg === true) {

        $(".push").attr("disable", "disabled");

        $.post("/Deskes/RerouteApps", { "staffID": id, "previousStaff": previousStaff, "AppID": apps }, function (respons) {
            var result = respons.trim().split("|");

            if (result[0] === "1") {
                alert(result[1]);
                var location = window.location.origin + "/Deskes/StaffDesk";
                window.location.href = location;
            }
            else {
                ErrorMessage("#InsAppDropInfo", result[1]);
                $(".push").removeAttr("disabled");
            }
        });
    }
}




/*
 * delete staff nomination
 */
function DeleteNomination(id) {

    var msg = confirm("Are you sure you want to remove this nominated staff?");

    if (msg === true) {

        $.post("/Applications/DeleteNomination", { "NominationID": id }, function (respons) {
            var result = respons.trim();

            if (result === "Deleted") {

                $("#divNominatedStaff").load(location.href + " #divNominatedStaff");
                alert("Nominated staff removed successfully.");
            }
            else {
                alert(result);
            }
        });
    }
}
