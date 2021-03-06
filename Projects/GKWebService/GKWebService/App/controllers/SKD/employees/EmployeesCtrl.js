﻿(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('employeesCtrl',
        ['$scope', '$http', '$timeout', '$window', 'employeesService',
         function ($scope, $http, $timeout, $window, employeesService) {
            $scope.gridOptions = {
                onRegisterApi: function(gridApi) {
                    $scope.gridApi = gridApi;
                    gridApi.selection.on.rowSelectionChanged($scope, function(row) {
                        employeesService.selectedEmployee = row.entity;
                        employeesService.selectedCard = null;
                    });
                },                
                enableRowHeaderSelection: false,
                enableSorting: false,
                showTreeExpandNoChildren: false,
                multiSelect: false,
                enableColumnMenus: false,
                enableRowSelection: true,
                noUnselect: true,
                showTreeRowHeader: false,
                columnDefs: [
                    { field: 'Name', width: 210, displayName: 'ФИО', cellTemplate: "<div class='ui-grid-cell-contents'><div style=\"float:left;\" class=\"ui-grid-tree-base-row-header-buttons\" ng-class=\"{'ui-grid-tree-base-header': row.treeLevel > -1 }\" ng-click=\"grid.appScope.toggleRow(row,evt)\"><i ng-class=\"{'ui-grid-icon-minus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'expanded', 'ui-grid-icon-plus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'collapsed', 'ui-grid-icon-blank': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length == 0 ) )}\" ng-style=\"{'padding-left': grid.options.treeIndent * row.treeLevel + 'px'}\"></i> &nbsp;</div>{{ CUSTOM_FILTERS}}<img style='vertical-align: middle; padding-right: 3px' ng-show='row.entity.IsOrganisation' ng-src='/Content/Image/Icon/Hr/Organisation.png'/><img style='vertical-align: middle; padding-right: 3px' ng-show='!row.entity.IsOrganisation' ng-src='/Content/Image/Icon/Hr/Employee.png'/><span ng-style='row.entity.IsDeleted && {opacity:0.5}'>{{row.entity[col.field]}}</span></div>" },
                    { field: 'Model.DepartmentName', width: 210, displayName: 'Подразделение', cellTemplate: "<div class='ui-grid-cell-contents'><span ng-show='!row.entity.Model.IsDepartmentDeleted || grid.appScope.isWithDeleted()' ng-style='(row.entity.IsDeleted || row.entity.Model.IsDepartmentDeleted) && {opacity:0.5}'>{{row.entity['Model']['DepartmentName']}}</span></div>" },
                    { field: 'Model.PositionName', width: 210, displayName: 'Должность', cellTemplate: "<div class='ui-grid-cell-contents'><span ng-show='!row.entity.Model.IsPositionDeleted || grid.appScope.isWithDeleted()' ng-style='(row.entity.IsDeleted || row.entity.Model.IsPositionDeleted) && {opacity:0.5}'>{{row.entity['Model']['PositionName']}}</span></div>" }
                ]
            };

            $scope.gridStyle = function () {
                var ctrlHeight = ($window.innerHeight - 100) / 2;
                return "height:" + ctrlHeight + "px";
            }();

            var reloadTree = function() {
                employeesService.getOrganisations($scope.filter).then(function (employees) {
                        $scope.employees = employees;
                        angular.forEach($scope.employees, function (value, key) {
                            value.$$treeLevel = value.Level;
                        });
                        $scope.gridOptions.data = $scope.employees;
                        employeesService.employees = $scope.employees;
                        employeesService.selectedEmployee = null;
                        employeesService.selectedCard = null;
                        $timeout(function() {
                            $scope.gridApi.treeBase.expandAllRows();
                    });
                });
            }

            employeesService.reload = reloadTree;

            $scope.$watch('filter', function (newValue, oldValue) {
                reloadTree();
            }, true);

            $scope.toggleRow = function (row, evt) {
                $scope.gridApi.treeBase.toggleRowTreeState(row);
            };
             
            $scope.$on('EditOrganisationEvent', function (event, organisation) {
                $scope.updateOrganisation($scope.employees, organisation);
            });

            $scope.$on('AddOrganisationEvent', function (event, organisation) {
                $scope.addOrganisation($scope.gridApi, $scope.employees, organisation);
            });

            $scope.$on('RemoveOrganisationEvent', function (event, organisation) {
                $scope.removeOrganisation($scope.employees, organisation);
            });

            $scope.$on('RestoreOrganisationEvent', function (event, organisation) {
                employeesService.getOrganisations($scope.filter).then(function (employees) {
                    $scope.restoreOrganisation($scope.gridApi, $scope.employees, employees, organisation);
                });
            });

            $scope.$on('EditEmployeeEvent', function (event, UID) {
                employeesService.getSingleShort(UID).then(function (employee) {
                    var oldEmployee;
                    for (var i = 0; i < $scope.employees.length; i++) {
                        if ($scope.employees[i].UID === UID) {
                            oldEmployee = $scope.employees[i];
                            break;
                        }
                    }
                    if (oldEmployee) {
                        oldEmployee.Description = employee.Description;
                        oldEmployee.RemovalDate = employee.RemovalDate;
                        oldEmployee.IsDeleted = employee.IsDeleted;
                        oldEmployee.OrganisationUID = employee.OrganisationUID;
                        oldEmployee.Model = employee;
                        oldEmployee.Name = employee.LastName + " " + employee.FirstName + " " + employee.SecondName;
                    }
                });
            });

            $scope.$on('AddEmployeeEvent', function (event, UID) {
                employeesService.getSingleShort(UID).then(function (employee) {
                    var newEmployee = {
                        UID: employee.UID,
                        OrganisationUID: employee.OrganisationUID,
                        ParentUID: employee.OrganisationUID,
                        Description: employee.Description,
                        RemovalDate: employee.RemovalDate,
                        IsDeleted: employee.IsDeleted,
                        OrganisationUID: employee.OrganisationUID,
                        Model: employee,
                        Name: employee.LastName + " " + employee.FirstName + " " + employee.SecondName,
                        $$treeLevel: 1
                    };

                    for (var i = 0; i < $scope.employees.length; i++) {
                        if ($scope.employees[i].UID === employee.OrganisationUID) {
                            $scope.employees.splice(i + 1, 0, newEmployee);
                            $timeout(function () {
                                $scope.gridApi.selection.selectRow($scope.gridApi.grid.getRow(newEmployee));
                            });
                            break;
                        }
                    }

                });
            });

         }]
    );

}());
