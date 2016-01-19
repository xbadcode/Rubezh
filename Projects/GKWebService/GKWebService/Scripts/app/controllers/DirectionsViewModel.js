﻿$(document).ready(function () {
    $("#jqGridDirections").jqGrid({
        url: '/Home/GetDirections',
        datatype: "json",
        colModel: [
            { label: 'No', name: 'No', key: true, hidden: false, sortable: false },
            { label: 'Name', name: 'Name', hidden: false, sortable: false }
        ],
        width: jQuery(window).width() - 242,
        height: 250,
        rowNum: 100,
        viewrecords: true
    });

});

function DirectionsViewModel() {
    var self = {};

    self.No = ko.observable();
    self.Name = ko.observable();

    $('#jqGridDirections').on('jqGridSelectRow', function (event, id, selected) {

        var myGrid = $('#jqgrid');

        self.No(myGrid.jqGrid('getCell', id, 'No'));
        self.Name(myGrid.jqGrid('getCell', id, 'Name'))

    });

    return self;
}