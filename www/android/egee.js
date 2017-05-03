var argscheck = require('cordova/argscheck'),
    utils = require('cordova/utils'),
    exec = require('cordova/exec'),
    channel = require('cordova/channel');


var Egee = function () {
};

Egee.totodelasvegas = function (success, error, message) {
    exec(success, error, "Egee", "totodelasvegas", [message]);
};

// channel.onCordovaReady.subscribe(function() {
//     exec(success, null, 'Egee', 'init', []);

//     function success(msg) {
//         var action = msg.charAt(0);
//     }
// });

module.exports = Egee;



