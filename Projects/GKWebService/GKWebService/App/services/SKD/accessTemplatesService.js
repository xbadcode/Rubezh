﻿
(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.factory('accessTemplatesService', ['$http', '$q', function ($http, $q) {
        var _getAccessTemplates = function (filter) {
            var deferred = $q.defer();

            $http.get('AccessTemplates/GetOrganisations', { params: filter }).then(function (response) {
                angular.forEach(response.data.rows, function (value, key) {
                    value.$$treeLevel = value.Level;
                });
                deferred.resolve(response.data.rows);
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка получения шаблонов доступа");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _getDoors = function (UID) {
            var deferred = $q.defer();

            $http.get('AccessTemplates/GetDoors/' + UID).then(function (response) {
                deferred.resolve(response.data);
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка получения точек доступа");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _getAccessTemplateDetails = function (organisationUID, UID) {
            var deferred = $q.defer();

            $http.get('AccessTemplates/GetAccessTemplateDetails', {
                params: {
                organisationId: organisationUID,
                id: UID
            }
            }).then(function (response) {
                deferred.resolve(response.data);
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка получения шаблона доступа");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _getLinkedCards = function (organisationUID, UID) {
            var deferred = $q.defer();

            $http.get('AccessTemplates/GetLinkedCards', {
                params: {
                organisationId: organisationUID,
                id: UID
            }
            }).then(function (response) {
                deferred.resolve(response.data);
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка получения шаблона доступа");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _saveAccessTemplate = function (accessTemplate, isNew) {
            var deferred = $q.defer();

            $http.post('AccessTemplates/AccessTemplateDetails', {
                accessTemplate: accessTemplate,
                isNew: isNew
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка сохранения шаблона доступа");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _markDeleted = function (UID, name) {
            var deferred = $q.defer();

            $http.post('AccessTemplates/MarkDeleted', {
                uid: UID,
                name: name
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка удаления шаблона доступа");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _paste = function (organisationUID, accessTemplateModel) {
            var deferred = $q.defer();

            accessTemplateModel.AccessTemplate.OrganisationUID = organisationUID;

            $http.post('AccessTemplates/AccessTemplatePaste', {
                accessTemplate: accessTemplateModel
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка вставки шаблона доступа");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        return {
            selectedAccessTemplate: null,
            reload: null,
            getAccessTemplates: _getAccessTemplates,
            getDoors: _getDoors,
            saveAccessTemplate: _saveAccessTemplate,
            getAccessTemplateDetails: _getAccessTemplateDetails,
            markDeleted: _markDeleted,
            getLinkedCards: _getLinkedCards,
            paste: _paste
        }
    }]);
}());
