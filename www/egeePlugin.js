var argscheck = require('cordova/argscheck'),
    utils = require('cordova/utils'),
    exec = require('cordova/exec'),
    channel = require('cordova/channel');


function EgeePlugin() {
};

EgeePlugin.prototype.helloworld = function (success, error, message) {
    exec(success, error, "EgeePlugin", "helloworld", [message]);
};



module.exports = new EgeePlugin();



