var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/notebooks/getall/",
            "type": "GET",
            "datatype": "json"
        },

        "columns": [
            { "data": "name", "width": "20%" },
            { "data": "surname", "width": "20%" },
            { "data": "phoneNumber", "width": "20%" },
            { "data": "city", "width": "20%" },
            { "data": "age", "width": "20%" },
        ],
        "language": {
            "emptyTable": "no data found"
        },
        "width": "100%"
    });
}
