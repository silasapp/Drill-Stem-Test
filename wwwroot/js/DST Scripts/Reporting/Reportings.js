$(document).ready(function () {

    $("#txtApplicationReportFrom").datetimepicker({
        defaultDate: new Date().setDate(new Date().getDate() + 0),
        timepicker: false,
        yearEnd: new Date().getFullYear() + 1,
        maxDate: new Date((new Date().getFullYear() + 0) + '/12/31')

    });


    $("#txtApplicationReportTo").datetimepicker({
        defaultDate: new Date().setDate(new Date().getDate() + 0),
        
        timepicker: false,
        yearEnd: new Date().getFullYear() + 1,
    });



    $("#btnApplicationReportSearch").on('click', function (event) {
        event.preventDefault();

        var type = StringToArray($("#txtSelType"));
        var stage = StringToArray($("#txtSelStage"));
        var status = StringToArray($("#txtSelStatus"));
        var dateFrom = $("#txtApplicationReportFrom").val();
        var dateTo = $("#txtApplicationReportTo").val();

        var table = $("#SearchTabless").DataTable({

            ajax: {
                url: "/Reports/ApplicationReport",
                type: "POST",
                data: function (d) {
                        d.type = type,
                        d.stage = stage,
                        d.status = status,
                        d.dateFrom = dateFrom,
                        d.dateTo = dateTo
                },
                dataType: "json",
            },
            lengthMenu: [[5000, 15000, 25000, 50000], [5000, 15000, 25000, 50000]],

            //scrollY: 1000,
            //scrollX: true,
            //scrollCollapse: true,
            //fixedColumns: true,

            dom: 'Bfrtip',
            buttons: [
                'pageLength',
                'copyHtml5',
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

            "columns": [

                { data: "refNo" },
                { data: "companyName" },
                { data: "facilities" },
                { data: "type" },
                { data: "stage" },
                { data: "status" },
                { data: "currentDesk", "orderable": false, "searchable": false},
                { data: "state", "orderable": false, "searchable": false},
                { data: "lga", "orderable": false, "searchable": false },
                { data: "dateApplied" },
                { data: "dateSubmitted"},
                { data: "wellNames"},
                { data: "reserviorNames"},
                { data: "fieldNames"},
            ],

            rowGroup: {
                
                startRender: function (rows, group) {
                    return $('<tr>').append('<td colspan="17" class="text-left" style="background-color:#f9e093b8; color:black"> <b>' + group + '</b></td></tr>');
                },

                dataSrc: 'type'
            },
            orderFixed: [2, 'desc'],
            "bDestroy": true

        });

    });



    $("#btnTransactionReportSearch").on('click', function (event) {
        event.preventDefault();

        var type = StringToArray($("#txtSelType"));
        var stage = StringToArray($("#txtSelStage"));
        var status = StringToArray($("#txtSelStatus"));
        var dateFrom = $("#txtApplicationReportFrom").val();
        var dateTo = $("#txtApplicationReportTo").val();

        var table = $("#SearchTransactionTabless").DataTable({

            ajax: {
                url: "/Reports/TransactionReport",
                type: "POST",
                data: function (d) {
                        d.type = type,
                        d.stage = stage,
                        d.status = status,
                        d.dateFrom = dateFrom,
                        d.dateTo = dateTo
                },
                dataType: "json",
            },
            lengthMenu: [[5000, 15000, 25000, 50000], [5000, 15000, 25000, 50000]],

            dom: 'Bfrtip',
            buttons: [
                'pageLength',
                'copyHtml5',
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

            'columnDefs': [{
                "targets": [7, 8, 9],
                "data" : "type",
                "render": function (data, type, row, meta) {

                    if (row['type'] === "EXTENDED WELL TEST (EWT)") {
                        return "$" + formatNumber(Number(data).toFixed(2));
                    }
                    else {
                        return "₦" + formatNumber(Number(data).toFixed(2));
                    }
                }
            }],

            "columns": [

                { data: "refNo" },
                { data: "rrr" },
                { data: "companyName" },
                { data: "facilities" },
                { data: "type" },
                { data: "stage" },
                { data: "status" },
                { data: "amount", "orderable": false, "searchable": false },
                { data: "servicCharge", "orderable": false, "searchable": false },
                { data: "totalAmount", "orderable": false, "searchable": false },
                { data: "transDate" },
            ],

            //"footerCallback": function (row, data, start, end, display) {
            //    var api = this.api();
            //    nb_cols = api.columns().nodes().length - 1;

            //    var j = 7;
            //    while (j < nb_cols) {
            //        var pageTotal = api
            //            .column(j, { page: 'current' })
            //            .data()
            //            .reduce(function (a, b) {
            //                return (Number(a) + Number(b));
            //            }, 0);
            //        // Update footer

            //        $(api.column(j).footer()).html('₦' + formatNumber(pageTotal.toFixed(2)));
            //        j++;
            //    }
            //},

            rowGroup: {
                endRender: function (rows, group) {

                    var a1 = GetSum(rows, 'amount');
                    var a2 = GetSum(rows, 'servicCharge');
                    var aSum = GetSum(rows, 'totalAmount');

                    if (group === "EXTENDED WELL TEST (EWT)") {

                        return $('<tr>')
                            .append('<td colspan="7" style="background-color:; color:red" class=""><b>Sub Total for ' + group + ' as sorted</b></td>')
                            .append('<td class="text-danger"><b>$' + formatNumber(a1.toFixed(2)) + '</b></td>')
                            .append('<td class="text-danger"><b>$' + formatNumber(a2.toFixed(2)) + '</b></td>')
                            .append('<td class="text-danger"><b>$' + formatNumber(aSum.toFixed(2)) + '</b></td>')
                            .append('<td></td>')
                            .append('</tr>');
                    }
                    else {

                        return $('<tr>')
                            .append('<td colspan="7" style="background-color:; color:red" class=""><b>Sub Total for ' + group + ' as sorted</b></td>')
                            .append('<td class="text-danger"><b>₦' + formatNumber(a1.toFixed(2)) + '</b></td>')
                            .append('<td class="text-danger"><b>₦' + formatNumber(a2.toFixed(2)) + '</b></td>')
                            .append('<td class="text-danger"><b>₦' + formatNumber(aSum.toFixed(2)) + '</b></td>')
                            .append('<td></td>')
                            .append('</tr>');
                    }
      
                    

                },
                startRender: function (rows, group) {
                    return $('<tr>').append('<td colspan="15" class="text-left" style="background-color:#f9e093b8; color:black"> <b>' + group + '</b></td></tr>');
                },

                dataSrc: 'type'
            },
            orderFixed: [2, 'desc'],
            "bDestroy": true

        });

    });




    $("#btnPermitReportSearch").on('click', function (event) {
        event.preventDefault();

        var type = StringToArray($("#txtSelType"));
        var stage = StringToArray($("#txtSelStage"));
        var dateFrom = $("#txtApplicationReportFrom").val();
        var dateTo = $("#txtApplicationReportTo").val();

        var table = $("#SearchPermitTabless").DataTable({

            ajax: {
                url: "/Reports/PermitReports",
                type: "POST",
                data: function (d) {
                        d.type = type,
                        d.stage = stage,
                        d.dateFrom = dateFrom,
                        d.dateTo = dateTo
                },
                dataType: "json",
            },
            lengthMenu: [[5000, 15000, 25000, 50000], [5000, 15000, 25000, 50000]],

            //scrollY: 1000,
            //scrollX: true,
            //scrollCollapse: true,
            //fixedColumns: true,

            dom: 'Bfrtip',
            buttons: [
                'pageLength',
                'copyHtml5',
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

            "columns": [

                { data: "permitNo" },
                { data: "companyName" },
                { data: "facilities" },
                { data: "type" },
                { data: "stage" },
                { data: "issuedDate", "searchable": false },
                { data: "expiryDate", "searchable": false },
                { data: "approvedBy", "orderable": false, "searchable": false },
            ],

            rowGroup: {
                startRender: function (rows, group) {
                    return $('<tr>').append('<td colspan="25" class="text-left" style="background-color:#f9e093b8; color:black"> <b>' + group + '</b></td></tr>');
                },

                dataSrc: 'type'
            },
            orderFixed: [2, 'desc'],
            "bDestroy": true

        });

    });





    function GetSum(rows, pluck) {
        var get = rows
            .data()
            .pluck('' + pluck + '')
            .reduce(function (a, b) {
                return a + b;
            });
        return get;
    }


    function GetTotal(column) {
        var total = api
            .column('' + column + '')
            .data()
            .reduce(function (a, b) {
                return intVal(a) + intVal(b);
            }, 0);
        return total;
    }


    function StringToArray(select) {
        var array = [];
        select.each(function () {
            array.push($(this).val());
        });

        return array;
    }


    function formatNumber(num) {
        return num.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
    }

});