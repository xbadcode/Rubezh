﻿function EmployeeCardDetailsViewModel(parentViewModel) {
    var self = this;

    self.Title = ko.observable();
    self.ParentViewModel = parentViewModel;
    self.CanChangeCardType = ko.observable();
    self.SelectedGKCardType = ko.observable();
    self.CanSelectGKControllers = ko.computed(function () {
        return self.SelectedGKCardType() != "Employee";
    }, self);

    self.employeeCardDetailsPages = {
        General: ko.observable(true),
        GKControllers: ko.observable(false),
        AccessDoors: ko.observable(false),
        AccessTemplates: ko.observable(false),
    };

    $.ajax({
        dataType: "json",
        url: "Employees/GetEmployeeCardDetails",
        async: false,
        data: null,
        success: function (data) {
            ko.mapping.fromJS(data, {}, self);
        }
    });

    self.InitEmployeeCardDetails = function (isNew) {
        self.IsNew = isNew;
        if (isNew) {
            self.Title("Создание пропуска");
        } else {
            self.Title(("Свойства пропуска: ") + self.Number());
        }
        self.CanChangeCardType(!self.ParentViewModel.IsGuest());
    };

    self.EmployeeCardDetailsPageClick = function (data, e, page) {
        for (var propertyName in self.employeeCardDetailsPages) {
            self.employeeCardDetailsPages[propertyName](false);
        }

        self.employeeCardDetailsPages[page](!self.employeeCardDetailsPages[page]());
        $('div#employee-card-details-box li').removeClass("active");
        $(e.currentTarget).parent().addClass("active");
    };

    self.EmployeeCardDetailsClose = function () {
        $('#mask , .save-cancel-popup').fadeOut(300, function () {
            $('#mask').remove();
        });

        return false;
    };

    self.SaveEmployeeCard = function () {
        var data = ko.mapping.toJSON(self);
        $.ajax({
            url: "Employees/EmployeeCardDetails",
            type: "post",
            contentType: "application/json",
            data: "{'card':" + data + ",'employeeName': '" + self.ParentViewModel.Name() + "','isNew': '" + self.IsNew + "'}",
            success: function () {
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert("request failed");
            },
        });
    };

    return self;
}
