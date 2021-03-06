﻿(function () {
    'use strict';

    angular.module("gkApp").config(function ($stateProvider, $urlRouterProvider) {

        $urlRouterProvider.otherwise("/");

        $stateProvider.state('login', {
            url: "/login",
            onEnter: ['$uibModal', function ($uibModal) {
                $uibModal.open({
                    templateUrl: '/Home/Login',
                    controller: 'loginCtrl',
                    backdrop: 'static',
                    size: 'rbzh',
                    keyboard: false,
                    resolve: {
                        options: function () {
                            return {
                                validateOnlyPassword: false
                            }
                        }
                    }
                });
            }]
        }).state('home', {
            url: ''
        }).state('state', {
            url: "/state/:alarmType",
            templateUrl: "/Alarms/Alarms"
        }).state('device', {
            url: "/devices/:uid",
            templateUrl: "/Devices/Index"
        }).state('params', {
            url: "/params/:uid",
            templateUrl: "/DeviceParameters/Index"
        }).state('fireZones', {
            url: "/fireZones/:uid",
            templateUrl: "/FireZones/Index"
        }).state('pumpStations', {
            url: "/pumpStations/:uid",
            templateUrl: "/PumpStations/PumpStations"
        }).state('directions', {
            url: "/directions/:uid",
            templateUrl: "/Directions/Index"
        }).state('delays', {
            url: "/delays/:uid",
            templateUrl: "/Delays/Delays"
        }).state('MPTs', {
            url: "/MPTs/:uid",
            templateUrl: "/MPTs/Index"
        }).state('journal', {
            url: "/journal/:uid",
            templateUrl: "/Journal/Index"
        }).state('archive', {
            url: "/archive/:uid",
            templateUrl: "/Archive/Index"
        }).state('plan', {
            url: "/plan/:uid",
            templateUrl: "/Plans/Index"
        }).state('guardZone', {
            url: "/guardZone/:uid",
            templateUrl: "/GuardZones/Guard"
        }).state('doors', {
            url: "/doors/:uid",
            templateUrl: "/Doors/Door"
        }).state('hr', {
            url: "/hr",
            templateUrl: "/Hr/Index",
            controller: "HRCtrl",
            resolve: {
                authData: function(authService) {
                    return authService.fillAuthData();
                }
            }
        });
    });

}());
