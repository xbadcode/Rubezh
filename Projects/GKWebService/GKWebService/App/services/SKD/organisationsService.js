﻿(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.factory('organisationsService', ['$http', '$q', function ($http, $q) {
        var _getOrganisations = function(filter) {
            var deferred = $q.defer();

            $http.get('Organisations/GetOrganisations', { params: filter }).then(function (response) {
                deferred.resolve(response.data);
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка получения организаций");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _getOrganisationDetails = function(UID) {
            var deferred = $q.defer();

            $http.get('Organisations/GetOrganisationDetails/' + UID).then(function (response) {
                deferred.resolve(response.data);
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка получения организации");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _getOrganisationEmployees = function(UID) {
            var deferred = $q.defer();

            $http.get('Hr/GetOrganisationEmployees/' + UID).then(function (response) {
                deferred.resolve(response.data.Employees);
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка получения сотрудников организации");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _getUsers = function (organisation) {
            var deferred = $q.defer();

            $http.get('Organisations/GetOrganisationUsers', { params: organisation })
                .then(function (response) {
                    deferred.resolve(response.data.Users);
                }, function (response) {
                    // TODO: реализовать обработку ошибок
                    alert("Ошибка получения пользователей организации");
                    deferred.reject();
                });

            return deferred.promise;
        };
    
        var _getDoors = function (organisation) {
            var deferred = $q.defer();

            $http.get('Organisations/GetOrganisationDoors', {
                params: organisation
            }).then(function (response) {
                deferred.resolve(response.data.Doors);
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка получения точек доступа организации");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _isAnyOrganisationItems = function (UID) {
            var deferred = $q.defer();

            $http.get('Organisations/IsAnyOrganisationItems/' + UID).then(function (response) {
                deferred.resolve(response.data);
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка получения привязанных объектов организации");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _setUsersChecked = function (organisation, user) {
            var deferred = $q.defer();

            $http.post('Organisations/SetUsersChecked', {
                organisation: organisation,
                user: user
            }).then(function (response) {
                deferred.resolve(response.data);
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка сохранения пользователя организации");
                deferred.reject();
            });

            return deferred.promise;
        };

        var _setDoorsChecked = function (organisation, door) {
            var deferred = $q.defer();

            $http.post('Organisations/SetDoorsChecked', {
                organisation: organisation,
                door: door
            }).then(function (response) {
                deferred.resolve(response.data);
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка сохранения точки доступа организации");
                deferred.reject();
            });

            return deferred.promise;
        };

        var _saveOrganisation = function (organisation, isNew) {
            var deferred = $q.defer();

            if (organisation.photoData === "//:0") {
                organisation.photoData = null;
            }

            $http.post('Organisations/OrganisationDetails', {
                organisation: organisation,
                isNew: isNew
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка сохранения организации");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _markDeleted = function (UID, name) {
            var deferred = $q.defer();

            $http.post('Organisations/MarkDeleted', {
                uid: UID,
                name: name
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка удаления организации");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        return {
            selectedOrganisation: null,
            reload: null,
            getOrganisations: _getOrganisations,
            getOrganisationDetails: _getOrganisationDetails,
            getOrganisationEmployees: _getOrganisationEmployees,
            getUsers: _getUsers,
            getDoors: _getDoors,
            isAnyOrganisationItems: _isAnyOrganisationItems,
            setUsersChecked: _setUsersChecked,
            setDoorsChecked: _setDoorsChecked,
            saveOrganisation: _saveOrganisation,
            markDeleted: _markDeleted
        }
    }]);
}());
