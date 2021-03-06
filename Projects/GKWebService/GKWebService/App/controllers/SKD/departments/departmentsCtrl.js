﻿(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('departmentsCtrl',
        ['$scope', '$timeout', '$window', 'departmentsService', 
         function ($scope, $timeout, $window, departmentsService) {
            $scope.gridOptions = {
                onRegisterApi: function(gridApi) {
                    $scope.gridApi = gridApi;
                    gridApi.selection.on.rowSelectionChanged($scope, function(row) {
                        departmentsService.selectedDepartment = row.entity;
                        departmentsService.selectedEmployee = null;
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
                    { field: 'Name', width: 210, displayName: 'Название', cellTemplate: "<div class='ui-grid-cell-contents'><div style=\"float:left;\" class=\"ui-grid-tree-base-row-header-buttons\" ng-class=\"{'ui-grid-tree-base-header': row.treeLevel > -1 }\" ng-click=\"grid.appScope.toggleRow(row,evt)\"><i ng-class=\"{'ui-grid-icon-minus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'expanded', 'ui-grid-icon-plus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'collapsed', 'ui-grid-icon-blank': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length == 0 ) )}\" ng-style=\"{'padding-left': grid.options.treeIndent * row.treeLevel + 'px'}\"></i> &nbsp;</div>{{ CUSTOM_FILTERS}}<img style='vertical-align: middle; padding-right: 3px' ng-show='row.entity.IsOrganisation' ng-src='/Content/Image/Icon/Hr/Organisation.png'/><img style='vertical-align: middle; padding-right: 3px' ng-show='!row.entity.IsOrganisation' ng-src='/Content/Image/Icon/Hr/Department.png'/><span ng-style='row.entity.IsDeleted && {opacity:0.5}'>{{row.entity[col.field]}}</span></div>" },
                    { field: 'Description', width: 210, displayName: 'Примечание', cellTemplate: "<div class='ui-grid-cell-contents'><span ng-style='row.entity.IsDeleted && {opacity:0.5}'>{{row.entity[col.field]}}</span></div>" },
                    { field: 'Model.Phone', width: 210, displayName: 'Телефон', cellTemplate: "<div class='ui-grid-cell-contents'><span ng-style='row.entity.IsDeleted && {opacity:0.5}'>{{row.entity['Model']['Phone']}}</span></div>" }
                ]
            };

            $scope.gridStyle = function () {
                var ctrlHeight = ($window.innerHeight - 100) / 2;
                return "height:" + ctrlHeight + "px";
            }();

            var reloadTree = function () {
                departmentsService.getDepartments($scope.filter)
                    .then(function (departments) {
                        $scope.departments = departments;
                        $scope.gridOptions.data = $scope.departments;
                        departmentsService.selectedDepartment = null;
                        departmentsService.selectedEmployee = null;
                        departmentsService.departments = $scope.departments;
                        $timeout(function() {
                            $scope.gridApi.treeBase.expandAllRows();
                    });
                });
            }

            departmentsService.reload = reloadTree;

            $scope.$watch('filter', function (newValue, oldValue) {
                reloadTree();
            }, true);

            $scope.toggleRow = function (row, evt) {
                $scope.gridApi.treeBase.toggleRowTreeState(row);
            };

            $scope.$on('EditOrganisationEvent', function (event, organisation) {
                $scope.updateOrganisation($scope.departments, organisation);
            });

            $scope.$on('AddOrganisationEvent', function (event, organisation) {
                $scope.addOrganisation($scope.gridApi, $scope.departments, organisation);
            });

            $scope.$on('RemoveOrganisationEvent', function (event, organisation) {
                $scope.removeOrganisation($scope.departments, organisation);
            });

            var addDepartment = function (department) {
                var parentUID = department.ParentDepartmentUID ? department.ParentDepartmentUID : departmentsService.selectedDepartment.OrganisationUID;

                for (var i = 0; i < $scope.departments.length; i++) {
                    if ($scope.departments[i].UID === parentUID) {
                        var newDepartment = {
                            UID: department.UID,
                            ParentUID: parentUID,
                            ParentDepartmentUID: department.ParentDepartmentUID,
                            OrganisationUID: $scope.departments[i].OrganisationUID,
                            Name: department.Name,
                            Description: department.Description,
                            Model: { ChiefUID: department.ChiefUID },
                            $$treeLevel: $scope.departments[i].$$treeLevel + 1
                        };
                        $scope.departments.splice(i + 1, 0, newDepartment);
                        $timeout(function () {
                            $scope.gridApi.selection.selectRow(newDepartment);
                            $scope.gridApi.core.scrollTo(newDepartment, $scope.gridOptions.columnDefs[0]);
                        })
                        break;
                    }
                }
            };

            $scope.$on('EditDepartmentEvent', function (event, department) {
                for (var i = 0; i < $scope.departments.length; i++) {
                    if ($scope.departments[i].UID === department.UID) {
                        if ($scope.departments[i].ParentDepartmentUID !== department.ParentDepartmentUID) {
                            $scope.departments.splice(i, 1);
                            addDepartment(department);
                        }
                        break;
                    }
                }
            });

            $scope.$on('AddDepartmentEvent', function (event, department) {
                addDepartment(department);
            });
         }]
    );

}());
