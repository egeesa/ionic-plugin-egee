/*global cordova, module*/

module.exports = {
  hello: function (input, successCallback, errorCallback) {
      cordova.exec(successCallback, errorCallback, "EgeePlugin", "hello", [input]);
  },
  getLicense: function (input, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "EgeePlugin", "getLicense", [input]);
  },
  getTelegram: function (successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "EgeePlugin", "getTelegram", []);
  },
  startBluetoothTelegrams: function (input, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "EgeePlugin", "startBluetoothTelegrams", [input]);
  },
  stopBluetoothTelegrams: function (input, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "EgeePlugin", "stopBluetoothTelegrams", [input]);
  },
  interpreter: function (input, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "EgeePlugin", "interpreter", [input]);
  },
  headInterpreter: function (input, successCallback, errorCallback) {
    cordova.exec(successCallback, errorCallback, "EgeePlugin", "headInterpreter", [input]);
  }
};