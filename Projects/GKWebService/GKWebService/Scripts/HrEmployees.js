﻿$(document).ready(function() {

    $("#jqGridEmployees").jqGrid({
        url: '/Employees/GetOrganisations',
        datatype: "json",
        colModel: [
            { label: 'UID', name: 'UID', key: true, hidden: true, sortable: false },
            { label: 'OrganisationUID', name: 'OrganisationUID', hidden: true, sortable: false },
            { label: 'ФИО', name: 'Name', width: 100, sortable: false },
            { label: 'Подразделение', name: 'DepartmentName', width: 100, sortable: false }
        ],
        width: jQuery(window).width() - 242,
        height: 300,
        rowNum: 100,
        viewrecords: true,

        treeGrid: true,
        ExpandColumn: "Name",
        treedatatype: "json",
        treeGridModel: "adjacency",
        loadonce: true,
        treeReader: {
            parent_id_field: "OrganisationUID",
            level_field: "Level",
            leaf_field: "IsLeaf",
            expanded_field: "IsExpanded"
        }
    });
});

function EmployeesViewModel() {
    var self = {};

    self.UID = ko.observable();
    self.Name = ko.observable();
    self.DepartmentName = ko.observable();

    $('#jqGridEmployees').on('jqGridSelectRow', function (event, id, selected) {

        var myGrid = $('#jqGrid');

        self.UID(id);
        self.Name(myGrid.jqGrid('getCell', id, 'Name'));
        self.DepartmentName(myGrid.jqGrid('getCell', id, 'DepartmentName'));
    });

    self.ShowEmployee = function (box) {
        //Fade in the Popup
        $(box).fadeIn(300);

        //Set the center alignment padding + border see css style
        var popMargTop = ($(box).height() + 24) / 2;
        var popMargLeft = ($(box).width() + 24) / 2;

        $(box).css({
            'margin-top': -popMargTop,
            'margin-left': -popMargLeft
        });

        // Add the mask to body
        $('body').append('<div id="mask"></div>');
        $('#mask').fadeIn(300);

        return false;
    }

    self.AddEmployeeClick = function (data, e, box) {
        self.ShowEmployee(box);
    }

    self.EditEmployeeClick = function (data, e, box) {

        self.ShowEmployee(box);
    }

    return self;
}