var argscheck = require('cordova/argscheck'),
    utils = require('cordova/utils'),
    exec = require('cordova/exec'),
    channel = require('cordova/channel');


function EgeePlugin() {};

EgeePlugin.prototype.helloworld = function (success, error, message) {
    exec(success, error, "EgeePlugin", "helloworld", [message]);
};

EgeePlugin.prototype.getVersion = function (success, error, message) {
    exec(success, error, "EgeePlugin", "getVersion", [message]);
};

EgeePlugin.prototype.sappelLicense = function (success, error, message) {
    exec(success, error, "EgeePlugin", "sappelLicense", [message]);
};

EgeePlugin.prototype.sappelTraitement = function (success, error, frame) {
    exec(success, error, "EgeePlugin", "sappelTraitement", [frame]);
};

EgeePlugin.prototype.sappelBluetooth = function (success, error, adresseMAC) {
    exec(success, error, "EgeePlugin", "sappelBluetooth", [adresseMAC]);
};



module.exports = new EgeePlugin();