var argscheck = require('cordova/argscheck'),
    utils = require('cordova/utils'),
    exec = require('cordova/exec'),
    channel = require('cordova/channel');


function Egee() {
};

Egee.prototype.helloworld = function (success, error, message) {
    exec(success, error, "Egee", "helloworld", [message]);
};

// channel.onCordovaReady.subscribe(function() {
//     exec(success, null, 'Egee', 'init', []);

//     function success(msg) {
//         var action = msg.charAt(0);
//     }
// });

module.exports = new Egee();



