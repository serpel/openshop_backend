function CreateTable(columns, options) {
    var table = $(options.id).DataTable({
        info: true,
        print: true,
        //autoWidth: true,
        responsive: true,
        stateSave: true,
        ajax: {
            url: options.url,
            dataSrc: '',
            stateSave: true
        },
        columns: columns,
        columnDefs: [{
            render: function (data, type, row) {

                var editUrl = options.editUrl + '/' + data;
                var deleteUrl = options.deleteUrl + '/' + data;

                var buttons = "<div class='pull-right'>" +
                    "<a class='btn btn-default' data-modal='' href='" + editUrl + "' title='Edit'><span class='glyphicon glyphicon-pencil'></span></a>&nbsp;" +
                    "<a class='btn btn-danger' data-modal='' href='" + deleteUrl + "' title='Delete'><span class='glyphicon glyphicon-trash'></span></a>" +
                    "</div>";

                return buttons;

                return options;
            },
            targets: -1,
            orderable: false,
            searchable: false
        }],
        order: [0, "desc"]
    });

    return table;
};

function ReloadTable(columns, options) {
    //table.ajax.reload();
    d = $(options.id).DataTable().destroy();
    d = CreateTable(columns, options);
};

function LoadModal(columns, options) {
    $("a[data-modal]").on("click", function (e) {
        $('#myModalContent').load(this.href, function () {
            $('#myModal').modal({
                keyboard: true
            }, 'show');
            bindForm(this, columns, options);
        });
        return false;
    });
};

function bindForm(dialog, columns, options) {
    $('form', dialog).submit(function (e) {
        $.ajax({
            url: this.action,
            type: this.method,
            data: new FormData( this ),
            processData: false,
            contentType: false,
            success: function (result) {
                if (result.success) {
                    $('#myModal').modal('hide');
                    $(options.id).DataTable().ajax.reload();
                    //ReloadTable(id, columns, source, controller);
                } else {

                    if (result.message != null) {
                        $('#myModal').modal('hide');
                        Messenger().post({
                            message: result.message,
                            type: 'error',
                            showCloseButton: true
                        });
                    } else {
                        $('#myModalContent').html(result);
                    }
                    //bindForm(dialog, id, columns, source, controller);
                }
            }
        });
        e.preventDefault();
        //return false;
    });
};