﻿(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('departmentsToolbarCtrl',
        ['$scope', '$uibModal', 'departmentsService', 'dialogService', 'authService',
         function ($scope, $uibModal, departmentsService, dialogService, authService) {
             $scope.$watch(function() {
                 return departmentsService.selectedDepartment;
             }, function (department) {
                 $scope.selectedDepartment = department;
             });

             $scope.isDepartmentsEditAllowed = authService.checkPermission('Oper_SKD_Departments_Etit');

             $scope.canAdd = function () {
                 return $scope.selectedDepartment && !$scope.selectedDepartment.IsDeleted && $scope.isDepartmentsEditAllowed;
             };
             $scope.canRemove = function () {
                 return $scope.selectedDepartment && !$scope.selectedDepartment.IsDeleted && !$scope.selectedDepartment.IsOrganisation && $scope.isDepartmentsEditAllowed;
             };
             $scope.canEdit = function () {
                 return $scope.selectedDepartment && !$scope.selectedDepartment.IsDeleted && !$scope.selectedDepartment.IsOrganisation && $scope.isDepartmentsEditAllowed;
             };
             $scope.canCopy = function () {
                 return $scope.selectedDepartment && !$scope.selectedDepartment.IsDeleted && !$scope.selectedDepartment.IsOrganisation && $scope.isDepartmentsEditAllowed;
             };
             $scope.canPaste = function () {
                 return $scope.selectedDepartment && !$scope.selectedDepartment.IsDeleted && $scope.clipboard && $scope.isDepartmentsEditAllowed;
             };
             $scope.canRestore = function () {
                 return $scope.selectedDepartment && $scope.selectedDepartment.IsDeleted && !$scope.selectedDepartment.IsOrganisation && $scope.isDepartmentsEditAllowed;
             };

             var showDepartmentDetails = function(UID, isNew, parentUID) {
                 var modalInstance = $uibModal.open({
                     animation: false,
                     templateUrl: 'Departments/DepartmentDetails',
                     controller: 'departmentDetailsCtrl',
                     backdrop: 'static',
                     resolve: {
                         department: function () {
                             return departmentsService.getDepartmentDetails($scope.selectedDepartment.OrganisationUID, UID, parentUID);
                         },
                         isNew: function () {
                             return isNew;
                         },
                         departments: function() {
                             return departmentsService.departments;
                         }
                     }
                 });

                 modalInstance.result.then(function (department) {
                     if (isNew) {
                         $scope.$parent.$broadcast('AddDepartmentEvent', department);
                     } else {
                         $scope.$parent.$broadcast('EditDepartmentEvent', department);
                     }
                 });
             };

             $scope.edit = function () {
                 showDepartmentDetails($scope.selectedDepartment.UID, false, $scope.selectedDepartment.ParentUID);
             };

             $scope.add = function () {
                 showDepartmentDetails('', true, $scope.selectedDepartment.UID);
             };

             $scope.remove = function () {
                 if (dialogService.showConfirm("Вы уверены, что хотите архивировать подразделение?")) {
                     departmentsService.getChildEmployeeUIDs($scope.selectedDepartment.UID).then(function (departmentUIDs) {
                        if (departmentUIDs.length > 0) {
                            if (dialogService.showConfirm("Существуют привязанные к подразделению сотрудники. Продолжить?")) {
                                departmentsService.markDeleted($scope.selectedDepartment.UID).then(function() {
                                    departmentsService.reload();
                                });
                            }
                        } else {
                            departmentsService.markDeleted($scope.selectedDepartment.UID).then(function () {
                                departmentsService.reload();
                            });
                        }
                    });
                 }
             };

             $scope.restore = function () {
                 $scope.restoreElement("подразделение", departmentsService.departments, $scope.selectedDepartment).then(function (){
                     departmentsService.restore($scope.selectedDepartment).then(function () {
                         departmentsService.reload();
                     });
                 });
             };

             $scope.copy = function () {
                 var departmentUIDs = [];
                 departmentUIDs.push($scope.selectedDepartment.UID);
                 for (var i = 0; i < departmentsService.departments.length; i++) {
                     var department = departmentsService.departments[i];
                     if (departmentUIDs.indexOf(department.ParentUID) !== -1) {
                         departmentUIDs.push(department.UID);
                     }
                 }

                 departmentsService.getDepartmentDetailsMany($scope.selectedDepartment.OrganisationUID, departmentUIDs)
                     .then(function (departments) {
                         $scope.clipboard = departments;
                     });
             };

             $scope.paste = function () {
                 var parentUID;
                 if ($scope.selectedDepartment.IsOrganisation) {
                     parentUID = null;
                 } else {
                     parentUID = $scope.selectedDepartment.UID;
                 }
                 departmentsService.paste($scope.selectedDepartment.OrganisationUID, parentUID, $scope.clipboard).then(function () {
                         departmentsService.reload();
                     });
             };
         }]
    );

}());
