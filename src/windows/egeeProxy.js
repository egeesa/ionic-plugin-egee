/**
 * @author e-GEE SA
 */

// function Egee () {
//     this._promise = null;
//     this._cancelled = false;
// }


// var egeeProxy = {

//     helloworld: function (success, error, message) {
//         alert('test windows');
//         //Créer une instance de la classe CS
//         var classe = new Egee.Egee();
//         alert('test windows 2');

//         return classe.HelloWorld(message);
//     }
// };
//

module.exports = {
    helloworld: function (success, error, message) {
        try {
            if (!message || !message.length) {
                error("Error, something was wrong with the input string. =>" + message);
            }
            else {
                success(message + "echo");
            }
        } catch (e) {
            error("Erreur helloworld");
        }

        // alert('test windows');
        // //Créer une instance de la classe CS
        // var classe = new Egee.Egee();
        // alert('test windows 2');

        // return classe.HelloWorld(message);

        //  +            var file = args[0];
        //  +            var dir = args[1];
        //  +
        //  +            var plugin = ZipCSComponent.ZipPlugin();
        //  +
        //  +            plugin.unZipByUriAsync(file, dir, function (loaded, total) {
        //  +            //plugin.unZipByPathAsync(file, dir, function (loaded, total) {
        //  +                var progressEvent = new ProgressEvent("OK", { loaded: loaded, total: total })
        //  +                var callbackParams = { keepCallback: true };
        //  +                successCallback(progressEvent, callbackParams);
        //  +            })
        //  +            .done(
        //  +                function () {
        //  +                    successCallback();
        //  +                },
        //  +                function (error) {
        //  +                    errorCallback(error);
        //  +                }
        //  +            );

    }
};

require("cordova/exec/proxy").add("Egee", module.exports);